using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotateSelfXYZ : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float angle = Random.Range(-180, 180);
        transform.Rotate(0, 0, angle);
        // Debug.Log(transform.localToWorldMatrix);
        // Debug.Log(RotateAroundZ(angle));
        Debug.Log(Camera.main.projectionMatrix);
    }

    // Update is called once per frame
    void Update()
    {

    }

    Matrix4x4 RotateAroundX(float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = 1; m[0, 1] = 0; m[0, 2] = 0; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = cos; m[1, 2] = -sin; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = sin; m[2, 2] = cos; m[2, 3] = 0;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = 0; m[3, 3] = 1;
        return m;
    }

    Matrix4x4 RotateAroundY(float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = cos; m[0, 1] = 0; m[0, 2] = sin; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = 1; m[1, 2] = 0; m[1, 3] = 0;
        m[2, 0] = -sin; m[2, 1] = 0; m[2, 2] = cos; m[2, 3] = 0;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = 0; m[3, 3] = 1;
        return m;
    }

    Matrix4x4 RotateAroundZ(float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = cos; m[0, 1] = -sin; m[0, 2] = 0; m[0, 3] = 0;
        m[1, 0] = sin; m[1, 1] = cos; m[1, 2] = 0; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = 1; m[2, 3] = 0;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = 0; m[3, 3] = 1;
        return m;
    }

    Matrix4x4 ProjectionMatrix(float fov, float aspect, float zn, float zf)
    {
        // opengl projection matrix
        // https://www.cnblogs.com/wantnon/p/7248999.html
    }
}
