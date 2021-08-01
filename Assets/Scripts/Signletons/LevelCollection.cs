using UnityEngine;
using System.Collections.Generic;

public class LevelCollection : MonoBehaviour
{
    public LevelData[] levelDataCollection;
    public int worlds;
    public Dictionary<int, List<LevelData>> levelDataSorted;

    private static LevelCollection instance;

    void Awake()
    {
        for(int i = 0; i < levelDataCollection.Length; i++)
        {
            levelDataCollection[i].id = i;
        }

        levelDataSorted = new Dictionary<int, List<LevelData>>();
        for(int i = 0; i < levelDataCollection.Length; i++)
        {
            if(!levelDataSorted.ContainsKey(levelDataCollection[i].worldNumber))
            {
                levelDataSorted.Add(levelDataCollection[i].worldNumber, new List<LevelData>());
            }
            levelDataSorted[levelDataCollection[i].worldNumber].Add(levelDataCollection[i]);
        }
    }
    public int GetNextLevelID(int currentID)
    {
        LevelData currentLevel = levelDataCollection[currentID];
        if (currentLevel.levelNumber == levelDataSorted[currentLevel.worldNumber - 1].Count)
        {
            //is last id, get next world first id
            if(currentLevel.worldNumber == worlds - 1)
            {
                //last world
                return -2;
            }
            else
            {
                return levelDataSorted[currentLevel.worldNumber + 1][0].id; 
                // assumes that the first instance in the list is the first level of the next world
            }
        }
        else
        {
            return levelDataSorted[currentLevel.worldNumber][levelDataSorted[currentLevel.worldNumber].IndexOf(currentLevel) + 1].id;
        } 
    }
}
