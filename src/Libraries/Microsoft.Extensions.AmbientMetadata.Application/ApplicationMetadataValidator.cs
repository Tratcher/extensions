﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Validation;

namespace Microsoft.Extensions.AmbientMetadata;

[OptionsValidator]
internal sealed partial class ApplicationMetadataValidator : IValidateOptions<ApplicationMetadata>
{
}
