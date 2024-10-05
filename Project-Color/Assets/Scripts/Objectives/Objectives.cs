using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controllers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    enum objectives
    {
        kill,
        platform,
        paint
    };

    public List<GameObject> enemySpawnPointGroups = new List<GameObject>();

    private List<objectives> activeObjectives = new List<objectives>();

    EnemyController enemyController;

    public TextMeshProUGUI killObjectiveText;

    public bool platformActive = false;
    bool spawning = false;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        UpdateKillObj();
        UpdatePaintObj();
    }

    void UpdateKillObj()
    {
        if (activeObjectives.Contains(objectives.kill))
        {
            killObjectiveText.text = enemyController.all.activeList.Count.ToString();

            if (enemyController.all.activeList.Count == 0 && !spawning)
            {
                activeObjectives.Remove(objectives.kill);
                killObjectiveText.text = "OBJECTIVE COMPLETE";
                StartCoroutine(ClearKillText());

                IEnumerator ClearKillText()
                {
                    yield return new WaitForSeconds(3);
                    killObjectiveText.text = "";
                }
            }
            killObjectiveText.text = enemyController.all.activeList.Count.ToString();

            if (enemyController.all.activeList.Count == 0 && !spawning) activeObjectives.Remove(objectives.kill);
        }
    }

    void UpdatePaintObj()
    {
        if (activeObjectives.Contains(objectives.paint))
        {
            // maalialueen tekstuuri pisteitten ja kaikkien pisteiden suhde, niiden avulla lasketaan kokonaispisteistä haluttu pistemäärä
            PaintTarget.TallyScore();
            float score = PaintTarget.scores.x + PaintTarget.scores.y + PaintTarget.scores.z + PaintTarget.scores.w;
            
            Debug.Log(score);
            if (score > PaintTarget.textureCoordinatesInPaintArea/384 * 8)
            {
                Debug.Log("Vilho tee jotain joskus"); // Huono debug log ei vilho tänne kuitenkaan kato
                activeObjectives.Remove(objectives.paint);
            }
        }
    }
    
    public void KillObj(int killGroup)
    {
        activeObjectives.Add(objectives.kill);
        StartCoroutine(StartWaves(killGroup));
    }
    
    public IEnumerator Platform()
    {
        activeObjectives.Add(objectives.platform);

        platformActive = true;

        yield return new WaitWhile(delegate { return platformActive; });

        activeObjectives.Remove(objectives.platform);
        Debug.Log("Platform objective completed");
    }

	public void PaintObj(Collider paintArea)
    {
        activeObjectives.Add(objectives.paint);
        PaintTarget.paintArea = paintArea;
    }

    IEnumerator StartWaves(int killGroup)
    {
        spawning = true;

        int wave = 0;
        while (true)
        {
            bool spawned = false;
            for (int i = 0; i < enemySpawnPointGroups[killGroup].transform.childCount; i++)
            {
                Transform spawnPoint = enemySpawnPointGroups[killGroup].transform.GetChild(i);
                if (spawnPoint.gameObject.GetComponent<EnemySpawnPoint>().wave == wave)
                {
                    StartCoroutine(Spawn(spawnPoint.gameObject, killGroup));
                    spawned = true;
                }
            }

            wave++;
            if (!spawned) break;
            yield return new WaitForSeconds(5);
        }

        spawning = false;
    }

    private IEnumerator Spawn(GameObject spawnPoint, int killGroup)
    {
        EnemySpawnPoint esp = spawnPoint.GetComponent<EnemySpawnPoint>();

        float spread = 2;
        float wait = 0.05f;
        
        for (int type = 0; type < enemyController.typeLists.Count; type++)
            for (int j = 0; j < esp.enemiesToSpawn[type]; j++)
            {
                Debug.Log("Spawn");
                Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
                GameObject enemy = enemyController.typeLists[type].inactiveList[0];
                
                // Spawn
                enemy.transform.position = spawnPoint.transform.position + random;
                enemy.GetComponent<EnemyType>().Activate(killGroup);
                enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;

                yield return new WaitForSeconds(wait);
            }
    }
}
