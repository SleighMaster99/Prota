using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparingEnemyGlove : MonoBehaviour
{
    AudioSource audioPlayer;
    public AudioClip uppercutSound;
    public AudioClip jabSound;

    SparingEnemyCtrl SparingEnemyCtrlScript;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = this.transform.root.transform.GetComponent<AudioSource>();

        SparingEnemyCtrlScript = this.transform.root.transform.GetComponent<SparingEnemyCtrl>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Head"))
            audioPlayer.PlayOneShot(uppercutSound);
        else if(collision.transform.CompareTag("Body") || collision.transform.CompareTag("Glove"))
            audioPlayer.PlayOneShot(jabSound);

        SparingEnemyCtrlScript.isHit = true;
    }
}
