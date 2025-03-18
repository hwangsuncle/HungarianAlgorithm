using System;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;

class Program
{
    static int N, M;
    static int[,] customers;
    static int[,] taxis;
    static double[,] distanceMatrix;
    static int[] bestAssignment;
    static double bestTotalDistance = double.MaxValue;
    static int[] visit;
    
    static int cnt;
    static void Main(string[] args)
    {
        // 파일에서 입력 읽기
        string[] lines = File.ReadAllLines("..\\..\\..\\..\\kukn-Numkers2_좌표정보\\bin\\Debug\\net8.0\\input.txt");

        // N과 M 읽기
        string[] firstLine = lines[0].Split(' ');
        N = int.Parse(firstLine[0]); // 손님의 수
        M = int.Parse(firstLine[1]); // 택시의 수
        visit = new int[M];

        if (N > M)
        {
            Console.WriteLine("손님의 수가 택시의 수보다 많아 매칭이 불가능합니다.");
            return;
        }

        // 손님 좌표 읽기
        customers = new int[N, 2];
        for (int i = 0; i < N; i++)
        {
            string[] coords = lines[i + 1].Split(' ');
            customers[i, 0] = int.Parse(coords[0]);
            customers[i, 1] = int.Parse(coords[1]);
        }

        // 택시 좌표 읽기
        taxis = new int[M, 2];
        for (int i = 0; i < M; i++)
        {
            string[] coords = lines[N + 1 + i].Split(' ');
            taxis[i, 0] = int.Parse(coords[0]);
            taxis[i, 1] = int.Parse(coords[1]);
        }

        // 거리 행렬 미리 계산
        distanceMatrix = new double[N, M];
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                distanceMatrix[i, j] = CalculateDistance(i, j);
            }
        }

        // 가능한 모든 매칭 조합을 계산하여 최소 거리 찾기
        int[] taxiIndices = Enumerable.Range(0, M).ToArray();
        //Permute(taxiIndices, 0);
        Permute2(taxiIndices, 0,0);
        // 결과 출력
        
        Console.WriteLine("\n최적의 매칭 조합:");
        double totalDistance = 0;
        for (int i = 0; i < N; i++)
        {
            int taxiIndex = bestAssignment[i];
            double distance = distanceMatrix[i, taxiIndex];
            totalDistance += distance;
            Console.WriteLine($"손님 {i + 1} (좌표: ({customers[i, 0]}, {customers[i, 1]})) -> " +
                              $"택시 {taxiIndex + 1} (좌표: ({taxis[taxiIndex, 0]}, {taxis[taxiIndex, 1]})) " +
                              $"이동 거리: {distance:F2}");
        }
        Console.WriteLine($"\n최소 총 이동 거리: {bestTotalDistance:F2}");
        Console.WriteLine($"\n{cnt}");
    }

 
    static void Permute(int[] arr, int start)
    {
        cnt++;
        if (start == N)
        {
            double totalDistance = 0;
            for (int i = 0; i < N; i++)
            {
                totalDistance += distanceMatrix[i, arr[i]];
            }
            if (totalDistance < bestTotalDistance)
            {
                bestTotalDistance = totalDistance;
                bestAssignment = arr.Take(N).ToArray();
            }
            return;
        }
        for (int i = start; i < M ; i++)
        {
            Swap(ref arr[start], ref arr[i]); 
            Permute(arr, start + 1);
            Swap(ref arr[start], ref arr[i]);
        }
    }



    static void Permute2(int[] arr, int start,double cum)
    {
        cnt++;
        if (cum > bestTotalDistance) return;
        if (start == N)
        {
          
            if (cum < bestTotalDistance)
            {
                bestTotalDistance = cum;
                bestAssignment = arr.Take(N).ToArray();
            }
            return;
        }
        for (int i = 0; i < M; i++)
        {

            if (visit[i] ==1) continue;
            visit[i] = 1;
            arr[start] = i;
            Permute2(arr, start + 1 ,cum + distanceMatrix[start,i]);
            visit[i] = 0;
            
        }
    }


    static double CalculateDistance(int customerIndex, int taxiIndex)
    {
        int dx = Math.Abs(customers[customerIndex, 0] - taxis[taxiIndex, 0]);
        int dy = Math.Abs(customers[customerIndex, 1] - taxis[taxiIndex, 1]);
        return dx + dy;
    }

    static void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}
