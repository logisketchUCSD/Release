/*
 * File: Point.cs
 *
 * Authors: Aaron Wolin, Devin Smith, Jason Fennell, and Max Pflueger.
 * Harvey Mudd College, Claremont, CA 91711.
 * Sketchers 2006.
 * 
 * Use at your own risk.  This code is not maintained and not guaranteed to work.
 * We take no responsibility for any harm this code may cause.
 */

using System;

namespace Sketch
{
	/// <summary>
	/// Point class.
	/// </summary>
	public class Point : IComparable<Point>
	{
		#region INTERNALS

		/// <summary>
		/// The XML attributes of the Point
		/// </summary>
		public XmlStructs.XmlPointAttrs XmlAttrs;

        // fields for determining endpoint type
        //private string type;
        //ArrayList wireps, line;
        //private double A, B, C, m, b;
		
		#endregion

		#region CONSTRUCTORS
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Point()
			: this(new XmlStructs.XmlPointAttrs())
		{
			//Calls the main constructor
		}

		/// <summary>
		/// Construct a new Point class from an existing Point.
		/// </summary>
		/// <param name="point">Existing Point to copy</param>
		public Point(Point point)
			: this(point.XmlAttrs.Clone())
		{
			// Calls the main constructor
		}
		
		
		/// <summary>
		/// Construct a new Point class from XML attributes.
		/// </summary>
		/// <param name="XmlAttrs">The XML attributes of the Point</param>
		public Point(XmlStructs.XmlPointAttrs XmlAttrs)
		{
			this.XmlAttrs = XmlAttrs;
		}
		
		#endregion

		#region GETTERS & SETTERS

		/// <summary>
		/// A getter for the x-coordinate of the Point.
		/// This is created so we can bypass having to go into the XML Attributes,
		/// because (x,y) coordinates are more closely tied to actual sketch and stroke
		/// attributes than the XML format.
		/// </summary>
		public float X
		{
			get
			{
                return this.XmlAttrs.X.Value; //(float)this.XmlAttrs.X;
			}
		}

		
		/// <summary>
		/// A getter for the y-coordinate of the Point.
		/// This is created so we can bypass having to go into the XML Attributes,
		/// because (x,y) coordinates are more closely tied to actual sketch and stroke
		/// attributes than the XML format.
		/// </summary>
		public float Y
		{
			get
			{
                return this.XmlAttrs.Y.Value;// (float)this.XmlAttrs.Y;
			}
		}


		/// <summary>
		/// Get the ulong Time of this Point.
		/// </summary>
		public ulong Time
		{
			get
			{
                return this.XmlAttrs.Time.Value;// (ulong)this.XmlAttrs.Time;
			}
		}


		/// <summary>
		/// Get the ushort Pressure of this Point
		/// </summary>
		public ushort Pressure
		{
			get
			{
                return this.XmlAttrs.Pressure.Value;// (ushort)this.XmlAttrs.Pressure;
			}
		}

        /*public string getType()
        {
            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double getSlope()
        {
            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double getOffset()
        {
            return b;
        }*/
		

		#endregion

		#region OTHER

		/// <summary>
		/// Clone this Point.
		/// </summary>
		/// <returns>A Cloned point.</returns>
		public Point Clone()
		{
			return new Point(this);
		}


		/// <summary>
		/// Compare this Point to another based on time.
		/// Returns less than 0 if this time is less than the other's.
		/// Returns 0 if this time is equal to the other's.
		/// Returns greater than 0 if this time is greater than the other's.
		/// </summary>
		/// <param name="point">The other Point to compare this one to</param>
		/// <returns>An integer indicating how the Point times compare</returns>
        int System.IComparable<Point>.CompareTo(Point point)
        {
            return (int)(this.Time - point.Time);
        }


		#endregion

        #region ENDPOINT DETERMINATION

