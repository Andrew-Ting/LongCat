using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    private LevelCollection levelCollection;

    //all data
    private bool[] unlockedWorlds;
    private Dictionary<int, Level> unlockedID;

    public void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance.gameObject);

        levelCollection = FindObjectOfType<LevelCollection>();

        unlockedWorlds = new bool[levelCollection.worlds];
        unlockedID = new Dictionary<int, Level>();
        //load stuff
        PlayerData data = SaveLoadManager.LoadPlayer();
        if(data != null)
        {
            //existing player
            SetData(data);
        }
        else
        {
            //new player
            UnlockWorld(0);
            UnlockID(0);
        }

        //for testing purposes only
        /*if(Application.isEditor)
        {
            for (int i = 0; i < unlockedWorlds.Length; i++)
            {
                UnlockWorld(i);
            }
            for (int i = 0; i < levelCollection.levelDataCollection.Length; i++)
            {
                UnlockID(i);
            }
        }*/
    }

    private void SetData(PlayerData data) //only done when data exists
    {
        data.worlds.CopyTo(unlockedWorlds, 0);
        unlockedID = data.ids;
    }

    //Getting stuff
    public bool IsUnlockedWorld(int n)
    {
        return unlockedWorlds[n];
    }
    public bool IsUnlockedID(int n)
    {
        return unlockedID.ContainsKey(n);
    }
    public bool IsFinished(int n)
    {
        if (unlockedID.ContainsKey(n)) return unlockedID[n].finished;
        return false;
    }
    public bool IsFished(int n)
    {
        if (unlockedID.ContainsKey(n)) return unlockedID[n].fished;
        return false;
    }
    public bool[] GetWorlds()
    {
        return unlockedWorlds;
    }
    public Dictionary<int, Level> GetIDs()
    {
        return unlockedID;
    }

    //unlocking stuff
    public void UnlockWorld(int n) // n is world number
    {
        if(!unlockedWorlds[n]) // if not yet unlocked
        {
            unlockedWorlds[n] = true;
        }
        else
        {
            Debug.Log("world already unlocked??");
        }
    }
    public void UnlockID(int n) // n is id
    {
        if (!unlockedID.ContainsKey(n)) // if not yet unlocked
        {
            unlockedID.Add(n, new Level(n));
        }
        else
        {
            Debug.Log("ID already unlocked??");
        }
    }
    public void LevelFinished(int n)
    {
        if(unlockedID.ContainsKey(n))
        {
            //safe
            unlockedID[n].finished = true;
            UnlockID(levelCollection.GetNextLevelID(n));
            SaveLoadManager.SavePlayer(this);
        }
        else
        {
            Debug.Log("something fishy going on");
        }
    }
    public void LevelFished(int n)
    {
        if (unlockedID.ContainsKey(n))
        {
            //safe
            unlockedID[n].fished = true;
            SaveLoadManager.SavePlayer(this);
        }
        else
        {
            Debug.Log("something very fishy going on");
        }
    }

}
