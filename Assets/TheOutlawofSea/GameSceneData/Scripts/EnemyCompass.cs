using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCompass : MonoBehaviour
{
    public GameObject compassDir;
    public GameObject[] enemyShips;

    Vector3 compassRotation;

    float shortDistance;

    int shortDistanceIndex;

    // Update is called once per frame
    void Update()
    {
        if (EnemySpawnManager.spawnedCnt > 0)
        {   // 적이 있을 때
            shortDistance = Vector3.Distance(this.transform.position, enemyShips[0].transform.position);

            for(int i = 0; i < EnemySpawnManager.spawnedCnt; i++)
            {
                if (enemyShips[i] != null)
                {
                    if (shortDistance >= Vector3.Distance(this.transform.position, enemyShips[i].transform.position))
                    {
                        shortDistance = Vector3.Distance(this.transform.position, enemyShips[i].transform.position);
                        shortDistanceIndex = i;
                    }
                }
                else
                    continue;
            }

            compassDir.transform.LookAt(enemyShips[shortDistanceIndex].transform);

            // 나침반 회전
            compassRotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y,
                                        Camera.main.transform.eulerAngles.y - compassDir.transform.eulerAngles.y);
            this.transform.rotation = Quaternion.Euler(compassRotation);
        }
        else
        {   // 적이 없을 때
            this.transform.Rotate(new Vector3(0, 0, this.transform.eulerAngles.z), Time.deltaTime * 100.0f);
        }
    }
}