        /*public void determineSlope(Substroke substroke)
        {
            wireps = new ArrayList(substroke.PointsAL);
            wireps.Sort();
            int index = wireps.IndexOf((Point)this);
            if (index < wireps.Count / 2)
            {
                line = new ArrayList(wireps.GetRange(0, wireps.Count / 5));
            }
            else
            {
                line = new ArrayList(wireps.GetRange(wireps.Count - wireps.Count / 5, wireps.Count / 5));
            }

            // Find the least squares fit to the points near the endpoint
            

            leastSquares();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void determineType(Point other)
        {
            if (Math.Abs(this.m) < 0.01)
            {
                if (this.X <= other.X)
                {
                    this.type = "left";
                }
                else
                {
                    this.type = "right";
                }
            }
            else if (Math.Abs(this.m) > 4)
            {
                if (this.Y <= other.Y)
                {
                    this.type = "top";
                }
                else
                {
                    this.type = "bottom";
                }
            }
            // Both X and Y coordinate will be smaller or larger for diagonal lines, so only need to check one
            else if ((this.m >= -4) && (this.m <= -0.01))
            {
                if (this.X <= other.X)
                {
                    this.type = "topleft";
                }
                else
                {
                    this.type = "bottomright";
                }
            }
            else
            {
                if (this.X <= other.X)
                {
                    this.type = "bottomleft";
                }
                else
                {
                    this.type = "topright";
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        public void leastSquares()
        {
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXX = 0.0;
            double sumYY = 0.0;
            double sumXY = 0.0;

            double sumDist = 0.0;

            //Calculate the sums
            foreach (Point p in line)
            {
                double currX = p.X;
                double currY = p.Y;
                sumX += currX;
                sumXX += (currX * currX);
                sumY += currY;
                sumYY += (currY * currY);
                sumXY += (currX * currY);
            }

            //denominator
            double denom = ((double)line.Count * sumXX) - (sumX * sumX);
            // make sure no divide by zero
            if (denom != 0)
            {
                //slope
                m = (double)((line.Count * sumXY) - (sumX * sumY)) / denom;
                //shift
                b = (double)((sumY * sumXX) - (sumX * sumXY)) / denom;


                foreach (Point p in line)
                {
                    double y = (m * p.X) + b;
                    B = 1.0;
                    A = (-1.0 * m) * B;
                    C = (-1.0 * b) / B;

                    double d = Math.Abs((A * p.X) + (B * p.Y) + C) / Math.Sqrt((A * A) + (B * B));

                    sumDist += d;
                }
            }
            else
            {
                m = Double.PositiveInfinity;
                b = 0.0;

                double avgX = 0.0;
                foreach (Point p in line)
                {
                    avgX += p.X;
                }

                avgX /= (double)(line.Count - 1);

                foreach (Point p in line)
                {
                    sumDist += Math.Abs(p.X - avgX) / Math.Sqrt(avgX);
                }

            }
        }*/

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string  ToString()
        {
            return "(" + this.X + ", " + this.Y + ")";
        }

        /// <summary>
        /// This method overrides the System.Equals method, and compares the two points based on the time
        /// rather than whether or not they point to the same object.
        /// </summary>
        /// <param name="obj">Another Point object</param>
        /// <returns>A boolean value whether or not the two Points are the same</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) 
                return false;

            return Equals((Point)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Equals(Point point)
        {
            //return this.Time.Equals(point.Time);
            return this.XmlAttrs.Id.Equals(point.XmlAttrs.Id);
        }
        
        /// <summary>
        /// This method was required to override the .Equals method
        /// </summary>
        /// <returns>The hash code of the base (?)</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void rotate(double theta, float xCenter, float yCenter)
        {
            /*Matrix multiplication for rotation about a center (xc, yc)
             *    
             * | X'| = |cos(theta) -sin(theta)| |x - xc| + |xc|
             * | Y'|   |sin(theta)  cos(theta)| |y - yc|   |yc|
             *   
             */
            double dx = this.XmlAttrs.X.Value - xCenter;
            double dy = this.XmlAttrs.Y.Value - yCenter;
            double sin = Math.Sin(theta);
            double cos = Math.Cos(theta);

            this.XmlAttrs.X = (float)(dx * cos - dy * sin + xCenter);
            this.XmlAttrs.Y = (float)(dx * sin + dy * cos + yCenter);
        }

        public Point cloneRotate(double theta, float xCenter, float yCenter)
        {
            Point clone = this.Clone();
            clone.rotate(theta, xCenter, yCenter);
            return clone;
        }
	}
}
