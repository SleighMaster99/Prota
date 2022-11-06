using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject waterEffcet;
    public GameObject child;
    GameObject player;

    AudioSource audioPlayer;
    public AudioClip explosionSound;

    float ballPower;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = this.transform.GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        ballPower = player.GetComponent<PlayerCtrl>().ballPower;
        this.transform.GetComponent<Rigidbody>().AddForce(this.transform.forward * ballPower);
    }

    void Update()
    {
        if (this.transform.position.y < -0.8f)
        {
            waterEffcet.SetActive(true);
            Destroy(this.gameObject, 2.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ship") || other.transform.CompareTag("PlayerShip"))
        {
            audioPlayer.PlayOneShot(explosionSound);
            explosionEffect.SetActive(true);
            Destroy(child);
        }
    }
}
