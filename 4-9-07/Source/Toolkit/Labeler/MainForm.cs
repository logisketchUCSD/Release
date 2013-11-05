using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Sketch;
using Converter;
using CommandManagement;

namespace Labeler
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private CommandManager CM;
		
		private Sketch.Sketch sketch;

		private DomainInfo domainInfo;

		private LabelerPanel labelerPanel;

		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem saveSketchMenuItem;
		private System.Windows.Forms.ToolBar mainToolBar;
		private System.Windows.Forms.ToolBarButton fragmentStrokeBtn;
		private System.Windows.Forms.MenuItem loadDomainMenuItem;
		private System.Windows.Forms.ToolBarButton openSketchBtn;
		private System.Windows.Forms.ToolBarButton saveSketchBtn;
		private System.Windows.Forms.ToolBarButton loadDomainBtn;
		private System.Windows.Forms.ToolBarButton separatorBtn1;
		private System.Windows.Forms.ToolBarButton autoFragmentBtn;
		private System.Windows.Forms.MenuItem openSketchMenuItem;
		private System.Windows.Forms.ToolBarButton undoBtn;
		private System.Windows.Forms.ToolBarButton redoBtn;
		private System.Windows.Forms.ToolBarButton separatorBtn2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem undoMenuItem;
		private System.Windows.Forms.MenuItem redoMenuItem;
		private System.Windows.Forms.MenuItem quitMenuItem;
		private System.Windows.Forms.MenuItem aboutMenuItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Double-buffering code
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();

			// Initialize the CommandManager
			CM = new CommandManager();
			
			// Initialize the DomainInfo
			this.domainInfo = null;

			// Initialize the LabelerPanel
			InitLabelerPanel();
			
			// Re-add all of the window's components
			// This is needed for some reason, otherwise if we just add the LabelerPanel
			// it will try to fill the entire window.
			this.Controls.Clear();
			this.Controls.Add(this.labelerPanel);
			this.Controls.Add(this.mainToolBar);
			this.Controls.Add(this.statusBar);

			// Debug stuff
			//LoadSketch(@"C:\Documents and Settings\Da Vinci\My Documents\Visual Studio Projects\E85\0128\0128_Sketches\convertedJnt\0128_1.1.1.labeled.xml");
			//LoadDomain(@"C:\Documents and Settings\Da Vinci\My Documents\Visual Studio Projects\E85\Domain3.txt");
			LoadDomain(@"DefaultDomain.txt");
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.openSketchMenuItem = new System.Windows.Forms.MenuItem();
			this.saveSketchMenuItem = new System.Windows.Forms.MenuItem();
			this.loadDomainMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.quitMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.undoMenuItem = new System.Windows.Forms.MenuItem();
			this.redoMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.aboutMenuItem = new System.Windows.Forms.MenuItem();
			this.mainToolBar = new System.Windows.Forms.ToolBar();
			this.openSketchBtn = new System.Windows.Forms.ToolBarButton();
			this.saveSketchBtn = new System.Windows.Forms.ToolBarButton();
			this.loadDomainBtn = new System.Windows.Forms.ToolBarButton();
			this.separatorBtn1 = new System.Windows.Forms.ToolBarButton();
			this.undoBtn = new System.Windows.Forms.ToolBarButton();
			this.redoBtn = new System.Windows.Forms.ToolBarButton();
			this.separatorBtn2 = new System.Windows.Forms.ToolBarButton();
			this.autoFragmentBtn = new System.Windows.Forms.ToolBarButton();
			this.fragmentStrokeBtn = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.statusBar.Location = new System.Drawing.Point(0, 546);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(792, 20);
			this.statusBar.TabIndex = 0;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.fileMenuItem,
																					 this.menuItem1,
																					 this.menuItem2});
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.Index = 0;
			this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.openSketchMenuItem,
																						 this.saveSketchMenuItem,
																						 this.loadDomainMenuItem,
																						 this.menuItem5,
																						 this.quitMenuItem});
			this.fileMenuItem.Text = "File";
			// 
			// openSketchMenuItem
			// 
			this.openSketchMenuItem.Index = 0;
			this.openSketchMenuItem.RadioCheck = true;
			this.openSketchMenuItem.Text = "Open Sketch";
			this.openSketchMenuItem.Click += new System.EventHandler(this.openSketchMenuItem_Click);
			// 
			// saveSketchMenuItem
			// 
			this.saveSketchMenuItem.Index = 1;
			this.saveSketchMenuItem.Text = "Save Sketch";
			this.saveSketchMenuItem.Click += new System.EventHandler(this.saveSketchMenuItem_Click);
			// 
			// loadDomainMenuItem
			// 
			this.loadDomainMenuItem.Index = 2;
			this.loadDomainMenuItem.Text = "Load Domain";
			this.loadDomainMenuItem.Click += new System.EventHandler(this.loadDomainMenuItem_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.Text = "-";
			// 
			// quitMenuItem
			// 
			this.quitMenuItem.Index = 4;
			this.quitMenuItem.Text = "Quit";
			this.quitMenuItem.Click += new System.EventHandler(this.quitMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.undoMenuItem,
																					  this.redoMenuItem});
			this.menuItem1.Text = "Edit";
			// 
			// undoMenuItem
			// 
			this.undoMenuItem.Index = 0;
			this.undoMenuItem.Text = "Undo";
			this.undoMenuItem.Click += new System.EventHandler(this.undoMenuItem_Click);
			// 
			// redoMenuItem
			// 
			this.redoMenuItem.Index = 1;
			this.redoMenuItem.Text = "Redo";
			this.redoMenuItem.Click += new System.EventHandler(this.redoMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 2;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.aboutMenuItem});
			this.menuItem2.Text = "Help";
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Index = 0;
			this.aboutMenuItem.Text = "About";
			this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
			// 
			// mainToolBar
			// 
			this.mainToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						   this.openSketchBtn,
																						   this.saveSketchBtn,
																						   this.loadDomainBtn,
																						   this.separatorBtn1,
																						   this.undoBtn,
																						   this.redoBtn,
																						   this.separatorBtn2,
																						   this.autoFragmentBtn,
																						   this.fragmentStrokeBtn});
			this.mainToolBar.ButtonSize = new System.Drawing.Size(80, 43);
			this.mainToolBar.Divider = false;
			this.mainToolBar.DropDownArrows = true;
			this.mainToolBar.Location = new System.Drawing.Point(0, 0);
			this.mainToolBar.Name = "mainToolBar";
			this.mainToolBar.ShowToolTips = true;
			this.mainToolBar.Size = new System.Drawing.Size(792, 47);
			this.mainToolBar.TabIndex = 1;
			this.mainToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.mainToolBar_ButtonClick);
			// 
			// openSketchBtn
			// 
			this.openSketchBtn.Text = "Open Sketch";
			this.openSketchBtn.ToolTipText = "Open a sketch from a file";
			// 
			// saveSketchBtn
			// 
			this.saveSketchBtn.Text = "Save Sketch";
			this.saveSketchBtn.ToolTipText = "Save the current sketch to a file";
			// 
			// loadDomainBtn
			// 
			this.loadDomainBtn.Text = "Load Domain";
			this.loadDomainBtn.ToolTipText = "Load a valid domain file";
			// 
			// separatorBtn1
			// 
			this.separatorBtn1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// undoBtn
			// 
			this.undoBtn.Text = "Undo";
			this.undoBtn.ToolTipText = "Undo previous action";
			// 
			// redoBtn
			// 
			this.redoBtn.Text = "Redo";
			this.redoBtn.ToolTipText = "Redo previous undo";
			// 
			// separatorBtn2
			// 
			this.separatorBtn2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// autoFragmentBtn
			// 
			this.autoFragmentBtn.Text = "Auto Fragment";
			this.autoFragmentBtn.ToolTipText = "Automatically fragment the current sketch";
			// 
			// fragmentStrokeBtn
			// 
			this.fragmentStrokeBtn.Text = "Frag. Stroke";
			this.fragmentStrokeBtn.ToolTipText = "Hand fragment a selected stroke";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.mainToolBar);
			this.Controls.Add(this.statusBar);
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(800, 600);
			this.Name = "MainForm";
			this.Text = "Labeler";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}


		/// <summary>
		/// Initialize a new Panel with certain, default attributes
		/// </summary>
		/// <param name="panel">Panel to set attributes for</param>
		private void SetPanelAttributes(Panel panel)
		{
			panel.AutoScroll = true;
			panel.BackColor = System.Drawing.Color.White;
			panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			panel.Dock = System.Windows.Forms.DockStyle.Fill;
		}


		/// <summary>
		/// Set the ToolBar attributes.
		/// This really isn't used, but is here incase we need multiple toolbars
		/// </summary>
		/// <param name="toolBar">ToolBar to set attributes for</param>
		private void SetToolBarAttributes(ToolBar toolBar)
		{
			toolBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			toolBar.ButtonSize = new System.Drawing.Size(80, 44);
			toolBar.DropDownArrows = true;
			toolBar.ShowToolTips = true;
		}
		
		
		/// <summary>
		/// Initialize the LabelerPanel
		/// </summary>
		private void InitLabelerPanel()
		{
			this.labelerPanel = new LabelerPanel(this.CM, this.domainInfo);
			SetPanelAttributes(this.labelerPanel);
		}
		
		#region MENU ITEMS

		/// <summary>
		/// Starts the "Open" dialog allowing the user to open an MIT XML file or a JNT file.
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void openSketchMenuItem_Click(object sender, System.EventArgs e)
		{
			OpenSketch();
		}


		/// <summary>
		/// Save a Sketch as a file.
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void saveSketchMenuItem_Click(object sender, System.EventArgs e)
		{
			SaveSketch();
		}
		
		
		/// <summary>
		/// Load a valid Domain file into the application.
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void loadDomainMenuItem_Click(object sender, System.EventArgs e)
		{
			LoadDomain();	
		}
		

		/// <summary>
		/// Quit the application
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void quitMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


		/// <summary>
		/// Undo the previous action
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void undoMenuItem_Click(object sender, System.EventArgs e)
		{
			CM.Undo();
		}


		/// <summary>
		/// Redo the previous undone action
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void redoMenuItem_Click(object sender, System.EventArgs e)
		{
			CM.Redo();
		}
		

		/// <summary>
		/// Open an About menu
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Passes an object specific to the event that is being handled</param>
		private void aboutMenuItem_Click(object sender, System.EventArgs e)
		{
			OpenAboutMenu();
		}

		#endregion

		#region ACTIONS
		
		/// <summary>
		/// Starts the "Open" dialog allowing the user to open an MIT XML file or a JNT file.
		/// </summary>
		private void OpenSketch()
		{
			System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
			
			openFileDialog.Title  = "Load a Sketch";
			openFileDialog.Filter = "MIT XML sketches (*.xml)|*.xml|" +
				"Microsoft Windows Journal Files (*.jnt)|*.jnt";
			
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				LoadSketch(openFileDialog.FileName);
			}
		}

		
		/// <summary>
		/// Loads a valid MIT XML file or a JNT file into the application as a Sketch.
		/// </summary>
		/// <param name="filename"></param>
		private void LoadSketch(string filename)
		{
			this.statusBar.Text = "Loading Sketch...";
				
			string extension = System.IO.Path.GetExtension(filename.ToLower());
				
			// Load the Sketch
			if (extension.Equals(".jnt"))
			{
				this.sketch = (new ReadJnt(filename)).Sketch;
			}
			else if (extension.Equals(".xml"))
			{
				this.sketch = (new ReadXML(filename)).Sketch;
			}
				
			this.Text = System.IO.Path.GetFileNameWithoutExtension(filename);

			// Initialize the new panels
			this.labelerPanel.Enabled = false;
			this.labelerPanel.Sketch = this.sketch;
			this.labelerPanel.Enabled = true;
		
			// Clear the CommandManager
			CM.ClearStacks();

			this.statusBar.Text = "";
		}


		/// <summary>
		/// NOTE: Want to later add flags for saving seperately:
		/// - original
		/// - labeled
		/// - fragged
		/// - combination of the above
		/// </summary>
		private void SaveSketch()
		{
			System.Windows.Forms.SaveFileDialog saveFileDialog = new SaveFileDialog();
			
			saveFileDialog.Filter = "MIT XML Files (*.xml)|*.xml";
			saveFileDialog.AddExtension = true;

			// Write the XML to a file
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (this.sketch != null)
				{
					Converter.MakeXML xmlHolder = new MakeXML(this.sketch);
					xmlHolder.WriteXML(saveFileDialog.FileName);
				}
				else 
					MessageBox.Show("No data to save", "Error");
			}
		}


		/// <summary>
		/// Load a valid Domain file into the application.
		/// </summary>
		private void LoadDomain()
		{
			System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
			
			openFileDialog.Title = "Load Domain File";
			openFileDialog.Filter = "Domain Files (*.txt)|*.txt";
			
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				LoadDomain(openFileDialog.FileName);
			}
		}

		
		/// <summary>
		/// Load a valid Domain file into the application.
		/// </summary>
		/// <param name="filepath">Filepath of the domain file</param>
		private void LoadDomain(string filepath)
		{
			this.statusBar.Text = "Loading domain file...";
			
			System.IO.StreamReader sr = new System.IO.StreamReader(filepath);

			this.domainInfo = new DomainInfo();
			string line = sr.ReadLine();
			string[] words = line.Split(null);
			
			// The first line is the study info
			domainInfo.AddInfo(words[0], words[1]);
			line = sr.ReadLine();
			
			// The next line is the domain
			words = line.Split(null);
			domainInfo.AddInfo(words[0], words[1]);
			line = sr.ReadLine();
			
			// Then the rest are labels
			while (line != null && line != "") 
			{
				words = line.Split(null);
				
				string label = words[0];
				int num = int.Parse(words[1]);
				string color = words[2];

				this.domainInfo.AddLabel(num, label, Color.FromName(color));
				line = sr.ReadLine();
			}

			ArrayList labels = this.domainInfo.GetLabels();
			string[] labelsWithColors = new string[labels.Count];

			for (int i = 0; i < labelsWithColors.Length; i++)
			{
				labelsWithColors[i] = (string)labels[i] + "   (" + 
					this.domainInfo.GetColor((string)labels[i]).Name + ")";
			}

			sr.Close();

			this.labelerPanel.InitLabels(this.domainInfo);
			
			this.statusBar.Text = "";
		}


		/// <summary>
		/// Opens an About menu
		/// </summary>
		private void OpenAboutMenu()
		{
			Labeler.About aboutDialog = new About();
			aboutDialog.ShowDialog();
		}

		#endregion

		#region TOOLBAR

		private void mainToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// Load the Sketch file
			if (e.Button == this.openSketchBtn)
			{
				OpenSketch();
			}

			// Save the Sketch
			if (e.Button == this.saveSketchBtn)
			{
				SaveSketch();
			}

			// Load the Domain file
			if (e.Button == this.loadDomainBtn)
			{
				LoadDomain();
			}

			// Undo the previous Command
			if (e.Button == this.undoBtn)
			{
				CM.Undo();
			}

			// Redo the previously undone Command
			if (e.Button == this.redoBtn)
			{
				CM.Redo();
			}

			// Autofragment the Sketch
			if (e.Button == this.autoFragmentBtn)
			{
				CM.ExecuteCommand( new CommandList.AutoFragmentCmd(this.labelerPanel) );
			}
			
			// Fragment a stroke by hand
			if (e.Button == this.fragmentStrokeBtn)
			{
				if (this.labelerPanel.Selection.Count > 0)
				{
					Sketch.Substroke selected = 
						this.labelerPanel.MIdToSubstroke[ this.labelerPanel.Selection[0].Id ] as Sketch.Substroke;

					if (selected != null)
					{
						Labeler.FragmentDialogBox fdb = new Labeler.FragmentDialogBox(
							new Sketch.Stroke[1] {selected.ParentStroke}, this.labelerPanel, this.CM);
						fdb.ShowDialog();
						fdb.Dispose();
					}
				}
			}

			this.labelerPanel.Refresh();
		}

		#endregion
	}
}
