namespace NuGet.Storage
{
    using System;
    using System.Linq;
    using NuGet.Storage.Diagnostics;

    public class Store
    {
        public event EventHandler Changed = (sender, args) => { };

        public Store(string packagesFolder)
        {
            this.PackagesFolder = packagesFolder;
            Tracing.Store.Initialized(packagesFolder);
        }

        public string PackagesFolder { get; private set; }
    }
}