﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Shared.Data.Validation;

namespace Microsoft.Extensions.Diagnostics.ResourceMonitoring;

/// <summary>
/// Options for <see cref="IResourceUtilizationTracker"/>.
/// </summary>
public class ResourceUtilizationTrackerOptions
{
    /// <remarks>
    /// Internal for testing.
    /// </remarks>
    internal const int MinimumSamplingWindow = 100;
    internal const int MaximumSamplingWindow = 900000; // 15 minutes.
    internal const int MinimumSamplingPeriod = 1;
    internal const int MaximumSamplingPeriod = 900000; // 15 minutes.
    internal static readonly TimeSpan DefaultCollectionWindow = TimeSpan.FromSeconds(5);
    internal static readonly TimeSpan DefaultSamplingInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum time window of which utilization can be requested.
    /// </summary>
    /// <remarks>
    /// Default set to 5 seconds.
    /// </remarks>
    [TimeSpan(MinimumSamplingWindow, MaximumSamplingWindow)]
    public TimeSpan CollectionWindow { get; set; } = DefaultCollectionWindow;

    /// <summary>
    /// Gets or sets the interval at which a new sample is captured.
    /// </summary>
    /// <remarks>
    /// Default set to 1 second.
    /// </remarks>
    [TimeSpan(MinimumSamplingPeriod, MaximumSamplingPeriod)]
    public TimeSpan SamplingInterval { get; set; } = DefaultSamplingInterval;

    /// <summary>
    /// Gets or sets the default period used for utilization calculation.
    /// </summary>
    /// <remarks>
    /// Default set to 5 seconds. The value needs to be less than or equal to the <see cref="CollectionWindow"/>.
    /// Most importantly, this period is used to calculate <see cref="Utilization"/> instances pushed to publishers.
    /// </remarks>
    [Experimental]
    [TimeSpan(MinimumSamplingWindow, MaximumSamplingWindow)]
    public TimeSpan CalculationPeriod { get; set; } = DefaultCollectionWindow;
}
