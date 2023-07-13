using UnityEngine;
using System.Collections;

public class Rigid_Bunny : MonoBehaviour 
{
	bool launched 		= false;
	float dt 			= 0.015f;
	Vector3 v 			= new Vector3(0, 0, 0);	// velocity
	Vector3 w 			= new Vector3(0, 0, 0);	// angular velocity
	
	float mass;									// mass
	Matrix4x4 I_ref;							// reference inertia

	float linear_decay	= 0.999f;				// for velocity decay
	float angular_decay	= 0.98f;				
	float restitution 	= 0.5f;                 // for collision

	Vector3 gravity;

	// Use this for initialization
	void Start () 
	{		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;

		float m=1;
		mass=0;
		for (int i=0; i<vertices.Length; i++) 
		{
			mass += m;
			float diag=m*vertices[i].sqrMagnitude;
			I_ref[0, 0]+=diag;
			I_ref[1, 1]+=diag;
			I_ref[2, 2]+=diag;
			I_ref[0, 0]-=m*vertices[i][0]*vertices[i][0];
			I_ref[0, 1]-=m*vertices[i][0]*vertices[i][1];
			I_ref[0, 2]-=m*vertices[i][0]*vertices[i][2];
			I_ref[1, 0]-=m*vertices[i][1]*vertices[i][0];
			I_ref[1, 1]-=m*vertices[i][1]*vertices[i][1];
			I_ref[1, 2]-=m*vertices[i][1]*vertices[i][2];
			I_ref[2, 0]-=m*vertices[i][2]*vertices[i][0];
			I_ref[2, 1]-=m*vertices[i][2]*vertices[i][1];
			I_ref[2, 2]-=m*vertices[i][2]*vertices[i][2];
		}
		I_ref [3, 3] = 1;

		gravity = new Vector3(0, mass * -9.8f, 0);
	}
	
	Matrix4x4 Get_Cross_Matrix(Vector3 a)
	{
		//Get the cross product matrix of vector a
		Matrix4x4 A = Matrix4x4.zero;
		A [0, 0] = 0; 
		A [0, 1] = -a [2]; 
		A [0, 2] = a [1]; 
		A [1, 0] = a [2]; 
		A [1, 1] = 0; 
		A [1, 2] = -a [0]; 
		A [2, 0] = -a [1]; 
		A [2, 1] = a [0]; 
		A [2, 2] = 0; 
		A [3, 3] = 1;
		return A;
	}

	// In this function, update v and w by the impulse due to the collision with
	//a plane <P, N>
	void Collision_Impulse(Vector3 P, Vector3 N)
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;

        Vector3 avg = Vector3.zero;
        int num = 0;
        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vert = transform.TransformPoint(vertices[i]);
            if(Vector3.Dot(vert - P, N) < 0)
            {
                avg += vert;
                num++;
            }
        }

        if(num == 0)
        {
            return;
        }

        avg /= num;

        Vector3 rri = avg - transform.position;
        Vector3 vi = v + Vector3.Cross(w, rri);
        float dot = Vector3.Dot(vi, N);
        if(dot >= 0)
        {
            return;
        }

        Vector3 vni = dot * N;
        Vector3 vti = vi - vni;
        float a = Mathf.Max(1 - restitution * (1 + restitution) * Vector3.Magnitude(vni) / Vector3.Magnitude(vti), 0);
        vni = -restitution * vni;
        vti = a * vti;
        Vector3 vinew = vni + vti;

        Matrix4x4 inv = Matrix4x4.Inverse(I_ref);
        Matrix4x4 k1 = Matrix4x4.identity;
		for(int i = 0; i < 4; i++)
		{
			k1[i, i] = 1 / mass;
		}
        Matrix4x4 k2 = Get_Cross_Matrix(rri) * inv * Get_Cross_Matrix(rri);
        Matrix4x4 k = new Matrix4x4();
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                k[i, j] = k1[i, j] - k2[i, j];
            }
        }

        Vector3 J = Matrix4x4.Inverse(k) * (vinew - vi);
        v = v + J / mass;
        Vector3 tmp1 = Vector3.Cross(rri, J);
        Vector4 tmp2 = new Vector4(tmp1.x, tmp1.y, tmp1.z, 0);
        Vector3 tmp3 = inv * tmp2;
        w = w + new Vector3(tmp3.x, tmp3.y, tmp3.z);
	}

	void Update_Velocity(out Vector3 v1, Vector3 v0, Vector3 f, float delta)
	{
		Vector3 a = f / mass * delta;
		v1 = linear_decay * (v0 + a * delta);
	}

    void Update_Angular_Velocity(out Vector3 w1, Vector3 w0)
    {
        w1 = angular_decay * w0;
    }

    void Update_Position(out Vector3 x1, Vector3 x0, Vector3 v01, float delta)
    {
        x1 = x0 + v01 * delta;
    }

    void Update_Rotation(out Quaternion q1, Quaternion q0, Vector3 w01, float delta)
    {
		Quaternion tmp = new Quaternion(w01.x * delta * 0.5f, w01.y * delta * 0.5f, w01.z * delta * 0.5f, 0) * q0;
		q1 = new Quaternion(q0.x + tmp.x, q0.y + tmp.y, q0.z + tmp.z, q0.w + tmp.w);
    }

	// Update is called once per frame
	void Update () 
	{
		//Game Control
		if(Input.GetKey("r"))
		{
			transform.position = new Vector3 (0, 0.6f, 0);
			restitution = 0.5f;
			launched=false;
		}
		if(Input.GetKey("l"))
		{
			//v = new Vector3 (5, 2, 0);
			//w = new Vector3 (5, 2, 0);
			launched=true;
		}

		// Part I: Update velocities
		if (launched)
		{
			Update_Velocity(out v, v, gravity, dt);
            Update_Angular_Velocity(out w, w);
		}

		// Part II: Collision Impulse
		Collision_Impulse(new Vector3(0, 0.01f, 0), new Vector3(0, 1, 0));
		//Collision_Impulse(new Vector3(2, 0, 0), new Vector3(-1, 0, 0));

		// Part III: Update position & orientation
		//Update linear status
		Vector3 x = transform.position;
        if (launched)
        {
            Update_Position(out x, x, v, dt);
        }
		//Update angular status
		Quaternion q = transform.rotation;
		if(launched)
		{
			Update_Rotation(out q, q, w, dt);
		}

		// Part IV: Assign to the object
		transform.position = x;
		transform.rotation = q;
	}
}
