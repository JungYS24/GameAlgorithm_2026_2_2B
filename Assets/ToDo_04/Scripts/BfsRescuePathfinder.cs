using System.Collections.Generic;
using AlgoCourse.Lesson4;

namespace AlgoCourse.StudentWork
{
    public sealed class BfsRescuePathfinder : IRescuePathfinder
    {
        private static readonly GridPosition[] Directions =
        {
            new GridPosition(1,0),
            new GridPosition(-1,0),
            new GridPosition(0,1),
            new GridPosition(0,-1)
        };

        public PathSearchResult FindPath(
            int width,
            int height,
            GridPosition start,
            GridPosition goal,
            IReadOnlyCollection<GridPosition> blockedCells)
        {
            // TODO 01: Queue, visited, cameFrom을 생성합니다. BFS가 앞으로 확인할 칸을 Queue 에 저장
            Queue<GridPosition> frontier = new Queue<GridPosition>();

            // TODO 02: 시작 칸을 Queue와 visited에 넣습니다. 같은 칸을 반복 방문 하지 않도록 한다.
            HashSet<GridPosition> visited = new HashSet<GridPosition>();

            // TODO 03: Queue가 빌 때까지 상하좌우 이웃을 탐색합니다. 목표에서 시작점 까지 길을 되짚기 위해 이전 칸을 기록
            Dictionary<GridPosition, GridPosition> cameFrom = new Dictionary<GridPosition, GridPosition>();

            List<GridPosition> visitedOrder = new List<GridPosition>();
            HashSet<GridPosition> blocked = new HashSet<GridPosition>(blockedCells);

            frontier.Enqueue(start);
            visited.Add(start);

            while (frontier.Count > 0)
            {
                GridPosition current = frontier.Dequeue();
                visitedOrder.Add(current);

                if (current == goal) break;

                foreach (GridPosition direction in Directions)
                {
                    GridPosition next = new GridPosition(current.X + direction.X, current.Y + direction.Y);

                    // TODO 04: 범위, 장애물, 방문 여부를 검사합니다.
                    if (!IsInside(next, width, height))
                    {
                        continue;
                    }

                    if (blocked.Contains(next) || visited.Contains(next))
                    {
                        continue;
                    }

                    visited.Add(next);
                    cameFrom[next] = current;
                    frontier.Enqueue(next);
                }
            }

            //목표를 방문하지 못했다면 빈 경로를 반환합니다.
            if (!visited.Contains(goal))
            {
                return new PathSearchResult(visitedOrder, new List<GridPosition>());
            }

            // TODO 05: cameFrom을 따라 최단 경로를 복원합니다.
            List<GridPosition> path = new List<GridPosition>();
            GridPosition pathCell = goal;
            path.Add(pathCell);

            while (pathCell != start)
            {
                pathCell = cameFrom[pathCell];
                path.Add(pathCell);
            }

            path.Reverse();

            return new PathSearchResult(
                visitedOrder,
                path);
        }

        private static bool IsInside(GridPosition position, int width, int height)
        {
            return position.X >= 0 && position.X < width &&
                   position.Y >= 0 && position.Y < height;
        }
    }
}            