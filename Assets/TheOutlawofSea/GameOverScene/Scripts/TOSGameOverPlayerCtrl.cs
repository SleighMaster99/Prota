using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TOSGameOverPlayerCtrl : MonoBehaviour
{
    public Image cursorGauge;

    AudioSource audioPlayer;
    public AudioClip stepSound;

    Vector3 screenCenter;
    Vector3 hitPoint;

    float gaugeTime;
    float walkSpeed;

    bool isFloor;
    bool isTrigger;
    bool isMoving;
    bool isTomb;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        walkSpeed = 20.0f;

        isFloor = false;
        isTrigger = false;
        isMoving = false;
        isTomb = false;
    }

    // Update is called once per frame
    void Update()
    {
        isTrigger = Input.GetMouseButtonDown(0);

        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        cursorGauge.fillAmount = gaugeTime;

        // Raycast
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            isFloor = hit.transform.CompareTag("FloorCollider");
            isTomb = hit.transform.CompareTag("Tomb");

            if ((isFloor || isTomb) && !isMoving)
            {
                gaugeTime += 1.0f / 2.0f * Time.deltaTime;
            }
            else
                gaugeTime = 0;

            if (gaugeTime >= 1.0f || isTrigger)
            {
                if (isFloor && !isMoving)
                {
                    hitPoint = new Vector3(hit.point.x, hit.point.y + 4.0f, hit.point.z);
                    isMoving = true;

                    StartCoroutine(MovePlayer());
                }

                if (isTomb)
                {
                    PlayerPrefs.DeleteKey("haveTOSData");
                    SceneManager.LoadScene("TheOutlawofSeaStartScene");
                }

                gaugeTime = 0;
            }
        }
        else
            gaugeTime = 0;

        if (this.transform.position == hitPoint)
        {
            isMoving = false;
            isFloor = false;
        }
    }

    IEnumerator MovePlayer()
    {
        while (isMoving)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, hitPoint, walkSpeed * Time.deltaTime);

            if (!audioPlayer.isPlaying)
                audioPlayer.PlayOneShot(stepSound);
            
            yield return null;
        }
    }
}
