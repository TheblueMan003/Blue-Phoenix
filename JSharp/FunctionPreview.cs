using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace JSharp
{
    public partial class FunctionPreview : Form
    {
        private Dictionary<string, List<Compiler.Function>> Functions;
        private Dictionary<string, Compiler.Structure> Structures;
        private Dictionary<string, Compiler.Variable> Variables;
        private Dictionary<string, Compiler.Enum> Enums;
        private Dictionary<string, List<Compiler.Predicate>> Predicates;
        private Dictionary<string, TagsList> Tags;
        private string[] names;
        private bool isClass;

        public FunctionPreview(Dictionary<string, List<Compiler.Function>> dic)
        {
            InitializeComponent();
            this.Functions = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, Compiler.Structure> dic, bool isClass)
        {
            InitializeComponent();
            this.Structures = dic;
            this.isClass = isClass;

            Reload();
        }
        public FunctionPreview(Dictionary<string, Compiler.Variable> dic)
        {
            InitializeComponent();
            this.Variables = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, Compiler.Enum> dic)
        {
            InitializeComponent();
            this.Enums = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, TagsList> dic)
        {
            InitializeComponent();
            this.Tags = dic;

            Reload();
        }
        public FunctionPreview(Dictionary<string, List<Compiler.Predicate>> dic)
        {
            InitializeComponent();
            this.Predicates = dic;

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
            if (Functions != null)
            {
                foreach (string key in Functions.Keys)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (Structures != null)
            {
                foreach (string key in Structures.Values.Distinct().Select(x => x.name))
                {
                    if (key.Contains(Filter.Text) && Structures[key].isClass == isClass)
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (Variables != null)
            {
                foreach (string key in Variables.Keys)
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (Enums != null)
            {
                foreach (string key in Enums.Values.Distinct().Select(x => x.name))
                {
                    if (key.Contains(Filter.Text))
                    {
                        listBox1.Items.Add(key);
                    }
                }
            }
            if (Tags != null)
            {
                foreach (string key in Tags.Keys)
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
            if (Predicates != null)
            {
                foreach (string key in Predicates.Keys)
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
                if (Functions != null)
                {
                    listBox2.Items.Clear();
                    richTextBox1.Text = "";
                    richTextBox1.Text += listBox1.SelectedItem.ToString()+"\n";
                    foreach (Compiler.Function f in Functions[listBox1.SelectedItem.ToString()])
                    {
                        foreach (var arg in f.args)
                        {
                            listBox2.Items.Add(arg.name + " : " + arg.GetTypeString());
                        }
                        if (Functions[listBox1.SelectedItem.ToString()].Count > 1)
                            listBox2.Items.Add("==========================");
                        richTextBox1.Text += f.desc + "\n==========================\n";
                        richTextBox1.Text += $"lazy: {f.lazy.ToString()} adj: {f?.package}\nattributes: {(f?.attributes.Count > 0 ? f?.attributes.Aggregate((x, y) => (x + ", " + y)):"")}\n\n";
                        foreach (string line in f.file.parsed)
                        {
                            richTextBox1.Text += line + "\n";
                        }
                        richTextBox1.Text += "\n==========================\n";
                    }
                }
                if (Structures != null)
                {
                    listBox2.Items.Clear();
                    Compiler.Structure f = Structures[listBox1.SelectedItem.ToString()];
                    foreach (var arg in f.fields)
                    {
                        listBox2.Items.Add(arg.name + " : " + arg.GetTypeString());
                    }
                    richTextBox1.Text = $"attributes: {(f.attributes.Count > 0 ? f.attributes.Aggregate((x, y) => (x + ", " + y)):"")}";
                }
                if (Variables != null)
                {
                    listBox2.Items.Clear();
                    Compiler.Variable f = Variables[listBox1.SelectedItem.ToString()];
                    richTextBox1.Text = f.gameName + ": " + f.GetTypeString() + " entity:" + f.entity.ToString() + $"\nattributes: {(f.attributes.Count>0?f.attributes.Aggregate((x, y) => (x + ", " + y)):"")}";
                }
                if (Enums != null)
                {
                    listBox2.Items.Clear();
                    List<string> f = Enums[listBox1.SelectedItem.ToString()].Values();

                    foreach (var arg in f)
                    {
                        listBox2.Items.Add(arg);
                    }
                }
                if (Tags != null)
                {
                    listBox2.Items.Clear();
                    List<string> f = Tags[listBox1.SelectedItem.ToString()].values;

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
                if (Predicates != null)
                {
                    listBox2.Items.Clear();
                    foreach (var v in Predicates[listBox1.SelectedItem.ToString()])
                    {
                        richTextBox1.Text += v.baseFile.content + "\n===================\n";
                    }
                }
            }
        }

        private void Filter_TextChanged(object sender, EventArgs e)
        {
            Reload();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1)
            {
                if (Enums != null)
                {
                    var en = Enums[listBox1.SelectedItem.ToString()];
                    var f = en.values[listBox2.SelectedIndex];

                    richTextBox1.Text =
                        f.fields.Select((x) =>
                            $"{x.Key}({en.fields.Find(y => y.name == x.Key).type}): {x.Value}\n"
                            ).Aggregate((x, y) => x + y);
                }
            }
        }
    }
}
