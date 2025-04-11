using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum Enemies
{
    basic,
    sniper,
    hulk,
    hanging,
    copter
}

namespace Controllers
{
    public class EnemyList
    {
        public List<GameObject> inactiveList;
        public List<GameObject> activeList;
        
        public EnemyList()
        {
            inactiveList = new List<GameObject>();
            activeList = new List<GameObject>();
        }
    
        public void MoveToActive(GameObject enemy)
        {
            if (inactiveList.Contains(enemy))
            { 
                inactiveList.Remove(enemy);
                activeList.Add(enemy);
            }
        }
    
        public void MoveToInactive(GameObject enemy)
        {
            if (activeList.Contains(enemy))
            { 
                activeList.Remove(enemy);
                inactiveList.Add(enemy);
            }
        }
    }

    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
        public static List<GameObject> _enemyPrefabs = new List<GameObject>();

        public static EnemyList all = new EnemyList();
        public static EnemyList basic = new EnemyList();
        public static EnemyList sniper = new EnemyList();
        public static EnemyList hulk = new EnemyList();
        public static EnemyList hanging = new EnemyList();
        public static EnemyList copter = new EnemyList();

        public static List<EnemyList> typeLists;

        [Header("Spawn enemies on start")]
        [SerializeField] private int spawnBasic = 100;
        [SerializeField] private int spawnSniper = 30;
        [SerializeField] private int spawnHulk = 10;
        [SerializeField] private int spawnHanging = 10;
        [SerializeField] private int spawnCopter = 10;

        private GameObject player;
        private float t;

        public static EnemyController inst;

        private void Awake()
        {
            inst = this;

            typeLists = new List<EnemyList>
            {
                basic,
                sniper,
                hulk,
                hanging,
                copter
            };

            _enemyPrefabs = enemyPrefabs;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");

            // Spawn all enemies
            SpawnNewEnemies(Enemies.basic, spawnBasic);
            SpawnNewEnemies(Enemies.sniper, spawnSniper);
            SpawnNewEnemies(Enemies.hulk, spawnHulk);
            SpawnNewEnemies(Enemies.hanging, spawnHanging);
            SpawnNewEnemies(Enemies.copter, spawnCopter);
        }

        void Update()
        {
            BasicEnemy();
            Hulk();
            Hanging();
        }

        public static void Activate(Enemies enemy, Vector3 position) // Activate an enemy
        {
            if (typeLists[(int)enemy].inactiveList.Count > 0)
                typeLists[(int)enemy].inactiveList[0].GetComponent<EnemyType>().Activate(position);
            else // Spawn more enemies if there are no more inactive enemies to use
            {
                SpawnNewEnemies(enemy, 1);
                inst.StartCoroutine(ActivateLate(enemy, position));
            }
        }
        static IEnumerator ActivateLate(Enemies enemy, Vector3 position) // Wait until a new enemy gets added to lists to activate it
        {
            yield return new WaitUntil(() => typeLists[(int)enemy].inactiveList.Count > 0);
            typeLists[(int)enemy].inactiveList[0].GetComponent<EnemyType>().Activate(position);
        }

        void BasicEnemy()
        {
            t -= Time.deltaTime;
            if (t < 0)
            { t = 0.5f;
                // Sort enemies based on the distance to player
                basic.activeList.Sort((obj1, obj2) =>
                {
                    Vector3 playerPos = player.transform.position;
                    return Vector3.Distance(obj1.transform.position, playerPos).CompareTo(Vector3.Distance(obj2.transform.position, playerPos));
                });

                // Assign stopping distances for enemies making some enemies get close and other stay furter away
                float stopDist = 2.5f;
                float enemiesOnLayer = 5;
                for (int i = 0; i < basic.activeList.Count; i++)
                {
                    if (i % enemiesOnLayer == 0 && i != 0)
                    {
                        stopDist += 1;
                        enemiesOnLayer += 8;
                    }

                    basic.activeList[i].GetComponent<BaseEnemyMovement>().stopDistance = stopDist;
                }
            }
        }
    
        void Hulk()
        {
            foreach (GameObject h in hulk.activeList)
            {
                h.GetComponent<HulkMovement>().stopDistance = 3;
            }
        }

        void Hanging()
        {
            
        }

        static void SpawnNewEnemies(Enemies enemy ,int count)
        {
            for (int i = 0; i < count; i++)
                Instantiate(_enemyPrefabs[(int)enemy], Vector3.down * 500, Quaternion.identity);
        }
    }
}