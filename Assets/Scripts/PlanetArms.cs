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
    public double leftArmOffsetAngle = Math.PI;

    public ArmLine armLinePrefab;

    ArmLine leftArm;
    ArmLine rightArm; 

    // Debug point
    Transform pointTransform;

    void Awake() {
        Renderer renderer = gameObject.GetComponent<Renderer>(); // not sure if this will scale properly
        sphereRadius = renderer.bounds.extents.x;
        sphereTransform = gameObject.GetComponent<Transform>();

        leftArm = Instantiate(armLinePrefab, sphereTransform);
        LineRenderer leftArmLineRenderer = leftArm.GetComponent<LineRenderer>();
        leftArmLineRenderer.startWidth = armWidth;
        leftArmLineRenderer.endWidth = armWidth;
        leftArm.angleOffset = leftArmOffsetAngle;

        rightArm = Instantiate(armLinePrefab, sphereTransform);
        LineRenderer rightArmLineRenderer = rightArm.GetComponent<LineRenderer>();
        rightArmLineRenderer.startWidth = armWidth;
        rightArmLineRenderer.endWidth = armWidth;
        rightArm.angleOffset = 0;
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        double mouseAngle = GetMouseAngle();
        UpdateArm(ref leftArm);
        UpdateArm(ref rightArm);
    }


    void UpdateArm(ref ArmLine arm) {
        double maxAngle = arm.angleOffset + degToRad(thresholdAngle);
        double minAngle = arm.angleOffset - degToRad(thresholdAngle);
        Vector3 pointOnCirclePos = GetPointOnCircleRestricted(GetMouseAngle(), sphereRadius, minAngle, maxAngle);

        Vector3 armStart = pointOnCirclePos + sphereTransform.position;
        Vector3 armEnd = GetMousePosition(Vector3.zero);
        float armLength = Vector3.Distance(armStart, armEnd);
        if (armLength > maxArmLength) {
            Vector3 armDirection = (armEnd - armStart).normalized;
            armEnd = armStart + (armDirection * maxArmLength);
        }

        arm.start = armStart;
        arm.end = armEnd;
    }

    Vector3 GetPointOnCircleRestricted(double angle, float radius, double angleOne, double angleTwo) {
        angle = angle > Math.PI && angleOne < 0 ? angle - (2f * Math.PI) : angle;
        double minAngle = angleOne;
        double maxAngle = angleTwo;
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
        Vector3 mouseWorldPos = GetMousePosition(sphereTransform.position);
        double angle =  Mathf.Atan2(mouseWorldPos.y, mouseWorldPos.x);
        return angle > 0 ? angle : ((2f * Math.PI) + angle);
    }

    // Returns mouse position relative to point
    Vector3 GetMousePosition(Vector3 point) {
        float distanceToPlayer = Vector3.Distance(Camera.main.transform.position, point);
        Vector3 cameraPoint = Input.mousePosition;
        cameraPoint.z = distanceToPlayer;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(cameraPoint);
        return mouseWorldPos - point;
    }

    double degToRad(double deg) {
        double rad = deg * Math.PI / 180f;
        rad = rad > 0 ? rad : ((2f * Math.PI) + rad);
        //Debug.Log("rad " + rad);
        return rad;
    }

    public Vector3 GetMidPointOfHands()
    {
        Vector3 midPoint = leftArm.end - (leftArm.end - rightArm.end)/2;

        return midPoint;
    }
}