namespace NuGet.Storage
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using NuGet.Storage.Diagnostics;
    using Xunit;

    public class TraceAttribute : BeforeAfterTestAttribute
    {
        private string sourceName;
        private SourceLevels level;
        private ConsoleTraceListener listener = new ConsoleTraceListener();

        public TraceAttribute(string sourceName, SourceLevels level)
        {
            this.sourceName = sourceName;
            this.level = level;
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            Tracer.Manager.AddListener(sourceName, listener);
            Tracer.Manager.SetTracingLevel(sourceName, level);

            base.Before(methodUnderTest);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            base.After(methodUnderTest);
            
            Tracer.Manager.SetTracingLevel(sourceName, SourceLevels.Off);
            Tracer.Manager.RemoveListener(sourceName, listener);
        }
    }
}