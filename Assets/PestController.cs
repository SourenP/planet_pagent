using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestController : MonoBehaviour
{
    public bool m_onPlanet;

    bool m_infest;

    public PlanetController m_planet;
    public Rigidbody m_rigidBody;
    
    public float m_moveSpeed;
    public float m_diveBombSpeed;
    public float m_divePositionAngularDeviation = 15f;
    public Color m_color;

    Vector3 m_direction;
    Vector3 m_position;
    float m_angleOnPlanet;

    public float m_spriteChangeTimer = 2f;
    public SpriteRenderer m_spriteRenderer;
    public Sprite m_diveSprite;
    public Sprite[] m_planetSprites = new Sprite[4];
    public int m_spriteCount;
    float m_currentSpriteIndex = 0;

    bool m_collidedWithPlanet = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(bool onPlanet, PlanetController planetController, bool infestation = false)
    {
        m_planet = planetController;
        m_onPlanet = onPlanet;
        m_infest = infestation;
        if (m_onPlanet)
        {
            m_angleOnPlanet = m_planet.GetPestSlot();
            GetComponent<BoxCollider>().isTrigger = true;
            RandomizeMe();
        }
        else
        {
            m_spriteRenderer.sprite = m_diveSprite;

            m_spriteRenderer.color = m_color = Color.white;
            m_direction = pestDirection(
            m_planet.m_angle,
            m_planet.m_speed,
            new Vector2(transform.position.x, transform.position.y),
            m_diveBombSpeed
            );
            transform.up = m_direction;

            m_rigidBody.velocity = m_direction * m_diveBombSpeed;
            Debug.Log(m_rigidBody.velocity);
        }


    }



    Vector3 pestDirection(
        float planet_angle,
        float planet_angle_speed,
        Vector2 ship_pos,
        float ship_speed
)
    {
        if (ship_speed == 0 || planet_angle_speed == 0)
            return Vector3.zero;

        // Note: We will need to make sure that the units for time are the same for this function and the rest of the code.
        // Note: This code is repetitive. Can you show me how one would make it nicer in Unity?

        // Initial search interval
        float t0 = 0.0f;
        float t1 = 1.0f; // adjustable parameter

        // Parameter
        float t_scale = 1.2f; // upper bound search scale
        float t_eps = 0.001f; // tolerance for finding the time of intersection


        // Scale time upper bound (t1) until an intersect
        float h; // arbitrary name
        do
        {
            t1 *= t_scale;


            Vector2 dxy = m_planet.NextPosition2d(planet_angle + t1 * planet_angle_speed) - ship_pos;
            // float dx = orbit_x_radius * Mathf.Cos(planet_angle + t1 * planet_angle_speed) - ship_pos.x;
            // float dy = orbit_y_radius * Mathf.Sin(planet_angle + t1 * planet_angle_speed) - ship_pos.y;
            // float d2 = dx * dx + dy * dy;
            float d2 = dxy.sqrMagnitude;
            float ship_d = t1 * ship_speed;
            float ship_d2 = ship_d * ship_d;
            h = d2 - ship_d2;
        } while (h > 0.0f);

        // Bisect the search interval until its length is smaller than t_eps
        float t_mid;
        while (t1 - t0 > t_eps)
        {
            t_mid = 0.5f * (t0 + t1);
            Vector2 dxy = m_planet.NextPosition2d((planet_angle + t_mid * planet_angle_speed)) - ship_pos;
            // float dx = orbit_x_radius * Mathf.Cos((planet_angle + t_mid * planet_angle_speed) * Mathf.Deg2Rad) - ship_pos.x;
            // float dy = orbit_y_radius * Mathf.Sin((planet_angle + t_mid * planet_angle_speed) * Mathf.Deg2Rad) - ship_pos.y;
            // float d2 = dx * dx + dy * dy;
            float d2 = dxy.sqrMagnitude;
            float ship_d = t_mid * ship_speed;
            float ship_d2 = ship_d * ship_d;
            float h_mid = d2 - ship_d2;

            if (h_mid > 0.0f)
            {
                t0 = t_mid;
            }
            else
            {
                t1 = t_mid;
            }
        }

        // Get the position of the planet relative to the ship's starting position at the time of intersection
        float intersect_t = 0.5f * (t0 + t1);

        Vector2 intersect_dxy = m_planet.NextPosition2d((planet_angle + intersect_t * planet_angle_speed)) - ship_pos;
        float intersect_length = intersect_dxy.magnitude;
        // float intersect_dx = orbit_x_radius * Mathf.Cos((planet_angle * Mathf.Deg2Rad + intersect_t * planet_angle_speed)* Mathf.Deg2Rad) - ship_pos.x;
        // float intersect_dy = orbit_y_radius * Mathf.Sin((planet_angle * Mathf.Deg2Rad + intersect_t * planet_angle_speed) * Mathf.Deg2Rad) - ship_pos.y;
        // float intersect_length = Mathf.Sqrt(intersect_dx * intersect_dx + intersect_dy * intersect_dy);

        Vector3 ship_heading;
        if (intersect_length > 0.0f)
        {
            Vector2 intersect = intersect_dxy / intersect_length;
            ship_heading = new Vector3(intersect.x, intersect.y, 0.0f);
        }
        else
        {
            ship_heading = new Vector3(0.0f, 0.0f, 0.0f);
        }
        return ship_heading;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_onPlanet)
        {
            if(!m_infest)
                MoveOnPlanet();
        }
        else
        {
            MoveTowardsPlanet();
        }
    }

    void MoveTowardsPlanet()
    {
        
    }

    public float m_sineAmp = 0.1f;

    public Vector3 NextPosition3d(float angle, Vector3 center, float radius)
    {
        Vector2 position_2d = NextPosition2d(angle, center, radius);
        Vector3 position_3d = new Vector3(position_2d.x, position_2d.y, 1f);
        return position_3d;
    }

    public Vector2 NextPosition2d(float angle, Vector3 center, float radius)
    {
        return new Vector2(
            center.x + (radius * Mathf.Cos(angle * Mathf.Deg2Rad)),
            center.y + (radius * Mathf.Sin(angle * Mathf.Deg2Rad))
        );
    }

    void MoveOnPlanet()
    {
        //m_angleOnPlanet += m_moveSpeed * Time.deltaTime;

        m_position = NextPosition3d(m_angleOnPlanet, m_planet.transform.position, m_planet.GetRadius());
        this.gameObject.transform.up = (transform.position - m_planet.transform.position).normalized;
        
        float modifier = Mathf.Clamp(Mathf.Sin(Time.time * m_sineAmp) + 1, 1, 1.5f);
        m_position += (modifier) * gameObject.transform.up * m_spriteRenderer.sprite.bounds.max.y / 2;

        this.gameObject.transform.position = m_position;
    }

    void RandomizeMe()
    {
        int spriteIndex = Random.Range(0, m_spriteCount);
        m_spriteRenderer.sprite = m_planetSprites[spriteIndex];

        m_color = new Color(Random.Range(0f, 1f),
                            Random.Range(0f, 1f),
                            Random.Range(0f, 1f),
                            1f);
        m_spriteRenderer.color = m_color;
        StartCoroutine(RandomizeMeAgain());

    }

    IEnumerator RandomizeMeAgain()
    {
        yield return new WaitForSeconds(m_spriteChangeTimer);

        RandomizeMe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("PlanetSurface") && !m_collidedWithPlanet)
        {
            m_collidedWithPlanet = true;
            m_planet.AddPest();
            Destroy(this.gameObject);
        }
    }
}
