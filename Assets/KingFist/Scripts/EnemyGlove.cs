using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGlove : MonoBehaviour
{
    GameObject player;

    AudioSource audioPlayer;
    public AudioClip uppercutSound;
    public AudioClip jabSound;

    KFEnemyCtrl KFEnemyCtrlCtrlScript;
    KFPlayerCtrl KFPlayerCtrlScript;

    float gloveDamage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        audioPlayer = this.transform.root.transform.GetComponent<AudioSource>();

        KFEnemyCtrlCtrlScript = this.transform.root.transform.GetComponent<KFEnemyCtrl>();
        KFPlayerCtrlScript = player.transform.GetChild(0).transform.GetComponent<KFPlayerCtrl>();

        gloveDamage = 10.0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Head"))
        {
            KFPlayerCtrlScript.HP -= gloveDamage * 1.5f;
            audioPlayer.PlayOneShot(uppercutSound);
        }
        else if (collision.transform.CompareTag("Body") || collision.transform.CompareTag("Glove"))
        {
            KFPlayerCtrlScript.HP -= gloveDamage * 0.8f;
            audioPlayer.PlayOneShot(jabSound);
        }

        KFEnemyCtrlCtrlScript.isHit = true;
    }
}
