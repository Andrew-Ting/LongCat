using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "Level Template")]
[Serializable]
public class LevelData : ScriptableObject
{
    [Header("Level Detail")]
    public int id;
    public int worldNumber;
    public int level;
    [Header("Level inside")]
    public Vector3 catStartingPos;
    public int catStartingHeight;
}
