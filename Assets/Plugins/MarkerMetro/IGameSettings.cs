using System.Collections.Generic;

namespace MarkerMetro.Unity.WinShared
{
    /// <summary>
    /// Defines setting managed by the game
    /// </summary>
    public interface IGameSettings
    {
        bool MusicEnabled { get; set; }
        bool SoundEnabled { get; set; }
        void CancelReminder();
    }
}
