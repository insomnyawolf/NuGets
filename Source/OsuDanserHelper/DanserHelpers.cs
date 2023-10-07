using System;
using System.IO;
using System.Threading.Tasks;
using CliArgumentBuilder;
using Extensions;
using Microsoft.Data.Sqlite;
using OsuApiHelper;
using OsuParsers.Decoders;

namespace OsuDanserHelper
{
    public class DanserHelper : CommandLineArgumentBuilder//, IDisposable
    {
        private static readonly string DbLocation = Path.Combine(AppContext.BaseDirectory, "danser.db");

        private OsuApi OsuApi { get; }
        public string ReplayPath { get; private set; }
        public string MapMD5 { get; private set; }

        public DanserHelper(OsuApi OsuApi)
        {
            this.OsuApi = OsuApi;
        }

        public void SetArtist(string value) => AddWithValueEscaped("-artist", value);
        public void SetTitle(string value) => AddWithValueEscaped("-title", value);
        public void SetDifficulty(string value) => AddWithValueEscaped("-difficulty", value);
        public void SetCreator(string value) => AddWithValueEscaped("-creator", value);
        public void SetMD5(string value)
        {
            MapMD5 = value;
            AddWithValueEscaped("-md5", value);
        }
        public void SetBeatmapID(int value) => AddWithValueEscaped("-id", value);
        public void SetCursors(int value) => AddWithValueEscaped("-cursors", value);
        public void SetTagCount(int value) => AddWithValueEscaped("-tag", value);
        public void SetSpeed(double value) => AddWithValueEscaped("-speed", value);
        public void SetPitch(double value) => AddWithValueEscaped("-pitch", value);
        public void SetSettingsJsonName(string value)
        {
            AddWithValueEscaped("-settings", value);
        }
        public void Debug() => Add("-debug");
        public void Play() => Add("-play");
        public void Skip() => Add("-skip");
        public void SetStartTime(double value) => AddWithValueEscaped("-start", value);
        public void SetEndTime(double value) => AddWithValueEscaped("-end", value);
        public void Knockout() => Add("-knockout");
        public void Record() => Add("-record");
        public void Record(string value) => AddWithValueEscaped("-out", value);
        public void SetReplayPath(string value)
        {
            ReplayPath = value;
            AddWithValueEscaped("-r", value);
        }
        public void SetMods(string value) => AddWithValueEscaped("-mods", value);
        public void SetSkin(string value) => AddWithValueEscaped("-skin", value);
        public void SetCS(double value) => AddWithValueEscaped("-cs", value);
        public void SetAR(double value) => AddWithValueEscaped("-ar", value);
        public void SetOD(double value) => AddWithValueEscaped("-od", value);
        public void SetHP(double value) => AddWithValueEscaped("-hp", value);
        public void NoDatabaseCheck() => Add("-nodbcheck");
        public void NoUpdateCheck() => Add("-noupdatecheck");
        public void ScreenShootAt(double value) => AddWithValueEscaped("-ss", value);
        public void Quickstart() => Add("-quickstart");

        public async Task<Danser> Prepare()
        {
            if (string.IsNullOrEmpty(ReplayPath))
            {
                throw new Exception("No replay was provided");
            }

            using var replayFile = File.Open(ReplayPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var replay = ReplayDecoder.Decode(ReplayPath);
            SetMD5(replay.BeatmapMD5Hash);

            var filename = Guid.NewGuid().ToString();

            var title = filename;

            var gamemode = replay.Ruleset.ToString();

            if (Arguments.ContainsKey("-record"))
            {
                var data = await OsuApi.BeatmapLookup(checksum: replay.BeatmapMD5Hash);

                // ➢ looks cool but sucks for debugging
                title = $"{replay.PlayerName} ➢ {data.beatmapset.title} - {data.beatmapset.artist} [{data.version}] ({replay.Ruleset}) +{replay.Mods} {replay.Accuracy:0.00}%";

                if (replay.PerfectCombo)
                {
                    title += " FC";
                }

                Arguments.Remove("-record");

                Record(filename);
            }

            var arguments = GetCliArguments();

            ExtensionsStdOutput.HighLightedInfoOutput(arguments);

            return new Danser()
            {
                Arguments = arguments,
                FileName = filename,
                Title = title,
                DanserSettings = DanserSettingsRead(),
                DbMapInfo = GetDbMapInfo(),
            };
        }

        private string GetCliArguments()
        {
            return ToString();
        }

        private DanserSettings DanserSettingsRead()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "settings", Arguments.ContainsKey("-settings") ? Arguments["-settings"] : "default.json");

            using var settingsFile = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return System.Text.Json.JsonSerializer.Deserialize<DanserSettings>(settingsFile);
        }

        public static void CheckDependences()
        {
            var exists = ExtensionsFile.FileExistWithAnyExtension(AppContext.BaseDirectory, "danser");

            if (!exists)
            {
                ExtensionsStdOutput.FatalOutput($"This program only works if it's on the same folder as danser, please check your install");
            }
        }

