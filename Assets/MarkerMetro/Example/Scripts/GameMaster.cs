using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MarkerMetro.Unity.WinIntegration;
using MarkerMetro.Unity.WinIntegration.Facebook;
using MarkerMetro.Unity.WinIntegration.Resources;
using MarkerMetro.Unity.WinIntegration.LocalNotifications;
using MarkerMetro.Unity.WinIntegration.Store;
using MarkerMetro.Unity.WinIntegration.VideoPlayer;
using LitJson;
using MarkerMetro.Unity.WinShared;
using MarkerMetro.Unity.WinIntegration.Logging;

#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FBNative;
#else
using FBWin = MarkerMetro.Unity.WinIntegration.Facebook.FB;
#endif

namespace MarkerMetro.Unity.WinShared.Example
{
    public class GameMaster : MonoBehaviour, IGameSettings
    {
        public static bool ReminderScheduled;
        public static bool ForceResetReminderText;

        bool _soundEnabled = true;
        bool _musicEnabled = true;
        AudioSource _musicAudioSource = null;
        bool _musicPlayed = false;

        public bool SoundEnabled
        {
            get
            {
                return _soundEnabled;
            }
            set
            {
                _soundEnabled = value;

                if (value)
                {
                    PlaySound(_flipSound);
                }
                else
                {
                    audio.Stop();
                }
            }
        }

        public bool MusicEnabled
        {
            get
            {
                return _musicEnabled;
            }
            set
            {
                _musicEnabled = value;

                if (value)
                {
                    PlaySound(_flipSound);
                }
                else
                {
                    _musicAudioSource.Stop();
                }
            }
        }

        public List<Product> StoreProducts { get; private set; }

        // Facebook.
        public string FacebookName { get; private set; }
        public Texture2D FacebookImage { get; private set; }
        public string FacebookFriends { get; private set; }

        // Reminder Info.
        public string ReminderInfo { get; private set; }

        // App/Device Info.
        public string AppVersion { get; private set; }
        public string Language { get; private set; }
        public string DeviceID { get; private set; }
        public string LowEnd { get; private set; }
        public string Internet { get; private set; }
        public string MeteredConnection { get; private set; }
        public string BuildConfiguration { get; private set; }
        public string ExceptionLoggingEnabledForBuildConfig { get; private set; }

        // Game info.
        public string Matches { get; private set; }
        public string MovesRemaining { get; private set; }
        public string GameResult { get; private set; }
        public GAME_STATE State { get; private set; }
        GAME_STATE _newState = GAME_STATE.GS_UNDEFINED;

        [SerializeField]
        GameObject _guiMain = null;
        [SerializeField]
        GameObject _guiStore = null;

        [SerializeField]
        AudioClip _flipSound = null;
        [SerializeField]
        AudioClip _matchSound = null;
        [SerializeField]
        AudioClip _failSound = null;

        List<GameObject> _tiles = new List<GameObject>();
        string[] _names = { "Keith", "Tony", "Greg", "Nigel", "Ivan", "Chad", "Damian", "Brian" };
        Tile _currentSwitched1 = null;
        Tile _currentSwitched2 = null;
        float _waitingTimer = 0.0f;

        int _maxMoves = 15;
        int _remainingMoves = 0;
        int _numberMatches = 0;

        Dictionary<string, Texture2D> _facebookFriends = new Dictionary<string, Texture2D>();

        DateTime _reminderStartTime = DateTime.Now;

        const float ReminderTime = 120f;
        const string ReminderTextPrefix = "Reminder scheduled for ";
        const string ReminderTextSuffix = " (+/- 1 minute)";
        const string NoReminderText = "No reminder scheduled.";

        const int Rows = 4;
        const int Cols = 4;
        const float WaitAfterSwitch = 0.5f;

        public enum GAME_STATE
        {
            GS_START,
            GS_PLAYING,
            GS_WAITING,
            GS_END,
            GS_STORE,
            GS_UNDEFINED
        };

        void Awake()
        {
            GameController.Instance.Init(this);
        }

