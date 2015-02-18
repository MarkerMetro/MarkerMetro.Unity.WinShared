using System;
using UnityEngine;

namespace MarkerMetro.Unity.WinShared
{
    /// <summary>
    /// Allows for control over the game
    /// </summary>
    public class GameController
    {
        private static GameController _instance;
        private static readonly object _sync = new object();

        private IGameConfig _gameConfig;
        private IGameSettings _gameSetting;

        public static event Action AppCrashTest;

        public static void InitConfig(IGameConfig gameConfig)
        {
            if (gameConfig == null)
            {
                throw new ArgumentNullException("gameConfig");
            }
            lock (_sync)
            {

                // create singleton
                if (_instance == null)
                {
                    _instance = new GameController();
                }
                if (_instance._gameConfig != null)
                {
                    throw new InvalidOperationException("GameController.InitConfig has already been invoked and can only be called once");
                }
                _instance._gameConfig = gameConfig;
            }
        }

        public static void InitSettings(IGameSettings gameSetting)
        {
            if (gameSetting == null)
            {
                throw new ArgumentNullException("gameSetting");
            }
            lock (_sync)
            {

                // create singleton
                if (_instance == null)
                {
                    _instance = new GameController();
                }
                if (_instance._gameSetting != null)
                {
                    throw new InvalidOperationException("GameController.InitSettings has already been invoked and can only be called once");
                }

                _instance._gameSetting = gameSetting;

                // initialize dependencies
                ExceptionLogger.Init();
            }
        }

        public static GameController Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("GameContoller.Instance is null, did you call InitConfig or InitSettings first?");
                }
                return _instance;
            }
        }

        public IGameConfig GameConfig
        {
            get { return _gameConfig; }
        }

        public IGameSettings GameSettings
        {
            get { return _gameSetting; }
        }

        public void DoAppCrashTest ()
        {
            if (AppCrashTest != null)
            {
                AppCrashTest();
            }
        }
    }
}