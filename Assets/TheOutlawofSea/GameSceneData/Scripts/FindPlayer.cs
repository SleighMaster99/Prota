using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : MonoBehaviour
{
    public bool isFindPlayer;

    // Start is called before the first frame update
    void Start()
    {
        isFindPlayer = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player") || other.transform.CompareTag("PlayerShip"))
            isFindPlayer = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player") || other.transform.CompareTag("PlayerShip"))
            isFindPlayer = false;
    }
}
