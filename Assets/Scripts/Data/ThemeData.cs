using UnityEngine;
using System.Collections.Generic;

namespace Data
{
    [CreateAssetMenu(fileName = "ThemeData", menuName = "ScriptableObjects/ThemeData", order = 3)]
    public class ThemeData : ScriptableObject
    {
        public string themeName;
        public Sprite themeIcon;
        public List<TileData> tileSet;
        public List<LevelData> levels;

        public int minRows;
        public int maxRows;
        public int minColumns;
        public int maxColumns;
        public int levelCount;
    }
}