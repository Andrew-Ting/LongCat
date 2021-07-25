public static class LevelLoader
{
    public static void LoadNextGame(int id)//this is used in game. Load the next game of id
    {
        //finds next id
        LevelCollection levelCollection = UnityEngine.Object.FindObjectOfType<LevelCollection>();
        SaveLoadManager.SaveGameData(UnityEngine.Object.FindObjectOfType<LevelCollection>().nextLevelID(id));
        //find next level id. to be done later
        UnityEngine.Object.FindObjectOfType<TransitionManager>().OpenScene(1, 0);
    }

    public static void LoadGame(int id) //this is game ID
    {
        SaveLoadManager.SaveGameData(id);
        UnityEngine.Object.FindObjectOfType<TransitionManager>().OpenScene(1, 0); //edit this when menu and startup is finished.
    }
}
