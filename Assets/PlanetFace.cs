using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace : MonoBehaviour
{

    public GameObject facePrefab;
    GameObject face;

    Transform faceTransform;
    SpriteRenderer faceRenderer;


    public Vector3 spriteScale = new Vector3(0.23f, 0.23f, 0.1f);
    public Vector3 spritePosition = new Vector3(0, 0, -2);

    // Start is called before the first frame update
    void Start()
    {
        Transform planetTransform = GetComponent<Transform>();
        face = Instantiate(facePrefab, planetTransform);
        faceRenderer = face.GetComponent<SpriteRenderer>();
        faceTransform = face.GetComponent<Transform>();
        faceTransform.localPosition = spritePosition;
        faceTransform.localScale = spriteScale;
    }

    // Update is called once per frame
    void Update()
    {
    }
}

