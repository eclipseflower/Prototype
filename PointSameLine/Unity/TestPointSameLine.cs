using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPointSameLine : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject g1;
    public GameObject g2;
    public GameObject g3;

    private Material[] mats = new Material[3];
    private float speed1 = 1.0f;
    private float speed2 = 1.0f;
    private Vector3 axis = Vector3.up;
    private float hitTime = 0.0f;
    void Start()
    {
        mats[0] = g1.GetComponent<MeshRenderer>().sharedMaterial;
        mats[1] = g2.GetComponent<MeshRenderer>().sharedMaterial;
        mats[2] = g3.GetComponent<MeshRenderer>().sharedMaterial;
        Generate();
    }

    void Generate()
    {
        Vector3 r = g2.transform.position;

        speed1 = Random.Range(10f, 100f);
        speed2 = Random.Range(10f, 100f);

        if(Mathf.Abs(r.x) > Mathf.Abs(r.y))
        {
            if(Mathf.Abs(r.y) > Mathf.Abs(r.z))
            {
                axis = new Vector3(-r.y, r.x, 0);
            }
            else
            {
                axis = new Vector3(-r.z, 0, r.x);
            }
        }
        else
        {
            if(Mathf.Abs(r.x) > Mathf.Abs(r.z))
            {
                axis = new Vector3(r.y, -r.x, 0);
            }
            else
            {
                axis = new Vector3(0, -r.z, r.y);
            }
        }
    }

    void Rotate()
    {
        g2.transform.RotateAround(g1.transform.position, axis, speed1 * Time.deltaTime);
        g3.transform.RotateAround(g1.transform.position, axis, speed2 * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Vector3 p1 = g1.transform.position;
        Vector3 p2 = g2.transform.position;
        Vector3 p3 = g3.transform.position;

        Vector3 l1 = p2 - p1;
        Vector3 l2 = p3 - p1;

        Vector3 res = Vector3.Cross(l1, l2);
        if(res.sqrMagnitude < 0.1f)
        {
            for(int i = 0; i < 2; i++)
            {
                mats[i].color = Color.red;
            }
            hitTime = Time.time;
            Generate();
        }
        else
        {
            if(Mathf.Abs(Time.time - hitTime) < 0.5f)
            {
                for(int i = 0; i < 2; i++)
                {
                    mats[i].color = Color.red;
                }
            }
            else
            {
                for(int i = 0; i < 2; i++)
                {
                    mats[i].color = Color.white;
                }
            }
        }
    }
}
