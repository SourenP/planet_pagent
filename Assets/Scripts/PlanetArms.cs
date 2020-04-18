using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlanetArms : MonoBehaviour
{
    float sphereRadius;
    Transform sphereTransform;
    LineRenderer lineRenderer;

    public float thresholdAngle = 30;
    public float maxArmLength = 0.5f;
    public float armWidth = 0.1f;

    // Debug point
    Transform pointTransform;

    void Awake() {
        Renderer renderer = gameObject.GetComponent<Renderer>(); // not sure if this will scale properly
        sphereRadius = renderer.bounds.extents.x;
        sphereTransform = gameObject.GetComponent<Transform>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.startWidth = armWidth;
        lineRenderer.endWidth = armWidth;
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        DrawArm();        
    }


    void DrawArm() {
        Vector3 pointOnCirclePos = GetPointOnCircleRestricted(
            GetMouseAngle(), sphereRadius, thresholdAngle * Math.PI / 180f, -thresholdAngle * Math.PI / 180f);

        Vector3 armStart = pointOnCirclePos + sphereTransform.position;
        Vector3 armEnd = GetMousePosition(Vector3.zero);
        float armLength = Vector3.Distance(armStart, armEnd);
        if (armLength > maxArmLength) {
            Vector3 armDirection = (armEnd - armStart).normalized;
            armEnd = armStart + (armDirection * maxArmLength);
        }

        lineRenderer.SetPosition(0, armStart);
        lineRenderer.SetPosition(1, armEnd);
    }

    Vector3 GetPointOnCircleRestricted(double angle, float radius, double maxAngle, double minAngle) {
        angle = Math.Max(minAngle, angle);
        angle = Math.Min(maxAngle, angle);
        return GetPointOnCircle(angle, radius);
    }

    // Returns a point at an <angle> for a circle centered at 0,0 with a <radius>
    Vector3 GetPointOnCircle(double angle, float radius) {
        return new Vector3((float) Math.Cos(angle)*radius, (float) Math.Sin(angle)*radius, 0);
    }

    // Returns the angle in radians between the center of the sphere and the mouse relative to the x axis
    // Goes positive in the counterclockwise direction until PI and negative in clockwise until -PI
    double GetMouseAngle() {
        Vector3 mouseWorldPos = GetMousePosition(sphereTransform.transform.position);
        return Mathf.Atan2(mouseWorldPos.y, mouseWorldPos.x);
    }

    // Returns mouse position relative to point
    Vector3 GetMousePosition(Vector3 point) {
        float distanceToPlayer = Vector3.Distance(Camera.main.transform.position, point);
        Vector3 cameraPoint = Input.mousePosition;
        cameraPoint.z = distanceToPlayer;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(cameraPoint);
        return mouseWorldPos - point;
    }
}
