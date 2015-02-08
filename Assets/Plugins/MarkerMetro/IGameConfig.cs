using System.Collections.Generic;

namespace MarkerMetro.Unity.WinShared
{
    /// <summary>
    /// Defines configuration values required by the game
    /// </summary>
    public interface IGameConfig
    {
        BuildConfig CurrentBuildConfig { get; }
        string FacebookAppId { get; }
        bool ExceptionLoggingEnabled { get; }
        string ExceptionLoggingApiKey { get; }
        List<BuildConfig> ExceptionLoggingBuildConfigs { get; }
        bool ExceptionLoggingAllowed { get; }
        bool DisplayMemoryUsageEnabled { get; }
        List<BuildConfig> DisplayMemoryUsageBuildConfigs { get; }
        bool DisplayMemoryUsageAllowed { get; }
    }
}
