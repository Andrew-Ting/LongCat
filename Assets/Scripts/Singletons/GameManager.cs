using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject WinUI;
    [HideInInspector] public LevelData levelData;
    [SerializeField] CatMovement cat;
    private bool fished;
    private bool fishCollected;

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
            levelData = levelCollection.levelDataCollection[gameData.puzzleID];
            SceneManager.LoadScene(levelData.GetSceneBuildIndex(), LoadSceneMode.Additive);
            cat.transform.position = levelData.catStartingPos;
            cat.SetCatHeight(levelData.catStartingHeight);
            cat.gameObject.SetActive(true);
            cat.StartSetObject();

            FindObjectOfType<AudioManager>().TransitionBGM("World" + levelData.worldNumber + "Theme", 1f);
            if (FindObjectOfType<PlayerManager>().IsFished(levelData.id))
            {
                fished = true;
                //remove the fish;
            }
            else
            {
                fished = false;
            }
            fishCollected = false;
        }
    }

    public void PlayerWin()
    {
        WinUI.SetActive(true);
        LevelLoader.SaveNextGame(levelData.id);
        FindObjectOfType<PlayerManager>().LevelFinished(levelData.id);
        if (fished) FindObjectOfType<PlayerManager>().LevelFished(levelData.id);
    }

    public void CollectFish()
    {
        fished = true;
        fishCollected = true;
    }
    public bool IsFished()
    {
        return fished;
    }

    public bool FishCollectedThisRound()
    {
        return fishCollected;
    }
}
