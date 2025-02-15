﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Http.Telemetry.Logging.Test.Internal;

internal class TestHttpClient2 : ITestHttpClient2
{
    private readonly System.Net.Http.HttpClient _httpClient;

    public TestHttpClient2(System.Net.Http.HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequestMessage)
    {
        return _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
    }
}
