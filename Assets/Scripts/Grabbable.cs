using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public PlanetController m_planetController;
    public ProblemBase m_problemController;
    public Rigidbody m_rigidBody;
    public bool m_isGrabbed = false;

    bool m_caressedByGrubbyHands = false;

    // Start is called before the first frame update
    void Start()
    {
        //Temp
        //m_ship.m_speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isGrabbed && m_caressedByGrubbyHands && (Input.GetMouseButton(0)) && !m_planetController.m_holdingSomething)
        {
            m_isGrabbed = true;
            m_planetController.Grab(gameObject);
            m_problemController.Grabbed();
        }
        else if (m_isGrabbed)
        {
            if (Input.GetMouseButtonUp(0))
            {
                ReleaseMeFilthyHands();
            }
            else
            {
                CarryMeFiltyHands();
            }
        }
    }

    private void ReleaseMeFilthyHands()
    {
        m_planetController.Release();
        m_isGrabbed = false;
        m_rigidBody.isKinematic = false;
        m_rigidBody.velocity = m_planetController.GetHandVelocity();
        m_rigidBody.constraints = ~RigidbodyConstraints.FreezeAll;

        m_rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
    }


    private void CarryMeFiltyHands()
    {
        //transform.position = m_planetController.GetHandPosition();
        m_rigidBody.velocity = m_planetController.GetHandVelocity();
        //m_rigidBody.isKinematic = true;
        //m_rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            m_caressedByGrubbyHands = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            m_caressedByGrubbyHands = false;
        }
    }
}
