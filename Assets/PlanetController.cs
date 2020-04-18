using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public GameObject m_sun;
    public float m_height;
    public float m_width;
    public float m_speed;

    public bool m_clockwise;

    Vector3 m_center;
    Vector3 m_direction;
    Vector3 m_position;
    float angle = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_center = m_sun.transform.position;
        transform.position.Set(0, m_sun.transform.position.y + m_height, 1);
        if (m_clockwise)
            m_direction = Vector3.right;
        else
            m_direction = Vector3.left;

        m_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        angle += m_speed;
        
        m_position.x = m_center.x + (m_width * Mathf.Cos(angle * .005f));
        m_position.y = m_center.y + (m_height * Mathf.Sin(angle * .005f));
        this.gameObject.transform.position = m_position;
    }
}
