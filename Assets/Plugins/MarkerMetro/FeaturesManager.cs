using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System;
using System.Linq;

using Assets.Plugins.MarkerMetro;
using System.Collections.Generic;


namespace MarkerMetro.Unity.WinShared.Tools
{
    /// <summary>
    /// Provides information on which Feature the developer has enabled to this particular app.
    /// The settings are defined in a xml file that resides in a Resources folder.
    /// In Unity Editor, this class also provides a way to set these settings. Setting a 
    /// property automatically saves it to the file.
    /// </summary>
    public class FeaturesManager
    {
        static FeaturesManager _instance = null;
        Settings _settings = null;
        public static FeaturesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FeaturesManager();
                }
                return _instance;
            }
        }

        private FeaturesManager()
        {
            _settings = Settings.Load();
        }

        public bool IsSettingsNotificationsOnOffEnabled
        {
            get
            {
                return _settings.SettingsNotificationsOnOffEnabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.SettingsNotificationsOnOffEnabled = value;
                _settings.Save();
            }
#endif
        }

        public bool IsSettingsMusicFXOnOffEnabled
        {
            get
            {
                return _settings.SettingsMusicFXOnOffEnabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.SettingsMusicFXOnOffEnabled = value;
                _settings.Save();
            }
#endif
        }

        public bool IsIapDisclaimerEnabled
        {
            get
            {
                return _settings.IapDisclaimerEnabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.IapDisclaimerEnabled = value;
                _settings.Save();
            }
#endif
        }

        /// <summary>
        /// Returns true if any of the features that belong to the Game Settings are enabled.
        /// </summary>
        public bool IsGameSettingsEnabled
        {
            get
            {
                return IsSettingsMusicFXOnOffEnabled || IsSettingsNotificationsOnOffEnabled;
            }
        }

        public bool IsExceptionLoggingEnabled
        {
            get
            {
                return _settings.ExceptionLoggingSettings.Enabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.ExceptionLoggingSettings.Enabled = value;
                _settings.Save();
            }
#endif
        }

        public string ExceptionLoggingApiKey
        {
            get
            {
                return _settings.ExceptionLoggingSettings.ApiKey;
            }
#if UNITY_EDITOR
            set
            {
                _settings.ExceptionLoggingSettings.ApiKey = value;
                _settings.Save();
            }
#endif
        }

        // Unity doesn't support ISet :(
        public HashSet<Environment> ExceptionLoggingAutoLogEnvironments
        {
            get
            {
                // Returning new instance since List<>.AsReadOnly() is not supported.
                return new HashSet<Environment>(_settings.ExceptionLoggingSettings.AutoLogEnvironments);
            }
#if UNITY_EDITOR
            set
            {
                _settings.ExceptionLoggingSettings.AutoLogEnvironments = value;
                _settings.Save();
            }
#endif
        }

        public bool IsExceptionLoggingEnabledForEnvironment(Environment env)
        {
            return ExceptionLoggingAutoLogEnvironments.Any(exceptionEnv => exceptionEnv == env);
        }

        public bool IsExceptionLoggingEnabledForCurrentEnvironment
        {
            get
            {
                return IsExceptionLoggingEnabledForEnvironment(DeviceInformation.GetEnvironment());
            }
        }

        public bool IsMemoryDisplayEnabled
        {
            get
            {
                return _settings.MemoryDisplaySettings.Enabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.MemoryDisplaySettings.Enabled = value;
                _settings.Save();
            }
#endif
        }

        public bool IsMemoryDisplayEnabledForCurrentEnvironment
        {
            get
            {
                return _settings.MemoryDisplaySettings.IsEnabledForCurrentEnvironment;
            }
        }

        public bool IsMemoryDisplayEnabledForEnvironment(Environment env)
        {
            return _settings.MemoryDisplaySettings.Enabled && 
                _settings.MemoryDisplaySettings.IsEnabledForEnvironment(env);
        }

        public HashSet<Environment> MemoryDisplayEnvironments
        {
            get
            {
                // Returning new instance since List<>.AsReadOnly() is not supported.
                return new HashSet<Environment>(_settings.MemoryDisplaySettings.Environments);
            }
#if UNITY_EDITOR
            set
            {
                _settings.MemoryDisplaySettings.Environments = value;
                _settings.Save();
            }
#endif
        }

        public override string ToString()
        {
            return _settings.ToString();
        }

        // This class is a simple container for the xml data. It's separated from the main
        // class to not expose the actual settings setters to other classes, mainly to
        // classes from WindowsSolution (serializable classes can't have internal accessors).
        // Having accessors in the manager class also allows us to save when settings something.
        public class Settings
        {
            public class ExceptionLogging
            {
                public bool Enabled = true;
                public string ApiKey = "";
                public HashSet<Environment> AutoLogEnvironments;

                public ExceptionLogging()
                {
                    AutoLogEnvironments = new HashSet<Environment>();
                }
            }

            public class EnvironmentDependentSettings
            {
                public bool Enabled = true;
                public HashSet<Environment> Environments;
                public EnvironmentDependentSettings()
                {
                    Environments = new HashSet<Environment>();
                }

                public bool IsEnabledForCurrentEnvironment
                {
                    get 
                    {
                        return IsEnabledForEnvironment(DeviceInformation.GetEnvironment()); 
                    }
                }
                public bool IsEnabledForEnvironment(Environment env)
                {
                    return Enabled && Environments.Any(exceptionEnv => exceptionEnv == env);
                }

            }

            const string _filename = "WinSharedSettings";
            const string _path = ".\\Assets\\MarkerMetro\\Resources\\" + _filename + ".xml";

            public bool SettingsNotificationsOnOffEnabled;
            public bool SettingsMusicFXOnOffEnabled;
            public bool IapDisclaimerEnabled;
     		public ExceptionLogging ExceptionLoggingSettings = new ExceptionLogging();
            public EnvironmentDependentSettings MemoryDisplaySettings = new EnvironmentDependentSettings();
       
#if UNITY_EDITOR
            /// <summary>
            /// Save is only available for Unity Editor. Use FeaturesManager to access and save settings.
            /// </summary>
            internal void Save()
            {
                var serializer = new XmlSerializer(typeof(Settings));
                var encoding = Encoding.GetEncoding("UTF-8");
                using (var stream = new StreamWriter(_path, false, encoding))
                {
                    serializer.Serialize(stream, this);
                }
            }
#endif

            internal static Settings Load()
            {
                TextAsset textAsset = (TextAsset)Resources.Load(_filename);
                var serializer = new XmlSerializer(typeof(Settings));
                using (var stream = new MemoryStream(textAsset.bytes))
                {
                    return serializer.Deserialize(stream) as Settings;
                }
            }

            public override string ToString()
            {
                return ((TextAsset)Resources.Load(_filename)).text;
            }
        }
    }
}