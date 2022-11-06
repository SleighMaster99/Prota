using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGlove : MonoBehaviour
{
    AudioSource audioPlayer;
    public AudioClip uppercutSound;
    public AudioClip jabSound;

    KFPlayerCtrl KFPlayerCtrlScript;
    KFEnemyCtrl KFEnemyCtrlScript;

    float gloveDamage;

    void Start()
    {
        audioPlayer = this.transform.root.transform.GetComponent<AudioSource>();

        KFPlayerCtrlScript = this.transform.parent.transform.GetComponent<KFPlayerCtrl>();

        gloveDamage = 10.0f;
    }

    void LateUpdate()
    {
        if(GameObject.FindWithTag("Enemy"))
            KFEnemyCtrlScript = GameObject.FindWithTag("Enemy").transform.GetComponent<KFEnemyCtrl>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Head"))
        {
            KFEnemyCtrlScript.HP -= gloveDamage * 1.5f;
            audioPlayer.PlayOneShot(uppercutSound);
        }
        else if (collision.transform.CompareTag("Body") || collision.transform.CompareTag("Glove"))
        {
            KFEnemyCtrlScript.HP -= gloveDamage * 0.8f;
            audioPlayer.PlayOneShot(jabSound);
        }
            
        KFPlayerCtrlScript.isHit = true;
    }
}
