using System.Collections;
using System.Collections.Generic;
using AASave;
using UnityEngine;

public static class GameData
{
    public static int levels = 3;
    public static int saveSlot = 1;

    public class Player
    {
        public static int health;
        public static Vector3 position;
        public static Quaternion rotation;
    }

    public class Enemy
    {
        public static int health;
        public static Vector3 position;
        public static Quaternion rotation;
    }

    public static List<Enemy> enemies = new List<Enemy>();

    public class Level
    {
        public int level;
        public int checkpoint;
        public bool completed;

        public Level(int level, int checkpoint, bool completed = false)
        {
            this.level = level;
            this.checkpoint = checkpoint;
            this.completed = completed;
        }
    }

    private static Vector3[] levelSaveData = new Vector3[levels];
    public static Level[] levelData = new Level[levels];

    public static void SaveLevelData(int level, int checkpoint, bool completed = false)
    {
        levelData[level - 1].level = level;
        levelData[level - 1].checkpoint = checkpoint;
        levelData[level - 1].completed = completed;

        levelSaveData[level - 1].Set(level, checkpoint, completed ? 1 : 0);
    }

    public static void SaveAllDataToFile(SaveSystem saveSystem)
    {
        saveSystem.SetSubFolderName($"Save Slot {saveSlot}");
        saveSystem.Save("Playerhealth", Player.health);
        saveSystem.Save("Levelsavedata", levelSaveData);
    }
}
