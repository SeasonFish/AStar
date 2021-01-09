using System;
using System.Collections.Generic;
using System.Text;

namespace AStar
{
    public interface IPathFinder
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
        IEnumerable<Position> FindPath(
            int width,
            int height,
            Position startPoint,
            Position endPoint,
            ISet<Position> obstacles);
    }
}
