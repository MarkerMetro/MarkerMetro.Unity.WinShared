using MarkerMetro.Unity.WinShared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UnityProject.Config
{
    /// <summary>
    /// All app configuration provided on start up
    /// </summary>
    public class AppConfig : IGameConfig
    {

        static AppConfig _instance = null;

        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppConfig();
                }
                return _instance;
            }
        }

        public BuildConfig CurrentBuildConfig
        {
            get
            {
#if QA
            return BuildConfig.QA;
#elif DEBUG
            return BuildConfig.Debug;
#else
            return BuildConfig.Master;
#endif
            }
        }

        public string FacebookAppId
        {
            get {return "540541885996234";} // microsoft fb sdk test app
        }

        public bool IapDisclaimerEnabled
        {
            get { return true; } 
        }

        public bool MusicFXControlEnabled
        {
            get { return true; }
        }

        public bool NoticationsControlEnabled
        {
            get { return true; }
        }

        public bool ExceptionLoggingEnabled
        {
            get { return true; }
        }

        public string ExceptionLoggingApiKey
        {
            get { return ""; } // set your api key here
        }

        public List<BuildConfig> ExceptionLoggingBuildConfigs
        {
            get { return new List<BuildConfig>() { BuildConfig.Master }; }
        }

        public bool ExceptionLoggingAllowed
        {
            get
            {
                return ExceptionLoggingEnabled && ExceptionLoggingBuildConfigs.Any(e => e == CurrentBuildConfig);
            }
        }

        public bool DisplayMemoryUsageEnabled
        {
            get { return true; }
        }

        public List<BuildConfig> DisplayMemoryUsageBuildConfigs
        {
            get { return new List<BuildConfig>() { BuildConfig.Debug, BuildConfig.QA }; }
        }

        public bool DisplayMemoryUsageAllowed
        {
            get
            {
                return DisplayMemoryUsageEnabled && DisplayMemoryUsageBuildConfigs.Any(e => e == CurrentBuildConfig);
            }
        }

    }
}
