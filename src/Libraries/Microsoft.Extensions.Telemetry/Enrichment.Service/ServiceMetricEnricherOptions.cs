﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.AmbientMetadata;

namespace Microsoft.Extensions.Telemetry.Enrichment;

/// <summary>
/// Options for the service metric enricher.
/// </summary>
public class ServiceMetricEnricherOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether <see cref="ApplicationMetadata.EnvironmentName"/> is used for metric enrichment.
    /// </summary>
    /// <remarks>
    /// Default set to <see langword="true" />.
    /// </remarks>
    public bool EnvironmentName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="ApplicationMetadata.ApplicationName"/> is used for metric enrichment.
    /// </summary>
    /// <remarks>
    /// Default set to <see langword="true" />.
    /// </remarks>
    public bool ApplicationName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="ApplicationMetadata.DeploymentRing"/> is used for metric enrichment.
    /// </summary>
    /// <remarks>
    /// Default set to <see langword="false" />.
    /// </remarks>
    public bool DeploymentRing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="ApplicationMetadata.BuildVersion"/> is used for metric enrichment.
    /// </summary>
    /// <remarks>
    /// Default set to <see langword="false" />.
    /// </remarks>
    public bool BuildVersion { get; set; }
}
