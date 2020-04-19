using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmLine : MonoBehaviour
{
    LineRenderer lineRenderer;

    public Vector3 start {set; get;}
    public Vector3 end {set; get;}
    public double angleOffset {set; get;}
    public float maxArmLength {set; get;}

    void Awake() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // lineRenderer.SetPosition(0, start);
        // lineRenderer.SetPosition(1, end);
        Debug.Log(maxArmLength);
        drawArmParabola(start, end, angleOffset != 0, maxArmLength);
    }

    void drawArmParabola(
            Vector3 begin, 
            Vector3 end, 
            bool isLeftArm,
            float maxArmLength)
    {
        Vector3 rel = end - begin;
        float relMagnitude = rel.magnitude;

        if (relMagnitude > maxArmLength) {
            Vector3[] points = new Vector3[2];
            points[0] = begin;
            points[1] = end;
            lineRenderer.SetPositions(points);
            Debug.Log("here");
        } else {
            // Reference frame vectors
            Vector3 vx = rel.normalized;
            Vector3 vz = new Vector3(0.0f, 0.0f, 1.0f); // this should point towards the camera, not sure if I've done that
            Vector3 vy;
            if (isLeftArm) {
                vy = Vector3.Cross(vx, vz);
            } else {
                vy = - Vector3.Cross(vx, vz);
            }
            
            // Modelling the arm as two equal-length line segments joined by a hinge
            // Line segment span (in the reference frame)
            float dx = 0.5f * rel.magnitude;
            float dy = 0.5f * Mathf.Sqrt(maxArmLength*maxArmLength - relMagnitude*relMagnitude);
            float a = dy / (dx*dx); // coefficient of x^2

            int n_lineSegments = 16;
            int n_lineSegments_half = n_lineSegments / 2;
            float n_lineSegments_float = n_lineSegments;
            Vector3[] points = new Vector3[n_lineSegments+1];
            for (int i = 0; i <= n_lineSegments; i++) {
                float x = 2*((i - n_lineSegments_half)/n_lineSegments_float) * dx;
                float y = a*x*x;
                points[i] = begin + (x+dx)*vx + (y-dy)*vy;
            }

            lineRenderer.positionCount = n_lineSegments + 1;
            lineRenderer.SetPositions(points);
        }
    }
}
