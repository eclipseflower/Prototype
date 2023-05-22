using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRayIntersectTriangle : MonoBehaviour
{
    public LineRenderer rayLine;
    public int rayLength = 100;
    public GameObject g1;
    public GameObject g2;
    public GameObject g3;

    public float rotationSpeed = 100f;

    private Material material;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;
        p1 = g1.transform.position;
        p2 = g2.transform.position;
        p3 = g3.transform.position;
        normal = Vector3.Cross(p2 - p1, p3 - p1);
        GenTriangle();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        rayLine.SetPosition(0, ray.origin);//SetPosition中第一个参数表示射线的发射点还是终点 0表示起始点，1表示终点
        rayLine.SetPosition(1, ray.origin + ray.direction * rayLength);

        float t = Vector3.Dot(p1 - transform.position, normal) / Vector3.Dot(transform.forward, normal);

        if(t > 0)
        {
            Vector3 p = transform.position + t * transform.forward;

            float r1 = Vector3.Dot(Vector3.Cross(p1 - p, p2 - p), normal);
            float r2 = Vector3.Dot(Vector3.Cross(p2 - p, p3 - p), normal);
            float r3 = Vector3.Dot(Vector3.Cross(p3 - p, p1 - p), normal);

            if(r1 > 0 && r2 > 0 && r3 > 0)
            {
                material.color = Color.red;
            }
            else
            {
                material.color = Color.white;
            }
        }
        else
        {
            material.color = Color.white;
        }

        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
    GameObject GenTriangle()
    {
        string name = "Triangle";

        Mesh mesh = new Mesh();
        mesh.name = name;

        Vector3[] vertices = new Vector3[3]
        {
            p1, p2, p3
        };
        mesh.vertices = vertices;

        int[] triangles = new int[3] { 0, 1, 2 };
        mesh.triangles = triangles;

        GameObject triangleGameObject = new GameObject(name);
        MeshFilter mf = triangleGameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer meshRenderer = triangleGameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        return triangleGameObject;
    }
}
