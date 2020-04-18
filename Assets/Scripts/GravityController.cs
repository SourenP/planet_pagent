using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{

    public AsteroidController m_asteroid;
    public SphereCollider m_rangeTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("SpaceBody"))
            return;

        AsteroidController otherAst = other.gameObject.GetComponent<AsteroidController>();

        if (otherAst.m_attached || otherAst.m_beingPulled)
            return;

        float daddyComp = m_asteroid.m_mass / otherAst.m_mass;

        if (daddyComp > m_asteroid.m_pullMassThreshold)
            otherAst.BeginPull(m_asteroid);
    }

}
