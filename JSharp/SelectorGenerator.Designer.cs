namespace JSharp
{
    partial class SelectorGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectorGenerator));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Selector = new System.Windows.Forms.ComboBox();
            this.Limit = new System.Windows.Forms.TextBox();
            this.Sort = new System.Windows.Forms.ComboBox();
            this.X = new System.Windows.Forms.TextBox();
            this.Y = new System.Windows.Forms.TextBox();
            this.Z = new System.Windows.Forms.TextBox();
            this.PositionBox = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.DistanceMax = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DistanceMin = new System.Windows.Forms.TextBox();
            this.DX = new System.Windows.Forms.TextBox();
            this.DZ = new System.Windows.Forms.TextBox();
            this.DY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.LevelMin = new System.Windows.Forms.TextBox();
            this.LevelMax = new System.Windows.Forms.TextBox();
            this.PositionBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(12, 342);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(776, 96);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // Selector
            // 
            this.Selector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Selector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Selector.ForeColor = System.Drawing.Color.White;
            this.Selector.FormattingEnabled = true;
            this.Selector.Items.AddRange(new object[] {
            "@a",
            "@p",
            "@r",
            "@e",
            "@s"});
            this.Selector.Location = new System.Drawing.Point(12, 32);
            this.Selector.Name = "Selector";
            this.Selector.Size = new System.Drawing.Size(121, 21);
            this.Selector.TabIndex = 1;
            // 
            // Limit
            // 
            this.Limit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Limit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Limit.ForeColor = System.Drawing.Color.White;
            this.Limit.Location = new System.Drawing.Point(139, 33);
            this.Limit.Name = "Limit";
            this.Limit.Size = new System.Drawing.Size(100, 20);
            this.Limit.TabIndex = 2;
            // 
            // Sort
            // 
            this.Sort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Sort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Sort.ForeColor = System.Drawing.Color.White;
            this.Sort.FormattingEnabled = true;
            this.Sort.Items.AddRange(new object[] {
            "nearest",
            "furthest",
            "random",
            "arbitrary"});
            this.Sort.Location = new System.Drawing.Point(245, 33);
            this.Sort.Name = "Sort";
            this.Sort.Size = new System.Drawing.Size(121, 21);
            this.Sort.TabIndex = 3;
            // 
            // X
            // 
            this.X.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.X.ForeColor = System.Drawing.Color.White;
            this.X.Location = new System.Drawing.Point(6, 29);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(93, 20);
            this.X.TabIndex = 4;
            // 
            // Y
            // 
            this.Y.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Y.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Y.ForeColor = System.Drawing.Color.White;
            this.Y.Location = new System.Drawing.Point(105, 28);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(93, 20);
            this.Y.TabIndex = 5;
            // 
            // Z
            // 
            this.Z.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Z.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Z.ForeColor = System.Drawing.Color.White;
            this.Z.Location = new System.Drawing.Point(204, 28);
            this.Z.Name = "Z";
            this.Z.Size = new System.Drawing.Size(93, 20);
            this.Z.TabIndex = 6;
            // 
            // PositionBox
            // 
            this.PositionBox.Controls.Add(this.label14);
            this.PositionBox.Controls.Add(this.DistanceMax);
            this.PositionBox.Controls.Add(this.label10);
            this.PositionBox.Controls.Add(this.label7);
            this.PositionBox.Controls.Add(this.label8);
            this.PositionBox.Controls.Add(this.label9);
            this.PositionBox.Controls.Add(this.label6);
            this.PositionBox.Controls.Add(this.label5);
            this.PositionBox.Controls.Add(this.label4);
            this.PositionBox.Controls.Add(this.DistanceMin);
            this.PositionBox.Controls.Add(this.DX);
            this.PositionBox.Controls.Add(this.DZ);
            this.PositionBox.Controls.Add(this.DY);
            this.PositionBox.Controls.Add(this.X);
            this.PositionBox.Controls.Add(this.Z);
            this.PositionBox.Controls.Add(this.Y);
            this.PositionBox.ForeColor = System.Drawing.Color.White;
            this.PositionBox.Location = new System.Drawing.Point(12, 59);
            this.PositionBox.Name = "PositionBox";
            this.PositionBox.Size = new System.Drawing.Size(303, 145);
            this.PositionBox.TabIndex = 7;
            this.PositionBox.TabStop = false;
            this.PositionBox.Text = "Position";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(105, 103);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 13);
            this.label14.TabIndex = 19;
            this.label14.Text = "Distance Max:";
            // 
            // DistanceMax
            // 
            this.DistanceMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.DistanceMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DistanceMax.ForeColor = System.Drawing.Color.White;
            this.DistanceMax.Location = new System.Drawing.Point(105, 118);
            this.DistanceMax.Name = "DistanceMax";
            this.DistanceMax.Size = new System.Drawing.Size(93, 20);
            this.DistanceMax.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 102);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Distance Min:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(201, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "dz:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(106, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "dy:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "dx:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "z:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(106, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "y:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "x:";
            // 
            // DistanceMin
            // 
            this.DistanceMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.DistanceMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DistanceMin.ForeColor = System.Drawing.Color.White;
            this.DistanceMin.Location = new System.Drawing.Point(6, 118);
            this.DistanceMin.Name = "DistanceMin";
            this.DistanceMin.Size = new System.Drawing.Size(93, 20);
            this.DistanceMin.TabIndex = 10;
            // 
            // DX
            // 
            this.DX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.DX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DX.ForeColor = System.Drawing.Color.White;
            this.DX.Location = new System.Drawing.Point(6, 73);
            this.DX.Name = "DX";
            this.DX.Size = new System.Drawing.Size(93, 20);
            this.DX.TabIndex = 7;
            // 
            // DZ
            // 
            this.DZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.DZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DZ.ForeColor = System.Drawing.Color.White;
            this.DZ.Location = new System.Drawing.Point(204, 72);
            this.DZ.Name = "DZ";
            this.DZ.Size = new System.Drawing.Size(93, 20);
            this.DZ.TabIndex = 9;
            // 
            // DY
            // 
            this.DY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.DY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DY.ForeColor = System.Drawing.Color.White;
            this.DY.Location = new System.Drawing.Point(105, 72);
            this.DY.Name = "DY";
            this.DY.Size = new System.Drawing.Size(93, 20);
            this.DY.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Selector:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Limit:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(245, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Sort:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(372, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Gamemode:";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.ForeColor = System.Drawing.Color.White;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "survival",
            "creative",
            "adventure",
            "spectator"});
            this.comboBox1.Location = new System.Drawing.Point(372, 32);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.LevelMin);
            this.groupBox1.Controls.Add(this.LevelMax);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(321, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 55);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "XP";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(108, 13);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Max:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 13);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "Min:";
            // 
            // LevelMin
            // 
            this.LevelMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.LevelMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LevelMin.ForeColor = System.Drawing.Color.White;
            this.LevelMin.Location = new System.Drawing.Point(8, 29);
            this.LevelMin.Name = "LevelMin";
            this.LevelMin.Size = new System.Drawing.Size(93, 20);
            this.LevelMin.TabIndex = 13;
            // 
            // LevelMax
            // 
            this.LevelMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.LevelMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LevelMax.ForeColor = System.Drawing.Color.White;
            this.LevelMax.Location = new System.Drawing.Point(107, 28);
            this.LevelMax.Name = "LevelMax";
            this.LevelMax.Size = new System.Drawing.Size(93, 20);
            this.LevelMax.TabIndex = 14;
            // 
            // SelectorGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PositionBox);
            this.Controls.Add(this.Sort);
            this.Controls.Add(this.Limit);
            this.Controls.Add(this.Selector);
            this.Controls.Add(this.richTextBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectorGenerator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selector Generator";
            this.PositionBox.ResumeLayout(false);
            this.PositionBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox Selector;
        private System.Windows.Forms.TextBox Limit;
        private System.Windows.Forms.ComboBox Sort;
        private System.Windows.Forms.TextBox X;
        private System.Windows.Forms.TextBox Y;
        private System.Windows.Forms.TextBox Z;
        private System.Windows.Forms.GroupBox PositionBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox DistanceMax;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox DistanceMin;
        private System.Windows.Forms.TextBox DX;
        private System.Windows.Forms.TextBox DZ;
        private System.Windows.Forms.TextBox DY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox LevelMin;
        private System.Windows.Forms.TextBox LevelMax;
    }
}