﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Shared.Diagnostics;
using Microsoft.Shared.Text;

namespace Microsoft.Extensions.Hosting.Testing.Internal;

internal sealed class StartupHostedService : IHostedService
{
    internal readonly TimeSpan Timeout;

    private const string TimeoutMessageTemplate = "Exceeded maximum server initialization time of {0}. Adjust {1} or split your work into smaller chunks.";
    private static readonly CompositeFormat _timeoutMessage = CompositeFormat.Parse(TimeoutMessageTemplate);
    private readonly TimeProvider _timeProvider;
    private IStartupInitializer[] _initializers;

    public StartupHostedService(IOptions<StartupInitializationOptions> options,
        IEnumerable<IStartupInitializer> initializers, TimeProvider? timeProvider = null, IDebuggerState? debugger = null)
    {
        Timeout = Throw.IfMemberNull(options, options.Value).Timeout;
        _initializers = initializers.ToArray();
        _timeProvider = timeProvider ?? TimeProvider.System;

        if (debugger?.IsAttached ?? DebuggerState.System.IsAttached)
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var tcts = _timeProvider.CreateCancellationTokenSource(Timeout);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, tcts.Token);
        cts.Token.ThrowIfCancellationRequested();

        var tasks = _initializers.Select(initializer => initializer.InitializeAsync(cts.Token));

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (TaskCanceledException e) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException(
                message: _timeoutMessage.Format(CultureInfo.InvariantCulture, Timeout, nameof(StartupInitializationOptions)),
                innerException: e);
        }

        // StartupHostedService will be in the memory for the lifetime of the process.
        // Looking at codebase, startup initializers are often holding many objects, so to allow GC to trace less of them,
        // we are allowing to collect the initializers.
        _initializers = Array.Empty<IStartupInitializer>();
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
