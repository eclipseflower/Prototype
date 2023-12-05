using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FVM : MonoBehaviour
{
	float dt 			= 0.003f;
    float mass 			= 1;
	float stiffness_0	= 20000.0f;
    float stiffness_1 	= 5000.0f;
    float damp			= 0.999f;

	int[] 		Tet;
	int tet_number;			//The number of tetrahedra

	Vector3[] 	Force;
	Vector3[] 	V;
	Vector3[] 	X;
	int number;				//The number of vertices

	Matrix4x4[] inv_Dm;

	//For Laplacian smoothing.
	Vector3[]   V_sum;
	int[]		V_num;

	SVD svd = new SVD();

    public bool bonus = false;

    // Start is called before the first frame update
    void Start()
    {
    	// FILO IO: Read the house model from files.
    	// The model is from Jonathan Schewchuk's Stellar lib.
    	{
    		string fileContent = File.ReadAllText("Assets/house2.ele");
    		string[] Strings = fileContent.Split(new char[]{' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
    		
    		tet_number=int.Parse(Strings[0]);
        	Tet = new int[tet_number*4];

    		for(int tet=0; tet<tet_number; tet++)
    		{
				Tet[tet*4+0]=int.Parse(Strings[tet*5+4])-1;
				Tet[tet*4+1]=int.Parse(Strings[tet*5+5])-1;
				Tet[tet*4+2]=int.Parse(Strings[tet*5+6])-1;
				Tet[tet*4+3]=int.Parse(Strings[tet*5+7])-1;
			}
    	}
    	{
			string fileContent = File.ReadAllText("Assets/house2.node");
    		string[] Strings = fileContent.Split(new char[]{' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
    		number = int.Parse(Strings[0]);
    		X = new Vector3[number];
       		for(int i=0; i<number; i++)
       		{
       			X[i].x=float.Parse(Strings[i*5+5])*0.4f;
       			X[i].y=float.Parse(Strings[i*5+6])*0.4f;
       			X[i].z=float.Parse(Strings[i*5+7])*0.4f;
       		}
    		//Centralize the model.
	    	Vector3 center=Vector3.zero;
	    	for(int i=0; i<number; i++)		center+=X[i];
	    	center=center/number;
	    	for(int i=0; i<number; i++)
	    	{
	    		X[i]-=center;
	    		float temp=X[i].y;
	    		X[i].y=X[i].z;
	    		X[i].z=temp;
	    	}
		}
        /*tet_number=1;
        Tet = new int[tet_number*4];
        Tet[0]=0;
        Tet[1]=1;
        Tet[2]=2;
        Tet[3]=3;

        number=4;
        X = new Vector3[number];
        V = new Vector3[number];
        Force = new Vector3[number];
        X[0]= new Vector3(0, 0, 0);
        X[1]= new Vector3(1, 0, 0);
        X[2]= new Vector3(0, 1, 0);
        X[3]= new Vector3(0, 0, 1);*/


        //Create triangle mesh.
       	Vector3[] vertices = new Vector3[tet_number*12];
        int vertex_number=0;
        for(int tet=0; tet<tet_number; tet++)
        {
        	vertices[vertex_number++]=X[Tet[tet*4+0]];
        	vertices[vertex_number++]=X[Tet[tet*4+2]];
        	vertices[vertex_number++]=X[Tet[tet*4+1]];

        	vertices[vertex_number++]=X[Tet[tet*4+0]];
        	vertices[vertex_number++]=X[Tet[tet*4+3]];
        	vertices[vertex_number++]=X[Tet[tet*4+2]];

        	vertices[vertex_number++]=X[Tet[tet*4+0]];
        	vertices[vertex_number++]=X[Tet[tet*4+1]];
        	vertices[vertex_number++]=X[Tet[tet*4+3]];

        	vertices[vertex_number++]=X[Tet[tet*4+1]];
        	vertices[vertex_number++]=X[Tet[tet*4+2]];
        	vertices[vertex_number++]=X[Tet[tet*4+3]];
        }

        int[] triangles = new int[tet_number*12];
        for(int t=0; t<tet_number*4; t++)
        {
        	triangles[t*3+0]=t*3+0;
        	triangles[t*3+1]=t*3+1;
        	triangles[t*3+2]=t*3+2;
        }
        Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.vertices  = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();


		V 	  = new Vector3[number];
        Force = new Vector3[number];
        V_sum = new Vector3[number];
        V_num = new int[number];

		//TODO: Need to allocate and assign inv_Dm
        inv_Dm = new Matrix4x4[tet_number];
        for(int i = 0; i < tet_number; i++)
        {
            Matrix4x4 Dm = Build_Edge_Matrix(i);
            inv_Dm[i] = Dm.inverse;
        }
    }

    Matrix4x4 Build_Edge_Matrix(int tet)
    {
    	Matrix4x4 ret=Matrix4x4.zero;
    	//TODO: Need to build edge matrix here.
        Vector4 e1 = X[Tet[tet * 4 + 1]] - X[Tet[tet * 4 + 0]];
        Vector4 e2 = X[Tet[tet * 4 + 2]] - X[Tet[tet * 4 + 0]];
        Vector4 e3 = X[Tet[tet * 4 + 3]] - X[Tet[tet * 4 + 0]];
        ret.SetColumn(0, e1);
        ret.SetColumn(1, e2);
        ret.SetColumn(2, e3);
        ret.SetColumn(3, new Vector4(0, 0, 0, 1));
        return ret;
    }

    Matrix4x4 Matrix_Add(Matrix4x4 a, Matrix4x4 b)
    {
    	Matrix4x4 ret = Matrix4x4.zero;
        for(int i = 0; i < 4; i++)
        {
            for(int j =0; j < 4; j++)
            {
                ret[i, j] = a[i, j] + b[i, j];
            }
        }
        return ret;
    }

    Matrix4x4 Matrix_Sub(Matrix4x4 a, Matrix4x4 b)
    {
    	Matrix4x4 ret = Matrix4x4.zero;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                ret[i, j] = a[i, j] - b[i, j];
            }
        }
        return ret;
    }

    Matrix4x4 Matrix_Mul(float s, Matrix4x4 m)
    {
        Matrix4x4 ret = Matrix4x4.zero;
        for(int i = 0; i < 4; i++)
        {
            for(int j =0; j < 4; j++)
            {
                ret[i, j] = s * m[i, j];
            }
        }
        return ret;
    }

    float Matrix_Trace(Matrix4x4 m)
    {
        float ret = 0.0f;
        for(int i = 0; i < 4; i++)
        {
            ret += m[i, i];
        }
        return ret;
    }

    void Laplacian_Smoothing(float blend)
    {
        for (int i = 0; i < number; i++)
        {
            V_sum[i] = Vector3.zero;
            V_num[i] = 0;
        }

    	for(int tet=0; tet<tet_number; tet++)
        {
            Vector3 sum = V[Tet[tet * 4 + 0]] + V[Tet[tet * 4 + 1]] + V[Tet[tet * 4 + 2]] + V[Tet[tet * 4 + 3]];
            V_sum[Tet[tet * 4 + 0]] += sum;
            V_sum[Tet[tet * 4 + 1]] += sum;
            V_sum[Tet[tet * 4 + 2]] += sum;
            V_sum[Tet[tet * 4 + 3]] += sum;
            V_num[Tet[tet * 4 + 0]] += 4;
            V_num[Tet[tet * 4 + 1]] += 4;
            V_num[Tet[tet * 4 + 2]] += 4;
            V_num[Tet[tet * 4 + 3]] += 4;
        }

        for(int i = 0; i < number; i++)
        {
            V[i] = (V[i] + blend * V_sum[i] / V_num[i]) / (1 + blend);
        }
    }

    void _Update()
    {
    	// Jump up.
		if(Input.GetKeyDown(KeyCode.Space))
    	{
    		for(int i=0; i<number; i++)
    			V[i].y+=0.2f;
    	}

    	for(int i=0 ;i<number; i++)
    	{
            //TODO: Add gravity to Force.
            Force[i] = new Vector3(0, -9.8f * mass, 0);
    	}

    	for(int tet=0; tet<tet_number; tet++)
    	{
    		//TODO: Deformation Gradient
            Matrix4x4 m = Build_Edge_Matrix(tet);
            Matrix4x4 F = m * inv_Dm[tet];

            Vector3 f1 = Vector3.zero;
            Vector3 f2 = Vector3.zero;
            Vector3 f3 = Vector3.zero;

            if(bonus)
            {
                Matrix4x4 U = Matrix4x4.zero;
                Matrix4x4 S = Matrix4x4.zero;
                Matrix4x4 V = Matrix4x4.zero;

                svd.svd(F, ref U, ref S, ref V);

                float lambda0 = S[0, 0];
                float lambda1 = S[1, 1];
                float lambda2 = S[2, 2];

                float Ic = lambda0 * lambda0 + lambda1 * lambda1 + lambda2 * lambda2;

                float dWdIc = 0.25f * stiffness_0 * (Ic - 3f) - 0.5f * stiffness_1;
                float dWdIIc = 0.25f * stiffness_1;
                float dIcdlambda0 = 2f * lambda0;
                float dIcdlambda1 = 2f * lambda1;
                float dIcdlambda2 = 2f * lambda2;
                float dIIcdlambda0 = 4f * lambda0 * lambda0 * lambda0;
                float dIIcdlambda1 = 4f * lambda1 * lambda1 * lambda1;
                float dIIcdlambda2 = 4f * lambda2 * lambda2 * lambda2;
                float dWd0 = dWdIc * dIcdlambda0 + dWdIIc * dIIcdlambda0;
                float dWd1 = dWdIc * dIcdlambda1 + dWdIIc * dIIcdlambda1;
                float dWd2 = dWdIc * dIcdlambda2 + dWdIIc * dIIcdlambda2;

                Matrix4x4 diag = Matrix4x4.zero;
                diag[0, 0] = dWd0;
                diag[1, 1] = dWd1;
                diag[2, 2] = dWd2;
                diag[3, 3] = 1;
                Matrix4x4 P = U * diag * V.transpose;

                Matrix4x4 fm = Matrix_Mul(-1.0f / (6.0f * inv_Dm[tet].determinant),
                    P * inv_Dm[tet].transpose);
                f1 = fm.GetColumn(0);
                f2 = fm.GetColumn(1);
                f3 = fm.GetColumn(2);

            }
            else
            {
                //TODO: Green Strain
                Matrix4x4 G = Matrix_Mul(0.5f, Matrix_Sub(F.transpose * F, Matrix4x4.identity));

                //TODO: Second PK Stress
                Matrix4x4 S = Matrix_Add(Matrix_Mul(2.0f * stiffness_1, G),
                    Matrix_Mul(stiffness_0 * Matrix_Trace(G), Matrix4x4.identity));

                //TODO: Elastic Force
                Matrix4x4 fm = Matrix_Mul(-1.0f / (6.0f * inv_Dm[tet].determinant),
                    F * S * inv_Dm[tet].transpose);
                f1 = fm.GetColumn(0);
                f2 = fm.GetColumn(1);
                f3 = fm.GetColumn(2);
            }

            Force[Tet[tet * 4 + 0]] -= f1 + f2 + f3;
            Force[Tet[tet * 4 + 1]] += f1;
            Force[Tet[tet * 4 + 2]] += f2;
            Force[Tet[tet * 4 + 3]] += f3;
    	}

        Laplacian_Smoothing(0.1f);

    	for(int i=0; i<number; i++)
    	{
    		//TODO: Update X and V here.
            V[i] += Force[i] * dt / mass;
            V[i] *= damp;
            X[i] += V[i]*dt;

            //TODO: (Particle) collision with floor.
            if (X[i].y < -3f)
            {
                V[i].y += (-3f - X[i].y) / dt;
                X[i].y = -3f;
            }
    	}
    }

    // Update is called once per frame
    void Update()
    {
    	for(int l=0; l<10; l++)
    		 _Update();

    	// Dump the vertex array for rendering.
    	Vector3[] vertices = new Vector3[tet_number*12];
        int vertex_number=0;
        for(int tet=0; tet<tet_number; tet++)
        {
        	vertices[vertex_number++]=X[Tet[tet*4+0]];
        	vertices[vertex_number++]=X[Tet[tet*4+2]];
        	vertices[vertex_number++]=X[Tet[tet*4+1]];
        	vertices[vertex_number++]=X[Tet[tet*4+0]];
        	vertices[vertex_number++]=X[Tet[tet*4+3]];
        	vertices[vertex_number++]=X[Tet[tet*4+2]];
        	vertices[vertex_number++]=X[Tet[tet*4+0]];
        	vertices[vertex_number++]=X[Tet[tet*4+1]];
        	vertices[vertex_number++]=X[Tet[tet*4+3]];
        	vertices[vertex_number++]=X[Tet[tet*4+1]];
        	vertices[vertex_number++]=X[Tet[tet*4+2]];
        	vertices[vertex_number++]=X[Tet[tet*4+3]];
        }
        Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.vertices  = vertices;
		mesh.RecalculateNormals ();
    }
}
