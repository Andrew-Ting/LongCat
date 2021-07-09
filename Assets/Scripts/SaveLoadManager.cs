using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class SaveLoadManager
{ 
    //Player save load functions
    public static void SavePlayer(PlayerManager player)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/player.maw", FileMode.Create);

        PlayerData data = new PlayerData(player);
        bf.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer()
    {
        if (File.Exists(Application.persistentDataPath + "/player.maw"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/player.maw", FileMode.Open);

            PlayerData data = bf.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            //no file exists
            return null;
        }
    }
    public static void DeletePlayer()//testing purposes only
    {
        if (File.Exists(Application.persistentDataPath + "/player.maw"))
        {
            File.Delete(Application.persistentDataPath + "/player.maw");
        }
    }

    //GameInOut functions
    public static void SaveGameData(int id)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/game.hueh", FileMode.Create);

        GameData data = new GameData(id);
        bf.Serialize(stream, data);
        stream.Close();
    }
    public static GameData LoadGameData()
    {
        if (File.Exists(Application.persistentDataPath + "/game.hueh"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/game.hueh", FileMode.Open);

            GameData data = bf.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            //no file exists
            return null;
        }
    }
    public static void DeleteGameData()//testing purposes only
    {
        if (File.Exists(Application.persistentDataPath + "/game.hueh"))
        {
            File.Delete(Application.persistentDataPath + "/game.hueh");
        }
    }
}

[Serializable]
public class PlayerData
{
    public bool[] worlds;
    public bool[] id;

    public PlayerData(PlayerManager player)
    {
        worlds = player.GetWorlds();
        id = player.GetIDs();
    }
}

[Serializable]
public class GameData
{
    public int puzzleID;

    public GameData(int id)
    {
        puzzleID = id;
    }
}