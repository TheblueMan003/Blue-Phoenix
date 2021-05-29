﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BluePhoenix;

namespace JSharp
{
    public partial class Form1 : Form
    {
        public Dictionary<string, string> code = new Dictionary<string, string>();
        public Dictionary<string, string> resources = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, TagsList>> TagsList = new Dictionary<string, Dictionary<string, TagsList>>();
        public Dictionary<string, Dictionary<string, TagsList>> MCTagsList = new Dictionary<string, Dictionary<string, TagsList>>();
        public Dictionary<string, DateTime> moddificationFileTime = new Dictionary<string, DateTime>();
        public Dictionary<string, DateTime> moddificationResTime = new Dictionary<string, DateTime>();
        public Dictionary<string, string> structures = new Dictionary<string, string>();
        public List<string> codeOrder = new List<string>();
        public List<string> resourceOrder = new List<string>();
        public ProjectVersion projectVersion = new ProjectVersion();
        public Compiler.CompilerSetting compilerSetting = new Compiler.CompilerSetting();
        public ResourcesPackEditor ResourcesPackEditor;
        
        public string projectDescription;
        
        private string previous = "load";
        private string projectName = "default";
        private string currentDataPack;
        private string currentResourcesPack;
        public string projectPath;
        public bool ignorNextListboxUpdate = false;
        public bool ignorNextKey = false;
        public bool isLibrary;
        public int isCompiling = 0;

        private Task CompileThread;
        private CancellationTokenSource tokenSource2 = new CancellationTokenSource();

        public List<Compiler.File> compileFile;
        public List<Compiler.File> compileResource;
        public List<Compiler.File> compileFiled;
        private List<DebugMessage> debugMSGs = new List<DebugMessage>();
        private int lastSeen = -1;
        private bool showForm;