        void Start()
        {
            InitializeInfo();

            _musicAudioSource = GetComponents<AudioSource>()[1];

            // Reminder and Facebook aren't supported in Unity Editor.
#if !UNITY_EDITOR && UNITY_WINRT
            if (ReminderManager.AreRemindersEnabled() && DateTime.TryParse(PlayerPrefs.GetString("_reminderStartTime", string.Empty), out _reminderStartTime))
            {
                CheckReminder();

                if (ReminderScheduled)
                {
                    ReminderInfo = ReminderTextPrefix + _reminderStartTime.AddSeconds(ReminderTime).ToString("hh:mm tt");
#if UNITY_WP8
                   ReminderInfo += ReminderTextSuffix;
#endif
                }
            }
            else
            {
                ReminderScheduled = false;
                ReminderInfo = NoReminderText;
            }
#else
            ReminderScheduled = false;
            ReminderInfo = NoReminderText;
#endif

            CreateTiles();
            DoChangeState(GAME_STATE.GS_START);

#if !UNITY_EDITOR && UNITY_WINRT
            FBWin.Init(SetFBInit, GameController.Instance.GameConfig.FacebookAppId, null);
#endif
        }

        void InitializeInfo()
        {
            _remainingMoves = _maxMoves;
            _numberMatches = 0;

#if !UNITY_EDITOR && UNITY_WINRT
            AppVersion = "AppVersion: " + Helper.Instance.GetAppVersion();
            Language = "Language: " + Helper.Instance.GetAppLanguage();
            try
            {
                DeviceID = "Device ID: " + Helper.Instance.GetUserDeviceId();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                DeviceID = "Device ID: not available.";
            }
            LowEnd = "Is Low End: " + Helper.Instance.IsLowEndDevice();
            Internet = "Is Online: " + Helper.Instance.HasInternetConnection;
            MeteredConnection = "Is metered connection: " + Helper.Instance.IsMeteredConnection;
            ExceptionLoggingEnabledForBuildConfig = "Exception logging for current build config: " + GameController.Instance.GameConfig.ExceptionLoggingAllowed.ToString();
            BuildConfiguration = "Build config: " + GameController.Instance.GameConfig.CurrentBuildConfig.ToString();
#else
            AppVersion = "AppVersion: Unknown";
            Language = "Language: Unknown";
            DeviceID = "Device ID: Unknown";
            LowEnd = "Is Low End: Unknown";
            Internet = "Is Online: Unknown";
            MeteredConnection = "Is metered connection: Unknown";
            ExceptionLoggingEnabledForBuildConfig = "Exception logging for current build config: Unknown";
            BuildConfiguration = "Build config: Unknown";
#endif

        }

        void Update()
        {
#if !UNITY_EDITOR && UNITY_WINRT
            // Update Info.
            Language = "Language: " + Helper.Instance.GetAppLanguage();
            Internet = "Is Online: " + Helper.Instance.HasInternetConnection;
            MeteredConnection = "Is metered connection: " + Helper.Instance.IsMeteredConnection;
#endif
            if (State != _newState)
            {
                DoChangeState(_newState);
            }

            if (State == GAME_STATE.GS_WAITING)
            {
                if (_waitingTimer > WaitAfterSwitch)
                {
                    _waitingTimer = 0.0f;
                    ChangeState(GAME_STATE.GS_PLAYING);
                }
                else
                {
                    _waitingTimer += Time.deltaTime;
                }
            }

            if (State == GAME_STATE.GS_PLAYING)
            {
                if (_numberMatches == _tiles.Count / 2)
                {
                    // Win
                    ChangeState(GAME_STATE.GS_END);
                    GameResult = "YOU WIN";
                }
                else if (_remainingMoves == 0)
                {
                    // Loss
                    ChangeState(GAME_STATE.GS_END);
                    GameResult = "YOU LOSE";
                }
            }

            if (ForceResetReminderText)
            {
                ForceResetReminderText = false;
                ReminderInfo = NoReminderText;
            }

            if (ReminderScheduled)
            {
                CheckReminder();
            }
        }

        public void ChangeState(GAME_STATE state)
        {
            _newState = state;
        }

