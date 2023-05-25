using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotateSelfXYZ : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float angle = Random.Range(-180, 180);
        transform.Rotate(angle, 0, 0);
        Debug.Log(transform.localToWorldMatrix);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Matrix4x4 RotateAroundX(float angle)
    {
        Matrix4x4 m = new Matrix4x4();
    }
}
