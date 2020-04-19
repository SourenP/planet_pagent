using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Camera m_camera;
    public float m_cameraSizeStart = 1f;
    public float m_cameraSizeMax = 10f;
    
    public float m_asteroidDespawnBuffer = 3;

    public float m_astroidCount = 10f;

    public GameObject m_asteroidPrefab;
    public GameObject m_shipPrefab;
    public GameObject m_orbitalProblemSpawner;
    public BackgroundHandler m_backgroundHandler;

    public PlanetController m_planet;

    public TextureGenerator m_textureGenerator;
    Texture2D m_astroidTexture;
    


    // Start is called before the first frame update
    void Start()
    {
        m_astroidTexture = m_textureGenerator.GenerateTexture(256, 256, 100f);

        for (int i = 0; i < m_astroidCount; ++i)
        { 
            SpawnAsteroid();
        }

        m_backgroundHandler.Init();
        m_planet.Init();

        //StartCoroutine(MaybeSpawnAProblem());
    }

    IEnumerator MaybeSpawnAProblem()
    {
        yield return new WaitForSeconds(5);
        int random = Random.Range(1, 100);
        
        if(random < 50)
        {
            GameObject ship = Instantiate(m_shipPrefab);
            ship.transform.position = m_orbitalProblemSpawner.transform.position;
            ship.GetComponent<ShipController>().m_planet = m_planet;
            ship.GetComponent<ShipController>().Init();
        }
        StartCoroutine(MaybeSpawnAProblem());
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void Fracture(AsteroidController target)
    {
        int count = 3;
        float angle = 2 * Mathf.PI / count;
        for (int i = 0; i < count; ++i)
        {
            GameObject ast = Instantiate(m_asteroidPrefab);
            AsteroidController astCont = ast.GetComponent<AsteroidController>();

            Vector3 pos = new Vector3(Mathf.Cos(angle * i) * target.m_mass, Mathf.Sin(angle * i) * target.m_mass, 0);

            pos = target.transform.TransformPoint(pos);

            astCont.Init(pos, target.m_mass/(count * 10f), this, AsteroidController.AstroidType.Hostile);

            Rigidbody rb = ast.GetComponent<Rigidbody>();

            //rb.AddForce(pos -target.transform.position, ForceMode.Impulse);

            astCont.SetTexture(m_astroidTexture);
        }

        Destroy(target.gameObject);
    }

    void SpawnAsteroid()
    {
        GameObject ast = Instantiate(m_asteroidPrefab);
        AsteroidController astCont = ast.GetComponent<AsteroidController>();

        RandomizeAsteroid(astCont);

        astCont.SetTexture(m_astroidTexture);
    }

    public void RandomizeAsteroid(AsteroidController ast)
    {
        float mass = 0.2f;
        AsteroidController.AstroidType astType;
        

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

        float posX, posY;

        int dice = Random.Range(-1, 1);
        if(dice == 0)
        {
            posX = Random.Range(spawnAreaOuterBL.x, spawnAreaOuterTR.x);
            dice = Random.Range(-1, 1);
            if (dice == 0)
            {
                posY = Random.Range(spawnAreaInnerTR.y, spawnAreaOuterTR.y);
            }
            else
            {
                posY = Random.Range(spawnAreaOuterBL.y, spawnAreaInnerBL.y);
            }

        }
        else
        {
            posY = Random.Range(spawnAreaOuterBL.y, spawnAreaOuterTR.y);
            dice = Random.Range(-1, 1);
            if (dice == 0)
            {
                posX = Random.Range(spawnAreaInnerTR.x, spawnAreaOuterTR.x);
            }
            else
            {
                posX = Random.Range(spawnAreaOuterBL.x, spawnAreaInnerBL.x);
            }
        }


        Debug.Log(mass);
        ast.Init(new Vector3(posX, posY, 0), mass, this, 0);        

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

        //Gizmos.DrawSphere(m_orbitalProblemSpawner.transform.position, 0.5f);
    }
}
