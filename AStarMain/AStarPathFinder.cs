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
            => position1.Equals(position2);

        public static bool operator !=(AStarPosition position1, AStarPosition position2)
            => !position1.Equals(position2);

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

            return null;
        }

        private void OpenSurroundingPoints(AStarPosition targetPoint)
        {
            int targetPointX = targetPoint.Position.axisX,
                targetPointY = targetPoint.Position.axisY;

            var nextPoint = new Position(targetPointX - 1, targetPointY);
            if (targetPointX > 0 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint();
            }

            nextPoint = new Position(targetPointX, targetPointY + 1);
            if (targetPointY < _height - 1 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint();
            }

            nextPoint = new Position(targetPointX + 1, targetPointY);
            if (targetPointX < _width - 1 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint();
            }

            nextPoint = new Position(targetPointX, targetPointY - 1);
            if (targetPointY > 0 && !_obstacles.Contains(nextPoint))
            {
                AddOpenPoint();
            }

            void AddOpenPoint()
            {
                int cost = GetDistance(nextPoint, _startPoint),
                    distance = GetDistance(nextPoint, _endPoint),
                    totalLength = cost + distance;

                var nextAstarPoint = new AStarPosition(nextPoint, targetPoint, GetDistance(nextPoint, _startPoint), GetDistance(nextPoint, _endPoint), totalLength);
                var existedClosePoint = _closePoints.FirstOrDefault(position =>
                    position.Position == nextAstarPoint.Position && position.TotalLength > totalLength);
                if (existedClosePoint != null)
                {
                    _closePoints.Remove(existedClosePoint);
                }

                var existedOpenPoint = _openPoints.FirstOrDefault(position =>
                    position.Position == nextAstarPoint.Position && position.TotalLength > totalLength);
                if (existedOpenPoint != null)
                {
                    existedOpenPoint.TotalLength = totalLength;
                    existedOpenPoint.PreviousPosition = targetPoint;
                }
                else
                {
                    _openPoints.Add(nextAstarPoint);
                }
            }
        }

        private static int GetDistance(Position position1, Position position2)
            => Math.Abs(position1.axisX - position2.axisX) + Math.Abs(position2.axisY - position2.axisY);
    }
}
