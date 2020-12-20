using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    /// <summary>
    /// A*アルゴリズム用の位置情報を保持するクラスです。
    /// </summary>
    public class AStarPosition
    {
        internal Position Position { get; set; }
        internal AStarPosition PreviousPosition { get; set; }
        internal int Cost { get; set; }
        internal int Distance { get; set; }
        internal int TotalLength { get; set; }

        public AStarPosition(Position position, AStarPosition previousPosition, int cost, int distance, int totalLength)
        {
            this.Position = position;
            this.PreviousPosition = previousPosition;
            this.Cost = cost;
            this.Distance = distance;
            this.TotalLength = totalLength;
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
        private HashSet<AStarPosition> _openPoints = new HashSet<AStarPosition>();
        private HashSet<AStarPosition> _closePoints = new HashSet<AStarPosition>();
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

        private IEnumerable<Position> FindPath()
        {
            AStarPosition currentPosition = null;
            while (!_closePoints.Any(point => point.Position == _endPoint) && _openPoints.Count != 0)
            {
                currentPosition = GetMinLengthPosition();
                _closePoints.Add(currentPosition);
                _openPoints.Remove(currentPosition);

                OpenSurroundingPoints(currentPosition);
            }

            if (currentPosition == null || !_closePoints.Any(point => point.Position == _endPoint))
            {
                return null;
            }
            else
            {
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
                var minLengthPosition = _openPoints.First();
                int minLength = minLengthPosition.TotalLength;
                foreach (var openPoint in _openPoints)
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

        private void OpenSurroundingPoints(AStarPosition targetPoint)
        {
            int targetPointX = targetPoint.Position.axisX,
                targetPointY = targetPoint.Position.axisY;

            var nextPoint = new Position(targetPointX - 1, targetPointY);
            if (targetPointX > 0 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPoint);
            }

            nextPoint = new Position(targetPointX, targetPointY + 1);
            if (targetPointY < _height - 1 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPoint);
            }

            nextPoint = new Position(targetPointX + 1, targetPointY);
            if (targetPointX < _width - 1 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPoint);
            }

            nextPoint = new Position(targetPointX, targetPointY - 1);
            if (targetPointY > 0 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint(nextPoint, targetPoint);
            }
        }

        private void AddOpenPoint(Position targetPosition, AStarPosition previousPosition)
        {
            int cost = (previousPosition?.Cost ?? 0) + 1,
                distance = GetDistance(targetPosition, _endPoint),
                totalLength = cost + distance;

            var nextAstarPoint = new AStarPosition(targetPosition, previousPosition, GetDistance(targetPosition, _startPoint), GetDistance(targetPosition, _endPoint), totalLength);
            var existedClosePoint = _closePoints.FirstOrDefault(position => position.Position == nextAstarPoint.Position);
            if (existedClosePoint != null)
            {
                if (existedClosePoint.TotalLength > totalLength)
                {
                    _closePoints.Remove(existedClosePoint);
                }
                else
                {
                    return;
                }
            }

            var existedOpenPoint = _openPoints.FirstOrDefault(position => position.Position == nextAstarPoint.Position);
            if (existedOpenPoint != null)
            {
                if (existedOpenPoint.TotalLength > totalLength)
                {
                    existedOpenPoint.TotalLength = totalLength;
                    existedOpenPoint.PreviousPosition = previousPosition;
                }
                else
                {
                    return;
                }
            }
            else
            {
                _openPoints.Add(nextAstarPoint);
            }
        }

        private static int GetDistance(Position position1, Position position2)
            => Math.Abs(position1.axisX - position2.axisX) + Math.Abs(position2.axisY - position2.axisY);
    }
}
