namespace NuGet.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Xunit;

    public class ObservableFileSystemFixture
    {
        private string targetDir = ".\\" + Guid.NewGuid().ToString();

        public ObservableFileSystemFixture()
        {
            Directory.CreateDirectory(targetDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(targetDir))
                Directory.Delete(targetDir, true);
        }

        [Fact]
        public void when_monitoring_observable_then_gets_last_change()
        {
            var watcher = new ObservableFileSystem(targetDir);
            var file = Guid.NewGuid().ToString() + ".txt";
            var changed = "";
            var changedCount = 0;
            watcher.Changes.Subscribe(s => { changed = s; changedCount++; });
            watcher.Start();

            new[] { file }
                // Generate file name once, repeat for each 65k chars chunk we'll generate.
                .SelectMany(x => Enumerable.Range(0, 50).Select(chunk => x))
                .ForEach(x => File.AppendAllText(Path.Combine(targetDir, x), new string('a', 65000)));

            var maxWait = DateTime.UtcNow.AddSeconds(10);
            while (string.IsNullOrEmpty(changed))
            {
                Thread.Sleep(50);
                if (DateTime.UtcNow > maxWait)
                    break;
            }

            Assert.Equal(Path.GetFileName(changed), file);
            Assert.Equal(1, changedCount);
            watcher.Dispose();
        }

        [Fact]
        public void when_monitored_subfolder_is_deleted_then_gets_change()
        {
            var watcher = new ObservableFileSystem(targetDir);
            var file = Guid.NewGuid().ToString() + ".txt";
            var changed = "";
            var changedCount = 0;
            watcher.Changes.Subscribe(s => { changed = s; changedCount++; });
            watcher.Start();

            new[] { file }
                // Generate file name once, repeat for each 65k chars chunk we'll generate.
                .SelectMany(x => Enumerable.Range(0, 50).Select(chunk => x))
                .ForEach(x => File.AppendAllText(Path.Combine(targetDir, x), new string('a', 65000)));

            var maxWait = DateTime.UtcNow.AddSeconds(10);
            while (string.IsNullOrEmpty(changed))
            {
                Thread.Sleep(50);
                if (DateTime.UtcNow > maxWait)
                    break;
            }

            Assert.Equal(Path.GetFileName(changed), file);
            Assert.Equal(1, changedCount);
            watcher.Dispose();
        }

    }
}