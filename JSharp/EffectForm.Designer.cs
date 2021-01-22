namespace JSharp
{
    partial class EffectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EffectForm));
            this.richTextBox1 = new ColorTextBox();
            this.EffectTextbox = new System.Windows.Forms.ComboBox();
            this.DurationTextbox = new System.Windows.Forms.TextBox();
            this.AmplierTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ParticuleTextbox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.EntityTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(776, 161);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // EffectTextbox
            // 
            this.EffectTextbox.FormattingEnabled = true;
            this.EffectTextbox.Location = new System.Drawing.Point(194, 201);
            this.EffectTextbox.Name = "EffectTextbox";
            this.EffectTextbox.Size = new System.Drawing.Size(161, 21);
            this.EffectTextbox.TabIndex = 1;
            // 
            // DurationTextbox
            // 
            this.DurationTextbox.Location = new System.Drawing.Point(361, 201);
            this.DurationTextbox.Name = "DurationTextbox";
            this.DurationTextbox.Size = new System.Drawing.Size(99, 20);
            this.DurationTextbox.TabIndex = 2;
            this.DurationTextbox.Text = "1";
            this.DurationTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // AmplierTextbox
            // 
            this.AmplierTextbox.Location = new System.Drawing.Point(466, 201);
            this.AmplierTextbox.Name = "AmplierTextbox";
            this.AmplierTextbox.Size = new System.Drawing.Size(90, 20);
            this.AmplierTextbox.TabIndex = 3;
            this.AmplierTextbox.Text = "0";
            this.AmplierTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(194, 185);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Effect:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(358, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Duration:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(463, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Amplier:";
            // 
            // ParticuleTextbox
            // 
            this.ParticuleTextbox.AutoSize = true;
            this.ParticuleTextbox.Location = new System.Drawing.Point(562, 201);
            this.ParticuleTextbox.Name = "ParticuleTextbox";
            this.ParticuleTextbox.Size = new System.Drawing.Size(67, 17);
            this.ParticuleTextbox.TabIndex = 8;
            this.ParticuleTextbox.Text = "Particule";
            this.ParticuleTextbox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(632, 199);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(713, 199);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(12, 239);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(394, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "Ok";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(412, 239);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(376, 23);
            this.button4.TabIndex = 12;
            this.button4.Text = "Cancel";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Entity:";
            // 
            // EntityTextbox
            // 
            this.EntityTextbox.Location = new System.Drawing.Point(12, 202);
            this.EntityTextbox.Name = "EntityTextbox";
            this.EntityTextbox.Size = new System.Drawing.Size(176, 20);
            this.EntityTextbox.TabIndex = 13;
            this.EntityTextbox.Text = "@a";
            this.EntityTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // EffectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 278);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.EntityTextbox);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ParticuleTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AmplierTextbox);
            this.Controls.Add(this.DurationTextbox);
            this.Controls.Add(this.EffectTextbox);
            this.Controls.Add(this.richTextBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EffectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EffectForm";
            this.Load += new System.EventHandler(this.EffectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorTextBox richTextBox1;
        private System.Windows.Forms.ComboBox EffectTextbox;
        private System.Windows.Forms.TextBox DurationTextbox;
        private System.Windows.Forms.TextBox AmplierTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ParticuleTextbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox EntityTextbox;
    }
}