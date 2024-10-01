using UnityEngine;

namespace Managers
{
    public class SaveManager : Singleton<SaveManager>
    {
        public bool soundOn { get; set; }
        public bool musicOn { get; set; }
        public int[] highestLevelUnlocked { get; private set; }

        private const string SoundOnKey = "SoundOn";
        private const string MusicOnKey = "MusicOn";
        private const string HighestLevelUnlockedKeyPrefix = "HighestLevelUnlocked_";

        private const int NumberOfThemes = 4;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
        }

        public void SetSoundOn(bool value)
        {
            soundOn = value;
            PlayerPrefs.SetInt(SoundOnKey, soundOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetMusicOn(bool value)
        {
            musicOn = value;
            PlayerPrefs.SetInt(MusicOnKey, musicOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetHighestLevelUnlocked(int themeIndex, int level)
        {
            if (themeIndex < 0 || themeIndex >= NumberOfThemes)
            {
                Debug.LogError("Invalid theme index");
                return;
            }

            highestLevelUnlocked[themeIndex] = level;
            PlayerPrefs.SetInt(HighestLevelUnlockedKeyPrefix + themeIndex, highestLevelUnlocked[themeIndex]);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            soundOn = PlayerPrefs.GetInt(SoundOnKey, 1) == 1;
            musicOn = PlayerPrefs.GetInt(MusicOnKey, 1) == 1;
            highestLevelUnlocked = new int[NumberOfThemes];

            for (int i = 0; i < NumberOfThemes; i++)
            {
                highestLevelUnlocked[i] = PlayerPrefs.GetInt(HighestLevelUnlockedKeyPrefix + i, 1);
            }
        }
    }
}