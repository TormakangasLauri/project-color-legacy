using System.Collections;
using System.Collections.Generic;
using AASave;
using UnityEngine;

public static class GameData
{
    public static int levels = 2;

    public class Player
    {
        public float health;
    }

    public class Level
    {
        public int checkpoint;
        public bool completed;
    }

    private static Vector2[] levelSaveData = new Vector2[levels];
    public static Level[] levelData = new Level[levels];

    public static void SaveLevelData(int level, int checkpoint)
    {
        levelSaveData[level].x = checkpoint;
    }
    public static void SaveLevelData(int level, bool completed)
    {
        levelSaveData[level].y = completed ? 1 : 0;
    }

    public static void SaveAllDataToFile(SaveSystem saveSystem)
    {
        
    }
}
