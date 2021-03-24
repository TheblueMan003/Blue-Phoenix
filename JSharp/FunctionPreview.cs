﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public partial class FunctionPreview : Form
    {
        private Dictionary<string, List<Compiler.Function>> dic;
        private Dictionary<string, Compiler.Structure> structs;
        private Dictionary<string, Compiler.Variable> vars;
        private Dictionary<string, Compiler.Enum> enums;
        private string[] names;

        public FunctionPreview(Dictionary<string, List<Compiler.Function>> dic)
        {
            InitializeComponent();
            this.dic = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, Compiler.Structure> dic)
        {
            InitializeComponent();
            this.structs = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, Compiler.Variable> dic)
        {
            InitializeComponent();
            this.vars = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, Compiler.Enum> dic)
        {
            InitializeComponent();
            this.enums = dic;

            Reload();
        }
        public FunctionPreview(string[] dic)
        {
            InitializeComponent();
            this.names = dic;

            Reload();
        }

        public void Reload()
        {
            listBox1.Items.Clear();
            if (dic != null)
            {
                foreach (string key in dic.Keys)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (structs != null)
            {
                foreach (string key in structs.Keys)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (vars != null)
            {
                foreach (string key in vars.Keys)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (enums != null)
            {
                foreach (string key in enums.Keys)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (names != null)
            {
                foreach (string key in names)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
        }
        private void FunctionPreview_Load(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                if (dic != null)
                {
                    listBox2.Items.Clear();
                    richTextBox1.Text = "";
                    foreach (Compiler.Function f in dic[listBox1.SelectedItem.ToString()])
                    {
                        foreach (var arg in f.args)
                        {
                            listBox2.Items.Add(arg.name + " : " + arg.GetTypeString());
                        }
                        if (dic[listBox1.SelectedItem.ToString()].Count > 1)
                            listBox2.Items.Add("==========================");
                        richTextBox1.Text += f.desc+"\n==========================\n";
                        foreach (string line in f.file.parsed) {
                            richTextBox1.Text += line+"\n";
                        }
                        richTextBox1.Text += "\n==========================\n";
                    }
                }
                if (structs != null)
                {
                    listBox2.Items.Clear();
                    Compiler.Structure f = structs[listBox1.SelectedItem.ToString()];
                    foreach (var arg in f.fields)
                    {
                        listBox2.Items.Add(arg.name + " : " + arg.GetTypeString());
                    }
                }
                if (vars != null)
                {
                    listBox2.Items.Clear();
                    Compiler.Variable f = vars[listBox1.SelectedItem.ToString()];
                    richTextBox1.Text = f.gameName + ": "+f.GetTypeString();
                    /*
                    foreach (var arg in f.enums)
                    {
                        listBox2.Items.Add(arg.name + " : " + arg.type.ToString());
                    }*/
                }
                if (enums != null)
                {
                    listBox2.Items.Clear();
                    List<string> f = enums[listBox1.SelectedItem.ToString()].Values();
                    
                    foreach (var arg in f)
                    {
                        listBox2.Items.Add(arg);
                    }
                }
                if (names != null)
                {
                    listBox2.Items.Clear();
                    listBox2.Items.Add(listBox1.SelectedItem.ToString());
                    richTextBox1.Text = listBox1.SelectedItem.ToString();
                }
            }
        }

        private void Filter_TextChanged(object sender, EventArgs e)
        {
            Reload();
        }
    }
}
