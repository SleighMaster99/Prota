using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannonBall : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject waterEffcet;
    public GameObject child;

    AudioSource audioPlayer;
    public AudioClip explosionSound;

    float ballPower;

    // Start is called before the first frame update
    void Start()
    {
        ballPower = Random.Range(3800.0f, 4501.0f);
        this.transform.GetComponent<Rigidbody>().AddForce(this.transform.forward * ballPower);
        audioPlayer = this.transform.GetComponent<AudioSource>();
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
        if (other.transform.CompareTag("PlayerShip") || other.transform.CompareTag("Ship"))
        {
            audioPlayer.PlayOneShot(explosionSound);
            explosionEffect.SetActive(true);
            Destroy(child);
        }
    }
}
