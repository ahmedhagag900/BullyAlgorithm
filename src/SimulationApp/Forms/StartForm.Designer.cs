namespace SimulationApp.Forms
{
    partial class StartForm
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
            this._strartSimulateBtn = new System.Windows.Forms.Button();
            this._noOfProcess = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _strartSimulateBtn
            // 
            this._strartSimulateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._strartSimulateBtn.Location = new System.Drawing.Point(126, 66);
            this._strartSimulateBtn.Name = "_strartSimulateBtn";
            this._strartSimulateBtn.Size = new System.Drawing.Size(144, 29);
            this._strartSimulateBtn.TabIndex = 0;
            this._strartSimulateBtn.Text = "Start Simulation";
            this._strartSimulateBtn.UseVisualStyleBackColor = true;
            this._strartSimulateBtn.Click += new System.EventHandler(this._strartSimulateBtn_Click);
            // 
            // _noOfProcess
            // 
            this._noOfProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._noOfProcess.Location = new System.Drawing.Point(310, 30);
            this._noOfProcess.Name = "_noOfProcess";
            this._noOfProcess.Size = new System.Drawing.Size(57, 27);
            this._noOfProcess.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Number of Processes to Simulate :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(296, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Maximum numbers 5 due to demo purpose";
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 153);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._noOfProcess);
            this.Controls.Add(this._strartSimulateBtn);
            this.MaximizeBox = false;
            this.Name = "StartForm";
            this.Text = "StartForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button _strartSimulateBtn;
        private TextBox _noOfProcess;
        private Label label1;
        private Label label2;
    }
}