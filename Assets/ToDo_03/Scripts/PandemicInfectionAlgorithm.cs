using System.Collections.Generic;
using AlgoCourse.Lesson3;

namespace AlgoCourse.StudentWork
{
    public sealed class PandemicInfectionAlgorithm : ICityInfectionAlgorithm
    {
        private const int MaximumInfectionLevel = 3;                                        //한 도시가 가질 수 있는 최대 감염 단계를 3으로 지정합니다.
        private readonly Dictionary<int, int[]> graph = new Dictionary<int, int[]>();       //도시 번호를 Key로 하고 연결된 이웃 도시 목록을 Value로 저장 합니다.
        private readonly Dictionary<int, int> infectionLevels = new Dictionary<int, int>(); //각 도시 번호별 현재 감염 단계를 저장합니다.
        private readonly Queue<int> infectionQueue = new Queue<int>();                      //감염 처리를 기다리고 있는 도시들을 순서대로 저장하는 큐 입니다
        private readonly HashSet<int> outbreakCities = new HashSet<int>();                  //현재 연쇄 감염 과정에서 이미 아웃브레이크가 발생한 도시를 저장

        public int PendingCount => infectionQueue.Count;                                    //감염 큐에 대기 중인 도시의 개수를 반환
        public int OutbreakCount { get; private set; }                                      //지금 까지 발생한 전체 아웃브레이크 횟수를 저장

        public void Initialize(IReadOnlyDictionary<int, int[]> cityGraph)
        {
            graph.Clear();
            infectionLevels.Clear();
            infectionQueue.Clear();
            outbreakCities.Clear();

            OutbreakCount = 0;

            //전달 받은 모든 도시 정보를 하나씩 확인 합니다
            foreach (KeyValuePair < int,int[]> city in cityGraph)
            {
                graph.Add(city.Key, city.Value);
                infectionLevels.Add(city.Key, 0); //해당 도시의 초기 감염 단계를 0으로 설정합니다
            }
        }
    

        public bool QueueInfection(int cityId) // 지정한 도시를 감염 처리 대기 큐에 추가
        {
            // TODO 04: 존재하는 도시의 감염 요청을 Enqueue합니다.
            if(!graph.ContainsKey(cityId))
            {
                return false;
            }

            if (infectionQueue.Count == 0)
            {
                outbreakCities.Clear();
            }

            infectionQueue.Enqueue(cityId); //지정한 도시를 감염 처리 대기큐의 마지막에 추가합니다
            return false;
        }

        public CityInfectionStep ProcessNext()
        {
            if (infectionQueue.Count == 0)    //처리할 도시가 감염 큐에 남아있는지 확인
            {
                return new CityInfectionStep(-1, 0, false, false);             //처리할 도시가 없다면 아무 변화가 없는 결과를 반환한다.
            }

            int cityId = infectionQueue.Dequeue();                             //큐에서 가장 먼저 들어온 도시 하나를 꺼냅니다.
            int currentLevel = infectionLevels[cityId];                        //해당 도시의 감염 단계를 가져 온다.

            if (currentLevel < MaximumInfectionLevel)
            {
                int nextLevel = currentLevel + 1;
                infectionLevels[cityId] = nextLevel;
                return new CityInfectionStep(cityId, nextLevel, false, true);  //감염 단계가 증가 했고 아웃 브레이크는 발생하지 않았다는 결과를 반환
            }

            if (!outbreakCities.Add(cityId))                                   //이미 이번 연쇄 감염에서 아웃브레이크가 발생했던 도시인지 확인
            {
                return new CityInfectionStep(cityId, currentLevel, false, false);          //발생한 도시라면 추가 확산 없다.
            }

            OutbreakCount++;                                                   //새로운 아웃브레이크가 발생 했으므로 전체 발생 횟수를 1 증가

            foreach (int neighborId in graph[cityId])                          //도시의 연결된 모든 이웃 도시를 확인
            {
                infectionQueue.Enqueue(neighborId);                            //각 이웃 도시를 감염 처리 대기 큐에 추가
            }

            return new CityInfectionStep(cityId, currentLevel, true, true);     //현재 도시에서 아웃브레이크가 발생하고 상태가 변경 되었다는 결과를 반환
        }

        public int GetInfectionLevel(int cityId)
        {
            // TODO 06: 도시의 현재 감염 단계를 반환합니다.
            return infectionLevels.TryGetValue(cityId, out int level) ? level : 0;
        }  
    }
}
