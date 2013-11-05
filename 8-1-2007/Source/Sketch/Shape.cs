/*
 * File: Shape.cs
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
	/// Shape class.
	/// </summary>
	public class Shape : IComparable
	{
		#region INTERNALS

		/// <summary>
		/// Substrokes
		/// </summary>
		private List<Substroke> substrokes;

		/// <summary>
		/// Shapes
		/// </summary>
		private List<Shape> shapes;

		/// <summary>
		/// Parent shape
		/// </summary>
		private Shape parentShape;

		/// <summary>
		/// Xml attributes of the Shape
		/// </summary>
		public XmlStructs.XmlShapeAttrs XmlAttrs;

		#endregion

		#region CONSTRUCTORS
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Shape() :
			this(new List<Shape>(), new List<Substroke>(), new XmlStructs.XmlShapeAttrs())
		{
			//Calls the main constructor
		}

		/// <summary>
		/// Constructor. Create from Shape.
		/// </summary>
		/// <param name="shape">A Shape.</param>
		public Shape(Shape shape)
			: this(shape.shapes.GetRange(0, shape.shapes.Count), 
                shape.substrokes.GetRange(0, shape.substrokes.Count), shape.XmlAttrs.Clone())
		{
			// Calls the main constructor
		}


		/// <summary>
		/// Construct a Shape with given Shapes, Substrokes, and XML attributes.
		/// </summary>
		/// <param name="shapes">Shapes to add</param>
		/// <param name="substrokes">Substrokes to add</param>
		/// <param name="XmlAttrs">XML attributes of the Shape</param>
		public Shape(Shape[] shapes, Substroke[] substrokes, XmlStructs.XmlShapeAttrs XmlAttrs)
			: this(new List<Shape>(shapes), new List<Substroke>(substrokes), XmlAttrs)
		{
			// Calls the main constructor
		}

	
		/// <summary>
		/// Construct a Shape with given Shapes, Substrokes, and XML attributes.
		/// </summary>
		/// <param name="shapes">Shapes to add</param>
		/// <param name="substrokes">Substrokes to add</param>
		/// <param name="XmlAttrs">XML attributes of the Shape</param>
		public Shape(List<Shape> shapes, List<Substroke> substrokes, XmlStructs.XmlShapeAttrs XmlAttrs)
		{
			this.shapes = new List<Shape>();
			this.substrokes = new List<Substroke>();
			this.parentShape = null;

			this.XmlAttrs = XmlAttrs;

            this.AddSubstrokes(substrokes);
			
            this.AddShapes(shapes);
		}

		#endregion

		#region ADD TO SHAPE

		#region ADD SUBSTROKE(S)

		/// <summary>
		/// Add a Substroke to this shape
		/// </summary>
		/// <param name="substroke">A Substroke</param>
		public void AddSubstroke(Substroke substroke)
		{
			//Console.WriteLine(this.XmlAttrs.Id + ": ADD :" + substroke.XmlAttrs.Id);
            if (!this.substrokes.Contains(substroke))
            {
                int low = 0;
                int high = this.substrokes.Count - 1;
                int mid;
                while (low <= high)
                {
                    mid = (high - low) / 2 + low;
                    if (substroke.XmlAttrs.Time.Value < this.substrokes[mid].XmlAttrs.Time.Value)
                        high = mid - 1;
                    else
                        low = mid + 1;
                }

                this.substrokes.Insert(low, substroke);

                if (substroke.ParentShapes != null && !substroke.ParentShapes.Contains(this))
                    substroke.ParentShapes.Add(this);

                UpdateAttributes(this);
            }
            else
            {
                throw new Exception("substroke already exists in shape");
            }
		}

		
		/// <summary>
		/// Add Substrokes to this shape
		/// </summary>
		/// <param name="substrokes">The Substrokes</param>
		public void AddSubstrokes(Substroke[] substrokes)
		{
			int length = substrokes.Length;
			for (int i = 0; i < length; ++i)
				this.AddSubstroke(substrokes[i]);
		}

		
		/// <summary>
		/// Add Substrokes to this shape
		/// </summary>
		/// <param name="substrokes">The Substrokes</param>
		public void AddSubstrokes(List<Substroke> substrokes)
		{
			int length = substrokes.Count;
			for (int i = 0; i < length; ++i)
				this.AddSubstroke(substrokes[i]);
		}

		#endregion

		#region ADD SHAPE(S)

		/// <summary>
		/// Add a Shape to this Shape
		/// </summary>
		/// <param name="shape">The Shape to add</param>
		public void AddShape(Shape shape)
		{
			if (!this.shapes.Contains(shape))
			{
				int low = 0;
				int high = this.shapes.Count - 1;
				int mid;
				while (low <= high)
				{
					mid = (high - low) / 2 + low;
					if (shape.XmlAttrs.Time.Value < this.shapes[mid].XmlAttrs.Time.Value)
						high = mid - 1;
					else
						low = mid + 1;
				}
		
				this.shapes.Insert(low, shape);
				shape.ChangeParentShape(this);

				UpdateAttributes(this);
			}
		}


		/// <summary>
		/// Add Shapes to this Shape
		/// </summary>
		/// <param name="shapes">The Shapes</param>
		public void AddShapes(Shape[] shapes)
		{
			int length = shapes.Length;
			for (int i = 0; i < length; ++i)
				this.AddShape(shapes[i]);
		}


		/// <summary>
		/// Add Shapes to this Shape
		/// </summary>
		/// <param name="shapes">The Shapes</param>
		public void AddShapes(List<Shape> shapes)
		{
			int length = shapes.Count;
			for (int i = 0; i < length; ++i)
				this.AddShape(shapes[i]);
		}


		#endregion

		#endregion

		#region REMOVE FROM SHAPE

		#region REMOVE SUBSTROKE(S)

		/// <summary>
		/// Removes a Substroke from the Shape.
		/// </summary>
		/// <param name="substroke">Substroke to remove</param>
		/// <returns>True iff the Substroke is removed</returns>
		public bool RemoveSubstroke(Substroke substroke)
		{
			//Console.WriteLine(this.XmlAttrs.Id + ": DEL :" + substroke.XmlAttrs.Id);
			if (this.substrokes.Contains(substroke))
			{
				substroke.ParentShapes.Remove(this);
				this.substrokes.Remove(substroke);

				ReapIfEmpty();

				UpdateAttributes(this);
               
				
				return true;
			}
			else
			{
				return false;
			}
		}

		
		/// <summary>
		/// Removes an ArrayList of Substrokes from the Shape.
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
					Console.WriteLine("Substroke " + currSubstroke.XmlAttrs.Id.Value + " not removed!");
				}
			}

			return completelyRemoved;
		}

		#endregion

		#region REMOVE SHAPE(S)

		/// <summary>
		/// Removes a Shape from the Shape.
		/// </summary>
		/// <param name="shape">Shape to remove</param>
		/// <returns>True iff the Shape is removed</returns>
		public bool RemoveShape(Shape shape)
		{
			if (this.shapes.Contains(shape))
			{
				shape.ParentShape = null;
				this.shapes.Remove(shape);
				UpdateAttributes(this);
				
				return true;
			}
			else
			{
				return false;
			}
		}

		
		/// <summary>
		/// Removes an ArrayList of Shapes from the Shape.
		/// </summary>
		/// <param name="shapes">Shapes to remove</param>
		/// <returns>True iff all Shapes are removed</returns>
		public bool RemoveShapes(List<Shape> shapes)
		{
			bool completelyRemoved = true;

            Shape currShape;
            int len = shapes.Count;
			for (int i = 0; i < len; ++i)
			{
				currShape = shapes[i];

				if (!RemoveShape(currShape))
				{
					completelyRemoved = false;
					Console.WriteLine("Shape " + currShape.XmlAttrs.Id.Value + " not removed!");
				}
			}

			return completelyRemoved;
		}

		#endregion

		#endregion

		#region UPDATE ATTRIBUTES

		/// <summary>
		/// Updates the spatial attributes of the Shape, such as the origin
		/// and width of the shape
		/// </summary>
		public void UpdateAttributes(Shape shape)
		{
            float minX = Single.PositiveInfinity;
            float maxX = Single.NegativeInfinity;

			float minY = Single.PositiveInfinity;
			float maxY = Single.NegativeInfinity;

			// Cycle through the Substrokes within the Shape
			foreach (Substroke s in shape.Substrokes)
			{
				minX = Math.Min(minX, s.XmlAttrs.X.Value);
				minY = Math.Min(minY, s.XmlAttrs.Y.Value);

				maxX = Math.Max(maxX, s.XmlAttrs.X.Value + s.XmlAttrs.Width.Value);
				maxY = Math.Max(maxY, s.XmlAttrs.Y.Value + s.XmlAttrs.Height.Value);
			}

			// Cycle through the other Shapes within the Shape
			foreach (Shape sh in shape.Shapes)
			{
				if (sh.XmlAttrs.X == null || sh.XmlAttrs.Y == null ||
					sh.XmlAttrs.Width == null || sh.XmlAttrs.Height == null)
				{
					// Since sh is an object, it gets updated when this is called
					UpdateAttributes(sh);
				}

				minX = Math.Min(minX, sh.XmlAttrs.X.Value);
				minY = Math.Min(minY, sh.XmlAttrs.Y.Value);

				maxX = Math.Max(maxX, sh.XmlAttrs.X.Value + sh.XmlAttrs.Width.Value);
				maxY = Math.Max(maxY, sh.XmlAttrs.Y.Value + sh.XmlAttrs.Height.Value);
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
				this.XmlAttrs.Start = this.substrokes[0].XmlAttrs.Id;
				this.XmlAttrs.End = this.substrokes[this.substrokes.Count - 1].XmlAttrs.Id;
				this.XmlAttrs.Time = this.substrokes[this.substrokes.Count - 1].XmlAttrs.Time;
			}
			else
			{
				this.XmlAttrs.Start = null;
				this.XmlAttrs.End = null;
				this.XmlAttrs.Time = null;
			}

			if (this.shapes.Count > 0 && 
				this.shapes[0].XmlAttrs.Time.Value < this.XmlAttrs.Time.Value)
			{
				this.XmlAttrs.Time = this.shapes[0].XmlAttrs.Time;
			}
		}

		#endregion

		private void ReapIfEmpty()
		{
			if (this.substrokes.Count == 0 && this.shapes.Count == 0)
			{
				// Do some reaping here...
			}
		}

		#region GETTERS & SETTERS

		/// <summary>
		/// Get Substrokes
		/// </summary>
		public Substroke[] Substrokes
		{
			get
			{
				return this.substrokes.ToArray();
			}
		}

        public List<Substroke> SubstrokesL
        {
            get
            {
                return this.substrokes;
            }
        }

		/// <summary>
		/// Get Shapes
		/// </summary>
		public Shape[] Shapes
		{
			get
			{
				return this.shapes.ToArray();
			}
		}

        public List<Shape> ShapesL
        {
            get
            {
                return this.shapes;
            }
        }


		/// <summary>
		/// Get or set ParentShape
		/// </summary>
		internal Shape ParentShape
		{
			get
			{
				if (this.parentShape == null)
					return new Shape();
				else
                    return this.parentShape;
			}

			set
			{
				this.parentShape = value;
			}
		}

		
		#endregion

		#region OTHER

		/// <summary>
		/// Change this parent shape
		/// </summary>
		/// <param name="shape"></param>
		public void ChangeParentShape(Shape shape)
		{
			this.parentShape = shape;
		}


		/// <summary>
		/// Compute the Clone of this shape.
		/// </summary>
		/// <returns>The Clone of this shape.</returns>
		public Shape Clone()
		{
			return new Shape(this);
		}


		/// <summary>
		/// Compare this Shape to another based on time.
		/// Returns less than 0 if this time is less than the other's.
		/// Returns 0 if this time is equal to the other's.
		/// Returns greater than if this time is greater than the other's.
		/// </summary>
		/// <param name="obj">The other Shape to compare this one to</param>
		/// <returns>An integer indicating how the Shape times compare</returns>
		int System.IComparable.CompareTo(Object obj)
		{
			return (int)(this.XmlAttrs.Time.Value - ((Stroke)obj).XmlAttrs.Time.Value);
		}

		#endregion
	}
}
