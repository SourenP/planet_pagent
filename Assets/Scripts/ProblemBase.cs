using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemBase : MonoBehaviour
{
    public PlanetProblem.ProblemType m_problemType;
    public GameHandler m_gameHandler;
    public PlanetController m_planet;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public virtual void Init(GameHandler handler, PlanetController planet, PlanetProblem.ProblemType type)
    {
        m_problemType = type;
        m_gameHandler = handler;
        m_planet = planet;
        GetComponent<Grabbable>().m_planetController = planet;
    }

    public virtual void Grabbed()
    {

    }
}
