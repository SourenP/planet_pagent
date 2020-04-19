using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanCounter : MonoBehaviour
{
    public PlanetController planetController;
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>(); 
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Human Count: " + planetController.humanCount;
    }
}
