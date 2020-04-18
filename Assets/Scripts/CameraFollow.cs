using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform m_cameraTransform;
    public Transform m_target;

    public float m_smoothSpeed = 0.125f;
    public Vector3 m_offset;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = m_target.position + m_offset;
        Vector3 smoothedPosition = Vector3.Lerp(m_cameraTransform.position, desiredPosition, m_smoothSpeed);

        m_cameraTransform.position = smoothedPosition;
    }
}
