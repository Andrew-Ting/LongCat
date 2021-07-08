using UnityEngine;
using System.Collections.Generic;

public class LevelCollection : MonoBehaviour
{
    public LevelData[] levelDataCollection;
    public int worlds;
    public Dictionary<int, List<LevelData>> levelDataSorted;

    static LevelCollection instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance.gameObject);

        if (LevelChecker())
        {
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
    }

    bool LevelChecker()
    {
        bool [] levelArray = new bool[levelDataCollection.Length];
        for (int i = 0; i < levelDataCollection.Length; i++)
        {
            if (levelDataCollection[i].id < levelDataCollection.Length)
            {
                if (!levelArray[levelDataCollection[i].id])
                {
                    levelArray[i] = true;
                }
                else
                {
                    Debug.Log("checker failed: repeat id @ " + i);
                    return false;
                }
            }
            else
            {
                Debug.Log("checker failed: id is greater");
                return false;
            }
        }
        return true;
    }
}
