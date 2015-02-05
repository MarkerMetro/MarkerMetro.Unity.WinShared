﻿using System;
using System.Collections;
using UnityEngine;

using MarkerMetro.Unity.WinIntegration;
using MarkerMetro.Unity.WinIntegration.LocalNotifications;
using MarkerMetro.Unity.WinIntegration.Facebook;
using MarkerMetro.Unity.WinIntegration.Logging;
using MarkerMetro.Unity.WinShared.Tools;

#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FBNative;
#else
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FB;
#endif

public class GUIMain : MonoBehaviour {

    const int FacebookWindowID = 0;
    const int ExceptionLoggingWindowID = 1;
    const int InfoWindowID = 2;
    const int PlatformIntegrationWindowID = 3;
    const int GameInfoWindowID = 4;
    const int GameMenuWindowID = 5;

    const float Offset = 10f;
    const float WindowWidth = 200;
    const float ButtonHeight = 40f;
    const string Separator = "---------------------------------------------";

    GameMaster _gameMasterScript;
    bool _showInfo = false;
    float _facebookMenuHeight;

    void Start ()
    {
        GameObject gameMasterObject = GameObject.Find("GameMaster");
        _gameMasterScript = gameMasterObject.GetComponent<GameMaster>();
    }

    void Update ()
    {
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MarkerMetro.Unity.WinIntegration.Helper.Instance.ShowDialog("Are you sure you want to quit?", "Quit Confirm", (okPressed) =>
            {
                if (okPressed)
                {
                    Application.Quit();
                }
            }, "Yes", "No");
        }
#endif
    }

	void OnGUI()
	{
        if (_gameMasterScript.State == GameMaster.GAME_STATE.GS_START)
        {
            // Facebook menu.
            Rect facebookScreenRect = GUILayout.Window(FacebookWindowID, new Rect(Offset, Offset, 0, 0), FacebookIntegrationGUI, "Facebook Integration", GUILayout.MinWidth(WindowWidth));

            // OnGUI will run twice every frame, once for EventType.Layout, and once for EventType.Repaint.
            // The rect returned by GUILayout.Window is correct only for EventType.Repaint.
            if (Event.current.type == EventType.Repaint)
            {
                _facebookMenuHeight = facebookScreenRect.height;
            }

            // Only show the exception logging menu when this feature is enabled.
            if (FeaturesManager.Instance.IsExceptionLoggingEnabled)
            {
                // Exception logging menu.
                GUILayout.Window(ExceptionLoggingWindowID, new Rect(Offset, Offset * 2 + _facebookMenuHeight, 0, 0), ExceptionLogginGUI, "Exception Logging", GUILayout.MinWidth(WindowWidth));
            }

            if (_showInfo)
            {
                // Info menu.
                GUILayout.Window(InfoWindowID, new Rect((Screen.width - WindowWidth - Offset), Offset, 0, 0), InfoGUI, "Info", GUILayout.MinWidth(WindowWidth));
            }
            else
            {
                // Platform integration menu.
                GUILayout.Window(PlatformIntegrationWindowID, new Rect((Screen.width - WindowWidth - Offset), Offset, 0, 0), PlatformIntegrationGUI, "Platform Integration", GUILayout.MinWidth(WindowWidth));
            }
        }
        else if (_gameMasterScript.State != GameMaster.GAME_STATE.GS_START && _gameMasterScript.State != GameMaster.GAME_STATE.GS_STORE)
        {
            // Game Info.
            GUILayout.Window(GameInfoWindowID, new Rect(Offset, Offset, 0, 0), GameInfoGUI, "Game Info", GUILayout.MinWidth(WindowWidth));
        }

        // Game menu.
        GUILayout.Window(GameMenuWindowID, new Rect((Screen.width - WindowWidth) * 0.5f, Offset, 0, 0), FaceFlipGUI, "Face Flip Game", GUILayout.MinWidth(WindowWidth));
    }

    void FacebookIntegrationGUI (int windowID)
    {
        if (!FBWin.IsLoggedIn)
        {
            if (GUILayout.Button("Login", GUILayout.MinHeight(ButtonHeight)))
            {
#if !UNITY_EDITOR && UNITY_WINRT
                FBWin.Login("email,publish_actions,user_friends", _gameMasterScript.FBLoginCallback);
#endif
            }
        }
        else
        {
            TextAnchor originalAlign = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            // Display Facebook user name.
            GUILayout.Label(_gameMasterScript.FacebookName);

            // Display Facebook user image.
            if (_gameMasterScript.FacebookImage != null)
            {
                GUILayout.Label(_gameMasterScript.FacebookImage, GUILayout.MinHeight(ButtonHeight * 2));
            }

            // Display number of Facebook friends.
            GUILayout.Label(_gameMasterScript.FacebookFriends);

            GUILayout.Label(Separator, GUILayout.MaxWidth(WindowWidth));
            
            GUI.skin.label.alignment = originalAlign;

            if (GUILayout.Button("Invite Friends", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.InviteFriends();
            }

            if (GUILayout.Button("Post to Feed", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.PostFeed();
            }

            if (GUILayout.Button("Logout", GUILayout.MinHeight(ButtonHeight)))
            {
                FBWin.Logout();
                StartCoroutine(_gameMasterScript.FBLogoutCallback());
            }
        }
    }

    void FaceFlipGUI (int windowID)
    {
        TextAnchor originalAlign = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        if (_gameMasterScript.State == GameMaster.GAME_STATE.GS_START)
        {
            if (!string.IsNullOrEmpty(_gameMasterScript.GameResult))
            {
                GUILayout.Label(_gameMasterScript.GameResult, GUILayout.MaxWidth(WindowWidth));

                GUILayout.Label(Separator, GUILayout.MaxWidth(WindowWidth));
            }
            GUI.skin.label.alignment = originalAlign;

            if (GUILayout.Button("Start Game", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.ChangeState(GameMaster.GAME_STATE.GS_PLAYING);
            }
        }
        else
        {
            if (GUILayout.Button("End", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.ChangeState(GameMaster.GAME_STATE.GS_END);
            }
        }
    }

    void GameInfoGUI(int windowID)
    {
        TextAnchor originalAlign = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label(_gameMasterScript.Matches, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.MovesRemaining, GUILayout.MaxWidth(WindowWidth));

        GUI.skin.label.alignment = originalAlign;
    }

    void PlatformIntegrationGUI (int windowID)
    {
        TextAnchor originalAlign = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        GUILayout.Label(_gameMasterScript.ReminderInfo, GUILayout.MaxWidth(WindowWidth));

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label(Separator, GUILayout.MaxWidth(WindowWidth));
        
        GUI.skin.label.alignment = originalAlign;

        if (GUILayout.Button("Show Info", GUILayout.MinHeight(ButtonHeight)))
        {
            _showInfo = true;
        }

        if (GameMaster.ReminderScheduled)
        {
            if (GUILayout.Button("Remove Reminder", GUILayout.MinHeight(ButtonHeight)))
            {
                GameMaster.CancelReminder();
            }
        }
        else
        {
            // Set reminder (120sec later).
            if (GUILayout.Button("Set Reminder", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.SetReminder();
            }
        }

        if (GUILayout.Button("Store", GUILayout.MinHeight(ButtonHeight)))
        {
            _gameMasterScript.RetrieveProducts();
            _gameMasterScript.ChangeState(GameMaster.GAME_STATE.GS_STORE);
        }

#if !UNITY_WP_8_1
        // Test share UI.
        if (GUILayout.Button("Share", GUILayout.MinHeight(ButtonHeight)))
        {
            _gameMasterScript.ShowShareUI();
        }
#endif

        // Show native dialog
        if (GUILayout.Button("Show Native Dialog", GUILayout.MinHeight(ButtonHeight)))
        {
            Debug.Log("Show Native Dialog.");
#if !UNITY_EDITOR && UNITY_WINRT
            Action<bool> callback = b => Debug.Log("Native Dialog: User response is " + b);
            Helper.Instance.ShowDialog("How cool is that?", "This is a native dialog!", callback,
                "So cool!", "Meh...");
#endif
        }

        // Test VideoPlayer.
        if (GUILayout.Button("Play Video", GUILayout.MinHeight(ButtonHeight)))
        {
            _gameMasterScript.PlayVideo();
        }

        // Send email
        if (GUILayout.Button("Send Email", GUILayout.MinHeight(ButtonHeight)))
        {
            _gameMasterScript.SendEmail();
        }

        // Test ExtractStackTrace.
        if (GUILayout.Button("Extract Stack Trace", GUILayout.MinHeight(ButtonHeight)))
        {
            _gameMasterScript.ExtractStackTrace();
        }

#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
        if (GUILayout.Button("Quit", GUILayout.MinHeight(ButtonHeight)))
        {
            Application.Quit();
        }
#endif
    }

    void InfoGUI (int windowID)
    {
        TextAnchor originalAlign = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        GUILayout.Label(_gameMasterScript.AppVersion, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.Language, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.DeviceID, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.LowEnd, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.Internet, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.MeteredConnection, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.EnvironmentConfiguration, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.ExceptionLoggingEnabledForEnvironment, 
            GUILayout.MaxWidth(WindowWidth));

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label(Separator, GUILayout.MaxWidth(WindowWidth));

        GUI.skin.label.alignment = originalAlign;

        if (GUILayout.Button("Back", GUILayout.MinHeight(ButtonHeight)))
        {
            _showInfo = false;
        }
    }

    void ExceptionLogginGUI (int windowID)
    {
        if (GUILayout.Button("Log App Crash", GUILayout.MinHeight(ButtonHeight)))
        {
            _gameMasterScript.LogAppCrash();
        }

        if (GUILayout.Button("Log Unity Exception", GUILayout.MinHeight(ButtonHeight)))
        {
            ExceptionLogger.IsEnabled = true;
            throw new System.Exception("This is test exception from Unity code");
        }
    }
}
