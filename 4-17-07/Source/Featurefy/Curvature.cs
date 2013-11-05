using System;
using Sketch;

namespace Featurefy
{
	/// <summary>
	/// Curvature class. Creates curvature profiles for the Points
	/// given, where each point can have a curvature in degrees/pixel.
	/// </summary>
	public class Curvature : LeastSquares
	{
		#region INTERNALS
		
		/// <summary>
		/// The Points to calculate curvature information for
		/// </summary>
		private Point[] points;

		/// <summary>
		/// The arc length profile for the set of Points
		/// </summary>
		private double[] arcLengthProfile;

		/// <summary>
		/// The tangent angle profile for the set of Points
		/// </summary>
		private double[] tanAngleProfile;

		/// <summary>
		/// The average (absolute value) curvature for the set of Points
		/// </summary>
		private double avgCurv;

		/// <summary>
		/// The minimum (absolute value) curvature for the set of Points
		/// </summary>
		private double minCurv;

		/// <summary>
		/// The maximum (absolute value) curvature for the set of Points
		/// </summary>
		private double maxCurv;

		/// <summary>
		/// The (un-normalized) curvature profile for the set of Points
		/// </summary>
		private double[] curvProfile;

		/// <summary>
		/// The normalized curvature profile for the set of Points
		/// </summary>
		private double[] normCurvProfile;

		/// <summary>
		/// Constant for a small end point window
		/// </summary>
		private const int SMALL_WINDOW = 12;

		/// <summary>
		/// Constant for a large end point window
		/// </summary>
		private const int LARGE_WINDOW = 15;
		
		#endregion

		#region CONSTRUCTOR

		/// <summary>
		/// Creates the curvature information for a given set of Points.
		/// The computations are lazy, and variables are only initialized
		/// when called.
		/// </summary>
		/// <param name="points">Given Points</param>
		/// <param name="arcLengthProfile">The arc length profile for the given Points</param>
		/// <param name="tanAngleProfile">The tangent angle profile for the given Points</param>
		public Curvature(Point[] points, double[] arcLengthProfile, double[] tanAngleProfile)
		{
			this.points = points;
			this.arcLengthProfile = arcLengthProfile;
			this.tanAngleProfile = tanAngleProfile;

			this.curvProfile = null;
			this.normCurvProfile = null;
			this.avgCurv = -1.0;
			this.minCurv = Double.PositiveInfinity;
			this.maxCurv = Double.NegativeInfinity;
		}

		#endregion

		#region CURVATURE COMPUTATIONS

		/// <summary>
		/// Calculates the curvature of a point at a given index. Curvature is equal to
		/// the LSQ Line slope of the tangent angle vs. arc length
		/// </summary>
		/// <param name="index">The index of the point to calculate curvature for</param>
		/// <returns>The curvature of the point</returns>
		private double curvature(int index)
		{
			// Use the points between (index - window) and (index + window)
			int window = 5;
			
			// Indices for the points on the stroke
			int startIndex = index - window;
			int endIndex = index + window;
			
			// Fix the start and end indices so that they are not out of bounds
			if (startIndex < 0)
				startIndex = 0;
			if (endIndex > this.points.Length - 1)
				endIndex = this.points.Length - 1;

			System.Drawing.PointF[] pts = new System.Drawing.PointF[endIndex - startIndex + 1];

			for (int i = 0; i < pts.Length; i++)
			{
				pts[i] = new System.Drawing.PointF((float)this.arcLengthProfile[startIndex + i], 
					(float)this.tanAngleProfile[startIndex + i] );
			}

			double m, b;
			double err = leastSquaresLineFit(pts, out m, out b);

			// Convert from radians / pixel to degrees / pixel
			if (!Double.IsPositiveInfinity(m))
				m = m * (180 / Math.PI);
			else
				return -1.0;

			// Absolute value the slope, since we want to know the magnitude of the curvature and not
			// the direction
			m = Math.Abs(m);

			// Set the min and max speed values if they have changed
			if (m < this.minCurv)
				this.minCurv = m;
			if (m > this.maxCurv && !Double.IsPositiveInfinity(m))
				this.maxCurv = m;

			// Slope of the tangent line
			return m;
		}


		/// <summary>
		/// Compute the average curvature of the set of Points.
		/// Does not take into account a certain number of Points on each end, since these
		/// sections are known to contain hooks.
		/// </summary>
		/// <returns>The average curvature</returns>
		private double averageCurvature()
		{
			double avgCurv = 0.0;

			// Don't count any points that were at the ends of the stroke
			int endPtWindow;
			if (this.points.Length > 20 && this.points.Length < 60)
				endPtWindow = SMALL_WINDOW;
			else if (this.points.Length >= 60)
				endPtWindow = LARGE_WINDOW;
			else
				endPtWindow = 0;

			for (int i = endPtWindow; i < Profile.Length - endPtWindow; i++)
			{
				avgCurv += Profile[i];
			}

			return avgCurv / ((double)Profile.Length - (2 * endPtWindow));
		}

		
		/// <summary>
		/// Computes the curvature at each Point and creates a double array of it all.
		/// </summary>
		/// <returns>The double array of curvature values</returns>
		private double[] calcCurvProfile()
		{
			double[] profile = new double[this.points.Length];
			
			for (int i = 0; i < profile.Length; i++)
			{
				double currCurvature = curvature(i);
				
				if (currCurvature == -1.0)
				{
					if (i > 0)
						profile[i] = profile[i - 1];
					else
						profile[i] = 0.0;
				}
				else
					profile[i] = currCurvature;
			}

			//return profile;
			return smoothFilter(profile);
		}

		
		/// <summary>
		/// Computes the normalized curvature at each point and creates a double 
		/// array of it all.
		/// </summary>
		/// <returns>The double array of curvature values</returns>
		private double[] calcNormCurvProfile()
		{
			double[] profile = new double[this.points.Length];
			
			for (int i = 0; i < profile.Length; i++)
			{
				profile[i] = Profile[i] / AverageCurvature;
			}

			return profile;
		}


