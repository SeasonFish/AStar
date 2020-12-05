using System;
using System.Collections.Generic;
using System.Linq;

namespace AStar
{
    /// <summary>
    /// 位置情報を保持する構造体です。
    /// </summary>
    public struct Position
    {
        public int axisX;
        public int axisY;

        public Position(int axisX, int axisY)
        {
            this.axisX = axisX;
            this.axisY = axisY;
        }

        public static bool operator ==(Position position1, Position position2)
            => position1.Equals(position2);

        public static bool operator !=(Position position1, Position position2)
            => !position1.Equals(position2);

        public override bool Equals(object obj)
        {
            return obj is Position otherPosition
                && axisX == otherPosition.axisX
                && axisY == otherPosition.axisY;
        }
    }

    /// <summary>
    /// AStarアルゴリズムを利用する経路探索処理のクラスです。
    /// </summary>
    public static class AStarPathFinder
    {
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
        public static IEnumerable<Position> FindPath(
            int width,
            int height,
            Position startPoint,
            Position endPoint,
            ISet<Position> obstacles)
            => FindPathRecursively(width, height, startPoint, endPoint, obstacles ?? new HashSet<Position>());


        /// <summary>
        /// 再帰的に経路を探索します。
        /// </summary>
        /// <param name="width">区域の幅</param>
        /// <param name="height">区域の高さ</param>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="closedPoints">通過不可の地点の一覧</param>
        /// <param name="path">経路</param>
        /// <returns>終点に辿れる場合は true 、そうでない場合は false を返します。</returns>
        private static IEnumerable<Position> FindPathRecursively(
            int width,
            int height,
            Position startPoint,
            Position endPoint,
            ISet<Position> closedPoints)
        {

            if (startPoint == endPoint)
            {
                return new[] { endPoint };
            }

            int currentX = startPoint.axisX, currentY = startPoint.axisY;
            if (currentX < 0 || currentX >= width
                || currentY < 0 || currentY >= height
                || closedPoints.Contains(startPoint))
            {
                return null;
            }

            closedPoints.Add(startPoint);

            var nextPath = FindPathRecursively(width, height, new Position(currentX - 1, currentY), endPoint, closedPoints);
            if (nextPath != null)
            {
                return new[] { startPoint }.Concat(nextPath);
            }

            nextPath = FindPathRecursively(width, height, new Position(currentX, currentY + 1), endPoint, closedPoints);
            if (nextPath != null)
            {
                return new[] { startPoint }.Concat(nextPath);
            }

            nextPath = FindPathRecursively(width, height, new Position(currentX + 1, currentY), endPoint, closedPoints);
            if (nextPath != null)
            {
                return new[] { startPoint }.Concat(nextPath);
            }

            nextPath = FindPathRecursively(width, height, new Position(currentX, currentY - 1), endPoint, closedPoints);
            if (nextPath != null)
            {
                return new[] { startPoint }.Concat(nextPath);
            }

            return null;
        }
    }
}
