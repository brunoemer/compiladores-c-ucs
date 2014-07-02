namespace InterfaceGrafica
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textTableSymbol = new System.Windows.Forms.TextBox();
            this.textBoxErrors = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxC3E = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fctb);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(931, 662);
            this.panel1.TabIndex = 0;
            // 
            // fctb
            // 
            this.fctb.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.fctb.BackBrush = null;
            this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctb.Location = new System.Drawing.Point(0, 0);
            this.fctb.Name = "fctb";
            this.fctb.Paddings = new System.Windows.Forms.Padding(0);
            this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctb.Size = new System.Drawing.Size(567, 419);
            this.fctb.TabIndex = 4;
            this.fctb.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctb_TextChanged);
            this.fctb.SelectionChanged += new System.EventHandler(this.fctb_SelectionChanged);
            this.fctb.SelectionChangedDelayed += new System.EventHandler(this.fctb_SelectionChangedDelayed);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Blue;
            this.panel4.Controls.Add(this.textTableSymbol);
            this.panel4.Controls.Add(this.textBoxErrors);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 419);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(567, 243);
            this.panel4.TabIndex = 3;
            // 
            // textTableSymbol
            // 
            this.textTableSymbol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textTableSymbol.Location = new System.Drawing.Point(223, 0);
            this.textTableSymbol.Multiline = true;
            this.textTableSymbol.Name = "textTableSymbol";
            this.textTableSymbol.ReadOnly = true;
            this.textTableSymbol.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textTableSymbol.Size = new System.Drawing.Size(344, 243);
            this.textTableSymbol.TabIndex = 2;
            // 
            // textBoxErrors
            // 
            this.textBoxErrors.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBoxErrors.Location = new System.Drawing.Point(0, 0);
            this.textBoxErrors.Multiline = true;
            this.textBoxErrors.Name = "textBoxErrors";
            this.textBoxErrors.ReadOnly = true;
            this.textBoxErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxErrors.Size = new System.Drawing.Size(223, 243);
            this.textBoxErrors.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.panel2.Controls.Add(this.textBoxC3E);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(567, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(364, 662);
            this.panel2.TabIndex = 1;
            // 
            // textBoxC3E
            // 
            this.textBoxC3E.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxC3E.Location = new System.Drawing.Point(0, 0);
            this.textBoxC3E.Multiline = true;
            this.textBoxC3E.Name = "textBoxC3E";
            this.textBoxC3E.ReadOnly = true;
            this.textBoxC3E.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxC3E.Size = new System.Drawing.Size(364, 662);
            this.textBoxC3E.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 662);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Compilador C para C3E";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private FastColoredTextBoxNS.FastColoredTextBox fctb;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxC3E;
        private System.Windows.Forms.TextBox textBoxErrors;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textTableSymbol;
    }
}

