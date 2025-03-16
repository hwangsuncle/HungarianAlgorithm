using System;
using System.Linq;

class KuhnMunkres
{
    static int[,] CreateCostMatrix(int[,] costs)
    {
        int n = costs.GetLength(0);
        int m = costs.GetLength(1);
        int size = Math.Max(n, m);
        int[,] costMatrix = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i < n && j < m)
                    costMatrix[i, j] = costs[i, j];
                else
                    costMatrix[i, j] = 1000000; // 더미 값 (int.MaxValue 대신 큰 값 사용)
            }
        }
        return costMatrix;
    }

    static int[] HungarianAlgorithm(int[,] costMatrix)
    {
        int n = costMatrix.GetLength(0);
        int[] u = new int[n + 1], v = new int[n + 1], p = new int[n + 1], way = new int[n + 1];

        for (int i = 1; i <= n; i++)
        {
            p[0] = i;
            int[] minv = Enumerable.Repeat(int.MaxValue, n + 1).ToArray();
            bool[] used = new bool[n + 1];
            int j0 = 0;

            do
            {
                used[j0] = true;
                int i0 = p[j0], delta = int.MaxValue, j1 = 0;

                for (int j = 1; j <= n; j++)
                {
                    if (!used[j])
                    {
                        int cur = costMatrix[i0 - 1, j - 1] - u[i0] - v[j];
                        if (cur < minv[j])
                        {
                            minv[j] = cur;
                            way[j] = j0;
                        }
                        if (minv[j] < delta)
                        {
                            delta = minv[j];
                            j1 = j;
                        }
                    }
                }

                for (int j = 0; j <= n; j++)
                {
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        minv[j] -= delta;
                    }
                }
                j0 = j1;
            } while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            } while (j0 != 0);
        }

        int[] result = new int[n];
        for (int j = 1; j <= n; j++)
        {
            if (p[j] > 0)
                result[p[j] - 1] = j - 1;
        }
        return result;
    }

    static void Main()
    {
        int[,] costs =
        {
            {3, 8, 9,5},
            {4, 12, 7,4},
            {4, 8, 5, 5  },
            {3, 2, 4, 12  },
            {13,22,34,22  }


        };

        int[,] costMatrix = CreateCostMatrix(costs);
        int[] assignment = HungarianAlgorithm(costMatrix);

        Console.WriteLine("최적의 작업 할당:");
        for (int i = 0; i < costs.GetLength(0); i++)
        {
            Console.WriteLine($"노동자 {i + 1} → 작업 {assignment[i] + 1}");
        }
    }
}
