using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public GameHandler m_gameHandler;
    public GameObject m_sun;
    public float m_height;
    public float m_width;
    public float m_speed;

    public GameObject m_problemSpawner;
    public float m_spawnerRadius;
    public float m_spawnerFrontAngle;
    public float m_spawnerRearAngle;
    public float m_spawnerSpeed;

    public bool m_grabWithoutClick = false;
    public Hands m_myHands;
    public PlanetArms m_arms;
    public GameObject m_mesh;
    public float m_meshRotationSpeed;

    public bool m_clockwise;

    public int humanCount = 0;

    Vector3 m_center;
    Vector3 m_direction;
    Vector3 m_position;
    public float m_angle = 90;
    float m_spawnerAngle = 90;
    int m_spawnerDirection = 1; 

    public bool m_holdingSomething = false;

    public GameObject m_pestPrefab;

    List<float> m_pestAngles;
    public Queue<int> m_availableSlots;
    public int m_pestCount;

    // Start is called before the first frame update
    public void Init()
    {
        m_center = m_sun.transform.position;
        m_position = NextPosition3d(m_angle);
        this.gameObject.transform.position = m_position;

        if (m_clockwise)
            m_direction = Vector3.right;
        else
            m_direction = Vector3.left;

        m_position = transform.position;
        Vector3 spawnerVec;
        spawnerVec.x = m_position.x + (m_spawnerRadius * Mathf.Cos(m_spawnerAngle * Mathf.Deg2Rad));
        spawnerVec.y = m_position.y + (m_spawnerRadius * Mathf.Sin(m_spawnerAngle * Mathf.Deg2Rad));
        spawnerVec.z = m_position.z;
        m_problemSpawner.transform.position = spawnerVec;
    }

    private void Start()
    {
        SetupPestSlots();
        //for(int i = 0; i < m_pestAngles.Count; ++i)
        //{
        //    GameObject pest = Instantiate(m_pestPrefab);
        //    pest.GetComponent<PestController>().Init(true, this);
        //}
    }
     
    public void AddPest()
    {
        GameObject pest = Instantiate(m_pestPrefab);
        pest.GetComponent<PestController>().Init(true, this);
    }

    public int m_infestaitonCount = 200;
    public float m_infestInterval = 0.01f;
    float m_infestCount = 0;
    public void Infest()
    {
        m_arms.Dead();
        StartCoroutine(SpawnInfester());

    }

    IEnumerator SpawnInfester()
    {
        yield return new WaitForSeconds(m_infestInterval);
        ++m_infestCount;
       

        GameObject pest = Instantiate(m_pestPrefab, this.transform);
        pest.transform.localScale = new Vector3(2, 2, 0);
        pest.GetComponent<PestController>().Init(true, this, true);
        float pestAngle = Random.Range(0, 360);
        float pestRadius = Random.Range(0, GetRadius() * 3);

        pest.transform.localPosition = new Vector3(Mathf.Cos(pestAngle) * pestRadius, Mathf.Sin(pestAngle) * pestRadius, -3);

        if (m_infestCount < m_infestaitonCount)
            StartCoroutine(SpawnInfester());

    }

    void SetupPestSlots()
    {
        float spriteLeft = m_pestPrefab.GetComponent<SpriteRenderer>().sprite.bounds.min.x * m_pestPrefab.transform.localScale.x;
        float spriteRight = m_pestPrefab.GetComponent<SpriteRenderer>().sprite.bounds.max.x * m_pestPrefab.transform.localScale.x;

        Vector3 rightVec = new Vector3(spriteRight, GetRadius(), 0);
        Vector3 leftVec = new Vector3(spriteLeft, GetRadius(), 0);

        float pestAngle = Vector3.Angle(leftVec, rightVec);

        int maxPestCount = (int)Mathf.RoundToInt(360f / pestAngle);

        m_pestAngles = new List<float>();
        List<int> indexList = new List<int>();
        for(int i = 0; i < maxPestCount; ++i)
        {
            m_pestAngles.Add(i * pestAngle);
            indexList.Add(i);
        }

        int n = maxPestCount;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            int value = indexList[k];
            indexList[k] = indexList[n];
            indexList[n] = value;
        }
        m_availableSlots = new Queue<int>();
        for(int i = 0; i < maxPestCount; ++i)
        {
            m_availableSlots.Enqueue(indexList[i]);
        }

    }

    public float GetPestSlot()
    {
        ++m_pestCount;
        if (m_availableSlots.Count == 0)
        {
            m_gameHandler.EndGame();
            return 0;
        }

        int slotIndex = m_availableSlots.Dequeue();
        return m_pestAngles[slotIndex];// + m_pestPrefab.GetComponent<PestController>().m_moveSpeed * Time.time / Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_angle += m_speed * Time.deltaTime;
        
        m_position = NextPosition3d(m_angle);
        this.gameObject.transform.position = m_position;

        m_mesh.transform.eulerAngles = new Vector3(0, m_mesh.transform.eulerAngles.y + Time.deltaTime * m_meshRotationSpeed, 0);

        UpdateSpawner();
    }

    public Vector3 NextPosition3d(float angle) {
        Vector2 position_2d = NextPosition2d(angle);
        Vector3 position_3d = new Vector3(position_2d.x, position_2d.y, 1f);
        return position_3d;
    }

    public Vector2 NextPosition2d(float angle) {
        return new Vector2(
            m_center.x + (m_width * Mathf.Cos(angle * Mathf.Deg2Rad)),
            m_center.y + (m_height * Mathf.Sin(angle * Mathf.Deg2Rad))
        );
    }

    void UpdateSpawner()
    {
        Vector3 spawnerVec = m_problemSpawner.transform.position - m_position;
        Vector3 sunToPlanet = m_position - m_center;

        spawnerVec.Normalize();
        sunToPlanet.Normalize();

        spawnerVec.z = 0;
        sunToPlanet.z = 0;

        float angle = Vector3.SignedAngle(sunToPlanet, spawnerVec, Vector3.forward);
        if (angle <= m_spawnerFrontAngle)
            m_spawnerDirection = 1;
        else if (angle >= m_spawnerRearAngle)
            m_spawnerDirection = -1;

        m_spawnerAngle += m_spawnerDirection*m_spawnerSpeed;

        spawnerVec.x = m_position.x + (m_spawnerRadius * Mathf.Cos(m_spawnerAngle * Mathf.Deg2Rad));
        spawnerVec.y = m_position.y + (m_spawnerRadius * Mathf.Sin(m_spawnerAngle * Mathf.Deg2Rad));
        spawnerVec.z = m_position.z;
        m_problemSpawner.transform.position = spawnerVec;
    }

    public Vector3 GetHandPosition()
    {
        return m_myHands.transform.position;
    }

    public Vector3 GetHandVelocity()
    {
        return m_myHands.GetVelocity();
    }

    public void Grab(GameObject grabbedThing)
    {
        m_holdingSomething = true;
        m_myHands.Attach(grabbedThing);
    }

    public void Release()
    {
        m_holdingSomething = false;
        m_myHands.Release();
    }

    public float GetRadius()
    {
        return transform.localScale.x / 2;
    }

    private void OnDrawGizmos()
    {
        return;
        Vector3 spawnerVec = m_problemSpawner.transform.position - m_position;
        Vector3 sunToPlanet = m_position - m_center;

        spawnerVec.Normalize();
        sunToPlanet.Normalize();

        Gizmos.DrawRay(m_position, spawnerVec);
        Gizmos.DrawRay(m_position, sunToPlanet);
    }
}
