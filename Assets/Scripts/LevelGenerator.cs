using System.Collections.Generic;
using System.IO;
using Data;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private int seed = 12345;
    private List<ThemeData> themes;
    private string filePath;

    public void InitializeLevelGeneration(List<ThemeData> availableThemes)
    {
        themes = availableThemes;

        filePath = Path.Combine(Application.persistentDataPath, "levels.json");

        if (!File.Exists(filePath))
        {
            GenerateLevels();
            SaveLevelsToFile();
        }
        else
        {
            LoadLevelsFromFile();
        }
    }

    void GenerateLevels()
    {
        System.Random random = new System.Random(seed);

        foreach (ThemeData theme in themes)
        {
            theme.levels = new List<LevelData>();
            int rows = theme.minRows;
            int columns = theme.minColumns;

            for (int i = 0; i < theme.levelCount; i++)
            {
                if (i > 0)
                {
                    if (rows == columns)
                    {
                        if (columns >= theme.maxColumns && rows < theme.maxRows)
                        {
                            rows++;
                        }
                        else if (rows >= theme.maxRows && columns < theme.maxColumns)
                        {
                            columns++;
                        }
                        else if (rows < theme.maxRows && columns < theme.maxColumns)
                        {
                            int option = random.Next(0, 3);
                            if (option == 0 && rows < theme.maxRows)
                            {
                                rows++;
                            }
                            else if (option == 1 && columns < theme.maxColumns)
                            {
                                columns++;
                            }
                            else if (option == 2 && rows < theme.maxRows && columns < theme.maxColumns)
                            {
                                rows++;
                                columns++;
                            }
                        }
                    }
                    else if (rows > columns && columns < theme.maxColumns)
                    {
                        columns++;
                    }
                    else if (columns > rows && rows < theme.maxRows)
                    {
                        rows++;
                    }
                    else if (rows >= theme.maxRows && columns < theme.maxColumns)
                    {
                        columns++;
                    }
                    else if (columns >= theme.maxColumns && rows < theme.maxRows)
                    {
                        rows++;
                    }
                }

                int totalTiles = rows * columns;

                if (totalTiles % 2 != 0)
                {
                    if (columns < theme.maxColumns)
                    {
                        columns++;
                    }
                    else if (rows < theme.maxRows)
                    {
                        rows++;
                    }
                
                    totalTiles = rows * columns;
                }

                List<int> tileIDs = new List<int>();
                List<int> availableTileIDs = new List<int>(theme.tileSet.Count);

                for (int j = 0; j < theme.tileSet.Count; j++)
                {
                    availableTileIDs.Add(theme.tileSet[j].tileID);
                }

                for (int j = 0; j < totalTiles / 2; j++)
                {
                    if (availableTileIDs.Count == 0)
                    {
                        availableTileIDs = new List<int>(theme.tileSet.Count);
                        for (int k = 0; k < theme.tileSet.Count; k++)
                        {
                            availableTileIDs.Add(theme.tileSet[k].tileID);
                        }
                    }

                    int randomIndex = random.Next(availableTileIDs.Count);
                    int tileID = availableTileIDs[randomIndex];
                    availableTileIDs.RemoveAt(randomIndex);

                    tileIDs.Add(tileID);
                    tileIDs.Add(tileID);
                }

                for (int j = 0; j < tileIDs.Count; j++)
                {
                    int temp = tileIDs[j];
                    int randomIndex = random.Next(0, tileIDs.Count);
                    tileIDs[j] = tileIDs[randomIndex];
                    tileIDs[randomIndex] = temp;
                }

                LevelData levelData = new LevelData
                {
                    rows = rows,
                    columns = columns,
                    tileIDs = tileIDs,
                    levelIndex = i + 1
                };
                theme.levels.Add(levelData);
            }
        }
    }

    void SaveLevelsToFile()
    {
        ThemeDataWrapper wrapper = new ThemeDataWrapper { themes = themes };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    void LoadLevelsFromFile()
    {
        string json = File.ReadAllText(filePath);
        ThemeDataWrapper wrapper = JsonUtility.FromJson<ThemeDataWrapper>(json);
        themes = wrapper.themes;
    }
}
