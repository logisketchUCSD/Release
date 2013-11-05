using System;
using Sketch;

namespace Featurefy
{
	/// <summary>
	/// FeatureStroke class. Takes in a regular Stroke and "Featurefies" it by
	/// assigning profiles for speed, arc length, slope, and curvature.
	/// </summary>
	public class FeatureStroke
	{
		#region INTERNALS

		/// <summary>
		/// Points of the Substroke to "featurefy"
		/// </summary>
		private Point[] points;
		
		/// <summary>
		/// Speed information for the given Substroke
		/// </summary>
		private Speed speed;

		/// <summary>
		/// Arc length information for the given Substroke
		/// </summary>
		private ArcLength arcLength;

		/// <summary>
		/// Slope information for the given Substroke
		/// </summary>
		private Slope slope;

		/// <summary>
		/// Curvature information for the given Substroke
		/// </summary>
		private Curvature curvature;

		/// <summary>
		/// Spatial spatial information for the given Substroke
		/// </summary>
		private Spatial spatial;

		/// <summary>
		/// The Sketch.Stroke Id that maps this FeatureStroke to a regular Stroke
		/// </summary>
		private System.Guid id;

		#endregion		

		#region CONSTURCTORS

		/// <summary>
		/// Constructs a new FeatureStroke from an existing one.
		/// </summary>
		/// <param name="fStroke">The given FeatureStroke to copy</param>
		public FeatureStroke(FeatureStroke fStroke) :
			this(new Substroke((Sketch.Point[])fStroke.Points.Clone(), new XmlStructs.XmlShapeAttrs()))
		{
			// Calls the main constructor
		}
		
		/// <summary>
		/// Finds the features of a Stroke. Converts the Stroke into a single Substroke first, 
		/// since we really always want features to be associated with substrokes.
		/// </summary>
		/// <param name="stroke">Stroke to find features for</param>
		public FeatureStroke(Sketch.Stroke stroke) :
			this(new Substroke(stroke.Points, stroke.XmlAttrs))
		{
			// Calls the main constructor
		}

		
		/// <summary>
		/// Finds the features of a Substroke
		/// </summary>
		/// <param name="substroke">Substroke to find features for</param>
		public FeatureStroke(Sketch.Substroke substroke)
		{
			this.points = substroke.Points;

			this.spatial   = new Spatial(this.points);
			this.arcLength = new ArcLength(this.points);
			this.slope	   = new Slope(this.points);
			this.speed	   = new Speed(this.points);
			this.curvature = new Curvature(this.points, ArcLength.Profile, Slope.TanProfile);
		
			this.id = (System.Guid)substroke.XmlAttrs.Id;
		}
	
		#endregion

		#region GETTERS & SETTERS

		/// <summary>
		/// Returns the Points of the Featurefied Stroke.
		/// </summary>
		public Point[] Points
		{
			get
			{
				return this.points;
			}
		}

		
		/// <summary>
		///  Returns the Spatial information for the substroke.
		/// </summary>
		public Spatial Spatial
		{
			get
			{
				return this.spatial;
			}
		}

		
		/// <summary>
		/// Returns the ArcLength information for the substroke.
		/// </summary>
		public ArcLength ArcLength
		{
			get
			{
				return this.arcLength;
			}
		}


		/// <summary>
		/// Returns the Slope information for the substroke.
		/// </summary>
		public Slope Slope
		{
			get
			{
				return this.slope;
			}
		}


		/// <summary>
		/// Returns the Speed information for the substroke.
		/// </summary>
		public Speed Speed
		{
			get
			{
				return this.speed;
			}
		}


		/// <summary>
		/// Returns the Curvature information for the FeatureStroke.
		/// </summary>
		public Curvature Curvature
		{
			get
			{
				return this.curvature;
			}
		}

		
		/// <summary>
		/// Returns the Guid information for the FeatureStroke.
		/// </summary>
		public System.Guid Id
		{
			get
			{
				return this.id;
			}
		}

		#endregion

		#region OTHER

		/// <summary>
		/// Computes all of the Profiles.
		/// This really isn't necessary. Everything is lazy and calculates when you need it.
		/// </summary>
		public void computeAll()
		{
			object temp;
			
			temp = this.arcLength.Profile;
			
			temp = this.spatial.AveragePoint;

			temp = this.speed.Profile;
			temp = this.speed.AverageSpeed;
		
			temp = this.curvature.Profile;
			temp = this.curvature.AverageCurvature;

			temp = this.slope.TanProfile;		
		}


		/// <summary>
		/// Clones a FeatureStroke.
		/// </summary>
		/// <returns>A new, cloned FeatureStroke</returns>
		public FeatureStroke Clone()
		{
			return new FeatureStroke(this);
		}

		#endregion
	}
}
