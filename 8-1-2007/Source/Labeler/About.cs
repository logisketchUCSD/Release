using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Labeler
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox aboutTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.aboutTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// aboutTextBox
			// 
			this.aboutTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.aboutTextBox.Location = new System.Drawing.Point(8, 8);
			this.aboutTextBox.Multiline = true;
			this.aboutTextBox.Name = "aboutTextBox";
			this.aboutTextBox.ReadOnly = true;
			this.aboutTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.aboutTextBox.Size = new System.Drawing.Size(272, 248);
			this.aboutTextBox.TabIndex = 0;
			this.aboutTextBox.Text = @"
HMC Labeler, beta 0.1

Research Advisor:
Christine Alvarado

Authors:
Aaron Wolin
Devin Smith, Jason Fennell, Max Pflueger

Contact:
sketchers@cs.hmc.edu


Labeler beta 0.1, Copyright (C) 2007  Christine Alvarado
Labeler comes with ABSOLUTELY NO WARRANTY 
This is free software, and you are welcome
to redistribute it under certain conditions;
see GNU GPA distributed with this software 
for more details.";
			this.aboutTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 262);
			this.Controls.Add(this.aboutTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "About";
			this.Text = "About";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
