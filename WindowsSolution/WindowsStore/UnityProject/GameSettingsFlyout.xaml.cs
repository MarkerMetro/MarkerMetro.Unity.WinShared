using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MarkerMetro.Unity.WinIntegration.LocalNotifications;
using MarkerMetro.Unity.WinShared.Tools;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace UnityProject.Win
{
    public sealed partial class GameSettingsFlyout : SettingsFlyout
    {
        public GameSettingsFlyout()
        {
            this.InitializeComponent();

            var fm = FeaturesManager.Instance;
            reminderSwitch.Visibility = fm.IsSettingsNotificationsOnOffEnabled ? 
                Visibility.Visible : Visibility.Collapsed;
            musicSwitch.Visibility = soundSwitch.Visibility = fm.IsSettingsMusicFXOnOffEnabled ? 
                Visibility.Visible : Visibility.Collapsed;

            // get the state of music/sound from the game
            //musicSwitch.IsOn = PlayerPrefs.GetInt(Constants.optMusic) == 0 ? false : true;
            //soundSwitch.IsOn = PlayerPrefs.GetInt(Constants.optSFX) == 0 ? false : true;
            reminderSwitch.IsOn = ReminderManager.AreRemindersEnabled();
        }

        public void ActivateSound(bool active)
        {
            // control sound in game
            //if (active)
            //    NGUITools.soundVolume = 1f;
            //else
            //    NGUITools.soundVolume = 0f;
            //OptionsScreen.WRT_FireSoundsChanged(active);
        }

        public void ActivateMusic(bool active)
        {
            // control music in game
            //if (active)
            //    SoundEngine.Play(UnityEngine.Resources.Load<AudioClip>(Constants.MUSIC_LOOP_NORMAL), Vector3.zero, .75f, 1f, true);
            //else
            //    SoundEngine.StopAllCurrentSounds();
            //OptionsScreen.WRT_FireMusicChanged(active);
        }

        public void ActivateReminder(bool active)
        {
            if (!active)
            {
                GameMaster.CancelReminder();
            }
        }

        void musicSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("musicSwitch_Toggled: " + musicSwitch.IsOn);
            var value = musicSwitch.IsOn;
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() => ActivateMusic(value), false);
            //PlayerPrefs.SetInt(Constants.optMusic, value ? 1 : 0);
        }

        void soundSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("soundSwitch_Toggled: " + soundSwitch.IsOn);
            var value = soundSwitch.IsOn;
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() => ActivateSound(value), false);
            //PlayerPrefs.SetInt(Constants.optSFX, value ? 1 : 0);
        }

        void reminderSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("reminderSwitch_Toggled: " + reminderSwitch.IsOn);
            var value = reminderSwitch.IsOn;
            ReminderManager.SetRemindersStatus(value);
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() => ActivateReminder(value), false);
        }
    }
}
