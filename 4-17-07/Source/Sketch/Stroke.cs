/**
 * File: Stroke.cs
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

namespace Sketch
{
	/// <summary>
	/// Stroke class.
	/// </summary>
	public class Stroke : IComparable
	{
		#region INTERNALS

		/// <summary>
		/// What substrokes compose this Stroke
		/// </summary>
		private ArrayList substrokes;

		/// <summary>
		/// The XML attributes of the Stroke
		/// </summary>
		public XmlStructs.XmlShapeAttrs XmlAttrs;
		
		#endregion

		#region	CONSTRUCTORS
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Stroke() : 
			this(new ArrayList(), new XmlStructs.XmlShapeAttrs())
		{
			//Calls the main constructor
		}

		/// <summary>
		/// Creates a Stroke from a Stroke
		/// </summary>
		/// <param name="stroke">The Stroke</param>
		public Stroke(Stroke stroke) : 
			this((ArrayList)stroke.substrokes.Clone(), stroke.XmlAttrs.Clone())
		{
			// Calls the main constructor
		}


		/// <summary>
		/// Creates a Stroke from the corresponding Substrokes and XML attributes
		/// </summary>
		/// <param name="substrokes">Substrokes of the Stroke</param>
		/// <param name="XmlAttrs">The XML attributes of the stroke</param>
		public Stroke(Substroke[] substrokes, XmlStructs.XmlShapeAttrs XmlAttrs) : 
			this(new ArrayList(substrokes), XmlAttrs)
		{
			// Calls the main constructor
		}

		
		/// <summary>
		/// Creates a Stroke from the corresponding Substrokes and XML attributes
		/// </summary>
		/// <param name="substrokes">Substrokes of the Stroke</param>
		/// <param name="XmlAttrs">The XML attributes of the stroke</param>
		public Stroke(ArrayList substrokes, XmlStructs.XmlShapeAttrs XmlAttrs)
		{
			this.substrokes = new ArrayList();
			this.XmlAttrs = XmlAttrs;

			this.AddSubstrokes(substrokes);
		}

		#endregion
	
		#region ADD TO STROKE

		/// <summary>
		/// Adds the Substroke to the Stroke's Substroke ArrayList.
		/// Inserts the Substroke in ascending order.
		/// </summary>
		/// <param name="substroke">Substroke to add</param>
		public void AddSubstroke(Substroke substroke)
		{
			if (!this.substrokes.Contains(substroke))
			{		
				int low = 0;
				int high = this.substrokes.Count - 1;
				int mid;
				while (low <= high)
				{
					mid = (high - low) / 2 + low;
					if ((ulong)substroke.XmlAttrs.Time < (ulong)((Substroke)this.substrokes[mid]).XmlAttrs.Time)
						high = mid - 1;
					else
						low = mid + 1;
				}
			
				this.substrokes.Insert(low, substroke);
				substroke.ParentStroke = this;

				UpdateAttributes();
			}
		}

		
		/// <summary>
		/// Adds the Substrokes to the Stroke's Substroke ArrayList
		/// </summary>
		/// <param name="substrokes">Substrokes to add</param>
		public void AddSubstrokes(Substroke[] substrokes)
		{
			int length = substrokes.Length;
			
			for (int i = 0; i < length; i++)
				this.AddSubstroke(substrokes[i]);
		}

		
		/// <summary>
		/// Adds the Substrokes to the Stroke's Substroke ArrayList
		/// </summary>
		/// <param name="substrokes">Substrokes to add</param>
		public void AddSubstrokes(ArrayList substrokes)
		{
			int length = substrokes.Count;

			for (int i = 0; i < length; i++)
				this.AddSubstroke((Substroke)substrokes[i]);
		}

		#endregion

		#region REMOVE FROM STROKE

		/// <summary>
		/// Removes a Substroke from the Stroke.
		/// </summary>
		/// <param name="substroke">Substroke to remove</param>
		/// <returns>True iff the Substroke is removed</returns>
		public bool RemoveSubstroke(Substroke substroke)
		{
			if (this.substrokes.Contains(substroke))
			{
				substroke.ParentStroke = null;
				this.substrokes.Remove(substroke);
				UpdateAttributes();
				
				return true;
			}
			else
			{
				return false;
			}
		}

		
		/// <summary>
		/// Removes an ArrayList of Substrokes from the Stroke.
		/// </summary>
		/// <param name="substrokes">Substrokes to remove</param>
		/// <returns>True iff all Substrokes are removed</returns>
		public bool RemoveSubstrokes(ArrayList substrokes)
		{
			bool completelyRemoved = true;

			for (int i = 0; i < substrokes.Count; ++i)
			{
				Substroke currSubstroke = (Substroke)substrokes[i];

				if (!RemoveSubstroke(currSubstroke))
				{
					completelyRemoved = false;
					Console.WriteLine("Substroke " + currSubstroke.XmlAttrs.Id + " not removed!");
				}
			}

			return completelyRemoved;
		}

		#endregion

		#region SPLIT STROKE

		/// <summary>
		/// Split the substroke at the given index at the point index
		/// </summary>
		/// <param name="substrokeIndex">Index of the substroke</param>
		/// <param name="pointIndex">Index of the point to split at (the second half has the point)</param>
		public void SplitSubstrokeAt(int substrokeIndex, int pointIndex)
		{
			Substroke first = (Substroke)this.substrokes[substrokeIndex];
			first.SplitAt(pointIndex);
		}


		/// <summary>
		/// Split the substroke at the given index at the given indices
		/// </summary>
		/// <param name="substrokeIndex">The index of the substroke</param>
		/// <param name="pointIndices">The indices to split at</param>
		public void SplitSubstrokeAt(int substrokeIndex, int[] pointIndices)
		{
			// We need to step through backwards so we dont mess up the indices
			Array.Sort(pointIndices);
			Array.Reverse(pointIndices);

			Substroke first = (Substroke)this.substrokes[substrokeIndex];
			first.SplitAt(pointIndices);
		}


		/// <summary>
		/// Split the substrokes given by their indices at the given point split indices
		/// </summary>
		/// <param name="substrokeIndices"></param>
		/// <param name="pointIndices"></param>
		public void SplitSubstrokesAt(int[] substrokeIndices, int[] pointIndices)
		{
			// Due to the way that we insert substrokes into the array, we must step through backwards...
			Array.Sort(substrokeIndices);
			Array.Reverse(substrokeIndices);

			int length = substrokeIndices.Length;
			for (int i = 0; i < length; ++i)
				this.SplitSubstrokeAt(substrokeIndices[i], pointIndices[i]);
		}


		/// <summary>
		/// Split the substrokes given by their indices at the given indices
		/// </summary>
		/// <param name="substrokeIndices"></param>
		/// <param name="pointIndices"></param>
		public void SplitSubstrokesAt(int[] substrokeIndices, int[][] pointIndices)
		{
			// Due to the way that we insert substrokes into the array, we must step through backwards...
			Array.Sort(substrokeIndices);
			Array.Reverse(substrokeIndices);

			int length = substrokeIndices.Length;
			for (int i = 0; i < length; ++i)
				this.SplitSubstrokeAt(substrokeIndices[i], pointIndices[i]);
		}


		/// <summary>
		/// Splits a Stroke's Substroke into two Substrokes at the given Point GUID.
		/// </summary>
		/// <param name="pointId">Point Id indicating where the Substroke should be split</param>
		public void SplitStrokeAt(System.Guid pointId)
		{
			int subLength;
			int ptsLength;

			// Cycle through the Stroke's Substrokes
			subLength = Substrokes.Length;
			for (int i = 0; i < subLength; i++)
			{
				// Cycle through the Substroke's Points
				Point[] currSubstrokePts = Substrokes[i].Points;

				// Don't split if k == 0 or k == length - 1
				ptsLength = currSubstrokePts.Length - 1;
				
				/*
				if ((System.Guid)currSubstrokePts[0].XmlAttrs.Id == pointId)
					Console.WriteLine("Given Point Id {0} starts a Substroke - There is nothing to split", pointId.ToString());
				else if ((System.Guid)currSubstrokePts[ptsLength].XmlAttrs.Id == pointId)
					Console.WriteLine("Given Point Id {0} ends a Substroke - We cannot split", pointId.ToString());
				*/
				
				for (int k = 1; k < ptsLength; k++)
				{
					// If the Substroke contains the given pointId to split
					if ((System.Guid)currSubstrokePts[k].XmlAttrs.Id == pointId)
					{
						this.SplitSubstrokeAt(i, k);
						return;
					}						
				}
			}

			// Point ID does not exist
			//Console.Writeline("Given Point Id {0} does not exist in this Sketch", pointId.ToString());
		}


		/// <summary>
		/// Split the given Stroke at the specified Point index. Automatically takes care of substroke messiness.
		/// </summary>
		/// <param name="pointIndex">Index to split the Stroke at</param>
		public void SplitStrokeAt(int pointIndex)
		{
			int[] substrokeIndex = this.StrokeIndexToSubstrokeIndex(pointIndex);
			
			// Make sure we are not splitting at a stupid index (trivial)
			if (substrokeIndex[1] != 0)
				this.SplitSubstrokeAt(substrokeIndex[0], substrokeIndex[1]);
		}

		
		/// <summary>
		/// Split the given Stroke at the specified Point indices. Automatically takes care of substroke messiness.
		/// </summary>
		/// <param name="pointIndices">Indices to split the Stroke at</param>
		public void SplitStrokeAt(int[] pointIndices)
		{
			int length = pointIndices.Length;
			for (int i = 0; i < length; ++i)
			{
				this.SplitStrokeAt(pointIndices[i]);
			}
		}


		/// <summary>
		/// Given a stroke index (from Points getter), return int[2]; int[0] is the index of the substroke; 
		/// int[1] is the index of the point within the substroke
		/// </summary>
		/// <param name="strokeIndex"></param>
		/// <returns></returns>
		public int[] StrokeIndexToSubstrokeIndex(int strokeIndex)
		{
			int[] toReturn = new int[2];
			
			int i;
			int length = this.substrokes.Count;
			for (i = 0; i < length; ++i)
			{
				int numPoints = ((Substroke)this.substrokes[i]).Points.Length;
				if (numPoints <= strokeIndex)
					strokeIndex -= numPoints;
				else
					break;
			}

			toReturn[0] = i;
			toReturn[1] = strokeIndex;

			return toReturn;
		}

		#endregion

		#region MERGE SUBSTROKES

		public bool MergeSubstrokes()
		{
			return MergeSubstrokes(new ArrayList(this.substrokes));
		}


		public bool MergeSubstrokes(ArrayList substrokesToMerge)
		{
			// Sort them (based in Time)
			substrokesToMerge.Sort();

			ArrayList mergedPoints = new ArrayList();
			
			bool allSameLabel = true;
			ArrayList parentShapes = null;

			foreach (Substroke s in substrokesToMerge)
			{
				// Get the Points
				mergedPoints.AddRange(s.Points);

				ArrayList currParentShapes = s.ParentShapes;
				
				if (parentShapes == null)
					parentShapes = new ArrayList(currParentShapes);
				else if (parentShapes.Count > 0 && parentShapes != currParentShapes)
					allSameLabel = false;

				for (int i = 0; i < currParentShapes.Count; i++)
				{
					(currParentShapes[i] as Shape).RemoveSubstroke(s);
				}
			}
			
			mergedPoints.Sort();
			
			// Initialize the merged substroke attributes
			XmlStructs.XmlShapeAttrs mergedAttrs = 
				(XmlStructs.XmlShapeAttrs)(substrokesToMerge[0] as Substroke).XmlAttrs.Clone();
			mergedAttrs.Start = (System.Guid)((mergedPoints[0] as Point).XmlAttrs.Id);
			mergedAttrs.End = (System.Guid)((mergedPoints[mergedPoints.Count - 1] as Point).XmlAttrs.Id);
			mergedAttrs.Time = (mergedPoints[mergedPoints.Count - 1] as Point).Time;

			// Get the X, Y, Width, and Height parameters in this loop
			double minX = Double.PositiveInfinity;
			double maxX = Double.NegativeInfinity;

			double minY = Double.PositiveInfinity;
			double maxY = Double.NegativeInfinity;

			for (int i = 0; i < substrokesToMerge.Count; i++)
			{
				Substroke s = substrokesToMerge[i] as Substroke;

				minX = Math.Min(minX, Convert.ToDouble(s.XmlAttrs.X));
				minY = Math.Min(minY, Convert.ToDouble(s.XmlAttrs.Y));

				maxX = Math.Max(maxX, Convert.ToDouble(s.XmlAttrs.X) + Convert.ToDouble(s.XmlAttrs.Width));
				maxY = Math.Max(maxY, Convert.ToDouble(s.XmlAttrs.Y) + Convert.ToDouble(s.XmlAttrs.Height));

				RemoveSubstroke(s);
			}

			// Set the origin at the top-left corner of the merged Substroke
			this.XmlAttrs.X = minX;
			this.XmlAttrs.Y = minY;

			// Set the width and height of the merged Substroke
			this.XmlAttrs.Width = maxX - minX;
			this.XmlAttrs.Height = maxY - minY;

			// Create the merged Substroke
			Substroke merged = new Substroke(mergedPoints, mergedAttrs);
			merged.ParentStroke = this;			
			
			// Update the labels
			if (allSameLabel)
			{
				mergedAttrs.Name = (substrokesToMerge[0] as Substroke).XmlAttrs.Type;
				merged.ParentShapes = parentShapes;

				foreach (Shape shape in merged.ParentShapes)
				{
					shape.AddSubstroke(merged);
				}
			}
			
			// Add the merged Substroke
			AddSubstroke(merged);

			return allSameLabel;
		}

		#endregion

		#region UPDATE ATTRIBUTES

		/// <summary>
		/// Updates the spatial attributes of the Shape, such as the origin
		/// and width of the shape
		/// </summary>
		public void UpdateAttributes()
		{
			double minX = Double.PositiveInfinity;
			double maxX = Double.NegativeInfinity;

			double minY = Double.PositiveInfinity;
			double maxY = Double.NegativeInfinity;

			// Cycle through the Substrokes within the Shape
			foreach (Substroke s in this.substrokes)
			{
				minX = Math.Min(minX, Convert.ToDouble(s.XmlAttrs.X));
				minY = Math.Min(minY, Convert.ToDouble(s.XmlAttrs.Y));

				maxX = Math.Max(maxX, Convert.ToDouble(s.XmlAttrs.X) + Convert.ToDouble(s.XmlAttrs.Width));
				maxY = Math.Max(maxY, Convert.ToDouble(s.XmlAttrs.Y) + Convert.ToDouble(s.XmlAttrs.Height));
			}

			// Set the origin at the top-left corner of the shape group
			this.XmlAttrs.X = minX;
			this.XmlAttrs.Y = minY;

			// Set the width and height of the shape
			this.XmlAttrs.Width = maxX - minX;
			this.XmlAttrs.Height = maxY - minY;

			// Sort the substrokes to ensure we are still in an ascending time order
			//this.substrokes.Sort();

			// Update the Start, End and Time attributes
			if (this.substrokes.Count > 0)
			{
				this.XmlAttrs.Start = (System.Guid)((this.substrokes[0] as Substroke).XmlAttrs.Id);
				this.XmlAttrs.End = (System.Guid)((this.substrokes[this.substrokes.Count - 1] as Substroke).XmlAttrs.Id);
				this.XmlAttrs.Time = Convert.ToUInt64((this.substrokes[this.substrokes.Count - 1] as Substroke).XmlAttrs.Time);
			}
			else
			{
				this.XmlAttrs.Start = null;
				this.XmlAttrs.End = null;
				this.XmlAttrs.Time = null;
			}
		}

		#endregion

		#region GETTERS & SETTERS

		/// <summary>
		/// Returns the sorted Substroke array of the Stroke
		/// </summary>
		public Substroke[] Substrokes
		{
			get
			{
				// Sort the Substrokes in ascending order based on time
				// NOTE: Since we add substrokes in a sorted order, we should not need to sort afterward
				//substrokes.Sort();

				return (Substroke[])this.substrokes.ToArray(typeof(Substroke));
			}
		}

		
		/// <summary>
		/// Get the Points of this Stroke
		/// </summary>
		public Point[] Points
		{
			get
			{
				ArrayList points = new ArrayList();
				
				int substrokeLength = this.substrokes.Count;
				int pointLength;
				int i;
				int j;
				Point[] ps;

				// Loop through all the substrokes
				for (i = 0; i < substrokeLength; ++i)
				{
					// Get the points
					ps = ((Substroke)this.substrokes[i]).Points;
					pointLength = ps.Length;

					// Add all the points
					for (j = 0; j < pointLength; ++j)
						points.Add(ps[j]);
				}
				
				// Sort the Points in ascending order based on time
				points.Sort();

				return (Point[])points.ToArray(typeof(Point));
			}
		}

		#endregion

		#region OTHER

		/// <summary>
		/// Clone this Stroke.
		/// </summary>
		/// <returns>Clones a Stroke.</returns>
		public Stroke Clone()
		{
			return new Stroke(this);
		}


		/// <summary>
		/// Compare this Stroke to another based on time.
		/// Returns less than 0 if this time is less than the other's.
		/// Returns 0 if this time is equal to the other's.
		/// Returns greater than if this time is greater than the other's.
		/// </summary>
		/// <param name="obj">The other Stroke to compare this one to</param>
		/// <returns>An integer indicating how the Stroke times compare</returns>
		int System.IComparable.CompareTo(Object obj)
		{
			Stroke other = (Stroke)obj;

			ulong thisTime  = (ulong)this.XmlAttrs.Time;
			ulong otherTime = (ulong)other.XmlAttrs.Time;

			return (int)(thisTime - otherTime);
		}

		#endregion	
	}
}
