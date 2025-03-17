#if true
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
    // 헝가리안 알고리즘 (Kuhn-Munkres 알고리즘) 구현
    // 입력: costMatrix(n×m 비용 행렬), n(작업자 수), m(작업 수)
    // 출력: 각 작업자에게 할당된 작업 인덱스 배열 (할당 불가 시 -1)
    static int[] HungarianAlgorithm(int[,] costMatrix, int n, int m)
    {
        // 1. 행렬 확장: n×m → size×size (size = max(n,m))
        int size = Math.Max(n, m);
        int[] u = new int[size + 1]; // 행 가중치 (Step 1-2의 축소량 저장)
        int[] v = new int[size + 1]; // 열 가중치 (Step 1-2의 축소량 저장)
        int[] p = new int[size + 1]; // 열 j에 할당된 행 인덱스 저장 (최종 매칭 결과)
        int[] way = new int[size + 1]; // 경로 추적용 배열

        // 2. 각 행에 대해 최적 열 탐색 (알고리즘 핵심 루프)
        for (int i = 1; i <= size; i++)
        {
            p[0] = i; // 가상의 0번 열 시작점 설정
            int[] minv = Enumerable.Repeat(int.MaxValue, size + 1).ToArray(); // 각 열의 최소값 추적
            bool[] used = new bool[size + 1]; // 열 사용 여부 체크
            int j0 = 0; // 현재 처리 중인 열

            // 3. 델타(δ) 계산 루프 (Step 4: 추가 0 생성)
            do
            {
                used[j0] = true; // 현재 열 사용 표시
                int i0 = p[j0]; // 현재 열에 할당된 행
                int delta = int.MaxValue; // 조정값 δ 초기화
                int j1 = 0; // 다음 최적 열 후보

                // 4. 모든 열 탐색하여 최소 δ 찾기
                for (int j = 1; j <= size; j++)
                {
                    if (!used[j])
                    {
                        // 현재 비용 계산: costMatrix - u[i] - v[j]
                        int cur = costMatrix[i0 - 1, j - 1] - u[i0] - v[j];

                        // minv[j] 업데이트 (Step 4의 ε 계산과 동일)
                        if (cur < minv[j])
                        {
                            minv[j] = cur;
                            way[j] = j0; // 경로 저장
                        }

                        // 최소 δ 후보 갱신
                        if (minv[j] < delta)
                        {
                            delta = minv[j];
                            j1 = j; // 다음 열 후보
                        }
                    }
                }

                // 5. 가중치 업데이트 (Step 1-2: 행/열 축소)
                for (int j = 0; j <= size; j++)
                {
                    if (used[j])
                    {
                        // 사용된 열: u(행 가중치) 증가, v(열 가중치) 감소
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        // 미사용 열: minv 감소 (ε 적용)
                        minv[j] -= delta;
                    }
                }
                j0 = j1; // 다음 열로 이동
            } while (p[j0] != 0); // 할당 가능한 열이 없을 때까지 반복

            // 6. 경로 역추적하여 매칭 갱신 (Step 5: 할당 결정)
            do
            {
                int j1 = way[j0]; // 이전 경로 추적
                p[j0] = p[j1]; // 매칭 갱신
                j0 = j1;
            } while (j0 != 0); // 시작점(0)에 도달할 때까지 반복
        }

        // 7. 결과 변환: size×size → n×m 매핑
        int[] result = Enumerable.Repeat(-1, n).ToArray();
        for (int j = 1; j <= m; j++)
        {
            if (p[j] - 1 < n) // 더미 행 제외
                result[p[j] - 1] = j - 1; // 실제 할당 저장
        }
        return result; // 예: result[0] = 2 → 작업자 0은 작업 2 할당
    }


    static ((int, int)[], (int, int)[]) LoadInputFromFile(string filename, out int n, out int m)
    {
        var lines = File.ReadAllLines(filename);
        var firstLine = lines[0].Split();
        n = int.Parse(firstLine[0]);
        m = int.Parse(firstLine[1]);

        var workers = new (int, int)[n];
        var tasks = new (int, int)[m];

        for (int i = 0; i < n; i++)
        {
            var coords = lines[i + 1].Split();
            workers[i] = (int.Parse(coords[0]), int.Parse(coords[1]));
        }
        for (int i = 0; i < m; i++)
        {
            var coords = lines[i + 1 + n].Split();
            tasks[i] = (int.Parse(coords[0]), int.Parse(coords[1]));
        }
        return (workers, tasks);
    }
    static void SaveInputToFile(int n, int m, (int, int)[] workers, (int, int)[] tasks)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "input.txt");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"{n} {m}");
                foreach (var worker in workers)
                    writer.WriteLine($"{worker.Item1} {worker.Item2}");
                foreach (var task in tasks)
                    writer.WriteLine($"{task.Item1} {task.Item2}");
            }
            Console.WriteLine($"파일 저장 완료: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"파일 저장 실패: {ex.Message}");
        }
    }

    static void Main()
    {
#if false
        // file 에서 읽어 들임
        int n, m;
        var (workers, tasks) = LoadInputFromFile("input.txt", out n, out m);
        int[,] costs = ComputeCostMatrix(workers, tasks);

#else
        //
       // 램덤하게 생성하고   입력 데이터를 파일로   저장
       // 이 input file 로 다른  로직과 비교하기 위함
       //
        Console.Write("작업 수 입력: ");
        int n = int.Parse(Console.ReadLine());
        Console.Write("AGV 수 입력: ");
        int m = int.Parse(Console.ReadLine());


        var workers = GenerateRandomPositions(n);
        var tasks = GenerateRandomPositions(m);
        int[,] costs = ComputeCostMatrix(workers, tasks);
        SaveInputToFile(n,m, workers, tasks);

#endif
        Console.WriteLine("\n작업(task) 위치:");
        for (int i = 0; i < n; i++)
            Console.WriteLine($"작업(task) {i + 1}: ({workers[i].Item1}, {workers[i].Item2})");

        Console.WriteLine("\nAGV 위치:");
        for (int j = 0; j < m; j++)
            Console.WriteLine($"AGV {j + 1}: ({tasks[j].Item1}, {tasks[j].Item2})");


        Console.WriteLine("\n비용(거리) 행렬:");
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

        int totalCost = 0;
        Console.WriteLine("\n최적의 AGV 할당:");
        for (int i = 0; i < n; i++)
        {
            if (assignment[i] == -1)
                Console.WriteLine($"작업 {i + 1} ({workers[i].Item1}, {workers[i].Item2}) → AGV 없음");
            else
            {
                Console.WriteLine($"작업 {i + 1} ({workers[i].Item1}, {workers[i].Item2}) → AGV {assignment[i] + 1} ({tasks[assignment[i]].Item1}, {tasks[assignment[i]].Item2}) : 거리:{costMatrix[i, assignment[i]]}");
                totalCost += costMatrix[i, assignment[i]];
            }
        }
        Console.WriteLine($"\n총 거리 비용: {totalCost}");
    }
}

#endif