﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.Extensions.Time.Testing;

/// <summary>
/// A synthetic clock used to provide deterministic behavior in tests.
/// </summary>
public class FakeTimeProvider : TimeProvider
{
    internal static readonly DateTimeOffset Epoch = new(2000, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

    internal readonly List<FakeTimeProviderTimer.Waiter> Waiters = new();

    private DateTimeOffset _now = Epoch;
    private TimeZoneInfo _localTimeZone;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class.
    /// </summary>
    /// <remarks>
    /// This creates a clock whose time is set to midnight January 1st 2000.
    /// The clock is set to not automatically advance every time it is read.
    /// </remarks>
    public FakeTimeProvider()
    {
        _localTimeZone = TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class.
    /// </summary>
    /// <param name="startTime">The initial time reported by the clock.</param>
    public FakeTimeProvider(DateTimeOffset startTime)
        : this()
    {
        _now = startTime;
    }

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow()
    {
        return _now;
    }

    /// <summary>
    /// Sets the date and time in the UTC timezone.
    /// </summary>
    /// <param name="value">The date and time in the UTC timezone.</param>
    public void SetUtcNow(DateTimeOffset value)
    {
        List<FakeTimeProviderTimer.Waiter> waiters;
        lock (Waiters)
        {
            _now = value;
            waiters = GetWaitersToWake();
        }

        WakeWaiters(waiters);
    }

    /// <summary>
    /// Advances the clock's time by a specific amount.
    /// </summary>
    /// <param name="delta">The amount of time to advance the clock by.</param>
    public void Advance(TimeSpan delta)
    {
        List<FakeTimeProviderTimer.Waiter> waiters;
        lock (Waiters)
        {
            _now += delta;
            waiters = GetWaitersToWake();
        }

        WakeWaiters(waiters);
    }

    /// <summary>
    /// Advances the clock's time by one millisecond.
    /// </summary>
    public void Advance() => Advance(TimeSpan.FromMilliseconds(1));

    /// <inheritdoc />
    public override long GetTimestamp()
    {
        // Notionally we're multiplying by frequency and dividing by ticks per second,
        // which are the same value for us. Don't actually do the math as the full
        // precision of ticks (a long) cannot be represented in a double during division.
        // For test stability we want a reproducible result.
        //
        // The same issue could occur converting back, in GetElapsedTime(). Unfortunately
        // that isn't virtual so we can't do the same trick. However, if tests advance
        // the clock in multiples of 1ms or so this loss of precision will not be visible.
        Debug.Assert(TimestampFrequency == TimeSpan.TicksPerSecond, "Assuming frequency equals ticks per second");
        return _now.Ticks;
    }

    /// <inheritdoc />
    public override TimeZoneInfo LocalTimeZone => _localTimeZone;

    /// <summary>
    /// Sets the local timezone.
    /// </summary>
    /// <param name="localTimeZone">The local timezone.</param>
    public void SetLocalTimeZone(TimeZoneInfo localTimeZone)
    {
        _localTimeZone = localTimeZone;
    }

    /// <summary>
    /// Gets the amount that the value from <see cref="GetTimestamp"/> increments per second.
    /// </summary>
    /// <remarks>
    /// We fix it here for test instability which would otherwise occur within
    /// <see cref="GetTimestamp"/> when the result of multiplying underlying ticks
    /// by frequency and dividing by ticks per second is truncated to long.
    ///
    /// Similarly truncation could occur when reversing this calculation to figure a time
    /// interval from the difference between two timestamps.
    ///
    /// As ticks per second is always 10^7, setting frequency to 10^7 is convenient.
    /// It happens that the system usually uses 10^9 or 10^6 so this could expose
    /// any assumption made that it is one of those values.
    /// </remarks>
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    /// <summary>
    /// Returns a string representation this clock's current time.
    /// </summary>
    /// <returns>A string representing the clock's current time.</returns>
    public override string ToString() => GetUtcNow().ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        return new FakeTimeProviderTimer(this, dueTime, period, callback, state);
    }

    internal void AddWaiter(FakeTimeProviderTimer.Waiter waiter)
    {
        lock (Waiters)
        {
            Waiters.Add(waiter);
        }
    }

    internal void RemoveWaiter(FakeTimeProviderTimer.Waiter waiter)
    {
        lock (Waiters)
        {
            _ = Waiters.Remove(waiter);
        }
    }

    private List<FakeTimeProviderTimer.Waiter> GetWaitersToWake()
    {
        var l = new List<FakeTimeProviderTimer.Waiter>(Waiters.Count);
        foreach (var w in Waiters)
        {
            if (_now >= w.WakeupTime)
            {
                l.Add(w);
            }
        }

        return l;
    }

    private void WakeWaiters(List<FakeTimeProviderTimer.Waiter> waiters)
    {
        foreach (var w in waiters)
        {
            if (_now >= w.WakeupTime)
            {
                w.TriggerAndSchedule(false);
            }
        }
    }
}
