using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{

    public AudioClip m_shipExplosionSound;
    public AudioClip m_sunExplosionSound;

    private AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayShipExplosionSound() {
        m_audioSource.PlayOneShot(m_shipExplosionSound);
    }

    public void PlaySunExplosionSound() {
        m_audioSource.PlayOneShot(m_sunExplosionSound);
    }
}
