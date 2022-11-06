using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TCPlayerCtrl : MonoBehaviour
{
    public GameObject boxingGloveL;
    public GameObject boxingGloveR;
    public GameObject glp;
    public GameObject grp;
    public GameObject punchPoint;
    public GameObject punchbagPoint;
    public GameObject sparingPoint;
    public GameObject headGear;
    public GameObject headAndbody;
    public GameObject head;

    public Image cursorGaugeImage;
    public Image energyGaugeImage;

    public Button punchbagUseButton;
    public Button punchbagExitButton;
    public Button sparingUseButton;
    public Button sparingExitButton;

    Vector3 ScreenCenter;
    Vector3 gloveLOriginPosition;
    Vector3 gloveROriginPosition;

    float cameraRZ;
    float gaugeTime;
    float energy;

    bool isTrigger;
    bool isPunch;
    bool isUsePunchbag;
    public bool isUseSparing;
    public bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        gaugeTime = 0;
        energy = 100.0f;

        isTrigger = false;
        isUsePunchbag = false;
        isUseSparing = false;
        isHit = false;

        ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        gloveLOriginPosition = boxingGloveL.transform.position;
        gloveROriginPosition = boxingGloveR.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        IDLE();

        // 샌드백 사용중
        if (isUsePunchbag)
        {
            Punch();

            energy = 100.0f;

            // 샌드백 사용중 뒤돌아보면 게이지 이미지 활성화
            if (Camera.main.transform.eulerAngles.y >= 125.0f && Camera.main.transform.eulerAngles.y <= 145.0f)
                cursorGaugeImage.gameObject.SetActive(true);
            else
                cursorGaugeImage.gameObject.SetActive(false);
        }

        // 스파링 사용중
        if (isUseSparing)
        {
            Sparing();
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

        headAndbody.transform.rotation = Camera.main.transform.rotation;
        this.transform.position = head.transform.position;
    }

    // 기본상태
    void IDLE()
    {
        Ray ray = Camera.main.ScreenPointToRay(ScreenCenter);
        RaycastHit hit;

        cursorGaugeImage.fillAmount = gaugeTime;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.transform.CompareTag("Button"))
            {
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            }
            else
                gaugeTime = 0;

            if(gaugeTime >= 1.0f)
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
        isTrigger = Input.GetMouseButtonDown(0);
        cameraRZ = Camera.main.transform.eulerAngles.z;

        if (isTrigger)
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

    void Sparing()
    {
        if (energy < 100.0f)
            energy += Time.deltaTime * 5.0f;

        energyGaugeImage.fillAmount = energy / 100.0f;

        Punch();

        // 스파링 사용중 뒤돌아보면 게이지 이미지 활성화
        if (Camera.main.transform.eulerAngles.y >= 215.0f && Camera.main.transform.eulerAngles.y <= 235.0f)
            cursorGaugeImage.gameObject.SetActive(true);
        else
            cursorGaugeImage.gameObject.SetActive(false);
    }

    // 샌드백 사용 버튼
    public void PunchbagUseButton()
    {
        punchbagUseButton.gameObject.SetActive(false);
        punchbagExitButton.gameObject.SetActive(true);
        cursorGaugeImage.gameObject.SetActive(false);

        isUsePunchbag = true;

        this.transform.root.position = punchbagPoint.transform.position;
    }

    // 샌드백 사용 종료 버튼
    public void PunchbagExitButton()
    {
        punchbagUseButton.gameObject.SetActive(true);
        punchbagExitButton.gameObject.SetActive(false);
        cursorGaugeImage.gameObject.SetActive(true);

        isUsePunchbag = false;

        this.transform.root.position = new Vector3(0, 14.0f, 0);
    }

    // 스파링 사용 버튼
    public void SparingUseButton()
    {
        sparingUseButton.gameObject.SetActive(false);
        sparingExitButton.gameObject.SetActive(true);
        cursorGaugeImage.gameObject.SetActive(false);
        energyGaugeImage.gameObject.SetActive(true);
        headGear.SetActive(true);

        energy = 100.0f;

        isUseSparing = true;

        this.transform.root.position = sparingPoint.transform.position;
    }

    // 스파링 사용 종료 버튼
    public void SparingExitButton()
    {
        sparingUseButton.gameObject.SetActive(true);
        sparingExitButton.gameObject.SetActive(false);
        cursorGaugeImage.gameObject.SetActive(true);
        energyGaugeImage.gameObject.SetActive(false);
        headGear.SetActive(false);

        isUseSparing = false;

        this.transform.root.position = new Vector3(0, 14.0f, 0);
    }

    // 트레이닝룸 나가기
    public void TrainnigRoomExitButton()
    {
        // 씬 변경
        SceneManager.LoadScene("KingFistMainScene");
    }
}
