public static class LevelLoader
{
    //static is temporary. Might need this if I need to remove the static
    /*
    static GameObject instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    */

    public static void LoadNextGame(int id)//this is used in game. Load the next game of id
    {
        //find next id
        SaveLoadManager.SaveGameData(id);
        UnityEngine.Object.FindObjectOfType<TransitionManager>().OpenScene(id, 0);
    }

    public static void LoadGame(int id) //this is game ID
    {
        SaveLoadManager.SaveGameData(id);
        UnityEngine.Object.FindObjectOfType<TransitionManager>().OpenScene(id + 1, 0);
    }
}
