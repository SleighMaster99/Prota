using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TOSStartScenePlayer : MonoBehaviour
{
    public GameObject howtoPlay;

    public Image cursorGauge;
    
    public Button loadGameButton;

    Vector3 screenCenter;

    float gaugeTime;

    bool isHowtoPlay;

    // Start is called before the first frame update
    void Start()
    {
        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        gaugeTime = 0;

        isHowtoPlay = false;

        if (PlayerPrefs.HasKey("haveTOSData"))
            loadGameButton.gameObject.SetActive(true);
        else
            loadGameButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHowtoPlay)
            StartMain();
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isHowtoPlay = false;
                howtoPlay.SetActive(false);
            }
        }
    }

    void StartMain()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        if (Physics.Raycast(ray, out hit, 100.0f))
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
    }

    public void TOSNewGameButton()
    {
        int haveTOSData = 0;
        int island = 3;
        int coin = 0;
        int shipCannonIndex = 0;
        int cannonBallCnt = 30;
        int upgradeShipCoin = 5000;
        float ballPower = 2500.0f;
        float playerShipHP = 1000.0f;
        float playerShipMaxHP = 1000.0f;
        float cannonBallDamage = 100.0f;
        float reloadTime = 3.0f;

        PlayerPrefs.SetInt("haveTOSData", haveTOSData);
        PlayerPrefs.SetInt("Island", island);
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("ShipCannonIndex", shipCannonIndex);
        PlayerPrefs.SetInt("CannonBallCnt", cannonBallCnt);
        PlayerPrefs.SetInt("UpgradeShipCoin", upgradeShipCoin);
        PlayerPrefs.SetFloat("ballPower", ballPower);
        PlayerPrefs.SetFloat("PlayerShipHP", playerShipHP);
        PlayerPrefs.SetFloat("PlayerShipMaxHP", playerShipMaxHP);
        PlayerPrefs.SetFloat("CannonBallDamage", cannonBallDamage);
        PlayerPrefs.SetFloat("ReloadTime", reloadTime);

        PlayerPrefs.Save();

        // TOSReadyScene으로 변경
        SceneManager.LoadScene("TheOutlawofSeaReadyScene");
    }

    public void TOSLoadGame()
    {
        // TOSReadyScene으로 변경
        SceneManager.LoadScene("TheOutlawofSeaReadyScene");
    }

    public void TOSHowToPlay()
    {
        isHowtoPlay = true;
        howtoPlay.SetActive(true);
    }

    public void TOSExitGame()
    {
        // AppMainScene으로 변경
        SceneManager.LoadScene("ProtaMainScene");
    }
}
