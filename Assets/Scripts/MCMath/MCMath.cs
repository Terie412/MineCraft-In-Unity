
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
    }
