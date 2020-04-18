using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmLine : MonoBehaviour
{
    LineRenderer lineRenderer;

    public Vector3 start {set; get;}
    public Vector3 end {set; get;}
    public double angleOffset {set; get;}

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
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
