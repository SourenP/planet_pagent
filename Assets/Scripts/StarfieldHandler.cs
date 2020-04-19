using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// http://guidohenkel.com/2018/05/endless_starfield_unity/
public class StarfieldHandler : MonoBehaviour
{
    public int m_maxStars = 0;
    public float m_starSize = 0.1f;
    public float m_starSizeRange = 0.5f;
    public float m_fieldWidth = 20f;
    public float m_fieldHeight = 25f;
    public float m_parallaxFactor = 0.4f;
    public bool Colorize = false;

    float m_xOffset;
    float m_yOffset;

    ParticleSystem m_particles;
    ParticleSystem.Particle[] m_stars;


    public void Init(StarfieldBaseSettings settings)
    {

        m_maxStars = settings.maxStars;
        m_starSize = settings.starSize;
        m_starSizeRange = settings.starSizeRange;
        m_fieldHeight = settings.fieldHeight;
        m_fieldWidth = settings.fieldWidth;
        m_parallaxFactor = settings.parallaxFactor;

        m_stars = new ParticleSystem.Particle[m_maxStars];
        m_particles = GetComponent<ParticleSystem>();

        Assert.IsNotNull(m_particles, "Particle system missing from object!");

        m_xOffset = m_fieldWidth * 0.5f;                                                                                                        // Offset the coordinates to distribute the spread
        m_yOffset = m_fieldHeight * 0.5f;                                                                                                       // around the object's center

        for (int i = 0; i < m_maxStars; i++)
        {
            float randSize = Random.Range(m_starSizeRange, m_starSizeRange + 1f);                       // Randomize star size within parameters
            float scaledColor = (true == Colorize) ? randSize - m_starSizeRange : 1f;         // If coloration is desired, color based on size

            m_stars[i].position = GetRandomInRectangle(m_fieldWidth, m_fieldHeight) + transform.position;
            m_stars[i].startSize = m_starSize * randSize;
            m_stars[i].startColor = new Color(1f, scaledColor, scaledColor, 1f);
        }

        m_particles.SetParticles(m_stars, m_stars.Length);                                                                // Write data to the particle system
    }


    // GetRandomInRectangle
    //----------------------------------------------------------
    // Get a random value within a certain rectangle area
    //
    Vector3 GetRandomInRectangle(float width, float height)
    {
        float x = Random.Range(0, width);
        float y = Random.Range(0, height);
        return new Vector3(x - m_xOffset, y - m_yOffset, 0);
    }

    public void Step(Vector3 cameraPos)
    {
        if (m_stars == null)
            return;
        for (int i = 0; i < m_maxStars - 1; i++)
        {
            if (m_stars == null || cameraPos == null || m_stars.Length < i || transform == null)
                continue;
            Vector3 pos = m_stars[i].position + transform.position;

            if (pos.x < (cameraPos.x - m_xOffset))
            {
                pos.x += m_fieldWidth;
            }
            else if (pos.x > (cameraPos.x + m_xOffset))
            {
                pos.x -= m_fieldWidth;
            }

            if (pos.y < (cameraPos.y - m_yOffset))
            {
                pos.y += m_fieldHeight;
            }
            else if (pos.y > (cameraPos.y + m_yOffset))
            {
                pos.y -= m_fieldHeight;
            }

            m_stars[i].position = pos - transform.position;
        }
        m_particles.SetParticles(m_stars, m_stars.Length);
        Vector3 newPos = cameraPos * m_parallaxFactor;                   // Calculate the position of the object
        newPos.z = 0;                       // Force Z-axis to zero, since we're in 2D
        transform.position = newPos;
    }
}