        private List<string> PreviousText = new List<string>();
        private bool ignoreMod = false;
        private bool noReformat = false;
        private bool exporting;
        private bool resourceSelected = false;
        private int index = 0;
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
        private void recallFile()
        {
            if (resourceSelected)
            {
                resources[previous] = CodeBox.Text;
            }
            else
            {
                code[previous] = CodeBox.Text;
            }
        }
        public void ReloadCodeBoxFileCode(string file)
        {
            noReformat = true;

            recallFile();
            resourceSelected = false;

            PreviousText.Clear();

            index = 0;
            if (code.ContainsKey(file))
                CodeBox.Text = code[file];
            else
            {
                CodeBox.Text = "";
            }

            PreviousText.Add(CodeBox.Text);

            previous = file;
            noReformat = false;
            UpdateCodeBox();

            //ignorNextListboxUpdate = true;
            //ResourceListBox.SelectedIndex = -1;
        }
        public void ReloadCodeBoxFileRes(string file)
        {
            noReformat = true;

            recallFile();
            resourceSelected = true;

            PreviousText.Clear();

            index = 0;
            if (resources.ContainsKey(file))
                CodeBox.Text = resources[file];
            else
            {
                CodeBox.Text = "";
            }
            PreviousText.Add(CodeBox.Text);
            previous = file;
            noReformat = false;
            UpdateCodeBox();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            exporting = false;
            CompileJava(true);
            UpdateCodeBox();
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
                AddLineNumbers();
                Formatter.reformat(CodeBox, this, true);
            }
        }
        private void CodeBox_Leave(object sender, EventArgs e)
        {
            recallFile();
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
                if (Compiler.resourcespackFiles.Count > 0)
                {
                    FilePreview rpfilePreview = new FilePreview(Compiler.resourcespackFiles);
                    filePreview.Show();
                }
            }
            if (isCompiling == 1000)
            {
                SystemSounds.Beep.Play();
                isCompiling = 0;
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


        public void Debug(object text, Color color)
        {
            ErrorBox.SelectionStart = ErrorBox.Text.Length;
            ErrorBox.SelectionColor = color;
            ErrorBox.AppendText("[" + DateTime.Now.ToString() + "] " + text.ToString()+"\n");
        }



        public void Save(string projectPath)
        {
            recallFile();

            int i = 0;
            ProjectSave project = new ProjectSave();
            project.projectName = projectName;
            project.TagsList = TagsList;
            project.mcTagsList = MCTagsList;
            project.version = projectVersion;
            project.offuscate = project.compilationSetting.isLibrary;
            project.isLibrary = project.compilationSetting.isLibrary;
            project.compilationSetting = compilerSetting;
            List<ProjectSave.FileSave> lst = new List<ProjectSave.FileSave>();
            List<ProjectSave.FileSave> lstRes = new List<ProjectSave.FileSave>();
            project.compileOrder = new List<string>();
            project.datapackDirectory = currentDataPack;
            project.resourcesPackDirectory = currentResourcesPack;
            string dir = Path.GetDirectoryName(projectPath)+"/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";

            foreach (string file in codeOrder)
            {
                if (project.compilationSetting.isLibrary)
                {
                    if (!file.Contains('.')) {
                        SafeWriteFile(dir + file + ".bps", code[file]);
                        moddificationFileTime[file] = DateTime.Now.AddSeconds(5);
                        project.compileOrder.Add(file);
                    }
                    else
                    {
                        SafeWriteFile(dir + file, code[file]);
                        moddificationFileTime[file] = DateTime.Now.AddSeconds(5);
                        project.compileOrder.Add(file);
                    }
                }
                else
                {
                    lst.Add(new ProjectSave.FileSave(file, code[file], i));
                }
            }
            foreach (string file in resourceOrder)
            {
                if (project.compilationSetting.isLibrary)
                {
                    SafeWriteFile(dirRes + file, resources[file]);
                    moddificationResTime[file] = DateTime.Now.AddSeconds(5);
                }
                else
                {
                    lstRes.Add(new ProjectSave.FileSave(file, resources[file], i));
                }
            }
            project.files = lst.ToArray();
            project.resources = lstRes.ToArray();
            project.description = projectDescription;

            File.WriteAllText(projectPath,JsonConvert.SerializeObject(project));
        }
        public void OpenFile(string name)
        {
            try
            {
                projectPath = name;
                Open(name, File.ReadAllText(name));
                Text = projectName + " - TBMScript";
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Open(string name, string data)
        {
            moddificationFileTime = new Dictionary<string, DateTime>();
            moddificationResTime = new Dictionary<string, DateTime>();

            ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);
            codeOrder.Clear();
            code.Clear();
            code = new Dictionary<string, string>();
            TagsList = project.TagsList;
            MCTagsList = project.mcTagsList;
            projectVersion = project.version;

            projectName = project.projectName;
            currentDataPack = project.datapackDirectory;
            currentResourcesPack = project.resourcesPackDirectory;
            project.compilationSetting.isLibrary = project.isLibrary;
            compilerSetting = project.compilationSetting;

            previous = "$$$$$$$$$";
            int i = 0;
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
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

                codeOrder.Add(file.name);
                
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
            
            if (project.resources != null)
            {
                foreach (var file in project.resources)
                {
                    ignorNextListboxUpdate = true;
                    try
                    {
                        resources.Add(file.name, file.content);
                    }
                    catch
                    {
                        Debug("Duplicated " + file.name + "-" + file.content + "////" + resources[file.name], Color.Red);
                    }

                    resourceOrder.Add(file.name);

                    i++;
                }
            }

            if (project.compileOrder != null)
            {
                foreach (var file in project.compileOrder)
                {
                    ignorNextListboxUpdate = true;
                    try
                    {
                        if (!file.Contains('.'))
                            code.Add(file, File.ReadAllText(dir + file + ".bps"));
                        else
                            code.Add(file, File.ReadAllText(dir + file));
                    }
                    catch (Exception e)
                    {
                        Debug("Exception while reading " + e.ToString(), Color.Red);
                    }

                    codeOrder.Add(file);

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
                    string fname;
                    if (file.EndsWith(".bps"))
                        fname = file.Substring(dir.Length,file.Length - dir.Length - Path.GetExtension(file).Length);
                    else
                        fname = file.Substring(dir.Length, file.Length - dir.Length);

                    moddificationFileTime.Add(fname.ToLower(), DateTime.Now);

                    if (!code.ContainsKey(fname.ToLower()) && fname != "desktop.ini")
                    {
                        try
                        {
                            code.Add(fname.ToLower(), File.ReadAllText(file));
                        }
                        catch (Exception e)
                        {
                            Debug("Exception while reading " + e.ToString(), Color.Red);
                        }

                        codeOrder.Add(fname.ToLower());
                    }
                }
            }
            if (Directory.Exists(dirRes))
            {
                foreach (var file in Directory.GetFiles(dirRes, "*.*", SearchOption.AllDirectories))
                {
                    string fname = file.Substring(dirRes.Length, file.Length - dirRes.Length);
                    moddificationResTime.Add(fname.ToLower(), DateTime.Now);

                    if (!resources.ContainsKey(fname.ToLower()))
                    {
                        try
                        {
                            resources.Add(fname.ToLower(), File.ReadAllText(file));
                        }
                        catch (Exception e)
                        {
                            Debug("Exception while reading " + e.ToString(), Color.Red);
                        }

                        resourceOrder.Add(fname.ToLower());
                    }
                }
            }

            ReloadTree();

            Debug("Project Loaded: " + projectPath+" ("+i.ToString()+" Files)", Color.Aqua);
            noReformat = false;
            exporting = false;
            CompileJava();
            UpdateCodeBox();
            ignorNextListboxUpdate = false;
            projectDescription = project.description;
        }
        public void CheckFileModdification()
        {
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
            UpdateFile.Result prev = UpdateFile.Result.No;
            List<string> keys = new List<string>();
            keys.AddRange(moddificationFileTime.Keys);
            foreach (string key in keys)
            {
                if (File.Exists(dir+key + ".bps") && moddificationFileTime[key] < File.GetLastWriteTime(dir+ key + ".bps"))
                {
                    if (prev == UpdateFile.Result.NoForAll)
                    {
                        moddificationFileTime[key] = File.GetLastWriteTime(dir + key + ".bps");
                    }
                    else if (prev == UpdateFile.Result.YesForAll)
                    {
                        code[key] = File.ReadAllText(dir + key + ".bps");
                        moddificationFileTime[key] = File.GetLastWriteTime(dir + key + ".bps");
                        if (previous == key)
                        {
                            CodeBox.Text = code[key];
                            Formatter.reformat(CodeBox, this, false);
                        }
                    }
                    else
                    {
                        var res = UpdateFile.ShowDialog("The File \"" + key + "\" has been moddify. Do you want to reload it?");
                        prev = res;
                        if (res == UpdateFile.Result.YesForAll || res == UpdateFile.Result.Yes)
                        {
                            code[key] = File.ReadAllText(dir + key + ".bps");
                            moddificationFileTime[key] = File.GetLastWriteTime(dir + key + ".bps");
                            if (previous == key)
                            {
                                CodeBox.Text = code[key];
                                Formatter.reformat(CodeBox, this, false);
                            }
                        }
                        else
                        {
                            moddificationFileTime[key] = File.GetLastWriteTime(dir + key + ".bps");
                        }
                    }
                }
            }
            keys = new List<string>();
            keys.AddRange(moddificationResTime.Keys);
            foreach (string key in keys)
            {
                if (File.Exists(dir + key) && moddificationResTime[key] < File.GetLastWriteTime(dir + key))
                {
                    if (prev == UpdateFile.Result.NoForAll)
                    {
                        moddificationResTime[key] = File.GetLastWriteTime(dir + key);
                    }
                    else if (prev == UpdateFile.Result.YesForAll)
                    {
                        resources[key] = File.ReadAllText(dirRes + key);
                        moddificationResTime[key] = File.GetLastWriteTime(dir + key);
                        if (previous == key)
                        {
                            CodeBox.Text = resources[key];
                            Formatter.reformat(CodeBox, this, false);
                        }
                    }
                    else
                    {
                        var res = UpdateFile.ShowDialog("The Resource File \"" + key + "\" has been moddify. Do you want to reload it?");
                        prev = res;
                        if (res == UpdateFile.Result.YesForAll || res == UpdateFile.Result.Yes)
                        {
                            resources[key] = File.ReadAllText(dir + key);
                            moddificationResTime[key] = File.GetLastWriteTime(dirRes + key);
                            if (previous == key)
                            {
                                CodeBox.Text = resources[key];
                                Formatter.reformat(CodeBox, this, false);
                            }
                        }
                        else
                        {
                            moddificationResTime[key] = File.GetLastWriteTime(dir + key);
                        }
                    }
                }
            }
        }
        public void UpdateProjectList()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";

            List<string> lst = new List<string>();
            if (File.Exists(path+"project.old"))
            {
                foreach (string s in File.ReadAllLines(path+"project.old")) lst.Add(s);
            }

            if (lst.Contains(projectPath))
                lst.Remove(projectPath);

            lst.Insert(0, projectPath);

            if (lst.Count > 20)
            {
                lst.RemoveAt(20);
            }

            File.WriteAllLines(path+"project.old", lst.ToArray());
        }
        public string GenerateDatapackLink(string path)
        {
            path = Path.GetDirectoryName(path);
            Debug("added external datapack: " + path, Color.Lime);
            string output = "";
            foreach (string n in Directory.GetDirectories(path + "/data/", "*", SearchOption.TopDirectoryOnly))
            {
                string names = n.Replace(path + "/data/", "");
                if (Directory.Exists(path + "/data/" + names + "/functions"))
                {
                    foreach (string file in Directory.GetFiles(path + "/data/" + names + "/functions", "*.mcfunction", SearchOption.AllDirectories))
                    {
                        output += "def external " + names + 
                            file.Replace("\\", "/")
                            .Replace(path.Replace("\\", "/") + "/data/" + names + "/functions", "")
                            .Replace("/", ".")
                            .Replace(".mcfunction", "") + "()\n";
                        Debug("added external function: " + file, Color.Lime);
                    }
                }
            }
            return output;
        }
        public void NewFile()
        {
            NewFile form = new NewFile();
            if (form.ShowDialog() == DialogResult.OK && !code.Keys.Contains(form.filename))
            {
                if (form.type == JSharp.NewFile.Type.EXTERNAL)
                {
                    if (DatapackOpen.ShowDialog() == DialogResult.OK)
                    {
                        codeOrder.Add(form.filename);
                        code.Add(form.filename.ToLower(), GenerateDatapackLink(DatapackOpen.FileName));
                    }
                }
                else if (form.type == JSharp.NewFile.Type.RESOURCE)
                {
                    resourceOrder.Add(form.filename);
                    resources.Add(form.filename, "");
                }
                else
                {
                    if (form.filename != "import")
                        codeOrder.Add(form.filename);
                    else
                        codeOrder.Insert(0, form.filename);
                    if (form.type == JSharp.NewFile.Type.STRUCTURE)
                    {
                        code.Add(form.filename, "package " + form.filename.Replace("/",".") + "\n\nstruct " + form.filename.Replace("/", ".") + "{\n\tdef __init__(){\n\n\t}\n}");
                    }
                    else if (form.type == JSharp.NewFile.Type.SUBPROGRAMME)
                    {
                        code.Add(form.filename, "package " + form.filename.Replace("/", ".") + "\n\nBOOL Enabled\ndef ticking main(){\n\twith(@a,true,Enabled){\n\t\t\n\t}\n}\n\ndef start(){\n\tEnabled = true\n}\n\ndef close(){\n\tEnabled = false\n}");
                    }
                    else
                    {
                        code.Add(form.filename, "package " + form.filename.Replace("/", "."));
                    }
                }
                ReloadTree();
            }
        }
        public void NewProject()
        {
            NewProjectForm form = new NewProjectForm();
            var res = form.ShowDialog();
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                codeOrder.Clear();
                code.Clear();
                resources.Clear();
                resourceOrder.Clear();
                projectVersion = new ProjectVersion();
                projectDescription = "";
                moddificationFileTime.Clear();
                moddificationResTime.Clear();
                compilerSetting = new Compiler.CompilerSetting();
                structures.Clear();
                MCTagsList.Clear();
                TagsList.Clear();
                CodeBox.Text = "";
            }
            if (res == DialogResult.OK)
            {
                projectName = form.ProjectName;
                compilerSetting.MCVersion = form.MCVersion;
                compilerSetting.libraryFolder.Insert(0, "./lib/1_17/");

                Text = projectName + " - TBMScript";

                code.Clear();
                LibImport libForm2 = new LibImport(compilerSetting.libraryFolder);
                libForm2.ShowDialog();
                string libs = libForm2.import.Count() > 0 ? libForm2.import.Select(x => $"import {x}").Aggregate((x, y) => x + "\n" + y) : "";
                code.Add("import", libs);
                code.Add(projectName.ToLower(), "package "+ projectName.ToLower()+"\n");

                codeOrder.Add("import");
                codeOrder.Add(projectName.ToLower());

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
                ReloadTree();
            }
            if (res == DialogResult.Yes)
            {
                projectPath = form.ProjectName;
                Open(form.ProjectName, File.ReadAllText(form.ProjectName));
                Text = projectName + " - TBMScript";

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
            Formatter.reformat(CodeBox, this, false);
        }
        public static void SafeCopy(string src, string dest)
        {
            string filePath = Path.GetDirectoryName(dest);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.Copy(src, dest, true);
        }
        public static void SafeWriteFile(string fileName, string content)
        {
            Task.Factory.StartNew(() =>
            {
                string filePath = fileName.Substring(0, fileName.LastIndexOf('/'));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                File.WriteAllText(fileName, content);
            }
            );
        }
        #region Compile & Export
        public void ExportDataPackThread()
        {
            isCompiling = 1;
            ExportDataPack(currentDataPack, currentResourcesPack);
            isCompiling = 1000;
        }
        public void ExportDataPack(string path, string rpPath)
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
                            JsonConvert.SerializeObject(new DataPackMeta(projectName +" - "+ projectDescription)));

