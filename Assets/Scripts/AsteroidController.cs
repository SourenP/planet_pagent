using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFixedUpdateParam
{
    public Vector3 m_direction = Vector3.zero;
    public Vector3 m_torque = Vector3.zero;
}

public class AsteroidController : MonoBehaviour
{
    public enum AstroidType
    {
        PlayerBaby,
        Player,
        Hostile,
        Chained,
        Rubble
    }

    private AstroidType m_type;

    public PolyGen m_polyGen;
    public GravityController m_gravityController;
    private GameHandler m_gameHandler;

    public float m_linearSpeed;
    public float m_angularSpeed;

    public float m_absorbedCount;

    public float m_sharpTurnThresholdAngle;
    public float m_sharpTurnSpeedModifier = 0.3f;
    
    public float m_pullMassThreshold = 0.7f;
    public float m_absorbMassThreshold = 0.2f;

    public Rigidbody m_rigidBody;
    public ForceMode m_forceMode;
    public float m_mass;
    public bool m_attached = false;
    public GameObject m_absorbedParticleEffect;
    public bool m_beingPulled = false;

    private AsteroidFixedUpdateParam m_param;

    public float m_health = 1f;
    public bool m_attacking = false;
    public bool m_wasHurt;

    public void SetFixedStepParam(AsteroidFixedUpdateParam param)
    {
        if (m_beingPulled)
            return;
        m_param = param;
    }

    public void BeginPull(AsteroidController other)
    {
        if (m_type != AstroidType.Rubble)
            return;

        m_beingPulled = true;
    } 

    public void SetType(AstroidType type)
    {
        m_type = type;
    }

    public void Absorbed()
    {
        GameObject effect = Instantiate(m_absorbedParticleEffect);

        effect.transform.position = transform.position;

        ParticleSystem particles = effect.GetComponent<ParticleSystem>();
        float angle = 180 - Vector3.Angle(new Vector3(1f, 0f, 0f), m_rigidBody.velocity);
        effect.transform.rotation = Quaternion.Euler(-angle, 90, 0);

        particles.Play();
        Destroy(effect, 1.1f);

        m_gameHandler.RandomizeAsteroid(this);
    }

    public void SetTexture(Texture2D texture)
    {
        m_polyGen.SetTexture(texture);
    }

    private void OnCollisionEnter(Collision collision)
    {
        AsteroidController ast = collision.gameObject.GetComponent<AsteroidController>();
        
    }

    public void Attacked()
    {
        if (m_wasHurt)
            return;

        m_wasHurt = true;
        StartCoroutine(ImmunityTimeout());

        --m_health;
        if (m_health <= 0)
            m_gameHandler.Fracture(this);
    }

    IEnumerator ImmunityTimeout()
    {
        yield return new WaitForSeconds(0.5f);
        m_wasHurt = false;
    }

    private void HandleDamage(AsteroidController other)
    {
        if (m_attacking)
            other.Attacked();
    }

    public void Init(Vector3 pos, float mass, GameHandler gameH, AstroidType type)
    {
        if (type == AstroidType.Rubble)
            m_rigidBody.isKinematic = false;
        else if (type == AstroidType.Hostile)
            m_rigidBody.isKinematic = true;

        m_absorbedCount = 0;
        SetType(type);
        m_gameHandler = gameH;
        m_mass = mass;
        m_polyGen.GenerateMesh(m_mass, 15);
        m_beingPulled = false;
        
        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.angularVelocity = Vector3.zero;

        //m_isActive = true;
        transform.position = pos;
        //m_rigidBody.mass = m_mass;
    }

    private void Update()
    {
        Vector3 asteroidPos;
        float camHeight = m_gameHandler.m_camera.orthographicSize;
        float camWidth = camHeight * 16 / 9;
        float despawnY = m_gameHandler.m_asteroidDespawnBuffer * camHeight;
        float despawnX = m_gameHandler.m_asteroidDespawnBuffer * camWidth;
        Vector3 camPos = m_gameHandler.m_camera.transform.position;

        bool respawn = false;

        asteroidPos = transform.position;

        if (asteroidPos.x < camPos.x - despawnX)
        {
            respawn = true;
        }
        else if (asteroidPos.x > camPos.x + despawnX)
        {
            respawn = true;
        }
        else if (asteroidPos.y < camPos.y - despawnY)
        {
            respawn = true;
        }
        else if (asteroidPos.y > camPos.y + despawnY)
        {
            respawn = true;
        }

        if (respawn)
            m_gameHandler.RandomizeAsteroid(this);
    }

    private void FixedUpdate()
    {
        if (m_param == null || m_beingPulled)
            return;

        if (Vector3.Angle(m_rigidBody.velocity.normalized, m_param.m_direction) > m_sharpTurnThresholdAngle)
            m_rigidBody.velocity *= m_sharpTurnSpeedModifier;

        m_rigidBody.AddForce(m_param.m_direction * m_linearSpeed * m_rigidBody.mass, m_forceMode);
        m_rigidBody.AddTorque(m_param.m_torque * m_rigidBody.mass, m_forceMode);

        if(m_param.m_torque != Vector3.zero)
        {
            m_attacking = true;
            StartCoroutine(AttackingTimeout());
        }
    }


    IEnumerator AttackingTimeout()
    {
        yield return new WaitForSeconds(5f);
        m_attacking = false;
    }
}
