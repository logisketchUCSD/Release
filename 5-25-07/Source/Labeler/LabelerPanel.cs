using System;
using System.Drawing;
using System.Collections;

using Microsoft.Ink;

using Featurefy;
using Fragmenter;
using Sketch;

using CommandManagement;

namespace Labeler
{
	/// <summary>
	/// Summary description for LabelerPanel.
	/// </summary>
	public class LabelerPanel : System.Windows.Forms.Panel
	{
		private CommandManager CM;
		
		private Sketch.Sketch sketch;
		
		private Featurefy.FeatureStroke[] featureStrokes;
		
		private Microsoft.Ink.InkPicture sketchInk;

		private Microsoft.Ink.InkOverlay overlayInk;

		private DomainInfo domainInfo;

		private ArrayList lassoPoints;
		
		private Microsoft.Ink.Strokes strokesClicked;

		private bool selectionMoving;

		private bool selectionResizing;

		private bool mouseDown;

		private bool clicked;

		/// <summary>
		/// Hashtable from Microsoft Strokes to Sketch.Substrokes.
		/// </summary>
		private Hashtable mIdToSubstroke;

		private Hashtable substrokeIdToMStroke;

		private Hashtable strokeToCorners;

		private Microsoft.Ink.DrawingAttributes fragmentPtAttributes;

		private Microsoft.Ink.DrawingAttributes thickenedLabelAttributes;

		private ArrayList thickenedStrokes;

		
		private const int MARGIN = 20;

		/// <summary>
		/// Previous scaling factor. Used in resizing the Ink and the Form.
		/// </summary>
		private float prevScale = 1.0f;
		
		/// <summary>
		/// New scaling factor. Used in resizing the Ink and the Form.
		/// </summary>
		private float newScale = 1.0f;
		
		/// <summary>
		/// Total scaling factor. Used in resizing the Ink and the Form.
		/// </summary>
		private float totalScale = 1.0f;

		private float inkMovedX;

		private float inkMovedY;
		
		private System.Windows.Forms.Button labelButton;
		
		private LabelMenu labelMenu;

		private System.Windows.Forms.ToolTip toolTip;

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="originalSketch"></param>
		public LabelerPanel(CommandManager CM, DomainInfo domainInfo) :
			base()
		{
			// Set the CommandManager
			this.CM = CM;

			// Set the domainInfo
			this.domainInfo = domainInfo;
			
			// Initialize the InkOverlay
			this.sketchInk = new InkPicture();
			
			this.Resize += new EventHandler(LabelerPanel_Resize);
			
			//oInk.SelectionChanged += new InkOverlaySelectionChangedEventHandler(oInk_SelectionChanged);

			// Hashtables so we can convert between Microsoft.Ink and our Sketch
			this.mIdToSubstroke = new Hashtable();
			this.substrokeIdToMStroke = new Hashtable();

			// Hashtables so we can store what fragment points are associated with FeatureStrokes
			this.strokeToCorners = new Hashtable();

			this.thickenedStrokes = new ArrayList();

			// Initialize the drawing attributes for fragment points
			InitFragPtAttributes(Color.Red, 45);

			// Initialize the drawing attributes for thickened labels
			InitThickenedLabelAttributes(120);
			
			// Label button & menu
			InitializeLabelMenu();

			// Resize the panel
			InkResize();
		}
			

