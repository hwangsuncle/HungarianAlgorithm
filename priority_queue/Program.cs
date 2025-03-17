using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    class Assignment : IComparable<Assignment>
    {
        public double Distance;
        public int CustomerId;
        public int TaxiId;

        public Assignment(double distance, int customerId, int taxiId)
        {
            Distance = distance;
            CustomerId = customerId;
            TaxiId = taxiId;
        }

        public int CompareTo(Assignment other)
        {
            if (this.Distance != other.Distance)
                return this.Distance.CompareTo(other.Distance); // 거리 오름차순
            return this.CustomerId.CompareTo(other.CustomerId); // 손님 ID 오름차순
        }
    }

    static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines("..\\..\\..\\..\\kukn-Numkers2_좌표정보\\bin\\Debug\\net8.0\\input.txt");

        string[] firstLine = lines[0].Split(' ');
        int N = int.Parse(firstLine[0]);
        int M = int.Parse(firstLine[1]);

        int[,] customers = new int[N, 2];
        for (int i = 0; i < N; i++)
        {
            string[] coords = lines[i + 1].Split(' ');
            customers[i, 0] = int.Parse(coords[0]);
            customers[i, 1] = int.Parse(coords[1]);
        }

        int[,] taxis = new int[M, 2];
        for (int i = 0; i < M; i++)
        {
            string[] coords = lines[N + 1 + i].Split(' ');
            taxis[i, 0] = int.Parse(coords[0]);
            taxis[i, 1] = int.Parse(coords[1]);
        }

        Console.WriteLine("\n[손님 리스트]");
        for (int i = 0; i < N; i++)
            Console.WriteLine($"손님 {i + 1}: ({customers[i, 0]}, {customers[i, 1]})");

        Console.WriteLine("\n[택시 리스트]");
        for (int i = 0; i < M; i++)
            Console.WriteLine($"택시 {i + 1}: ({taxis[i, 0]}, {taxis[i, 1]})");

        PriorityQueue<Assignment, double> pq = new PriorityQueue<Assignment, double>();
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                int dx = Math.Abs(customers[i, 0] - taxis[j, 0]);
                int dy = Math.Abs(customers[i, 1] - taxis[j, 1]);
                double distance = dx + dy;
                pq.Enqueue(new Assignment(distance, i, j), distance);
            }
        }

        int[] assignment = new int[N];
        for (int i = 0; i < N; ++i) assignment[i] = -1;
        bool[] assignedTaxis = new bool[M];

        while (pq.Count > 0)
        {
            Assignment assign = pq.Dequeue();
            if (assignment[assign.CustomerId] == -1 && !assignedTaxis[assign.TaxiId])
            {
                assignment[assign.CustomerId] = assign.TaxiId;
                assignedTaxis[assign.TaxiId] = true;
            }
        }

        double totalDistance = 0;
        Console.WriteLine("\n[결과]");
        for (int i = 0; i < N; i++)
        {
            int taxiIndex = assignment[i];
            if (taxiIndex >= 0)
            {
                int dx = Math.Abs(customers[i, 0] - taxis[taxiIndex, 0]);
                int dy = Math.Abs(customers[i, 1] - taxis[taxiIndex, 1]);
                double distance = dx + dy;
                totalDistance += distance;
                Console.WriteLine($"손님 {i + 1} (좌표: ({customers[i, 0]}, {customers[i, 1]})) -> " +
                                  $"택시 {taxiIndex + 1} (좌표: ({taxis[taxiIndex, 0]}, {taxis[taxiIndex, 1]})) " +
                                  $"이동 거리: {distance:F2}");
            }
            else
            {
                Console.WriteLine($"손님 {i + 1} (좌표: ({customers[i, 0]}, {customers[i, 1]})) -> 배정 없음");
            }
        }
        Console.WriteLine($"\n전체 이동 거리 합: {totalDistance:F2}");
    }
}
