using System;
using System.Collections.Generic;
using System.Text;

namespace Featurefy
{
    /// <summary>
    /// 
    /// </summary>
    public class Distance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float minimumDistance(Sketch.Substroke a, Sketch.Substroke b)
        {
            return Distance.minimumDistance(a, b, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="coarseness"></param>
        /// <returns></returns>
        public static float minimumDistance(Sketch.Substroke a, Sketch.Substroke b, int coarseness)
        {
            float min = float.PositiveInfinity;
            Sketch.Point[] aPoints = a.Points;
            Sketch.Point aPoint;
            Sketch.Point[] bPoints = b.Points;
            Sketch.Point bPoint;

            int len = aPoints.Length;
            int i;
            int len2 = bPoints.Length;
            int j;

            float x, y, dist;

            for (i = 0; i < len; i += coarseness)
            {
                aPoint = aPoints[i];
                for (j = 0; j < len2; j += coarseness)
                {
                    bPoint = bPoints[j];
                    x = aPoint.X - bPoint.X;
                    y = aPoint.Y - bPoint.Y;
                    dist = Convert.ToSingle(Math.Sqrt(x * x + y * y));
                    if (dist < min)
                        min = dist;
                }
            }
            return min;    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float minXDistance(Sketch.Substroke a, Sketch.Substroke b)
        {
            float min = float.PositiveInfinity;
            Sketch.Point[] aPoints = a.Points;
            Sketch.Point aPoint;
            Sketch.Point[] bPoints = b.Points;
            Sketch.Point bPoint;

            int len = aPoints.Length;
            int i;
            int len2 = bPoints.Length;
            int j;

            float dist;

            for (i = 0; i < len; i++)
            {
                aPoint = aPoints[i];
                for (j = 0; j < len2; j++)
                {
                    bPoint = bPoints[j];
                    dist = Math.Abs(aPoint.X - bPoint.X);
                    if (dist < min)
                        min = dist;
                }
            }
            return min;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float minYDistance(Sketch.Substroke a, Sketch.Substroke b)
        {
            float min = float.PositiveInfinity;
            Sketch.Point[] aPoints = a.Points;
            Sketch.Point aPoint;
            Sketch.Point[] bPoints = b.Points;
            Sketch.Point bPoint;

            int len = aPoints.Length;
            int i;
            int len2 = bPoints.Length;
            int j;

            float dist;

            for (i = 0; i < len; i++)
            {
                aPoint = aPoints[i];
                for (j = 0; j < len2; j++)
                {
                    bPoint = bPoints[j];
                    dist = Math.Abs(aPoint.Y - bPoint.Y);
                    if (dist < min)
                        min = dist;
                }
            }
            return min;
        }

        public static float minimumDistance(Sketch.Substroke a, List<Sketch.Substroke> subs, int coarseness)
        {
            float dist, min = float.PositiveInfinity;
            int i, len = subs.Count;
            Sketch.Substroke sub;
            for (i = 0; i < len; ++i)
            {
                dist = minimumDistance(a, subs[i], coarseness);
                if (dist < min)
                    min = dist;
            }
            return min;
        }

        public static float minimumDistance(Sketch.Substroke a, List<Sketch.Substroke> subs)
        {
            return minimumDistance(a, subs, 1);
        }
    }
}
