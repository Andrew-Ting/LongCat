using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
    // Assets/Scenes/Levels/Level0-0.unity
    public int GetSceneBuildIndex()
    {
        int level = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/Levels/Level" + worldNumber + "-" + levelNumber + ".unity");
        return level;
    }
}
