using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparingGloves : MonoBehaviour
{
    AudioSource audioPlayer;
    public AudioClip uppercutSound;
    public AudioClip jabSound;

    TCPlayerCtrl TCPlayerCtrlScript;

    static int soundCnt;

    void Start()
    {
        audioPlayer = this.transform.root.transform.GetComponent<AudioSource>();

        TCPlayerCtrlScript = this.transform.parent.transform.GetComponent<TCPlayerCtrl>();

        soundCnt = 1;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Head"))
            audioPlayer.PlayOneShot(uppercutSound);
        else if(collision.transform.CompareTag("Body") || collision.transform.CompareTag("Glove"))
            audioPlayer.PlayOneShot(jabSound);
        else if (collision.transform.CompareTag("Punchbag"))
        {
            if(soundCnt % 3 == 0)
                audioPlayer.PlayOneShot(uppercutSound);
            else
                audioPlayer.PlayOneShot(jabSound);

            soundCnt++;
        }

        TCPlayerCtrlScript.isHit = true;
    }
}
