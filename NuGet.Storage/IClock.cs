namespace NuGet.Storage
{
    using System;

    public interface IClock
    {
        /// <summary>
        /// Current system date and time.
        /// </summary>
        DateTimeOffset Now { get; }

        /// <summary>
        /// An observable tick that is issued at the interval 
        /// specified by the implementation of this interface, 
        /// and containing the <see cref="Now"/> value at each 
        /// interval.
        /// </summary>
        IObservable<DateTimeOffset> Tick { get; }
    }
}