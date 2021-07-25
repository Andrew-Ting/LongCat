using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject cat;

    LevelCollection levelCollection;
    // Start is called before the first frame update
    private void Awake()
    {
        cat.gameObject.SetActive(false);
    }

    void Start()
    {
        levelCollection = FindObjectOfType<LevelCollection>();
        GameData gameData = SaveLoadManager.LoadGameData();
        if(gameData == null)
        {
            Debug.Log("no game data");
            return;
        }
        else
        {
            //do loading
            Debug.Log(gameData.puzzleID);
            SceneManager.LoadScene(gameData.puzzleID + 2, LoadSceneMode.Additive);
            cat.transform.position = levelCollection.levelDataCollection[gameData.puzzleID].catStartingPos;
            cat.gameObject.SetActive(true);
        }
    }
}
