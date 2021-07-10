using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    private LevelCollection levelCollection;

    //all data
    private bool[] unlockedWorlds;
    private bool[] unlockedID;

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
        unlockedID = new bool[levelCollection.levelDataCollection.Length];

        //load stuff
        PlayerData data = SaveLoadManager.LoadPlayer();
        if(data != null)
        {
            //existing player
            SetData(data);
        }

        //for testing purposes only
        if(Application.isEditor)
        {
            for(int i = 0; i < unlockedWorlds.Length; i++)
            {
                UnlockWorld(i);
            }
            for (int i = 0; i < unlockedID.Length; i++)
            {
                UnlockID(i);
            }
        }
    }

    private void SetData(PlayerData data) //only done when 
    {
        data.worlds.CopyTo(unlockedWorlds, 0);
        data.id.CopyTo(unlockedID, 0);
    }

    //Getting stuff
    public bool IsUnlockedWorld(int n)
    {
        return unlockedWorlds[n];
    }
    public bool IsUnlockedID(int n)
    {
        return unlockedID[n];
    }
    public bool[] GetWorlds()
    {
        return unlockedWorlds;
    }
    public bool[] GetIDs()
    {
        return unlockedID;
    }

    //unlocking stuff
    public void UnlockWorld(int n)
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
    public void UnlockID(int n)
    {
        if (!unlockedID[n]) // if not yet unlocked
        {
            unlockedID[n] = true;
        }
        else
        {
            Debug.Log("ID already unlocked??");
        }
    }
}
