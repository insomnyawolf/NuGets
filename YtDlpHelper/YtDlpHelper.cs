using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace YtDlp
{
    public class YtDlpHelper
    {
        public delegate Task MetadataRecived(TrackMetadata trackMetadata);
        public event MetadataRecived? OnMetadataRecived;

        public delegate Task FinishedRecivingMetadata();
        public event FinishedRecivingMetadata? OnFinishedRecivingMetadata;

        private Semaphore SemaphoreGetInfo { get; }
        private Semaphore SemaphoreDownloadToStream { get; }

        private ILogger? Logger { get; }

        public YtDlpHelper(ILogger? Logger = null)
        {
            this.Logger = Logger;
            SemaphoreGetInfo = new Semaphore(1, 1);
            SemaphoreDownloadToStream = new Semaphore(1, 1);
        }

        public async Task GetInfoEvents(string target)
        {
            var ticks = DateTime.UtcNow.ToShortDateString();
            Console.WriteLine("Test");
            SemaphoreGetInfo.WaitOne();

            // No url will search on youtube, otherwise will thry the specified service
            if (!target.StartsWith("http"))
            {
                target = $"\"ytsearch:{target}\"";
            }

            using var ytdlInfo = new Process();

            ytdlInfo.StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $@"--quiet --dump-json --ignore-no-formats-error --parse-metadata "":(?P<formats>)"" --parse-metadata "":(?P<thumbnails>)"" --parse-metadata "":(?P<categories>)"" --parse-metadata "":(?P<tags>)"" --parse-metadata "":(?P<automatic_captions>)"" --parse-metadata "":(?P<subtitles>)"" --parse-metadata "":(?P<requested_formats>)"" {target}",
                RedirectStandardOutput = true,
                RedirectStandardInput = false,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            // This is a workaround to support playlist, hopefully i never have to touch this again 
            // I hate myself what i did before trying this, who needs to keep threads with perfect timmings or infinite reading console operations
            // Getting data trough events is <3, whoever designed that, thankyou
            // To the dude that designed "ReadLine" and such without timeouts, fuck you >:(
            // But in the end i'm a bit thankfull because that forced me to find a better way that wasn't even that hard tbh
            // Total time spent 7h (╯°□°）╯︵ ┻━┻

            ytdlInfo.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                var entry = JsonSerializer.Deserialize<TrackMetadata>(e.Data);

                if (entry == null)
                {
                    throw new Exception("Something went wrong, the track metadata was deserialized as null");
                }

                OnMetadataRecived?.Invoke(entry);

                Logger?.LogTrace($"ytdl-info => {ytdlInfo.StartInfo.FileName} {ytdlInfo.StartInfo.Arguments} => Recived:\n{JsonSerializer.Serialize(entry)}");
            };

            ytdlInfo.ErrorDataReceived += (sender, e) =>
            {
                Logger?.LogError($"ytdl-info => {ytdlInfo.StartInfo.FileName} {ytdlInfo.StartInfo.Arguments} => Error:\n{JsonSerializer.Serialize(e.Data)}");
            };

            ytdlInfo.Exited += (sender, e) =>
            {
                OnFinishedRecivingMetadata?.Invoke();

                Logger?.LogTrace("ytdl-info Finished");
            };

            ytdlInfo.Start();
            ytdlInfo.BeginErrorReadLine();
            ytdlInfo.BeginOutputReadLine();

            await ytdlInfo.WaitForExitAsync();

            ytdlInfo.Close();

            SemaphoreGetInfo.Release();
        }

        public delegate Task StatusUpdated(TrackMetadata trackMetadata, ProgressUpdateData progressUpdateData);
        public event StatusUpdated? OnStatusUpdated;

        public delegate Task FinishedUpdatingStatus();
        public event FinishedUpdatingStatus? OnFinishedUpdatingStatus;

        public async Task DownloadToStream(TrackMetadata trackMetadata, Func<Stream, Task> streamProcessing, Action<Process>? YtDlProcess = null, Action<Process>? ffmpegProcess = null)
        {
            using var ytdlContent = new Process();

            ytdlContent.StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $@"--quiet --format bestaudio ""{trackMetadata.webpage_url}"" -o -",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            ytdlContent.ErrorDataReceived += (sender, e) =>
            {
                Logger?.LogError($"ytdl-content => {ytdlContent.StartInfo.FileName} {ytdlContent.StartInfo.Arguments} => {e.Data}");
            };

            ytdlContent.Exited += (sender, e) =>
            {
                Logger?.LogTrace("ytdl-content Finished");
            };

            using var ffmpeg = new Process();

            // Convert to raw pcm format (.pcm files)
            ffmpeg.StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@" -i - -ac 2 -f s16le -ar 48000 -hide_banner pipe:1",
                //Arguments = $@" -i - -ac 2 -f s16le -ar 48000 -hide_banner test.pcm",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            ffmpeg.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data?.StartsWith("size=") == true)
                {
                    // Progress Report 
                    // Example output
                    // size=    5320kB time=00:00:28.38 bitrate=1535.6kbits/s speed=0.944x
                    // size=    4128kB time=00:00:22.02 bitrate=1535.5kbits/s speed=1.03x
                    Logger?.LogInformation($"ffmpeg => {trackMetadata.webpage_url} => {e.Data}");

                    var dataToFind = new string[] { "size=", "time=", "bitrate=", "speed=" };
                    var indexes = new int[dataToFind.Length];

                    for (int i = 0; i < indexes.Length; i++)
                    {
                        indexes[i] = e.Data.IndexOf(dataToFind[i]);
                    }

                    var progressUpdate = new ProgressUpdateData()
                    {
                        Size = getValue(0),
                        Time = TimeSpan.Parse(getValue(1)),
                        Bitrate = getValue(2),
                        Speed = getValue(3),
                    };

                    string? getValue(int index)
                    {
                        if (indexes[index] == -1)
                        {
                            // data not found
                            return null;
                        }

                        // add offset to ignoe the field header and only get the data
                        int startingPoint = indexes[index] + dataToFind[index].Length;

                        index++;

                        // search till the next term is found
                        while (index < indexes.Length)
                        {
                            if (indexes[index] != -1)
                            {
                                break;
                            }
                            index++;
                        }

                        if (index == indexes.Length)
                        {
                            // no other term was found
                            // return from starting point to end
                            return e.Data[startingPoint..];
                        }

                        // other term was found
                        // return from starting point to nextTerm
                        return e.Data[startingPoint..indexes[index]].Trim();
                    }

                    OnStatusUpdated?.Invoke(trackMetadata, progressUpdate);

                    return;
                }

                Logger?.LogError($"ffmpeg => {ffmpeg.StartInfo.FileName} {ffmpeg.StartInfo.Arguments} => {e.Data}");
            };

            ffmpeg.Exited += (sender, e) =>
            {
                OnFinishedUpdatingStatus?.Invoke();

                Logger?.LogTrace("ffmpeg Finished");
            };

            OnStatusUpdated?.Invoke(trackMetadata, new ProgressUpdateData()
            {
                Time = TimeSpan.FromSeconds(0)
            });

            YtDlProcess?.Invoke(ytdlContent);
            ffmpegProcess?.Invoke(ffmpeg);

            ytdlContent.Start();
            ffmpeg.Start();

            ytdlContent.BeginErrorReadLine();
            ffmpeg.BeginErrorReadLine();

            // Avoid deadlocks
            _ = Task.Run(async () =>
            {
                await ytdlContent.StandardOutput.BaseStream.CopyToAsync(ffmpeg.StandardInput.BaseStream);
                // No mas datos
                ffmpeg.StandardInput.Close();
                ytdlContent.Close();
            });

            // Done that way bc object disposal shinenigans
            await streamProcessing.Invoke(ffmpeg.StandardOutput.BaseStream);

            ffmpeg.Close();
        }
    }
}