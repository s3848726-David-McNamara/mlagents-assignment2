using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject explosionParticles;
    public TrailRenderer tr;
    public float movementSpeed;
    private GameObject particle;
    private bool destroyed;

    private void Start()
    {
        particle = Instantiate(explosionParticles, transform.position, Quaternion.identity, transform.parent);
        particle.GetComponent<ParticleSystem>();
        particle.GetComponent<ParticleSystem>().Stop();
        particle.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.up * movementSpeed;
    }

    private IEnumerator TurnOff(float time)
    {
        particle.SetActive(true);
        destroyed = true;
        transform.GetChild(0).gameObject.SetActive(false);
        tr.Clear();
        particle.transform.localPosition = transform.localPosition;
        particle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((collider.CompareTag("Wall") || collider.CompareTag("Player")) && !destroyed)
        {
            tr.Clear();
            StartCoroutine(TurnOff(0.25f));
        }
    }


    private void OnEnable()
    {
        tr.Clear();
        destroyed = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