        private DbMapInfo GetDbMapInfo()
        {
            using var conn = new SqliteConnection($"Data Source={DbLocation};Mode=ReadOnly;");
            conn.Open();

            using (var statement = conn.CreateCommand())
            {
                statement.CommandText = @$"SELECT * FROM beatmaps where md5 = ""{MapMD5}"";";

                using var reader = statement.ExecuteReader();
                while (reader.Read())
                {
                    return new DbMapInfo()
                    {
                        Directory = (string)reader["dir"],
                        File = (string)reader["file"],
                        LastModified = (long)reader["lastModified"],
                        Title = (string)reader["title"],
                        TitleUnicode = (string)reader["titleUnicode"],
                        Artist = (string)reader["artist"],
                        ArtistUnicode = (string)reader["artistUnicode"],
                        Creator = (string)reader["creator"],
                        DifficultyName = (string)reader["version"],
                        Source = (string)reader["source"],
                        Tags = (string)reader["tags"],
                        CircleSize = (double)reader["cs"],
                        ApproachRate = (double)reader["ar"],
                        SliderMultiplier = (double)reader["sliderMultiplier"],
                        SliderTickRate = (double)reader["sliderTickRate"],
                        AudioFile = (string)reader["audioFile"],
                        PreviewTime = (long)reader["previewTime"],
                        SampleSet = (long)reader["sampleSet"],
                        StackLeniency = (double)reader["stackLeniency"],
                        Mode = (long)reader["mode"],
                        BackGround = (string)reader["bg"],
                        MD5 = (string)reader["md5"],
                        DateAdded = (long)reader["dateAdded"],
                        PlayCount = (long)reader["playCount"],
                        LastPlayed = (long)reader["lastPlayed"],
                        HpDrain = (double)reader["hpdrain"],
                        OverallDifficulty = (double)reader["od"],
                        Stars = (double)reader["stars"],
                        BpmMin = (double)reader["bpmMin"],
                        BpmMax = (double)reader["bpmMax"],
                        Circles = (long)reader["circles"],
                        Sliders = (long)reader["sliders"],
                        Spinners = (long)reader["spinners"],
                        EndTime = (long)reader["endTime"],
                        SetId = (long)reader["setID"],
                        MapId = (long)reader["mapID"],
                    };
                }
            }

            return null;
        }

        //// Destructor
        //bool IsFinalized = false;

        //~DanserHelper()
        //{
        //    Dispose();
        //}

        //public virtual void Dispose()
        //{
        //    if (!IsFinalized)
        //    {
        //        IsFinalized = true;
        //        GC.SuppressFinalize(this);
        //    }
        //}
    }

    public class Danser
    {
        private static readonly string CurrentVidDir = Path.Combine(AppContext.BaseDirectory, "videos");
        public DanserSettings DanserSettings { get; internal set; }
        public DbMapInfo DbMapInfo { get; internal set; }
        public string Arguments { get; internal set; }
        public string Title { get; internal set; }
        public string FileName { get; internal set; }
        public string FilePath => ExtensionsFile.FileExistWithAnyExtensionGetName(CurrentVidDir, FileName);

        public string GetBackgroundImagePath()
        {
            return Path.Combine(DanserSettings.General.OsuSongsDir, DbMapInfo.Directory, DbMapInfo.BackGround);
        }

        public async Task<string> Run()
        {
#if !DEBUG
            using var danserStartInfo = new Process();

            danserStartInfo.StartInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = false,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "danser",
                Arguments = Arguments,
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
#endif
            return FilePath;
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class DbMapInfo
    {
        public string Directory { get; set; }
        public string File { get; set; }
        public long LastModified { get; set; }
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string Creator { get; set; }
        public string DifficultyName { get; set; }
        public string Source { get; set; }
        public string Tags { get; set; }
        public double CircleSize { get; set; }
        public double ApproachRate { get; set; }
        public double SliderMultiplier { get; set; }
        public double SliderTickRate { get; set; }
        public string AudioFile { get; set; }
        public long PreviewTime { get; set; }
        public long SampleSet { get; set; }
        public double StackLeniency { get; set; }
        public long Mode { get; set; }
        public string BackGround { get; set; }
        public string MD5 { get; set; }
        public long DateAdded { get; set; }
        public long PlayCount { get; set; }
        public long LastPlayed { get; set; }
        public double HpDrain { get; set; }
        public double OverallDifficulty { get; set; }
        public double Stars { get; set; }
        public double BpmMin { get; set; }
        public double BpmMax { get; set; }
        public long Circles { get; set; }
        public long Sliders { get; set; }
        public long Spinners { get; set; }
        public long EndTime { get; set; }
        public long SetId { get; set; }
        public long MapId { get; set; }
        public string Url => $"https://osu.ppy.sh/b/{MapId}";
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}