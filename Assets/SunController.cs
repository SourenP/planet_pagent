using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public GameObject m_explosionPrefab;

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


        GameObject explosion = Instantiate(m_explosionPrefab);
        Transform explosionTransform = explosion.GetComponent<Transform>();
        explosionTransform.position = collision.contacts[0].point;
        Animator explosionAnimation = explosion.GetComponent<Animator>();
        float explosionAnimationLength = explosionAnimation.GetCurrentAnimatorStateInfo(0).length;
        Destroy(explosion, explosionAnimationLength);
    }
}
