using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject ship;
    public GameObject shipDrivePoint;
    public GameObject wheel;
    public GameObject shipCannonPointL;
    public GameObject shipCannonPointR;
    public GameObject[] shipCannonL;
    public GameObject[] shipCannonR;
    public GameObject[] cannon;
    public GameObject[] cannonBallSP;
    public GameObject[] cannonSmoke;
    public GameObject cannonBall;
    public GameObject cannonBallCntImage;
    public GameObject playerCanvas;

    public Image cursorGauge;
    public Image reloadImage;
    public Image compass;

    AudioSource audioPlayer;
    public AudioClip cannonFireSound;
    public AudioClip stepSound;

    EnemyCompass enemyCompassScript;

    Vector3 screenCenter;
    Vector3 hitPoint;
    Vector3 originPos;      // 대포 흔들림 효과위한 원래 포지션

    float gaugeTime;
    // 배 운전 각도 변수
    float wheelAngleY;
    float shipSpeed;
    // 함포 사용 변수
    float reloadingTime;
    float reloadTime;
    public float ballPower;

    public bool isKill;
    bool isTriggered;       // 트리거 클릭
    bool isFloor;           // 배 바닥
    bool isMove;            // 플레이어가 배 위에서 움직이는 중인지
    bool isWheel;           // 플레이어 배 운전 여부
    bool isUseCannon;       // 플레이어 함포 사용 여부
    bool isReload;          // 함포 재장전 중

    public int coin;
    public int cannonBallCnt;      // 탄약개수
    public int shipCannonIndex;    // 배의 함포 종류
    public int enemyshipIndex;
    int selectCannon;       // 트리거한 함포가 왼쪽인지 오른쪽인지 0이면 왼쪽 1이면 오른쪽

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        gaugeTime = 0;
        shipSpeed = 75.0f;

        isTriggered = false;
        isFloor = false;
        isMove = false;
        isWheel = false;
        isUseCannon = false;
        isReload = false;

        // PlayerPrefs에서 데이터 받아오기
        coin = PlayerPrefs.GetInt("Coin");
        shipCannonIndex = PlayerPrefs.GetInt("ShipCannonIndex");
        cannonBallCnt = PlayerPrefs.GetInt("CannonBallCnt");
        ballPower = PlayerPrefs.GetFloat("ballPower");
        reloadTime = PlayerPrefs.GetFloat("ReloadTime");

        // 함포 흔들림 효과 위한 기본 위치
        originPos = cannon[shipCannonIndex].transform.localPosition;

        shipCannonL[shipCannonIndex].SetActive(true);
        shipCannonR[shipCannonIndex].SetActive(true);

        enemyCompassScript = playerCanvas.transform.GetChild(3).transform.GetChild(1).GetComponent<EnemyCompass>();
    }

    // Update is called once per frame
    void Update()
    {
        // 트리거
        isTriggered = Input.GetMouseButtonDown(0);
        
        if (isWheel)
            ShipDrive();    // 배 운전
        else if (isUseCannon)
            UseCannon();    // 함포 사용
        else
            Gaze();         // 배안에서 움직임

        // 적을 죽였을 때 적나침반 배열 수정
        if (isKill)
        {
            enemyCompassScript.enemyShips[enemyshipIndex] = null;

            isKill = false;
        }
    }

    // 배안에서 움직임
    void Gaze()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        // Raycast
        if (Physics.Raycast(ray, out hit, 25.0f))
        {
            isFloor = hit.transform.CompareTag("FloorCollider");

            // 게이지 증가 조건
            if (isFloor && !isMove && !isWheel && !isUseCannon)
            {
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            }
            else
                gaugeTime = 0;

            // 게이지 꽉차거나 클릭했을 때
            if (gaugeTime >= 1.0f || isTriggered && !isMove)
            {
                // 선택한 물체가 움직일 수 있는 공간일 때
                if (isFloor)
                {
                    isMove = true;
                    hitPoint = new Vector3(hit.point.x, hit.point.y + 2.5f, hit.point.z);

                    audioPlayer.PlayOneShot(stepSound);
                    this.transform.position = hitPoint;
                }

                // 선택한 물체가 키일 때
                if (hit.transform.name.Equals("Wheel"))
                    isWheel = true;

                // 선택한 물체가 함포일 때
                if (hit.transform.CompareTag("CannonL"))
                {
                    isUseCannon = true;
                    selectCannon = 0;
                }
                else if (hit.transform.CompareTag("CannonR"))
                {
                    isUseCannon = true;
                    selectCannon = 1;
                }

                gaugeTime = 0;
                isTriggered = false;
            }
        }
        else
            gaugeTime = 0.0f;

        // 클릭 지점에 도착시 isMove false
        if (this.transform.position == hitPoint)
            isMove = false;
    }

    // 배 운전
    void ShipDrive()
    {
        this.transform.position = shipDrivePoint.transform.position;
        this.transform.rotation = shipDrivePoint.transform.rotation;
        this.transform.SetParent(shipDrivePoint.transform, false);
        compass.gameObject.SetActive(true);

        // 배 이동
        if ((ship.transform.position.x <= 5000.0f && ship.transform.position.x >= -5000.0f)
            && (ship.transform.position.z <= 5000.0f && ship.transform.position.z >= -5000.0f))
        {
            ship.transform.Translate(Vector3.forward * Time.deltaTime * shipSpeed);
        }
        else
        {
            if (ship.transform.position.x > 5000.0f)
                ship.transform.position = new Vector3(ship.transform.position.x - 1.0f, ship.transform.position.y, ship.transform.position.z);

            if (ship.transform.position.x < -5000.0f)
                ship.transform.position = new Vector3(ship.transform.position.x + 1.0f, ship.transform.position.y, ship.transform.position.z);

            if (ship.transform.position.z > 5000.0f)
                ship.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, ship.transform.position.z - 1.0f);

            if (ship.transform.position.z < -5000.0f)
                ship.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, ship.transform.position.z + 1.0f);
        }

        // 플레이어가 보는 방향으로 회전
        ship.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, ship.transform.eulerAngles.y, 0),
            Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0), Time.deltaTime * 0.5f);

        // 키 회전
        wheelAngleY = ship.transform.eulerAngles.y;

        if (wheelAngleY > 0)
            wheel.transform.rotation = Quaternion.Euler(new Vector3(0, wheelAngleY, -wheelAngleY * 15.0f));
        else if(wheelAngleY < 0)
            wheel.transform.rotation = Quaternion.Euler(new Vector3(0, wheelAngleY, wheelAngleY * 15.0f));
        else
            wheel.transform.rotation = Quaternion.Euler(new Vector3(0, wheelAngleY, wheelAngleY));

        // 배 운전 그만두기
        if (isTriggered)
        {
            isWheel = false;
            this.transform.SetParent(null, false);
            compass.gameObject.SetActive(false);
            this.transform.position = shipDrivePoint.transform.position;
            this.transform.rotation = shipDrivePoint.transform.rotation;
        }
    }

    // 함포 사용
    void UseCannon()
    {
        cursorGauge.gameObject.SetActive(false);
        cannonBallCntImage.SetActive(true);

        if (selectCannon == 0)
        {
            shipCannonL[shipCannonIndex].SetActive(false);
            cannon[shipCannonIndex].SetActive(true);
            this.transform.position = new Vector3(shipCannonPointL.transform.position.x, shipCannonPointL.transform.position.y + 2.0f, shipCannonPointL.transform.position.z);
        }
        else if (selectCannon == 1)
        {
            shipCannonR[shipCannonIndex].SetActive(false);
            cannon[shipCannonIndex].SetActive(true);
            this.transform.position = new Vector3(shipCannonPointR.transform.position.x, shipCannonPointR.transform.position.y + 2.0f, shipCannonPointR.transform.position.z);
        }

        // 클릭시 함포 사격
        if (isTriggered && (mainCam.transform.eulerAngles.x <= 20.0f || mainCam.transform.eulerAngles.x >= 275.0f) 
            && !isReload && cannonBallCnt > 0)
        {
            StartCoroutine(Shake(0.1f, 0.2f));
            audioPlayer.PlayOneShot(cannonFireSound);
            
            if(shipCannonIndex == 2)
            {
                cannonSmoke[shipCannonIndex].SetActive(true);
                cannonSmoke[shipCannonIndex + 1].SetActive(true);
                cannonSmoke[shipCannonIndex + 2].SetActive(true);

                Instantiate(cannonBall, cannonBallSP[shipCannonIndex].transform.position, cannonBallSP[shipCannonIndex].transform.rotation);
                Instantiate(cannonBall, cannonBallSP[shipCannonIndex + 1].transform.position, cannonBallSP[shipCannonIndex + 1].transform.rotation);
                Instantiate(cannonBall, cannonBallSP[shipCannonIndex + 2].transform.position, cannonBallSP[shipCannonIndex + 2].transform.rotation);

                cannonBallCnt -= 3;
            }
            else
            {
                cannonSmoke[shipCannonIndex].SetActive(true);

                Instantiate(cannonBall, cannonBallSP[shipCannonIndex].transform.position, cannonBallSP[shipCannonIndex].transform.rotation);
                
                cannonBallCnt -= 1;
            }

            isReload = true;
        }

        // 장전시간
        if (isReload)
        {
            reloadImage.gameObject.SetActive(true);
            reloadingTime += Time.deltaTime;
            reloadImage.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 100.0f));

            if (reloadingTime >= reloadTime)
            {
                isReload = false;
                reloadingTime = 0;
                reloadImage.gameObject.SetActive(false);

                if (shipCannonIndex == 2)
                {
                    cannonSmoke[shipCannonIndex].SetActive(false);
                    cannonSmoke[shipCannonIndex + 1].SetActive(false);
                    cannonSmoke[shipCannonIndex + 2].SetActive(false);
                }
                else
                    cannonSmoke[shipCannonIndex].SetActive(false);
            }
        }

        // 뒤돌아보면 함포 사용 중지
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 25.0f) && hit.transform.gameObject.name.Equals("ExitCannon"))
        {
            isUseCannon = false;
            cursorGauge.gameObject.SetActive(true);
            shipCannonL[shipCannonIndex].SetActive(true);
            shipCannonR[shipCannonIndex].SetActive(true);
            cannon[shipCannonIndex].SetActive(false);
            cannonBallCntImage.SetActive(false);
            isReload = false;
            reloadingTime = 0;
            reloadImage.gameObject.SetActive(false);
            this.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 2.0f, hit.transform.position.z);

            if (shipCannonIndex == 2)
            {
                cannonSmoke[shipCannonIndex].SetActive(false);
                cannonSmoke[shipCannonIndex + 1].SetActive(false);
                cannonSmoke[shipCannonIndex + 2].SetActive(false);
            }
            else
                cannonSmoke[shipCannonIndex].SetActive(false);
        }
    }

    // 사격시 진동 효과
    IEnumerator Shake(float amount, float durationTime)
    {
        float timer = 0;

        while (timer <= durationTime)
        {
            cannon[shipCannonIndex].transform.localPosition = (Vector3)Random.insideUnitCircle * amount + originPos;

            timer += Time.deltaTime;
            yield return null;
        }

        cannon[shipCannonIndex].transform.localPosition = originPos;
    }
}
