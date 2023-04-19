using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane
{
    public Vector3 center;
    public Vector3 normal;
}

public class TestPointInBox : MonoBehaviour
{
    public Plane[] planes = new Plane[6];
    public GameObject sphere = null;
    public Material material = null;
    // Start is called before the first frame update

    void Initialize()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;
        sphere = GameObject.Find("Sphere");

        for(int i = 0; i < 6; i++)
        {
            planes[i] = new Plane();
        }
    }

    void ConstructPlane()
    {
        var pos = transform.position;
        var rot = transform.rotation;
        var scale = transform.lossyScale;

        var posMat = Matrix4x4.Translate(pos);
        var rotMat = Matrix4x4.Rotate(rot);
        var mat = posMat * rotMat;

        float x = scale[0] * 0.5f;
        float y = scale[1] * 0.5f;
        float z = scale[2] * 0.5f;

        planes[0].center = new Vector3(-x, 0, 0);
        planes[1].center = new Vector3(+x, 0, 0);
        planes[2].center = new Vector3(0, -y, 0);
        planes[3].center = new Vector3(0, +y, 0);
        planes[4].center = new Vector3(0, 0, -z);
        planes[5].center = new Vector3(0, 0, +z);

        planes[0].normal = new Vector3(+1, 0, 0);
        planes[1].normal = new Vector3(-1, 0, 0);
        planes[2].normal = new Vector3(0, +1, 0);
        planes[3].normal = new Vector3(0, -1, 0);
        planes[4].normal = new Vector3(0, 0, +1);
        planes[5].normal = new Vector3(0, 0, -1);

        for(int i = 0; i < 6; i++)
        {
            planes[i].center = mat * new Vector4(planes[i].center.x, planes[i].center.y, planes[i].center.z, 1.0f);
            planes[i].normal = Vector3.Normalize(mat * planes[i].normal);
        }
    }

    void Start()
    {
        Initialize();
        ConstructPlane();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = sphere.transform.position;
        ConstructPlane();

        bool hit = true;
        for(int i = 0; i < 6; i++) 
        {
            var res = Vector3.Dot(pos - planes[i].center, planes[i].normal);
            if(res <= 0)
            {
                hit = false; 
                break;
            }
        }

        if(hit) 
        {
            material.color = Color.red;
        }
        else
        {
            material.color = Color.white;
        }
    }
}
