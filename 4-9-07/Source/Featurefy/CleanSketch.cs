using System;
using Sketch;

namespace Featurefy
{
	/// <summary>
	/// Summary description for CleanSketch.
	/// </summary>
	public class CleanSketch
	{
		/// <summary>
		/// Stored, cleaned sketch
		/// </summary>
		private Sketch.Sketch sketch;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sketch"></param>
		public CleanSketch(Sketch.Sketch sketch)
		{
			this.sketch = sketch;
		
			Substroke[] substrokes = this.sketch.Substrokes;
			
			for (int i = 0; i < substrokes.Length; i++)
			{
				CleanSubstroke(substrokes[i]);
			}
		}


		/// <summary>
		/// Removes overlapping Points from a Substroke. Two Points are considered
		/// to overlap if their X and Y coordinates are the same. If a set of overlapping
		/// Points is removed, a new Point will take their place consisting of the last Point's
		/// Id, Name, & Time, but the largest Pressure value between all Points in the set.
		/// </summary>
		/// <param name="substroke">Substroke to clean</param>
		public void CleanSubstroke(Substroke substroke)
		{
			Point[] points = substroke.Points;

			Point tempPoint = new Point();
			
			bool mergingPoints  = false;
			int startIndex      = -1;
			ushort tempPressure = Convert.ToUInt16(0.0);

			int length = points.Length;
			for (int i = 0; i < length - 1; i++)
			{
				// If the Point's overlap, merge them into the temp Point variable
				if (points[i].X == points[i + 1].X && points[i].Y == points[i + 1].Y)
				{
					// If we begin to merge points here, set the flag, start index, 
					// and the previous Point's pressure
					if (!mergingPoints)
					{
						mergingPoints = true;
						startIndex = i;
						tempPressure = (ushort)points[i].XmlAttrs.Pressure;
					}
					
					// Store the higher Pressure between the two Points 
					if (tempPressure < (ushort)points[i + 1].XmlAttrs.Pressure)
						tempPressure = (ushort)points[i + 1].XmlAttrs.Pressure;
					
					// Give the temp Point the current Point's XmlAttrs, but make
					// sure that the pressure is correct
					tempPoint.XmlAttrs = points[i + 1].XmlAttrs;
					tempPoint.XmlAttrs.Pressure = tempPressure;
				}

				// Otherwise if we have finished a merge (known because of the flag)
				else if (mergingPoints)
				{
					substroke.DeleteRange(startIndex, i - startIndex);
					substroke.AddPoint(tempPoint);

					points = substroke.Points;
					length = points.Length;
					mergingPoints = false;

					i = startIndex;
				}
			}

			// One last check outside the for loop for the final points
			if (mergingPoints)
			{
				substroke.DeleteRange(startIndex, points.Length - 1 - startIndex);
				substroke.AddPoint(tempPoint);
			}
		}


		/// <summary>
		/// Returns the cleaned Sketch.Sketch.
		/// </summary>
		public Sketch.Sketch CleanedSketch
		{
			get
			{
				return this.sketch;
			}
		}
	}
}
