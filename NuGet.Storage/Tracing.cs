namespace NuGet.Storage
{
    using System;
    using System.Linq;
    using NuGet.Storage.Diagnostics;
    using NuGet.Storage.Properties;

    internal static class Tracing
    {
        public static class Store
        {
            private static readonly ITracer tracer = Tracer.Get(typeof(NuGet.Storage.Store));

            public static void Initialized(string packagesFolder)
            {
                tracer.Verbose(Strings.Store.Initialized(packagesFolder));
            }
        }
    }
}