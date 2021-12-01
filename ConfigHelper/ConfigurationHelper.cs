using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ConfigHelper
{
    public class ConfigurationHelper<T> where T : class, new()
    {
        private readonly string ConfigFullPath;
        private readonly string ConfigDirectory;
        private FileSystemWatcher watcher;

        static SemaphoreSlim Semaphore = new(1);

        private ILogger<ConfigurationHelper<T>> Logger;

        public T Config { get; private set; }

        public ConfigurationHelper(string ConfigPath, ILoggerFactory logger)
        {
            Logger = logger?.CreateLogger<ConfigurationHelper<T>>();

            if (string.IsNullOrEmpty(ConfigPath))
            {
                throw new Exception($"{nameof(ConfigPath)} can not be null or empty.");
            }

            ConfigFullPath = Path.GetFullPath(ConfigPath);
            ConfigDirectory = Path.GetDirectoryName(ConfigPath);

            Logger?.LogInformation($"The config file will be => {ConfigFullPath}");

            Load();

            FileWatch();
        }

        private void FileWatch()
        {
            var fileName = Path.GetFileName(ConfigFullPath);
            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher()
            {
                // Set the watch to look in that folder
                Path = ConfigDirectory,
                // Watch for changes (LastWrite)
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                // Only watch config file.
                Filter = fileName,
                // Begin watching.
                EnableRaisingEvents = true,
                // ??
                InternalBufferSize = 4096 * 4,
            };

            // Add event handlers.
            watcher.Created += ChangeDetected;
            watcher.Changed += ChangeDetected;

            watcher.Error += (object sender, ErrorEventArgs e) =>
            {
                Logger.LogCritical("Error Detecting Config Changes", e.ToString());
            };
            
            watcher.EnableRaisingEvents = true;
        }

        private void ChangeDetected(object source, FileSystemEventArgs e)
        {
#warning Maybe use a sleep there to let the program that modified the file close it's handle
            Load();
        }

        public void Load()
        {
            if (!File.Exists(ConfigFullPath))
            {
                Save();
            }

            Semaphore.Wait();

            using (var fs = File.Open(ConfigFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Config = JsonSerializer.Deserialize<T>(fs);
                fs.Close();
            }

            Semaphore.Release();

            Logger?.LogInformation("Configuration Read");
        }

        public void Save()
        {
            if (Config is null)
            {
                Config = new T();
            }

            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }

            Semaphore.Wait();

            using (var fs = File.Open(ConfigFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                JsonSerializer.Serialize(fs, Config);
                fs.Flush();
                fs.SetLength(fs.Position);
                fs.Close();
            }

            Semaphore.Release();

            Logger?.LogInformation("Configuration Written");

        }

        public void OverwriteCurrent(T newSettings = null)
        {
            if (Config is null)
            {
                Config = new T();
            }

            Config = newSettings;
        }
    }
}
