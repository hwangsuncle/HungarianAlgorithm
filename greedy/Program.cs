#if true
// 탐욕   알고리즘을 사용한 택시 배정 문제
using System;

class Program
{
    static void Main(string[] args)
    {
        // 파일에서 입력 읽기
        string[] lines = File.ReadAllLines("C:\\Users\\황광순\\source\\repos\\HungarianAlgorithm2\\kukn-Numkers2_좌표정보\\bin\\Debug\\net8.0\\input.txt");

        // N과 M 읽기
        string[] firstLine = lines[0].Split(' ');
        int N = int.Parse(firstLine[0]); // 손님의 수
        int M = int.Parse(firstLine[1]); // 택시의 수

        // 손님 좌표 읽기
        int[,] customers = new int[N, 2];
        for (int i = 0; i < N; i++)
        {
            string[] coords = lines[i + 1].Split(' ');
            customers[i, 0] = int.Parse(coords[0]); // PXi
            customers[i, 1] = int.Parse(coords[1]); // PYi
        }

        // 택시 좌표 읽기
        int[,] taxis = new int[M, 2];
        for (int i = 0; i < M; i++)
        {
            string[] coords = lines[N + 1 + i].Split(' ');
            taxis[i, 0] = int.Parse(coords[0]); // TXi
            taxis[i, 1] = int.Parse(coords[1]); // TYi
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
                int dx = Math.Abs(customers[i, 0] - taxis[j, 0]);
                int dy = Math.Abs(customers[i, 1] - taxis[j, 1]);
                distanceMatrix[i, j] = dx + dy;
            }
        }

        Console.WriteLine("\n비용 행렬:");
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M;j++)
            {
                Console.Write(distanceMatrix[i, j].ToString().PadLeft(5));
            }
            Console.WriteLine();
        }
        // 최적 배정 찾기 (탐욕 알고리즘 사용)
        int[] assignment = new int[N];
        for (int i = 0; i < N; ++i) assignment[i] = -1;


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
            if (selectedTaxi != -1 )
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
         
           
            if (taxiIndex >= 0)
            {
                double distance = distanceMatrix[i, taxiIndex];
                totalDistance += distance;
                Console.WriteLine($"손님 {i + 1} (좌표: ({customers[i, 0]}, {customers[i, 1]})) -> " +
                                  $"택시 {taxiIndex + 1} (좌표: ({taxis[taxiIndex, 0]}, {taxis[taxiIndex, 1]})) " +
                                  $"이동 거리: {distance:F2}");
            }
            else
            {
                Console.WriteLine($"손님 {i + 1} (좌표: ({customers[i, 0]}, {customers[i, 1]})) -> " + " 배정 없음");
                                 
            }
        }
        Console.WriteLine($"\n전체 이동 거리 합: {totalDistance:F2}");
    }
} 
#endif