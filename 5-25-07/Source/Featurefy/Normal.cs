/*
 * File: Normal.cs
 *
 * Authors: Aaron Wolin, Devin Smith, Jason Fennell, and Max Pflueger.
 * Harvey Mudd College, Claremont, CA 91711.
 * Sketchers 2006.
 * 
 * Use at your own risk.  This code is not maintained and not guaranteed to work.
 * We take no responsibility for any harm this code may cause.
 */

using System;
using System.Collections;
using Sketch;

namespace Featurefy
{
	/// <summary>
	/// Summary description for Normal.
	/// </summary>
	public class Normal
	{
		/// <summary>
		/// Substrokes
		/// </summary>
		private ArrayList substrokes;

		/// <summary>
		/// FeatureStrokes
		/// </summary>
		private ArrayList featureStrokes;

		/// <summary>
		/// Normalized features
		/// </summary>
		private double[][] features;


		/// <summary>
		/// Constructor
		/// </summary>
		public Normal()
		{			
			this.substrokes		= new ArrayList();
			this.featureStrokes = new ArrayList();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strokes">The Strokes to be normalized</param>
		public Normal(Stroke[] strokes) : this()
		{
			int sLength = strokes.Length;
			for (int i = 0; i < sLength; ++i)
			{
				Substroke[] tempSubstrokes = strokes[i].Substrokes;
				
				int tempLength = tempSubstrokes.Length;
				for (int j = 0; j < tempLength; ++j)
					addSubstroke(tempSubstrokes[j]);
			}

			computeFeatures();
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="substrokes">The Substrokes to be normalized</param>
		public Normal(Substroke[] substrokes) : this()
		{
			int length = substrokes.Length;
			for (int i = 0; i < length; ++i)
				addSubstroke(substrokes[i]);

			computeFeatures();
		}


		/// <summary>
		/// Add a Substroke that will be normalized
		/// </summary>
		/// <param name="substroke"></param>
		private void addSubstroke(Substroke substroke)
		{
			//if (!this.substrokes.Contains(substroke))
				this.substrokes.Add(substroke);
		}


		/// <summary>
		/// Compute the normalized features from all the Substrokes that were added to this Normal
		/// </summary>
		private void computeFeatures()
		{
			FeatureStroke fstroke;

			int length = this.substrokes.Count;
			this.features = new double[length][];
			
			for (int i = 0; i < length; ++i)
			{
				fstroke = new FeatureStroke((Substroke)this.substrokes[i]);
				//fstroke.computeAll();

				featureStrokes.Add(fstroke);
				this.features[i] = new double[] { 	
													Convert.ToDouble(fstroke.Spatial.AveragePoint.X), 
													Convert.ToDouble(fstroke.Spatial.AveragePoint.Y), 
													Convert.ToDouble(fstroke.Spatial.AveragePoint.XmlAttrs.Time),
													Convert.ToDouble(fstroke.Spatial.AveragePoint.XmlAttrs.Pressure),
													
													Convert.ToDouble(fstroke.Spatial.FirstPoint.X),
													Convert.ToDouble(fstroke.Spatial.FirstPoint.Y),
													Convert.ToDouble(fstroke.Spatial.FirstPoint.XmlAttrs.Time),
													Convert.ToDouble(fstroke.Spatial.FirstPoint.XmlAttrs.Pressure),
																		
													Convert.ToDouble(fstroke.Spatial.LastPoint.X),
													Convert.ToDouble(fstroke.Spatial.LastPoint.Y),
													Convert.ToDouble(fstroke.Spatial.LastPoint.XmlAttrs.Time),
													Convert.ToDouble(fstroke.Spatial.LastPoint.XmlAttrs.Pressure),
															   
												    Convert.ToDouble(fstroke.ArcLength.Height),
													Convert.ToDouble(fstroke.ArcLength.Width),													

													fstroke.ArcLength.InkDensity,

													fstroke.ArcLength.TotalLength, 

													fstroke.Curvature.AverageCurvature,

													fstroke.Slope.AverageSlope,

													fstroke.Speed.AverageSpeed,
					
												    fstroke.Spatial.DistanceFromFirstToLast
													
												   
												};
				if(fstroke.Slope.S.Equals(double.NaN))
				{
					double d = fstroke.Slope.S;
				}
			}
			
			int NUMFEATURES = this.features[0].Length;

			//Normalize
			for(int i = 0; i < NUMFEATURES; ++i)
			{

				double min = double.PositiveInfinity;
				double max = double.NegativeInfinity;
				double val;
				for(int j = 0; j < length; ++j)
				{
					val = this.features[j][i];
					if(val < min)
						min = val;
					if(val > max)
						max = val;
				}

				double range = max - min;
				
				if(range == 0.0)
				{
					for(int j = 0; j < length; ++j)
						this.features[j][i] = 0.0; //0.5; //1.0;
				}
				else
				{
					for(int j = 0; j < length; ++j)
					{
						this.features[j][i] -= min;
						this.features[j][i] /= range;
					}
				}
			}
		}


		/// <summary>
		/// Get the normalized features
		/// </summary>
		public double[][] Features
		{
			get
			{
				return this.features;
			}
		}
	}
}
