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
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance.gameObject);

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
}
