public static class LevelLoader
{
    public static void LoadNextGame(int id)//this is used in game. Load the next game of id
    {
        //find next id
        SaveLoadManager.SaveGameData(id);
        UnityEngine.Object.FindObjectOfType<TransitionManager>().OpenScene(id, 0);
    }

    public static void LoadGame(int id) //this is game ID
    {
        SaveLoadManager.SaveGameData(id);
        UnityEngine.Object.FindObjectOfType<TransitionManager>().OpenScene(id + 1, 0); //edit this when 
    }
}