                ExportResourcePack(rpPath);
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
            foreach (string f in codeOrder)
            {
                files.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
            }

            string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
            if (Directory.Exists(rpdir))
            {
                foreach (var file in Directory.GetFiles(rpdir, "*.bps", SearchOption.AllDirectories))
                {
                    var f = new Compiler.File(file.Replace(rpdir,""), File.ReadAllText(file).Replace('\t' + "", ""));
                    f.resourcespack = true;
                    files.Add(f);
                }
            }

            List<Compiler.File> resourcesfiles = new List<Compiler.File>();
            foreach (string f in resourceOrder)
            {
                resourcesfiles.Add(new Compiler.File(f, resources[f].Replace('\t' + "", "")));
            }

            List<Compiler.File> cFiles = Compiler.compile(new CompilerCoreJava(), projectName, files, resourcesfiles,
                                        DebugThread, compilerSetting, projectVersion,
                                        Path.GetDirectoryName(projectPath));
            
            foreach (Compiler.File f in cFiles)
            {
                string fileName;
                if (f.type == "json" && f.name.Contains("json"))
                    fileName = path + "/data/" + projectName.ToLower() + "/" + f.name;
                else if (f.type == "json" && !f.name.Contains("json"))
                    fileName = path + "/data/" + projectName.ToLower() + "/" + f.name + ".json";
                else
                    fileName = path + "/data/" + projectName.ToLower() + "/functions/" + f.name + ".mcfunction";
                try
                {
                    SafeWriteFile(fileName, f.content);
                }
                catch (Exception e)
                {
                    throw new Exception(fileName + " " + e.ToString());
                }
            }
            if (Directory.Exists(rpdir))
            {

                string rpPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/tmp_rp";
                if (Directory.Exists(rpPath))
                {
                    Directory.Delete(rpPath, true);
                    Directory.CreateDirectory(rpPath);
                }
                else
                {
                    Directory.CreateDirectory(rpPath);
                }

                foreach (Compiler.File f in Compiler.resourcespackFiles)
                {
                    string fileName = rpPath + "/" + f.name + ".json";
                    try
                    {
                        SafeWriteFile(fileName, f.content);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(fileName + " " + e.ToString());
                    }
                }

                foreach (var file in Directory.GetFiles(rpdir, "*.*", SearchOption.AllDirectories))
                {
                    if (!file.EndsWith(".bps"))
                    {
                        string fileName = file.Replace(rpdir, rpPath);
                        SafeCopy(file, fileName);
                    }
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
            foreach (string key in Compiler.entityTags.Keys)
            {
                SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/entity_types/" + key + ".json",
                        JsonConvert.SerializeObject(Compiler.entityTags[key]));
            }
            foreach (string key in Compiler.itemTags.Keys)
            {
                SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/items/" + key + ".json",
                        JsonConvert.SerializeObject(Compiler.itemTags[key]));
            }
        }
        public void ExportReadMe(string path)
        {
            string readme = "This Datapack was made using TheblueMan003's Compiler.\n" +
                            "Therefor all variables & functions might have wierd name.\n" +
                            "Please refers to MAPS.txt";
            string offuscation = "#==================================#\n" +
                                    "In order to compile each variable to a unique scoreboard name the compiler use an offuscation map\n" +
                                    "#==================================#\n";
            offuscation += "variable count = " + Compiler.offuscationMap.Keys.Count + "\n";
            offuscation += "alphabet = " + Compiler.alphabet + "\n\n";
            foreach (string key in Compiler.offuscationMap.Keys)
            {
                offuscation += key + " <===> " + Compiler.offuscationMap[key] + "\n";
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
            Task.Factory.StartNew(() =>
            {
                if (Directory.Exists(ProjectFolder() + "/structures"))
                {
                    Directory.CreateDirectory(path + "/data/" + projectName.ToLower() + "/structures/");

                    foreach (string file in Directory.GetFiles(ProjectFolder() + "/structures"))
                    {
                        File.Copy(file, path + "/data/" + projectName.ToLower() + "/structures/" + Path.GetFileName(file));
                    }
                }
            });
        }
        public void ExportResourcePack(string path)
        {
            if (path != null && path != "")
            {
                string rpPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/tmp_rp";
                SafeWriteFile(rpPath + "/pack.mcmeta",
                                JsonConvert.SerializeObject(new ResourcePackMeta(projectName + " - " + projectDescription)));

                if (Directory.Exists(rpPath))
                {
                    if (File.Exists(path)) { File.Delete(path); }
                    ZipFile.CreateFromDirectory(rpPath, path);
                }
            }
        }
        #endregion
        public void ChangeCompileOrder()
        {
            CompileOrder form = new CompileOrder(codeOrder, codeOrder.Contains("import")?1:0);
            if (form.ShowDialog() == DialogResult.OK)
            {
                ignorNextListboxUpdate = true;
                codeOrder.Clear();
                codeOrder.AddRange(form.Content);
                Debug("Compile Order Changed", Color.Aqua);
            }
        }
        public void CompileJava(bool showForm = false)
        {
            if (isCompiling > 0)
            {
                tokenSource2.Cancel();
                isCompiling = 0;
            }
            if (isCompiling == 0)
            {
                this.showForm = showForm;
                projectVersion.Build();
                compileFile = new List<Compiler.File>();

                recallFile();

                foreach (string f in codeOrder)
                {
                    compileFile.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
                }
                string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
                if (Directory.Exists(rpdir))
                {
                    foreach (var file in Directory.GetFiles(rpdir, "*.bps", SearchOption.AllDirectories))
                    {
                        var f = new Compiler.File(file.Replace(rpdir, ""), File.ReadAllText(file).Replace('\t' + "", ""));
                        f.resourcespack = true;
                        compileFile.Add(f);
                    }
                }

                compileResource = new List<Compiler.File>();
                foreach (string f in resourceOrder)
                {
                    compileResource.Add(new Compiler.File(f, resources[f].Replace('\t' + "", "")));
                }

                CompileThread = new Task(CompileJavaThreaded, tokenSource2.Token);
                CompileThread.Start();
            }
        }
        public void GetCallStackTrace()
        {
            if (isCompiling == 0)
            {
                this.showForm = true;
                projectVersion.Build();
                compileFile = new List<Compiler.File>();

                recallFile();

                foreach (string f in codeOrder)
                {
                    compileFile.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
                }

                compileResource = new List<Compiler.File>();
                foreach (string f in resourceOrder)
                {
                    compileResource.Add(new Compiler.File(f, resources[f].Replace('\t' + "", "")));
                }

                CompileThread = new Task(GetCallStackTraceThreaded, tokenSource2.Token);
                CompileThread.Start();
            }
        }
        public void CompileBedrock(bool showForm = false)
        {
            if (isCompiling == 0)
            {
                this.showForm = showForm;
                projectVersion.Build();
                compileFile = new List<Compiler.File>();

                recallFile();
                foreach (string f in codeOrder)
                {
                    compileFile.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
                }

                compileResource = new List<Compiler.File>();
                foreach (string f in resourceOrder)
                {
                    compileResource.Add(new Compiler.File(f, resources[f].Replace('\t' + "", "")));
                }

                CompileThread = new Task(CompileBedrockThreaded, tokenSource2.Token);
                CompileThread.Start();
            }
        }

        public void CompileJavaThreaded()
        {
            try
            {
                isCompiling = 1;
                if (exporting)
                {
                    compileFiled = Compiler.compile(new CompilerCoreJava(), projectName, compileFile, compileResource,
                        DebugThread, compilerSetting, projectVersion,
                        Path.GetDirectoryName(projectPath));
                }
                else
                {
                    compileFiled = Compiler.compile(new CompilerCoreJava(), projectName, compileFile, compileResource,
                        DebugThread, compilerSetting.withoutOffuscation(), projectVersion,
                        Path.GetDirectoryName(projectPath));
                }

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
                if (exporting)
                {
                    compileFiled = Compiler.compile(new CompilerCoreBedrock(), projectName, compileFile, compileResource,
                        DebugThread, compilerSetting, projectVersion,
                        Path.GetDirectoryName(projectPath));
                }
                else
                {
                    compileFiled = Compiler.compile(new CompilerCoreBedrock(), projectName, compileFile, compileResource,
                        DebugThread, compilerSetting.withoutOffuscation(), projectVersion,
                        Path.GetDirectoryName(projectPath));
                }

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
        public void GetCallStackTraceThreaded()
        {
            try
            {
                isCompiling = 1;
                string file = Compiler.getStackCall(new CompilerCoreJava(), projectName, compileFile, compileResource,
                    DebugThread, compilerSetting.withoutOffuscation(), projectVersion,
                    Path.GetDirectoryName(projectPath));
                compileFiled = new List<Compiler.File>();
                compileFiled.Add(new Compiler.File("Call Stacks", file));
                
                try
                {
                    System.Diagnostics.Process.Start("https://dreampuf.github.io/GraphvizOnline/");
                }
                catch(Exception e)
                {
                    DebugThread(e.StackTrace, Color.Red);
                }
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

                    if (textArr[i].Contains(")"))
                        shift += textArr[i].Split(')').Length - textArr[i].Split('(').Length;

                    if (textArr[i].Contains("]"))
                        shift += textArr[i].Split(']').Length - textArr[i].Split('[').Length;

                    for (int j = 0; j < chars.Count - shift; j++)
                    {
                        text += "\t";
                    }
                    text += Compiler.smartExtract(textArr[i].Replace("\t", ""));
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
                        else if (c == '(' && !inComment && !inString)
                        {
                            chars.Push(c);
                        }
                        else if (c == '[' && !inComment && !inString)
                        {
                            chars.Push(c);
                        }
                        else if (c == '}' && !inComment && !inString && chars.Pop() != '{')
                        {
                            chars.Pop();
                        }
                        else if (c == ')' && !inComment && !inString && chars.Pop() != '(')
                        {
                            chars.Pop();
                        }
                        else if (c == ']' && !inComment && !inString && chars.Pop() != '[')
                        {
                            chars.Pop();
                        }
                    }
                    /*
                    if (reg.Match(textArr[i]).Success && !textArr[i].Contains("{"))
                    {
                        softCond++;
                        chars.Push('{');
                    }
                    else if (softCond > 0)
                    {
                        chars.Pop();
                        softCond -= 1;
                    }*/
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


        private void CodeBox_VScroll(object sender, EventArgs e)
        {
            AddLineNumbers();
        }
        
        private void CodeBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ignorNextKey)
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
                        ignorNextKey = true;
                        SendKeys.Send(temp2);
                        ignorNextKey = false;
                        break;

                    case '\t':
                        string line = CodeBox.Lines[CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart)].Replace(" ", "").Replace("\t", "");
                        if (line.StartsWith("for") && !line.Contains("("))
                        {
                            ignorNextKey = true;
                            SendKeys.Send("{(}int i = 0;i < length;i{+}{+}{)}{{}{ENTER}{ENTER}{}}{UP}");
                            ignorNextKey = false;
                        }
                        else if (line.StartsWith("dtm") && !line.Contains("("))
                        {
                            ignorNextKey = true;
                            SendKeys.Send("{BACKSPACE}{BACKSPACE}{BACKSPACE}{BACKSPACE}def ticking main{(}{)}{{}{ENTER}{ENTER}{}}{UP}");
                            ignorNextKey = false;
                        }
                        break;
                    // '}'
                    case '}':
                        ignorNextKey = true;
                        SendKeys.Send("{LEFT}{BACKSPACE}{RIGHT}");
                        ignorNextKey = false;
                        break;

                    default:
                        break;
                }
                switch (e.KeyCode)
                {
                    case Keys.End:
                        string line = CodeBox.Lines[CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart)];
                        int start = CodeBox.GetFirstCharIndexFromLine(CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart));
                        int length = CodeBox.Lines[CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart)].Length;

                        if (start + length != CodeBox.SelectionStart && e.Shift)
                        {
                            ignorNextKey = true;
                            SendKeys.Send("{LEFT}");
                            ignorNextKey = false;
                        }
                        break;

                    case Keys.Home:
                        line = CodeBox.Lines[CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart)];
                        start = CodeBox.GetFirstCharIndexFromLine(CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart));
                        int shift = 0;
                        string tmp2 = "";
                        while (line.StartsWith("\t"))
                        {
                            tmp2 += "{RIGHT}";
                            shift++;
                            line = line.Substring(1, line.Length - 1);
                        }
                        if (start + shift != CodeBox.SelectionStart)
                        {
                            ignorNextKey = true;
                            SendKeys.Send(tmp2);
                            ignorNextKey = false;
                        }
                        break;

                    case Keys.Left:
                        if (e.Control && !selfShift)
                        {
                            int i = CodeBox.SelectionStart - 1;
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
                            ignorNextKey = true;
                            SendKeys.Send(tmp2);
                            ignorNextKey = false;
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
                            ignorNextKey = true;
                            SendKeys.Send(tmp2);
                            ignorNextKey = false;
                        }
                        break;

                    default:
                        selfShift = false;
                        break;
                }
            }
        }

        public void DebugThread(object msg, Color c)
        {
            debugMSGs.Add(new DebugMessage(msg.ToString(), c));
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CompileThread != null)
                tokenSource2.Cancel();
        }

        #region Preview Button
        private void FunctionPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.functions);
            fp.Show();
        }
        private void StructPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.structs, false);
            fp.Show();
        }
        private void VariablePreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.variables);
            fp.Show();
        }
        private void EnumPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.enums);
            fp.Show();
        }
        private void BlockPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(CommandParser.names);
            fp.Show();
        }
        private void SoundPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(CommandParser.sounds);
            fp.Show();
        }
        private void PredicatePreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.predicates);
            fp.Show();
        }
        private void ClassPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.structs, true);
            fp.Show();
        }
        private void BlockTagsPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.blockTags);
            fp.Show();
        }
        private void EntityTagsPreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(Compiler.entityTags);
            fp.Show();
        }
        private void EffectGenerator_Click(object sender, EventArgs e)
        {
            EffectForm form = new EffectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text += form.Content;
            }
        }
        private void GameruleGenerator_Click(object sender, EventArgs e)
        {
            GameruleForm form = new GameruleForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text += form.Content;
            }
        }
        private void CompileJava_Click(object sender, EventArgs e)
        {
            exporting = false;
            CompileJava(true);
            UpdateCodeBox();
        }
        private void CompileBedrock_Click(object sender, EventArgs e)
        {
            exporting = false;
            CompileBedrock(true);
            UpdateCodeBox();
        }
        private void LibraryButton_Click(object sender, EventArgs e)
        {
            List<string> imported = code["import"].Split('\n').Where(x => x.StartsWith("import")).Select(x => x.Replace("import ", "")).ToList();
            LibImport libForm2 = new LibImport(compilerSetting.libraryFolder, imported);
            if (libForm2.ShowDialog() == DialogResult.OK)
            {
                string libs = libForm2.import.Count() > 0 ? libForm2.import.Select(x => $"import {x}").Aggregate((x, y) => x + "\n" + y) : "";
                code["import"] = libs;
                if (previous == "import")
                {
                    CodeBox.Text = code["import"];
                    Formatter.reformat(CodeBox, this, false);
                }
            }
        }
        #endregion
        #region Menu Strip
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

                string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
                Debug(rpdir, Color.Yellow);
                if (!Directory.Exists(rpdir))
                {
                    if (isCompiling == 0)
                    {
                        Thread t = new Thread(new ThreadStart(ExportDataPackThread));
                        t.Start();
                    }
                }
                else if (currentResourcesPack!=null || ExportRP.ShowDialog() == DialogResult.OK)
                {
                    string rpPath = currentResourcesPack==null ? ExportRP.FileName: currentResourcesPack;
                    currentResourcesPack = rpPath;
                    if (isCompiling == 0)
                    {
                        Thread t = new Thread(new ThreadStart(ExportDataPackThread));
                        t.Start();
                    }
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
        private void newDatapackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ExportSave.ShowDialog() == DialogResult.OK)
            {
                string path = ExportSave.FileName;
                
                string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
                if (!Directory.Exists(rpdir))
                {
                    currentDataPack = path;
                    if (isCompiling == 0)
                    {
                        Thread t = new Thread(new ThreadStart(ExportDataPackThread));
                        t.Start();
                    }
                }
                else if (ExportRP.ShowDialog() == DialogResult.OK)
                {
                    string rpPath = ExportRP.FileName;
                    currentDataPack = path;
                    currentResourcesPack = rpPath;
                    if (isCompiling == 0)
                    {
                        Thread t = new Thread(new ThreadStart(ExportDataPackThread));
                        t.Start();
                    }
                }
            }
        }
        private void structuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StructureImport(projectPath.Replace(Path.GetFileName(projectPath),"")).ShowDialog();
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
        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectSetting settingForm = new ProjectSetting(projectName, projectVersion, projectDescription, compilerSetting, Compiler.variables);
            settingForm.ShowDialog();

            projectName = settingForm.ProjectName;
            projectVersion = settingForm.version;
            projectDescription = settingForm.description;
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
        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            EffectForm form = new EffectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text.Insert(CodeBox.SelectionStart, form.Content);
            }
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
        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exporting = false;
            CompileJava(true);
            UpdateCodeBox();
        }
        private void resourcesPackEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(projectPath) + "/resourcespack"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/textures/block");
                Directory.CreateDirectory(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/textures/item");
                Directory.CreateDirectory(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/sounds");
                Directory.CreateDirectory(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/models");
                Directory.CreateDirectory(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/font");
            }
            ResourcesPackEditor = new ResourcesPackEditor(Path.GetDirectoryName(projectPath) + "/resourcespack");
            ResourcesPackEditor.Show();
        }
        private void getCallStackTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetCallStackTrace();
        }
        #endregion

        private void Form1_Activated(object sender, EventArgs e)
        {
            CheckFileModdification();
        }

        private void ReloadTree()
        {
            treeView1.Nodes.Clear();
            var paths = codeOrder.Select(x => "src/"+x.Replace("\\", "/")).ToList();
            BuildTree(paths, "", treeView1.Nodes);
            paths = resourceOrder.Select(x => "resources/" + x.Replace("\\", "/")).ToList();
            BuildTree(paths, "", treeView1.Nodes);
            treeView1.ExpandAll();
        }

        private void BuildTree(List<string> paths, string parent, TreeNodeCollection addInMe, int rec = 0)
        {
            if (rec > 100)
            {
                paths.ForEach(x => addInMe.Add(x));
            }
            else
            {
                paths.Where(x => x.Contains("/"))
                     .Select(x => Compiler.smartSplit(x, '/', 1))
                     .GroupBy(x => x[0]).ToList()
                     .ForEach(x =>
                     {
                         TreeNode curNode = addInMe.Add(x.Key);
                         BuildTree(x.Select(z => z.Last())
                                                 .ToList(),
                                                 parent, curNode.Nodes, rec + 1);

                     });
                paths.Where(x => !x.Contains("/")).ToList().ForEach(x => addInMe.Add(x));
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (ignorNextListboxUpdate)
            {
                ignorNextListboxUpdate = false;
            }
            else
            {
                string fullPath = treeView1.SelectedNode.FullPath;
                if (fullPath.StartsWith("src") && fullPath.Length > 4)
                {
                    string path = fullPath.Substring(4, fullPath.Length - 4);
                    resourceSelected = false;
                    if (code.ContainsKey(path))
                    {
                        ReloadCodeBoxFileCode(path);
                    }
                }
                if (fullPath.StartsWith("resources") && fullPath.Length > 10)
                {
                    string path = fullPath.Substring(10, fullPath.Length - 10);
                    resourceSelected = true;
                    if (resources.ContainsKey(path))
                    {
                        ReloadCodeBoxFileRes(path);
                    }
                }
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            ChangeCompileOrder();
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
    }
}
