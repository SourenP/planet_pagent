﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public PlanetController m_planet;
    public PlanetArms m_planetArms;
    public FixedJoint m_joint;
    public Rigidbody m_rigidBody;

    private List<Vector3> m_mouseBuffer;
    private Vector3 m_mouseVelocity;

    public float m_flickThreshold = 1f;

    private Vector3 m_lastMousePosition;

    private const int MOUSE_BUFFER_MAX = 5;

    // Start is called before the first frame update
    void Start()
    {
        m_mouseBuffer = new List<Vector3>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(m_joint.connectedBody == null)
        {
            m_planet.Release();
        }
    }

    private void FixedUpdate()
    {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = m_planetArms.GetMidPointOfHands();
        while (m_mouseBuffer.Count >= MOUSE_BUFFER_MAX)
        {
            m_mouseBuffer.RemoveAt(0);
        }

        m_mouseBuffer.Add(mousePosition - m_lastMousePosition);

        m_mouseVelocity = m_mouseBuffer[m_mouseBuffer.Count - 1];

        for (int i = m_mouseBuffer.Count - 2; i > -1; --i)
        {
            m_mouseVelocity += m_mouseBuffer[i];
        }

        //m_mouseVelocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_lastMousePosition;
        m_lastMousePosition = mousePosition;
        m_mouseVelocity.z = 0;
        m_rigidBody.velocity = m_mouseVelocity;
    }

    public void Attach(GameObject other)
    {
        //other.transform.parent = null;
        m_joint.transform.position = other.transform.position;
        //other.transform.position = this.transform.position;
        m_joint.connectedBody = other.GetComponent<Rigidbody>();
    }

    public void Release()
    {
        m_joint.connectedBody = null;
    }

    public Vector3 GetVelocity()
    {
        return m_mouseVelocity * 5;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.005f);
    }
}
