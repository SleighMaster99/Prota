using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparingEnemyCtrl : MonoBehaviour
{
    public GameObject player;
    public GameObject gloveL;
    public GameObject gloveR;
    public GameObject headPoint;
    public GameObject bodyPoint;
    public GameObject glp;
    public GameObject grp;

    TCPlayerCtrl TCPlayerCtrlScript;

    float punchTime;

    int randomRotate;
    int randomPunch;

    bool isRotating;
    bool isZero;
    bool isUseSparing;
    bool isPunch;
    bool isTrigger;
    public bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        TCPlayerCtrlScript = player.transform.GetChild(0).GetComponent<TCPlayerCtrl>();

        punchTime = 0;

        isRotating = false;
        isZero = true;
        isUseSparing = TCPlayerCtrlScript.isUseSparing;
        isPunch = false;
        isTrigger = true;
        isHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        isUseSparing = TCPlayerCtrlScript.isUseSparing;

        if (isUseSparing)
        {
            punchTime += Time.deltaTime;

            Moving();

            if(punchTime >= 2.0f)
                Punch();
        }
    }

    // 스파링 적 움직임
    void Moving()
    {
        // 피할 방향 정하기
        if (!isRotating)
        {
            if (isZero)
            {
                randomRotate = Random.Range(0, 2);
                isRotating = true;
                isZero = false;
            }
            else
            {
                if (this.transform.eulerAngles.z <= 0.1f || this.transform.eulerAngles.z >= 359.9f)
                {
                    isZero = true;
                }
                else
                {
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                        Quaternion.Euler(this.transform.eulerAngles.x, this.transform.eulerAngles.y, 0), 5.0f * Time.deltaTime);
                }
            }
        }
        else
        {
            if (randomRotate == 0)
            {   // 왼쪽으로 피하기
                if (this.transform.eulerAngles.z <= 60.0f || this.transform.eulerAngles.z >= 359.9f)
                    this.transform.Rotate(Vector3.forward * Time.deltaTime * 300.0f);
                else
                    isRotating = false;
            }
            else
            {   // 오른쪽으로 피하기
                if (this.transform.eulerAngles.z >= 300.0f || this.transform.eulerAngles.z <= 0.1f)
                    this.transform.Rotate(Vector3.back * Time.deltaTime * 300.0f);
                else
                    isRotating = false;
            }
        }
    }

    void Punch()
    {
        if (isTrigger && !isPunch)
        {
            randomPunch = Random.Range(0, 4);

            isTrigger = false;
            isPunch = true;
        }

        if (isPunch)
        {
            if (randomPunch == 0)
                gloveL.transform.position = Vector3.MoveTowards(gloveL.transform.position, headPoint.transform.position, 50.0f * Time.deltaTime);
            else if (randomPunch == 1)
                gloveL.transform.position = Vector3.MoveTowards(gloveL.transform.position, bodyPoint.transform.position, 50.0f * Time.deltaTime);
            else if (randomPunch == 2)
                gloveR.transform.position = Vector3.MoveTowards(gloveR.transform.position, headPoint.transform.position, 50.0f * Time.deltaTime);
            else if (randomPunch == 3)
                gloveR.transform.position = Vector3.MoveTowards(gloveR.transform.position, bodyPoint.transform.position, 50.0f * Time.deltaTime);

            if (gloveL.transform.position == headPoint.transform.position || gloveL.transform.position == bodyPoint.transform.position ||
                gloveR.transform.position == headPoint.transform.position || gloveR.transform.position == bodyPoint.transform.position)
            {
                isPunch = false;
                isHit = true;
            }
        }
        else
        {
            if ((gloveL.transform.position != glp.transform.position || gloveR.transform.position != grp.transform.position) && isHit)
            {
                gloveL.transform.position = Vector3.MoveTowards(gloveL.transform.position, glp.transform.position, 50.0f * Time.deltaTime);
                gloveR.transform.position = Vector3.MoveTowards(gloveR.transform.position, grp.transform.position, 50.0f * Time.deltaTime);
            }
            else
            {
                punchTime = 0;
                isTrigger = true;
                isHit = false;
            }
        }
    }
}
