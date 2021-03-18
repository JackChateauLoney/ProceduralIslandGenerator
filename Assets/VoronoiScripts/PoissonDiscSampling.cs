using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling : MonoBehaviour
{

	public List<Vector3> points;
	List<Vector3> grid;
	List<Vector3> active;
	Vector3 pos;

	int x;
	int z;
	int currentCol;
	int currentRow;

	


    private void Awake()
    {
		points = new List<Vector3>();
		grid = new List<Vector3>();
		active = new List<Vector3>();

		for (int i = 0; i < 50 * 50; i++)
        {
			grid.Add(new Vector3(0,0,0));
        }

        points = PoissonDiscSample(4, 60, 50, 50);

    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
		{ 
			Gizmos.color = Color.red;
            Gizmos.DrawSphere(points[i], 0.1f);
			Gizmos.color = Color.white;
			//Debug.Log("points poisson 2: " + points[i]);
		}
    }




    public List<Vector3> PoissonDiscSample(int r, int k, int width, int height)
	{
		int w = r / (int) Mathf.Sqrt(2);
		int cols = (int) Mathf.Floor(width / w);
		int rows = (int) Mathf.Floor(height / w);

		//x = width / 2;
		//z = height / 2;
		x = 0;
		z = 0;
		currentRow = (int) Mathf.Floor(x / w);
		currentCol = (int) Mathf.Floor(z / w);
		pos = new Vector3(x + transform.position.x,0, z + transform.position.z);
		grid[currentRow + currentCol * cols] = pos;
		active.Add(pos);



		while (active.Count > 0)
		{
			int randIndex = (int) Mathf.Floor(Random.Range(0, active.Count - 1));
			Vector3 aPos = active[randIndex];
			bool found = false;
			for (int n = 0; n < k; n++)
			{
				//get random unit vector
				Vector3 sample;
				sample = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
				sample = sample.normalized;
				//set magnitude of vector to be between r and 2r
				float m = Random.Range(r, r * 2);
				float mag = Mathf.Sqrt(sample.x * sample.x + sample.z * sample.z);
				sample.x = sample.x * m / mag;
				sample.z = sample.z * m / mag;
				sample += aPos;


				int colPos = (int) Mathf.Floor(sample.x / w);
				int rowPos = (int) Mathf.Floor(sample.z / w);




				//check neighbours, make sure they're not null
				if (colPos > 0 && rowPos > 0 && colPos < cols && rowPos < rows && colPos + rowPos * cols < grid.Count)
				{
					bool ok = true;
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							int index = (colPos + i) + (rowPos + j) * cols;
							if (index < grid.Count)
							{
								Vector3 neighbour = grid[index];
								if (neighbour.x != 0 && neighbour.z != 0)
								{
									float d = Vector3.Distance(sample, neighbour);

									if (d < r)
										ok = false;
								}
							}
						}
					}

					//point is far enough away from other point
					if (ok)
					{
						found = true;
						if (colPos + rowPos * cols < grid.Count)
						{
							grid[colPos + rowPos * cols] = sample;
							active.Add(sample);
						}
						break;

					}
				}




			}

			if (!found)
			{
				active.RemoveAt(randIndex);
			}
		}

		return grid;
	}
}
