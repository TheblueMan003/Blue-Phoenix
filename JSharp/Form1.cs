using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public partial class Form1 : Form
    {
        public Dictionary<string, string> code = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, TagsList>> TagsList = new Dictionary<string, Dictionary<string, TagsList>>();
        public Dictionary<string, Dictionary<string, TagsList>> MCTagsList = new Dictionary<string, Dictionary<string, TagsList>>();
        public Dictionary<string, string> structures = new Dictionary<string, string>();
        public ProjectVersion projectVersion = new ProjectVersion();

        private string previous = "load";
        private string projectName = "default";
        private string currentDataPack;
        public string projectPath;
        public bool ignorNextListboxUpdate = false;
        public int isCompiling = 0;
        public List<Compiler.File> compileFile;
        public List<Compiler.File> compileFiled;
        private List<DebugMessage> debugMSGs = new List<DebugMessage>();
        private int lastSeen = -1;
        private bool showForm;

        private List<string> PreviousText = new List<string>();
        private bool ignoreMod = false;
        private bool noReformat = false;
        private bool exporting;
        private int index = 0;
        private int changedTime;
        private bool selfShift;


        public Form1(string project = null)
        {
            InitializeComponent();
            CommandParser.loadDict();
            Formatter.loadDict();
            CodeBox.SelectionTabs = new int[] { 50, 100, 150, 200 };
            if (project != null)
            {
                OpenFile(project);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            exporting = false;
            CompileJava(true);
            UpdateCodeBox();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignorNextListboxUpdate)
            {
                ignorNextListboxUpdate = false;
            }
            else if (listBox1.SelectedIndex >= 0)
            {
                noReformat = true;
                code[previous] = CodeBox.Text;

                PreviousText.Clear();
                PreviousText.Add(CodeBox.Text);
                index = 0;
                if (code.ContainsKey(listBox1.SelectedItem.ToString()))
                    CodeBox.Text = code[listBox1.SelectedItem.ToString()];
                else
                {
                    CodeBox.Text = "";
                }
                
                previous = listBox1.SelectedItem.ToString();
                noReformat = false;
                UpdateCodeBox();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            NewFile();
        }
        private void CodeBox_TextChanged(object sender, EventArgs e)
        {
            if (!Formatter.reformating && !noReformat)
            {
                if (!ignoreMod)
                {
                    while (index < PreviousText.Count - 1)
                    {
                        PreviousText.RemoveAt(PreviousText.Count - 1);
                    }
                    PreviousText.Add(CodeBox.Text);
                    index++;
                }
                changedTime = 1;
                AddLineNumbers();
                Formatter.reformat(CodeBox, this, true);
            }
        }
        private void CodeBox_Leave(object sender, EventArgs e)
        {
            code[previous] = CodeBox.Text;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            while(lastSeen < debugMSGs.Count-1)
            {
                lastSeen++;
                Debug(debugMSGs[lastSeen].msg, debugMSGs[lastSeen].color);
            }

            if (isCompiling == 2)
            {
                FilePreview filePreview = new FilePreview(compileFiled);
                filePreview.Show();
                isCompiling = 0;
            }
            if (isCompiling == 1000)
            {
                SystemSounds.Beep.Play();
                isCompiling = 0;
            }
        }
        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            EffectForm form = new EffectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text.Insert(CodeBox.SelectionStart, form.Content);
            }
        }

        private void CodeBox_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (projectPath == null)
                NewProject();

            if (!CodeBox.AutoWordSelection)
            {
                CodeBox.AutoWordSelection = true;
                CodeBox.AutoWordSelection = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EffectForm form = new EffectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text += form.Content;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GameruleForm form = new GameruleForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text += form.Content;
            }
        }

        public void Debug(object text, Color color)
        {
            ErrorBox.SelectionStart = ErrorBox.Text.Length;
            ErrorBox.SelectionColor = color;
            ErrorBox.AppendText("[" + DateTime.Now.ToString() + "] " + text.ToString()+"\n");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPath == null)
            {
                if (ProjectSave.ShowDialog() == DialogResult.OK)
                {
                    projectPath = ProjectSave.FileName;
                }
                else
                    return;
            }
            Save(projectPath);
            UpdateProjectList();
            Debug("Project save as " + projectPath, Color.Aqua);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectSave.ShowDialog() == DialogResult.OK)
            {
                projectPath = ProjectSave.FileName;
            }
            else
                return;

            Save(projectPath);
            UpdateProjectList();
            Debug("Project save as " + projectPath, Color.Aqua);
        }
        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }


        public void Save(string projectPath)
        {
            code[previous] = CodeBox.Text;

            int i = 0;
            ProjectSave save = new ProjectSave();
            save.projectName = projectName;
            save.TagsList = TagsList;
            save.mcTagsList = MCTagsList;
            save.version = projectVersion;
            save.offuscate = isLibraryCheckbox.Checked;
            save.isLibrary = isLibraryCheckbox.Checked;
            List<ProjectSave.FileSave> lst = new List<ProjectSave.FileSave>();
            save.compileOrder = new List<string>();
            save.datapackDirectory = currentDataPack;
            string dir = Path.GetDirectoryName(projectPath)+"/scripts/";
            
            foreach (string file in listBox1.Items)
            {
                if (isLibraryCheckbox.Checked)
                {
                    SafeWriteFile(dir + file + ".bps", code[file]);
                    save.compileOrder.Add(file);
                }
                else
                {
                    lst.Add(new ProjectSave.FileSave(file, code[file], i));
                }
            }
            save.files = lst.ToArray();

            File.WriteAllText(projectPath,JsonConvert.SerializeObject(save));
        }
        public void OpenFile(string name)
        {
            try
            {
                Open(name, File.ReadAllText(name));
                projectPath = name;
                Text = projectName + " - TBMScript";
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Open(string name, string data)
        {
            ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);
            listBox1.Items.Clear();
            code.Clear();
            code = new Dictionary<string, string>();
            TagsList = project.TagsList;
            MCTagsList = project.mcTagsList;
            projectVersion = project.version;

            projectName = project.projectName;
            currentDataPack = project.datapackDirectory;
            isLibraryCheckbox.Checked = project.isLibrary;
            previous = "$$$$$$$$$";
            int i = 0;
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            foreach (var file in project.files)
            {
                ignorNextListboxUpdate = true;
                try
                {
                    code.Add(file.name, file.content);
                }
                catch
                {
                    Debug("Duplicated " +file.name+"-"+file.content+"////"+code[file.name], Color.Red);
                }
                
                listBox1.Items.Add(file.name);
                
                if (i == 0)
                {
                    noReformat = true;
                    CodeBox.Text = file.content;
                    PreviousText.Clear();
                    PreviousText.Add(CodeBox.Text);
                    index = 0;
                    previous = file.name;
                }
                i++;
            }
            if (project.compileOrder != null)
            {
                foreach (var file in project.compileOrder)
                {
                    ignorNextListboxUpdate = true;
                    try
                    {
                        code.Add(file, File.ReadAllText(dir + file + ".bps"));
                    }
                    catch (Exception e)
                    {
                        Debug("Exception while reading " + e.ToString(), Color.Red);
                    }

                    listBox1.Items.Add(file);

                    if (i == 0)
                    {
                        noReformat = true;
                        CodeBox.Text = code[file];
                        PreviousText.Clear();
                        PreviousText.Add(CodeBox.Text);
                        index = 0;
                        previous = file;
                    }
                    i++;
                }
            }
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                {
                    string fname = file.Substring(dir.Length,file.Length - dir.Length - Path.GetExtension(file).Length);

                    if (!code.ContainsKey(fname.ToLower()))
                    {
                        try
                        {
                            code.Add(fname.ToLower(), File.ReadAllText(file));
                        }
                        catch (Exception e)
                        {
                            Debug("Exception while reading " + e.ToString(), Color.Red);
                        }

                        listBox1.Items.Add(fname.ToLower());
                    }
                }
            }
            Debug("Project Loaded: " + projectPath+" ("+i.ToString()+" Files)", Color.Aqua);
            noReformat = false;
            exporting = false;
            CompileJava();
            UpdateCodeBox();
            ignorNextListboxUpdate = false;
        }
        public void UpdateProjectList()
        {
            List<string> lst = new List<string>();
            if (File.Exists("project.old"))
            {
                foreach (string s in File.ReadAllLines("project.old")) lst.Add(s);
            }

            if (lst.Contains(projectPath))
                lst.Remove(projectPath);

            lst.Insert(0, projectPath);

            if (lst.Count > 10)
            {
                lst.RemoveAt(10);
            }

            File.WriteAllLines("project.old", lst.ToArray());
        }
        public void NewFile()
        {
            NewFile form = new NewFile();
            if (form.ShowDialog() == DialogResult.OK && !listBox1.Items.Contains(form.filename))
            {
                if (form.filename != "import")
                    listBox1.Items.Add(form.filename);
                else
                    listBox1.Items.Insert(0, form.filename);
                if (form.type == JSharp.NewFile.Type.STRUCTURE)
                {
                    code.Add(form.filename, "package " + form.filename + "\n\nstruct " + form.filename + "{\n\tdef __init__(){\n\n\t}\n}");
                }
            }
        }
        public void NewProject()
        {
            NewProjectForm form = new NewProjectForm();
            var res = form.ShowDialog();
            bool newP = false;
            if (res == DialogResult.OK)
            {
                projectName = form.ProjectName;
                Text = projectName + " - TBMScript";
                newP = true;

                code.Clear();
                code.Add("import", "import standard.java\nimport standard.entity_id\n");
                code.Add("load", "package main\n");
                code.Add("main", "package main\n");

                MCTagsList.Clear();
                MCTagsList.Add("blocks", new Dictionary<string, TagsList>());
                MCTagsList.Add("functions", new Dictionary<string, TagsList>());
                MCTagsList["functions"].Add("load", new TagsList());
                MCTagsList["functions"].Add("tick", new TagsList());
                MCTagsList["functions"]["load"].values.Add("load");
                MCTagsList["functions"]["tick"].values.Add("main");

                TagsList.Clear();
                TagsList.Add("blocks", new Dictionary<string, TagsList>());
                TagsList.Add("functions", new Dictionary<string, TagsList>());
                structures.Clear();
                currentDataPack = null;
            }
            if (res == DialogResult.Yes)
            {
                projectPath = form.ProjectName;
                Open(form.ProjectName, File.ReadAllText(form.ProjectName));
                Text = projectName + " - TBMScript";
                newP = true;

                if (TagsList == null)
                {
                    MCTagsList=new Dictionary<string, Dictionary<string, TagsList>>();
                    MCTagsList.Add("blocks", new Dictionary<string, TagsList>());
                    MCTagsList.Add("functions", new Dictionary<string, TagsList>());
                    MCTagsList["functions"].Add("load", new TagsList());
                    MCTagsList["functions"].Add("tick", new TagsList());
                    MCTagsList["functions"]["load"].values.Add("load");
                    MCTagsList["functions"]["tick"].values.Add("main");
                    TagsList = new Dictionary<string, Dictionary<string, TagsList>>();
                    TagsList.Add("blocks", new Dictionary<string, TagsList>());
                    TagsList.Add("functions", new Dictionary<string, TagsList>());
                }
                if (structures == null)
                {
                    structures = new Dictionary<string, string>();
                }
            }
        }
        public void AddLineNumbers()
        {
            LineNumberTextBox.ZoomFactor = CodeBox.ZoomFactor;
            LineNumberTextBox.Font = CodeBox.Font;
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);
            // get First Index & First Line from richTextBox1    
            int First_Index = CodeBox.GetCharIndexFromPosition(pt);
            int First_Line = CodeBox.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = CodeBox.Width;
            pt.Y = CodeBox.Height;
            // get Last Index & Last Line from richTextBox1    
            int Last_Index = CodeBox.GetCharIndexFromPosition(pt);
            int Last_Line = CodeBox.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox    
            
            // set LineNumberTextBox text to null & width to getWidth() function value    
            LineNumberTextBox.Text = (First_Line+1).ToString()+"\n";
            LineNumberTextBox.SelectionAlignment = HorizontalAlignment.Center;
            LineNumberTextBox.SelectionFont = CodeBox.Font;
            
            //LineNumberTextBox.Width = getWidth();
            // now add each line number to LineNumberTextBox upto last line    
            for (int i = First_Line+1; i <= Last_Line + 1; i++)
            {
                LineNumberTextBox.Text += i + 1 + "\n";
            }
        }
        public void UpdateCodeBox()
        {
            Formatter.reformat(CodeBox, this, false);/*
            if (Compiler.enviroments != null)
            {
                autocompleteMenu1.Items = new string[] { };
                try
                {
                    foreach (var m in Compiler.enviroments[previous])
                    {
                        autocompleteMenu1.AddItem(m);
                    }
                }
                catch
                {

                }
            }*/
        }
        public static void SafeWriteFile(string fileName, string content)
        {
            string filePath = fileName.Substring(0, fileName.LastIndexOf('/'));
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.WriteAllText(fileName, content);
        }
        public void ExportDataPackThread()
        {
            isCompiling = 1;
            ExportDataPack(currentDataPack);
            //Process.Start("explorer.exe", currentDataPack);
            isCompiling = 1000;
        }
        public void ExportDataPack(string path)
        {
            try
            {
                if (File.Exists(path + "/pack.mcmeta"))
                {
                    Directory.Delete(path, true);
                }
                projectVersion.Build();
                ExportFiles(path);
                ExportTags(path);
                ExportStructures(path);
                ExportReadMe(path);

                SafeWriteFile(path + "/pack.mcmeta",
                            JsonConvert.SerializeObject(new DataPackMeta(projectName)));

                DebugThread("Datapack successfully exported!", Color.Aqua);
            }
            catch (Exception er)
            {
                DebugThread(er, Color.Red);
            }
        }
        public void ExportFiles(string path)
        {
            List<Compiler.File> files = new List<Compiler.File>();
            foreach (string f in listBox1.Items)
            {
                files.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
            }

            List<Compiler.File> cFiles = Compiler.compile(new CompilerCoreJava(),projectName, files, DebugThread, true, projectVersion, Path.GetDirectoryName(projectPath));
            foreach (Compiler.File f in cFiles)
            {
                string fileName = path + "/data/" + projectName.ToLower() + "/functions/" + f.name + ".mcfunction";
                try
                {
                    SafeWriteFile(fileName, f.content);
                }
                catch(Exception e)
                {
                    throw new Exception(fileName + " "+e.ToString());
                }
            }
        }
        public void ExportTags(string path)
        {
            foreach (string key in MCTagsList.Keys)
            {
                foreach (string file in MCTagsList[key].Keys)
                {
                    if (key == "functions")
                    {
                        SafeWriteFile(path + "/data/minecraft/tags/" + key + "/" + file + ".json",
                            JsonConvert.SerializeObject(MCTagsList[key][file].ToFunctions(projectName.ToLower())));
                    }
                    else
                    {
                        SafeWriteFile(path + "/data/minecraft/tags/" + key + "/" + file + ".json",
                            JsonConvert.SerializeObject(MCTagsList[key][file]));
                    }
                }
            }
            foreach (string key in TagsList.Keys)
            {
                foreach (string file in TagsList[key].Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/" + key + "/" + file + ".json",
                        JsonConvert.SerializeObject(TagsList[key][file]));
                }
            }
            foreach (string key in Compiler.blockTags.Keys)
            {
                SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/blocks/" + key + ".json",
                        JsonConvert.SerializeObject(Compiler.blockTags[key]));
            }
        }
        public void ExportReadMe(string path)
        {
            string readme = "This Datapack was made using TheblueMan003's Compiler.\n" +
                            "Therefor all variables & functions might have wierd name.\n"+
                            "Please refers to MAPS.txt";
            string offuscation = "#==================================#\n" +
                                 "In order to compile each variable to a unique scoreboard name the compiler use an offuscation map\n"+
                                 "#==================================#\n";
            offuscation += "variable count = " + Compiler.offuscationMap.Keys.Count+"\n";
            offuscation += "alphabet = " + Compiler.alphabet + "\n\n";
            foreach (string key in Compiler.offuscationMap.Keys)
            {
                offuscation += key + " <===> " + Compiler.offuscationMap[key]+"\n";
            }
            string fileNameReadMe = path + "/README.txt";
            string fileNameOffuscation = path + "/MAPS.txt";
            try
            {
                SafeWriteFile(fileNameReadMe, readme);
                SafeWriteFile(fileNameOffuscation, offuscation);
            }
            catch (Exception e)
            {
                throw new Exception("Notice " + e.ToString());
            }
            
        }
        public void ExportStructures(string path)
        {
            if (Directory.Exists(ProjectFolder() + "/structures"))
            {
                Directory.CreateDirectory(path + "/data/" + projectName.ToLower() + "/structures/");
                
                foreach (string file in Directory.GetFiles(ProjectFolder() + "/structures"))
                {
                    File.Copy(file, path + "/data/" + projectName.ToLower() + "/structures/" + Path.GetFileName(file));
                }
            }
        }
        public void ChangeCompileOrder()
        {
            CompileOrder form = new CompileOrder(listBox1.Items, listBox1.Items.Contains("import")?1:0);
            if (form.ShowDialog() == DialogResult.OK)
            {
                ignorNextListboxUpdate = true;
                listBox1.Items.Clear();
                listBox1.Items.AddRange(form.Content);
                Debug("Compile Order Changed", Color.Aqua);
            }
        }
        public void CompileJava(bool showForm = false)
        {
            if (isCompiling == 0)
            {
                this.showForm = showForm;
                projectVersion.Build();
                compileFile = new List<Compiler.File>();
                foreach (string f in listBox1.Items)
                {
                    compileFile.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
                }

                code[previous] = CodeBox.Text;
                Thread t = new Thread(new ThreadStart(CompileJavaThreaded));
                t.Start();
            }
        }
        public void CompileBedrock(bool showForm = false)
        {
            if (isCompiling == 0)
            {
                this.showForm = showForm;
                projectVersion.Build();
                compileFile = new List<Compiler.File>();
                foreach (string f in listBox1.Items)
                {
                    compileFile.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
                }

                code[previous] = CodeBox.Text;
                Thread t = new Thread(new ThreadStart(CompileBedrockThreaded));
                t.Start();
            }
        }

        public void CompileJavaThreaded()
        {
            try
            {
                isCompiling = 1;
                compileFiled = Compiler.compile(new CompilerCoreJava(),projectName, compileFile, DebugThread, exporting, projectVersion,
                    Path.GetDirectoryName(projectPath));

                if (showForm)
                {
                    isCompiling = 2;
                }
                else
                {
                    isCompiling = 0;
                }
            }
            catch (Exception er)
            {
                isCompiling = 0;
                DebugThread(er, Color.Red);
            }

        }
        public void CompileBedrockThreaded()
        {
            try
            {
                isCompiling = 1;
                compileFiled = Compiler.compile(new CompilerCoreBedrock(), projectName, compileFile, DebugThread, exporting, projectVersion,
                    Path.GetDirectoryName(projectPath));

                if (showForm)
                {
                    isCompiling = 2;
                }
                else
                {
                    isCompiling = 0;
                }
            }
            catch (Exception er)
            {
                isCompiling = 0;
                DebugThread(er, Color.Red);
            }

        }

        public void ReIndent()
        {
            try
            {
                Regex reg = new Regex(@"(?s)^\s*((if)|(for)|(while))\b");
                Stack<char> chars = new Stack<char>();
                string[] textArr = CodeBox.Text.Split('\n');
                string text = "";
                int softCond = 0;
                for (int i = 0; i < textArr.Length; i++)
                {
                    int shift = 0;
                    if (textArr[i].Contains("}"))
                        shift += textArr[i].Split('}').Length - textArr[i].Split('{').Length;

                    for (int j = 0; j < chars.Count - shift; j++)
                    {
                        text += "\t";
                    }
                    text += textArr[i].Replace("\t", "");
                    if (i < textArr.Length - 1)
                        text += "\n";

                    bool inComment = false;
                    bool inString = false;
                    char cPrev = '\n';

                    foreach (char c in textArr[i])
                    {
                        if (c == '"' && cPrev != '\\')
                        {
                            inString = !inString;
                        }
                        else if (c == '\\' && cPrev == '\\')
                        {
                            inComment = true;
                        }
                        else if (c == '{' && !inComment && !inString)
                        {
                            chars.Push(c);
                        }
                        else if (c == '}' && !inComment && !inString && chars.Pop() != '{')
                        {
                            chars.Pop();
                        }
                    }
                    if (reg.Match(textArr[i]).Success && !textArr[i].Contains("{"))
                    {
                        softCond++;
                        chars.Push('{');
                    }
                    else if (softCond > 0)
                    {
                        chars.Pop();
                        softCond -= 1;
                    }
                }
                CodeBox.Text = text;
            }
            catch(Exception e)
            {
                Debug(e, Color.Red);
            }
        }
        public string ProjectFolder()
        {
            return projectPath.Replace(Path.GetFileName(projectPath), "");
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ProjectOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenFile(ProjectOpen.FileName);
                }
                catch(Exception error)
                {
                    MessageBox.Show(error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                return;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void datapackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentDataPack != null || ExportSave.ShowDialog() == DialogResult.OK)
            {
                string path = (currentDataPack == null) ? ExportSave.FileName : currentDataPack;
                currentDataPack = path;
                if (isCompiling == 0)
                {
                    Thread t = new Thread(new ThreadStart(ExportDataPackThread));
                    t.Start();
                    //ExportDataPack(path);
                }
            }
        }

        private void compileOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCompileOrder();
        }
        private void reformatToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ReIndent();
            UpdateCodeBox();
        }
        private void CodeBox_VScroll(object sender, EventArgs e)
        {
            AddLineNumbers();
        }
        private void tagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TagsForm form = new TagsForm(TagsList);
            if (form.ShowDialog() == DialogResult.OK)
            {
                TagsList = form.data;
                
                Debug("Project Tags Changed", Color.Aqua);
            }
        }
        private void projectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TagsForm form = new TagsForm(TagsList);
            if (form.ShowDialog() == DialogResult.OK)
            {
                TagsList = form.data;

                Debug("Project Tags Changed", Color.Aqua);
            }
        }
        private void minecraftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TagsForm form = new TagsForm(MCTagsList);
            if (form.ShowDialog() == DialogResult.OK)
            {
                TagsList = form.data;

                Debug("Project Tags Changed", Color.Aqua);
            }
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ChangeCompileOrder();
        }
        private void newDatapackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ExportSave.ShowDialog() == DialogResult.OK)
            {
                string path = ExportSave.FileName;
                currentDataPack = path;
                ExportDataPack(path);
            }
        }

        private void structuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StructureImport(projectPath.Replace(Path.GetFileName(projectPath),"")).ShowDialog();
        }
        
        private void CodeBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                // Enter
                case '\r':
                    // On compte le nombre de 'tab' de la ligne précédente
                    string temp = Convert.ToString(CodeBox.Lines.GetValue(CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart - 1)));
                    Regex tab = new Regex("\t");

                    int indent = tab.Matches(temp).Count;
                    if (temp.EndsWith("{"))
                        // si la ligne finit par "{" (début de struct / class / etc...) on ajoute une indentation
                        indent++;

                    temp = Convert.ToString(CodeBox.Lines.GetValue(CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart)));
                    if (temp.Contains("}") && !temp.Contains("{"))
                        // si la ligne contient "}" (fin de struct / class / etc...) on enlève une indentation
                        indent = Math.Max(0, indent - 1);

                    // et on la place dans le texte
                    string temp2 = "{TAB " + indent + "}";

                    SendKeys.Send(temp2);
                    break;

                // '}'
                case '}':
                    SendKeys.Send("{LEFT}{BACKSPACE}{RIGHT}");
                    break;

                default:
                    break;
            }
            switch (e.KeyCode)
            {
                case Keys.Home:
                    string line = CodeBox.Lines[CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart)];
                    int start = CodeBox.GetFirstCharIndexFromLine(CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart));
                    int shift = 0;
                    string tmp2 = "";
                    while (line.StartsWith("\t"))
                    {
                        tmp2 += "{RIGHT}";
                        shift++;
                        line = line.Substring(1, line.Length - 1);
                    }
                    if (start+shift != CodeBox.SelectionStart)
                        SendKeys.Send(tmp2);
                    break;

                case Keys.Left:
                    if (e.Control && !selfShift)
                    {
                        int i = CodeBox.SelectionStart-1;
                        tmp2 = "";
                        Regex reg = new Regex("[a-zA-Z0-9_\\$]");
                        while (i > 0 && reg.Match(CodeBox.Text[i].ToString()).Success)
                        {
                            i--;
                            if (CodeBox.Text[i] == '_')
                            {
                                tmp2 += "{Left}{Left}";
                            }
                        }
                        SendKeys.Send(tmp2);
                        selfShift = true;
                    }
                    break;
                case Keys.Right:
                    if (e.Control && !selfShift)
                    {
                        int i = CodeBox.SelectionStart;
                        tmp2 = "";
                        Regex reg = new Regex("[a-zA-Z0-9_\\$]");
                        while (i < CodeBox.Text.Length && reg.Match(CodeBox.Text[i].ToString()).Success)
                        {
                            i++;
                            if (CodeBox.Text[i] == '_')
                            {
                                tmp2 += "{RIGHT}{RIGHT}";
                            }
                        }
                        selfShift = true;
                        SendKeys.Send(tmp2);
                    }
                    break;

                default:
                    selfShift = false;
                    break;
            }
        }

        public void DebugThread(object msg, Color c)
        {
            debugMSGs.Add(new DebugMessage(msg.ToString(), c));
        }
        private class DebugMessage
        {
            public string msg;
            public Color color;
            public DebugMessage(string msg, Color color)
            {
                this.msg = msg;
                this.color = color;
            }
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.functions);
            fp.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.structs);
            fp.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.variables);
            fp.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.enums);
            fp.Show();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find_and_Replace far = new Find_and_Replace(CodeBox);
            far.Show();
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find_and_Replace far = new Find_and_Replace(CodeBox);
            far.Show();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (index >= 1)
            {
                ignoreMod = true;
                CodeBox.Text = PreviousText[--index];
                ignoreMod = false;
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (index < PreviousText.Count-1)
            {
                ignoreMod = true;
                CodeBox.Text = PreviousText[++index];
                ignoreMod = false;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(CommandParser.names);
            fp.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(CommandParser.sounds);
            fp.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            exporting = false;
            CompileBedrock(true);
            UpdateCodeBox();
        }
    }

}
