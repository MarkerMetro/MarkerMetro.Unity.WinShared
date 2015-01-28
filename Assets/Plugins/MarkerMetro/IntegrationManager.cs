using System;
using UnityEngine;

namespace Assets.Plugins.MarkerMetro
{

    /**
     * This singleton is an entry point for Marker Metro MonoBehaviours.
     * It creates a Game Object that is not destroyed between scene loads.
     * It offers an easy way to hook up scripts from the game code or from the UI thread and
     * contains Windows-related events.
     */
    public sealed class IntegrationManager : MonoBehaviour
    {
        private static IntegrationManager instance = null;
        public static IntegrationManager Instance
        {
            get
            {
                if (instance == null) Init();
                return instance;
            }
        }

        // Delete this and change to Action when this bug is solved:
        // http://fogbugz.unity3d.com/default.asp?609123_4r6pfprov9jibi2g
        public delegate void action();

        /**
            * Fired every time the Windows key is down.
            */
        public event action OnWindowsKeyDown;

        /**
            * Fired every time the Windows key is down.
            */
        public event action OnBackspaceKeyDown;

        /**
            * Fired every MonoBehaviour Update.
            */
        public event action OnUpdate;

        /**
            * Creates a Game Object and adds the IntegrationManager in it.
            */
        public static void Init()
        {
            if (instance != null) return;

            if (!IsWinRT())
                throw new PlatformNotSupportedException();

            GameObject go = new GameObject("WindowsIntegrationManager");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<IntegrationManager>();
        }

        /**
            * Instantiates a MonoBehaviour of the given type inside the Manager's Game Object.
            */
        public void AddComponent<T>() where T : MonoBehaviour
        {
            gameObject.AddComponent<T>();
        }

        /**
            * Destroy the singleton and it's GameObject.
            */
        public void Destroy()
        {
            Destroy(gameObject);
            instance = null;
        }

        private static bool IsWinRT()
        {
            return
                Application.platform == RuntimePlatform.MetroPlayerARM ||
                Application.platform == RuntimePlatform.MetroPlayerX64 ||
                Application.platform == RuntimePlatform.MetroPlayerX86 ||
                Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WP8Player;
        }

        private void Update()
        {
            if (OnUpdate != null)
                OnUpdate();

            if ((Input.GetKeyDown(KeyCode.LeftWindows) ||
                Input.GetKeyDown(KeyCode.RightWindows) ||
                Input.GetKeyDown(KeyCode.LeftApple) ||
                Input.GetKeyDown(KeyCode.RightApple))
                && OnWindowsKeyDown != null)
                OnWindowsKeyDown();

            if (Input.GetKeyDown(KeyCode.Backspace) && OnBackspaceKeyDown != null)
                OnBackspaceKeyDown();
        }
    }


}