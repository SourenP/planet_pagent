using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : ProblemBase
{
    public PolyGen m_polyGen;
    private GameHandler m_gameHandler;

    public float m_radius = 0.1f;

    public void SetTexture(Texture2D texture)
    {
        m_polyGen.SetTexture(texture);
    }

    
    public override void Init(GameHandler handler, PlanetController planet)
    {
        base.Init(handler, planet);
        m_polyGen.GenerateMesh(m_radius, 15);
        SetTexture(handler.m_textureGenerator.GenerateTexture(100, 100, 1));
        //m_isActive = true;
        //m_rigidBody.mass = m_mass;
    }
}
