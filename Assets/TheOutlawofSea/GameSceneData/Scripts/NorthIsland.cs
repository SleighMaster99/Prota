using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NorthIsland : MonoBehaviour
{
    public GameObject player;
    public GameObject playerShipCollider;

    PlayerCtrl playerCtrlScript;
    PlayerShipHP playerShipHPScript;

    float playerShipHP;

    int coin;
    int cannonBallCnt;

    bool isArrival;

    // Start is called before the first frame update
    void Start()
    {
        isArrival = false;

        playerCtrlScript = player.transform.GetComponent<PlayerCtrl>();
        playerShipHPScript = playerShipCollider.transform.GetComponent<PlayerShipHP>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PlayerShip") && !isArrival)
        {
            isArrival = true;
            coin = playerCtrlScript.coin;
            cannonBallCnt = playerCtrlScript.cannonBallCnt;
            playerShipHP = playerShipHPScript.playerShipHP;

            PlayerPrefs.SetInt("Island", 4);
            PlayerPrefs.SetInt("Coin", coin);
            PlayerPrefs.SetInt("CannonBallCnt", cannonBallCnt);
            PlayerPrefs.SetFloat("PlayerShipHP", playerShipHP);
            PlayerPrefs.Save();

            // 씬 변경
            Invoke("SceneChange", 1.0f);
        }
    }

    void SceneChange()
    {
        SceneManager.LoadScene("TheOutlawofSeaReadyScene");
    }
}
