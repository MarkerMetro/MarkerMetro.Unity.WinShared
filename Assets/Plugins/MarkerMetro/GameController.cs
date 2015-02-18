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

        public void Init(IGameSettings gameSettings)
        {
            if (_gameSettings != null)
            {
                throw new InvalidOperationException("GameSettings has already been initialized");
            }
            _gameSettings = gameSettings;
        }

        public void Init(IGameConfig gameConfig)
        {
            if (_gameConfig != null)
            {
                throw new InvalidOperationException("GameConfig has already been initialized");
            }
            _gameConfig = gameConfig;
        }

        /// <summary>
        /// Game Configuration required by the Unity game
        /// </summary>
        public IGameConfig GameConfig
        {
            get { return _gameConfig; }
        }
        
        /// <summary>
        /// Game Settings control provided by the Unity game
        /// </summary>
        public IGameSettings GameSettings
        {
            get { return _gameSettings; }
        }

    }
}