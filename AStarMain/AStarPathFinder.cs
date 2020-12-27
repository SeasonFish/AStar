using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    /// <summary>
    /// A*ノードの位置情報を保持するクラスです。
    /// </summary>
    public class AStarPosition
    {
        /// <summary>
        /// 位置情報を取得または設定します。
        /// </summary>
        internal Position Position { get; }

        /// <summary>
        /// 一個前のノード情報を取得または設定します。
        /// </summary>
        internal AStarPosition PreviousPosition { get; set; }

        /// <summary>
        /// このノードにたどり着くまでの経路の長さ(g)を取得または設定します。
        /// </summary>
        internal int Cost { get; set; }

        /// <summary>
        /// 終点までの予想的な距離(h)を取得または設定します。
        /// </summary>
        internal int Distance { get; set; }

        /// <summary>
        /// <see cref="Cost"/>と<see cref="Distance"/>の合計値(f)を取得します。
        /// </summary>
        internal int TotalLength => Cost + Distance;

        public AStarPosition(Position position, AStarPosition previousPosition, int cost, int distance)
        {
            Position = position;
            PreviousPosition = previousPosition;
            Cost = cost;
            Distance = distance;
        }

        public static bool operator ==(AStarPosition position1, AStarPosition position2)
        {
            if (position1 as object == null)
                return position1 as object == null;

            return position1.Equals(position2) == true;
        }

        public static bool operator !=(AStarPosition position1, AStarPosition position2)
            => !(position1 == position2);

        public override bool Equals(object obj)
        {
            return obj is AStarPosition otherPosition
                && Position == otherPosition.Position;
        }

        public override int GetHashCode() => $"{nameof(AStarPosition)}{Position.GetHashCode()}".GetHashCode();

    }

    /// <summary>
    /// A*アルゴリズムを利用する経路探索処理のクラスです。
    /// </summary>
    public sealed class AStarPathFinder : IPathFinder
    {
        private int _width;
        private int _height;
        private Position _startPoint;
        private Position _endPoint;
        private HashSet<AStarPosition> _openList = new HashSet<AStarPosition>();
        private HashSet<AStarPosition> _closeList = new HashSet<AStarPosition>();
        private HashSet<Position> _obstacles = new HashSet<Position>();

        /// <summary>
        /// 起点から終点までの最短経路を算出します。
        /// </summary>
        /// <param name="width">区域の幅</param>
        /// <param name="height">区域の高さ</param>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="obstacles">障害物の位置の一覧</param>
        /// <returns>
        /// 起点から終点までの経路。
        /// 通過した地点を順番に返します。
        /// 経路が存在しない場合はnullを返します。
        /// </returns>
        public IEnumerable<Position> FindPath(
            int width,
            int height,
            Position startPoint,
            Position endPoint,
            ISet<Position> obstacles)
        {
            _width = width;
            _height = height;
            _startPoint = startPoint;
            _endPoint = endPoint;
            if (obstacles != null)
            {
                _obstacles = new HashSet<Position>(obstacles);
            }

            AddOpenPoint(_startPoint, null);

            return FindPath();
        }

        /// <summary>
        /// 経路を算出します。
        /// </summary>
        private IEnumerable<Position> FindPath()
        {
            AStarPosition currentPosition = null;
            // 毎回f値が一番小さいノードを_closeListに追加して、隣接するノードを_openListに追加します。
            while (!_closeList.Any(point => point.Position == _endPoint) && _openList.Count != 0)
            {
                currentPosition = GetMinLengthPosition();
                _closeList.Add(currentPosition);
                _openList.Remove(currentPosition);

                OpenSurroundingPoints(currentPosition);
            }

            if (currentPosition == null || !_closeList.Any(point => point.Position == _endPoint))
            {
                // 経路が見つからなかった場合、nullを返します。
                return null;
            }
            else
            {
                // 経路を返します。
                var path = new List<Position>();
                path.Add(currentPosition.Position);
                while (currentPosition.PreviousPosition != null)
                {
                    currentPosition = currentPosition.PreviousPosition;
                    path.Add(currentPosition.Position);
                }
                path.Reverse();

                return path;
            }

            AStarPosition GetMinLengthPosition()
            {
                var minLengthPosition = _openList.First();
                int minLength = minLengthPosition.TotalLength;
                foreach (var openPoint in _openList)
                {
                    if (openPoint.TotalLength < minLength)
                    {
                        minLength = openPoint.TotalLength;
                        minLengthPosition = openPoint;
                    }
                }

                return minLengthPosition;
            }
        }

        /// <summary>
        /// <paramref name="targetPosition"/>と隣接する追加可能のノードを<see cref="_openList"/>に追加します。
        /// </summary>
        private void OpenSurroundingPoints(AStarPosition targetPosition)
        {
            int targetPositionX = targetPosition.Position.axisX,
                targetPositionY = targetPosition.Position.axisY;

            // 隣接する追加可能なノードを_openListsに追加します。
            var nextPoint = new Position(targetPositionX - 1, targetPositionY);
            if (targetPositionX > 0 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPosition);
            }

            nextPoint = new Position(targetPositionX, targetPositionY + 1);
            if (targetPositionY < _height - 1 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPosition);
            }

            nextPoint = new Position(targetPositionX + 1, targetPositionY);
            if (targetPositionX < _width - 1 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPosition);
            }

            nextPoint = new Position(targetPositionX, targetPositionY - 1);
            if (targetPositionY > 0 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPosition);
            }
        }

        /// <summary>
        /// 対象位置にあるノードを<see cref="_openList"/>に追加します。
        /// </summary>
        private void AddOpenPoint(Position targetPosition, AStarPosition previousPosition)
        {
            int cost = (previousPosition?.Cost ?? 0) + 1,
                distance = GetDistance(targetPosition, _endPoint),
                totalLength = cost + distance;

            var nextAstarPosition = new AStarPosition(targetPosition, previousPosition, cost, distance);
            var existedClosePosition = _closeList.FirstOrDefault(position => position.Position == nextAstarPosition.Position);
            if (existedClosePosition != null)
            {
                // _closeListに存在し、f値が既存のより小さい場合、_closeListからノードを削除し、後ほど_openListに追加します。
                if (existedClosePosition.TotalLength > totalLength)
                {
                    _closeList.Remove(existedClosePosition);
                }
                else
                {
                    return;
                }
            }

            var existedOpenPosition = _openList.FirstOrDefault(position => position.Position == nextAstarPosition.Position);
            if (existedOpenPosition != null)
            {
                // _openListに存在し、f値が既存のより小さい場合、既存のノードを置き換えます。
                if (existedOpenPosition.TotalLength > totalLength)
                {
                    existedOpenPosition.Cost = cost;
                    existedOpenPosition.Distance = distance;
                    existedOpenPosition.PreviousPosition = previousPosition;
                }
                else
                {
                    return;
                }
            }
            else
            {
                _openList.Add(nextAstarPosition);
            }
        }

        /// <summary>
        /// <paramref name="position1"/>と<paramref name="position2"/>の距離を算出します。
        /// </summary>
        private static int GetDistance(Position position1, Position position2)
            => Math.Abs(position1.axisX - position2.axisX) + Math.Abs(position2.axisY - position2.axisY);
    }
}
