using System;
using System.Collections;

using Microsoft.Ink;

using Sketch;
using Featurefy;
using Fragmenter;

using CommandManagement;

namespace Labeler.CommandList
{
	/// <summary>
	/// Summary description for AutoFragmentCmd.
	/// </summary>
	public class AutoFragmentCmd : Command
	{
		/// <summary>
		/// Is the Command undoable?
		/// </summary>
		private bool isUndoable = true;

		/// <summary>
		/// New Hashtable mapping FeatureStrokes to auto-fragged points
		/// </summary>
		private Hashtable strokeToCorners;

		/// <summary>
		/// Old Hashtable mapping FeatureStrokes to fragmentation points
		/// </summary>
		private Hashtable oldStrokeToCorners;

		/// <summary>
		/// LabelerPanel
		/// </summary>
		private LabelerPanel labelerPanel;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="labelerPanel">LabelerPanel holding the Strokes to fragment</param>
		public AutoFragmentCmd(LabelerPanel labelerPanel)
		{
			this.labelerPanel = labelerPanel;

			this.oldStrokeToCorners = new Hashtable();
			foreach (Sketch.Stroke stroke in this.labelerPanel.Sketch.Strokes)
			{
				this.oldStrokeToCorners.Add(stroke, this.labelerPanel.StrokeToCorners[stroke]);
			}

			this.strokeToCorners = new Hashtable();
			
			// Initialize the FeatureStrokes
			Sketch.Stroke[] strokes = this.labelerPanel.Sketch.Strokes;
			FeatureStroke[] featureStrokes = new FeatureStroke[strokes.Length];
			for (int i = 0; i < featureStrokes.Length; i++)
			{
				featureStrokes[i] = new FeatureStroke(strokes[i]);
			}

			for (int i = 0; i < featureStrokes.Length; i++)
			{
				ArrayList currCorners = new ArrayList();
			
				int[] corners = new Corners(featureStrokes[i]).FindCorners();
				
				if (corners.Length > 0)
				{
					this.strokeToCorners.Add(strokes[i], new ArrayList(corners));
				}
				else
				{
					this.strokeToCorners.Add(strokes[i], new ArrayList());
				}
			}
		}

		
		/// <summary>
		/// Auto-fragments the strokes in the LabelerPanel
		/// </summary>
		public override void Execute()
		{
			this.labelerPanel.UpdateFragmentCorners(this.strokeToCorners);
		}

		
		/// <summary>
		/// Undoes the Auto-fragmentation
		/// </summary>
		public override void UnExecute()
		{
			this.labelerPanel.UpdateFragmentCorners(this.oldStrokeToCorners);
		}

		
		/// <summary>
		/// Returns true if the Command is undoable
		/// </summary>
		/// <returns>True if the Command is undoable, false otherwise</returns>
		public override bool IsUndoable()
		{
			return isUndoable;
		}
	}
}
