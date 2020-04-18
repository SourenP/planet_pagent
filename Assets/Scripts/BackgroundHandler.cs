using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    public StarfieldBaseSettings m_starfieldBaseSettings;

    public int m_maxStarIncrease = 30;
    public float m_starSizeDecrease = 0.1f;
    public float m_starSizeRangeDecrease = 0.05f;
    public float m_parallaxFactorIncrease = 0.1f;

    public List<StarfieldHandler> m_starfields;

    // Start is called before the first frame update
    public void Init()
    {
        StarfieldBaseSettings settings = new StarfieldBaseSettings();

        for(int i = 0; i < m_starfields.Count; ++i)
        {
            m_starfields[i].Init(settings);
            settings.maxStars += m_maxStarIncrease;
            settings.starSize -= m_starSizeDecrease;
            settings.starSizeRange -= m_starSizeRangeDecrease;
            settings.parallaxFactor += m_parallaxFactorIncrease;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        for (int i = 0; i < m_starfields.Count; ++i)
        {
            m_starfields[i].Step(cameraPos);
        }
    }
}
