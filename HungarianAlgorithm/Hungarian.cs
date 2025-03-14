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
        double[,] distanceMatrix = new double[N, M] , distanceMatrixTmp = new double[N,M];
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                int dx = customers[i, 0] - taxis[j, 0];
                int dy = customers[i, 1] - taxis[j, 1];
                distanceMatrix[i, j] = distanceMatrixTmp[i,j]= Math.Sqrt(dx * dx + dy * dy); // 유클리드 거리
            }
        }

        // 헝가리안 알고리즘 적용
        HungarianAlgorithm hungarian = new HungarianAlgorithm(distanceMatrixTmp);
        int[] assignment = hungarian.Solve();

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

// 헝가리안 알고리즘 구현
public class HungarianAlgorithm
{
    private double[,] costMatrix;
    private int numRows;
    private int numCols;
    private int[] assignment;

    public HungarianAlgorithm(double[,] costMatrix)
    {
        this.costMatrix = costMatrix;
        numRows = costMatrix.GetLength(0);
        numCols = costMatrix.GetLength(1);
        assignment = new int[numRows];
    }

    public int[] Solve()
    {
        // Step 1: 행 감소
        for (int i = 0; i < numRows; i++)
        {
            double min = costMatrix[i, 0];
            for (int j = 1; j < numCols; j++)
            {
                if (costMatrix[i, j] < min)
                    min = costMatrix[i, j];
            }
            for (int j = 0; j < numCols; j++)
            {
                costMatrix[i, j] -= min;
            }
        }

        // Step 2: 열 감소
        for (int j = 0; j < numCols; j++)
        {
            double min = costMatrix[0, j];
            for (int i = 1; i < numRows; i++)
            {
                if (costMatrix[i, j] < min)
                    min = costMatrix[i, j];
            }
            for (int i = 0; i < numRows; i++)
            {
                costMatrix[i, j] -= min;
            }
        }

        // Step 3: 0인 셀을 찾고 배정
        bool[] rowCovered = new bool[numRows];
        bool[] colCovered = new bool[numCols];
        int[] assignment = new int[numRows];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                if (costMatrix[i, j] == 0 && !rowCovered[i] && !colCovered[j])
                {
                    assignment[i] = j;
                    rowCovered[i] = true;
                    colCovered[j] = true;
                    break;
                }
            }
        }

        // Step 4: 모든 손님이 배정되었는지 확인
        if (Array.TrueForAll(rowCovered, covered => covered))
        {
            return assignment;
        }

        // Step 5: 최적 배정을 찾기 위해 추가 작업 수행
        while (true)
        {
            // 커버되지 않은 행과 열을 찾음
            int[] rowCover = new int[numRows];
            int[] colCover = new int[numCols];
            for (int i = 0; i < numRows; i++)
            {
                if (!rowCovered[i])
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (costMatrix[i, j] == 0 && !colCovered[j])
                        {
                            assignment[i] = j;
                            rowCovered[i] = true;
                            colCovered[j] = true;
                            break;
                        }
                    }
                }
            }

            // 모든 손님이 배정되었는지 확인
            if (Array.TrueForAll(rowCovered, covered => covered))
            {
                return assignment;
            }

            // 커버되지 않은 행과 열을 찾아 최소값을 빼고 다시 시도
            double minUncovered = double.MaxValue;
            for (int i = 0; i < numRows; i++)
            {
                if (!rowCovered[i])
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (!colCovered[j] && costMatrix[i, j] < minUncovered)
                        {
                            minUncovered = costMatrix[i, j];
                        }
                    }
                }
            }

            for (int i = 0; i < numRows; i++)
            {
                if (!rowCovered[i])
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        costMatrix[i, j] -= minUncovered;
                    }
                }
            }

            for (int j = 0; j < numCols; j++)
            {
                if (colCovered[j])
                {
                    for (int i = 0; i < numRows; i++)
                    {
                        costMatrix[i, j] += minUncovered;
                    }
                }
            }
        }
    }
}