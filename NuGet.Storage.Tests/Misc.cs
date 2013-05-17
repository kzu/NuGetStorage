namespace NuGet.Storage
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using NuGet.Storage.Diagnostics;
    using Xunit;

    public class Misc
    {
        [Trace("NuGet.Storage.Misc", SourceLevels.All)]
        [Fact]
        public void when_tracing_then_traces()
        {
            Tracer.Get<Misc>().Verbose("when_tracing_then_traces");
        }

        [Fact]
        public void when_not_tracing_then_does_not_trace()
        {
            Tracer.Get<Misc>().Info("when_not_tracing_then_does_not_trace");
        }
    }
}