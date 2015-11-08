using System;
using System.IO;

namespace Elephanet.Tests.Infrastructure
{
    internal static class Environment
    {
        internal static bool IsRunningOnMono { get; } = Type.GetType("Mono.Runtime") != null;
    }
}