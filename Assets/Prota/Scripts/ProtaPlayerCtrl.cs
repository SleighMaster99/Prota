using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProtaPlayerCtrl : MonoBehaviour
{
    public GameObject phone;
    public GameObject cardBoard;
    public GameObject[] selectGameMenu;
    public GameObject[] lamp;

    AudioSource audioPlayer;
    public AudioClip welcomeSound;

    public Image cursorGauge;

    Vector3 screenCenter;

    float gaugeTime;

    bool isCardBoardMvoe;
    bool isFog;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        screenCenter = screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        isCardBoardMvoe = false;
        isFog = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        if (Physics.Raycast(ray, out hit, 25.0f))
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

        // 카드보드 움직이기
        if (isCardBoardMvoe)
        {
            if (cardBoard.transform.position.z > 0.65)
            {
                cardBoard.transform.Translate(Vector3.down * Time.deltaTime * 0.5f);
                isFog = true;
                RenderSettings.fog = true;
                RenderSettings.fogDensity += 1.0f * Time.deltaTime;
            }
            else
            {
                isCardBoardMvoe = false;
                cardBoard.SetActive(false);
                selectGameMenu[0].SetActive(true);
                selectGameMenu[1].SetActive(true);
                lamp[0].SetActive(true);
                lamp[1].SetActive(true);
            }
        }

        if(isFog && !isCardBoardMvoe)
        {
            RenderSettings.fogDensity -= 1.0f * Time.deltaTime;

            if (RenderSettings.fogDensity <= 0)
            {
                isFog = false;
                RenderSettings.fog = false;

                audioPlayer.PlayOneShot(welcomeSound);
            }
        }
    }

    // 스마트폰의 시작 버튼을 눌렀을 때
    public void StartButton()
    {
        phone.SetActive(false);
        cardBoard.SetActive(true);

        isCardBoardMvoe = true;
    }

    // 앱 종료 버튼
    public void QuitAppButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // SelectGameCanvas Game0 Play
    public void Game0StartButton()
    {
        SceneManager.LoadScene("TheOutlawofSeaStartScene");
    }

    // SelectGameCanvas Game1 Play
    public void Game1StartButton()
    {
        SceneManager.LoadScene("KingFistMainScene");
    }
}
