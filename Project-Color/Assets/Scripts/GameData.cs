using System.Collections;
using System.Collections.Generic;
using AASave;
using Unity.VisualScripting;
using UnityEngine;

public class GameData
{
    public static SaveSystem saveSystem;

    public static int levels = 3;
    public static int saveSlot = 1;
    public static int currentLevel = 0;

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

        public class Player
        {
            public int health;
            public Vector3 position;
            public Quaternion rotation;
        }
        public Player player = new Player();

        public class Enemy
        {
            public int health;
            public Vector3 position;
            public Quaternion rotation;
        }
        public List<Enemy> enemies = new List<Enemy>();
    }

    public static Level[] levelData = new Level[3] {new(0, 0), new(0,0), new(0,0)};
    private static Vector3[] levelSaveData = new Vector3[levels];

    public static void SaveAllData(int saveSlot = 1)
    {
        saveSystem.subFolderName = $"SaveSlot{saveSlot}";
        for (int i = 0; i < levels; i++)
        {
            Level level = levelData[i];
            saveSystem.Save($"{i}level", level.level);
            saveSystem.Save($"{i}checkpoint", level.checkpoint);
            saveSystem.Save($"{i}completed", level.completed);

            saveSystem.Save($"{i}playerhealth", level.player.health);
            saveSystem.Save($"{i}playerposition", level.player.position);
            saveSystem.Save($"{i}playerrotation", level.player.rotation);

            for (int j = 0; j < level.enemies.Count; j++)
            {
                Level.Enemy enemy = level.enemies[j];
                saveSystem.Save($"{i}enemy{j}health", enemy.health);
                saveSystem.Save($"{i}enemy{j}position", enemy.position);
                saveSystem.Save($"{i}enemy{j}rotation", enemy.rotation);
            }
        }
    }

    public static void LoadData(int saveSlot = 1)
    {
        saveSystem.subFolderName = $"SaveSlot{saveSlot}";
        for (int i = 0; i < levels; i++)
        {
            levelData[i] = new Level(i, 0);

            levelData[i].level = saveSystem.Load($"{i}level").AsInt();
            levelData[i].checkpoint = saveSystem.Load($"{i}checkpoint").AsInt();
            levelData[i].completed = saveSystem.Load($"{i}completed").AsBool();
            
            levelData[i].player.health = saveSystem.Load($"{i}playerhealth").AsInt();
            levelData[i].player.position = saveSystem.Load($"{i}playerposition").AsVector3();
            levelData[i].player.rotation = saveSystem.Load($"{i}playerrotation").AsQuaternion();

            for (int j = 0; j < levelData[i].enemies.Count; j++)
            {
                levelData[i].enemies[j].health = saveSystem.Load($"{i}enemy{j}health").AsInt();
                levelData[i].enemies[j].position = saveSystem.Load($"{i}enemy{j}position").AsVector3();
                levelData[i].enemies[j].rotation = saveSystem.Load($"{i}enemy{j}rotation").AsQuaternion();
            }
        }
    }

    public static void DeleteSavedData(int saveSlot = 1)
    {
        saveSystem.subFolderName = $"SaveSlot{saveSlot}";
        for (int i = 0; i < levels; i++)
        {
            Level level = levelData[i];
            saveSystem.Delete($"{i}level");
            saveSystem.Delete($"{i}checkpoint");
            saveSystem.Delete($"{i}completed");

            saveSystem.Delete($"{i}playerhealth");
            saveSystem.Delete($"{i}playerposition");
            saveSystem.Delete($"{i}playerrotation");

            for (int j = 0; j < level.enemies.Count; j++)
            {
                Level.Enemy enemy = level.enemies[j];
                saveSystem.Delete($"{i}enemy{j}health");
                saveSystem.Delete($"{i}enemy{j}position");
                saveSystem.Delete($"{i}enemy{j}rotation");
            }
        }
    }
}
