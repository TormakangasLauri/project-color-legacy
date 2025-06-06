using System.Collections;
using System.Collections.Generic;
using AASave;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject playerObject;

    public int levelIndex;
    public int currentCheckpoint;

    public static bool paused;

    public static GameController inst;

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
    public static List<Level> levels = new List<Level>();

    private Dictionary<int, Checkpoint> checkpoints = new Dictionary<int, Checkpoint>();

    private void OnValidate()
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
    }

    private void Awake()
    {
        inst = this;
        levelIndex = SceneManager.GetActiveScene().buildIndex - 1; // First level index = 0

        if (GameData.saveSystem == null)
        {
            SaveSystem saveSystem = gameObject.AddComponent<SaveSystem>();
            saveSystem.SetSubFolderOption(true);
            GameData.saveSystem = saveSystem;
        }
        GameData.LoadData();

        if (GameData.levelData[levelIndex] == null) GameData.levelData[levelIndex] = new (levelIndex, 0);
        else GameData.levelData[levelIndex].level = levelIndex;
        GameData.currentLevel = levelIndex;

        foreach (Transform t in transform)
            checkpoints.Add(t.GetComponent<Checkpoint>().index, t.GetComponent<Checkpoint>());

        int checkpoint = GameData.levelData[GameData.currentLevel].checkpoint;
        if (GameObject.FindWithTag("PlayerRoot")) Destroy(GameObject.FindWithTag("PlayerRoot")); // Destroy player if it already exists
        Instantiate(playerObject, checkpoints[checkpoint].spawnPosition, checkpoints[checkpoint].transform.rotation);
    }

    private void Start()
    {
        currentCheckpoint = GameData.levelData[GameData.currentLevel].checkpoint;
        int checkpoint = GameData.levelData[GameData.currentLevel].checkpoint;
        //if (!GameObject.FindWithTag("PlayerRoot"))
        //{
        //    Instantiate(playerObject, checkpoints[checkpoint].spawnPosition, checkpoints[checkpoint].transform.rotation);
        //}
        //else if (currentCheckpoint != 0)
        //{
        //    GameObject player = GameObject.FindWithTag("PlayerRoot");
        //    player.transform.position = checkpoints[checkpoint].spawnPosition;
        //    player.transform.rotation = checkpoints[checkpoint].transform.rotation;
        //}

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) EndLevel();
    }

    public void SaveLevel()
    {
        GameData.SaveAllData(1);
    }

    public static void StartLevel(int level)
    {
        HUDText.SaveAllText();
        SceneManager.LoadScene(level);
    }
    
    public void EndLevel()
    {
        GameData.levelData[levelIndex - 1].completed = true;
        GameData.SaveAllData(1);
        //GameData.SaveAllDataToFile(saveSystem, levelIndex);

        HUDText.SetText(new[]{0}, new[]{"Level complete !!!"}, HUDTextFill.Fill);
        HUDText.stopUpdates = true;
        StartCoroutine(End());
        IEnumerator End()
        {
            float timer = 2;
            yield return new WaitUntil(() =>
            {
                timer -= Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Max(0, timer/2);
                return timer <= 0;
            });
            HUDText.stopUpdates = false;
            StartLevel(0);
            Time.timeScale = 1;
        }
    }

    private void OnApplicationQuit()
    {
        GameData.SaveAllData(1);
    }
}