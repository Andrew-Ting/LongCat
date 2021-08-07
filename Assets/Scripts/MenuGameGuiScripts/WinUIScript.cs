using UnityEngine;

public class WinUIScript : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void ReturnHome()
    {
        LevelLoader.ReturnHome();
    }

    public void NextLevel()
    {
        LevelLoader.LoadNextGame(gameManager.levelData.id);
    }
}