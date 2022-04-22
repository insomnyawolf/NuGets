using System.Diagnostics;
using Extensions.CliArgumentBuilder;
using Extensions.StdOutput;
using Extensions.File;
using OsuParsers.Decoders;
using OsuApiHelper;
using System.Text;

namespace DanserHelpers
{
    public class DanserHelper : ExtensionsCliArgumentBuilder
    {
        private static readonly string CurrentVidDir = Path.Combine(AppContext.BaseDirectory, "videos");
        private OsuApi OsuApi { get; }
        private string ReplayPath { get; set; }

        public DanserHelper(OsuApi OsuApi)
        {
            this.OsuApi = OsuApi;
        }

        public void SetArtist(string value) => AddEscaped("-artist", value);
        public void SetTitle(string value) => AddEscaped("-title", value);
        public void SetDifficulty(string value) => AddEscaped("-difficulty", value);
        public void SetCreator(string value) => AddEscaped("-creator", value);
        public void SetMD5(string value) => AddEscaped("-md5", value);
        public void SetBeatmapID(int value) => AddEscaped("-id", value);
        public void SetCursors(int value) => AddEscaped("-cursors", value);
        public void SetTagCount(int value) => AddEscaped("-tag", value);
        public void SetSpeed(float value) => AddEscaped("-speed", value);
        public void SetPitch(float value) => AddEscaped("-pitch", value);
        public void SetSettingsJsonName(string value) => AddEscaped("-settings", value);
        public void Debug() => AddEscaped("-debug");
        public void Play() => AddEscaped("-play");
        public void Skip() => AddEscaped("-skip");
        public void SetStartTime(float value) => AddEscaped("-start", value);
        public void SetEndTime(float value) => AddEscaped("-end", value);
        public void Knockout() => AddEscaped("-knockout");
        public void Record() => AddEscaped("-record");
        public void Record(string value) => AddEscaped("-out", value);
        public void SetReplayPath(string value)
        {
            ReplayPath = value;
            AddEscaped("-r", value);
        }
        public void SetMods(string value) => AddEscaped("-mods", value);
        public void SetSkin(string value) => AddEscaped("-skin", value);
        public void SetCS(float value) => AddEscaped("-cs", value);
        public void SetAR(float value) => AddEscaped("-ar", value);
        public void SetOD(float value) => AddEscaped("-od", value);
        public void SetHP(float value) => AddEscaped("-hp", value);
        public void NoDatabaseCheck() => AddEscaped("-nodbcheck");
        public void NoUpdateCheck() => AddEscaped("-noupdatecheck");
        public void ScreenShootAt(float value) => AddEscaped("-ss", value);
        public void Quickstart() => AddEscaped("-quickstart");

        public async Task<string?> RunReplay()
        {
            if (string.IsNullOrEmpty(ReplayPath))
            {
                throw new Exception("No replay was provided");
            }

            using var replayFile = File.Open(ReplayPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var replay = ReplayDecoder.Decode(ReplayPath);
            SetMD5(replay.BeatmapMD5Hash);

            var gamemode = replay.Ruleset.ToString();

            string? tempVideoName = null;

            if (Arguments.ContainsKey("-record"))
            {
                var data = await OsuApi.BeatmapLookup(checksum: replay.BeatmapMD5Hash);

                // ➢ looks cool but sucks for debugging
                tempVideoName = $"{replay.PlayerName} @ {data.beatmapset.title} - {data.beatmapset.artist} [{data.version}] ({replay.Ruleset}) +{replay.Mods} {replay.Accuracy:0.00}%";

                // Sanitize Path
                var test = Path.GetInvalidFileNameChars();
                foreach (var ch in test)
                {
                    if (tempVideoName.IndexOf(ch) != -1)
                    {
                        ExtensionsStdOutput.WarningOutput($"'{ch}' cannot be used in paths on this machine and was deleted from the title");
                        tempVideoName = tempVideoName.Replace(ch + "", string.Empty);
                    }
                }

                if (replay.PerfectCombo)
                {
                    tempVideoName += " FC";
                }

                Arguments.Remove("-record");

                Record(tempVideoName);
            }

            // Do things and such
            await ExecuteDanser();

            tempVideoName = ExtensionsFile.FileExistWithAnyExtensionGetName(CurrentVidDir, tempVideoName);

            ExtensionsStdOutput.HighLightedInfoOutput($"The video will be saved on {tempVideoName}");

            return tempVideoName;
        }

        private async Task ExecuteDanser()
        {
            using var danserStartInfo = new Process();

            var arguments = GetCliArguments();

            ExtensionsStdOutput.HighLightedInfoOutput(arguments);

            danserStartInfo.StartInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = false,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "danser",
                Arguments = arguments,
                //StandardInputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
            };

            danserStartInfo.OutputDataReceived += (sender, e) =>
            {
                ExtensionsStdOutput.InfoOutput(e.Data);
            };

            danserStartInfo.ErrorDataReceived += (sender, e) =>
            {
                ExtensionsStdOutput.ErrorOutput(e.Data);
            };

            danserStartInfo.Start();
            danserStartInfo.BeginErrorReadLine();
            danserStartInfo.BeginOutputReadLine();
            await danserStartInfo.WaitForExitAsync();
            danserStartInfo.Close();
        }

        public string GetCliArguments()
        {
            return ToString();
        }

        public static void CheckDependences()
        {
            var exists = ExtensionsFile.FileExistWithAnyExtension(AppContext.BaseDirectory, "danser");

            if (!exists)
            {
                ExtensionsStdOutput.FatalOutput($"This program only works if it's on the same folder as danser, please check your install");
            }
        }
    }
}