		private void InitializePanel(Sketch.Sketch sketch)
		{
			this.Enabled = false;
			this.Controls.Clear();

			Sketch.Substroke[] substrokes = sketch.Substrokes;
			this.sketchInk = new InkPicture();
			this.sketchInk.EditingMode = InkOverlayEditingMode.Select;
			
			this.mIdToSubstroke.Clear();
			this.substrokeIdToMStroke.Clear();

			// Setup the Ink in the InkOverlay
			for (int i = 0; i < substrokes.Length; i++)
			{
				System.Drawing.Point[] pts = new System.Drawing.Point[substrokes[i].Points.Length];
				
				for (int k = 0; k < pts.Length; k++)
				{
					pts[k] = new System.Drawing.Point( (int)(substrokes[i].Points[k].X), 
						(int)(substrokes[i].Points[k].Y) );
				}
			
				this.sketchInk.Ink.CreateStroke(pts);

				// Allows us to look up a Sketch.Stroke from its Microsoft counterpart
				int mId = sketchInk.Ink.Strokes[sketchInk.Ink.Strokes.Count - 1].Id;
				this.mIdToSubstroke.Add(mId, substrokes[i]);
				this.substrokeIdToMStroke.Add((System.Guid)substrokes[i].XmlAttrs.Id, 
					this.sketchInk.Ink.Strokes[sketchInk.Ink.Strokes.Count - 1]);
			}

			// Move center the ink's origin to the top-left corner
			this.inkMovedX = -sketchInk.Ink.GetBoundingBox().X + 150;
			this.inkMovedY = -sketchInk.Ink.GetBoundingBox().Y + 150;
			this.sketchInk.Renderer.Move(this.inkMovedX, this.inkMovedY);

			this.sketchInk.Enabled = true;

			// Give the panel the mInk component
			this.Controls.Add(sketchInk);

			// Another layer of ink overlaying the current
			// Used when drawing the fragment points
			this.overlayInk = new InkOverlay(this.sketchInk);
			this.overlayInk.Renderer.Move(this.inkMovedX, this.inkMovedY);

			// Adds event handlers to the InkPicture
			this.sketchInk.EditingMode = InkOverlayEditingMode.Select;
			
			this.selectionMoving = false;
			this.sketchInk.SelectionMoving += new InkOverlaySelectionMovingEventHandler(sketchInk_SelectionMoving);
			this.sketchInk.SelectionMoved += new InkOverlaySelectionMovedEventHandler(sketchInk_SelectionMoved);
			
			this.selectionResizing = false;
			this.sketchInk.SelectionResizing += new InkOverlaySelectionResizingEventHandler(sketchInk_SelectionResizing);
			this.sketchInk.SelectionResized += new InkOverlaySelectionResizedEventHandler(sketchInk_SelectionResized);
			
			this.mouseDown = false;
			this.lassoPoints = new ArrayList();
			this.clicked = false;
			this.strokesClicked = null;
			this.sketchInk.MouseDown += new System.Windows.Forms.MouseEventHandler(sketchInk_MouseDown);
			this.sketchInk.MouseMove += new System.Windows.Forms.MouseEventHandler(sketchInk_MouseMove);
			this.sketchInk.MouseUp += new System.Windows.Forms.MouseEventHandler(sketchInk_MouseUp);
			
			this.sketchInk.SelectionChanging += new InkOverlaySelectionChangingEventHandler(sketchInk_SelectionChanging);
			this.sketchInk.SelectionChanged += new InkOverlaySelectionChangedEventHandler(sketchInk_SelectionChanged);
		
			// Handle the ToolTip
			//this.sketchInk.MouseHover += new EventHandler(sketchInk_MouseHover);
			
			// Initialize the label menu
			InitializeLabelMenu();
			
			// Update the fragment points
			UpdateFragmentCorners();

			// Update the stroke colors
			UpdateColors();
			
			// Create the ToolTip to be used in displaying Substroke information
			this.toolTip = new System.Windows.Forms.ToolTip();
			this.toolTip.InitialDelay = 100;
			this.toolTip.ShowAlways = true;
			
			// Resize the InkPicture
			InkResize();
		}


		/// <summary>
		/// Initialize the LabelMenu we are using to label strokes.
		/// </summary>
		private void InitializeLabelMenu()
		{
			this.labelButton = new System.Windows.Forms.Button();
			this.labelButton.BackColor = Color.Coral;
			this.labelButton.Size = new Size(50, 20);
			this.labelButton.Text = "LABEL";

			this.labelButton.MouseDown += new System.Windows.Forms.MouseEventHandler(labelButton_MouseDown);
			
			this.labelMenu = new LabelMenu(this, this.CM);

			this.Controls.Add(this.labelButton);
			this.Controls.Add(this.labelMenu);

			this.labelMenu.InitLabels(this.domainInfo);

			this.labelButton.Hide();
			this.labelMenu.Hide();

			this.Enabled = true;
		}


		/// <summary>
		/// Initialize the labels we are using for this domain
		/// </summary>
		/// <param name="domainInfo">Domain to retrieve labels from</param>
		public void InitLabels(DomainInfo domainInfo)
		{
			this.domainInfo = domainInfo;
			
			if (this.labelMenu != null)
				this.labelMenu.InitLabels(domainInfo);

			UpdateColors();
		}


		public void UpdateColors()
		{
			if (this.sketch != null)
				UpdateColors(new ArrayList(this.sketch.Substrokes));
		}

