using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Transform playContinueButton;
    LevelData LevelDataCurrentlyHold;
    LevelCollection levelCollection;
    void Start()
    {
        levelCollection = FindObjectOfType<LevelCollection>();
        GameData gameData = SaveLoadManager.LoadGameData();
        if(gameData != null)
        {
            //continue last game
            LevelDataCurrentlyHold = levelCollection.levelDataCollection[gameData.puzzleID];
            playContinueButton.Find("Text").GetComponent<Text>().text = "Continue\n" + LevelDataCurrentlyHold.GetWorldLevelString();
        }
        else
        {
            LevelDataCurrentlyHold = levelCollection.levelDataCollection[0];
            playContinueButton.Find("Text").GetComponent<Text>().text = "Play";
        }
    }

    public void ContinueGame()
    {
        LevelLoader.LoadGame(LevelDataCurrentlyHold.id);
    }
}
