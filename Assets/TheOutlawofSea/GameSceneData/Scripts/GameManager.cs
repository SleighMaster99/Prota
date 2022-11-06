using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject playerShip;
    public GameObject playerShipHP;
    public GameObject eastIslandSpawnPoint;
    public GameObject westIslandSpawnPoint;
    public GameObject southIslandSpawnPoint;
    public GameObject northIslandSpawnPoint;
    public GameObject playerSpawnPoint;

    public Text enemyCnt;
    public Text playerHPText;
    public Text cannonBallCntText;
    public Text coinText;

    public Slider playerHPSlider;

    public Image cannonBalCntImage;

    PlayerCtrl playerCtrlScript;
    PlayerShipHP playerShipHPScript;

    public static float gameTime;
    public static float cannonBallDamage;

    int maxCannonBallCnt;

    // Start is called before the first frame update
    void Start()
    {
        // 게임 시작시 항구 위치
        SpawnPosition();

        cannonBallDamage = PlayerPrefs.GetFloat("CannonBallDamage");

        playerShipHPScript = playerShipHP.GetComponent<PlayerShipHP>();
        playerCtrlScript = player.GetComponent<PlayerCtrl>();

        playerHPSlider.maxValue = playerShipHPScript.playerShipMaxHP;

        maxCannonBallCnt = playerCtrlScript.cannonBallCnt;

    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;

        enemyCnt.text = EnemySpawnManager.spawnedCnt.ToString();
        playerHPSlider.value = playerShipHPScript.playerShipHP;
        playerHPText.text = playerShipHPScript.playerShipHP + " / " + playerShipHPScript.playerShipMaxHP;
        coinText.text = playerCtrlScript.coin.ToString();

        cannonBallCntText.text = playerCtrlScript.cannonBallCnt.ToString();
        cannonBalCntImage.fillAmount = (float)playerCtrlScript.cannonBallCnt / (float)maxCannonBallCnt;
    }

    /*
    // 맵 끝라인 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-5000.0f, 1.0f, -5000.0f), new Vector3(5000.0f, 1.0f, -5000.0f));
        Gizmos.DrawLine(new Vector3(5000.0f, 1.0f, -5000.0f), new Vector3(5000.0f, 1.0f, 5000.0f));
        Gizmos.DrawLine(new Vector3(5000.0f, 1.0f, 5000.0f), new Vector3(-5000.0f, 1.0f, 5000.0f));
        Gizmos.DrawLine(new Vector3(-5000.0f, 1.0f, 5000.0f), new Vector3(-5000.0f, 1.0f, -5000.0f));
    }
    */
    

    // 플레이어 스폰
    void SpawnPosition()
    {
        if (PlayerPrefs.GetInt("Island") == 1)
        {   // 동쪽 섬
            playerShip.transform.position = eastIslandSpawnPoint.transform.position;
            playerShip.transform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, -90.0f, this.transform.eulerAngles.z);
        }
        else if (PlayerPrefs.GetInt("Island") == 2)
        {   // 서쪽 섬
            playerShip.transform.position = westIslandSpawnPoint.transform.position;
            playerShip.transform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, 90.0f, this.transform.eulerAngles.z);
        }
        else if (PlayerPrefs.GetInt("Island") == 3)
        {   // 남쪽 섬 - 기본위치
            playerShip.transform.position = southIslandSpawnPoint.transform.position;
        }
        else if (PlayerPrefs.GetInt("Island") == 4)
        {   // 북쪽 섬
            playerShip.transform.position = northIslandSpawnPoint.transform.position;
            playerShip.transform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, 180.0f, this.transform.eulerAngles.z);
        }
        else
        {
            playerShip.transform.position = southIslandSpawnPoint.transform.position;
        }

        player.transform.position = playerSpawnPoint.transform.position;
    }
}
