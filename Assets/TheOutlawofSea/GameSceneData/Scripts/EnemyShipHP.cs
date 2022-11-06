using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipHP : MonoBehaviour
{
    public GameObject[] smokePoints;

    float enemyShipHP;

    public bool isEnemyDie;

    void Start()
    {
        enemyShipHP = 700.0f;
        isEnemyDie = false;
    }

    void Update()
    {
        if (enemyShipHP <= 0)
        {
            // 배 침몰
            isEnemyDie = true;
        }
        else if (enemyShipHP > 200.0f && enemyShipHP <= 500.0f)
        {
            smokePoints[0].SetActive(true);
            smokePoints[2].SetActive(true);
            smokePoints[4].SetActive(true);
            smokePoints[5].SetActive(true);
        }
        else if (enemyShipHP > 0 && enemyShipHP <= 200.0f)
        {
            smokePoints[1].SetActive(true);
            smokePoints[3].SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("CannonBallChild"))
        {
            enemyShipHP -= GameManager.cannonBallDamage;
        }
    }
}
