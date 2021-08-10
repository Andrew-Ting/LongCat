using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBlockController : MonoBehaviour
{
    public event Action StartGame;
    public event Action EndGame;

    private int goalCubeQuantity = 0;
    private int goalCubeSatisfied = 0;
    private GameObject endGamePanel;
    private BlockManager blockManager;
    private GameManager gameManager;
    void Start()
    {
        StartGame?.Invoke();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        //endGamePanel.SetActive(false);
        //if (!endGamePanel)
        //    Debug.LogError("The end game panel does not exist, is not active, or is not named `WinPanel`. Please either change the end game panel name, or this script");
        goalCubeQuantity = transform.childCount;
        foreach (Transform children in transform) {
            children.gameObject.AddComponent<GoalCubeController>();
        }
        Debug.Log(goalCubeQuantity);
        blockManager = FindObjectOfType<BlockManager>();
        blockManager.moveCompleted += CheckEndGame;
    }
    public void AddSatisfiedCount() {
        goalCubeSatisfied++;
        if (goalCubeSatisfied > goalCubeQuantity)
            Debug.LogError("There cannot be more goal blocks satisfied than the number of goal blocks in the level");
    }
    public void DeductSatisfiedCount() {
        goalCubeSatisfied--;
        if (goalCubeSatisfied < 0)
            Debug.LogError("There cannot be a negative number of goal blocks satisfied in the level");
    }
    void CheckEndGame() {
        if (goalCubeQuantity == goalCubeSatisfied && goalCubeQuantity != 0) { // if the value was 0, it means the cat was moved initially to set the level; this is not an end game condition
            //endGamePanel.SetActive(true);
            gameManager.PlayerWin();
            EndGame?.Invoke();
        }
    }
}
