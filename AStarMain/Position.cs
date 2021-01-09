using System;
using System.Collections.Generic;
using System.Text;

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

        public override int GetHashCode() => $"{axisX},{axisY}".GetHashCode();
    }
}
