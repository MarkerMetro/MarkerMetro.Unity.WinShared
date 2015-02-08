using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkerMetro.Unity.WinShared
{

    /// <summary>
    /// Provides game configuration values
    /// </summary>
    public static class GameConfig
    {

        private static IGameConfig _instance;
        private static readonly object _sync = new object();

        /// <summary>
        /// implemented on app side to return all game configuration
        /// </summary>
        public static Func<IGameConfig> DoGetGameConfig;

        public static IGameConfig Instance
        {
            get
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = DoGetGameConfig != null ? DoGetGameConfig() : null;
                }
                return _instance;
            }
        }

    }
}
