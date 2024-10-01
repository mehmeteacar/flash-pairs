using System.Collections.Generic;
using Data;
using Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : SceneSingleton<UIManager>
    {
        public Image backgroundImage;

        public Button soundButton;
        public Button musicButton;
        public Button playButton;
        public Button backButtonThemeView;
        public Button backButtonLevelView;

        public GameObject logo;
        public GameObject buttonsContainer;
        public GameObject themesView;
        public GameObject levelsView;

        public Sprite soundOnSprite;
        public Sprite soundOffSprite;
        public Sprite musicOnSprite;
        public Sprite musicOffSprite;

        public GameObject themeItemPrefab;
        public Transform themeGridLayout;

        public Transform levelGridLayout;
        public GameObject levelItemPrefab;
        
        public Sprite tickIcon;
        public Color disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        
        void Start()
        {
            InitializeUI();
        }
        
        public void InitializeUI()
        {
            ExpandBackground();

            UpdateButtonSprites();

            soundButton.onClick.RemoveAllListeners();
            soundButton.onClick.AddListener(ToggleSound);

            musicButton.onClick.RemoveAllListeners();
            musicButton.onClick.AddListener(ToggleMusic);

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayButtonClicked);

            backButtonThemeView.onClick.RemoveAllListeners();
            backButtonThemeView.onClick.AddListener(OnBackButtonClicked);

            backButtonLevelView.onClick.RemoveAllListeners();
            backButtonLevelView.onClick.AddListener(OnBackButtonClicked);
        }

        void ExpandBackground()
        {
            if (backgroundImage == null)
            {
                Debug.LogError("Background Image is not assigned.");
                return;
            }

            RectTransform rectTransform = backgroundImage.GetComponent<RectTransform>();

            float imageAspectRatio = backgroundImage.sprite.bounds.size.x / backgroundImage.sprite.bounds.size.y;

            float screenAspectRatio = (float)Screen.width / Screen.height;

            if (imageAspectRatio > screenAspectRatio)
            {
                float scaleFactor = (float)Screen.height / rectTransform.sizeDelta.y;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * scaleFactor, Screen.height);
            }
            else
            {
                float scaleFactor = (float)Screen.width / rectTransform.sizeDelta.x;
                rectTransform.sizeDelta = new Vector2(Screen.width, rectTransform.sizeDelta.y * scaleFactor);
            }

            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void ToggleSound()
        {
            AudioManager.Instance.ToggleSFX();
            UpdateButtonSprites();
        }

        private void ToggleMusic()
        {
            AudioManager.Instance.ToggleMusic();
            UpdateButtonSprites();
        }

        private void UpdateButtonSprites()
        {
            soundButton.image.sprite = SaveManager.Instance.soundOn ? soundOnSprite : soundOffSprite;
            musicButton.image.sprite = SaveManager.Instance.musicOn ? musicOnSprite : musicOffSprite;
        }
        
        private void UpdateLevelItemStatus(LevelItem levelItem, int levelIndex, int lastFinishedLevel)
        {
            Button levelButton = levelItem.GetComponent<Button>();
            Image levelImage = levelItem.GetComponent<Image>();
            
            if (levelIndex <= lastFinishedLevel + 1)
            {
                levelButton.interactable = true;
                levelImage.color = Color.white;

                if (levelIndex < lastFinishedLevel + 1) 
                {
                    levelItem.EnableTickIcon(true);
                }
                else
                {
                    levelItem.EnableTickIcon(false);
                }
            }
            else
            {
                levelButton.interactable = false;
                levelImage.color = disabledColor;
                levelItem.EnableTickIcon(false);
            }
        }

        private void OnPlayButtonClicked()
        {
            AudioManager.Instance.PlaySFX(0);
            logo.SetActive(false);
            buttonsContainer.SetActive(false);

            themesView.SetActive(true);

            PopulateThemeItems();
        }

        public void OnThemeSelected(ThemeData selectedTheme)
        {
            AudioManager.Instance.PlaySFX(0);
            Debug.Log("Selected theme: " + selectedTheme.themeName);
            themesView.SetActive(false);
            levelsView.SetActive(true);

            PopulateLevelItems(selectedTheme);
        }

        private void PopulateThemeItems()
        {
            if (themeItemPrefab == null || themeGridLayout == null)
            {
                Debug.LogError("Theme item prefab or grid layout is not assigned.");
                return;
            }

            foreach (Transform child in themeGridLayout)
            {
                Destroy(child.gameObject);
            }

            List<ThemeData> availableThemes = ThemeManager.Instance.GetAvailableThemes();

            foreach (ThemeData theme in availableThemes)
            {
                GameObject themeItem = Instantiate(themeItemPrefab, themeGridLayout);

                ThemeItem themeItemScript = themeItem.GetComponent<ThemeItem>();
                if (themeItemScript != null)
                {
                    themeItemScript.SetThemeData(theme);
                }
            }
        }

        private void PopulateLevelItems(ThemeData selectedTheme)
        {
            foreach (Transform child in levelGridLayout)
            {
                Destroy(child.gameObject);
            }
            
            ThemeManager.Instance.LoadThemeProgress(selectedTheme.themeName);
            int lastFinishedLevel = ThemeManager.Instance.lastFinishedLevel;

            foreach (LevelData level in selectedTheme.levels)
            {
                GameObject levelItemObj = Instantiate(levelItemPrefab, levelGridLayout);
                LevelItem levelItem = levelItemObj.GetComponent<LevelItem>();
                if (levelItem != null)
                {
                    levelItem.SetLevelData(level, selectedTheme);
                    UpdateLevelItemStatus(levelItem, level.levelIndex, lastFinishedLevel);
                }
            }
        }


        private void OnBackButtonClicked()
        {
            if (levelsView.activeSelf)
            {
                levelsView.SetActive(false);
                themesView.SetActive(true);
            }
            else if (themesView.activeSelf)
            {
                themesView.SetActive(false);
                logo.SetActive(true);
                buttonsContainer.SetActive(true);
            }
            
            AudioManager.Instance.PlaySFX(0);
        }
    }
}
