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
using UnityProject.Config;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace UnityProject.Win
{
    public sealed partial class GameSettingsFlyout : SettingsFlyout
    {
        public GameSettingsFlyout()
        {
            this.InitializeComponent();

            reminderSwitch.Visibility = AppConfig.Instance.NoticationsControlEnabled ? 
                Visibility.Visible : Visibility.Collapsed;
            musicSwitch.Visibility = soundSwitch.Visibility = AppConfig.Instance.MusicFXControlEnabled ?
                Visibility.Visible : Visibility.Collapsed;

            reminderSwitch.IsOn = ReminderManager.AreRemindersEnabled();
            
            // get the state of music/sound from the game
            musicSwitch.IsOn = GameMaster.MusicEnabled;
            soundSwitch.IsOn = GameMaster.SoundEnabled;
        }

        public void ActivateSound(bool active)
        {
            // if clause to not play the sound when GameSettingsFlyout is constructed.
            if (GameMaster.SoundEnabled != active)
            {
                GameMaster.SoundEnabled = active;
            }
        }

        public void ActivateMusic(bool active)
        {
            // if clause to not play the sound when GameSettingsFlyout is constructed.
            if (GameMaster.MusicEnabled != active)
            {
                GameMaster.MusicEnabled = active;
            }
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
            var value = musicSwitch.IsOn;
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() => ActivateMusic(value), false);
        }

        void soundSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var value = soundSwitch.IsOn;
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() => ActivateSound(value), false);
        }

        void reminderSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var value = reminderSwitch.IsOn;
            ReminderManager.SetRemindersStatus(value);
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() => ActivateReminder(value), false);
        }
    }
}
