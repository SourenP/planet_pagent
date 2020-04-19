using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public ParticleSystem m_explosionParticleSystem;
    public ParticleSystem m_debrisParticleSystem;

    public GameObject m_sunParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.transform.gameObject);

        m_sunParticleSystem.transform.position = collision.contacts[0].point;
        float angle = Vector3.Angle(new Vector3(1f, 0f, 0f), m_sunParticleSystem.transform.position - transform.position);
        m_sunParticleSystem.transform.rotation = Quaternion.Euler(angle, 90, 0);

        m_explosionParticleSystem.Play();
        m_debrisParticleSystem.Play();
    }
}
