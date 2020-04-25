using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTemplateProjects
{
    public class MyRandom
    {
        // Randomly select an index of a non-empty array of weights. Selects an
        // index with probability proportional to the weight at that index.
        static public int selectFromWeights(float[] weights)
        {
            int len = weights.Length;
            if (len == 0)
            {
                Debug.Log("Warning: `selectFromWeights` called with empty array");
                return 0;
            }

            // Find cumulative weights
            float[] cumulative_weights = new float[len];
            float cumulative_weight = 0.0f;
            for (int i = 0; i < len; i++)
            {
                cumulative_weight += weights[i];
                cumulative_weights[i] = cumulative_weight;
            }

            // Roll
            float r = Random.Range(0.0f, cumulative_weight);
            for (int i = 0; i < len; i++)
            {
                if (r <= cumulative_weights[i])
                {
                    return i;
                }
            }

            // This should never be reached
            Debug.Log("Something bad happened in function `selectFromWeights`");
            return 0;
        }
    }
}

public class GameHandler : MonoBehaviour
{
    public Text m_gameOver;

    public Camera m_camera;
    public float m_cameraSizeStart = 1f;
    public float m_cameraSizeMax = 10f;

    public float m_defaultSpawnTime = 3f;
    public float m_spawnTimeMin = 1.5f;
    public float m_spawnModifier = 0.1f;
    public float m_spawnTime;
    public float m_asteroidDespawnBuffer = 3;

    public float m_astroidCount = 10f;

    public GameObject m_asteroidPrefab;
    public GameObject m_shipPrefab;
    public GameObject m_orbitalProblemSpawner;
    public float m_astroidSpawnerDeviation = 0.1f;
    public float m_astroidSpawnerAngleDeviation = 30f;
    float m_currentAstroidSpawnerAngle = 0;
    public GameObject m_astroidSpawnerInner;
    public GameObject m_astroidSpawnerOuter;
    public BackgroundHandler m_backgroundHandler;

    public PlanetController m_planet;

    public TextureGenerator m_textureGenerator;
    Texture2D m_astroidTexture;

    bool m_gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        m_spawnTime = m_defaultSpawnTime;
        m_astroidTexture = m_textureGenerator.GenerateTexture(256, 256, 100f);

        m_planet.Init();
        m_currentAstroidSpawnerAngle = m_planet.m_angle + m_astroidSpawnerAngleDeviation;

        StartCoroutine(MaybeSpawnAProblem());
    }

    IEnumerator MaybeSpawnAProblem()
    {
        yield return new WaitForSeconds(m_spawnTime);
        if (m_gameStarted)
        {
            float[] weights = { .25f, 0.75f, 0.75f };
            int type = UnityTemplateProjects.MyRandom.selectFromWeights(weights);

            if (type == (int)PlanetProblem.ProblemType.Asteroid)
            {
                SpawnAsteroid();
            }
            else
            {
                SpawnShip((PlanetProblem.ProblemType)type);
            }
        }

        StartCoroutine(MaybeSpawnAProblem());
    }

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

    // Update is called once per frame
    void Update()
    {
        {
            m_currentAstroidSpawnerAngle += m_planet.m_speed * Time.deltaTime;
            Vector3 position = m_planet.NextPosition3d(m_currentAstroidSpawnerAngle);

            Vector3 sunToPlanet = (position - m_planet.m_sun.transform.position).normalized;

            Vector3 idontcareanymore = sunToPlanet * m_astroidSpawnerDeviation; 

            m_astroidSpawnerInner.transform.position = position - idontcareanymore;
            m_astroidSpawnerOuter.transform.position = position + idontcareanymore;
        }
        m_spawnTime = m_defaultSpawnTime - (Time.time * m_spawnModifier);
        m_spawnTime = Mathf.Clamp(m_spawnTime, m_spawnTimeMin, m_defaultSpawnTime);
    }

    public void StartGame()
    {
        Camera.main.transform.parent = m_planet.transform;
        Camera.main.transform.localPosition = Vector3.zero + Vector3.forward * -5;
        Camera.main.orthographicSize = 1;


        m_gameStarted = true;
    }

    public void EndGame()
    {
        if (m_gameOver.gameObject.activeSelf)
            return;

        m_gameOver.enabled = true;
        m_gameOver.gameObject.SetActive(true);
        m_gameStarted = false;
        m_planet.Infest();
    }

    public void Fracture(AsteroidController target)
    {
        int count = 3;
        float angle = 2 * Mathf.PI / count;
        for (int i = 0; i < count; ++i)
        {
            GameObject ast = Instantiate(m_asteroidPrefab);
            AsteroidController astCont = ast.GetComponent<AsteroidController>();


            Rigidbody rb = ast.GetComponent<Rigidbody>();

            //rb.AddForce(pos -target.transform.position, ForceMode.Impulse);

            astCont.SetTexture(m_astroidTexture);
        }

        Destroy(target.gameObject);
    }

    void SpawnAsteroid()
    {
        int random = Random.Range(1, 100);

        GameObject ast = Instantiate(m_asteroidPrefab);
        if (random < 50)
        {
            ast.transform.position = m_astroidSpawnerInner.transform.position;
        }
        else
        {
            ast.transform.position = m_astroidSpawnerOuter.transform.position;
        }
        ast.GetComponent<ProblemBase>().Init(this, m_planet, PlanetProblem.ProblemType.Asteroid);
    }

    void SpawnShip(PlanetProblem.ProblemType type)
    {
        GameObject ship = Instantiate(m_shipPrefab);
        ship.transform.position = m_orbitalProblemSpawner.transform.position;
        ship.GetComponent<ProblemBase>().Init(this, m_planet, type);
    }
    
    private void OnDrawGizmos()
    {
        float camHeight = m_camera.orthographicSize;
        float camWidth = camHeight * 16 / 9;
        float despawnY = m_asteroidDespawnBuffer * camHeight;
        float despawnX = m_asteroidDespawnBuffer * camWidth;
        Vector3 camPos = m_camera.transform.position;

        Vector3 camSize = new Vector3(camWidth, camHeight);

        Vector3 spawnAreaInnerTR = camPos + camSize;
        Vector3 spawnAreaInnerBL = camPos - camSize;

        Vector3 spawnAreaOuterTR = camPos + 2f * camSize;
        Vector3 spawnAreaOuterBL = camPos - 2f * camSize;


        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(camPos, spawnAreaInnerTR - spawnAreaInnerBL);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(camPos, spawnAreaOuterTR - spawnAreaOuterBL);

        
        Gizmos.DrawSphere(m_orbitalProblemSpawner.transform.position, 0.2f);
        Gizmos.DrawSphere(m_astroidSpawnerInner.transform.position, 0.1f);
        Gizmos.DrawSphere(m_astroidSpawnerOuter.transform.position, 0.1f);
    }
}
