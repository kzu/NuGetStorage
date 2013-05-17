namespace NuGet.Storage
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    public class Clock : IClock
    {
        public static IClock Default { get; private set; }

        static Clock()
        {
            Default = new Clock(TimeSpan.FromMilliseconds(200));
        }

        internal Clock(TimeSpan tickInterval)
        {
            this.Tick = Observable.Interval(tickInterval)
                // Keep a single timer running
                .Publish()
                // Stop running it when we have no connections 
                // to keep alive.
                .RefCount()
                .Select(_ => DateTimeOffset.Now);
        }

        public DateTimeOffset Now { get { return DateTimeOffset.Now; } }
        public IObservable<DateTimeOffset> Tick { get; private set; }
    }
}