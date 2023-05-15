using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPointInTriangle : MonoBehaviour
{
    public GameObject g1;
    public GameObject g2;
    public GameObject g3;
    // Start is called before the first frame update
    public Vector3 normal = -Vector3.forward;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;
    private Material material = null;
    private Vector3 targetPos;
    private bool away = true;
    public float moveSpeed = 1.0f;

    void genTarget()
    {
        if(away)
        {
            targetPos = new Vector3(Random.Range(-10, 10), Random.Range(-4, 5), 0);
        }
        else
        {
            targetPos = Vector3.zero;
        }
    }

    void Start()
    {
        p1 = g1.transform.position;
        p2 = g2.transform.position;
        p3 = g3.transform.position;

        material = GetComponent<MeshRenderer>().sharedMaterial;
        genTarget();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p = transform.position;
        float r1 = Vector3.Dot(Vector3.Cross(p2 - p, p1 - p), normal);
        float r2 = Vector3.Dot(Vector3.Cross(p3 - p, p2 - p), normal);
        float r3 = Vector3.Dot(Vector3.Cross(p1 - p, p3 - p), normal);

        if(r1 > 0 && r2 > 0 && r3 > 0)
        {
            material.color = Color.red;
        }
        else
        {
            material.color = Color.white;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Vector3 posDelta = transform.position - targetPos;
        if(posDelta.sqrMagnitude < 0.1f)
        {
            away =!away;
            genTarget();
        }
    }
}
