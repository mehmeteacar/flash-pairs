using Data;
using TMPro;
using UnityEngine;
using Managers;
using UnityEngine.SceneManagement;

namespace Objects
{
    public class LevelItem : MonoBehaviour
    {
        public TextMeshProUGUI levelLabel;
        public GameObject tickIcon;
        private LevelData levelData;
        private ThemeData themeData;

        public void SetLevelData(LevelData data, ThemeData theme)
        {
            levelData = data;
            themeData = theme;

            if (levelLabel != null)
            {
                levelLabel.text = $"{levelData.levelIndex}";
            }
        }

        public void OnSelectLevel()
        {
            AudioManager.Instance.PlaySFX(0);
            
            Debug.Log("Selected Level: " + levelData.levelIndex);
            
            LevelLoader.SelectedTheme = themeData;
            LevelLoader.SelectedLevelIndex = levelData.levelIndex - 1;
            
            SceneManager.LoadScene("Game");
        }

        public LevelData GetLevelData()
        {
            return levelData;
        }
        
        public void EnableTickIcon(bool enable)
        {
            if (tickIcon != null)
            {
                tickIcon.SetActive(enable);
            }
        }
    }
}