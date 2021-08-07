using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    private GoalBlockController goalBlockController;
    void Awake()
    {
        goalBlockController = GetComponent<GoalBlockController>();
        goalBlockController.EndGame += PauseTime;
        goalBlockController.StartGame += StartTime;
    }

    void StartTime(){
        Time.timeScale = 1;   
    }

    void PauseTime() {
        Time.timeScale = 0; 
    }
}