		public void UpdateColors(ArrayList substrokes)
		{
			if (this.domainInfo != null && this.sketch != null)
			{
				foreach (Sketch.Substroke substroke in substrokes)
				{
					string[] labels = substroke.GetLabels();

					int bestColorRank = Int32.MinValue;
					Color bestColor = Color.Black;
					foreach (string label in labels)
					{
						int currColorRank = Int32.MinValue; 
						Color currColor = this.domainInfo.GetColor(label);

						if (this.domainInfo.ColorHierarchy.Contains(currColor))
							currColorRank = (int)this.domainInfo.ColorHierarchy[currColor];
							
						if (currColorRank > bestColorRank)
						{
							bestColor = currColor;
							bestColorRank = currColorRank;
						}
					}

                    if(this.substrokeIdToMStroke.ContainsKey(substroke.XmlAttrs.Id))
					    (this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Color = bestColor;
				}
			}
		}


		private void InitThickenedLabelAttributes(int thickness)
		{
			this.thickenedLabelAttributes = new Microsoft.Ink.DrawingAttributes();
			this.thickenedLabelAttributes.Width = thickness;
			this.thickenedLabelAttributes.Height = thickness;
		}
		

		public void ThickenLabel(Microsoft.Ink.Strokes newSelection)
		{
			foreach (Microsoft.Ink.Stroke mStroke in newSelection)
			{
				if(!this.mIdToSubstroke.ContainsKey(mStroke.Id))
					continue;

				ArrayList labels = (this.mIdToSubstroke[mStroke.Id] as Sketch.Substroke).ParentShapes;

				foreach (Sketch.Shape labelShape in labels)
				{
					Sketch.Substroke[] labelSubstrokes = labelShape.Substrokes;
					foreach (Sketch.Substroke substroke in labelSubstrokes)
					{
						if(!this.substrokeIdToMStroke.ContainsKey(substroke.XmlAttrs.Id))
							continue;

						Microsoft.Ink.Stroke toModify = (this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke);
						toModify.DrawingAttributes.Width = this.thickenedLabelAttributes.Width;
						toModify.DrawingAttributes.Height = this.thickenedLabelAttributes.Height;
						/*
						(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Width = 
							this.thickenedLabelAttributes.Width;
						(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Height = 
							this.thickenedLabelAttributes.Height;
						*/
						this.thickenedStrokes.Add(substroke);
					}
				}
			}

			this.sketchInk.Invalidate();
		}


		public void UnThickenLabel(Microsoft.Ink.Strokes previousSelection)
		{
			/*foreach (Microsoft.Ink.Stroke mStroke in previousSelection)
			{
				ArrayList labels = (this.mIdToSubstroke[mStroke.Id] as Sketch.Substroke).ParentShapes;

				if (labels.Count == 0)
				{
					mStroke.DrawingAttributes.Width = this.sketchInk.DefaultDrawingAttributes.Width;
					mStroke.DrawingAttributes.Height = this.sketchInk.DefaultDrawingAttributes.Height;
				}
				
				foreach (Sketch.Shape labelShape in labels)
				{
					Sketch.Substroke[] labelSubstrokes = labelShape.Substrokes;
				
					foreach (Sketch.Substroke substroke in labelSubstrokes)
					{
						// IMPORTANT: For some reason we need the following line or our colors do not update
						// correctly. THIS IS A HACK
						// It's also broken, since we update all the Strokes in all labels with the color
						(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes =
							mStroke.DrawingAttributes;

						(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Width = 
							this.sketchInk.DefaultDrawingAttributes.Width;
						(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Height = 
							this.sketchInk.DefaultDrawingAttributes.Height;
					}
				}
			}*/

			foreach (Substroke substroke in this.thickenedStrokes)
			{
				
				if(!this.substrokeIdToMStroke.ContainsKey(substroke.XmlAttrs.Id))
					continue;

				Microsoft.Ink.Stroke toModify = (this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke);
				toModify.DrawingAttributes.Width = this.sketchInk.DefaultDrawingAttributes.Width;
				toModify.DrawingAttributes.Height = this.sketchInk.DefaultDrawingAttributes.Height;

				/*
				(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Width = 
					this.sketchInk.DefaultDrawingAttributes.Width;
				(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).DrawingAttributes.Height = 
					this.sketchInk.DefaultDrawingAttributes.Height;
				*/
			}

			UpdateColors(thickenedStrokes);
			this.thickenedStrokes.Clear();
			
			this.sketchInk.Invalidate();
		}


		/// <summary>
		/// Fragment point properties
		/// </summary>
		/// <param name="color">Drawing color</param>
		/// <param name="thickness">Point width and height</param>
		private void InitFragPtAttributes(Color color, int thickness)
		{
			this.fragmentPtAttributes = new DrawingAttributes();
			this.fragmentPtAttributes.Color = color;
			this.fragmentPtAttributes.Width = thickness;
			this.fragmentPtAttributes.Height = thickness;
		}
		
		
		public void UpdateFragmentCorners(Hashtable fragStrokeToCorners)
		{
			// Clear the current InkOverlay
			overlayInk.Ink.DeleteStrokes();

			// Split the Stroke at the new fragment points
			foreach (DictionaryEntry entry in fragStrokeToCorners)
			{
				Sketch.Stroke stroke = entry.Key as Sketch.Stroke;
				ArrayList corners = fragStrokeToCorners[stroke] as ArrayList;
				
				bool notEqualOrNull = false;
				if (this.strokeToCorners[stroke] != null && corners == null)
				{
					notEqualOrNull = true;
				}
				else if (this.strokeToCorners[stroke] == null && corners != null && corners.Count > 0)
				{
					notEqualOrNull = true;
				}
				else if (this.strokeToCorners[stroke] != null && corners != null
					&& (int[])corners.ToArray(typeof(int)) != (int[])(this.strokeToCorners[stroke] as ArrayList).ToArray(typeof(int)))
				{
					notEqualOrNull = true;
				}

				if (notEqualOrNull)
				{
					if ((corners == null || corners.Count == 0) 
						&& (this.strokeToCorners[stroke] as ArrayList).Count > 0)
						this.strokeToCorners.Remove(stroke);
					else
						this.strokeToCorners[stroke] = corners;

					// Remove all of the substrokes within our InkPicture and hashtables
					foreach (Sketch.Substroke substroke in stroke.Substrokes)
					{
						this.mIdToSubstroke.Remove((this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke).Id);
						this.sketchInk.Ink.DeleteStroke(this.substrokeIdToMStroke[substroke.XmlAttrs.Id] as Microsoft.Ink.Stroke);
						this.substrokeIdToMStroke.Remove(substroke.XmlAttrs.Id);
					}

					// Merge the substrokes together
					bool labelSame = stroke.MergeSubstrokes();
					if (!labelSame)
					{
						System.Windows.Forms.DialogResult ok = System.Windows.Forms.MessageBox.Show(
							"Labels removed from Stroke " + stroke.XmlAttrs.Id.ToString() + 
							"\ndue to fragmenting a non-uniformly labeled stroke", 
							"Important", 
							System.Windows.Forms.MessageBoxButtons.OK,
							System.Windows.Forms.MessageBoxIcon.Exclamation);
					}

					// Fragment the substroke at the specified indices
					if (corners != null && corners.Count > 0)
						stroke.SplitStrokeAt( (int[])corners.ToArray(typeof(int)) );

					// Draw the substrokes in the InkPicture
					foreach (Sketch.Substroke substroke in stroke.Substrokes)
					{
						Sketch.Point[] substrokePts = substroke.Points;
						System.Drawing.Point[] pts = new System.Drawing.Point[substrokePts.Length];
						
						for (int i = 0; i < pts.Length; i++)
						{
							pts[i] = new System.Drawing.Point( (int)(substrokePts[i].X), 
								(int)(substrokePts[i].Y) );
						}
			
						this.sketchInk.Ink.CreateStroke(pts);

						// Allows us to look up a Sketch.Stroke from its Microsoft counterpart
						int mId = sketchInk.Ink.Strokes[sketchInk.Ink.Strokes.Count - 1].Id;
						this.mIdToSubstroke.Add(mId, substroke);
						this.substrokeIdToMStroke.Add((System.Guid)substroke.XmlAttrs.Id, 
							this.sketchInk.Ink.Strokes[sketchInk.Ink.Strokes.Count - 1]);
					}

					// Update the colors associated with the substrokes
					UpdateColors(new ArrayList(stroke.Substrokes));
				}
			}

			// Add the InkOverlay points we'll be drawing
			ArrayList ptsToDraw = new ArrayList();
			
			Sketch.Stroke[] strokes = this.sketch.Strokes;
			foreach (Sketch.Stroke stroke in strokes)
			{
				ArrayList corners = this.strokeToCorners[stroke] as ArrayList;
				
				if (corners != null)
				{
					Sketch.Point[] points = stroke.Points;
					
					foreach (int index in corners)
					{
						ptsToDraw.Add(new System.Drawing.Point((int)points[index].X, (int)points[index].Y));
					}
				}
			}
			
			System.Drawing.Point[] fragPts = (System.Drawing.Point[])ptsToDraw.ToArray(typeof(System.Drawing.Point));
			
			// Create strokes consisting of one point each.
			// This way we can draw the points in our InkPicture and correctly scale the points
			// accordingly.
			foreach (System.Drawing.Point pt in fragPts)
			{
				System.Drawing.Point[] p = new System.Drawing.Point[1];
				p[0] = pt;

				overlayInk.Ink.CreateStroke(p);
			}

			// Render each point
			foreach (Microsoft.Ink.Stroke s in overlayInk.Ink.Strokes)
			{
				s.DrawingAttributes = this.fragmentPtAttributes;
			}

			// Update the Hashtable
			foreach (DictionaryEntry entry in fragStrokeToCorners)
			{
				Sketch.Stroke key = entry.Key as Sketch.Stroke;
				ArrayList val = entry.Value as ArrayList;
				
				if (val == null || val.Count == 0)
					this.strokeToCorners.Remove(key);
				else 
					this.strokeToCorners[key] = new ArrayList(val);
			}
			
			this.sketchInk.Invalidate();
			this.sketchInk.Refresh();
		}

			
		/// <summary>
		/// OLD AND NOT USED!!!
		/// </summary>
		public void UpdateFragmentCorners()
		{
			// Clear the current InkOverlay
			overlayInk.Ink.DeleteStrokes();

			ArrayList ptsArray = new ArrayList();
			
			Sketch.Stroke[] strokes = this.sketch.Strokes;
			for (int i = 0; i < strokes.Length; i++)
			{
				ArrayList corners = this.strokeToCorners[strokes[i]] as ArrayList;
				
				if (corners != null)
				{
					Sketch.Point[] points = strokes[i].Points;
					
					foreach (int index in corners)
					{
						ptsArray.Add(new System.Drawing.Point((int)points[index].X, (int)points[index].Y));
					}
				}
			}
			
			System.Drawing.Point[] pts = (System.Drawing.Point[])ptsArray.ToArray(typeof(System.Drawing.Point));
			
			// Create strokes consisting of one point each
			// This is done so that we can draw the points in our InkPicture and correctly scale the points
			// accordingly.
			for (int i = 0; i < pts.Length; i++)
			{
				System.Drawing.Point[] p = new System.Drawing.Point[1];
				p[0] = pts[i];

				overlayInk.Ink.CreateStroke(p);
			}

			// Render each point
			foreach (Microsoft.Ink.Stroke s in overlayInk.Ink.Strokes)
			{
				s.DrawingAttributes = this.fragmentPtAttributes;
			}

			this.Refresh();
		}


		private void LabelerPanel_Resize(object sender, System.EventArgs e)
		{
			InkResize();
		}


		private void InkResize()
		{
			//const double MARGIN = 0.03;

			// Actual stroke bounding box (in Ink Space)
			int strokeWidth  = sketchInk.Ink.Strokes.GetBoundingBox().Width + 300;
			int strokeHeight = sketchInk.Ink.Strokes.GetBoundingBox().Height + 300;
			
			int inkWidth  = this.Width;
			int inkHeight = this.Height;

			System.Drawing.Graphics g = this.CreateGraphics();

			if (sketchInk.Ink.Strokes.GetBoundingBox().Width != 0 
				&& sketchInk.Ink.Strokes.GetBoundingBox().Height != 0 
				&& inkWidth != 0 && inkHeight != 0)
			{
				// If we want to scale by the panel's width
				if (/*this.panelWidthBox.Checked ==*/ true)
				{
					// Convert the rendering space from Ink Space to Pixels
					System.Drawing.Point bottomRight = new System.Drawing.Point(strokeWidth, strokeHeight);
					sketchInk.Renderer.InkSpaceToPixel(g, ref bottomRight); 				
				
					System.Drawing.Point topLeft = new System.Drawing.Point(0, 0);
					sketchInk.Renderer.InkSpaceToPixel(g, ref topLeft); 				
				
					System.Drawing.Point scalePt = new System.Drawing.Point(bottomRight.X - topLeft.X, 
						bottomRight.Y - topLeft.Y);
				
					// Scale the rendered strokes by the width scaling factor
					float xScale = (float)inkWidth / (float)scalePt.X;
					float yScale = (float)inkHeight / (float)scalePt.Y;
		
					float scale = xScale;

					// Scale the stroke rendering by the scaling factor
					sketchInk.Renderer.Scale(scale, scale, false);

					// Scale the fragment point rendering by the scaling factor
					overlayInk.Renderer.Scale(scale, scale, true);

					// Resize the mInk component to encompass all of the scaled strokes
					System.Drawing.Rectangle scaledBoundingBox = sketchInk.Ink.GetBoundingBox();
					System.Drawing.Point scaledBottomRight = new System.Drawing.Point(scaledBoundingBox.Width,
						scaledBoundingBox.Height);

					sketchInk.Renderer.InkSpaceToPixel(g, ref scaledBottomRight); 

					sketchInk.Size = new Size(scaledBottomRight.X + 150, scaledBottomRight.Y + 150);
					
					// Expand the InkPicture to encompass the entire Panel
					if (sketchInk.Width < this.Width)
						sketchInk.Width = this.Width;
					if (sketchInk.Height < this.Height)
						sketchInk.Height = this.Height;
					
					// Move the Renderer's (x,y) origin so that we can see the entire Sketch
					float toMoveX = (this.inkMovedX * scale) - this.inkMovedX;
					float toMoveY = (this.inkMovedY * scale) - this.inkMovedY;
					sketchInk.Renderer.Move(toMoveX, toMoveY);

					System.Drawing.Point toMove = new System.Drawing.Point((int)toMoveX + 1, (int)toMoveY + 1);
					sketchInk.Renderer.InkSpaceToPixel(g, ref toMove);
					
					this.inkMovedX = (this.inkMovedX * scale);
					this.inkMovedY = (this.inkMovedY * scale);

					// Update the scaling factors
					totalScale *= scale;
					prevScale = totalScale;

					// Update the user-displayed zoom
					//zoomTextBox.Text = totalScale.ToString();
				}
				else
				{
					if (prevScale != 0.0f)
						sketchInk.Renderer.Scale(newScale / prevScale, newScale / prevScale, false);
					
					this.totalScale = prevScale = newScale;	
				}
			}
		}


		private void sketchInk_SelectionMoving(object sender, InkOverlaySelectionMovingEventArgs e)
		{
			this.selectionMoving = true;
		}


		/// Handles the InkOverlay to ensure that displayed strokes have not been moved.
		/// Code pulled from: http://windowssdk.msdn.microsoft.com/en-us/library/microsoft.ink.inkoverlay.selectionmoved.aspx
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Selection moving event</param>
		private void sketchInk_SelectionMoved(object sender, InkOverlaySelectionMovedEventArgs e)
		{
			// Get the selection's bounding box
			System.Drawing.Rectangle newBounds = this.sketchInk.Selection.GetBoundingBox();

			// Move to back to original spot
			this.sketchInk.Selection.Move(e.OldSelectionBoundingRect.Left - newBounds.Left,
				e.OldSelectionBoundingRect.Top - newBounds.Top);

			// Trick to insure that selection handles are updated
			this.sketchInk.Selection = this.sketchInk.Selection;
			
			// Reset our moving flag
			this.selectionMoving = false;
		}

		
		private void sketchInk_SelectionResizing(object sender, InkOverlaySelectionResizingEventArgs e)
		{
			this.selectionResizing = true;
		}
		
		
		/// <summary>
		/// Handles the InkOverlay to ensure that displayed strokes have not been resized.
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Selection resizing event</param>
		private void sketchInk_SelectionResized(object sender, InkOverlaySelectionResizedEventArgs e)
		{
			// Move to back to original spot
			this.sketchInk.Selection.ScaleToRectangle(e.OldSelectionBoundingRect);

			// Trick to insure that selection handles are updated
			this.sketchInk.Selection = this.sketchInk.Selection;
			
			// Reset our resizing flag
			this.selectionResizing = false;
		}


		private void sketchInk_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				this.mouseDown = true;
		}

		
		private void sketchInk_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point mousePosition = new System.Drawing.Point(e.X, e.Y);

