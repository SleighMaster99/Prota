using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KFMainPlayerCtrl : MonoBehaviour
{
    public Image cursorGauge;

    public Text playerRankText;
    public Text nextEnemyRankText;

    Vector3 screenCenter;

    float gaugeTime;

    int playerRank;
    int enemyRank;

    // Start is called before the first frame update
    void Start()
    {
        screenCenter = screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        if (PlayerPrefs.HasKey("PlayerRank"))
            playerRank = PlayerPrefs.GetInt("PlayerRank");
        else
            playerRank = 11;

        if (PlayerPrefs.HasKey("EnemyRank"))
            enemyRank = PlayerPrefs.GetInt("EnemyRank");
        else
            enemyRank = 10;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            // Ray에 부딫힌 물체의 태그가 Button이면 게이지 증가
            if (hit.transform.CompareTag("Button"))
            {
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            }
            else
                gaugeTime = 0;

            // 게이지가 다 찼을 때
            if (gaugeTime >= 1.0f)
            {
                hit.transform.GetComponent<Button>().onClick.Invoke();
                gaugeTime = 0;
            }
        }
        else
            gaugeTime = 0;

        // 랭킹별 텍스트 변경
        if (enemyRank > 0)
            nextEnemyRankText.text = "다음 상대 랭킹\n" + enemyRank + "위";
        else if (enemyRank == 0)
            nextEnemyRankText.text = "다음 상대 랭킹\n챔피언";
        else
            nextEnemyRankText.text = "다음 상대 랭킹\n도전자";

        if (playerRank > 0)
            playerRankText.text = "랭킹 :  " + playerRank + "위";
        else
            playerRankText.text = "랭킹 : 챔피언";
    }

    public void TrainingButton()
    {
        SceneManager.LoadScene("KingFistTrainingScene");
    }

    public void NewGame()
    {
        PlayerPrefs.SetFloat("ChallengerHP", 1000.0f);
        PlayerPrefs.SetInt("PlayerRank", 11);
        PlayerPrefs.SetInt("EnemyRank", 10);

        SceneManager.LoadScene("KingFistGameScene");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("KingFistGameScene");
    }

    public void ExitKingFist()
    {
        SceneManager.LoadScene("ProtaMainScene");
    }
}
