﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace JSharp
{
    public partial class ProjectSetting : Form
    {
        public ProjectVersion version;
        public Compiler.CompilerSetting compilerSetting;

        public object Keyboard { get; private set; }
        public string ProjectName;
        public string description;

        public ProjectSetting(string ProjectName, ProjectVersion version, string description, Compiler.CompilerSetting compilerSetting)
        {
            InitializeComponent();
            this.ProjectName = ProjectName;
            this.description = description;
            this.version = version;
            this.compilerSetting = compilerSetting;

            textBox1.Text = ProjectName;
            label3.Text = version.ToString();
            textBox2.Text = description;

            TreeSizeBox.Text = compilerSetting.TreeMaxSize.ToString();
            FloatPrecBox.Text = compilerSetting.FloatPrecision.ToString();
            RMFileBox.Checked = compilerSetting.removeUselessFile;

            ValueScoreboardBox.Text = compilerSetting.scoreboardValue;
            ConstScoreboardBox.Text = compilerSetting.scoreboardConst;
            TempScoreboardBox.Text = compilerSetting.scoreboardTmp;
        }

        private void ProjectSetting_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            version.major++;
            label3.Text = version.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            version.minor++;
            label3.Text = version.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            version.patch++;
            label3.Text = version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                compilerSetting.TreeMaxSize = int.Parse(TreeSizeBox.Text);
                compilerSetting.FloatPrecision = int.Parse(FloatPrecBox.Text);
                compilerSetting.removeUselessFile = RMFileBox.Checked;

                compilerSetting.scoreboardValue = ValueScoreboardBox.Text.Replace(" ","").Replace("\t", "");
                compilerSetting.scoreboardConst = ConstScoreboardBox.Text.Replace(" ", "").Replace("\t", "");
                compilerSetting.scoreboardTmp = TempScoreboardBox.Text.Replace(" ", "").Replace("\t", "");
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }

            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.ProjectName = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.description = textBox2.Text;
        }
    }
}
