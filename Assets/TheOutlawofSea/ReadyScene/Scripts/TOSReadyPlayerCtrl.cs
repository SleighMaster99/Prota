using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TOSReadyPlayerCtrl : MonoBehaviour
{
    public GameObject storeCanvas;
    public GameObject shoppingPoint;
    public GameObject[] usingCannon;

    AudioSource audioPlayer;
    public AudioClip stepSound;
    public AudioClip noMoneySound;
    public AudioClip buySound;

    public Image cursorGauge;

    public Text playerCoinText;
    public Text upgradeShipCoinText;
    public Text playerShipHPText;
    public Text repairShipCoinText;
    public Text cannonBallCntText;
    public Text IslandText;

    public Slider playerShipSlider;

    public Button[] cannonBuyButton;
    public Button repairShipButton;

    Vector3 screenCenter;
    Vector3 hitPoint;
    Vector3 orginPos;

    int coin;                       // 소지한 돈
    int shipCannonIndex;            // 배에 장착된 함포 종류
    int cannonBallCnt;              // 가지고 있는 포탄수
    int upgradeShipCoin;            // 배 업그레이드 하는데 필요한 비용
    int repairShipCoin;             // 배 수리시 필요한 비용
    int island;

    float gaugeTime;                // 게이지 증가시간 측정 
    float walkSpeed;                // 플레이어 걷는 속도
    float ballPower;                // 함포 파워
    float playerShipHP;             // 배 현재 HP
    float playerShipMaxHP;          // 배의 최대 HP
    float cannonBallDamage;         // 포탄 데미지
    float reloadTime;               // 함포 재장전 시간

    bool isFloor;                   // 갈 수 있는 바닥인지
    bool isMoving;                  // 플레이어가 움직이고 있는지
    bool isTriggered;               // 트리거가 클릭 되었는지
    bool isStoreHost;               // 상점 주인인지
    bool isShopping;                // 상점 이용중인지
    bool isBed;                     // 침대인지
    bool isSailor;                  // 선원인지

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        // Vector3
        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        // int
        coin = PlayerPrefs.GetInt("Coin");
        shipCannonIndex = PlayerPrefs.GetInt("ShipCannonIndex");
        cannonBallCnt = PlayerPrefs.GetInt("CannonBallCnt");
        upgradeShipCoin = PlayerPrefs.GetInt("UpgradeShipCoin");
        island = PlayerPrefs.GetInt("Island");

        // float
        ballPower = PlayerPrefs.GetFloat("ballPower");
        playerShipHP = PlayerPrefs.GetFloat("PlayerShipHP");
        playerShipMaxHP = PlayerPrefs.GetFloat("PlayerShipMaxHP");
        cannonBallDamage = PlayerPrefs.GetFloat("CannonBallDamage");
        reloadTime = PlayerPrefs.GetFloat("ReloadTime");
        walkSpeed = 20.0f;

        // bool
        isFloor = false;
        isMoving = false;
        isStoreHost = false;
        isShopping = false;
        isBed = false;
        isSailor = false;
    }

    // Update is called once per frame
    void Update()
    {
        isTriggered = Input.GetMouseButtonDown(0);

        if (isShopping)
            Shopping();         // 상점 이용중
        else
            IDLE();             // 기본상태

        // 코인 및 플레이어 배 HP, 섬
        playerCoinText.text = coin.ToString();
        cannonBallCntText.text = cannonBallCnt + " 개";
        playerShipHPText.text = playerShipHP + " / " + playerShipMaxHP;
        playerShipSlider.value = playerShipHP;
        playerShipSlider.maxValue = playerShipMaxHP;

        switch (island)
        {
            case 1:
                IslandText.text = "동쪽섬";
                break;
            case 2:
                IslandText.text = "서쪽섬";
                break;
            case 3:
                IslandText.text = "남쪽섬";
                break;
            case 4:
                IslandText.text = "북쪽섬";
                break;
        }
    }

    // 기본상태
    void IDLE()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        // Raycast
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            isFloor = hit.transform.CompareTag("FloorCollider");
            isStoreHost = hit.transform.name.Equals("StoreHost");
            isBed = hit.transform.name.Equals("Bed");
            isSailor = hit.transform.name.Equals("Sailor");

            // 게이지 증가 조건
            if ((isFloor || isStoreHost || isBed || isSailor) && !isMoving)
            {
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            }
            else
                gaugeTime = 0;

            // 게이지 꽉차거나 클릭했을 때
            if (gaugeTime >= 1.0f || isTriggered)
            {
                // 이동
                if (isFloor && !isMoving)
                {
                    hitPoint = new Vector3(hit.point.x, hit.point.y + 4.0f, hit.point.z);
                    isMoving = true;

                    StartCoroutine(MoveHitPoint());
                }

                // 상점 주인 선택
                if (isStoreHost)
                {
                    isShopping = true;
                    storeCanvas.SetActive(true);
                    orginPos = this.transform.position;
                    this.transform.position = shoppingPoint.transform.position;
                }

                // 침대
                if (isBed)
                {
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

                    SceneManager.LoadScene("TheOutlawofSeaStartScene");
                }

                // 선원
                if (isSailor)
                {
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

                    SceneManager.LoadScene("TheOutlawofSeaGameScene");
                }

                gaugeTime = 0;
            }
        }
        else
            gaugeTime = 0.0f;

        if (this.transform.position == hitPoint)
        {
            isMoving = false;
            isFloor = false;
        }
    }

    // 상점 이용중
    void Shopping()
    {
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        // Raycast
        if (Physics.Raycast(ray, out hit, 25.0f))
        {
            // 게이지 증가 조건
            if (hit.transform.CompareTag("Button"))
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            else
                gaugeTime = 0;

            // 게이지가 꽉차거나 트리거 클릭시
            if (gaugeTime > 1.0f || isTriggered)
            {
                if (hit.transform.CompareTag("Button"))
                    hit.transform.GetComponent<Button>().onClick.Invoke();
                gaugeTime = 0;
            }
        }
        else
            gaugeTime = 0.0f;

        // 배를 수리할 필요가 없을 때 구매칸 막기
        CheckMustFixShip();

        // 배 수리 비용 계산 및 가격텍스트 표시
        repairShipCoin = (int)(playerShipMaxHP - playerShipHP);
        repairShipCoinText.text = repairShipCoin.ToString();

        // 함포 구매 및 구매 불가능
        if (shipCannonIndex == 0)
        {
            cannonBuyButton[0].gameObject.SetActive(false);
            cannonBuyButton[1].gameObject.SetActive(true);
            cannonBuyButton[2].gameObject.SetActive(true);

            usingCannon[1].gameObject.SetActive(false);
            usingCannon[2].gameObject.SetActive(false);
        }
        else if (shipCannonIndex == 1)
        {
            cannonBuyButton[0].gameObject.SetActive(true);
            cannonBuyButton[1].gameObject.SetActive(false);
            cannonBuyButton[2].gameObject.SetActive(true);

            usingCannon[0].gameObject.SetActive(false);
            usingCannon[2].gameObject.SetActive(false);
        }
        else if (shipCannonIndex == 2)
        {
            cannonBuyButton[0].gameObject.SetActive(true);
            cannonBuyButton[1].gameObject.SetActive(true);
            cannonBuyButton[2].gameObject.SetActive(false);

            usingCannon[0].gameObject.SetActive(false);
            usingCannon[1].gameObject.SetActive(false);
        }

        usingCannon[shipCannonIndex].gameObject.SetActive(true);
    }

    // 배를 수리할 필요가 없을 때 구매칸 막기
    void CheckMustFixShip()
    {
        if (repairShipCoin <= 0)
            repairShipButton.gameObject.SetActive(false);
        else
            repairShipButton.gameObject.SetActive(true);
    }

    // 캐논0 구매 버튼
    public void Cannon0Button()
    {
        if (coin >= 5000)
        {
            coin -= 5000;
            shipCannonIndex = 0;
            ballPower = 2500.0f;
            cannonBallDamage = 100.0f;
            reloadTime = 3.0f;

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 캐논1 구매 버튼
    public void Cannon1Button()
    {
        if (coin >= 10000)
        {
            coin -= 10000;
            shipCannonIndex = 1;
            ballPower = 3500.0f;
            cannonBallDamage = 200.0f;
            reloadTime = 1.5f;

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 캐논2 구매 버튼
    public void Cannon2Button()
    {
        if (coin >= 20000)
        {
            coin -= 20000;
            shipCannonIndex = 2;
            ballPower = 4000.0f;
            cannonBallDamage = 200.0f;
            reloadTime = 3.0f;

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 포탄 5개 구매 버튼
    public void CannonBallButton1()
    {
        if (coin >= 500)
        {
            coin -= 500;
            cannonBallCnt += 5;

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 포탄 10개 구매 버튼
    public void CannonBallButton2()
    {
        if (coin >= 1000)
        {
            coin -= 1000;
            cannonBallCnt += 10;

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 배 수리 버튼
    public void RepairShipButton()
    {
        if(coin >= repairShipCoin)
        {
            coin -= repairShipCoin;
            playerShipHP += playerShipMaxHP - playerShipHP;

            audioPlayer.PlayOneShot(buySound);
        }
        else if(coin < repairShipCoin && coin > 0)
        {
            playerShipHP += coin;
            coin = 0;

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 배 업그레이드 버튼 - 최대 HP 증가
    public void UpgradeShipButton()
    {
        if (coin >= upgradeShipCoin)
        {
            playerShipMaxHP += 500;
            coin -= upgradeShipCoin;
            upgradeShipCoin += upgradeShipCoin;
            upgradeShipCoinText.text = upgradeShipCoin.ToString();

            audioPlayer.PlayOneShot(buySound);
        }
        else
        {
            // 돈없음
            audioPlayer.PlayOneShot(noMoneySound);
        }
    }

    // 상점 이용 종료 버튼
    public void ExitStoreButton()
    {
        isShopping = false;
        storeCanvas.SetActive(false);
        this.transform.position = orginPos;
    }

    IEnumerator MoveHitPoint()
    {
        while (isMoving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, hitPoint, Time.deltaTime * walkSpeed);

            if (!audioPlayer.isPlaying)
                audioPlayer.PlayOneShot(stepSound);

            yield return null;
        }
    }
}
