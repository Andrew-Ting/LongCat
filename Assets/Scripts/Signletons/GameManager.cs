using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] CatMovement cat;

    LevelCollection levelCollection;
    // Start is called before the first frame update
    private void Awake()
    {
        levelCollection = FindObjectOfType<LevelCollection>();
        #if UNITY_EDITOR
        if (levelCollection == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        #endif 
        cat.gameObject.SetActive(false);
        GameData gameData = SaveLoadManager.LoadGameData();
        if (gameData == null)
        {
            Debug.Log("no game data");
            return;
        }
        else
        {
            //do loading
            LevelData levelData = levelCollection.levelDataCollection[gameData.puzzleID];
            SceneManager.LoadScene(levelData.GetSceneBuildIndex(), LoadSceneMode.Additive);
            cat.transform.position = levelData.catStartingPos;
            cat.SetCatHeight(levelData.catStartingHeight);
            cat.gameObject.SetActive(true);
            cat.StartSetObject();
        }
    }

    public void PlayerWin()
    {

    }
}
