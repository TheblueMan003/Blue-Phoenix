using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace JSharp
{
    public partial class ProjectSetting : Form
    {
        public ProjectVersion version;
        public Compiler.CompilerSetting compilerSetting;
        public Dictionary<string, Compiler.Variable> variables;
        public object Keyboard { get; private set; }
        public string ProjectName;
        public string description;

        public ProjectSetting(string ProjectName, ProjectVersion version, string description, Compiler.CompilerSetting compilerSetting, Dictionary<string, Compiler.Variable> variables)
        {
            InitializeComponent();
            this.ProjectName = ProjectName;
            this.description = description;
            this.version = version;
            this.compilerSetting = compilerSetting;
            this.variables = variables;

            textBox1.Text = ProjectName;
            label3.Text = version.ToString();
            textBox2.Text = description;
            PackFormat_Box.Text = compilerSetting.packformat.ToString();
            RPPackFormat_Box.Text = compilerSetting.rppackformat.ToString();

            TreeSizeBox.Text = compilerSetting.TreeMaxSize.ToString();
            FloatPrecBox.Text = compilerSetting.FloatPrecision.ToString();
            RMFileBox.Checked = compilerSetting.removeUselessFile;
            PathTags.Checked = compilerSetting.tagsFolder;

            ValueScoreboardBox.Text = compilerSetting.scoreboardValue;
            ConstScoreboardBox.Text = compilerSetting.scoreboardConst;
            TempScoreboardBox.Text = compilerSetting.scoreboardTmp;
            IsLibCheckbox.Checked = !compilerSetting.isLibrary;
            LibPaths.Text = compilerSetting.libraryFolder.Count > 0 ? compilerSetting.libraryFolder.Aggregate((s1, s2) => (s1 + ";" + s2)) : "";
            CompilerCore_Box.Text = compilerSetting.CompilerCoreName;

            ExportAsZip_Box.Checked = compilerSetting.ExportAsZip;
            AuthorsBox.Text = compilerSetting.Authors;
            FunctionTagOptiBox.Checked = compilerSetting.opti_FunctionTagsReplace;
            ShowExceptionBox.Checked = compilerSetting.opti_ShowException;
            ShowDebugBox.Checked = compilerSetting.opti_ShowDebug;
            FileCleanUp.Checked = compilerSetting.opti_FileCleanUp;

            GenMAPSFile.Checked = compilerSetting.generateMAPSFile;
            GenReadMeFile.Checked = compilerSetting.generateREADMEFile;

            HighlighEnum_Box.Checked = Formatter.showEnumValue;
            HighlighFunction_Box.Checked = Formatter.showFunc;
            HighlightName_Box.Checked = Formatter.showName;
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
                compilerSetting.packformat = int.Parse(PackFormat_Box.Text);
                compilerSetting.rppackformat = int.Parse(RPPackFormat_Box.Text);
                compilerSetting.removeUselessFile = RMFileBox.Checked;
                compilerSetting.tagsFolder = PathTags.Checked;

                compilerSetting.scoreboardValue = ValueScoreboardBox.Text.Replace(" ", "").Replace("\t", "");
                compilerSetting.scoreboardConst = ConstScoreboardBox.Text.Replace(" ", "").Replace("\t", "");
                compilerSetting.scoreboardTmp = TempScoreboardBox.Text.Replace(" ", "").Replace("\t", "");

                compilerSetting.libraryFolder = LibPaths.Text.Split(';').ToList();
                compilerSetting.isLibrary = !IsLibCheckbox.Checked;

                compilerSetting.ExportAsZip = ExportAsZip_Box.Checked;

                compilerSetting.CompilerCoreName = CompilerCore_Box.Text;
                compilerSetting.Authors = AuthorsBox.Text;

                compilerSetting.opti_FunctionTagsReplace = FunctionTagOptiBox.Checked;
                compilerSetting.opti_ShowException = ShowExceptionBox.Checked;
                compilerSetting.opti_ShowDebug = ShowDebugBox.Checked;
                compilerSetting.opti_FileCleanUp = FileCleanUp.Checked;

                compilerSetting.generateMAPSFile = GenMAPSFile.Checked;
                compilerSetting.generateREADMEFile = GenReadMeFile.Checked;

                Formatter.showEnumValue = HighlighEnum_Box.Checked;
                Formatter.showFunc = HighlighFunction_Box.Checked;
                Formatter.showName = HighlightName_Box.Checked;
            }
            catch (Exception exc)
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

        private void button5_Click(object sender, EventArgs e)
        {
            ForceOffuscation form = new ForceOffuscation(variables, compilerSetting.forcedOffuscation);
            form.ShowDialog();
        }
    }
}
