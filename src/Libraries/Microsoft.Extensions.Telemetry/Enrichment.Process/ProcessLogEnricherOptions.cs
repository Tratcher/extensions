﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Telemetry.Enrichment;

/// <summary>
/// Options for the process enricher.
/// </summary>
public class ProcessLogEnricherOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether current process ID is used for log enrichment.
    /// </summary>
    /// <remarks>
    /// Default set to <see langword="true" />.
    /// </remarks>
    public bool ProcessId { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether current thread ID is used for log enrichment.
    /// </summary>
    /// <remarks>
    /// Default set to <see langword="false" />.
    /// </remarks>
    public bool ThreadId { get; set; }
}
