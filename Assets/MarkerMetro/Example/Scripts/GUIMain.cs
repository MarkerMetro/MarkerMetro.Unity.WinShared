using System;
using System.Collections;
using UnityEngine;

using MarkerMetro.Unity.WinIntegration;
using MarkerMetro.Unity.WinIntegration.LocalNotifications;
using MarkerMetro.Unity.WinIntegration.Facebook;

#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FBNative;
#else
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FB;
#endif

public class GUIMain : MonoBehaviour {

    const float Offset = 10f;
    const float WindowWidth = 200;
    const float ButtonHeight = 40f;
    const string Separator = "---------------------------------------------";

    GameMaster _gameMasterScript;
    bool _showInfo = false;

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
            // facebook menu.
            GUILayout.Window(0, new Rect(Offset, Offset, 0, 0), FacebookIntegrationGUI, "Facebook Integration", GUILayout.MinWidth(WindowWidth));
            
            if (_showInfo)
            {
                // Info menu.
                GUILayout.Window(3, new Rect((Screen.width - WindowWidth - Offset), Offset, 0, 0), InfoGUI, "Info", GUILayout.MinWidth(WindowWidth));
            }
            else
            {
                // platform integration menu.
                GUILayout.Window(2, new Rect((Screen.width - WindowWidth - Offset), Offset, 0, 0), PlatformIntegrationGUI, "Platform Integration", GUILayout.MinWidth(WindowWidth));
            }
        }

        // game menu.
        Rect gameMenuPos = new Rect((Screen.width - WindowWidth) * 0.5f, Offset, 0, 0);
        if (_gameMasterScript.State != GameMaster.GAME_STATE.GS_START)
        {
            gameMenuPos.x = Offset;
        }
        GUILayout.Window(1, gameMenuPos, FaceFlipGUI, "Face Flip Game", GUILayout.MinWidth(WindowWidth));
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
            GUILayout.Label(_gameMasterScript.Matches, GUILayout.MaxWidth(WindowWidth));
            GUILayout.Label(_gameMasterScript.MovesRemaining, GUILayout.MaxWidth(WindowWidth));

            GUILayout.Label(Separator, GUILayout.MaxWidth(WindowWidth));

            GUI.skin.label.alignment = originalAlign;

            if (GUILayout.Button("End", GUILayout.MinHeight(ButtonHeight)))
            {
                _gameMasterScript.ChangeState(GameMaster.GAME_STATE.GS_END);
            }
        }
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

        // Test crash button
        if (GUILayout.Button("Throw an Exception", GUILayout.MinHeight(ButtonHeight)))
        {
            throw new System.Exception("This is test exception from Unity code");
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
#if !UNITY_WP_8_1
        GUILayout.Label(_gameMasterScript.LowEnd, GUILayout.MaxWidth(WindowWidth));
#endif
        GUILayout.Label(_gameMasterScript.Internet, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.MeteredConnection, GUILayout.MaxWidth(WindowWidth));
        GUILayout.Label(_gameMasterScript.EnvironmentConfiguration, GUILayout.MaxWidth(WindowWidth));

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label(Separator, GUILayout.MaxWidth(WindowWidth));

        GUI.skin.label.alignment = originalAlign;

        if (GUILayout.Button("Back", GUILayout.MinHeight(ButtonHeight)))
        {
            _showInfo = false;
        }
    }
}
