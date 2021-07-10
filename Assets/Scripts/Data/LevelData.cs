using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "Level Template")]
[Serializable]
public class LevelData : ScriptableObject
{
    [Header("Level Detail")]
    [HideInInspector]
    public int id;
    [Tooltip("starts at 1")]
    public int worldNumber;
    [Tooltip("starts at 1")]
    public int level;
    [Header("Level inside")]
    public Vector3 catStartingPos;
    public int catStartingHeight;

    public string GetWorldLevelString()
    {
        return worldNumber + " - " + level;
    }
}
