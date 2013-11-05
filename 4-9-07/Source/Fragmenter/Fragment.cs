using System;
using Featurefy;

namespace Fragmenter
{
	/// <summary>
	/// Summary description for Fragment.
	/// </summary>
	public class Fragment
	{
		/// <summary>
		/// Empty Constructor.
		/// </summary>
		public Fragment()
		{
			// Do nothing
		}


		/// <summary>
		/// Fragments a given Sketch.Sketch.
		/// </summary>
		/// <param name="sketch">Sketch to fragment</param>
		public static void fragmentSketch(Sketch.Sketch sketch)
		{
			// Cleaned sketch
			sketch = (new Featurefy.CleanSketch(sketch)).CleanedSketch;
				
			Sketch.Stroke[] strokes = sketch.Strokes;

			// Create the featured strokes
			Featurefy.FeatureStroke[] fStrokes = new Featurefy.FeatureStroke[strokes.Length];
			for (int i = 0; i < strokes.Length; i++)
			{
				fStrokes[i] = new FeatureStroke(strokes[i]);
			}

			// Break up the Sketch's strokes based on the Corners found for the FeatureStrokes
			for (int i = 0; i < fStrokes.Length; i++)
			{
				// Find the corners
				int[] corners = new Corners(fStrokes[i]).FindCorners();

				// Split the Stroke at those corners
				strokes[i].SplitStrokeAt(corners);
			}
		}
	}
}
