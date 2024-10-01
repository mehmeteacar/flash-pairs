using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managers
{
    public class ThemeManager : Singleton<ThemeManager>
    {
        private List<ThemeData> availableThemes;
        private LevelGenerator levelGenerator;
        
        public int lastFinishedLevel;

        protected override void Awake()
        {
            base.Awake();
            levelGenerator = FindObjectOfType<LevelGenerator>();
            LoadThemesFromResources();
        }

        private void LoadThemesFromResources()
        {
            availableThemes = new List<ThemeData>(Resources.LoadAll<ThemeData>("Themes"));
            if (availableThemes.Count == 0)
            {
                Debug.LogError("No themes found in the Resources/Themes folder.");
            }
            else
            {
                levelGenerator.InitializeLevelGeneration(availableThemes);
            }   
        }
        
        public void SaveThemeProgress(string themeName, int finishedLevel)
        {
            if (finishedLevel > lastFinishedLevel)
            {
                lastFinishedLevel = finishedLevel;

                PlayerPrefs.SetInt($"Theme_{themeName}_LastFinishedLevel", lastFinishedLevel);
                PlayerPrefs.Save();
            }
        }
        
        public void LoadThemeProgress(string themeName)
        {
            lastFinishedLevel = PlayerPrefs.GetInt($"Theme_{themeName}_LastFinishedLevel", 0);
        }

        public List<ThemeData> GetAvailableThemes()
        {
            return availableThemes;
        }
    }
}