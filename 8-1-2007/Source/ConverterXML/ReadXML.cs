/*
 * File: ReadXML.cs
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
using System.Xml;
using Sketch;

namespace ConverterXML
{
    /// <summary>
    /// ReadXML class. Takes in a MIT XML file path and name and creates a Sketch.Sketch object
    /// from the information.
    /// </summary>
    public class ReadXML
    {
        #region INTERNALS

        /// <summary>
        /// The Sketch to be created from the XML file
        /// </summary>
        private Sketch.Sketch sketch;

        private Dictionary<Guid, XmlStructs.XmlShapeAttrs> shapeToAttrs;
        private Dictionary<Guid, List<Guid>> shapeToShapeIDs;
        private Dictionary<Guid, List<Guid>> shapeToStrokeIDs;
        private Dictionary<Guid, List<Guid>> shapeToSubstrokeIDs;

        private Dictionary<Guid, XmlStructs.XmlShapeAttrs> strokeToAttrs;        
        private Dictionary<Guid, List<Guid>> strokeToSubstrokeIDs;

        private Dictionary<Guid, XmlStructs.XmlShapeAttrs> substrokeToAttrs;
        private Dictionary<Guid, List<Guid>> substrokeToPointIDs;

        /// <summary>
        /// A Hashtable linking Point Ids to Points
        /// </summary>
        //private Hashtable pointsHT;
        private Dictionary<Guid, Point> pointsHT;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Constructor. Creates a Sketch from an XML file.
        /// </summary>
        /// <param name="filepath">File path</param>
        public ReadXML(string filepath)
        {
            init();
            this.CreateSketch(filepath);
        }

        /// <summary>
        /// Constructor.  Creates a sketch from an XMLTextReader 
        /// (a reader for an XML source).
        /// </summary>
        /// <param name="xmlReader">The reader to read in XML data.</param>
        public ReadXML(XmlTextReader xmlReader)
        {
            init();
            this.CreateSketch(xmlReader);
        }

        /// <summary>
        /// Initializes data structure members for holding
        /// the sketch and associated shape data.
        /// </summary>
        private void init()
        {
            this.sketch = new Sketch.Sketch();

            this.shapeToAttrs           = new Dictionary<Guid, XmlStructs.XmlShapeAttrs>();
            this.shapeToShapeIDs        = new Dictionary<Guid, List<Guid>>();
            this.shapeToStrokeIDs       = new Dictionary<Guid, List<Guid>>();
            this.shapeToSubstrokeIDs    = new Dictionary<Guid, List<Guid>>();

            this.strokeToAttrs          = new Dictionary<Guid, XmlStructs.XmlShapeAttrs>();
            this.strokeToSubstrokeIDs    = new Dictionary<Guid, List<Guid>>(); 
            
            this.substrokeToAttrs       = new Dictionary<Guid, XmlStructs.XmlShapeAttrs>();
            this.substrokeToPointIDs    = new Dictionary<Guid, List<Guid>>();

            this.pointsHT               = new Dictionary<Guid, Point>();
        }

        #endregion

        #region IO INITIALIZATION

        /// <summary>
        /// Gets an IO stream for the file.
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <returns>System.IO.Streamreader for the file</returns>
        private System.IO.StreamReader getStreamReader(string filepath)
        {
            return new System.IO.StreamReader(filepath);
        }


        /// <summary>
        /// Gets an XML Text reader for the IO stream
        /// </summary>
        /// <param name="stream">Stream to get an XML reader for</param>
        /// <returns>An XML text reader for the IO stream</returns>
        private XmlTextReader getXmlTextReader(System.IO.StreamReader stream)
        {
            XmlTextReader reader = new XmlTextReader(stream);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            return reader;
        }

        #endregion

        #region READ FILE & CREATE SKETCH

        /// <summary>
        /// Constructs a Sketch from the given filename.
        /// </summary>
        /// <param name="filename">Filename/path for the MIT XML file</param>
        private void CreateSketch(string filename)
        {
            System.IO.StreamReader stream = this.getStreamReader(filename);
            XmlTextReader xmlReader = this.getXmlTextReader(stream);
            CreateSketch(xmlReader);
            stream.Close();
        }

        /// <summary>
        /// Constructs a sketch from the given stream
        /// </summary>
        /// <param name="reader">The XML reader that will read in XML data</param>
        private void CreateSketch(XmlTextReader reader)
        {
            // Read all the data from the XML file
            this.ReadSketchData(reader);

            List<Shape> shapes = new List<Shape>();
            List<Stroke> strokes = new List<Stroke>();

            Dictionary<Guid, Stroke> allStrokes = new Dictionary<Guid, Stroke>();
            Dictionary<Guid, Substroke> allSubstrokes = new Dictionary<Guid, Substroke>();
            

            //Hashtable allStrokes = new Hashtable();
            //Hashtable allSubstrokes = new Hashtable();

            // Cycle through the Stroke Id's pulled from the file
         
            foreach (KeyValuePair<Guid, XmlStructs.XmlShapeAttrs> stroke in this.strokeToAttrs)
            //foreach(KeyValuePair<Guid, ArrayList> stroke in this.strokeToSubstrokesHT)
            {                
                XmlStructs.XmlShapeAttrs strokeAttrs = stroke.Value;
                List<Guid> substrokeIds = this.strokeToSubstrokeIDs[stroke.Key];
                
                List<Substroke> substrokes = new List<Substroke>();
                
                // Cycle through the Substroke Id's pulled from the file
                int len = substrokeIds.Count;
                for (int i = 0; i < len; ++i)
                {
                    List<Guid> ptsArray = this.substrokeToPointIDs[substrokeIds[i]];
                    XmlStructs.XmlShapeAttrs substrokeAttrs = this.substrokeToAttrs[substrokeIds[i]];

                    List<Point> points = new List<Point>();

                    // Get all of the Points associated with the Substroke
                    int len2 = ptsArray.Count;
                    for (int k = 0; k < len2; ++k)
                    {
                        points.Add(this.pointsHT[ptsArray[k]]);
                    }

                    // Create a new Substroke and add it to the local Hashtable and Stroke ArrayList
                    Substroke newSubstroke = new Substroke(points, substrokeAttrs);

                    allSubstrokes.Add(newSubstroke.XmlAttrs.Id.Value, newSubstroke);
                    substrokes.Add(newSubstroke);
                }

                // Add a new Stroke
                Stroke newStroke = new Stroke(substrokes, strokeAttrs);
                allStrokes.Add(newStroke.XmlAttrs.Id.Value, newStroke);
                strokes.Add(newStroke);
            }

            // Cycle through the Shape Id's pulled from the file
            //foreach (KeyValuePair<Guid, ArrayList> shape in this.shapeToArgsHT)
            foreach(KeyValuePair<Guid, XmlStructs.XmlShapeAttrs> shape in this.shapeToAttrs)
            {
                /*
                XmlStructs.XmlShapeAttrs shapeAttrs = shape.Value;
                
                List<Shape> shapeIds = this.shapeToShapeIDs[shape.Key];
                List<Stroke> strokeIds = this.shapeToStrokeIDs[shape.Key];
                List<Substroke> substrokeIds = this.shapeToSubstrokeIDs[shape.Key];
                */
                
                List<Shape> currShapes;
                List<Substroke> substrokes = RecShape(shape.Key, allStrokes, allSubstrokes, out currShapes);

                // Add a new Shape
                shapes.Add(new Shape(currShapes, substrokes, shape.Value));
            }

            // Add the Shapes and Strokes to the Sketch
            sketch.AddStrokes(strokes);
            sketch.AddShapes(shapes);
            
        }


        /// <summary>
        /// Recursively goes through a Shape in the Shapes hashtable and pulls out 
        /// all of the Substrokes associated with the Shape.
        /// 
        /// NOTE: Shapes can currently have Shape, Stroke, and Substroke arguments for subelements
        /// (but no Points)
        /// </summary>
        /// <param name="id">Shape's Id</param>
        /// <param name="allStrokes">A hashtable containing all of the file's Strokes (Id -> Stroke)</param>
        /// <param name="allSubstrokes">A hashtable containing all of the file's Substrokes (Id -> Substroke)</param>
        /// <param name="shapes">An ArrayList corresponding to the Shapes within this Shape</param>
        /// <returns>An ArrayList of Substrokes</returns>
        private List<Substroke> RecShape(System.Guid id, Dictionary<Guid, Stroke> allStrokes, Dictionary<Guid, Substroke> allSubstrokes, out List<Shape> shapes)
        {
            List<Substroke> substrokes = new List<Substroke>();
            shapes = new List<Shape>();

            //ArrayList shapeValue = (ArrayList)this.shapeToArgsHT[id];

            List<Guid> shapeIds = this.shapeToShapeIDs[id];
            List<Guid> strokeIds = this.shapeToStrokeIDs[id];
            List<Guid> substrokeIds = this.shapeToSubstrokeIDs[id];

            // Add all of the Substroke arguments for the Shape
            int i, len = substrokeIds.Count;
            for (i = 0; i < len; ++i)
            {
                Substroke tempSubstroke = allSubstrokes[substrokeIds[i]];

                if (tempSubstroke == null)
                    throw new XmlException("Invalid Format: Substroke found that is not part of a Stroke");
                else
                    substrokes.Add(tempSubstroke);
            }

            // Go through all of the Stroke arguments for the Shape and pull out the Substrokes
            len = strokeIds.Count;
            for (i = 0; i < len; ++i)
            {
                Stroke tempStroke = allStrokes[strokeIds[i]];
                Substroke[] tempSubstrokes = tempStroke.Substrokes;

                int len2 = tempSubstrokes.Length;
                for (int k = 0; k < len; ++k)
                    substrokes.Add(tempSubstrokes[k]);
            }

            // If no Shapes are left, return the Substrokes
            if (shapeIds.Count == 0)
                return substrokes;
            else
            {
                // Recursively go through all of the Shapes
                len = shapeIds.Count;
                for (i = 0; i < len; ++i)
                {
                    List<Shape> tempShapes;
                    List<Substroke> tempSubstrokes = RecShape(shapeIds[i], allStrokes, allSubstrokes, out tempShapes);
                    XmlStructs.XmlShapeAttrs tempShapeAttrs = this.shapeToAttrs[shapeIds[i]];

                    //for (int k = 0; k < tempSubstrokes.Count; k++)
                    //	substrokes.Add((Substroke)tempSubstrokes[k]);*/

                    shapes.Add(new Shape(tempShapes, tempSubstrokes, tempShapeAttrs));
                }
            }

            return substrokes;
        }

        #endregion

        #region READ XML ELEMENTS

        /// <summary>
        /// Reads the Sketch XML attributes and cycles through the XML elements
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadSketchData(XmlTextReader reader)
        {
            reader.MoveToContent();

            if (reader.Name != "sketch")
            {
                throw new XmlException("MIT XML must have a sketch as its root element");
            }

            // Modify the Sketch's attributes
            Sketch.XmlStructs.XmlSketchAttrs sketchAttrs = new Sketch.XmlStructs.XmlSketchAttrs();
            sketchAttrs.Id = new Guid(reader.GetAttribute("id"));

            if (reader.MoveToAttribute("units"))
            {
                sketchAttrs.Units = reader.Value;
            }
            else
            {
                sketchAttrs.Units = "himetric";
            }

            sketch.XmlAttrs = sketchAttrs;

            // Go past the sketch tag and start reading subelements
            reader.Read();

            // Now read the subelements
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "sketcher":
                            ReadSketcher(reader);
                            break;
                        case "study":
                            ReadStudy(reader);
                            break;
                        case "domain":
                            ReadDomain(reader);
                            break;
                        case "point":
                            ReadPoint(reader);
                            break;
                        case "shape":
                            ReadShape(reader);
                            break;
                        case "edit":
                            ReadEdit(reader);
                            break;
                        case "speech":
                            ReadSpeech(reader);
                            break;
                        case "mediaInfo":
                            ReadMediaInfo(reader);
                            break;
                    }
                }
                else
                {
                    string message = "Invalid file format: " + reader.NodeType + " not expected";
                    throw new XmlException(message);
                }
            }

        }


        // All of the functions below read in the corresponding part of the file
        // They assume that the reader starts pointing to the element tag of the
        // correct type and position the reader at the end tag


        /// <summary>
        /// Reads the Sketch XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadSketcher(XmlTextReader reader)
        {
            reader.ReadStartElement("sketcher");

            Sketch.XmlStructs.XmlSketcherAttrs sketcherAttrs = new Sketch.XmlStructs.XmlSketcherAttrs();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "id")
                {
                    sketcherAttrs.Id = new Guid(reader.ReadElementString());
                }
                else if (reader.Name == "dpi")
                {
                    sketcherAttrs.XDpi = Convert.ToSingle(reader.GetAttribute("xdpi"));
                    sketcherAttrs.YDpi = Convert.ToSingle(reader.GetAttribute("ydpi"));
                    reader.Read();
                }
                else if (reader.Name == "nickname")
                {
                    sketcherAttrs.Nickname = reader.ReadElementString();
                }
            }

            // TODO: Do something with the Sketcher
            reader.ReadEndElement();
        }


        /// <summary>
        /// Reads the Study XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadStudy(XmlTextReader reader)
        {
            Sketch.XmlStructs.XmlStudyAttrs studyAttrs = new Sketch.XmlStructs.XmlStudyAttrs();
            studyAttrs.Name = reader.ReadElementString();

            // TODO: Do something with the Study
        }


        /// <summary>
        /// Reads the Domain XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadDomain(XmlTextReader reader)
        {
            Sketch.XmlStructs.XmlDomainAttrs domainAttrs = new Sketch.XmlStructs.XmlDomainAttrs();
            domainAttrs.Name = reader.ReadElementString();

            // TODO: Do something with the Domain
        }


        /// <summary>
        /// Reads the Point XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadPoint(XmlTextReader reader)
        {
            Sketch.XmlStructs.XmlPointAttrs pointAttrs = new Sketch.XmlStructs.XmlPointAttrs();

            // All of these attributes are required for Points
            string x = reader.GetAttribute("x");
            string y = reader.GetAttribute("y");
            string pressure = reader.GetAttribute("pressure");
            string time = reader.GetAttribute("time");
            string id = reader.GetAttribute("id");
            string name = reader.GetAttribute("name");

            if ((x != null) && (y != null) && (pressure != null) && (time != null) && (id != null) && (name != null))
            {
                pointAttrs.X = Convert.ToSingle(x);
                pointAttrs.Y = Convert.ToSingle(y);
                pointAttrs.Pressure = Convert.ToUInt16(pressure);
                pointAttrs.Time = Convert.ToUInt64(time);
                pointAttrs.Id = new Guid(id);
                pointAttrs.Name = name;
            }
            else
            {
                throw new ApplicationException("Invalid Format: Points must contain an X, Y, Pressure, Time, Id, and Name value");
            }

            // Create the new Point and add it to the corresponding Hashtable
            Point point = new Point(pointAttrs);
            if (!this.pointsHT.ContainsKey(point.XmlAttrs.Id.Value))
                this.pointsHT.Add(point.XmlAttrs.Id.Value, point);

            reader.Read();
        }


        /// <summary>
        /// Reads the Shape XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadShape(XmlTextReader reader)
        {
            Sketch.XmlStructs.XmlShapeAttrs shapeAttrs = new Sketch.XmlStructs.XmlShapeAttrs();

            // Required MIT XML attributes
            string type = reader.GetAttribute("type");
            string name = reader.GetAttribute("name");
            string id = reader.GetAttribute("id");
            string time = reader.GetAttribute("time");

            if ((type != null) && (name != null) && (id != null) && (time != null))
            {
                shapeAttrs.Type = type;
                shapeAttrs.Name = name;
                shapeAttrs.Id = new Guid(id);
                shapeAttrs.Time = Convert.ToUInt64(time);
            }
            else
            {
                string bad = "";
                if (type == null)
                    bad += "Type ";
                if (name == null)
                    bad += "Name ";
                if (id == null)
                    bad += "ID ";
                if (time == null)
                    bad += "Time ";

                throw new ApplicationException("Invalid Format: Shapes must contain a Type, Name, Id, and Time. Missing " + bad);
            }

            // Get the rest of the attributes, if they are there
            // We use temporary strings here so that we can check if the attribute is <null>
            string author = reader.GetAttribute("author");
            string color = reader.GetAttribute("color");
            string height = reader.GetAttribute("height");
            string width = reader.GetAttribute("width");
            string area = reader.GetAttribute("area");
            string laysInk = reader.GetAttribute("laysInk");
            string orientation = reader.GetAttribute("orientation");
            string penTip = reader.GetAttribute("penTip");
            string penWidth = reader.GetAttribute("penWidth");
            string penHeight = reader.GetAttribute("penHeight");
            string raster = reader.GetAttribute("raster");
            string substrokeOf = reader.GetAttribute("substrokeOf");
            string p1 = reader.GetAttribute("p1");
            string p2 = reader.GetAttribute("p2");
            string x = reader.GetAttribute("x");
            string y = reader.GetAttribute("y");
            string probability = reader.GetAttribute("probability");
            string text = reader.GetAttribute("text");
            string leftx = reader.GetAttribute("leftx");
            string topy = reader.GetAttribute("topy");
            string control1 = reader.GetAttribute("control1");
            string control2 = reader.GetAttribute("control2");
            string start = reader.GetAttribute("start");
            string end = reader.GetAttribute("end");
            string source = reader.GetAttribute("source");

            if (author != null) shapeAttrs.Author = new Guid(author);
            if (color != null) shapeAttrs.Color = Convert.ToInt32(color);
            if (height != null) shapeAttrs.Height = Convert.ToSingle(height);
            if (width != null) shapeAttrs.Width = Convert.ToSingle(width);
            if (area != null) shapeAttrs.Area = Convert.ToSingle(area);// Convert.ToDouble(area);
            if (laysInk != null) shapeAttrs.LaysInk = Convert.ToBoolean(laysInk);
            if (orientation != null) shapeAttrs.Orientation = Convert.ToSingle(orientation);
            if (penTip != null) shapeAttrs.PenTip = penTip;
            if (penWidth != null) shapeAttrs.PenWidth = Convert.ToSingle(penWidth);// penWidth;
            if (penHeight != null) shapeAttrs.PenHeight = Convert.ToSingle(penHeight);// penHeight;
            if (raster != null) shapeAttrs.Raster = raster;
            if (substrokeOf != null) shapeAttrs.SubstrokeOf = new Guid(substrokeOf);
            if (p1 != null) shapeAttrs.P1 = p1;
            if (p2 != null) shapeAttrs.P2 = p2;
            if (x != null) shapeAttrs.X = Convert.ToSingle(x);
            if (y != null) shapeAttrs.Y = Convert.ToSingle(y);
            if (probability != null) shapeAttrs.Probability = Convert.ToSingle(probability);
            if (text != null) shapeAttrs.Text = text;
            if (leftx != null) shapeAttrs.LeftX = Convert.ToSingle(leftx);
            if (topy != null) shapeAttrs.TopY = Convert.ToSingle(topy);
            if (control1 != null) shapeAttrs.Control1 = control1;// Convert.ToDouble(control1);
            if (control2 != null) shapeAttrs.Control2 = control2;// Convert.ToDouble(control2);
            if (start != null) shapeAttrs.Start = new Guid(start);
            if (end != null) shapeAttrs.End = new Guid(end);
            if (source != null) shapeAttrs.Source = source;

            // Some local variables concerning the arguments
            List<Guid> args = new List<Guid>();
            string uniformArgtype = null;

            bool isShape = false;
            List<Guid> shapeArgs = new List<Guid>();
            List<Guid> strokeArgs = new List<Guid>();
            List<Guid> substrokeArgs = new List<Guid>();

            // Boolean to check if the current shape element is actually a Sketch.Shape
            if (!shapeAttrs.Type.ToLower().Equals("stroke")
                 && !shapeAttrs.Type.ToLower().Equals("substroke"))
            {
                isShape = true;
            }

            reader.Read();
            string argname = reader.Name;

            // Now read the subelements, until we get to the end of the shape
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (argname.ToLower().Equals("arg"))
                {
                    string argtype = reader.GetAttribute("type");
                    string argid = reader.ReadElementString();

                    // Set the argtype (must be the same for all regular shapes, such as Strokes and Substrokes)
                    if (uniformArgtype == null)
                        uniformArgtype = argtype;

                    // If the current element is not a Sketch.Shape
                    if (!isShape)
                    {
                        // Check to make sure that the argument is the same
                        if (uniformArgtype == argtype)
                        {
                            args.Add(new Guid(argid));
                        }
                        else
                        {
                            throw new ApplicationException("Invalid Format: Only abnormal Shapes are allowed to contain multiple arg types");
                        }
                    }
                    // If the current shape is a Sketch.Shape
                    else
                    {
                        // Add the argument to either a Substroke ArrayList or a Shape ArrayList
                        if (argtype.ToLower().Equals("stroke"))
                            strokeArgs.Add(new Guid(argid));
                        else if (argtype.ToLower().Equals("substroke"))
                            substrokeArgs.Add(new Guid(argid));
                        else
                            shapeArgs.Add(new Guid(argid));
                    }
                }

                // TODO: Handle aliases
                else if (argname.ToLower().Equals("alias"))
                {
                    string aliastype = reader.GetAttribute("type");
                    string aliasname = reader.GetAttribute("name");
                    string aliasid = reader.ReadElementString();

                    //s.Aliases.Add(new Shape.Alias(aliastype, aliasname, aliasid));
                }
            }

            // If the shape is a Substroke
            if (shapeAttrs.Type.ToLower().Equals("substroke"))
            {
                if (uniformArgtype.ToLower().Equals("point"))
                {                    
                    this.substrokeToAttrs.Add(shapeAttrs.Id.Value, shapeAttrs);
                    this.substrokeToPointIDs.Add(shapeAttrs.Id.Value, args);
                }
                else
                {
                    throw new ApplicationException("Invalid Format: Substrokes must be made of points");
                }
            }

            // If the shape is a Stroke
            else if (shapeAttrs.Type.ToLower().Equals("stroke"))
            {
                // If the arguments are Points we need to create a corresponding Substroke
                if (uniformArgtype.ToLower().Equals("point"))
                {
                    Sketch.XmlStructs.XmlShapeAttrs substrokeAttrs = shapeAttrs.Clone();

                    // Create a new Substroke and link it to the Points in the hashtable
                    substrokeAttrs.Id = System.Guid.NewGuid();
                    substrokeAttrs.Name = "substroke";
                    substrokeAttrs.Type = "Substroke";
                                        
                    this.substrokeToAttrs.Add(substrokeAttrs.Id.Value, substrokeAttrs);
                    this.substrokeToPointIDs.Add(substrokeAttrs.Id.Value, args);

                    List<Guid> s = new List<Guid>(1);
                    s.Add(substrokeAttrs.Id.Value);

                    this.strokeToAttrs.Add(shapeAttrs.Id.Value, shapeAttrs);
                    this.strokeToSubstrokeIDs.Add(shapeAttrs.Id.Value, s);

                }
                else if (uniformArgtype.ToLower().Equals("substroke"))
                {
                    this.strokeToAttrs.Add(shapeAttrs.Id.Value, shapeAttrs);
                    this.strokeToSubstrokeIDs.Add(shapeAttrs.Id.Value, args);
                }
                else
                {
                    throw new ApplicationException("Invalid Format: Strokes are required to have "
                        + "Substroke or Point arguments");
                }
            }

            // If the shape is a Sketch.Shape
            else
            {

                this.shapeToAttrs.Add(shapeAttrs.Id.Value, shapeAttrs);
                this.shapeToShapeIDs.Add(shapeAttrs.Id.Value, shapeArgs);
                this.shapeToStrokeIDs.Add(shapeAttrs.Id.Value, strokeArgs);
                this.shapeToSubstrokeIDs.Add(shapeAttrs.Id.Value, substrokeArgs);
            }

            // And consume the </shape>
            reader.ReadEndElement();
        }


        /// <summary>
        /// Reads the Edit XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadEdit(XmlTextReader reader)
        {
            throw new ApplicationException("Exception: Read Edit not yet supported");
        }


        /// <summary>
        /// Reads the Speech XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadSpeech(XmlTextReader reader)
        {
            throw new ApplicationException("Exception: Read Speech not yet supported");
        }


        /// <summary>
        /// Reads the Media XML attributes
        /// </summary>
        /// <param name="reader">XML text reader for the file</param>
        private void ReadMediaInfo(XmlTextReader reader)
        {
            throw new ApplicationException("Exception: Read Media Info not yet supported");
        }

        #endregion

        #region GETTERS & SETTERS

        /// <summary>
        /// Returns the Sketch created.
        /// </summary>
        public Sketch.Sketch Sketch
        {
            get
            {
                return this.sketch;
            }
        }

        #endregion
    }
}
