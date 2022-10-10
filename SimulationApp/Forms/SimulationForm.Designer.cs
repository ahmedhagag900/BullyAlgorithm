namespace SimulationApp.Forms
{
    partial class SimulationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._logBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _logBox
            // 
            this._logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logBox.FormattingEnabled = true;
            this._logBox.ItemHeight = 20;
            this._logBox.Location = new System.Drawing.Point(12, 214);
            this._logBox.Name = "_logBox";
            this._logBox.Size = new System.Drawing.Size(876, 264);
            this._logBox.TabIndex = 0;
            // 
            // SimulationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 506);
            this.Controls.Add(this._logBox);
            this.MaximizeBox = false;
            this.Name = "SimulationForm";
            this.Text = "SimulationForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimulationForm_FormClosing);
            this.Load += new System.EventHandler(this.SimulationForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox _logBox;
    }
}