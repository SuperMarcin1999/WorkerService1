using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Linq;
using System.Threading;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private static ILogger<Worker> _logger;
        private static PhysicalFileProvider _fileProvider;
        private static IChangeToken _fileChangeToken;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _fileProvider = new PhysicalFileProvider("/ZippCubePath4");
            WatchForFileChanges();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1, stoppingToken);
            }
        }

        private static void WatchForFileChanges()
        {
            _fileChangeToken = _fileProvider.Watch("*.*");
            _fileChangeToken.RegisterChangeCallback(Notify, default);
        }

        private static void Notify(object state)
        {
            var changedFiles = _fileProvider.GetDirectoryContents(string.Empty);

            foreach (var changedFile in changedFiles)
            {
                if (changedFile.IsDirectory)
                {
                    continue;
                }

                string fileName = changedFile.Name;
                _logger.LogInformation("New file created: " + fileName);
            }

            WatchForFileChanges();
        }
    }
}