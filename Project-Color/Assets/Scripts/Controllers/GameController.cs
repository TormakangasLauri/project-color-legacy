using System.Collections;
using System.Collections.Generic;
using AASave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject playerObject;
    public static GameObject playerRoot;
    public static GameObject player;

    public GameObject saveSystemObject;
    public GameObject levelLoaderObject;

    public int levelIndex;
    public int currentCheckpoint;

    public static SaveSystem saveSystem;
    public static LevelLoader levelLoader;
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
        if (!GameObject.Find("Save System(Clone)") && !GameObject.Find("LevelLoader(Clone)"))
        {
            GameObject s = Instantiate(saveSystemObject);
            saveSystem = s.GetComponent<SaveSystem>();
            GameData.saveSystem = saveSystem;

            GameObject l = Instantiate(levelLoaderObject);
            levelLoader = l.GetComponent<LevelLoader>();
        }

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
        {
            Checkpoint c = t.GetComponent<Checkpoint>();
            c.SetSpawnPosition();
            checkpoints.Add(c.index, c);
        }

        int checkpoint = GameData.levelData[GameData.currentLevel].checkpoint;
        if (GameObject.FindWithTag("PlayerRoot")) DestroyImmediate(GameObject.FindWithTag("PlayerRoot")); // Destroy player if it already exists
        GameObject pr = Instantiate(playerObject, checkpoints[checkpoint].spawnPosition, checkpoints[checkpoint].transform.rotation);
        playerRoot = pr;
        player = pr.transform.GetChild(0).GetChild(0).gameObject;
    }

    private void Start()
    {
        currentCheckpoint = GameData.levelData[GameData.currentLevel].checkpoint;
        int checkpoint = GameData.levelData[GameData.currentLevel].checkpoint;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) EndLevel();
    }

    public void SaveLevel()
    {
        GameData.SaveAllData(1);
    }

    public static void LoadLevel(int level) { levelLoader.Load(level); }
    
    public void EndLevel()
    {
        GameData.levelData[levelIndex].completed = true;
        GameData.SaveAllData(1);

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
            LoadLevel(0);
            Time.timeScale = 1;
        }
    }

    private void OnApplicationQuit()
    {
        GameData.SaveAllData(1);
    }
}