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
    [Min(0), Tooltip("starts at 0")]
    public int worldNumber;
    [Min(0), Tooltip("starts at 0")]
    public int levelNumber;

    [Header("Level inside")]
    public Vector3 catStartingPos;
    [Range(1,5)]
    public int catStartingHeight;

#if UNITY_EDITOR
    [Multiline(5), Header("For developer part only")]
    public string notes;
#endif
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

    public int GetSceneBuildIndex()
    {
        int level = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/Levels/Level" + worldNumber + "-" + levelNumber + ".unity");
        return level;
    }
}
