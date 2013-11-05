/*
 * File: Substroke.cs
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
	/// Substroke class.
	/// </summary>
	public class Substroke : IComparable<Substroke>
	{
		#region INTERNALS

		/// <summary>
		/// The points of the Substroke
		/// </summary>
        private List<Point> points;
		
		/// <summary>
		/// This is the parent stroke
		/// </summary>
		private Stroke parentStroke;
        
		/// <summary>
		/// This is the parent shape
		/// </summary>
        private List<Shape> parentShapes;

		/// <summary>
		/// The XML attributes of the Substroke
		/// </summary>
		public XmlStructs.XmlShapeAttrs XmlAttrs;
		
		#endregion

		#region CONSTRUCTORS

		/// <summary>
		/// Constructor
		/// </summary>
		public Substroke() 
			: this(new List<Point>(), new XmlStructs.XmlShapeAttrs())
		{
			// Calls the main constructor
		}
		
		/// <summary>
		/// Creates a Substroke from an array of Points and the XML attributes
		/// </summary>
		/// <param name="points">Array of Points</param>
		/// <param name="XmlAttrs">The XML attributes of the stroke</param>
		public Substroke(Point[] points, XmlStructs.XmlShapeAttrs XmlAttrs)
			: this(new List<Point>(points), XmlAttrs)
		{
            
		}
		
		/// <summary>
		/// Creates a Substroke from a List of Points and the XML attributes
		/// </summary>
		/// <param name="points">List of Points</param>
		/// <param name="XmlAttrs">The XML attributes of the stroke</param>
		public Substroke(List<Point> points, XmlStructs.XmlShapeAttrs XmlAttrs)
		{           
            this.points = new List<Point>(points.Count);
            this.parentShapes = new List<Shape>();
			this.parentStroke = null;
			this.XmlAttrs = XmlAttrs;

			this.AddPoints(points);
            this.UpdateAttributes();
		}
		
		
		#endregion
		
		#region ADD TO SUBSTROKE
		
		/// <summary>
		/// Adds the Point to the Substroke's Point List. Uses binary search to find insertion spot
		/// </summary>
		/// <param name="point">Point to add</param>
		public void AddPoint(Point point)
		{
			//if (this.points.Contains(point))
			//	this.points.Remove(point);

			if (!this.points.Contains(point))
			{
				int low = 0;
				int high = this.points.Count - 1;
				int mid;
				while (low <= high)
				{
					mid = (high - low) / 2 + low;
					if (point.Time < ((Point)this.points[mid]).Time)
						high = mid - 1;
					else
						low = mid + 1;
				}
		
				this.points.Insert(low, point);
			}
		}


		/// <summary>
		/// Adds the Points to the Substroke's Point ArrayList.
		/// </summary>
		/// <param name="points">Points to add</param>
		public void AddPoints(Point[] points)
		{
			int length = points.Length;
			
			for (int i = 0; i < length; i++)
				this.AddPoint(points[i]);
		}

		
		/// <summary>
		/// Adds the Points to the Substroke's Point ArrayList.
		/// </summary>
		/// <param name="points">Points to add</param>
		public void AddPoints(List<Point> points)
		{
			int length = points.Count;
			
			for (int i = 0; i < length; i++)
				this.AddPoint(points[i]);
		}

		#endregion

		#region REMOVE

		/// <summary>
		/// Deletes a range of Points from the Substroke.
		/// </summary>
		/// <param name="index">Index of the initial Point to delete</param>
		/// <param name="count">How many to remove</param>
		public void DeleteRange(int index, int count)
		{
			this.points.RemoveRange(index, count);
		}


		/// <summary>
		/// Removes a label from the Substroke
		/// </summary>
		/// <param name="label">Label to remove</param>
		/// <returns>The shape that was removed</returns>
		public Shape RemoveLabel(String label)
		{
			foreach (Shape shape in this.parentShapes)
			{
				if (shape.XmlAttrs.Type.Equals(label))
				{
					shape.RemoveSubstroke(this);
					return shape;
				}
			}

			return null;
		}

		#endregion

        /// <summary>
        /// Updates X, Y, Width, Height
        /// </summary>
        private void UpdateAttributes()
        {
            float minX = Single.PositiveInfinity;
            float maxX = Single.NegativeInfinity;

            float minY = Single.PositiveInfinity;
            float maxY = Single.NegativeInfinity;

            Point point;
            int len = points.Count;
            int i;
            for (i = 0; i < len; ++i)
            {
                point = points[i];

                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);

                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            this.XmlAttrs.X = minX;
            this.XmlAttrs.Y = minY;
            this.XmlAttrs.Width = maxX - minX;
            this.XmlAttrs.Height = maxY - minY;

            //Other attributes are updated in split
        }

        #region SPLITTERS

        /// <summary>
        /// Splits a Substroke at the given index.
        /// </summary>
        /// <param name="index">Index where the Substroke should be split</param>
        public void SplitAt(int index)
		{
			// If there is nothing to split...
			if (index < 1 || index > this.points.Count - 2)
				return;
			
			Substroke lastHalf = new Substroke();

			// We invalidate these things
			this.XmlAttrs.Height = null;
			this.XmlAttrs.Width  = null;
			this.XmlAttrs.Area   = null;
			this.XmlAttrs.LeftX  = null;
			this.XmlAttrs.TopY   = null;
			this.XmlAttrs.X      = null;
			this.XmlAttrs.Y      = null;

			// Copy the rest of the attributes into the second half
			lastHalf.XmlAttrs = this.XmlAttrs.Clone();
			lastHalf.XmlAttrs.Id = System.Guid.NewGuid();

			int length = points.Count;

			// Add points to the last Half
			lastHalf.AddPoints(points.GetRange(index, length - index)); 

			// Change the start of the last half
			lastHalf.XmlAttrs.Start = lastHalf.points[0].XmlAttrs.Id;

			// Remove all the points in the second half
			this.points.RemoveRange(index, length - index);
			//this.points.RemoveRange(index + 1, length - index - 1);	// Used if we want to include the middle, split point in both Substrokes
			
			// Add the end to the first half
			this.XmlAttrs.End = this.points[index - 1].XmlAttrs.Id;

			// The new first half time is equal to the last point's time in it
			this.XmlAttrs.Time = this.points[index - 1].XmlAttrs.Time;

            //Update X, Y...
            this.UpdateAttributes();
            lastHalf.UpdateAttributes();

			// Update the parent info
			lastHalf.ParentStroke = this.parentStroke;
			lastHalf.ParentStroke.AddSubstroke(lastHalf);

            //lastHalf.ParentShapes = (ArrayList)this.parentShapes;
            lastHalf.ParentShapes = this.parentShapes.GetRange(0, this.parentShapes.Count); //Like clone
			
			length = lastHalf.parentShapes.Count;
			for (int i = 0; i < length; ++i)
				if (lastHalf.parentShapes[i] != null)
					lastHalf.parentShapes[i].AddSubstroke(lastHalf);

		}


		/// <summary>
		/// Splits a Substroke at the given indices.
		/// </summary>
		/// <param name="indices">Indices where the Substroke should be split</param>
		public void SplitAt(int[] indices)
		{
			// Due to the way that we insert substrokes into the array, we must step through backwards...
			Array.Sort(indices);
			Array.Reverse(indices);

			int length = indices.Length;
			for (int i = 0; i < length; i++)
				this.SplitAt(indices[i]);
		}

		#endregion

		#region GETTERS & SETTERS
		
		/// <summary>
		/// Get the number of Points in this substroke
		/// </summary>
		public int Length
		{
			get
			{
				return this.points.Count;
			}
		}


		/// <summary>
		/// Gets a Point[] of the points contained in the Substroke.
		/// </summary>
		public Point[] Points
		{
			get
			{
				return this.points.ToArray();
			}
		}

        /// <summary>
        /// Gets a Point List of the points contained in the Substroke
        /// </summary>
        public List<Point> PointsL
        {
            get
            {
                return this.points;
            }
        }


		/// <summary>
		/// Get or set ParentStroke
		/// </summary>
		public Stroke ParentStroke
		{
			get
			{
				return this.parentStroke;
			}

			set
			{
				this.parentStroke = value;
			}
		}


		/// <summary>
		/// Get or set ParentShapes
		/// </summary>
		public List<Shape> ParentShapes
		{
			get
			{
                return this.parentShapes;
			}

			set
			{
                this.parentShapes = value;
			}
		}


		/// <summary>
		/// Get the labels associated with a Substroke.
		/// </summary>
		/// <returns></returns>
		public string[] GetLabels()
		{
			List<string> labels = new List<string>();

			Shape shape;
			int length = this.ParentShapes.Count;
			for (int i = 0; i < length; ++i)
			{
				shape = this.ParentShapes[i];
				
				if (!labels.Contains(shape.XmlAttrs.Type))
					labels.Add(shape.XmlAttrs.Type);
			}
			
			return labels.ToArray();
		}


		/// <summary>
		/// Get the beliefs associated with the labels
		/// </summary>
		/// <returns></returns>
		public double[] GetBeliefs()
		{
			List<string> labels = new List<string>();
			List<double> beliefs = new List<double>();

			Shape shape;
			int length = this.ParentShapes.Count;
			for (int i = 0; i < length; ++i)
			{
				shape = this.ParentShapes[i];
				
				if (!labels.Contains(shape.XmlAttrs.Type))
				{
					labels.Add(shape.XmlAttrs.Type);
					beliefs.Add(shape.XmlAttrs.Probability.Value);
				}
			}
			
			return beliefs.ToArray();
		}

		/// <summary>
		/// Get the first label
		/// </summary>
		/// <returns></returns>
		public string GetFirstLabel()
		{
			if (this.parentShapes.Count == 0)
				return "unlabeled";
			else
				return this.parentShapes[0].XmlAttrs.Type;
		}

		/// <summary>
		/// Get the first probability
		/// </summary>
		/// <returns></returns>
		public double GetFirstBelief()
		{
			if(this.parentShapes == null || this.parentShapes.Count == 0)
				return -1.0;
			else
			{
                if (!this.parentShapes[0].XmlAttrs.Probability.HasValue)
                    return -1.0;
                else
                    return this.ParentShapes[0].XmlAttrs.Probability.Value;
			}
		}

		#endregion

		#region OTHER

        public void removeFromParentShapes()
        {
            int i, len = this.parentShapes.Count;
            for (i = 0; i < len; ++i)
            {
                this.parentShapes[i].RemoveSubstroke(this);
            }
        }

        /*
		/// <summary>
		/// Clone this Substroke
		/// </summary>
		/// <returns>The cloned Substroke</returns>
		public Substroke Clone()
		{
			return new Substroke(this);
		}*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return Equals((Substroke)obj);           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="substroke"></param>
        /// <returns></returns>
        public bool Equals(Substroke substroke)
        {
            if (this.Length != substroke.Length || this.Length == 0 || substroke.Length == 0)
                return false;

            return (this.points[0].Equals(substroke.points[0]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

		/// <summary>
		/// Compare this Substroke to another based on time.
		/// Returns less than 0 if this time is less than the other's.
		/// Returns 0 if this time is equal to the other's.
		/// Returns greater than 0 if this time is greater than the other's.
		/// </summary>
		/// <param name="substroke">The other Substroke to compare this one to</param>
		/// <returns>An integer indicating how the Substroke times compare</returns>
        int System.IComparable<Substroke>.CompareTo(Substroke substroke)
        {
            return (int)(this.XmlAttrs.Time.Value - substroke.XmlAttrs.Time.Value);
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Substroke Clone()
        {
            Substroke clone = default(Substroke);
            
            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = 
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();

                bf.Serialize(memStream, this);
                memStream.Flush();
                memStream.Position = 0;
                clone = ((Substroke)bf.Deserialize(memStream));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error cloning substroke, {0}, {1}", ex.Message, ex.StackTrace);
            }

            return clone;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Substroke Clone()
        {
            Substroke clone = new Substroke(this.ClonePoints(), this.XmlAttrs.Clone());
            
            //Do we want to deep clone parent shapes or should they be left as references?
            if (this.parentShapes != null)
                clone.ParentShapes = this.CloneParentShapes();

            //Do we want to deep clone parent stroke or should they be left as references?
            if (this.parentStroke != null)
                clone.ParentStroke = this.parentStroke.Clone();
            return clone;
        }

        /// <summary>
        /// Returns a deep copy of the points
        /// </summary>
        /// <returns></returns>
        public List<Point> ClonePoints()
        {
            int i, len = this.points.Count;
            List<Point> copyPoints = new List<Point>(len);
            for (i = 0; i < len; ++i)
                copyPoints.Add(this.points[i].Clone());
            return copyPoints;
        }

        /// <summary>
        /// Returns a shallow copy of the points
        /// </summary>
        /// <returns></returns>
        public List<Shape> CloneParentShapes()
        {
            return new List<Shape>(this.parentShapes);
            /*
            int i, len = this.parentShapes.Count;
            List<Shape> copyShapes = new List<Shape>(len);
            for (i = 0; i < len; ++i)
                copyShapes.Add(this.parentShapes[i].Clone());
            return copyShapes;
            */
        }

        #endregion

        public void rotate(double theta, float xCenter, float yCenter)
        {
            int i, len = this.points.Count;
            for (i = 0; i < len; ++i)
            {
                this.points[i].rotate(theta, xCenter, yCenter);
            }
        }

        public Substroke cloneRotate(double theta, float xCenter, float yCenter)
        {
            Substroke clone = this.Clone();
            clone.rotate(theta, xCenter, yCenter);
            return clone;
        }
    }
}
