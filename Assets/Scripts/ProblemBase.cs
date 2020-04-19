using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemBase : MonoBehaviour
{
    public GameHandler m_gameHandler;
    public PlanetController m_planetController;
    // Start is called before the first frame update
    void Start()
    {
        
    }
   public virtual void Init(GameHandler handler, PlanetController planet)
    {
        m_gameHandler = handler;
        m_planetController = planet;
        GetComponent<Grabbable>().m_planetController = planet;
    }
}
