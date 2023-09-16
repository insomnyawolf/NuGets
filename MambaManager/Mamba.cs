using GithubHelper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MambaManager;

// https://github.com/mamba-org
// https://www.imranabdullah.com/2021-08-21/Conda-and-Mamba-Commands-for-Managing-Virtual-Environments
public partial class Mamba
{
    private Mamba() { }

    const string MambaReleasesRepo = "https://github.com/mamba-org/micromamba-releases";

    private static Mamba InstanceField;

    private string ExecutableName { get; init; }
    private string ExecutablePath { get; init; }
    private string AvailableVersion { get; set; }
    internal string Channel { get; private set; } = "conda-forge";

    public static async Task<Mamba> GetAsync()
    {
        if (InstanceField is not null)
        {
            return InstanceField;
        }

        var executableName = GetExecutableName();

        InstanceField = new Mamba()
        {
            ExecutableName = executableName,
            ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, executableName),
        };

        await InstanceField.TryUpdate();

        return InstanceField;
    }

    private static string GetExecutableName()
    {
        var os = Environment.OSVersion.Platform;
        var architecture = RuntimeInformation.OSArchitecture;

        var osString = os switch
        {
            PlatformID.Win32NT => "win",
            PlatformID.Unix => "linux",
            PlatformID.MacOSX => "osx",
            _ => throw new PlatformNotSupportedException($"{os} {architecture}"),
        };

        var architectureString = architecture switch
        {
            Architecture.X64 => "64",
            Architecture.Arm64 => os == PlatformID.Unix ? "aarch64" : "arm64",
            Architecture.Ppc64le => "ppc64le",
            _ => throw new PlatformNotSupportedException($"{os} {architecture}"),
        };

        var extension = os switch
        {
            PlatformID.Win32NT => "exe",
            PlatformID.Unix => "sh",
            PlatformID.MacOSX => "pkg",
            _ => throw new PlatformNotSupportedException($"{os} {architecture}"),
        };

        return $"micromamba-{osString}-{architectureString}";
    }

    private async Task TryUpdate()
    {
        await InstanceField.LoadLocalVersionAsync();

        var release = await GithubTools.GetLastestReleaseAvailable(MambaReleasesRepo);

        if (AvailableVersion is null)
        {
            // No version available, we need to download it
            goto forceDownload;
        }

        if (release.TagName.StartsWith(AvailableVersion))
        {
            // Already Up To Date
            return;
        }

    forceDownload:

#warning consider micromamba-win-64.exe self-update --channel conda-forge
        var platformBinary = release.Assets.FirstOrDefault(item => item.Name == ExecutableName);

        var httpclient = new HttpClient();
        var response = await httpclient.GetAsync(platformBinary.BrowserDownloadUrl);

        var targetFileStream = File.Open(ExecutablePath, FileMode.OpenOrCreate);
        var responseStream = await response.Content.ReadAsStreamAsync();
        await responseStream.CopyToAsync(targetFileStream);
        targetFileStream.SetLength(responseStream.Length);
        await targetFileStream.FlushAsync();
        await targetFileStream.DisposeAsync();

        await LoadLocalVersionAsync();
    }

    private async Task LoadLocalVersionAsync()
    {
        if (!File.Exists(ExecutablePath))
        {
            AvailableVersion = null;
            return;
        }

        var startInfo = GetProcessStartInfoTemplate();
        startInfo.ArgumentList.Add("--version");

        var data = await ExecuteCommandAsync(startInfo);

        data = data.Trim();

        AvailableVersion = data;
    }



    internal static async Task<TResponse> ExecuteCommandAsync<TResponse>(ProcessStartInfo processStartInfo)
    {
        var dataRaw = await ExecuteCommandAsync(processStartInfo);

        var data = JsonSerializer.Deserialize<TResponse>(dataRaw);

        return data;
    }

    internal static async Task<string> ExecuteCommandAsync(ProcessStartInfo processStartInfo)
    {
        var debugArguments = string.Join(' ', processStartInfo.ArgumentList);

        var proc = new Process
        {
            StartInfo = processStartInfo,
        };

        proc.Start();

        proc.StandardInput.Close();

        await proc.WaitForExitAsync();

        var data = await proc.StandardOutput.ReadToEndAsync();
        var error = await proc.StandardError.ReadToEndAsync();

        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new Exception(error);
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            return "null";
        }

        return data;
    }

    internal ProcessStartInfo GetProcessStartInfoTemplate()
    {
        var temp = new ProcessStartInfo
        {
            FileName = ExecutablePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            CreateNoWindow = true,
        };

        var args = temp.ArgumentList;

        args.Add("--json");
        args.Add("--yes");
        //args.Add("--quiet");

        return temp;
    }
}
