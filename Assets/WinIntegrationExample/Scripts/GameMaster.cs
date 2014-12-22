using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MarkerMetro.Unity.WinIntegration.Facebook;
using MarkerMetro.Unity.WinIntegration.Resources;
using MarkerMetro.Unity.WinIntegration.LocalNotifications;
using LitJson;

#if UNITY_WP8 && !UNITY_EDITOR
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FBNative;
#else
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FB;
#endif

public class GameMaster : MonoBehaviour {

	public GameObject 	gui_start_;
	public GameObject 	gui_play_;
	public GameObject 	gui_end_;
	public GameObject	gui_store_;
    public GameObject   facebook_image_;
    public GameObject   login_name_;
    public GameObject   number_friends_;
    public TextMesh     reminderCountdownTextMesh;

	public GUIText 		gui_matches_;
	public GUIText 		gui_remaining_;
	public GUIText 		gui_result_;

	public GAME_STATE state_;
    public bool reminderStarted;

	private List<GameObject> tiles_ = new List<GameObject>();
	private string[] names_ = {"Keith", "Tony", "Greg", "Nigel", "Ivan", "Chad", "Damian", "Brian" };
	private Tile current_switched_1 = null;
	private Tile current_switched_2 = null;
	private float waiting_timer_ = 0.0f;
    private DateTime reminderStartTime;

	private int max_moves_ = 15;
	private int remaining_moves_ = 0;
	private int number_matches_ = 0;

    private Dictionary<string, Texture2D> facebook_friends_ = new Dictionary<string, Texture2D>();

	const int rows_ = 4;
	const int cols_ = 4;
	const float wait_after_switch_ = 0.5f;
    const float reminderTime = 30f;
    const string reminderTextPrefix = "Seconds till reminder: ";

	public enum GAME_STATE
	{
		GS_START,
		GS_PLAYING,
		GS_END,
		GS_WAITING,
		GS_STORE
	};

	void Start () {
        if (DateTime.TryParse(PlayerPrefs.GetString("reminderStartTime", string.Empty), out reminderStartTime))
        {
            reminderStarted = true;
            float timeDiff = (float)DateTime.Now.Subtract(reminderStartTime).TotalSeconds;
            if (timeDiff >= reminderTime)
            {
                reminderStarted = false;
                reminderCountdownTextMesh.text = reminderTextPrefix + "0";
            }
            else
            {
                reminderStarted = true;
                reminderCountdownTextMesh.text = reminderTextPrefix + (Mathf.Ceil(reminderTime - timeDiff)).ToString();
            }
        }


		CreateTiles();
		ChangeState( GAME_STATE.GS_START );
        FBWin.Init(SetFBInit, Assets.Plugins.MarkerMetro.Constants.FBAppId, OnHideUnity);
    }
	
	void Update () {
		if ( state_ == GAME_STATE.GS_WAITING )
		{
			if ( waiting_timer_ > wait_after_switch_ )
			{
				waiting_timer_ = 0.0f;
				ChangeState( GAME_STATE.GS_PLAYING );
			}
			else
			{
				waiting_timer_ += Time.deltaTime;
			}
		}

		if ( state_ == GAME_STATE.GS_PLAYING )
		{
			if ( number_matches_ == tiles_.Count / 2 )
			{
				// Win
				ChangeState( GAME_STATE.GS_END );
				gui_result_.text = "YOU WIN";
			}
			else if ( remaining_moves_ == 0 )
			{
				// Loss
				ChangeState( GAME_STATE.GS_END );
				gui_result_.text = "YOU LOSE";
			}
		}

        if (reminderStarted)
        {
            float timeDiff = (float)DateTime.Now.Subtract(reminderStartTime).TotalSeconds;
            if (timeDiff >= reminderTime)
            {
                reminderStarted = false;
                reminderCountdownTextMesh.text = reminderTextPrefix + "0";
            }
            else
            {
                reminderCountdownTextMesh.text = reminderTextPrefix + (Mathf.Ceil(reminderTime - timeDiff)).ToString();
            }
        }
	}

