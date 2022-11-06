using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerShipHP : MonoBehaviour
{
    public GameObject[] smokePoints;

    AudioSource audioPlayer;
    public AudioClip fireBurningSound;

    public Text gameOverText;

    public float playerShipHP;
    public float playerShipMaxHP;

    void Start()
    {
        playerShipHP = PlayerPrefs.GetFloat("PlayerShipHP");
        playerShipMaxHP = PlayerPrefs.GetFloat("PlayerShipMaxHP");

        audioPlayer = this.transform.GetComponent<AudioSource>();
    }

    void Update()
    {
        if(playerShipHP <= 0)
        {   // 게임오버
            gameOverText.gameObject.SetActive(true);

            Invoke("GameOver", 2.0f);
        }
        else if (playerShipHP > 200.0f && playerShipHP <= 500.0f)
        {
            smokePoints[0].SetActive(true);
            smokePoints[2].SetActive(true);
            smokePoints[4].SetActive(true);
            smokePoints[5].SetActive(true);
        }
        else if (playerShipHP > 0 && playerShipHP <= 200.0f)
        {
            smokePoints[1].SetActive(true);
            smokePoints[3].SetActive(true);
        }

        if (playerShipHP <= 500.0f)
        {
            if (!audioPlayer.isPlaying)
                audioPlayer.PlayOneShot(fireBurningSound);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("EnemyCannonBallChild"))
        {
            playerShipHP -= 100.0f;
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene("TheOutlawofSeaGameOverScene");
    }
}
