using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KFGameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject cageLight;
    public GameObject enemySpawnPoint;
    GameObject enemy;

    public Transform enemyPrefabs;

    AudioSource audioPlayer;
    public AudioClip applauseSound;
    public AudioClip bellSound;

    public Image enemyHPGuage;

    public Text enemyRankText;
    public Text playerRankText;

    KFPlayerCtrl KFPlayerCtrlScript;

    float challengerHP;

    int playerRank;
    int enemyRank;
    public int downCnt;

    public bool isGaming;
    public bool isPause;

    // Start is called before the first frame update
    void Start()
    {
        enemy = null;

        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.PlayOneShot(applauseSound);

        KFPlayerCtrlScript = player.GetComponent<KFPlayerCtrl>();

        challengerHP = PlayerPrefs.GetFloat("ChallengerHP");

        playerRank = PlayerPrefs.GetInt("PlayerRank");
        enemyRank = PlayerPrefs.GetInt("EnemyRank");
        downCnt = 3;

        isGaming = false;
        isPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGaming && !isPause)
            cageLight.SetActive(true);
        else
            cageLight.SetActive(false);

        if (enemy == null)
        {
            SpawnEnemy();
        }

        enemyHPGuage.fillAmount = enemy.GetComponent<KFEnemyCtrl>().HP / enemy.GetComponent<KFEnemyCtrl>().maxHP;

        if (enemyRank > 0)
            enemyRankText.text = "랭킹 :  " + enemyRank + "위";
        else if (enemyRank == 0)
            enemyRankText.text = "랭킹 : 챔피언";
        else
            enemyRankText.text = "랭킹 : 도전자";

        if (playerRank > 0)
            playerRankText.text = "랭킹 :  " + playerRank + "위";
        else
            playerRankText.text = "랭킹 : 챔피언";
    }

    // 적 스폰
    void SpawnEnemy()
    {
        enemy = Instantiate(enemyPrefabs.gameObject, enemySpawnPoint.transform.position, enemySpawnPoint.transform.rotation);

        switch (enemyRank)
        {
            case 10:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 100;
                break;
            case 9:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 120.0f;
                break;
            case 8:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 150.0f;
                break;
            case 7:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 175.0f;
                break;
            case 6:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 200.0f;
                break;
            case 5:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 250.0f;
                break;
            case 4:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 300.0f;
                break;
            case 3:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 400.0f;
                break;
            case 2:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 500.0f;
                break;
            case 1:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 850.0f;
                break;
            case 0:
                enemy.GetComponent<KFEnemyCtrl>().maxHP += 1000.0f;
                break;
            default:
                enemy.GetComponent<KFEnemyCtrl>().maxHP = challengerHP + 100.0f;
                break;
        }

        Invoke("Fight", 3.0f);
    }

    // 경기시작
    void Fight()
    {
        audioPlayer.PlayOneShot(bellSound);
        isGaming = true;
    }
    
    // 적 KO
    public void EnemyFinish()
    {
        isGaming = false;
        if(enemyRank > -1)
            enemyRank -= 1;
        if(playerRank > 0)
            playerRank -= 1;
        KFPlayerCtrlScript.HP = 100.0f;
        KFPlayerCtrlScript.energy = 100.0f;
        downCnt = 3;
        audioPlayer.PlayOneShot(applauseSound);
        audioPlayer.PlayOneShot(bellSound);
    }

    public void Playerfinish()
    {
        isGaming = false;
        downCnt -= 1;
        audioPlayer.PlayOneShot(applauseSound);
    }

    public void PlayerKO()
    {
        if (enemyRank >= 0)
            enemyRank += 1;
        else
            enemyRank = 0;
        playerRank += 1;
        audioPlayer.PlayOneShot(applauseSound);
        audioPlayer.PlayOneShot(bellSound);
    }

    // 나가기 버튼
    public void ExitStadium()
    {
        PlayerPrefs.SetFloat("ChallengerHP", challengerHP);
        PlayerPrefs.SetInt("PlayerRank", playerRank);
        PlayerPrefs.SetInt("EnemyRank", enemyRank);

        // 씬 변경
        SceneManager.LoadScene("KingFistMainScene");
    }
}
