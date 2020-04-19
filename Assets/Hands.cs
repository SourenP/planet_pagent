using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public PlanetArms m_planetArms;

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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = m_planetArms.GetMidPointOfHands();
        while (m_mouseBuffer.Count >= MOUSE_BUFFER_MAX)
        {
            m_mouseBuffer.RemoveAt(0);
        }

        m_mouseBuffer.Add(mousePosition - m_lastMousePosition);

        m_mouseVelocity = m_mouseBuffer[m_mouseBuffer.Count-1];

        for (int i = m_mouseBuffer.Count - 2; i > -1; --i)
        {
            m_mouseVelocity += m_mouseBuffer[i];
        }

        //m_mouseVelocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_lastMousePosition;
        m_lastMousePosition = mousePosition;
        m_mouseVelocity.z = 0;
        Debug.Log(m_mouseVelocity);
    }

    public bool HandleFlick()
    {
        return false;
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
