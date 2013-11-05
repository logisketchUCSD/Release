using System;
using Sketch;

namespace Featurefy
{
	/// <summary>
	/// Spatial class. Creates spatial information for the Points
	/// given, such as the average point.
	/// </summary>
	public class Spatial
	{		
		#region INTERNALS

		/// <summary>
		/// The Points to calculate spatial information for.
		/// </summary>
		private Point[] points;

		/// <summary>
		/// The bounding box for the given Points.
		/// </summary>
		private BoundingBox boundingBox;
		
		/// <summary>
		/// The average, or center point of the set of Points.
		/// </summary>
		private Point averagePoint;

        /// <summary>
        /// The weighted average point, see An image-based, trainable symbol recognizer for more
        /// </summary>
        private Point weightedAveragePoint;

        /// <summary>
        /// Length of the points (like arclength, but this is calculated while also finding the averagepoint or weightedaverage)
        /// </summary>
        private float? length;

		#endregion

		#region CONSTRUCTORS

		/// <summary>
		/// Computes some Spatial spatial information from the given
		/// set of Points.
		/// </summary>
		/// <param name="points"></param>
		public Spatial(Point[] points)
		{
			this.points = points;
			this.boundingBox = null;
			this.averagePoint = null;
            this.weightedAveragePoint = null;
		}
		
		#endregion

		#region COMPUTATIONS

		/// <summary>
		/// Finds the average Point for the current set of Points.
		/// </summary>
		/// <returns>A Point that is the average of the current Points</returns>
		private Point computeAveragePoint()
		{			
			double totalX     = 0.0;
			double totalY     = 0.0;
			ulong totalTime	  = 0;
			int totalPressure = 0;
			
			int length = this.points.Length;
			for (int i = 0; i < length; ++i)
			{
				// Sum the X coordinates
				totalX += (float)this.points[i].XmlAttrs.X;
				
				// Sum the Y coordinates
				totalY += (float)this.points[i].XmlAttrs.Y;
				
				// Sum the Time information
				totalTime += (ulong)this.points[i].XmlAttrs.Time;
				
				// Sum the Pressure information
				totalPressure += (ushort)this.points[i].XmlAttrs.Pressure;
			}

			Point avgPoint = new Point();

			// Do we need this?
			avgPoint.XmlAttrs.Id   = System.Guid.NewGuid();
			avgPoint.XmlAttrs.Name = "average point";
			
			// Averages
            avgPoint.XmlAttrs.X		   = Convert.ToSingle(totalX / length);
			avgPoint.XmlAttrs.Y		   = Convert.ToSingle(totalY / length);
			avgPoint.XmlAttrs.Time	   = totalTime / Convert.ToUInt64(length);
			avgPoint.XmlAttrs.Pressure = Convert.ToUInt16(totalPressure / length);

			return avgPoint;
		}


