using GithubHelper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MambaManager;

// https://github.com/mamba-org
public partial class Mamba
{
    public async Task<MambaResponse> GetKnownEnvironmentsAsync()
    {
        var startInfo = GetProcessStartInfoTemplate();
        startInfo.ArgumentList.Add("env");
        startInfo.ArgumentList.Add("list");

        var data = await ExecuteCommandAsync<MambaResponse>(startInfo);

        return data;
    }

    public async Task<MambaEnv> GetEnvironmentAsync(string name)
    {
        var known = await GetKnownEnvironmentsAsync();

        var envNames = known.Envs.Select(item => Path.GetFileName(item));

        var targetEnv = envNames.FirstOrDefault(found =>
        {
            return found == name;
        });

        if (targetEnv is null)
        {
            return null;
        }

        var envData = await GetActivationScriptAsync(targetEnv);
        return envData;
    }

    public async Task<MambaEnv> GetOrCreateEnvironmentAsync(string name, params string[] extras)
    {
        var alreadyTried = false;

    retry:

        var envData = await GetEnvironmentAsync(name);

        if (envData is not null)
        {
            return envData;
        }

        if (alreadyTried)
        {
            throw new Exception("Could not get or create the requested env");
        }

        alreadyTried = true;

        var data = CreateEnvironmentAsync(name, extras);

        goto retry;
    }

    public async Task<MambaEnv> GetActivationScriptAsync(string name)
    {
        var startInfo = GetProcessStartInfoTemplate();

        startInfo.ArgumentList.Add("shell");
        startInfo.ArgumentList.Add("activate");
        startInfo.ArgumentList.Add("--name");
        startInfo.ArgumentList.Add(name);
        startInfo.ArgumentList.Add("--shell");
        startInfo.ArgumentList.Add("xonsh");

        var activateScript = await ExecuteCommandAsync(startInfo);

        var data = new MambaEnv(activateScript, this);

        return data;
    }

#warning to do handle creating env that already exist properly
    public async Task<MambaResponse> CreateEnvironmentAsync(string name, params string[] extras)
    {
        var startInfo = GetProcessStartInfoTemplate();

        startInfo.AddArg("create");

        startInfo.AddArg("--name");
        startInfo.AddArg(name);

        startInfo.AddArg("--channel");
        startInfo.AddArg(Channel);

        foreach (var item in extras)
        {
            startInfo.AddArg(item);
        }

        var data = await ExecuteCommandAsync<MambaResponse>(startInfo);

        return data;
    }

#warning to do handle deleting env that does not exist properly 
    public async Task<MambaResponse> DeleteEnvironmentAsync(string name)
    {
        var startInfo = GetProcessStartInfoTemplate();

        startInfo.AddArg("env");

        startInfo.AddArg("remove");
        startInfo.AddArg("--name");
        startInfo.AddArg(name);

        var data = await ExecuteCommandAsync<MambaResponse>(startInfo);

        return data;
    }
}
