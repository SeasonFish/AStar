using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AStar
{
    /// <summary>
    /// AStar経路探索処理のテストをまとめるクラスです。
    /// </summary>
    [TestClass]
    public sealed class AStarPathFinderTest
    {
        /// <summary>
        /// 5x5の障害物なしの区域でのテストを行います。
        /// </summary>
        [TestMethod]
        public void Size5x5_WithoutObstacles()
        {
            int width = 5, height = 5;
            Position startPoint = new Position(0, 0), endPoint = new Position(4, 4);

            PathFinderTest(width, height, startPoint, endPoint, null, 9);
        }

        /// <summary>
        /// 5x5の障害物なしの、起点と終点が逆転する区域でのテストを行います。
        /// </summary>
        [TestMethod]
        public void Size5x5_WithoutObstacles_Reverse()
        {
            int width = 5, height = 5;
            Position startPoint = new Position(4, 4), endPoint = new Position(0, 0);

            PathFinderTest(width, height, startPoint, endPoint, null, 9);
        }

        /// <summary>
        /// 5x5の障害物ありの区域でのテストを行います。
        /// </summary>
        [TestMethod]
        public void Size5x5_WithObstacles()
        {
            int width = 5, height = 5;
            Position startPoint = new Position(0, 0), endPoint = new Position(4, 4);
            var obstacles = new HashSet<Position>(new[]{
                new Position(2,3),
                new Position(2,4),
                new Position(4,3),
            });

            PathFinderTest(width, height, startPoint, endPoint, obstacles, 9);
        }

        /// <summary>
        /// 5x5の区域で、終点に辿りつけないテストを行います。
        /// </summary>
        [TestMethod]
        public void Size5x5_NoAccess()
        {
            int width = 5, height = 5;
            Position startPoint = new Position(0, 0), endPoint = new Position(4, 4);
            var obstacles = new HashSet<Position>(new[]{
                new Position(3,3),
                new Position(3,4),
                new Position(4,3),
            });

            PathFinderTest(width, height, startPoint, endPoint, obstacles, 0);
        }

        /// <summary>
        /// 指定した入力情報で経路探索処理を実行し、その結果を確認します。
        /// </summary>
        /// <param name="width">区域の幅</param>
        /// <param name="height">区域の高さ</param>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="obstacles">障害物の位置の一覧</param>
        /// <param name="expectedPathLength">
        /// 経路の長さ。
        /// 終点にたどり着けない場合は0を渡してください。
        /// </param>
        private static void PathFinderTest(
            int width,
            int height,
            Position startPoint,
            Position endPoint,
            ISet<Position> obstacles,
            int expectedPathLength)
        {
            Position[] path = null;
            try
            {
                path = new SimpleRecursivePathFinder().FindPath(width, height, startPoint, endPoint, obstacles)?.ToArray();
            }
            catch (Exception e)
            {
                Assert.Fail(e.StackTrace + e.Message);
            }

            // 終点にたどり着けるかを確認します。
            var canReachEndPoint = expectedPathLength != 0;
            Assert.AreEqual(canReachEndPoint, path != null);
            if (!canReachEndPoint)
            {
                return;
            }

            // 経路が最短かを確認します。
            Assert.AreEqual(expectedPathLength, path.Length);

            // 起点が入力情報と一致するかを確認します。
            var lastPosition = path[0];
            Assert.AreEqual(startPoint, lastPosition);

            for (int index = 1; index < path.Length; index++)
            {
                var currentPosition = path[index];
                int currentPositionX = currentPosition.axisX,
                    currentPositionY = currentPosition.axisY,
                    distanceX = Math.Abs(currentPositionX - lastPosition.axisX),
                    distanceY = Math.Abs(currentPositionY - lastPosition.axisY);

                if (obstacles != null)
                {
                    // 障害物のある所を通過していないかを確認します。
                    Assert.IsFalse(obstacles.Contains(currentPosition));
                }

                // 経路が連続しているかを確認します。
                Assert.IsTrue(currentPositionX >= 0 && currentPositionX < width);
                Assert.IsTrue(currentPositionY >= 0 && currentPositionY < height);
                Assert.IsTrue(distanceX == 0 && distanceY == 1 || distanceX == 1 && distanceY == 0);

                lastPosition = currentPosition;
            }

            // 終点が入力情報と一致するかを確認します。
            Assert.AreEqual(endPoint, lastPosition);
        }
    }
}
