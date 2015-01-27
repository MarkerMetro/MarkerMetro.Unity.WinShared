using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration.Facebook;

#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FBNative;
#else
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FB;
#endif

public class GUIStart : MonoBehaviour {

    GameMaster _gameMasterScript;

    void Start ()
    {
        GameObject gameMasterObject = GameObject.Find("GameMaster");
        _gameMasterScript = gameMasterObject.GetComponent<GameMaster>();
    }

#if UNITY_WP8 || UNITY_WP_8_1
    void Update ()
    {
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
    }
#endif

	void OnGUI()
	{
		// Make a background box
		int half_width = Screen.width / 2;
		int half_height = Screen.height / 2;

		int box_width = 200;
		int box_height = 430;

        if (FBWin.IsLoggedIn)
        {
            box_height += 150;
        }
#if UNITY_METRO && !UNITY_WP_8_1
        box_height -= 50;
#endif
#if UNITY_WP_8_1
        box_height -= 50;
#endif

        int box_x = half_width - box_width / 2;
		int box_y = half_height - box_height / 2;

        GUI.Box(new Rect( box_x, box_y, box_width, box_height), "Main Menu");

        int y_modifier = 30;

        // Make the first button. If it is pressed, Start Game
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Start Game"))
        {
            _gameMasterScript.RetrieveProducts();
            _gameMasterScript.ChangeState(GameMaster.GAME_STATE.GS_PLAYING);
        }

        y_modifier += 50;

        if (GameMaster.ReminderScheduled)
        {
            // cancel reminder.
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Remove Reminder"))
            {
                GameMaster.CancelReminder();
            }
        }
        else
        {
            // Set reminder (120sec later).
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Set Reminder"))
            {
                _gameMasterScript.SetReminder();
            }
        }

        y_modifier += 50;

        if (!FBWin.IsLoggedIn)
        {
            // Second Button Login to FB
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Login"))
            {
                FBWin.Login("email,publish_actions,user_friends", _gameMasterScript.FBLoginCallback);
            }
        }
        else
        {
            // Second Button Logout to FB
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Logout"))
            {
                FBWin.Logout();
                StartCoroutine(_gameMasterScript.FBLogoutCallback());
            }

            y_modifier += 50;

            // Get Friends That Play the game
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Get Friends"))
            {
                _gameMasterScript.PopulateFriends();
            }

            y_modifier += 50;

            // Invite friends that don't play
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Invite Friends"))
            {
                _gameMasterScript.InviteFriends();
            }

            y_modifier += 50;

            // Post feed
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Post Feed"))
            {
                _gameMasterScript.PostFeed();
            }
        }

        y_modifier += 50;

        //Test crash button
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Throw an exception"))
        {
            throw new System.Exception("This is test exception from Unity code");
        }

#if !UNITY_WP_8_1
        y_modifier += 50;

        // Test share UI.
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Share"))
        {
            _gameMasterScript.ShowShareUI();
        }
#endif

        y_modifier += 50;

        // Test ExtractStackTrace.
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "ExtractStackTrace"))
        {
            _gameMasterScript.ExtractStackTrace();
        }

        y_modifier += 50;

        // Test VideoPlayer.
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Play Video"))
        {
            _gameMasterScript.PlayVideo();
        }

        y_modifier += 50;

#if !UNITY_METRO || UNITY_WP_8_1
        // Third Button Login to Quit
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Quit"))
        {
            Application.Quit();
        }
#endif
    }
}
