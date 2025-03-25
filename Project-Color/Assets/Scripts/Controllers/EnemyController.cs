using System.Collections.Generic;
using UnityEngine;

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
        public List<GameObject> enemyPrefabs = new List<GameObject>();

        public static EnemyList all = new EnemyList();
        public static EnemyList basic = new EnemyList();
        public static EnemyList sniper = new EnemyList();
        public static EnemyList hulk = new EnemyList();
        public static EnemyList hanging = new EnemyList();
        public static EnemyList copter = new EnemyList();

        public static List<EnemyList> typeLists;

        public static Dictionary<string, int> enemies = new Dictionary<string, int>
        {
            { "basic", 0 },
            { "sniper", 1 },
            { "hulk", 2 },
            { "hanging", 3 },
            { "copter", 4 }
        };

        private GameObject player;
        private float t;

        private void Awake()
        {
            typeLists = new List<EnemyList>
            {
                basic,
                sniper,
                hulk,
                hanging,
                copter
            };
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");

            // Spawn all enemies
            for (int i = 0; i < 100; i++) // Basic
                Instantiate(enemyPrefabs[0], Vector3.down * 500, Quaternion.identity);
            for (int i = 0; i < 30; i++) // Sniper
                Instantiate(enemyPrefabs[1], Vector3.down * 500, Quaternion.identity);
            for (int i = 0; i < 10; i++) // Hulk
                Instantiate(enemyPrefabs[2], Vector3.down * 500, Quaternion.identity);
            for (int i = 0; i < 10; i++) // Hanging
                Instantiate(enemyPrefabs[3], Vector3.down * 500, Quaternion.identity);
            for (int i = 0; i < 10; i++) // Copter
                Instantiate(enemyPrefabs[3], Vector3.down * 500, Quaternion.identity);
        }

        void Update()
        {
            BasicEnemy();
            Hulk();
            Hanging();
        }

        public static void Activate(int enemy, Vector3 position)
        {
            typeLists[enemy].inactiveList[0].GetComponent<EnemyType>().Activate(position);
        }
        public static void Activate(string enemy, Vector3 position)
        {
            typeLists[enemies[enemy]].inactiveList[0].GetComponent<EnemyType>().Activate(position);
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
                float stopDist = 0.5f;
                float enemiesOnLayer = 5;
                for (int i = 0; i < basic.activeList.Count; i++)
                {
                    if (i % enemiesOnLayer == 0)
                    {
                        stopDist += 1;
                        enemiesOnLayer += stopDist;
                    }

                    basic.activeList[i].GetComponent<BaseEnemyMovement>().stopDistance = stopDist;
                }
            }
        }
    
        void Hulk()
        {
            foreach (GameObject h in hulk.activeList)
            {
                h.GetComponent<EnemyMovement>().stopDistance = 3;
            }
        }

        void Hanging()
        {
            
        }
    }
}