using Python.Runtime;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MambaManager;

public class MambaEnv
{
    public Mamba Mamba { get; private set; }
    public string EnvPath { get; private set; }
    public string EnvName { get; private set; }
    public string CondaPrefix { get; private set; }
    public string CondaShLvl { get; private set; }
    public string CondaDefaultEnv { get; private set; }
    public string CondaPromptModifier { get; private set; }

    internal MambaEnv(string raw, Mamba mamba)
    {
        Mamba = mamba;

        var lines = raw.Split('$', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            var parts = line.Split(" = \"", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var key = parts[0];
            var value = parts[1][..^1];
            if (key == "PATH")
            {
                EnvPath = value;
            }
            else if (key == "CONDA_PREFIX")
            {
                EnvName = Path.GetFileName(value);
                CondaPrefix = value;
            }
            else if (key == "CONDA_SHLVL")
            {
                CondaShLvl = value;
            }
            else if (key == "CONDA_DEFAULT_ENV")
            {
                CondaDefaultEnv = value;
            }
            else if (key == "CONDA_PROMPT_MODIFIER")
            {
                CondaPromptModifier = value;
            }
        }
    }

    public string GetPythonPath()
    {
#warning add custom pythonpath additions
        var thingsToAdd = new string?[]
        {
            CondaPrefix,
            Path.Combine(CondaPrefix, "DLLs"),
            Path.Combine(CondaPrefix, "Lib"),
            Path.Combine(CondaPrefix, "Lib", "site-packages"),
            Environment.ProcessPath,
            AppDomain.CurrentDomain.BaseDirectory,
        };

        var sb = new StringBuilder();

        for (int i = 0; i < thingsToAdd.Length; i++)
        {
            var current = thingsToAdd[i];

            if (current is null)
            {
                continue;
            }

            if (sb.Length > 0)
            {
                sb.Append(';');
            }

            if (!Path.IsPathRooted(current))
            {
                current = Path.GetFullPath(current);
            }

            sb.Append(current);
        }

        return sb.ToString();
    }

    public void InitializePythonBridge()
    {
        Environment.SetEnvironmentVariable("PATH", EnvPath, EnvironmentVariableTarget.Process);

        var pythonDll = Path.Combine(CondaPrefix, GetPythonDll());

        Runtime.PythonDLL = pythonDll;

        PythonEngine.DebugGIL = true;

        PythonEngine.PythonHome = CondaPrefix;
        Environment.SetEnvironmentVariable("PYTHONHOME", CondaPrefix, EnvironmentVariableTarget.Process);

        var pythonPath = GetPythonPath();
        PythonEngine.PythonPath = pythonPath;
        Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath, EnvironmentVariableTarget.Process);


        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    private string[] GetPossiblePythonDlls()
    {
        string binaryExtension;
        if (OperatingSystem.IsWindows()) binaryExtension = "dll";
        else if (OperatingSystem.IsLinux()) binaryExtension = "so";
        else if (OperatingSystem.IsMacOS()) binaryExtension = "dylib";
        else throw new NotImplementedException($"'{nameof(binaryExtension)}' is not implemented for '{RuntimeInformation.OSDescription}'");

        var envFiles = Directory.GetFiles(CondaPrefix, $"python*.{binaryExtension}");

        return envFiles;
    }

    private string GetPythonDll()
    {
        var possibles = GetPossiblePythonDlls();

#warning to do optimize this, it sucks but i am lazy
        var pickedDll = possibles.OrderByDescending(s => s.Length).First();

        return pickedDll;
    }

    public async Task<string> Test()
    {
        var startInfo = GetProcessStartInfoTemplate();

        startInfo.AddArg("run");
        startInfo.AddArg("tree");
        startInfo.AddArg("/A");
        startInfo.AddArg("/F");

        var output = await Mamba.ExecuteCommandAsync(startInfo);
        return output;
    }

    public async Task<string> InstallRequierimentsMambaAsync(string requierimentsFile)
    {
        var startInfo = GetProcessStartInfoTemplate();

        startInfo.AddArg("install");

        startInfo.AddArg("--channel");
        startInfo.AddArg(Mamba.Channel);

        startInfo.AddArg("--file");
        startInfo.AddArg(requierimentsFile);

        var output = await Mamba.ExecuteCommandAsync(startInfo);

        return output;
    }

    public async Task<string> InstallRequierimentsPipAsync(string requierimentsFile = "requirements.txt")
    {
        var startInfo = GetProcessStartInfoTemplate();

        startInfo.AddArg("run");

        startInfo.AddArg("pip");
        startInfo.AddArg("install");
        startInfo.AddArg("--requirement");
        startInfo.AddArg(requierimentsFile);

        var output = await Mamba.ExecuteCommandAsync(startInfo);

        return output;
    }

    internal ProcessStartInfo GetProcessStartInfoTemplate()
    {
        var startInfo = Mamba.GetProcessStartInfoTemplate();

        //startInfo.AddArg("--root-prefix");
        //var root = CondaPrefix[..^(EnvName.Length + 1)];
        //startInfo.AddArg(root);

        startInfo.AddArg("--name");
        startInfo.AddArg(EnvName);

        return startInfo;
    }
}
