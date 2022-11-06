using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    public GameObject findPlayer;
    public GameObject[] enemyCannon;
    public GameObject[] enemyCannonBallSP;
    public GameObject[] cannonSmoke;
    public GameObject enemyCannonBall;
    GameObject enemyShipHP;
    GameObject playerShip;
    GameObject player;

    Transform[] spawnPoint;

    AudioSource audioPlayer;
    public AudioClip cannonFireSound;

    float pToeDistance;
    float shipSpeed;
    float playerRotationY;
    float attackTime;

    public int shipIndex;
    int goalPointIndex;

    bool isGoal;
    bool isFindPlayer;
    bool isAttackMode;
    bool isEnemyDie;
    bool isGetCoin;

    FindPlayer findPlayerScript;
    EnemyShipHP enemyShipHPScript;
    PlayerCtrl playerCtrlScript;
    EnemyCompass enemyCompass;

    Vector3 rayPosition;
    Vector3 rayDirectionL;
    Vector3 rayDirectionR;

    // Start is called before the first frame update
    void Start()
    {
        // bool
        isGoal = true;
        isFindPlayer = false;
        isAttackMode = false;
        isGetCoin = false;

        // flaot
        shipSpeed = 50.0f;
        attackTime = 0;

        // GameObject GetComponent
        spawnPoint = GameObject.Find("EnemySpawnPoints").GetComponentsInChildren<Transform>();
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        player = GameObject.FindGameObjectWithTag("Player");
        //enemyCompass = GameObject.Find("EnemyCompass").GetComponent<EnemyCompass>();

        // Scrit GetComponent
        findPlayerScript = findPlayer.GetComponent<FindPlayer>();
        playerCtrlScript = player.GetComponent<PlayerCtrl>();
        enemyShipHP = this.transform.GetChild(20).gameObject;
        enemyShipHPScript = enemyShipHP.GetComponent<EnemyShipHP>();

        // AudioSource GetComponent
        audioPlayer = this.transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //  EnemyShipHP 스크립트에서 적 생존 여부 받아오기
        isEnemyDie = enemyShipHPScript.isEnemyDie;

        // FindPlayer 스크립트에서 플레이어 탐지 여부 받아오기
        isFindPlayer = findPlayerScript.isFindPlayer;

        if (isEnemyDie)
            EnemyDie();                     // 적 HP가 0일 때
        else if (isGoal)
            GetGoal();                      // 목표 지점에 도착했을 때
        else if (isFindPlayer)
            FindPlayer();                   // 플레이어를 탐지했을 때
        else
        {
            isAttackMode = false;
            this.transform.LookAt(spawnPoint[goalPointIndex].transform);
            this.transform.position = Vector3.MoveTowards(this.transform.position, spawnPoint[goalPointIndex].transform.position, shipSpeed * Time.deltaTime);
        }

        // isAttackMode가 true면 공격
        if (isAttackMode)
            AttackPlayer();

        // 목표 위치 도착시 isGoal을 true로 변경
        if (this.transform.position == spawnPoint[goalPointIndex].transform.position)
            isGoal = true;

        Debug.DrawRay(rayPosition, rayDirectionL * 300.0f, Color.red);
        Debug.DrawRay(rayPosition, rayDirectionR * 300.0f, Color.red);
    }

    // 목표 위치 도착시 랜덤한 새로운 목표 생성
    void GetGoal()
    {
        goalPointIndex = Random.Range(1, 13);
        this.transform.LookAt(spawnPoint[goalPointIndex].transform);
        isGoal = false;
    }

    // 플레이어 탐지 isFindPlayer가 false면 목표지점으로 이동
    void FindPlayer()
    {
        pToeDistance = Vector3.Distance(this.transform.position, playerShip.transform.position);

        if (pToeDistance > 200)
        {   // 플레이어와 거리가 200보다 멀 때
            this.transform.LookAt(playerShip.transform);
            this.transform.position = Vector3.MoveTowards(this.transform.position, playerShip.transform.position, shipSpeed * Time.deltaTime);
            isAttackMode = false;
        }
        else
        {   // 플레이어와 거리가 200보다 가까울 때
            CheckRotation();
            this.transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, this.transform.eulerAngles.y, 0),
                Quaternion.Euler(0, playerRotationY, 0), 30.0f * Time.deltaTime);

            isAttackMode = true;
        }
    }

    // 플레이어의 위치에 따라 회전 각도 수정
    void CheckRotation()
    {
        float dX = (this.transform.position.x - playerShip.transform.position.x);
        float dY = (this.transform.position.y - playerShip.transform.position.y);
        float dZ = (this.transform.position.z - playerShip.transform.position.z);

        if (dX <= 30 && dX >= -30)
        {
            playerRotationY = Vector3.forward.y + 90.0f;
        }
        else if (dX > 30 && dZ > 30)
        {
            playerRotationY = Vector3.forward.y - 45.0f;
        }
        else if (dX > 30 && dZ < -30)
        {
            playerRotationY = Vector3.forward.y + 45.0f;
        }
        else if (dX < -30 && dZ < -30)
        {
            playerRotationY = Vector3.forward.y - 45.0f;
        }
        else if (dX < -30 && dZ > 30)
        {
            playerRotationY = Vector3.forward.y + 45.0f;
        }
        else
        {
            playerRotationY = Vector3.forward.y;
        }
    }

    // 공격모드
    void AttackPlayer()
    {
        attackTime += Time.deltaTime;
        rayPosition = new Vector3(this.transform.position.x, this.transform.position.y + 60.0f, this.transform.position.z);
        rayDirectionL = new Vector3(0, -1, 0) + -this.transform.right;
        rayDirectionR = new Vector3(0, -1, 0) + this.transform.right;

        // 4초 마다 함포 공격
        if (attackTime >= 4.0f)
        {
            RaycastHit hit;

            // 플레이어가 왼쪽에 있을 때 왼쪽 함포 사용
            if (Physics.Raycast(rayPosition, rayDirectionL, out hit, 300.0f))
            {
                if (hit.transform.CompareTag("AttackCollider"))
                {
                    enemyCannon[0].transform.LookAt(playerShip.transform);
                    audioPlayer.PlayOneShot(cannonFireSound);
                    cannonSmoke[0].SetActive(true);
                    Instantiate(enemyCannonBall, enemyCannonBallSP[0].transform.position, enemyCannonBallSP[0].transform.rotation);

                    // 거리가 가까우면 양쪽 함포 모두 사용
                    if (pToeDistance < 125.0f)
                    {
                        enemyCannon[0].transform.LookAt(playerShip.transform);
                        audioPlayer.PlayOneShot(cannonFireSound);
                        cannonSmoke[0].SetActive(true);
                        Instantiate(enemyCannonBall, enemyCannonBallSP[0].transform.position, enemyCannonBallSP[0].transform.rotation);
                    }
                }
            }

            // 플레이어가 오른쪽에 있을 때 오른쪽 함포 사용
            if (Physics.Raycast(rayPosition, rayDirectionR, out hit, 300.0f))
            {
                if (hit.transform.CompareTag("AttackCollider"))
                {
                    enemyCannon[1].transform.LookAt(playerShip.transform);
                    audioPlayer.PlayOneShot(cannonFireSound);
                    cannonSmoke[1].SetActive(true);
                    Instantiate(enemyCannonBall, enemyCannonBallSP[1].transform.position, enemyCannonBallSP[1].transform.rotation);

                    // 거리가 가까우면 양쪽 함포 모두 사용
                    if(pToeDistance < 125.0f)
                    {
                        enemyCannon[0].transform.LookAt(playerShip.transform);
                        audioPlayer.PlayOneShot(cannonFireSound);
                        cannonSmoke[0].SetActive(true);
                        Instantiate(enemyCannonBall, enemyCannonBallSP[0].transform.position, enemyCannonBallSP[0].transform.rotation);
                    }
                }
            }

            attackTime = 0;     // 시간 초기화
        }
        else if(attackTime >= 3.5f)
        {
            // 함포 발사시 연기 비활성화
            cannonSmoke[0].SetActive(false);
            cannonSmoke[1].SetActive(false);
        }
    }

    // enemyHP가 0일 때 침몰시키기
    void EnemyDie()
    {
        isFindPlayer = false;
        isAttackMode = false;

        if (!isGetCoin)
        {
            playerCtrlScript.coin += Random.Range(1000, 5001);
            playerCtrlScript.isKill = true;
            EnemySpawnManager.spawnedCnt -= 1;
            isGetCoin = true;
        }

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(-90.0f, this.transform.eulerAngles.y, this.transform.eulerAngles.z),
                                    1.0f * Time.deltaTime);

        this.transform.Translate(Vector3.down * Time.deltaTime * 5.0f, Space.World);

        if (this.transform.position.y < -70.0f)
        {
            Destroy(this.gameObject);
            playerCtrlScript.enemyshipIndex = shipIndex;
        }
    }
}
