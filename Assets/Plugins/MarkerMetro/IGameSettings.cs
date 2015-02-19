using System.Collections.Generic;

namespace MarkerMetro.Unity.WinShared
{
    /// <summary>
    /// Defines settings values required by the game
    /// </summary>
    public interface IGameSettings
    {
        bool MusicEnabled { get; set; }
        bool SoundEnabled { get; set; }
        bool RemindersEnabled { get; set; }
    }
}
