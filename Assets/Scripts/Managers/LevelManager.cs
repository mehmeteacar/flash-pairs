using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public enum GameStatus
    {
        InGame,
        Finished,
        Paused
    }   
    
    public class LevelManager : SceneSingleton<LevelManager>
    {
        public GameObject cardPrefab;
        public ThemeData currentTheme;
        public RectTransform cardGridPanel;
        public GridLayoutGroup gridLayoutGroup;
        public float appearanceDelay = 0.08f;
        public float startDelay = 0.0f; 
        
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI pairsLeftText;
        public Button menuButton;
        public GameObject levelCompletionPanel;
        public TextMeshProUGUI levelCompleteText;
        public TextMeshProUGUI timeElapsedText;
        
        public Button homeButton;
        public Button nextLevelButton;

        private float elapsedTime;
        private int pairsLeft;
        
        private Card firstFlippedCard;
        private Card secondFlippedCard;
        private bool isFlipping = false;
        
        public GameStatus gameStatus;
        
        private void Start()
        {
            if (LevelLoader.SelectedTheme != null)
            {
                currentTheme = LevelLoader.SelectedTheme;
                LoadLevel(LevelLoader.SelectedLevelIndex);
            }
            else
            {
                Debug.LogError("No theme or level data found in LevelLoader.");
            }
            
            menuButton.onClick.AddListener(ReturnToMenu);
            homeButton.onClick.AddListener(ReturnToMenu);
            nextLevelButton.onClick.AddListener(ProceedToNextLevel);
            
            gameStatus = GameStatus.Paused;
        }
        
        private void Update()
        {
            if (gameStatus == GameStatus.InGame)
            {
                elapsedTime += Time.deltaTime;
                System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
                timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            }
        }

        private void LoadLevel(int levelIndex)
        {
            if (currentTheme == null || currentTheme.levels == null || currentTheme.levels.Count == 0)
            {
                Debug.LogError("No levels found for the current theme.");
                return;
            }

            LevelData levelData = currentTheme.levels[levelIndex];

            float panelWidth = cardGridPanel.rect.width;
            float panelHeight = cardGridPanel.rect.height;

            float cellSizeBasedOnWidth = (panelWidth - gridLayoutGroup.spacing.x * (levelData.columns - 1)) / levelData.columns;
            float cellSizeBasedOnHeight = (panelHeight - gridLayoutGroup.spacing.y * (levelData.rows - 1)) / levelData.rows;

            float cellSize = Mathf.Min(cellSizeBasedOnWidth, cellSizeBasedOnHeight);

            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

            foreach (Transform child in gridLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
            
            pairsLeft = levelData.tileIDs.Count / 2;
            pairsLeftText.text = $"Left: {pairsLeft}";
            
            elapsedTime = 0f;
            gameStatus = GameStatus.Paused;

            StartCoroutine(AppearCardsSequentially(levelData));
        }
        
        public void OnCardClicked(Card clickedCard)
        {
            if (gameStatus != GameStatus.InGame)
                return;
            
            if (isFlipping || clickedCard.IsMatched() || (firstFlippedCard != null && clickedCard == firstFlippedCard))
                return;

            if (firstFlippedCard == null)
            {
                firstFlippedCard = clickedCard;
                FlipCard(clickedCard);
            }
            else if (secondFlippedCard == null)
            {
                secondFlippedCard = clickedCard;
                FlipCard(clickedCard);

                StartCoroutine(CheckForMatch());
            }
        }
        
        private void FlipCard(Card card)
        {
            AudioManager.Instance.PlaySFX(1);
            isFlipping = true;
            card.FlipCard().OnComplete(() =>
            {
                isFlipping = false;
            });
        }
        
        private void FlipBackCards()
        {
            if (firstFlippedCard != null)
            {
                firstFlippedCard.FlipBackCard();
            }
            if (secondFlippedCard != null)
            {
                secondFlippedCard.FlipBackCard();
            }
        }
        
        private IEnumerator CheckForMatch()
        {
            while (isFlipping)
            {
                yield return null;
            }

            if (firstFlippedCard.IsMatching(secondFlippedCard))
            {
                OnPairMatched();
            }
            else
            {
                yield return new WaitForSeconds(1f);
                FlipBackCards();
            }

            firstFlippedCard = null;
            secondFlippedCard = null;
        }
        
        public void OnPairMatched()
        {
            firstFlippedCard.MarkAsMatched();
            secondFlippedCard.MarkAsMatched();

            pairsLeft--;
            pairsLeftText.text = $"Left: {pairsLeft}";

            if (pairsLeft <= 0)
            {
                gameStatus = GameStatus.Finished;
                Debug.Log("Level Completed!");
                
                if (LevelLoader.SelectedLevelIndex + 1 > ThemeManager.Instance.lastFinishedLevel)
                {
                    ThemeManager.Instance.SaveThemeProgress(currentTheme.themeName, LevelLoader.SelectedLevelIndex+1);
                    Debug.Log("Progress Saved.");
                }

                StartCoroutine(ShowLevelCompletionPanel(1.0f));
            }
        }
        
        private IEnumerator ShowLevelCompletionPanel(float delay)
        {
            yield return new WaitForSeconds(delay);

            levelCompletionPanel.SetActive(true);

            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
            timeElapsedText.text = $"in {timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            
            levelCompleteText.text = $"Level {LevelLoader.SelectedLevelIndex + 1}\nCompleted";
            
            if (LevelLoader.SelectedLevelIndex >= currentTheme.levels.Count - 1)
                nextLevelButton.interactable = false;
            else
                nextLevelButton.interactable = true;

        }
        
        private IEnumerator AppearCardsSequentially(LevelData levelData)
        {
            List<Card> allCards = new List<Card>();

            for (int row = 0; row < levelData.rows; row++)
            {
                for (int col = 0; col < levelData.columns; col++)
                {
                    int index = row * levelData.columns + col;
                    int tileID = levelData.tileIDs[index];
                    TileData tileData = currentTheme.tileSet.Find(t => t.tileID == tileID);

                    GameObject card = Instantiate(cardPrefab, gridLayoutGroup.transform);
                    Card cardScript = card.GetComponent<Card>();
                    cardScript.Initialize(tileData);

                    allCards.Add(cardScript);

                    cardScript.AppearCard(0.2f);

                    yield return new WaitForSeconds(appearanceDelay);
                }
            }

            yield return new WaitForSeconds(0.6f);

            foreach (Card card in allCards)
            {
                card.FlipCard();
            }

            yield return new WaitForSeconds(1.2f);

            foreach (Card card in allCards)
            {
                card.FlipBackCard();
            }

            yield return new WaitForSeconds(startDelay);
            gameStatus = GameStatus.InGame;
        }
        
        private void ReturnToMenu()
        {
            AudioManager.Instance.PlaySFX(0);
            
            SceneManager.LoadScene("Menu");
        }
        
        private void ProceedToNextLevel()
        {
            AudioManager.Instance.PlaySFX(0);
            
            if (LevelLoader.SelectedLevelIndex < currentTheme.levels.Count - 1)
            {
                LevelLoader.SelectedLevelIndex++;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
