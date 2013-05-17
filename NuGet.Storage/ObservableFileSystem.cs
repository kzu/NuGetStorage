namespace NuGet.Storage
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;
    using System.Threading;
    using NuGet.Storage.Diagnostics;

    /// <summary>
    /// Exposes changes in the file system as an observable stream 
    /// of file or directory names that changed.
    /// </summary>
    public class ObservableFileSystem : IDisposable
    {
        private static readonly ITracer tracer = Tracer.Get<ObservableFileSystem>();

        private IClock clock;
        private string path;
        private string filter;
        private bool includeSubdirs;
        private FileSystemWatcher directoryWatcher;
        private ConcurrentDictionary<string, SlidingNotification> fileWatchers = new ConcurrentDictionary<string, SlidingNotification>();
        private Subject<FileSystemEventArgs> changesSubject;

        private CompositeDisposable disposables = new CompositeDisposable();

        public ObservableFileSystem(IClock clock, string path, bool includeSubdirectories = false)
            : this(clock, path, "*.*")
        {
        }

        public ObservableFileSystem(IClock clock, string path, string filter, bool includeSubdirectories = false)
        {
            this.clock = clock;
            this.path = path;
            this.filter = filter;
            this.includeSubdirs = includeSubdirectories;
            this.changesSubject = new Subject<FileSystemEventArgs>();
        }

        public IObservable<FileSystemEventArgs> Changes { get { return this.changesSubject; } }

        public void Start()
        {
            if (disposables.IsDisposed)
                throw new ObjectDisposedException(this.ToString());

            directoryWatcher = new FileSystemWatcher(this.path, this.filter)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.DirectoryName | NotifyFilters.FileName,
                IncludeSubdirectories = false,
                InternalBufferSize = 16384,
                EnableRaisingEvents = true,
            };
            disposables.Add(directoryWatcher);

            directoryWatcher.Changed += OnDirectoryChanged;
            // TODO: not sure we need all these...
            directoryWatcher.Created += OnDirectoryChanged;
            directoryWatcher.Deleted += OnDirectoryChanged;
            directoryWatcher.Renamed += OnDirectoryChanged;

            tracer.Info("Monitoring of folder {0} has started.", this.path);
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ObservableFileSystem()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposables.IsDisposed)
            {
                if (disposing)
                {
                    disposables.Dispose();

                    this.fileWatchers.Values.ToList().ForEach(x => x.Dispose());
                    this.fileWatchers.Clear();

                    tracer.Info("Monitoring of folder {0} has stopped.", this.path);
                }
            }
        }

        private void OnDirectoryChanged(object sender, FileSystemEventArgs args)
        {
            // Deletions can be notified right-away.

        }

        private void NotifyChange(FileSystemEventArgs change)
        {
            //if (File.Exists(path))
            //    tracer.Verbose("Change detected on file {0}.", path);
            //else if (Directory.Exists(path))
            //    tracer.Verbose("Change detected on directory {0}.", path);
            //else
            //    tracer.Verbose("Change detected on file or directory {0}.", path);

            SlidingNotification entry;
            this.fileWatchers.TryRemove(change.FullPath, out entry);
            this.changesSubject.OnNext(change);
        }

        /// <summary>
        /// Sliding expiration timer to track when a file is not getting 
        /// any further changes.
        /// </summary>
        private class SlidingNotification : IDisposable
        {
            private SerialDisposable slidingTick = new SerialDisposable();
            private ObservableFileSystem observable;
            private FileSystemEventArgs change;
            private Timer timer;

            public SlidingNotification(ObservableFileSystem observable, FileSystemEventArgs change)
            {
                this.observable = observable;
                this.change = change;
            }

            public void OnTimer(object state)
            {
                observable.NotifyChange(change);
            }

            public void Touch(FileSystemEventArgs change)
            {
                this.change = change;
                timer.Change(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(-1));
            }   

            public void Dispose()
            {
                timer.Dispose();
            }
        }
    }
}
