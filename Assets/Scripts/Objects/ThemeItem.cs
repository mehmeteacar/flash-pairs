using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Objects
{
    public class ThemeItem : MonoBehaviour
    {
        public Image themeImage;
        public TextMeshProUGUI themeNameText;
        public Button button;
        private ThemeData themeData;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            if (button == null) { button = gameObject.AddComponent<Button>(); }

            button.onClick.AddListener(SelectTheme);
        }
        
        public void SetThemeData(ThemeData theme)
        {
            themeData = theme;
            if (themeImage != null)
            {
                themeImage.sprite = theme.themeIcon;
            }
            if (themeNameText != null)
            {
                themeNameText.text = (themeNameText.text.Contains("\n") ? 
                    theme.themeName + themeNameText.text.Substring(themeNameText.text.IndexOf('\n')) : 
                    theme.themeName);
            }
        }
        
        private void SelectTheme()
        {
            UIManager.Instance.OnThemeSelected(themeData);
        }
        
        void OnDestroy()
        {
            button.onClick.RemoveListener(SelectTheme);
        }
    }
}