	// Change the games state
	public void ChangeState( GAME_STATE state )
	{
		switch ( state )
		{
			case GAME_STATE.GS_START:
			{
				gui_start_.SetActive( true );
				gui_end_.SetActive( false );
				gui_play_.SetActive( false );
				gui_store_.SetActive( false );
				state_ = state;

				remaining_moves_ = max_moves_;
				number_matches_ = 0;
				gui_result_.text = "YOU LOSE";

                SetupTiles();
			}
			break;
			case GAME_STATE.GS_PLAYING:
			{
                gui_start_.SetActive(false);
				gui_end_.SetActive( false );
				gui_play_.SetActive( true );
				gui_store_.SetActive( false );
				state_ = state;

				if ( current_switched_1 != null )
				{
					current_switched_1.Rotate();
					current_switched_1 = null;
				}
				if ( current_switched_2 != null )
				{
					current_switched_2.Rotate();
					current_switched_2 = null;
				}

				SetGUIText();
			}
			break;
			case GAME_STATE.GS_END:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( true );
				gui_play_.SetActive( false );
				gui_store_.SetActive( false );
				state_ = state;
			}
			break;
			case GAME_STATE.GS_WAITING:
			{
				waiting_timer_ = 0.0f;
				state_ = state;
			}
			break;
			case GAME_STATE.GS_STORE:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( false );
				gui_play_.SetActive( false );
				gui_store_.SetActive( true );
				state_ = state;
			}
			break;
			default: break;
		}
	}

    // Create the tiles initially
	void CreateTiles()
	{
		GameObject tile_base = GameObject.Find("GameTile");
		for ( int x = 0; x < rows_; ++x )
		{
			for ( int y = 0; y < cols_; ++y )
			{
				GameObject new_tile = Instantiate( tile_base, new Vector3( x * 1.5f, y * 1.5f, 0.0f ), Quaternion.identity ) as GameObject;
				tiles_.Add( new_tile );
			}
		}
	}

    // Set the tiles up for each run of the game.  If the player has any facebook friends associated with the app they will
    // be used before the hardcoded pictures
	void SetupTiles()
	{
		int number_tiles = tiles_.Count;
		for ( int i = 0; i < number_tiles; ++i )
		{
			var temp = tiles_[i];
			int random_index = UnityEngine.Random.Range( i, tiles_.Count );
			tiles_[i] = tiles_[ random_index ];
			tiles_[ random_index ] = temp;
		}

		for ( int i = 0; i < number_tiles / 2; ++i )
		{
            string name = names_[i];
            Texture2D texture = Resources.Load(name) as Texture2D;

            if ( facebook_friends_.Count > i )
            {
                int count = 0;
                foreach ( var item in facebook_friends_ )
                {
                    if ( count == i )
                    {
                        name = item.Key;
                        texture = item.Value;
                        break;
                    }
                    ++count;
                }
            }

			Tile script_1 = tiles_[i * 2].GetComponent<Tile>();
			script_1.SetImage( names_[i], texture );
			Tile script_2 = tiles_[i * 2 + 1].GetComponent<Tile>();
			script_2.SetImage( names_[i], texture );
		}
	}

    // Called when a tile is tapped, if first keep a ref if second check for a match
	public void OnTileSwitch( Tile script )
	{
		if ( current_switched_1 == null )
		{
			current_switched_1 = script;
		}
		else
		{
			--remaining_moves_;
			if ( current_switched_1.name_ == script.name_ )
			{
				// match
				current_switched_1 = null;
				++number_matches_;
			}
			else
			{
				current_switched_2 = script;
				ChangeState( GAME_STATE.GS_WAITING );
			}
		}

		SetGUIText();
	}

	void SetGUIText()
	{
		gui_matches_.text = "Matches: " + number_matches_.ToString();
		gui_remaining_.text = "Moves Remaining: " + remaining_moves_.ToString();
	}

    // This will be called by the IAP 
	public void AddMoves()
	{
		remaining_moves_ += 5;
		SetGUIText();
	}


    //
    // Facebook Test Functions
    //
    private void SetFBInit()
    {
        Debug.Log("Set FB Init");
        if (FBWin.IsLoggedIn)
        {
            Debug.Log("Already logged in to FB");
            StartCoroutine(RefreshFBStatus());
        }
    }

    private void OnHideUnity( bool hide_unity )
    {
        Debug.Log("OnHideUnity" + hide_unity);
    }

    /// <summary>
    /// login callback
    /// </summary>
    /// <param name="result"></param>
    public void FBLoginCallback( FBResult result )
    {
        Debug.Log("LoginCallback");
        if (result.Error != null)
        {
            Debug.Log("Login error occurred");
            if (result.Error == "-1")
            {
                Debug.Log("Login was cancelled");
            }
        }
        if (FBWin.IsLoggedIn)
        {
            StartCoroutine(RefreshFBStatus());
        }
    }

    public IEnumerator FBLogoutCallback()
    {
        while (FBWin.IsLoggedIn)
        {
            yield return null;
        }

        StartCoroutine(RefreshFBStatus());
    }

    // Set the players name and picture
    private IEnumerator RefreshFBStatus()
    {
        yield return new WaitForEndOfFrame();

        TextMesh text = (TextMesh)login_name_.GetComponent<TextMesh>();
        Renderer renderer = facebook_image_.GetComponent<MeshRenderer>().renderer;
        if (FBWin.IsLoggedIn)
        {
#if (UNITY_METRO && !UNITY_EDITOR)
            text.text = FB.UserName; 
            Texture2D texture = new Texture2D(128, 128, TextureFormat.DXT1, false);

            yield return StartCoroutine(GetFBPicture(FB.UserId, texture));

            renderer.material.mainTexture = texture;
#elif UNITY_WP8 && !UNITY_EDITOR
            FBNative.GetCurrentUser((user) =>
            {
                StartCoroutine(SetFBStatus(user));
            });
#else
            // TODO picture and name not yet supported on FBNative
            text.text = "Logged In (picture and name to do!)";
            yield break;
#endif
        }
        else
        {
            text.text = "Not Logged In";
            renderer.material.mainTexture = null;
            TextMesh number_text = (TextMesh)number_friends_.GetComponent<TextMesh>();
            number_text.text = "No Friends";
        }
    }

    private IEnumerator SetFBStatus (FBUser user)
    {
        TextMesh text = (TextMesh)login_name_.GetComponent<TextMesh>();
        Renderer fbImageRenderer = facebook_image_.GetComponent<MeshRenderer>().renderer;

        text.text = user.Name;
        Texture2D texture = new Texture2D(128, 128, TextureFormat.DXT1, false);

        yield return StartCoroutine(GetFBPicture(user.Id, texture));

        fbImageRenderer.material.mainTexture = texture;
    }

    // Request the players friends
    // As per FB API v2.0 You can only request friends that have installed and logged in on the app, 
    // you can no longer poll all the players friends.
    // https://developers.facebook.com/bugs/1502515636638396/
    public void PopulateFriends()
    {
        if (FBWin.IsLoggedIn)
        {
            // Get the friends
            FBWin.API("/me/friends", HttpMethod.GET, GetFriendsCallback);
        }
    }

    public void InviteFriends()
    {
        if (FBWin.IsLoggedIn)
        {
            FBWin.AppRequest(message: "Come Play FaceFlip!", callback: (result) =>
            {
                Debug.Log("AppRequest result: " + result.Text);
                if (result.Json != null)
                    Debug.Log("AppRequest Json: " + result.Json.ToString());
            }, title: "FaceFlip Invite");


            // Test AppRequest with to param assigned
            //string[] toList = new string[1];
            //toList[0] = "10152837634773832";    // Freddy's Facebook ID, replace with someone's ID that is in your friend list, or add Freddy to your friend list :D
            //FBWin.AppRequest(message: "Come Play FaceFlip!", to: toList, callback: (result) =>
            //{
            //    Debug.Log(result.Text);
            //    Debug.Log(result.Json.ToString());
            //}, title: "FaceFlip Invite");
        }
    }

    public void PostFeed()
    {
        if (FBWin.IsLoggedIn)
        {
            FBWin.Feed(
                link: "http://www.markermetro.com",
                linkName: "linkName",
                linkCaption: "linkCaption",
                linkDescription: "linkDescription",
                picture: "https://pbs.twimg.com/profile_images/1668748982/icon-metro-tw-128_normal.png",
                callback: (result) =>
            {
                Debug.Log("Feed result: " + result.Text);
                if (result.Json != null)
                    Debug.Log("Feed Json: " + result.Json.ToString());
            });
        }
    }

    // Parse the json and request the friend pictures
    private void GetFriendsCallback( FBResult result )
    {
        if (result.Error != null)
        {
            Debug.Log("Failed to get FB Friends");
            return;
        }

        try
        {
            facebook_friends_.Clear();
            JsonData data = JsonMapper.ToObject(result.Text);

            JsonData friends = data["data"];
            for (int i = 0; i < friends.Count; ++i)
            {
                JsonData friend = friends[i];
                string name = (string)friend["name"];
                string id = (string)friend["id"];
                Texture2D texture = new Texture2D(128, 128, TextureFormat.DXT1, false);
                StartCoroutine(GetFBPicture(id, texture));

                facebook_friends_.Add(name, texture);
            }

            TextMesh text = (TextMesh)number_friends_.GetComponent<TextMesh>();
            text.text = "Friends: " + facebook_friends_.Count;
            SetupTiles();
        }
        catch( Exception e )
        {
            Debug.Log(e.Message);
        }
    }

    // Load a picture into the texture
    private IEnumerator GetFBPicture(string id, Texture2D texture)
    {     
        WWW url = new WWW("https" + "://graph.facebook.com/" + id + "/picture?type=large");

        while (!url.isDone)
        {
            yield return null;
        }
        
        url.LoadImageIntoTexture(texture);
    }

    public void SetReminder ()
    {
        reminderStarted = true;
        reminderStartTime = DateTime.Now;
        PlayerPrefs.SetString("reminderStartTime", reminderStartTime.ToString());
        ReminderManager.SetRemindersStatus(true);
        ReminderManager.RegisterReminder("testID", "Face Flip", "This is a reminder.", DateTime.Now.AddSeconds(reminderTime));
    }

    public void CancelReminder ()
    {
        reminderStarted = false;
        reminderStartTime = DateTime.Now.AddSeconds(-reminderTime);
        reminderCountdownTextMesh.text = reminderTextPrefix + "0";
        PlayerPrefs.SetString("reminderStartTime", reminderStartTime.ToString());
        ReminderManager.RemoveReminder("testID");
    }
}
