
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlanetProblem", order = 2)]

public class PlanetProblem : ScriptableObject
{
    public enum ProblemType
    {
        WreckingShip = 0,
        BombShip = 1,
        Asteroid = 2,

        NUM = 3
    }

    public enum ProblemRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,

        NUM = 3
    }

    public float speed;
    public GameObject prefab = null;
    public Color color;

    public ProblemType problemType;
    public ProblemRarity problemRarity;

    // For later
    public GameObject behaviorScript;
}
