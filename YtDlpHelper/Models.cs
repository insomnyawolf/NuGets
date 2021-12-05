namespace YtDlp
{
#pragma warning disable IDE1006 // Estilos de nombres
    // Done it that way to preserve the api structure
    public class TrackMetadata
    {
        public string title { get; set; }
        public double duration { get; set; }
        public string duration_string { get; set; }
        public string artist { get; set; }
        public string description { get; set; }
        public string webpage_url { get; set; }
    }
#pragma warning restore IDE1006 // Estilos de nombres

    //size = 5320kB time = 00:00:28.38 bitrate = 1535.6kbits / s speed = 0.944x
    public struct ProgressUpdateData
    {
        public string? Size { get; internal set; }
        public TimeSpan Time { get; internal set; } = TimeSpan.Zero;
        public string? Bitrate { get; internal set; }
        public string? Speed { get; internal set; }
    }
}

