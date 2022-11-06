using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject enemyShip;
    public GameObject playerCanvas;
    
    Transform[] spawnPoint;

    EnemyCompass enemyCompassScript;

    float spawnTime;
    float checkTime;

    int randomIndex;
    public static int spawnedCnt;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = 60.0f;
        spawnedCnt = 0;

        spawnPoint = GetComponentsInChildren<Transform>();
        enemyCompassScript = playerCanvas.transform.GetChild(3).transform.GetChild(1).GetComponent<EnemyCompass>();
    }

    // Update is called once per frame
    void Update()
    {
        checkTime += Time.deltaTime;

        if (checkTime >= spawnTime)
        {
            randomIndex = Random.Range(1, 13);
            if (spawnedCnt < enemyCompassScript.enemyShips.Length)
            {
                for(int i=0; i < enemyCompassScript.enemyShips.Length; i++)
                {
                    if (enemyCompassScript.enemyShips[i] == null)
                    {
                        enemyCompassScript.enemyShips[i] = Instantiate(enemyShip, spawnPoint[randomIndex].transform.position, spawnPoint[randomIndex].transform.rotation);
                        enemyCompassScript.enemyShips[i].GetComponent<EnemyCtrl>().shipIndex = i;
                        spawnedCnt += 1;
                        break;
                    }
                }
            }

            checkTime = 0;
        }

        if (GameManager.gameTime >= 180.0f && GameManager.gameTime < 300.0f)
        {
            spawnTime = 50.0f;
        }
        else if (GameManager.gameTime >= 300.0f)
        {
            spawnTime = 45.0f;
        }
    }

    /*
    // 적 이동 동선
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = 1; i <= spawnPoint.Length; i++)
        {
            for (int j = i + 1; j <= spawnPoint.Length; j++)
            {
                if (j + 1 <= spawnPoint.Length)
                    Gizmos.DrawLine(spawnPoint[i].transform.position, spawnPoint[j].transform.position);
            }
        }
    }
    */
}