			// Display a ToolTip at the current cursor coordinates
			DisplayToolTip(mousePosition);

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				this.lassoPoints.Add(new System.Drawing.Point(e.X, e.Y));
		}

		
		private void sketchInk_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Handles hiding our label menu if we clicked outside its bounds
			if (this.labelMenu.Visible)
			{
				System.Drawing.Rectangle labelBoundingBox = new Rectangle(this.labelMenu.Location,
					this.labelMenu.Size);
				
				if (labelBoundingBox.Contains(new System.Drawing.Point(e.X, e.Y)))
				{
					this.lassoPoints.Clear();
					this.mouseDown = false;
					return;
				}
				else
				{
					this.labelMenu.Hide();
				}
			}
			
			// Don't do anything if we were moving or resizing the selection
			if (this.selectionMoving || this.selectionResizing)
			{
				this.lassoPoints.Clear();
				this.mouseDown = false;
				return;
			}

			// Clear our current Selection
			// I don't know why this is required, but if we don't our strokesSelected becomes
			// bloated for some odd reason...
			//this.sketchInk.Selection.Clear();
			
			// The maximum number of points allowed in the lasso for the MouseUp
			// to be considered a click
			const int LASSOTHRESHOLD = 15;
			
			if (this.lassoPoints.Count < LASSOTHRESHOLD)
				this.clicked = true;
			else
				this.clicked = false;

