using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KFPlayerCtrl : MonoBehaviour
{
    public GameObject kfGameManager;
    public GameObject boxingGloveL;
    public GameObject boxingGloveR;
    public GameObject glp;
    public GameObject grp;
    public GameObject punchPoint;
    public GameObject headAndbody;
    public GameObject head;

    public Image cursorGaugeImage;
    public Image HPGaugeImage;
    public Image energyGaugeImage;

    public Text countDownText;
    public Text KOText;
    public Text clickToMenuText;

    KFGameManager KFGameManagerScript;

    Vector3 ScreenCenter;
    Vector3 gloveLOriginPosition;
    Vector3 gloveROriginPosition;

    float cameraRZ;
    float gaugeTime;
    float countdown;
    public float energy;
    public float HP;

    bool isTrigger;
    bool isPunch;
    public bool isHit;
    bool isDown;
    bool isKO;

    // Start is called before the first frame update
    void Start()
    {
        KFGameManagerScript = kfGameManager.GetComponent<KFGameManager>();

        ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        energy = 100.0f;
        HP = 100.0f;
        countdown = 3.0f;

        isTrigger = false;
        isHit = false;
        isDown = false;
        isKO = false;

        gloveLOriginPosition = boxingGloveL.transform.position;
        gloveROriginPosition = boxingGloveR.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        isTrigger = Input.GetMouseButtonDown(0);

        HPGaugeImage.fillAmount = HP / 100.0f;
        energyGaugeImage.fillAmount = energy / 100.0f;

        if (!KFGameManagerScript.isPause && KFGameManagerScript.isGaming)
            Gaming();

        if(HP <= 0 && !isDown && !isKO)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                       Quaternion.Euler(-90.0f, this.transform.eulerAngles.y, this.transform.eulerAngles.z),
                                                       5.0f * Time.deltaTime);

            if (this.transform.eulerAngles.x <= 270.0f)
            {
                KFGameManagerScript.Playerfinish();
                countDownText.gameObject.SetActive(true);
                isDown = true;
            }
                
        }

        if (isDown && !isKO)
        {
            if (KFGameManagerScript.downCnt > 0)
                PlayerDown();
            else
                PlayerKO();
        }

        if (isKO)
        {
            clickToMenuText.gameObject.SetActive(true);

            if (isTrigger)
                KFGameManagerScript.ExitStadium();
        }

        // 뒤돌아보면 게이즈 생성
        if(Camera.main.transform.eulerAngles.y >= 180.0f && Camera.main.transform.eulerAngles.y <= 270.0f)
        {
            KFGameManagerScript.isPause = true;
            cursorGaugeImage.gameObject.SetActive(true);
            IDLE();
        }
        else
        {
            KFGameManagerScript.isPause = false;
            cursorGaugeImage.gameObject.SetActive(false);
        }
    }

    // 화면 회전시 글러브 위치 초기화
    private void LateUpdate()
    {
        if (!isPunch)
        {
            boxingGloveL.transform.position = glp.transform.position;
            boxingGloveR.transform.position = grp.transform.position;
            boxingGloveL.transform.rotation = glp.transform.rotation;
            boxingGloveR.transform.rotation = grp.transform.rotation;
        }

        if (HP < 0)
            HP = 0;

        headAndbody.transform.rotation = Camera.main.transform.rotation;
        this.transform.position = head.transform.position;
    }

    // 기본상태
    void IDLE()
    {
        Ray ray = Camera.main.ScreenPointToRay(ScreenCenter);
        RaycastHit hit;

        cursorGaugeImage.fillAmount = gaugeTime;

        if (Physics.Raycast(ray, out hit, 20.0f))
        {
            if (hit.transform.CompareTag("Button"))
            {
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            }
            else
                gaugeTime = 0;

            if (gaugeTime >= 1.0f)
            {
                hit.transform.GetComponent<Button>().onClick.Invoke();
                gaugeTime = 0;
            }
        }
        else
            gaugeTime = 0;
    }

    // 트리거 클릭시 펀치 기능
    void Punch()
    {
        //isTrigger = Input.GetMouseButtonDown(0);
        cameraRZ = Camera.main.transform.eulerAngles.z;

        if (isTrigger && KFGameManagerScript.isGaming)
        {
            if (energy > 0)
            {
                isPunch = true;
                energy -= 5.0f;
            }
            else
                return;
        }

        if (isPunch && !isHit)
        {
            if (cameraRZ >= 275.0f && cameraRZ <= 360.0f)
            {
                boxingGloveL.transform.position = Vector3.MoveTowards(boxingGloveL.transform.position, punchPoint.transform.position, 100.0f * Time.deltaTime);
            }
            else
            {
                boxingGloveR.transform.position = Vector3.MoveTowards(boxingGloveR.transform.position, punchPoint.transform.position, 100.0f * Time.deltaTime);
            }
        }
        else
        {
            if (boxingGloveL.transform.position != gloveLOriginPosition)
                boxingGloveL.transform.position = Vector3.MoveTowards(boxingGloveL.transform.position, gloveLOriginPosition, 100.0f * Time.deltaTime);

            if (boxingGloveR.transform.position != gloveROriginPosition)
                boxingGloveR.transform.position = Vector3.MoveTowards(boxingGloveR.transform.position, punchPoint.transform.position, 100.0f * Time.deltaTime);
        }

        if ((boxingGloveL.transform.position == punchPoint.transform.position || boxingGloveR.transform.position == punchPoint.transform.position) || isHit)
        {
            isPunch = false;
            isHit = false;
        }
    }

    void Gaming()
    {
        if (energy < 100.0f)
            energy += Time.deltaTime * 5.0f;

        Punch();
    }

    void PlayerDown()
    {
        countdown -= Time.deltaTime;
        countDownText.text = countdown.ToString("F1");

        if (countdown <= 0)
        {   // 카운트 다운이 끝났을 때
            if (HP <= 0)
            {   // HP가 0이면 KO
                PlayerKO();
            }
            else
            {   // HP가 0보다 크면 다시 일어나기
                countDownText.gameObject.SetActive(false);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                               Quaternion.Euler(360.0f, this.transform.eulerAngles.y, this.transform.eulerAngles.z),
                                               5.0f * Time.deltaTime);

                if (this.transform.eulerAngles.x >= 359.0f)
                {
                    isDown = false;
                    KFGameManagerScript.isGaming = true;
                    countdown = 3.0f;
                }
            }
        }
        else
        {   // 카운트 다운을 세고 있을 때
            if (isTrigger)
                HP += (float)KFGameManagerScript.downCnt * 2.0f;
        }
    }

    void PlayerKO()
    {
        countDownText.gameObject.SetActive(false);
        KFGameManagerScript.PlayerKO();
        KOText.gameObject.SetActive(true);
        isKO = true;
    }
}
