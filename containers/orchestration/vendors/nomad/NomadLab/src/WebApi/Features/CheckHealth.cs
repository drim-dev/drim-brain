// Copyright 2017-2022 Digital Asset Exchange Limited. All rights reserved.
// Use of this source code is governed by Microsoft Reference Source
// License (MS-RSL) that can be found in the LICENSE file.

using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Features;

public static class CheckHealth
{
    [AllowAnonymous]
    [HttpGet("/health")]
    public class Endpoint : EndpointWithoutRequest<string>
    {
        private static readonly bool Healthy;

        static Endpoint()
        {
            Healthy = Random.Shared.Next() % 2 == 0;
        }

        public override Task<string> ExecuteAsync(CancellationToken cancellationToken) =>
            Healthy
                ? Task.FromResult("HEALTHY")
                : throw new Exception("UNHEALTHY");
    }
}
