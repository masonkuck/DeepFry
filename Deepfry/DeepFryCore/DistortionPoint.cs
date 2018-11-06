using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepFryCore
{
    [DebuggerDisplay("XDistance = {XDistance}, YDistance = {YDistance}, Distance = {DistanceToOrigin}")]
    public class DistortionPoint
    {
        public Point PointBacker;

        public int X { get { return PointBacker.X; } set { PointBacker.X = value; } }

        public int Y { get { return PointBacker.Y; } set { PointBacker.Y = value; } }

        public int OriginX { get; set; }

        public int OriginY { get; set; }

        public double XDistance { get { return X - OriginX; } }

        public double YDistance { get { return Y - OriginY; } }

        public double DistanceToOrigin { get { return Math.Pow(XDistance, 2) + Math.Pow(YDistance, 2); } }


        public DistortionPoint(int x, int y, int originX, int originY)
        {
            PointBacker = new Point();

            X = x;
            Y = y;
            OriginX = originX;
            OriginY = originY;
        }


    }
}
