using UnityEngine;
using System.Collections;

public class wave_motion : MonoBehaviour 
{
	int size 		= 100;
	float rate 		= 0.005f;
	float gamma		= 0.004f;
	float damping 	= 0.996f;
	float[,] 	old_h;
	float[,]	low_h;
	float[,]	vh;
	float[,]	b;

	bool [,]	cg_mask;
	float[,]	cg_p;
	float[,]	cg_r;
	float[,]	cg_Ap;
	bool 	tag=true;

	Vector3 	cube_v = Vector3.zero;
	Vector3 	cube_w = Vector3.zero;


	// Use this for initialization
	void Start () 
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		Vector3[] X=new Vector3[size*size];

		for (int i=0; i<size; i++)
		for (int j=0; j<size; j++) 
		{
			X[i*size+j].x=i*0.1f-size*0.05f;
			X[i*size+j].y=0;
			X[i*size+j].z=j*0.1f-size*0.05f;
		}

		int[] T = new int[(size - 1) * (size - 1) * 6];
		int index = 0;
		for (int i=0; i<size-1; i++)
		for (int j=0; j<size-1; j++)
		{
			T[index*6+0]=(i+0)*size+(j+0);
			T[index*6+1]=(i+0)*size+(j+1);
			T[index*6+2]=(i+1)*size+(j+1);
			T[index*6+3]=(i+0)*size+(j+0);
			T[index*6+4]=(i+1)*size+(j+1);
			T[index*6+5]=(i+1)*size+(j+0);
			index++;
		}
		mesh.vertices  = X;
		mesh.triangles = T;
		mesh.RecalculateNormals ();

		low_h 	= new float[size,size];
		old_h 	= new float[size,size];
		vh 	  	= new float[size,size];
		b 	  	= new float[size,size];

		cg_mask	= new bool [size,size];
		cg_p 	= new float[size,size];
		cg_r 	= new float[size,size];
		cg_Ap 	= new float[size,size];

