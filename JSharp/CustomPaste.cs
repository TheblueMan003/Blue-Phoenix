using JSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluePhoenix
{
    public partial class CustomPaste : Form
    {
        List<CustomPasteReplace> customPasteReplaces;
        public CustomPasteReplace customPasteReplace;

        public CustomPaste()
        {
            InitializeComponent();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            if (File.Exists(path + "/setting/custompaste.json"))
            {
                customPasteReplaces = JsonConvert.DeserializeObject<List<CustomPasteReplace>>(File.ReadAllText(path + "/setting/custompaste.json"));
            }
            else
            {
                customPasteReplaces = new List<CustomPasteReplace>();
            }
            customPasteReplaces.ForEach(x => listBox1.Items.Add(x));
        }

        private void OK_Click(object sender, EventArgs e)
        {
            customPasteReplace = new CustomPasteReplace(NameTB.Text, RegexTB.Text, ReplaceTB.Text);
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            CMD_Compile.SafeWriteFile(path + "/setting/custompaste.json", JsonConvert.SerializeObject(customPasteReplaces));

            DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            customPasteReplaces = customPasteReplaces.Where(x => x.name != NameTB.Text).ToList();
            customPasteReplaces.Add(new CustomPasteReplace(NameTB.Text, RegexTB.Text, ReplaceTB.Text));

            listBox1.Items.Clear();
            customPasteReplaces.ForEach(x => listBox1.Items.Add(x));
            Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                NameTB.Text = ((CustomPasteReplace)listBox1.SelectedItem).name;
                RegexTB.Text = ((CustomPasteReplace)listBox1.SelectedItem).regex;
                ReplaceTB.Text = ((CustomPasteReplace)listBox1.SelectedItem).replace;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            customPasteReplaces = customPasteReplaces.Where(x => x.name != NameTB.Text).ToList();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
    public class CustomPasteReplace
    {
        public string name;
        public string regex;
        public string replace;

        public CustomPasteReplace(string name, string regex, string replace)
        {
            this.name = name;
            this.regex = regex;
            this.replace = replace;
        }

        public string Get(string clipboard)
        {
            return new Regex(regex).Replace(clipboard, replace);
        }
        public override string ToString()
        {
            return name;
        }
    }
}
