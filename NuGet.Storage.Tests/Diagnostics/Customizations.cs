namespace NuGet.Storage.Diagnostics
{
    using System;
    using System.Diagnostics;

    partial class Tracer
    {
        /// <summary>
        /// Initializes the <see cref="Tracer"/> class by specifying the 
        /// diagnostics tracer as the default implementation.
        /// </summary>
        static Tracer()
        {
            manager = new TracerManager();
        }

        public static ITracerManager Manager { get { return manager; } }

        partial class DefaultManager
        {
            public void AddListener(string sourceName, TraceListener listener) { throw new NotSupportedException(); }
            public void RemoveListener(string sourceName, TraceListener listener) { throw new NotSupportedException(); }
            public void SetTracingLevel(string sourceName, SourceLevels level) { throw new NotSupportedException(); }
        }
    }

    partial interface ITracerManager
    {
        void AddListener(string sourceName, TraceListener listener);
        void RemoveListener(string sourceName, TraceListener listener);
        void SetTracingLevel(string sourceName, SourceLevels level);
    }
}