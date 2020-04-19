
using UnityEngine;

public enum ProblemType
{
    Ship = 0,
    Asteroid = 1,

    NUM = 2
}

public enum ProblemRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,

    NUM = 3
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlanetProblem", order = 2)]

public class PlanetProblem : ScriptableObject
{
    public float speed;
    public GameObject prefab = null;
    public Color color;

    public ProblemType problemType;
    public ProblemRarity problemRarity;

    // For late
    public GameObject behaviorScript;
}
