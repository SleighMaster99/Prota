using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandCompass : MonoBehaviour
{
    public GameObject compassDir;
    public GameObject eastIsland;
    public GameObject westIsland;
    public GameObject southIsland;
    public GameObject northIsland;

    Vector3 compassRotation;

    float[] islandDistance;
    float shortDistance;

    int shortDistanceIndex;

    // Start is called before the first frame update
    void Start()
    {
        islandDistance = new float[4];
    }

    // Update is called once per frame
    void Update()
    {
        islandDistance[0] = Vector3.Distance(this.transform.position, eastIsland.transform.position);
        islandDistance[1] = Vector3.Distance(this.transform.position, westIsland.transform.position);
        islandDistance[2] = Vector3.Distance(this.transform.position, southIsland.transform.position);
        islandDistance[3] = Vector3.Distance(this.transform.position, northIsland.transform.position);

        ShortDistance();
        ShortIsland();
    }

    // 가장 짧은 거리 구하기
    void ShortDistance()
    {
        shortDistance = islandDistance[0];

        for(int i = 0; i < islandDistance.Length; i++)
        {
            if (shortDistance >= islandDistance[i])
            {
                shortDistance = islandDistance[i];
                shortDistanceIndex = i;
            }
        }
    }

    // 나침반을 가장 짧은 거리에 있는 섬으로 향하게 하기
    void ShortIsland()
    {
        if(shortDistanceIndex == 0)
        {
            compassDir.transform.LookAt(eastIsland.transform);
        }
        else if (shortDistanceIndex == 1)
        {
            compassDir.transform.LookAt(westIsland.transform);
        }
        else if (shortDistanceIndex == 2)
        {
            compassDir.transform.LookAt(southIsland.transform);
        }
        else if (shortDistanceIndex == 3)
        {
            compassDir.transform.LookAt(northIsland.transform);
        }
        
        compassRotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y,
                                        Camera.main.transform.eulerAngles.y - compassDir.transform.eulerAngles.y);
        this.transform.rotation = Quaternion.Euler(compassRotation);
    }
}
