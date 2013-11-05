/*
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
using System.Collections.Generic;

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
		private List<Substroke> substrokes;

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
			this(new List<Substroke>(), new XmlStructs.XmlShapeAttrs())
		{
			//Calls the main constructor
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="substroke"></param>
        public Stroke(Substroke substroke)
        {
            this.substrokes = new List<Substroke>();
            this.XmlAttrs = new XmlStructs.XmlShapeAttrs();
            this.XmlAttrs.Id = Guid.NewGuid();
            this.AddSubstroke(substroke);
        }

		/// <summary>
		/// Creates a Stroke from the corresponding Substrokes and XML attributes
		/// </summary>
		/// <param name="substrokes">Substrokes of the Stroke</param>
		/// <param name="XmlAttrs">The XML attributes of the stroke</param>
		public Stroke(Substroke[] substrokes, XmlStructs.XmlShapeAttrs XmlAttrs) : 
			this(new List<Substroke>(substrokes), XmlAttrs)
		{
			// Calls the main constructor
		}

		
		/// <summary>
		/// Creates a Stroke from the corresponding Substrokes and XML attributes
		/// </summary>
		/// <param name="substrokes">Substrokes of the Stroke</param>
		/// <param name="XmlAttrs">The XML attributes of the stroke</param>
		public Stroke(List<Substroke> substrokes, XmlStructs.XmlShapeAttrs XmlAttrs)
		{
			this.substrokes = new List<Substroke>(substrokes.Count);
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
					if (substroke.XmlAttrs.Time.Value < ((Substroke)this.substrokes[mid]).XmlAttrs.Time.Value)
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
		public void AddSubstrokes(List<Substroke> substrokes)
		{
			int length = substrokes.Count;

			for (int i = 0; i < length; i++)
				this.AddSubstroke(substrokes[i]);
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
		public bool RemoveSubstrokes(List<Substroke> substrokes)
		{
			bool completelyRemoved = true;

            Substroke currSubstroke;
            int len = substrokes.Count;
			for (int i = 0; i < len; ++i)
			{
				currSubstroke = substrokes[i];

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
					if (currSubstrokePts[k].XmlAttrs.Id.Value == pointId)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public bool MergeSubstrokes()
		{
			return MergeSubstrokes(new List<Substroke>(this.substrokes));
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="substrokesToMerge"></param>
        /// <returns></returns>
		public bool MergeSubstrokes(List<Substroke> substrokesToMerge)
		{
			// Sort them (based in Time)
			substrokesToMerge.Sort();

			List<Point> mergedPoints = new List<Point>();
			
			bool allSameLabel = true;
			List<Shape> parentShapes = null;
            Substroke s;

            int i, len = substrokesToMerge.Count;
            for(i = 0; i < len; ++i)
			{
                s = substrokesToMerge[i];

				// Get the Points
				mergedPoints.AddRange(s.Points);

				List<Shape> currParentShapes = s.ParentShapes;
				
				if (parentShapes == null)
					parentShapes = new List<Shape>(currParentShapes);
				else if (parentShapes.Count > 0 && parentShapes != currParentShapes)
					allSameLabel = false;

                int len2 = currParentShapes.Count;
                int j;
				for (j = 0; j < len2; ++j)
				{
					currParentShapes[j].RemoveSubstroke(s);
				}
			}
			
			mergedPoints.Sort();
			
			// Initialize the merged substroke attributes
            XmlStructs.XmlShapeAttrs mergedAttrs = substrokesToMerge[0].XmlAttrs.Clone();
			mergedAttrs.Start = mergedPoints[0].XmlAttrs.Id;
			mergedAttrs.End = mergedPoints[mergedPoints.Count - 1].XmlAttrs.Id;
			mergedAttrs.Time = mergedPoints[mergedPoints.Count - 1].XmlAttrs.Time;

			// Get the X, Y, Width, and Height parameters in this loop
			float minX = float.PositiveInfinity;
			float maxX = float.NegativeInfinity;

			float minY = float.PositiveInfinity;
			float maxY = float.NegativeInfinity;

            len = substrokesToMerge.Count;
			for (i = 0; i < len; ++i)
			{
				s = substrokesToMerge[i];

				minX = Math.Min(minX, s.XmlAttrs.X.Value);
				minY = Math.Min(minY, s.XmlAttrs.Y.Value);

				maxX = Math.Max(maxX, s.XmlAttrs.X.Value + s.XmlAttrs.Width.Value);
				maxY = Math.Max(maxY, s.XmlAttrs.Y.Value + s.XmlAttrs.Height.Value);

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
				mergedAttrs.Name = substrokesToMerge[0].XmlAttrs.Type;
				merged.ParentShapes = parentShapes;

                Shape shape;
                List<Shape> shapes = merged.ParentShapes;
                len = shapes.Count;
                for(i = 0; i < len; ++i)
                {
                    shape = merged.ParentShapes[i];
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
			float minX = Single.PositiveInfinity;
			float maxX = Single.NegativeInfinity;

			float minY = Single.PositiveInfinity;
			float maxY = Single.NegativeInfinity;

			// Cycle through the Substrokes within the Shape
			foreach (Substroke s in this.substrokes)
			{
				minX = Math.Min(minX, s.XmlAttrs.X.Value);
				minY = Math.Min(minY, s.XmlAttrs.Y.Value);

				maxX = Math.Max(maxX, s.XmlAttrs.X.Value + s.XmlAttrs.Width.Value);
				maxY = Math.Max(maxY, s.XmlAttrs.Y.Value + s.XmlAttrs.Height.Value);
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
				this.XmlAttrs.Start = (this.substrokes[0] as Substroke).XmlAttrs.Id;
				this.XmlAttrs.End = (this.substrokes[this.substrokes.Count - 1] as Substroke).XmlAttrs.Id;
				this.XmlAttrs.Time = (this.substrokes[this.substrokes.Count - 1] as Substroke).XmlAttrs.Time;
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

				return this.substrokes.ToArray();
			}
		}

		
		/// <summary>
		/// Get the Points of this Stroke
		/// </summary>
		public Point[] Points
		{
			get
			{
				List<Point> points = new List<Point>();
				
				int substrokeLength = this.substrokes.Count;
				int pointLength;
				int i;
				int j;
				Point[] ps;

				// Loop through all the substrokes
				for (i = 0; i < substrokeLength; ++i)
				{
					// Get the points
					ps = this.substrokes[i].Points;
					pointLength = ps.Length;

					// Add all the points
					for (j = 0; j < pointLength; ++j)
						points.Add(ps[j]);
				}
				
				// Sort the Points in ascending order based on time
				points.Sort();

				return points.ToArray();
			}
		}

		#endregion

		#region OTHER

        public Stroke Clone()
        {
            Stroke clone = new Stroke(this.CloneSubstrokes(), this.XmlAttrs.Clone());
            return clone;
            /*
            Stroke clone = default(Stroke);

            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();

                bf.Serialize(memStream, this);
                memStream.Flush();
                memStream.Position = 0;
                clone = ((Stroke)bf.Deserialize(memStream));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error cloning substroke, {0}, {1}", ex.Message, ex.StackTrace);
            }

            return clone;
            */
        }

        public List<Substroke> CloneSubstrokes()
        {
            return new List<Substroke>(this.substrokes);
            /*
            int i, len = this.substrokes.Count;
            List<Substroke> subs = new List<Substroke>(len);
            for (i = 0; i < len; ++i)
                subs[i] = this.substrokes[i].Clone();
            return subs;
            */
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
			return (int)(this.XmlAttrs.Time.Value - ((Stroke)obj).XmlAttrs.Time.Value);
		}

        public override bool Equals(object obj)
        {
            //return (this.XmlAttrs.Time.Value - ((Stroke)obj).XmlAttrs.Time.Value) == 0;
            return (this.XmlAttrs.Id == ((Stroke)obj).XmlAttrs.Id);
        }

		#endregion	
	}
}
