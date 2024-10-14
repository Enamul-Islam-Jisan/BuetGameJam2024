using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public static class UnityServiceInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetEnvironmentName("development");
        UnityServices.InitializeAsync(options);
    }
}