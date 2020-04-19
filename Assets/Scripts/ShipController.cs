using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipController : ProblemBase
{
    public PlanetController m_planet;

    public float m_speed = 0.2f;
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
        Debug.Log("t1 " + t1);
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
        Debug.Log("intersect_t " + intersect_t);

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
        m_direction = getShipDirection(
            m_planet.m_angle,
            m_planet.m_speed,
            new Vector2(transform.position.x, transform.position.y),
            m_speed
        );
        Debug.Log("direction " + m_direction);
    }

    public override void Init(GameHandler gameHandler, PlanetController planet)
    {
        base.Init(gameHandler, planet);
        m_direction = getShipDirection(
            m_planet.m_angle,
            m_planet.m_speed,
            new Vector2(transform.position.x, transform.position.y),
            m_speed
        );
        Debug.Log("direction " + m_direction);
        //transform.forward = new Vector3(m_direction.x, 0, m_direction.y);

    }

    // Update is called once per frame
    void Update()
    {
        // m_direction = getShipDirection(
        //     m_planet.m_angle,
        //     m_planet.m_speed,
        //     new Vector2(transform.position.x, transform.position.y),
        //     m_speed
        // );
        Debug.Log("position " + transform.position);
        transform.position += m_direction * m_speed * Time.deltaTime;
    }


}
