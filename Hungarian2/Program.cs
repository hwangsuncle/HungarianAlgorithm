using System;
using System.Linq;
using System.Diagnostics;

class Program
{
    static Random rand = new Random();

    static int ManhattanDistance((int x, int y) p1, (int x, int y) p2)
    {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
    }

    static int[,] CreateCostMatrix((int, int)[] passengers, (int, int)[] taxis)
    {
        int N = passengers.Length;
        int M = taxis.Length;
        int size = Math.Max(N, M);
        int[,] cost = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                cost[i, j] = (i < N && j < M)
                    ? ManhattanDistance(passengers[i], taxis[j])
                    : 1000000000; // 무한대
            }
        }
        return cost;
    }

    static int[] HungarianAlgorithm(int[,] costMatrix)
    {
        int n = costMatrix.GetLength(0);
        int[] u = new int[n], v = new int[n], p = new int[n], way = new int[n];

        for (int i = 1; i < n; i++)
        {
            int[] minv = Enumerable.Repeat(int.MaxValue, n).ToArray();
            bool[] used = new bool[n];
            int j0 = 0;
            p[0] = i;

            do
            {
                used[j0] = true;
                int i0 = p[j0], delta = int.MaxValue, j1 = 0;

                for (int j = 1; j < n; j++)
                {
                    if (!used[j])
                    {
                        int cur = costMatrix[i0, j] - u[i0] - v[j];
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
                if (delta == int.MaxValue)
                {
                    break;
                }
                for (int j = 0; j < n; j++)
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
        return p.Select(x => x - 1).ToArray();
    }

    static void Main()
    {
        int N = 100, M = 200;
        var passengers = Enumerable.Range(0, N).Select(_ => (rand.Next(10000), rand.Next(10000))).ToArray();
        var taxis = Enumerable.Range(0, M).Select(_ => (rand.Next(10000), rand.Next(10000))).ToArray();

        Console.WriteLine("승객 위치:");
        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"승객 {i + 1}: ({passengers[i].Item1}, {passengers[i].Item2})");
        }

        Console.WriteLine("\n택시 위치:");
        for (int i = 0; i < M; i++)
        {
            Console.WriteLine($"택시 {i + 1}: ({taxis[i].Item1}, {taxis[i].Item2})");
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        int[,] costMatrix = CreateCostMatrix(passengers, taxis);
        int[] assignment = HungarianAlgorithm(costMatrix);
        stopwatch.Stop();

        var validAssignments = assignment
            .Select((taxiIdx, passengerIdx) => (taxiIdx, passengerIdx))
            .Where(x => x.taxiIdx >= 0 && x.taxiIdx < M)
            .Take(Math.Min(N, M))
            .ToList();

        Console.WriteLine("\n승객 위치 -- 택시 위치 , 거리");
        int totalDistance = 0;
        foreach (var assign in validAssignments)
        {
            int pIdx = assign.passengerIdx;
            int tIdx = assign.taxiIdx;
            int distance = ManhattanDistance(passengers[pIdx], taxis[tIdx]);
            totalDistance += distance;
            Console.WriteLine($"승객({pIdx + 1}) ({passengers[pIdx].Item1},{passengers[pIdx].Item2}) -- " +
                              $"택시({tIdx + 1}) ({taxis[tIdx].Item1},{taxis[tIdx].Item2}) , 거리 {distance}");
        }

        int unassigned = Math.Max(N, M) - validAssignments.Count;
        Console.WriteLine($"\nTotal Assigned: {validAssignments.Count}, Unassigned: {unassigned}");
        Console.WriteLine($"Total Distance: {totalDistance}");
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}
