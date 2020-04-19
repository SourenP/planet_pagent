using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public PlanetController m_planetController;
    public ShipController m_ship;
    public Rigidbody m_rigidBody;
    public bool m_isGrabbed = false;

    bool m_caressedByGrubbyHands = false;

    // Start is called before the first frame update
    void Start()
    {
        //Temp
        m_ship.m_speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isGrabbed && m_caressedByGrubbyHands && Input.GetMouseButton(0))
        {
            m_isGrabbed = true;
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
        m_isGrabbed = false;

        m_rigidBody.velocity = m_planetController.GetHandVelocity();
    }


    private void CarryMeFiltyHands()
    {
        m_rigidBody.velocity = Vector3.zero;
        transform.position = m_planetController.GetHandPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            m_caressedByGrubbyHands = true;
            Debug.Log("EEEEK");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            m_caressedByGrubbyHands = false;
            Debug.Log("PHEW");
        }
    }
}
