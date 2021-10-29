using System;
using UnityEngine;

public class MCMath
{
	public static float Determinant(float[][] det)
	{
		if (det.Length == 0) return 0;
		if (det.Length == 1) return det[0][0];
		if (det.Length == 2)
			return det[0][0] * det[1][1] - det[0][1] * det[1][0];


		float dSum = 0, dSign = 1;
		for (int i = 0; i < det.Length; i++)
		{
			float[][] matrixTemp = new float[det.Length - 1][];
			for (int count = 0; count < det.Length - 1; count++)
			{
				matrixTemp[count] = new float[det.Length - 1];
			}

			for (int j = 0; j < matrixTemp.Length; j++)
			{
				for (int k = 0; k < matrixTemp.Length; k++)
				{
					matrixTemp[j][k] = det[j + 1][k >= i ? k + 1 : k];
				}
			}

			dSum += det[0][i] * dSign * Determinant(matrixTemp);
			dSign = dSign * -1;
		}

		return dSum;
	}

	public static float GetPositionOrientationToEdge(float[] v, float[][] edge)
	{
		float x1 = edge[0][0];
		float y1 = edge[0][1];
		float x2 = edge[1][0];
		float y2 = edge[1][1];
		float x = v[0];
		float y = v[1];
		return x1 * (y2 - y) + y1 * (x - x2) + x2 * y - x * y2;
	}
	
	/// check if vertex v in circumcircle of tri
	public static bool InCircumcircleTest(float[] v, float[][] tri)
	{
		var x0 = tri[0][0];
		var y0 = tri[0][1];
		var x1 = tri[1][0];
		var y1 = tri[1][1];
		var x2 = tri[2][0];
		var y2 = tri[2][1];

		var d = MCMath.Determinant(new[]
		{
			new[] {x0, y0, 1},
			new[] {x1, y1, 1},
			new[] {x2, y2, 1},
		}) * 2;

		var x = MCMath.Determinant(new[]
		{
			new[] {x0 * x0 + y0 * y0, y0, 1},
			new[] {x1 * x1 + y1 * y1, y1, 1},
			new[] {x2 * x2 + y2 * y2, y2, 1},
		}) / d;

		var y = MCMath.Determinant(new[]
		{
			new[] {x0, x0 * x0 + y0 * y0, 1},
			new[] {x1, x1 * x1 + y1 * y1, 1},
			new[] {x2, x2 * x2 + y2 * y2, 1},
		}) / d;

		var d1 = (x - x0) * (x - x0) + (y - y0) * (y - y0);
		var d2 = (x - v[0]) * (x - v[0]) + (y - v[1]) * (y - v[1]);
		var d3 = Mathf.Sqrt(d1);
		var d4 = Mathf.Sqrt(d2);
		
		return d4 <= d3;
	}
}