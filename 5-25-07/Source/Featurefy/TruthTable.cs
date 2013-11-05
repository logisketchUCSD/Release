using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sketch;

namespace Featurefy
{
    /// <summary>
    /// author: Sara
    /// USED FOR TRUTH TABLES!
    /// Goals for this class:
    /// -identify the shapes in the truth table
    /// -classify the shapes by type
    /// -classify the shapes into columns if they are not dividers
    /// -eventually classify columns as input/output
    /// </summary>
    public class TruthTable
    {
        /// <summary>
        /// data member sketch
        /// </summary>
        private Sketch.Sketch sketch;

        /// <summary>
        /// constructor for column; creates a blank sketch
        /// </summary>
        public TruthTable()
        {
            this.sketch = new Sketch.Sketch();
        }

        /// <summary>
        /// returns true if two strokes overlap and false otherwise
        /// </summary>
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        /// <returns></returns>
        public bool isOverlapping(Sketch.Stroke st1, Sketch.Stroke st2)
        {
            for(int i = 0; i < st1.Points.Length; i++)
            {
                for (int j = 0; j < st1.Points.Length; j++)
                {
                    if (st1.Points[i].XmlAttrs.X == st2.Points[i].XmlAttrs.X && st1.Points[i].XmlAttrs.Y == st2.Points[i].XmlAttrs.Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /*public ArrayList findShapes()
        {
            for(int i = 0; i < this.sketch.Substrokes.Length; i++)
            {
                for(int j = 0; j < this.sketch.Substrokes.Length; j++)
                {
                    if (this.sketch.Substrokes[i].Equals(this.sketch.Substrokes[j]) == false)
                    {
                        if (this.isOverlapping(this.sketch.Substrokes[i], this.sketch.Substrokes[j]))
                        {

            { }
        }*/

        /// <summary>
        /// finds the midpoint of a shape
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        public Sketch.Point shapeMidPoint(Sketch.Shape sh)
        {
            Sketch.Point p = new Sketch.Point();
            p.XmlAttrs.X = (double)sh.XmlAttrs.X + (double)sh.XmlAttrs.Width / 2;
            p.XmlAttrs.Y = (double)sh.XmlAttrs.Y + (double)sh.XmlAttrs.Height / 2;

            return p;
        }

        /// <summary>
        /// returns the type of a shape
        /// based on data about its height and width and the number of substrokes
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        public string assignType(Sketch.Shape sh)
        {
            double ratio = (double)sh.XmlAttrs.Height / (double)sh.XmlAttrs.Width;
            if (ratio > 10.0 || ratio < .1)
            {
                return "Divider";
            }
            if ((ratio > 5.0 || ratio < .2) && sh.Substrokes.Length == 1)
            {
                return "True";
            }
            if (ratio < 2.0 && ratio > .5 && sh.Substrokes.Length == 1)
            {
                return "False";
            }
            else
            {
                return "Label";
            }
        }

        /// <summary>
        /// sorts the list of shapes by the x-coordinate of their midpoints
        /// </summary>
        /// <returns></returns>
        /*public Array sortByX()
        {
            Array shapes = new Array[this.sketch.Shapes.Length];
            for (int i = 0; i < this.sketch.Shapes.Length; i++)
            {
                shapes = this.sketch.Shapes[i];
            }

            return Array.Sort(shapes);
        }*/

        /// <summary>
        /// will assign a column number to each shape in the sketch
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        public int assignColumn(Sketch.Shape sh)
        {
            return 0;
        }

    }
}
