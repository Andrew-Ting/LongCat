using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Transform playContinueButton;
    LevelData LevelDataCurrentlyHold;
    LevelCollection levelCollection;
    // Start is called before the first frame update
    void Start()
    {
        levelCollection = FindObjectOfType<LevelCollection>();
        GameData gameData = SaveLoadManager.LoadGameData();
        if(gameData != null)
        {
            //continue last game
            LevelDataCurrentlyHold = levelCollection.levelDataCollection[gameData.puzzleID];
            playContinueButton.Find("Text").GetComponent<Text>().text = "Continue\n" + LevelDataCurrentlyHold.worldNumber.ToString() + " - " + LevelDataCurrentlyHold.level.ToString();
        }
        else
        {
            LevelDataCurrentlyHold = levelCollection.levelDataCollection[0];
            playContinueButton.Find("Text").GetComponent<Text>().text = "Play";
        }
    }

    public void ContinueGame()
    {
        //should be level loader manager
        //FindObjectOfType<LevelLoader>().LoadGame(LevelDataCurrentlyHold.id);
        LevelLoader.LoadGame(LevelDataCurrentlyHold.id);
    }
}