		private double[] smoothFilter(double[] profile)
		{
			double[] smoothProfile = new double[profile.Length];

			smoothProfile[0] = profile[0];
			
			for (int i = 1; i < profile.Length - 1; i++)
			{
				smoothProfile[i] = (profile[i - 1] + profile[i + 1]) / 2.0;
			}

			smoothProfile[smoothProfile.Length - 1] = profile[profile.Length - 1];
			
			return smoothProfile;
		}


		/// <summary>
		/// Trying out something new from Yu's paper...
		/// </summary>
		/// <returns></returns>
		private void newCurvCalculator()
		{
			// Calculate direction stuff for each point
			double[] d = new double[this.points.Length];

			for (int i = 0; i < d.Length - 1; i++)
			{
				d[i] = Math.Atan2( (points[i + 1].Y - points[i].Y), (points[i + 1].X - points[i].X) );
			}

			d[d.Length - 1] = d[d.Length - 2];


			// Calculate curvature based on direction stuff

			double[] c = new double[this.points.Length];

			for (int i = 0; i < c.Length; i++)
			{
				// Use the points between (index - window) and (index + window)
				int window = 5;
			
				// Indices for the points on the stroke
				int startIndex = i - window;
				int endIndex = i + window;
			
				// Fix the start and end indices so that they are not out of bounds
				if (startIndex < 0)
					startIndex = 0;
				if (endIndex > this.points.Length - 1)
					endIndex = this.points.Length - 1;
				
				double sumD = 0.0;
				for (int k = startIndex; k < endIndex; k++)
				{
					double prePhi = d[k + 1] - d[k];
					
					double postPhi = prePhi;
 
					while (postPhi > Math.PI)
						postPhi -= Math.PI;

					while (postPhi < -Math.PI)
						postPhi += Math.PI;
					
					sumD += postPhi;
				}

				c[i] = Math.Abs(sumD) / (arcLengthProfile[endIndex] - arcLengthProfile[startIndex]);

				c[i] *= 100;

				if (c[i] < this.minCurv)
					this.minCurv = c[i];
				if (c[i] > this.maxCurv && !Double.IsPositiveInfinity(c[i]))
					this.maxCurv = c[i];
			}

			this.curvProfile = c;
		}

		#endregion

		#region GETTERS & SETTERS

		/// <summary>
		/// Returns the average curvature for the set of Points.
		/// Calculates it once if no average curvature currently exists.
		/// </summary>
		public double AverageCurvature
		{
			get
			{
				if (this.avgCurv == -1.0)
				{
					this.avgCurv = averageCurvature();
				}

				return this.avgCurv;
			}
		}


		/// <summary>
		/// Returns the minimum curvature for the set of Points.
		/// Calculates it once if no minimum curvature currently exists.
		/// </summary>
		public double MinimumCurvature
		{
			get
			{
				if (this.minCurv == Double.PositiveInfinity)
				{
					this.curvProfile = calcCurvProfile();
					//this.newCurvCalculator();
				}

				return this.minCurv;
			}
		}


		/// <summary>
		/// Returns the maximum curvature for the set of Points.
		/// Calculates it once if no maximum curvature currently exists.
		/// </summary>
		public double MaximumCurvature
		{
			get
			{
				if (this.maxCurv == Double.NegativeInfinity)
				{
					this.curvProfile = calcCurvProfile();
					//this.newCurvCalculator();
				}

				return this.maxCurv;
			}
		}
		

		/// <summary>
		/// Returns the (un-normalized) curvature profile for the set of Points.
		/// Calculates it once if no profile currently exists.
		/// </summary>
		public double[] Profile
		{
			get
			{
				if (this.curvProfile == null)
				{
					this.curvProfile = calcCurvProfile();
					//this.newCurvCalculator();
				}

				return this.curvProfile;
			}
		}


		/// <summary>
		/// Returns the normalized curvature profile for the set of Points.
		/// Calculates it once if no profile currently exists.
		/// </summary>
		public double[] NormProfile
		{
			get
			{
				if (this.normCurvProfile == null)
				{
					this.normCurvProfile = calcNormCurvProfile();
				}

				return this.normCurvProfile;
			}
		}

		#endregion
	}
}