			// Clear our lassoPoints holder
			this.lassoPoints.Clear();
			this.mouseDown = false;
			
			// Radius (in ink space coordiantes)
			int closeRadius = 30;
			int farRadius = 60;

			// Cursor point (in ink space coordinates)
			System.Drawing.Point cursorPt = new System.Drawing.Point(e.X, e.Y);
			this.sketchInk.Renderer.PixelToInkSpace(this.sketchInk.CreateGraphics(), ref cursorPt);
			
			// Get the strokes close to the point
			Microsoft.Ink.Strokes justClicked = this.sketchInk.Ink.HitTest(cursorPt, closeRadius);
			
			// Initialize the strokesClicked holder
			if (this.strokesClicked == null)
			{
				this.strokesClicked = justClicked;
				this.sketchInk.Selection = this.strokesClicked;
			}
			
			// Checks to see if we haven't clicked on anything
			else if (justClicked.Count == 0 && this.clicked)
			{
				justClicked = this.sketchInk.Ink.HitTest(cursorPt, farRadius);
			
				// Clear the strokesClicked holder if we clicked far enough away
				// from strokes
				if (justClicked.Count == 0)
					this.strokesClicked.Clear();

				this.sketchInk.Selection = this.strokesClicked;
			}

			// Add or remove what we just clicked on
			else if (this.clicked)
			{
				foreach (Microsoft.Ink.Stroke s in justClicked)
				{
					if (this.strokesClicked.Contains(s))
						this.strokesClicked.Remove(s);
					else
						this.strokesClicked.Add(s);
				}

				this.sketchInk.Selection = this.strokesClicked;
			}

