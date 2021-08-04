using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUIScript : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameObject.SetActive(false);
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
