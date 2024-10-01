using System.Collections.Generic;

namespace Data
{
    [System.Serializable]
    public class LevelData
    {
        public int levelIndex;
        public int rows;
        public int columns;
        public List<int> tileIDs;
    }
}