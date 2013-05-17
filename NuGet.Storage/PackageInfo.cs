namespace NuGet.Storage
{
    using System;
    using System.Linq;

    public class PackageInfo
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Owners { get; set; }
   }
}