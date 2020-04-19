using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public PlanetArms m_planetArms;

    private List<Vector3> m_mouseBuffer;
    private Vector3 m_mouseVelocity;

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
        transform.position = m_planetArms.GetMidPointOfHands();
        //while(m_mouseBuffer.Count >= MOUSE_BUFFER_MAX)
        //{
        //    m_mouseBuffer.RemoveAt(0);
        //}

        //m_mouseBuffer.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //m_mouseVelocity = Vector3.zero;

        //for(int i = 1; i < m_mouseBuffer.Count; ++i)
        //{
        //    m_mouseVelocity += m_mouseBuffer[i];
        //}

        m_mouseVelocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_lastMousePosition;
        m_lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_mouseVelocity.z = 0;
        Debug.Log(m_mouseVelocity);
    }

    public Vector3 GetVelocity()
    {
        return m_mouseVelocity * 10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.005f);
    }
}