        // Change the games state.
        void DoChangeState(GAME_STATE state)
        {
            switch (state)
            {
                case GAME_STATE.GS_START:
                    {
                        _guiMain.SetActive(true);
                        _guiStore.SetActive(false);
                        State = state;

                        SetupTiles();
                        _currentSwitched1 = null;
                        _currentSwitched2 = null;
                    }
                    break;
                case GAME_STATE.GS_PLAYING:
                    {
                        PlayMusic();
                        _guiMain.SetActive(true);
                        _guiStore.SetActive(false);
                        State = state;

                        if (_currentSwitched1 != null)
                        {
                            _currentSwitched1.Rotate();
                            _currentSwitched1 = null;
                        }
                        if (_currentSwitched2 != null)
                        {
                            _currentSwitched2.Rotate();
                            _currentSwitched2 = null;
                        }

                        SetGUIText();
                    }
                    break;
                case GAME_STATE.GS_WAITING:
                    {
                        _waitingTimer = 0.0f;
                        State = state;
                    }
                    break;
                case GAME_STATE.GS_END:
                    {
                        _musicPlayed = false;
                        _remainingMoves = _maxMoves;
                        _numberMatches = 0;
                        ChangeState(GAME_STATE.GS_START);
                    }
                    break;
                case GAME_STATE.GS_STORE:
                    {
                        _guiMain.SetActive(false);
                        _guiStore.SetActive(true);
                        State = state;
                    }
                    break;
                default: break;
            }
        }

        // Create the tiles initially.
        void CreateTiles()
        {
            GameObject tileBase = GameObject.Find("GameTile");
            for (int x = 0; x < Rows; ++x)
            {
                for (int y = 0; y < Cols; ++y)
                {
                    GameObject new_tile = Instantiate(tileBase, new Vector3(x * 1.5f, y * 1.5f, 0.0f), Quaternion.identity) as GameObject;
                    _tiles.Add(new_tile);
                }
            }
        }

        // Set the tiles up for each run of the game.  If the player has any facebook friends associated with the app they will
        // be used before the hardcoded pictures.
        void SetupTiles()
        {
            int numberTiles = _tiles.Count;
            for (int i = 0; i < numberTiles; ++i)
            {
                var temp = _tiles[i];
                int random_index = global::UnityEngine.Random.Range(i, _tiles.Count);
                _tiles[i] = _tiles[random_index];
                _tiles[random_index] = temp;
            }

            for (int i = 0; i < numberTiles / 2; ++i)
            {
                string name = _names[i];
                Texture2D texture = Resources.Load(name) as Texture2D;

                if (_facebookFriends.Count > i)
                {
                    int count = 0;
                    foreach (var item in _facebookFriends)
                    {
                        if (count == i)
                        {
                            name = item.Key;
                            texture = item.Value;
                            break;
                        }
                        ++count;
                    }
                }

                Tile script1 = _tiles[i * 2].GetComponent<Tile>();
                script1.SetImage(_names[i], texture);
                Tile script2 = _tiles[i * 2 + 1].GetComponent<Tile>();
                script2.SetImage(_names[i], texture);
            }
        }

        // Called when a tile is tapped, if first keep a ref if second check for a match.
        public void OnTileSwitch(Tile script)
        {
            if (_currentSwitched1 == null)
            {
                _currentSwitched1 = script;
                PlaySound(_flipSound);
            }
            else
            {
                --_remainingMoves;
                if (_currentSwitched1.name_ == script.name_)
                {
                    // match
                    _currentSwitched1 = null;
                    ++_numberMatches;
                    PlaySound(_matchSound);
                }
                else
                {
                    _currentSwitched2 = script;
                    ChangeState(GAME_STATE.GS_WAITING);
                    PlaySound(_failSound);
                }
            }

            SetGUIText();
        }

        void PlaySound(AudioClip clip)
        {
            if (SoundEnabled)
            {
                audio.clip = clip;
                audio.Play();
            }
        }

        void PlayMusic()
        {
            if (MusicEnabled && !_musicPlayed)
            {
                _musicAudioSource.Play();
                // Music plays once, during start:
                _musicPlayed = true;
            }
        }

        void SetGUIText()
        {
            Matches = "Matches: " + _numberMatches.ToString();
            MovesRemaining = "Moves Remaining: " + _remainingMoves.ToString();
        }

        // This will be called by the IAP.
        public void AddMoves()
        {
            _remainingMoves += 5;
            SetGUIText();
        }


        //
        // Facebook Test Functions.
        //
        void SetFBInit()
        {
            Debug.Log("Set FB Init");
            if (FBWin.IsLoggedIn)
            {
                Debug.Log("Already logged in to FB");
                StartCoroutine(RefreshFBStatus());
            }
        }

