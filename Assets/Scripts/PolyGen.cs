using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyGen : MonoBehaviour
{
    //
    public MeshRenderer m_meshRenderer;
    public MeshFilter m_meshFilter;
    public float m_randomMin = 0.3f;
    public float m_randomMax = 1.0f;

    public float m_extendAngleDeviation = 15f;

    private Mesh m_mesh;

    private float m_radius = 0.5f;
    private int m_sections = 15;

    Vector3 m_coreVector = new Vector3(1, 0, 0);

    List<Vector3> m_verts = new List<Vector3>();

    // Use this for initialization
    public void GenerateMesh (float radius, int sections,float scaleFactor = 4)
    {
        m_verts.Clear();

        m_radius = radius;
        m_sections = sections;

        m_mesh = m_meshFilter.mesh;
        m_mesh.MarkDynamic();
        
        float angle = 2 * Mathf.PI / m_sections;

        List<int> tris = new List<int>();
        Vector3 center = Vector3.zero;
        Vector3 vert = new Vector3(m_radius, 0, 0);

        float xMul = Random.Range(m_randomMin, m_randomMax);

        vert *= xMul;

        m_verts.Add(center);
        m_verts.Add(vert);
        for (int i = 1; i < m_sections; ++i)
        {
            vert.x = Mathf.Cos(angle * i) * m_radius;
            vert.y = Mathf.Sin(angle * i) * m_radius;

            float div = Mathf.Cos(Vector3.Angle(m_coreVector, vert));

            xMul = Random.Range(m_randomMin, m_randomMax);
            vert.x *= xMul;

            m_verts.Add(vert);

            tris.Add(m_verts.Count - 2);
            tris.Add(0);
            tris.Add(m_verts.Count - 1);
        }

        tris.Add(m_verts.Count - 1);
        tris.Add(0);
        tris.Add(1);

        List<Vector2> uvs = new List<Vector2>(m_verts.Count);
        for (int index = 0; index < tris.Count; index += 3)
        {
            // Get the three vertices bounding this triangle.
            Vector3 v1 = m_verts[tris[index]];
            Vector3 v2 = m_verts[tris[index + 1]];
            Vector3 v3 = m_verts[tris[index + 2]];

            // Compute a vector perpendicular to the face.
            Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

            // Form a rotation that points the z+ axis in this perpendicular direction.
            // Multiplying by the inverse will flatten the triangle into an xy plane.
            Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

            // Assign the uvs, applying a scale factor to control the texture tiling.
            uvs.Add((Vector2)(rotation * v1) * scaleFactor);
            uvs.Add((Vector2)(rotation * v2) * scaleFactor);
            uvs.Add((Vector2)(rotation * v3) * scaleFactor);
        }                                                 

        m_mesh.SetVertices(m_verts);
        m_mesh.SetTriangles(tris, 0);
        //m_mesh.SetUVs(0, uvs);
    }

    public void ExtendNearestPoints(Vector3 point, Vector3 extDir, int extendCount, float extendAmount)
    {
        point = transform.InverseTransformPoint(point);

        if(extendCount < 3)
        {
            extendCount = 3;
        }
        else if (extendCount % 2 == 0)
        {
            ++extendCount;
        }

        List<int> extendIndices = new List<int>();

        float minDist = Mathf.Infinity;
        float dist;
        int index = 0;
        for(int i = 0; i < m_verts.Count; ++i)
        {
            dist = Vector3.Distance(point, m_verts[i]);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        extendIndices.Add(index);
        int dir = 1;
        while(extendIndices.Count < extendCount)
        {
            index += extendIndices.Count * dir;

            if (index < 0)
                index += m_verts.Count;
            else if (index > m_verts.Count)
                index -= m_verts.Count;

            extendIndices.Add(index);
            dir *= -1;
        }

       
        m_verts[extendIndices[0]] += extDir * extendAmount;
        float angle = m_extendAngleDeviation;
        int deviationSign = 1;
        Vector3 deviatedDir;

        // Cache unfriendly, observation
        for (int i = 1; i < extendIndices.Count; ++i)
        {
            deviatedDir = Quaternion.AngleAxis(angle * deviationSign, Vector3.forward) * extDir;
            m_verts[extendIndices[i]] += extDir * extendAmount;
            deviationSign *= -1;
            if (i % 2  == 0)
            {
                angle += m_extendAngleDeviation;
            }
        }



        m_mesh.SetVertices(m_verts);
    }

    public void SetTexture(Texture2D texture)
    {
        m_meshRenderer.material.mainTexture = texture;
    }
}