			// Trick to ensure we update the selection anyway
			else
			{
				this.sketchInk.Selection = this.sketchInk.Selection;
			}
		}


		private void sketchInk_SelectionChanging(object sender, InkOverlaySelectionChangingEventArgs e)
		{
			// Don't do anything if we were moving or resizing the selection
			if (this.mouseDown || this.selectionMoving || this.selectionResizing)
			{
				UnThickenLabel(this.sketchInk.Selection);
				return;
			}

			// If we've clicked outside our selection or not close enough to a stroke
			else if (e.NewSelection.Count == 0)
			{
				UnThickenLabel(this.sketchInk.Selection);

				this.strokesClicked.Clear();
			}
			
			// If we've clicked on a stroke or group of closely connected strokes
			else if (this.clicked)
			{
				UnThickenLabel(this.sketchInk.Selection);
			
				foreach (Microsoft.Ink.Stroke s in this.strokesClicked)
				{
					if (!e.NewSelection.Contains(s))
						e.NewSelection.Add(s);
				}

				ThickenLabel(e.NewSelection);
			}

			// If we've performed a lasso selection
			else
			{
				UnThickenLabel(this.sketchInk.Selection);
				
				this.strokesClicked.Clear();
				this.strokesClicked.Add(e.NewSelection);
				
				ThickenLabel(this.strokesClicked);
			}
		}


		private void sketchInk_SelectionChanged(object sender, EventArgs e)
		{
			if (this.sketchInk.Selection.Count > 0)
			{
				int x, y;
				x = this.sketchInk.Selection.GetBoundingBox().X + 
					this.sketchInk.Selection.GetBoundingBox().Width;
				y = this.sketchInk.Selection.GetBoundingBox().Y + 
					this.sketchInk.Selection.GetBoundingBox().Height;

				System.Drawing.Point bottomRight = new System.Drawing.Point(x, y);
				this.sketchInk.Renderer.InkSpaceToPixel(this.sketchInk.CreateGraphics(),
					ref bottomRight);
				
				bottomRight.X -= 15 - this.AutoScrollPosition.X;
				bottomRight.Y -= 2 - this.AutoScrollPosition.Y;
				this.labelButton.Location = bottomRight;

				this.labelButton.Visible = true;
				this.labelButton.BringToFront();
			}
			else
			{
				this.labelButton.Visible = false;
			}
		}
		
		
		private void labelButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point topLeft = this.labelButton.Location;

			if (topLeft.X + this.labelMenu.Width > this.sketchInk.Width)
				topLeft.X -= (this.labelMenu.Width - this.labelButton.Width);
			if (topLeft.Y + this.labelMenu.Height > this.sketchInk.Height)
				topLeft.Y -= (this.labelMenu.Height - this.labelButton.Height);

			this.labelMenu.UpdateLabelMenu(this.sketchInk.Selection);
			
			// Should move the scroll to the top somehow
			
			this.labelMenu.Location = topLeft;
			this.labelMenu.BringToFront();
			this.labelMenu.Show();

			this.labelMenu.Focus();
		}
		
		
		/// <summary>
		/// Displays a ToolTip for a Substroke based on where the mouse is located.
		/// </summary>
		/// <param name="coordinates">Mouse coordinates</param>
		private void DisplayToolTip(System.Drawing.Point coordinates)
		{
			// NOTE: We do this currently since the ToolTip is worthless.
			// In C# 1.1 we can't position it, so it's under the user's hand.
			if (this.sketchInk == null || true) //~Re-enable for demos
				return;

			// Get the current mouse position and convert it into InkSpace
			System.Drawing.Point mousePt = coordinates;
			this.sketchInk.Renderer.PixelToInkSpace(this.CreateGraphics(), ref mousePt);

			// Get the closest Microsoft Stroke (in the InkOverlay)
			float strokePt, distance;
			Microsoft.Ink.Stroke closestMSubstroke = 
				this.sketchInk.Ink.NearestPoint(mousePt, out strokePt, out distance);

			// Get the Sketch's corresponding Substroke
			Substroke closestSubstroke = (Substroke)this.mIdToSubstroke[closestMSubstroke.Id];
			
			// If the distance to a Substroke is less than some threshold...
			if (distance < 30 && closestSubstroke != null)
			{
				// Create the ToolTip string with and Id and Labels
				string toolTipLabel = "Id: " + closestSubstroke.XmlAttrs.Id.ToString() + "\nLabels: ";
				string[] labels = closestSubstroke.GetLabels();
				for (int i = 0; i < labels.Length; i++)
				{
					toolTipLabel += labels[i];
				
					if (i < labels.Length - 1)
						toolTipLabel += ", ";        
				}

				toolTipLabel += "\nProbabilities: " + closestSubstroke.GetFirstBelief();
				
				// Show the ToolTip
				this.toolTip.SetToolTip(this.sketchInk, toolTipLabel);
				this.toolTip.Active = true;
			}
			else
			{
				// Don't show the ToolTip if we aren't close enough to any Substroke
				this.toolTip.Active = false;
			}
		}


		public Microsoft.Ink.Strokes Selection
		{
			get
			{
				return this.sketchInk.Selection;
			}
			set
			{
				this.sketchInk.Selection = value;
			}
		}


		public Sketch.Sketch Sketch
		{
			get
			{
				return this.sketch;
			}
			set
			{
				this.sketch = value;
				InitializePanel(this.sketch);
			}
		}


		public Microsoft.Ink.InkPicture SketchInk
		{
			get
			{
				return this.sketchInk;
			}
		}


		public Hashtable MIdToSubstroke
		{
			get
			{
				return this.mIdToSubstroke;
			}
		}


		public Hashtable StrokeToCorners
		{
			get
			{
				return this.strokeToCorners;
			}
			set
			{
				this.strokeToCorners = value;
			}
		}


		public LabelMenu LabelMenu
		{
			get
			{
				return this.labelMenu;
			}
		}

	}
}
