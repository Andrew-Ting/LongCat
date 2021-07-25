using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "Level Template")]
[Serializable]
public class LevelData : ScriptableObject
{
    [Header("Level Detail")]
    [HideInInspector]
    public int id;
    [Tooltip("starts at 0")]
    public int worldNumber;
    [Tooltip("starts at 0")]
    public int levelNumber;
    [Header("Level inside")]
    public Vector3 catStartingPos;
    public int catStartingHeight;

    public string GetWorldLevelString(bool world, bool level)
    {
        if (world && level)
            return (worldNumber + 1) + " - " + (levelNumber + 1);
        else if (world)
            return (worldNumber + 1).ToString();
        else if (level)
            return (levelNumber + 1).ToString();
        else
            return "-1";
    }
}
