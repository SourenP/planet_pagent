using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AsteroidController m_asteroid;

    public float m_mouseBuffer = 1f;

    private bool m_didSpin;
    private float m_spinDir = 1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_asteroid.transform.position;

        AsteroidFixedUpdateParam param = new AsteroidFixedUpdateParam();

        if (Input.GetMouseButton(0))
        {
            param.m_direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            param.m_direction.z = 0f;
            param.m_direction.Normalize();
        }

        if (Input.GetMouseButton(1))
        {
            param.m_torque.z = 1;
        }
        else if (m_didSpin)
        {
            param.m_torque.z = -1;
        }

        m_didSpin = false;

        m_asteroid.SetFixedStepParam(param);
    }
}
