using System;
using System.IO;
using System.Xml;
using System.Collections;

using Converter;
using Sketch;

namespace XmlUpdater
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ArrayList argArray = new ArrayList(args);			
			int numArgs = argArray.Count;

			string[] files;
 
			if (numArgs == 0)
			{
				Console.WriteLine("*****************************************************************");
				Console.WriteLine("*** XmlUpdater.exe");
				Console.WriteLine("*** by Aaron Wolin, Devin Smith, Jason Fennell, and Max Pflueger.");
				Console.WriteLine("*** Harvey Mudd College, Claremont, CA 91711.");
				Console.WriteLine("*** Sketchers 2007.");
				Console.WriteLine("***");
				Console.WriteLine("*** Usage: XmlUpdater.exe (-c | -d directory | -r) (-o)");
				Console.WriteLine("*** Usage: XmlUpdater.exe input1.xml [input2.xml ...] (-o)");
				Console.WriteLine("***");
				Console.WriteLine("*** -c: convert all files in current directory");
				Console.WriteLine("*** -d directory: convert all files in the specified directory");
				Console.WriteLine("*** -r: recursively convert files from the current directory");
				Console.WriteLine("*** -o: overwrite current files");
				
				return;
			}
			// Update everything in this directory
			else if (argArray.Contains("-c"))
			{
				files = Directory.GetFiles(Directory.GetCurrentDirectory());
			}
			// Update everything in specified directory
			else if (argArray.Contains("-d"))
			{
				// Are we in range?
				if (argArray.IndexOf("-d") + 1  >= argArray.Count)
				{
					Console.Error.WriteLine("No directory specified.");
					return;
				}
				// Does the directory exist?
				else if(!Directory.Exists((string)argArray[argArray.IndexOf("-d") + 1]))
				{
					Console.Error.WriteLine("Directory doesn't exist.");
					return;
				}
				else
					files = Directory.GetFiles((string)argArray[argArray.IndexOf("-d") + 1]);
			}
			// Recursively update everything starting from the current directory
			else if (argArray.Contains("-r"))
			{
				// Get files recursively
				ArrayList rFiles = new ArrayList();
				DirSearch(Directory.GetCurrentDirectory(), ref rFiles);
			
				// Get current directory files
				string [] currDir = Directory.GetFiles(Directory.GetCurrentDirectory());

				files = new string[rFiles.Count + currDir.Length];

				// Populate both recursive and current into files
				int current;
				for (current = 0; current < currDir.Length; ++current)
					files[current] = currDir[current];

				foreach (string s in rFiles)
				{
					files[current++] = s;
				}
			}
			// Update only the specified files
			else
			{
				files = args;
			}

			string subDir = "updateXml";
			Directory.CreateDirectory(subDir);
						
			Converter.MakeXML newXml;
			int i;
			string output;

			foreach (string input in files)
			{
				// The file must end with .xml
				if (!input.ToLower().EndsWith(".xml"))
				{
					Console.Error.WriteLine("Unknown extension in " + input + ", must be .xml");
					continue;
				}				
				
				try
				{
					Console.Write("Trying " + input + " ");
				
					Console.Write("...");
						
					if (argArray.Contains("-o"))
						output = Path.GetFullPath(input);
					else
						output = subDir + "\\" + Path.GetFileName(input);
					
					Sketch.Sketch sketch = new ReadXML(input).Sketch;

					UpdateSketch(sketch);
			
					newXml = new Converter.MakeXML(sketch);
					newXml.WriteXML(output);
					
					Console.WriteLine();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.InnerException);
					Console.WriteLine(e.StackTrace);
					Console.ReadLine();
					continue;
				}				
			}
		}


		/// <summary>
		/// Perform a recursive directory search. http://support.microsoft.com/default.aspx?scid=kb;en-us;303974
		/// </summary>
		/// <param name="sDir">Directory to search recursively</param>
		/// <param name="rFiles">Array to add the files to</param>
		static void DirSearch(string sDir, ref ArrayList rFiles) 
		{
			try	
			{
				foreach (string d in Directory.GetDirectories(sDir)) 
				{
					foreach (string f in Directory.GetFiles(d, "*.*")) 
					{
						rFiles.Add(f);
					}
					DirSearch(d, ref rFiles);
				}
			}
			catch (System.Exception excpt) 
			{
				Console.WriteLine(excpt.Message);
			}
		}


		/// <summary>
		/// Updates a Sketch by calling various update methods
		/// </summary>
		/// <param name="sketch">Sketch to update</param>
		static void UpdateSketch(Sketch.Sketch sketch)
		{
			// Update Substrokes
			Substroke[] substrokesToUpdate = sketch.Substrokes;
			foreach (Substroke substroke in substrokesToUpdate)
			{
				UpdateSpatialAttributes(substroke);
			}
			
			// Update Strokes
			Stroke[] strokesToUpdate = sketch.Strokes;
			foreach (Stroke stroke in strokesToUpdate)
			{
				UpdateSpatialAttributes(stroke);
			}
			
			// Update Shapes
			Shape[] shapesToUpdate = sketch.Shapes;
			foreach (Shape shape in shapesToUpdate)
			{
				NameTypeUpdate(shape);
				UpdateSpatialAttributes(shape);
			}
		}


		/// <summary>
		/// Swap the Name and Type parameters of old XML files if necessary
		/// </summary>
		/// <param name="shape">Shape to update</param>
		static void NameTypeUpdate(Shape shape)
		{
			if ((string)shape.XmlAttrs.Name != "shape" && (string)shape.XmlAttrs.Type == "shape")
			{
				string swapString = (string)shape.XmlAttrs.Name;
				shape.XmlAttrs.Name = (string)shape.XmlAttrs.Type;
				shape.XmlAttrs.Type = swapString;
			}
		}


		/// <summary>
		/// Update the bounding box of a Shape
		/// </summary>
		/// <param name="shape">Shape to update</param>
		static void UpdateSpatialAttributes(Shape shape)
		{
			shape.UpdateAttributes(shape);

			shape.XmlAttrs.LeftX = null;
			shape.XmlAttrs.TopY = null;
		}


		/// <summary>
		/// Update the bounding box of a Stroke
		/// </summary>
		/// <param name="stroke">Stroke to update</param>
		static void UpdateSpatialAttributes(Stroke stroke)
		{
			stroke.UpdateAttributes();

			stroke.XmlAttrs.LeftX = null;
			stroke.XmlAttrs.TopY = null;
		}

		
		/// <summary>
		/// Update the bounding box of a Substroke
		/// </summary>
		/// <param name="stroke">Substroke to update</param>
		static void UpdateSpatialAttributes(Substroke substroke)
		{
			substroke.XmlAttrs.LeftX = null;
			substroke.XmlAttrs.TopY = null;
		}
	}
}