        /// <summary>
        /// See An image-based, trainable symbol recognizer for hand-drawn sketches for more on this
        /// </summary>
        /// <returns></returns>
        private Point computeWeightedAveragePoint()
        {
            Point center;
            this.length = 0.0f;

            int i, len = this.points.Length;
            if (len == 1)
            {
                center = new Point();
                center.XmlAttrs.X = this.points[0].X;
                center.XmlAttrs.Y = this.points[0].Y;
                return center;
            }

            ulong XL = 0, YL = 0;
            float x, y, dx, dy, dl, L = 0.0f;
            for (i = 0; i < len - 1; ++i)
            {
                //Find the center of the segment
                x = (this.points[i].X + this.points[i + 1].X) / 2;
                y = (this.points[i].Y + this.points[i + 1].Y) / 2;

                //Find the difference
                dx = this.points[i].X - this.points[i + 1].X;
                dy = this.points[i].Y - this.points[i + 1].Y;

                dl = (float)Math.Sqrt(Math.Pow(dx, 2.0) + Math.Pow(dy, 2.0));
                this.length += dl;

                XL += (ulong)(x * dl);
                YL += (ulong)(y * dl);
                L += dl;
            }

            center = new Point();
            center.XmlAttrs.X = XL / L;
            center.XmlAttrs.Y = YL / L;
            return center;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float computeLength()
        {
            computeWeightedAveragePoint();
            return this.length.Value;
        }


		/// <summary>
		/// Calculates the Euclidean distance between two points
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private double distance(Point a, Point b)
		{
			float x1 = a.X;
			float y1 = a.Y;
			float x2 = b.X;
			float y2 = b.Y;
	
			return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
		}

		#endregion

		#region GETTERS & SETTERS

		/// <summary>
		/// Returns a Point consisting of the averages of the set of Points.
		/// Averages the X, Y, Time, and Pressure.
		/// </summary>
		public Point AveragePoint
		{
			get
			{
				if (this.averagePoint == null)
				{
					this.averagePoint = computeAveragePoint();
				}

				return this.averagePoint;
			}
		}

        /// <summary>
        /// Returns a Point consisting of the weighted average of the set of Points.
        /// Weighted average X, Y
        /// </summary>
        public Point WeightedAveragePoint
        {
            get
            {
                if (this.weightedAveragePoint == null)
                {
                    this.weightedAveragePoint = computeWeightedAveragePoint();
                }

                return this.weightedAveragePoint;
            }
        }

        /// <summary>
        /// Arc length
        /// </summary>
        public float Length
        {
            get
            {
                if (this.length == null)
                {
                    this.length = computeLength(); 
                }
                return this.length.Value;
            }
        }
		
		/// <summary>
		/// Returns the first Point in the set.
		/// </summary>
		public Point FirstPoint
		{
			get
			{
				return this.points[0];
			}
		}

		
		/// <summary>
		/// Returns the last Point in the set.
		/// </summary>
		public Point LastPoint
		{
			get
			{
				return this.points[this.points.Length - 1];
			}
		}


		/// <summary>
		/// Returns the distance from the first point to the last point.
		/// </summary>
		public double DistanceFromFirstToLast
		{
			get
			{
				return distance(FirstPoint, LastPoint);
			}
		}


		/// <summary>
		/// Returns the upper-left corner for the points.
		/// </summary>
		public System.Drawing.PointF UpperLeft
		{
			get
			{
                if (this.boundingBox == null)
                {
                    this.boundingBox = new BoundingBox(points);
                }
				return this.boundingBox.UpperLeft;
			}
		}

		/// <summary>
		/// Returns the upper-left corner for the points.
		/// </summary>
		public System.Drawing.PointF LowerRight
		{
			get
			{
                if (this.boundingBox == null)
                {
                    this.boundingBox = new BoundingBox(points);
                }
				return this.boundingBox.LowerRight;
			}
		}

		#endregion

		#region PRIVATE CLASS - BOUNDING BOX

		/// <summary>
		/// Stores BoundingBox information for Sketch.Points in the form of an upper-left
		/// corner and a lower-right corner
		/// </summary>
		private class BoundingBox
		{
			#region INTERNALS

			/// <summary>
			/// Stores the upper-left corner of the bounding box
			/// </summary>
			private System.Drawing.PointF upperLeft;
		
			/// <summary>
			/// Stores the lower-right corner of the bounding box
			/// </summary>
			private System.Drawing.PointF lowerRight;

			#endregion

			#region CONSTRUCTOR

			/// <summary>
			/// Constructor. Calculates a bounding box for the given set of points, and sets the
			/// upperLeft and lowerRight variables to the corresponding bounding box values.
			/// </summary>
			/// <param name="points">Points of a (sub)stroke</param>
			public BoundingBox(Point[] points)
			{
				// Initialize temp variables for the x-coordinates of the bounding box
				float xl = points[0].X;
				float xr = xl;

				// Initialize temp variables for the y-coordinates of the bounding box
				float yt = points[0].Y;
				float yb = yt;

				// Jump through the points quickly, obtaining a rough bounding box of the stroke
				int skip = 5;

				// Go through the points
				for (int i = 1; i < points.Length; i += skip)
				{
					float currX = points[i].X;
					float currY = points[i].Y;

					if (currX < xl)
						xl = currX;
					else if (currX > xr)
						xr = currX;

					if (currY < yt)
						yt = currY;
					else if (currY > yb)
						yb = currY;
				}

				// Set our bounding box variables
				this.upperLeft = new System.Drawing.PointF(xl, yt);
				this.lowerRight = new System.Drawing.PointF(xr, yb);
			}

			#endregion

			#region GETTERS & SETTERS

			/// <summary>
			/// Returns the upper-left corner of the bounding box.
			/// </summary>
			public System.Drawing.PointF UpperLeft
			{
				get
				{
					return this.upperLeft;
				}
			}


			/// <summary>
			/// Returns the lower-right corner of the bounding box.
			/// </summary>
			public System.Drawing.PointF LowerRight
			{
				get
				{
					return this.lowerRight;
				}
			}

			#endregion
		}

		#endregion
	}


	
}
