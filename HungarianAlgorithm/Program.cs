#if false
// 탐욕   알고리즘을 사용한 택시 배정 문제
using System;

class Program
{
    static void Main(string[] args)
    {
        // 입력 받기
        Console.Write("손님의 수 N: ");
        int N = int.Parse(Console.ReadLine());
        Console.Write("택시의 수 M: ");
        int M = int.Parse(Console.ReadLine());

        // 좌표 랜덤 생성
        Random rand = new Random();
        int[,] customers = new int[N, 2]; // 손님 좌표
        int[,] taxis = new int[M, 2];     // 택시 좌표

        for (int i = 0; i < N; i++)
        {
            customers[i, 0] = rand.Next(0, 10000); // PXi
            customers[i, 1] = rand.Next(0, 10000); // PYi
        }

        for (int i = 0; i < M; i++)
        {
            taxis[i, 0] = rand.Next(0, 10000); // TXi
            taxis[i, 1] = rand.Next(0, 10000); // TYi
        }

        // 입력된 데이터 출력
        Console.WriteLine("\n[손님 리스트]");
        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"손님 {i + 1}: ({customers[i, 0]}, {customers[i, 1]})");
        }

        Console.WriteLine("\n[택시 리스트]");
        for (int i = 0; i < M; i++)
        {
            Console.WriteLine($"택시 {i + 1}: ({taxis[i, 0]}, {taxis[i, 1]})");
        }

        // 거리 행렬 계산
        double[,] distanceMatrix = new double[N, M];
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                int dx = customers[i, 0] - taxis[j, 0];
                int dy = customers[i, 1] - taxis[j, 1];
                distanceMatrix[i, j] = Math.Sqrt(dx * dx + dy * dy); // 유클리드 거리
            }
        }

        // 최적 배정 찾기 (탐욕 알고리즘 사용)
        int[] assignment = new int[N];
        bool[] assignedTaxis = new bool[M]; // 이미 배정된 택시를 표시

        for (int i = 0; i < N; i++)
        {
            double minDistance = double.MaxValue;
            int selectedTaxi = -1;

            for (int j = 0; j < M; j++)
            {
                if (!assignedTaxis[j] && distanceMatrix[i, j] < minDistance)
                {
                    minDistance = distanceMatrix[i, j];
                    selectedTaxi = j;
                }
            }

            if (selectedTaxi != -1)
            {
                assignment[i] = selectedTaxi;
                assignedTaxis[selectedTaxi] = true;
            }
        }

        // 결과 출력
        double totalDistance = 0;
        Console.WriteLine("\n[결과]");
        for (int i = 0; i < N; i++)
        {
            int taxiIndex = assignment[i];
            double distance = distanceMatrix[i, taxiIndex];
            totalDistance += distance;

            Console.WriteLine($"손님 {i + 1} (좌표: ({customers[i, 0]}, {customers[i, 1]})) -> " +
                              $"택시 {taxiIndex + 1} (좌표: ({taxis[taxiIndex, 0]}, {taxis[taxiIndex, 1]})) " +
                              $"이동 거리: {distance:F2}");
        }
        Console.WriteLine($"\n전체 이동 거리 합: {totalDistance:F2}");
    }
} 
#endif