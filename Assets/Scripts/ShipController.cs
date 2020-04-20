using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipController : ProblemBase
{
    public float m_selfDestructDelay = 5f;
    public float m_bombShipDelay = 3f;
    public float m_pestReleaseDelay = 1f;
    public int m_pestCount = 10;
    public float m_speed = 0.2f;

    Transform m_transform;
    public Vector3 spriteScale = new Vector3(0.5f, 0.5f, 0f);

    public SpriteRenderer m_renderer;
    public Sprite m_wreckingShipSprite;
    public Sprite m_bombShipSprite;

    public GameObject m_pestPrefab;
    public float m_pestSpawnInterval = 0.1f;

    public GameObject m_wreckingShipPrefab;
    public GameObject m_bombShipPrefab;

    GameObject m_wreckingShip;
    GameObject m_bombShip;

    public GameObject m_explosionPrefab;
    public Vector3 explosionScale = new Vector3(0.2f, 0.2f, 0f);

    Vector3 m_direction;
    Vector3 getShipDirection(
            float planet_angle,
            float planet_angle_speed,
            Vector2 ship_pos,
            float ship_speed
    )
    {
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

    // Start is called before the first frame update
    void Start()
    {

        //m_direction = getShipDirection(
        //    m_planet.m_angle,
        //    m_planet.m_speed,
        //    new Vector2(transform.position.x, transform.position.y),
        //    m_speed
        //);
       // ReleasePests();
        Init(m_gameHandler, m_planet, m_problemType);

        m_transform = GetComponent<Transform>(); 

        if (m_problemType == PlanetProblem.ProblemType.WreckingShip)
        {
            m_wreckingShip = Instantiate(m_wreckingShipPrefab, m_transform);
            m_wreckingShip.GetComponent<Transform>().localScale = spriteScale;
        }
        else
        {
            m_bombShip = Instantiate(m_bombShipPrefab, m_transform);
            m_bombShip.GetComponent<Transform>().localScale = spriteScale;
        }
    }

    public override void Init(GameHandler gameHandler, PlanetController planet, PlanetProblem.ProblemType type)
    {
        base.Init(gameHandler, planet, type);
        m_direction = getShipDirection(
            m_planet.m_angle,
            m_planet.m_speed,
            new Vector2(transform.position.x, transform.position.y),
            m_speed
        );
        transform.up = m_direction;

    }

    // Update is called once per frame
    void Update()
    {
        // DO NOT DO THIS
        //m_direction = getShipDirection(
        //    m_planet.m_angle,
        //    m_planet.m_speed,
        //    new Vector2(transform.position.x, transform.position.y),
        //    m_speed
        //);
        transform.position += m_direction * m_speed * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player"))
            return;

        m_speed = 0;
        gameObject.transform.parent = other.transform;

        StartExplosionTimer(true);

    }

    public override void Grabbed()
    {
        m_speed = 0;
        if (m_problemType == PlanetProblem.ProblemType.BombShip)
        {
            StartExplosionTimer(false);
        }
    }

    public void StartExplosionTimer(bool timedOut)
    {
        float explosionTime = m_selfDestructDelay;
        if (!timedOut)
            explosionTime = m_bombShipDelay;

        StartCoroutine(ReleaseTimer(explosionTime - m_pestReleaseDelay));
        StartCoroutine(SelfDestructTimer(explosionTime));
    }

    IEnumerator ReleaseTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ReleasePests();
    }

    IEnumerator SelfDestructTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        GameObject explosion = Instantiate(m_explosionPrefab);
        Transform explosionTransform = explosion.GetComponent<Transform>();
        explosionTransform.position = this.transform.position;
        explosionTransform.localScale = explosionScale;
        Animator explosionAnimation = explosion.GetComponent<Animator>();
        float explosionAnimationLength = explosionAnimation.GetCurrentAnimatorStateInfo(0).length;
        Destroy(explosion, explosionAnimationLength);
        Destroy(this.gameObject);
    }

    public void ReleasePests()
    {
        StartCoroutine(SpawnPests());
    }

    IEnumerator SpawnPests()
    {
        yield return new WaitForSeconds(m_pestSpawnInterval);

        --m_pestCount;
        if(m_pestCount >= 0)
        {
            GameObject pest = Instantiate(m_pestPrefab);
            pest.transform.position = transform.position;
            pest.GetComponent<PestController>().Init(false, m_planet);
            StartCoroutine(SpawnPests());
        }
    }
}
