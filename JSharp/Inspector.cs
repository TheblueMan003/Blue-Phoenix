using Cyotek.Data.Nbt;
using Cyotek.Data.Nbt.Serialization;
using JSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BluePhoenix
{
    public partial class InspectorForm : Form
    {
        List<Scoreboard> lst = new List<Scoreboard>();
        List<Scoreboard> lst2 = new List<Scoreboard>();
        string path;
        public InspectorForm(string path)
        {
            InitializeComponent();
            this.path = Directory.GetParent(path).Parent.FullName + "/data/scoreboard.dat";
        }

        private void Inspector_Load(object sender, EventArgs e)
        {
            bool good = false;
            if (File.Exists(path))
            {
                good = true;
            }
            if (!good && openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                good = true;
                path = openFileDialog1.FileName;
            }
            if (good)
            {
                Dictionary<string, string> mapRevert = new Dictionary<string, string>();
                foreach (var key in Compiler.offuscationMap.Keys)
                {
                    mapRevert[Compiler.offuscationMap[key]] = key;
                }
                BinaryTagReader file = new BinaryTagReader(new FileStream(path, FileMode.Open));
                foreach (TagCompound i in file.ReadDocument().GetCompound("data").GetList("PlayerScores").Value)
                {
                    var name = i.GetString("Name").Value;
                    var objective = i.GetString("Objective").Value;
                    var value = i.GetInt("Score").Value;
                    if (mapRevert.ContainsKey(name))
                    {
                        lst.Add(new Scoreboard(mapRevert[name], objective, GetValue(mapRevert[name], value)));
                    }
                    else if (mapRevert.ContainsKey(objective))
                    {
                        lst.Add(new Scoreboard(name, mapRevert[objective], GetValue(mapRevert[objective], value)));
                    }
                    else
                    {
                        lst2.Add(new Scoreboard(name, objective, value.ToString()));
                    }
                }
                file.Close();
            }
            lst.Sort((x, y) => x.name.CompareTo(y.name));
            lst2.Sort((x, y) => x.name.CompareTo(y.name));
            Reload();
        }
        public string GetValue(string name, int value)
        {
            var v = Compiler.variables[name];
            if (v.type == Compiler.Type.FLOAT)
            {
                return ((value * 1f) / Compiler.compilerSetting.FloatPrecision).ToString();
            }
            else if (v.type == Compiler.Type.ENUM)
            {
                try
                {
                    return Compiler.enums[v.enums].valuesName[value];
                }
                catch
                {
                    return $"Enum out of bound: {value}";
                }
            }
            else if (v.type == Compiler.Type.BOOL)
            {
                return value > 0 ? "true" : "false";
            }
            else
            {
                return value.ToString();
            }
        }
        public void Reload()
        {
            dataGridView1.Rows.Clear();

            lst.Where(x => x.name.Contains(textBox1.Text) && x.objective.Contains(textBox2.Text))
               .ToList()
               .ForEach(x => dataGridView1.Rows.Add(x.name, x.objective, x.value));

            lst2.Where(x => x.name.Contains(textBox1.Text) && x.objective.Contains(textBox2.Text))
               .ToList()
               .ForEach(x => dataGridView1.Rows.Add(x.name, x.objective, x.value));
        }
        private class Scoreboard
        {
            public string name;
            public string objective;
            public string value;
            public Scoreboard(string name, string objective, string value)
            {
                this.name = name;
                this.objective = objective;
                this.value = value;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Reload();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Reload();
        }
    }
}
