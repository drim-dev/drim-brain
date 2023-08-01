// Copyright 2017-2022 Digital Asset Exchange Limited. All rights reserved.
// Use of this source code is governed by Microsoft Reference Source
// License (MS-RSL) that can be found in the LICENSE file.

using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Features;

public static class Greet
{
    [AllowAnonymous]
    [HttpGet("/greet")]
    public class Endpoint : EndpointWithoutRequest<string>
    {
        private static readonly string Name = Guid.NewGuid().ToString();

        public override Task<string> ExecuteAsync(CancellationToken cancellationToken) =>
            Task.FromResult($"Hello from {Name}!");
    }
}
