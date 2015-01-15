using UnityEngine;
using System.Collections;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;


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
                    _instance = new FeaturesManager();
                return _instance;
            }
        }

        private FeaturesManager()
        {
            _settings = Settings.Load();
        }

        public bool IsRemindersEnabled
        {
            get
            {
                return _settings.RemindersEnabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.RemindersEnabled = value;
                _settings.Save();
            }
#endif
        }

        public bool IsAudioCharmEnabled
        {
            get
            {
                return _settings.AudioCharmEnabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.AudioCharmEnabled = value;
                _settings.Save();
            }
#endif
        }

        public bool IsIapEnabled
        {
            get
            {
                return _settings.IapEnabled;
            }
#if UNITY_EDITOR
            set
            {
                _settings.IapEnabled = value;
                _settings.Save();
            }
#endif
        }

        /// <summary>
        /// Returns true if any of the features that belong to the Game Settings charm are enabled.
        /// </summary>
        public bool IsGameSettingsEnabled
        {
            get 
            { 
                return IsAudioCharmEnabled || IsRemindersEnabled;
            }
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
            const string _filename = "WinSharedSettings";
            const string _path = ".\\Assets\\Scripts\\MarkerMetro\\Resources\\" + _filename + ".xml";

            public bool RemindersEnabled;
            public bool AudioCharmEnabled;
            public bool IapEnabled;

            /// <summary>
            /// Save is only available for Unity Editor. Use FeaturesManager to access and save settings.
            /// </summary>
            internal void Save()
            {
#if UNITY_EDITOR
                var serializer = new XmlSerializer(typeof(Settings));
                var encoding = Encoding.GetEncoding("UTF-8");
                using (var stream = new StreamWriter(_path, false, encoding))
                {
                    serializer.Serialize(stream, this);
                }
#else
            throw new System.InvalidOperationException("Setting a Feature is only possible via Unity Editor");
#endif
            }

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