		for (int i=0; i<size; i++)
		for (int j=0; j<size; j++) 
		{
			low_h[i,j]=99999;
			old_h[i,j]=0;
			vh[i,j]=0;
		}
	}

	void A_Times(bool[,] mask, float[,] x, float[,] Ax, int li, int ui, int lj, int uj)
	{
		for(int i=li; i<=ui; i++)
		for(int j=lj; j<=uj; j++)
		if(i>=0 && j>=0 && i<size && j<size && mask[i,j])
		{
			Ax[i,j]=0;
			if(i!=0)		Ax[i,j]-=x[i-1,j]-x[i,j];
			if(i!=size-1)	Ax[i,j]-=x[i+1,j]-x[i,j];
			if(j!=0)		Ax[i,j]-=x[i,j-1]-x[i,j];
			if(j!=size-1)	Ax[i,j]-=x[i,j+1]-x[i,j];
		}
	}

	float Dot(bool[,] mask, float[,] x, float[,] y, int li, int ui, int lj, int uj)
	{
		float ret=0;
		for(int i=li; i<=ui; i++)
		for(int j=lj; j<=uj; j++)
		if(i>=0 && j>=0 && i<size && j<size && mask[i,j])
		{
			ret+=x[i,j]*y[i,j];
		}
		return ret;
	}

	void Conjugate_Gradient(bool[,] mask, float[,] b, float[,] x, int li, int ui, int lj, int uj)
	{
		//Solve the Laplacian problem by CG.
		A_Times(mask, x, cg_r, li, ui, lj, uj);

		for(int i=li; i<=ui; i++)
		for(int j=lj; j<=uj; j++)
		if(i>=0 && j>=0 && i<size && j<size && mask[i,j])
		{
			cg_p[i,j]=cg_r[i,j]=b[i,j]-cg_r[i,j];
		}

		float rk_norm=Dot(mask, cg_r, cg_r, li, ui, lj, uj);

		for(int k=0; k<128; k++)
		{
			if(rk_norm<1e-10f)	break;
			A_Times(mask, cg_p, cg_Ap, li, ui, lj, uj);
			float alpha=rk_norm/Dot(mask, cg_p, cg_Ap, li, ui, lj, uj);

			for(int i=li; i<=ui; i++)
			for(int j=lj; j<=uj; j++)
			if(i>=0 && j>=0 && i<size && j<size && mask[i,j])
			{
				x[i,j]   +=alpha*cg_p[i,j];
				cg_r[i,j]-=alpha*cg_Ap[i,j];
			}

			float _rk_norm=Dot(mask, cg_r, cg_r, li, ui, lj, uj);
			float beta=_rk_norm/rk_norm;
			rk_norm=_rk_norm;

			for(int i=li; i<=ui; i++)
			for(int j=lj; j<=uj; j++)
			if(i>=0 && j>=0 && i<size && j<size && mask[i,j])
			{
				cg_p[i,j]=cg_r[i,j]+beta*cg_p[i,j];
			}
		}

	}

	void Block_Water_Coupling(string name, float [,] new_h)
	{
        //TODO: for block 1, calculate low_h.
		var go = GameObject.Find(name);
		var renderer = go.GetComponent<Renderer>();
		var bounds = renderer.bounds;
		var pos = go.transform.position;
		int grid_x = (int)(pos.x * 10 + size * 0.5f);
		int grid_z = (int)(pos.z * 10 + size * 0.5f);
		int li = grid_x - 6;
		int ui = grid_x + 6;
		int lj = grid_z - 6;
		int uj = grid_z + 6;

		for(int i = li; i <= ui; i++)
		{
			for(int j = lj; j <= uj; j++)
			{
				if (i >= 0 && j >= 0 && i < size && j < size)
				{
					float dist = 99999;
					bounds.IntersectRay(new Ray(new Vector3(i * 0.1f - size * 0.05f, 0, j * 0.1f - size * 0.05f),
						Vector3.up), out dist);
					low_h[i, j] = dist;
				}
			}
		}
		//TODO: then set up b and cg_mask for conjugate gradient.
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				if (low_h[i, j] <= new_h[i, j])
				{
					b[i, j] = (new_h[i, j] - low_h[i, j]) / rate;
					cg_mask[i, j] = true;
				}
				else
				{
					b[i, j] = 0;
					cg_mask[i, j] = false;
					vh[i, j] = 0;
				}
            }
		}
		//TODO: Solve the Poisson equation to obtain vh (virtual height).
		Conjugate_Gradient(cg_mask, b, vh, li, ui, lj, uj);
	}

	void Shallow_Wave(float[,] old_h, float[,] h, float [,] new_h)
	{		
		//Step 1:
		//TODO: Compute new_h based on the shallow wave model.
		for(int i = 0; i < size; i++)
		{
            for(int j = 0; j < size; j++)
			{
				new_h[i, j] = h[i, j] + damping * (h[i, j] - old_h[i, j]);
				if(i > 0) new_h[i, j] += rate * (h[i - 1, j] - h[i, j]);
				if(i < size - 1) new_h[i, j] += rate * (h[i + 1, j] - h[i, j]);
				if(j > 0) new_h[i, j] += rate * (h[i, j - 1] - h[i, j]);
				if(j < size - 1) new_h[i, j] += rate * (h[i, j + 1] - h[i, j]);
            }
        }

		//Step 2: Block->Water coupling
		Block_Water_Coupling("Cube", new_h);
		Block_Water_Coupling("Block", new_h);

		//TODO: Diminish vh.
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
                if (cg_mask[i, j])
				{
					vh[i, j] *= gamma;
                }
            }
		}

		//TODO: Update new_h by vh.
		for(int i = 0; i < size; i++)
		{
            for(int j = 0; j < size; j++)
			{
				if(i > 0) new_h[i, j] += rate * (vh[i - 1, j] - vh[i, j]);
				if(i < size - 1) new_h[i, j] += rate * (vh[i + 1, j] - vh[i, j]);
				if(j > 0) new_h[i, j] += rate * (vh[i, j - 1] - vh[i, j]);
				if(j < size - 1) new_h[i, j] += rate * (vh[i, j + 1] - vh[i, j]);
            }
        }

		//Step 3
		//TODO: old_h <- h; h <- new_h;
		for(int i = 0; i < size; i++)
		{
            for(int j = 0; j < size; j++)
			{
				old_h[i, j] = h[i, j];
				h[i, j] = new_h[i, j];
            }
        }

		//Step 4: Water->Block coupling.
		//More TODO here.
	}
	

	// Update is called once per frame
	void Update () 
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] X    = mesh.vertices;
		float[,] new_h = new float[size, size];
		float[,] h     = new float[size, size];

		//TODO: Load X.y into h.
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				h[i, j] = X[i * size + j].y;
			}
		}

		if (Input.GetKeyDown ("r")) 
		{
			//TODO: Add random water.
			int ri = Random.Range(0, size);
			int rj = Random.Range(0, size);
			float rh = Random.Range(0.1f, 1.0f);
			h[ri, rj] += rh;

			int neighbors = 0;
			if(ri > 0) neighbors++;
			if(ri < size - 1) neighbors++;
			if(rj > 0) neighbors++;
			if(rj < size - 1) neighbors++;

			rh /= neighbors;
			if(ri > 0) h[ri - 1, rj] -= rh;
			if(ri < size - 1) h[ri + 1, rj] -= rh;
			if(rj > 0) h[ri, rj - 1] -= rh;
			if(rj < size - 1) h[ri, rj + 1] -= rh;
		}
	
		for(int l=0; l<8; l++)
		{
			Shallow_Wave(old_h, h, new_h);
		}

		//TODO: Store h back into X.y and recalculate normal.
		for(int i = 0; i < size; i++)
		{
            for(int j = 0; j < size; j++)
			{
				X[i * size + j].y = h[i, j];
            }
        }
		mesh.vertices = X;
		mesh.RecalculateNormals();
	}
}
