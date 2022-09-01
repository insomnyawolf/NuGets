using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OsuDanserHelper
{
    public class General
    {
        public string OsuSongsDir { get; set; }
        public string OsuSkinsDir { get; set; }
        public bool DiscordPresenceOn { get; set; }
        public bool UnpackOszFiles { get; set; }
    }

    public class Experimental
    {
        public bool UsePersistentBuffers { get; set; }
    }

    public class Graphics
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public bool Fullscreen { get; set; }
        public bool VSync { get; set; }
        public int FPSCap { get; set; }
        public int MSAA { get; set; }
        public bool ShowFPS { get; set; }
        public Experimental Experimental { get; set; }
    }

    public class LinuxUnix
    {
        public int BassPlaybackBufferLength { get; set; }
        public int BassDeviceBufferLength { get; set; }
        public int BassUpdatePeriod { get; set; }
        public int BassDeviceUpdatePeriod { get; set; }
    }

    public class Audio
    {
        public double GeneralVolume { get; set; }
        public double MusicVolume { get; set; }
        public double SampleVolume { get; set; }
        public int Offset { get; set; }
        public int HitsoundPositionMultiplier { get; set; }
        public bool IgnoreBeatmapSamples { get; set; }
        public bool IgnoreBeatmapSampleVolume { get; set; }
        public bool PlayNightcoreSamples { get; set; }
        public double BeatScale { get; set; }
        public bool BeatUseTimingPoints { get; set; }

        [JsonPropertyName("Linux/Unix")]
        public LinuxUnix LinuxUnix { get; set; }
    }

    public class Input
    {
        public string LeftKey { get; set; }
        public string RightKey { get; set; }
        public string RestartKey { get; set; }
        public string SmokeKey { get; set; }
        public bool MouseButtonsDisabled { get; set; }
        public bool MouseHighPrecision { get; set; }
        public int MouseSensitivity { get; set; }
    }

    public class HitErrorMeter
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public bool ShowPositionalMisses { get; set; }
        public bool ShowUnstableRate { get; set; }
        public int UnstableRateDecimals { get; set; }
        public int UnstableRateScale { get; set; }
    }

    public class AimErrorMeter
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int DotScale { get; set; }
        public string Align { get; set; }
        public bool ShowUnstableRate { get; set; }
        public int UnstableRateScale { get; set; }
        public int UnstableRateDecimals { get; set; }
        public bool CapPositionalMisses { get; set; }
        public bool AngleNormalized { get; set; }
    }

    public class Score
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public string ProgressBar { get; set; }
        public bool ShowGradeAlways { get; set; }
    }

    public class HpBar
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
    }

    public class ComboCounter
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
    }

    public class Color
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
        public bool EnableRainbow { get; set; }
        public int RainbowSpeed { get; set; }
        public BaseColor BaseColor { get; set; }
        public bool EnableCustomHueOffset { get; set; }
        public int HueOffset { get; set; }
        public bool FlashToTheBeat { get; set; }
        public int FlashAmplitude { get; set; }
    }

    public class PPCounter
    {
        public bool Show { get; set; }
        public double Scale { get; set; }
        public int Opacity { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public Color Color { get; set; }
        public int Decimals { get; set; }
        public string Align { get; set; }
        public bool ShowInResults { get; set; }
        public bool ShowPPComponents { get; set; }
    }

    public class HitCounter
    {
        public bool Show { get; set; }
        public double Scale { get; set; }
        public int Opacity { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public List<Color> Color { get; set; }
        public int Spacing { get; set; }
        public int FontScale { get; set; }
        public string Align { get; set; }
        public string ValueAlign { get; set; }
        public bool Vertical { get; set; }
        public bool Show300 { get; set; }
    }

    public class BgColor
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public double Value { get; set; }
    }

    public class FgColor
    {
        public int Hue { get; set; }
        public double Saturation { get; set; }
        public double Value { get; set; }
    }

    public class StrainGraph
    {
        public bool Show { get; set; }
        public int Opacity { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public string Align { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public BgColor BgColor { get; set; }
        public FgColor FgColor { get; set; }
    }

    public class KeyOverlay
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
    }

    public class ScoreBoard
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public bool AlignRight { get; set; }
        public bool HideOthers { get; set; }
        public bool ShowAvatars { get; set; }
        public int ExplosionScale { get; set; }
    }

    public class Mods
    {
        public bool Show { get; set; }
        public int Scale { get; set; }
        public int Opacity { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public bool HideInReplays { get; set; }
        public bool FoldInReplays { get; set; }
        public int AdditionalSpacing { get; set; }
    }

    public class BorderColor
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
    }

    public class BackgroundColor
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
    }

    public class Boundaries
    {
        public bool Enabled { get; set; }
        public int BorderThickness { get; set; }
        public int BorderFill { get; set; }
        public BorderColor BorderColor { get; set; }
        public int BorderOpacity { get; set; }
        public BackgroundColor BackgroundColor { get; set; }
        public double BackgroundOpacity { get; set; }
    }

    public class Gameplay
    {
        public HitErrorMeter HitErrorMeter { get; set; }
        public AimErrorMeter AimErrorMeter { get; set; }
        public Score Score { get; set; }
        public HpBar HpBar { get; set; }
        public ComboCounter ComboCounter { get; set; }
        public PPCounter PPCounter { get; set; }
        public HitCounter HitCounter { get; set; }
        public StrainGraph StrainGraph { get; set; }
        public KeyOverlay KeyOverlay { get; set; }
        public ScoreBoard ScoreBoard { get; set; }
        public Mods Mods { get; set; }
        public Boundaries Boundaries { get; set; }
        public bool ShowResultsScreen { get; set; }
        public int ResultsScreenTime { get; set; }
        public bool ResultsUseLocalTimeZone { get; set; }
        public bool ShowWarningArrows { get; set; }
        public bool ShowHitLighting { get; set; }
        public int FlashlightDim { get; set; }
        public string PlayUsername { get; set; }
        public bool UseLazerPP { get; set; }
    }

    public class Cursor
    {
        public bool UseSkinCursor { get; set; }
        public int Scale { get; set; }
        public int TrailScale { get; set; }
        public bool ForceLongTrail { get; set; }
        public int LongTrailLength { get; set; }
        public int LongTrailDensity { get; set; }
        public int TrailStyle { get; set; }
        public double Style23Speed { get; set; }
        public double Style4Shift { get; set; }
        public Colors Colors { get; set; }
        public bool EnableCustomTagColorOffset { get; set; }
        public int TagColorOffset { get; set; }
        public bool EnableTrailGlow { get; set; }
        public bool EnableCustomTrailGlowOffset { get; set; }
        public int TrailGlowOffset { get; set; }
        public bool ScaleToCS { get; set; }
        public int CursorSize { get; set; }
        public bool CursorExpand { get; set; }
        public bool ScaleToTheBeat { get; set; }
        public bool ShowCursorsOnBreaks { get; set; }
        public bool BounceOnEdges { get; set; }
        public double TrailEndScale { get; set; }
        public double TrailDensity { get; set; }
        public int TrailMaxLength { get; set; }
        public int TrailRemoveSpeed { get; set; }
        public double GlowEndScale { get; set; }
        public double InnerLengthMult { get; set; }
        public bool AdditiveBlending { get; set; }
        public bool CursorRipples { get; set; }
        public bool SmokeEnabled { get; set; }
    }

    public class Skin
    {
        public string CurrentSkin { get; set; }
        public string FallbackSkin { get; set; }
        public bool UseColorsFromSkin { get; set; }
        public bool UseBeatmapColors { get; set; }
        public Cursor Cursor { get; set; }
    }

    public class BaseColor
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
    }

    public class Colors
    {
        public bool EnableRainbow { get; set; }
        public int RainbowSpeed { get; set; }
        public BaseColor BaseColor { get; set; }
        public bool EnableCustomHueOffset { get; set; }
        public int HueOffset { get; set; }
        public bool FlashToTheBeat { get; set; }
        public int FlashAmplitude { get; set; }
        public int MandalaTexturesTrigger { get; set; }
        public double MandalaTexturesAlpha { get; set; }
        public Color Color { get; set; }
        public bool UseComboColors { get; set; }
        public List<ComboColor> ComboColors { get; set; }
        public bool UseSkinComboColors { get; set; }
        public bool UseBeatmapComboColors { get; set; }
        public Sliders Sliders { get; set; }
    }

    public class Quality
    {
        public int CircleLevelOfDetail { get; set; }
        public int PathLevelOfDetail { get; set; }
    }

    public class Snaking
    {
        public bool In { get; set; }
        public bool Out { get; set; }
        public bool OutFadeInstant { get; set; }
        public int DurationMultiplier { get; set; }
        public int FadeMultiplier { get; set; }
    }

    public class Sliders
    {
        public bool ForceSliderBallTexture { get; set; }
        public bool DrawEndCircles { get; set; }
        public bool DrawSliderFollowCircle { get; set; }
        public bool DrawScorePoints { get; set; }
        public bool SliderMerge { get; set; }
        public bool SliderDistortions { get; set; }
        public int BorderWidth { get; set; }
        public Quality Quality { get; set; }
        public Snaking Snaking { get; set; }
        public bool WhiteScorePoints { get; set; }
        public int ScorePointColorOffset { get; set; }
        public bool SliderBallTint { get; set; }
        public Border Border { get; set; }
        public Body Body { get; set; }
    }

    public class ComboColor
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
    }

    public class Border
    {
        public bool UseHitCircleColor { get; set; }
        public Color Color { get; set; }
        public bool EnableCustomGradientOffset { get; set; }
        public int CustomGradientOffset { get; set; }
    }

    public class Body
    {
        public bool UseHitCircleColor { get; set; }
        public Color Color { get; set; }
        public double InnerOffset { get; set; }
        public double OuterOffset { get; set; }
        public double InnerAlpha { get; set; }
        public double OuterAlpha { get; set; }
    }

    public class Objects
    {
        public bool DrawApproachCircles { get; set; }
        public bool DrawComboNumbers { get; set; }
        public bool DrawFollowPoints { get; set; }
        public bool LoadSpinners { get; set; }
        public bool ScaleToTheBeat { get; set; }
        public bool StackEnabled { get; set; }
        public Sliders Sliders { get; set; }
        public Colors Colors { get; set; }
    }

    public class SeizureWarning
    {
        public bool Enabled { get; set; }
        public int Duration { get; set; }
    }

    public class Dim
    {
        public int Intro { get; set; }
        public double Normal { get; set; }
        public double Breaks { get; set; }
    }

    public class Parallax
    {
        public double Amount { get; set; }
        public double Speed { get; set; }
    }

    public class Values
    {
        public int Intro { get; set; }
        public double Normal { get; set; }
        public double Breaks { get; set; }
    }

    public class Blur
    {
        public bool Enabled { get; set; }
        public Values Values { get; set; }
    }

    public class Triangles
    {
        public bool Enabled { get; set; }
        public bool Shadowed { get; set; }
        public bool DrawOverBlur { get; set; }
        public double ParallaxMultiplier { get; set; }
        public int Density { get; set; }
        public int Scale { get; set; }
        public int Speed { get; set; }
    }

    public class Background
    {
        public bool LoadStoryboards { get; set; }
        public bool LoadVideos { get; set; }
        public bool FlashToTheBeat { get; set; }
        public Dim Dim { get; set; }
        public Parallax Parallax { get; set; }
        public Blur Blur { get; set; }
        public Triangles Triangles { get; set; }
    }

    public class Logo
    {
        public bool DrawSpectrum { get; set; }
        public Dim Dim { get; set; }
    }

    public class Bloom
    {
        public bool Enabled { get; set; }
        public bool BloomToTheBeat { get; set; }
        public double BloomBeatAddition { get; set; }
        public int Threshold { get; set; }
        public double Blur { get; set; }
        public double Power { get; set; }
    }

    public class Playfield
    {
        public bool DrawObjects { get; set; }
        public bool DrawCursors { get; set; }
        public int Scale { get; set; }
        public bool OsuShift { get; set; }
        public int ShiftY { get; set; }
        public int ShiftX { get; set; }
        public bool ScaleStoryboardWithPlayfield { get; set; }
        public int LeadInTime { get; set; }
        public int LeadInHold { get; set; }
        public int FadeOutTime { get; set; }
        public SeizureWarning SeizureWarning { get; set; }
        public Background Background { get; set; }
        public Logo Logo { get; set; }
        public Bloom Bloom { get; set; }
    }

    public class Mover
    {
        [JsonPropertyName("Mover")]
        public string Moves { get; set; }
        public bool SliderDance { get; set; }
        public bool RandomSliderDance { get; set; }
    }

    public class Spinner
    {
        public string Mover { get; set; }
        public int Radius { get; set; }
    }

    public class Bezier
    {
        public int Aggressiveness { get; set; }
        public int SliderAggressiveness { get; set; }
    }

    public class Flower
    {
        public int AngleOffset { get; set; }
        public double DistanceMult { get; set; }
        public int StreamAngleOffset { get; set; }
        public int LongJump { get; set; }
        public double LongJumpMult { get; set; }
        public bool LongJumpOnEqualPos { get; set; }
    }

    public class HalfCircle
    {
        public int RadiusMultiplier { get; set; }
        public int StreamTrigger { get; set; }
    }

    public class Spline
    {
        public bool RotationalForce { get; set; }
        public bool StreamHalfCircle { get; set; }
        public bool StreamWobble { get; set; }
        public double WobbleScale { get; set; }
    }

    public class Momentum
    {
        public bool SkipStackAngles { get; set; }
        public bool StreamRestrict { get; set; }
        public int DurationMult { get; set; }
        public int DurationTrigger { get; set; }
        public double StreamMult { get; set; }
        public int RestrictAngle { get; set; }
        public int RestrictArea { get; set; }
        public bool RestrictInvert { get; set; }
        public double DistanceMult { get; set; }
        public double DistanceMultOut { get; set; }
    }

    public class ExGon
    {
        public int Delay { get; set; }
    }

    public class Linear
    {
        public bool WaitForPreempt { get; set; }
        public int ReactionTime { get; set; }
        public bool ChoppyLongObjects { get; set; }
    }

    public class Pippi
    {
        public double RotationSpeed { get; set; }
        public double RadiusMultiplier { get; set; }
        public int SpinnerRadius { get; set; }
    }

    public class MoverSettings
    {
        public List<Bezier> Bezier { get; set; }
        public List<Flower> Flower { get; set; }
        public List<HalfCircle> HalfCircle { get; set; }
        public List<Spline> Spline { get; set; }
        public List<Momentum> Momentum { get; set; }
        public List<ExGon> ExGon { get; set; }
        public List<Linear> Linear { get; set; }
        public List<Pippi> Pippi { get; set; }
    }

    public class CursorDance
    {
        public List<Mover> Movers { get; set; }
        public List<Spinner> Spinners { get; set; }
        public bool ComboTag { get; set; }
        public bool Battle { get; set; }
        public bool DoSpinnersTogether { get; set; }
        public bool TAGSliderDance { get; set; }
        public MoverSettings MoverSettings { get; set; }
    }

    public class Knockout
    {
        public int Mode { get; set; }
        public string ExcludeMods { get; set; }
        public string HideMods { get; set; }
        public int MaxPlayers { get; set; }
        public int BubbleMinimumCombo { get; set; }
        public bool RevivePlayersAtEnd { get; set; }
        public bool LiveSort { get; set; }
        public string SortBy { get; set; }
        public bool HideOverlayOnBreaks { get; set; }
        public int MinCursorSize { get; set; }
        public int MaxCursorSize { get; set; }
        public bool AddDanser { get; set; }
        public string DanserName { get; set; }
    }

    public class BlendWeights
    {
        public bool UseManualWeights { get; set; }
        public string ManualWeights { get; set; }
        public int AutoWeightsID { get; set; }
        public double GaussWeightsMult { get; set; }
    }

    public class MotionBlur
    {
        public bool Enabled { get; set; }
        public int OversampleMultiplier { get; set; }
        public int BlendFrames { get; set; }
        public BlendWeights BlendWeights { get; set; }
    }

    public class Recording
    {
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FPS { get; set; }
        public int EncodingFPSCap { get; set; }
        public string Encoder { get; set; }
        public string EncoderOptions { get; set; }
        public string Profile { get; set; }
        public string Preset { get; set; }
        public string PixelFormat { get; set; }
        public string Filters { get; set; }
        public string AudioCodec { get; set; }
        public string AudioOptions { get; set; }
        public string AudioFilters { get; set; }
        public string OutputDir { get; set; }
        public string Container { get; set; }
        public bool ShowFFmpegLogs { get; set; }
        public MotionBlur MotionBlur { get; set; }
    }

    public class DanserSettings
    {
        public General General { get; set; }
        public Graphics Graphics { get; set; }
        public Audio Audio { get; set; }
        public Input Input { get; set; }
        public Gameplay Gameplay { get; set; }
        public Skin Skin { get; set; }
        public Cursor Cursor { get; set; }
        public Objects Objects { get; set; }
        public Playfield Playfield { get; set; }
        public CursorDance CursorDance { get; set; }
        public Knockout Knockout { get; set; }
        public Recording Recording { get; set; }
    }


}