        /// <summary>
        /// login callback.
        /// </summary>
        /// <param name="result"></param>
        public void FBLoginCallback(FBResult result)
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

        // Set the players name and picture.
        private IEnumerator RefreshFBStatus()
        {
            FacebookName = string.Empty;
            FacebookFriends = string.Empty;
            FacebookImage = null;

            yield return new WaitForEndOfFrame();

            if (FBWin.IsLoggedIn)
            {
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
                FBNative.GetCurrentUser((user) =>
                {
                    StartCoroutine(SetFBStatus(user));
                });
#elif (UNITY_METRO && !UNITY_EDITOR)
                FacebookName = FB.UserName; 
                PopulateFriends();
                FacebookImage = new Texture2D(128, 128, TextureFormat.DXT1, false);
                yield return StartCoroutine(GetFBPicture(FB.UserId, FacebookImage));
#else
                FacebookName = "Logged In (picture and name to do!)";
                yield break;
#endif
            }
            else
            {
                FacebookName = "Not Logged In";
                FacebookFriends = "No Friends";
                FacebookImage = null;
            }
        }

        private IEnumerator SetFBStatus(FBUser user)
        {
            FacebookName = user.Name;
            PopulateFriends();
            FacebookImage = new Texture2D(128, 128, TextureFormat.DXT1, false);
            yield return StartCoroutine(GetFBPicture(user.Id, FacebookImage));
        }

        // Request the players friends.
        // As per FB API v2.0 You can only request friends that have installed and logged in on the app, 
        // you can no longer poll all the players friends.
        // https://developers.facebook.com/bugs/1502515636638396/
        public void PopulateFriends()
        {
            Debug.Log("Populate Friends.");
#if !UNITY_EDITOR && UNITY_WINRT
            if (FBWin.IsLoggedIn)
            {
                // Get the friends
                FBWin.API("/me/friends", HttpMethod.GET, GetFriendsCallback);
            }
#endif
        }

