using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ConfigHelper
{
    public class ConfigurationHelper<T> where T : new()
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;

        static ConfigurationHelper()
        {
            JsonSerializerOptions = new()
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                IncludeFields = true,

            };

            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public delegate void ConfigurationChanged();
        public event ConfigurationChanged OnConfigurationChanged;

        private readonly string ConfigFullPath;
        private readonly string ConfigDirectory;
        private FileSystemWatcher watcher;

        private readonly FileStream FileStream;

        private readonly SemaphoreSlim Semaphore = new(1);

        private readonly ILogger<ConfigurationHelper<T>> Logger;

        public T Config { get; private set; }

        public ConfigurationHelper(string ConfigPath, ILoggerFactory logger = null)
        {
            Logger = logger?.CreateLogger<ConfigurationHelper<T>>();

            if (string.IsNullOrEmpty(ConfigPath))
            {
                throw new Exception($"{nameof(ConfigPath)} can not be null or empty.");
            }

            ConfigFullPath = Path.GetFullPath(ConfigPath);
            ConfigDirectory = Path.GetDirectoryName(ConfigPath) + '/';

            FileStream = File.Open(ConfigFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            Logger?.LogInformation($"The config file will be => {ConfigFullPath}");

            Load();

            FileWatch();
        }

        public bool IsEmpty()
        {
            return FileStream.Length < 3;
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

        private bool IsDispatched = false;

        // It calls the event twice and i can't figure why
        private void ChangeDetected(object source, FileSystemEventArgs e)
        {
            if (IsDispatched)
            {
                IsDispatched = false;
                return;
            }

            IsDispatched = true;

            //var src = (FileSystemWatcher)source;
            Load();
            OnConfigurationChanged?.Invoke();
        }

        public void Load()
        {
            Semaphore.Wait();
            if (!File.Exists(ConfigFullPath) || FileStream.Length == 0)
            {
                // IMPORTANT, prevents deadlock
                Semaphore.Release();
                Save();
            }

            FileStream.Position = 0;
            Config = JsonSerializer.Deserialize<T>(FileStream, JsonSerializerOptions);

            Semaphore.Release();

            Logger?.LogInformation("Configuration Read");
        }

        public void Save()
        {
            Semaphore.Wait();

            if (watcher is not null)
            {
                watcher.EnableRaisingEvents = false;
            }

            if (Config is null)
            {
                Config = new T();
            }

            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }

            FileStream.Position = 0;
            JsonSerializer.Serialize(FileStream, Config, JsonSerializerOptions);
            FileStream.Flush();
            FileStream.SetLength(FileStream.Position);

            if (watcher is not null)
            {
                watcher.EnableRaisingEvents = true;
            }

            Semaphore.Release();

            Logger?.LogInformation("Configuration Written");
        }

        public void OverwriteCurrent(T newSettings)
        {
            Config = newSettings;
        }
    }
}
