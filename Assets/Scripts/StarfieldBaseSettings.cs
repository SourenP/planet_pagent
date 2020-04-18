
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StarfieldBaseSettings", order = 2)]
public class StarfieldBaseSettings : ScriptableObject
{
    public int maxStars = 100;
    public float starSize = 0.1f;
    public float starSizeRange = 0.5f;
    public float parallaxFactor = 0.5f;
    public float fieldWidth = 20f;
    public float fieldHeight = 25f;
    public bool colorize = false;
}