        public void InviteFriends()
        {
            Debug.Log("Invite Friends.");
#if !UNITY_EDITOR && UNITY_WINRT
            if (FBWin.IsLoggedIn)
            {
                // title param only supported in WP8 (FBNative) at the moment.
                FBWin.AppRequest(message: "Come Play FaceFlip!", callback: (result) =>
                {
                    Debug.Log("AppRequest result: " + result.Text);
                    if (result.Json != null)
                        Debug.Log("AppRequest Json: " + result.Json.ToString());
#if UNITY_WP8 || UNITY_WP_8_1
                }, title: "FaceFlip Invite");
#else
                });
#endif
            }
#endif
        }

        public void PostFeed()
        {
            Debug.Log("Post to Feed.");
#if !UNITY_EDITOR && UNITY_WINRT
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
#endif
        }

        // Parse the json and request the friend pictures.
        private void GetFriendsCallback(FBResult result)
        {
            if (result.Error != null)
            {
                Debug.Log("Failed to get FB Friends");
                return;
            }

            try
            {
                _facebookFriends.Clear();
                JsonData data = JsonMapper.ToObject(result.Text);

                JsonData friends = data["data"];
                for (int i = 0; i < friends.Count; ++i)
                {
                    JsonData friend = friends[i];
                    string name = (string)friend["name"];
                    string id = (string)friend["id"];
                    Texture2D texture = new Texture2D(128, 128, TextureFormat.DXT1, false);
                    StartCoroutine(GetFBPicture(id, texture));

                    _facebookFriends.Add(name, texture);
                }

                FacebookFriends = "Friends: " + _facebookFriends.Count;
                SetupTiles();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        // Load a picture into the texture.
        private IEnumerator GetFBPicture(string id, Texture2D texture)
        {
            WWW url = new WWW("https" + "://graph.facebook.com/" + id + "/picture?type=large");

            while (!url.isDone)
            {
                yield return null;
            }

            url.LoadImageIntoTexture(texture);
        }

        public void SetReminder()
        {
            ReminderScheduled = true;
            _reminderStartTime = DateTime.Now;
            PlayerPrefs.SetString("reminderStartTime", _reminderStartTime.ToString());
            ReminderInfo = ReminderTextPrefix + _reminderStartTime.AddSeconds(ReminderTime).ToString("hh:mm tt");

            Debug.Log("Set Reminder.");

#if !UNITY_EDITOR && UNITY_WINRT
            ReminderManager.SetRemindersStatus(true);
            ReminderManager.RegisterReminder("testID", "Face Flip", "This is a reminder.", DateTime.Now.AddSeconds(ReminderTime));
#endif
        }

        /// <summary>
        /// Check whether the reminder has expired, and reset the reminder info if it has.
        /// </summary>
        void CheckReminder()
        {
            DateTime reminderExpireTime = _reminderStartTime.AddSeconds(ReminderTime);

            if (DateTime.Compare(DateTime.Now, reminderExpireTime) > 0)
            {
                ReminderScheduled = false;
                ReminderInfo = NoReminderText;
            }
            else
            {
                ReminderScheduled = true;
            }
        }

        /// <summary>
        /// Switch off/Cancel reminder.
        /// Will be called from GameSettingsFlyout.
        /// </summary>
        public void CancelReminder()
        {
            ReminderScheduled = false;
            ForceResetReminderText = true;
            if (PlayerPrefs.HasKey("reminderStartTime"))
            {
                PlayerPrefs.DeleteKey("reminderStartTime");
            }

            Debug.Log("Remove Reminder.");
#if !UNITY_EDITOR && UNITY_WINRT
            ReminderManager.RemoveReminder("testID");
#endif
        }

        public void SendEmail()
        {
            Debug.Log("Send Email.");
#if !UNITY_EDITOR && UNITY_WINRT
            Helper.Instance.SendEmail("test@example.com;test2@example.com", "Hello!",
                "This is a test mail.\nBye!");
#endif
        }

        public void RetrieveProducts()
        {
            Debug.Log("Retrieve Products.");
#if !UNITY_EDITOR && UNITY_WINRT
            // retrieve store products.
            StoreManager.Instance.RetrieveProducts((products) =>
            {
                if (products != null)
                {
                    StoreProducts = products;
                    StoreProducts.Sort((a, b) => { return string.Compare(a.ProductID, b.ProductID); });
                }
                else
                {
                    Helper.Instance.ShowDialog("Please switch to Debug/QA build, store not functional in Master configuration.", "Error", null, "OK");
                }
            });
#endif
        }

        public void PurchaseMove(Product product)
        {
            Debug.Log("Purchase Move.");
#if !UNITY_EDITOR && UNITY_WINRT
            StoreManager.Instance.PurchaseProduct(product, (receipt) =>
            {
                if (receipt.Success)
                {
                    _remainingMoves += int.Parse(receipt.Product.Name.Split(' ')[0]);
                    Helper.Instance.ShowDialog("You now have " + _remainingMoves + " moves.", "Success", null, "OK");
                }
                else
                {
                    Helper.Instance.ShowDialog(receipt.Status.ToString(), "Error", null, "OK");
                }
            });
#endif
        }

        public void TryCatchPlatformNotSupportedException(Action action)
        {
            try
            {
                action();
            }
            catch (PlatformNotSupportedException e)
            {
                Debug.LogError(e);
            }
        }

        public void ShowShareUI()
        {
            Debug.Log("Show Share UI.");
#if !UNITY_EDITOR && UNITY_WINRT
#if UNITY_METRO
            Helper.Instance.ShowShareUI();
#else
            Helper.Instance.ShowShareUI("Title", "Message", "http://www.markermetro.com");
#endif
#endif
        }

        private void ThrowException()
        {
            throw new Exception("This is used to test ExtractStackTrace.");
        }

        /// There's a bug causing Video to remain on the screen after it finishes playing on WP8.
        /// Submitted a bug report to Unity: http://fogbugz.unity3d.com/default.asp?663800_4o1v5omb7fan6gfq
        public void PlayVideo()
        {
            Debug.Log("PlayVideo.");
#if !UNITY_EDITOR && UNITY_WINRT
            string path = Application.streamingAssetsPath + "/MarkerMetro/Example/ExampleVideo.mp4";
            VideoPlayer.PlayVideo(path, () =>
            {
                Debug.Log("Video Stopped.");
            }, VideoStretch.Uniform);
#endif
        }

        public void LogAppCrash()
        {
            ExceptionLogger.IsEnabled = true;
            ExceptionManager.Instance.DoAppCrashTest();
        }
    }
}