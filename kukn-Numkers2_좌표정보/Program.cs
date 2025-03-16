using System;
using System.Linq;

class KuhnMunkres
{
    static Random rand = new Random();

    static (int, int)[] GenerateRandomPositions(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => (rand.Next(0, 1000), rand.Next(0, 1000)))
            .ToArray();
    }

    static int[,] ComputeCostMatrix((int, int)[] workers, (int, int)[] tasks)
    {
        int n = workers.Length;
        int m = tasks.Length;
        int[,] costs = new int[n, m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                costs[i, j] = Math.Abs(workers[i].Item1 - tasks[j].Item1) + Math.Abs(workers[i].Item2 - tasks[j].Item2);
            }
        }
        return costs;
    }

    static int[,] CreateCostMatrix(int[,] costs, int n, int m)
    {
        int size = Math.Max(n, m);
        int[,] costMatrix = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i < n && j < m)
                    costMatrix[i, j] = costs[i, j];
                else
                    costMatrix[i, j] = 1000000; // 더미 값
            }
        }
        return costMatrix;
    }

    static int[] HungarianAlgorithm(int[,] costMatrix, int n, int m)
    {
        int size = Math.Max(n, m);
        int[] u = new int[size + 1], v = new int[size + 1], p = new int[size + 1], way = new int[size + 1];

        for (int i = 1; i <= size; i++)
        {
            p[0] = i;
            int[] minv = Enumerable.Repeat(int.MaxValue, size + 1).ToArray();
            bool[] used = new bool[size + 1];
            int j0 = 0;

            do
            {
                used[j0] = true;
                int i0 = p[j0], delta = int.MaxValue, j1 = 0;

                for (int j = 1; j <= size; j++)
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

                for (int j = 0; j <= size; j++)
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

        int[] result = Enumerable.Repeat(-1, n).ToArray();
        for (int j = 1; j <= m; j++)
        {
            if (p[j] - 1 < n)
                result[p[j] - 1] = j - 1;
        }
        return result;
    }

    static void Main()
    {
        Console.Write("작업 수 입력: ");
        int n = int.Parse(Console.ReadLine());
        Console.Write("AGV 수 입력: ");
        int m = int.Parse(Console.ReadLine());

       
        var workers = GenerateRandomPositions(n);
        var tasks = GenerateRandomPositions(m);
        int[,] costs = ComputeCostMatrix(workers, tasks);

        Console.WriteLine("\n노동자 위치:");
        for (int i = 0; i < n; i++)
            Console.WriteLine($"노동자 {i + 1}: ({workers[i].Item1}, {workers[i].Item2})");

        Console.WriteLine("\n작업 위치:");
        for (int j = 0; j < m; j++)
            Console.WriteLine($"작업 {j + 1}: ({tasks[j].Item1}, {tasks[j].Item2})");

        Console.WriteLine("\n비용 행렬:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Console.Write(costs[i, j].ToString().PadLeft(5));
            }
            Console.WriteLine();
        }

        int[,] costMatrix = CreateCostMatrix(costs, n, m);
        int[] assignment = HungarianAlgorithm(costMatrix, n, m);

        Console.WriteLine("\n최적의 작업 할당:");
        for (int i = 0; i < n; i++)
        {
            if (assignment[i] == -1)
                Console.WriteLine($"작업 {i + 1} ({workers[i].Item1}, {workers[i].Item2}) → AGV 없음");
            else
                Console.WriteLine($"작업 {i + 1} ({workers[i].Item1}, {workers[i].Item2}) → AGV {assignment[i] + 1} ({tasks[assignment[i]].Item1}, {tasks[assignment[i]].Item2})");
        }
    }
}
