using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRayIntersectTriangle : MonoBehaviour
{
    public LineRenderer rayLine;
    public int rayLength = 10;

    // Start is called before the first frame update
    void Start()
    {
        rayLine = GetComponent<LineRenderer>();
        // GenTriangle();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        rayLine.SetPosition(0, ray.origin);//SetPosition中第一个参数表示射线的发射点还是终点 0表示起始点，1表示终点
        rayLine.SetPosition(1, ray.origin + ray.direction * rayLength);
    }
    GameObject GenTriangle()
    {
        string name = "Triangle";
  
        Mesh mesh = new Mesh();
        mesh.name = name;

        Vector3[] vertices = new Vector3[3]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 10, 0),
            new Vector3(10, 0, 0)
        };        
        mesh.vertices = vertices;

        int[] triangles = new int[3] { 0, 1, 2 };
        mesh.triangles = triangles;

        GameObject triangleGameObject = new GameObject(name);
        MeshFilter mf = triangleGameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer meshRenderer = triangleGameObject.AddComponent<MeshRenderer>();
     
        return triangleGameObject;
    }
}
