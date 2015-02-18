using System;
using UnityEngine;

namespace MarkerMetro.Unity.WinShared
{
    /// <summary>
    /// Provides configuration and settings control for the game
    /// </summary>
    /// <remarks>
    /// A gateway class between the application and Unity game
    /// </remarks>
    public class GameController
    {
        private static GameController _instance;
        private static readonly object _sync = new object();

        private IGameConfig _gameConfig;
        private IGameSettings _gameSettings;

        private GameController() { }

        public static GameController Instance
        {
            get
            {
                lock (_sync)
                {
                    if (_instance == null)
                    {
                        _instance = new GameController();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Game Configuration required by the Unity game
        /// </summary>
        public IGameConfig GameConfig
        {
            get { return _gameConfig; }
            set { _gameConfig = value; }
        }
        
        /// <summary>
        /// Game Settings control provided by the Unity game
        /// </summary>
        public IGameSettings GameSettings
        {
            get { return _gameSettings; }
            set { _gameSettings = value; }
        }

    }
}