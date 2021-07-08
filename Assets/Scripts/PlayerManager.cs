using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    private LevelCollection levelCollection;

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
        //load stuff

        unlockedWorlds = new bool[levelCollection.worlds];
        unlockedID = new bool[levelCollection.levelDataCollection.Length];

        //use array.CopyTo(<old array>, 0)
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
