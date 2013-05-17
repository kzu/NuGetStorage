namespace NuGet.Storage
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class StoreFixture
    {
        [Trace("NuGet.Storage.Store", SourceLevels.All)]
        [Fact]
        public void when_initializing_then_traces()
        {
            var store = new Store(new DirectoryInfo(".").FullName);
        }
    }
}