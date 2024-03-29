﻿using BluePhoenix;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static JSharp.Compiler.Function;

namespace JSharp
{
    public partial class Form1 : Form
    {
        public Dictionary<string, FormFile> code = new Dictionary<string, FormFile>();
        public Dictionary<string, FormFile> resources = new Dictionary<string, FormFile>();
        public Dictionary<string, DateTime> rpmoddate = new Dictionary<string, DateTime>();
        public Dictionary<string, Dictionary<int, int>> collaspe = new Dictionary<string, Dictionary<int, int>>();
        public Dictionary<string, Dictionary<string, TagsList>> TagsList = new Dictionary<string, Dictionary<string, TagsList>>();
        public Dictionary<string, Dictionary<string, TagsList>> MCTagsList = new Dictionary<string, Dictionary<string, TagsList>>();
        
        public Dictionary<string, string> structures = new Dictionary<string, string>();
        public List<string> codeOrder = new List<string>();
        public List<string> resourceOrder = new List<string>();
        public ProjectVersion projectVersion = new ProjectVersion();
        public Compiler.CompilerSetting compilerSetting = new Compiler.CompilerSetting();

        private CompilerCore core;
        public string projectDescription;

        private string previous = "load";
        private string projectName = "default";
        private bool settingChanged = false;
        private bool exportRP = false;
        private string currentDataPack;
        private string currentResourcesPack;
        public string projectPath;
        public bool ignorNextListboxUpdate = false;
        public bool ignorNextKey = false;
        private List<string> openedFullPath = new List<string>();
        public int isCompiling = 0;

        private Thread CompileThread;
        private CustomPasteReplace customPaste;

        public List<Compiler.File> compileFile;
        public List<Compiler.File> compileResource;
        public List<Compiler.File> compileFiled;
        private List<DebugMessage> debugMSGs = new List<DebugMessage>();
        private List<DebugMessage> debugMSGsShowned = new List<DebugMessage>();
        private int lastSeen = -1;
        private bool showForm;

        private List<string> PreviousText = new List<string>();
        private bool ignoreMod = false;
        private bool noReformat = false;
        private bool exporting;
        private string resourceSelected = "src";
        private int index = 0;

        #region image
        private Image minusPath;
        private Image plusPath;
        private Image filePath;
        private Image fileCSVPath;
        private Image fileINIPath;
        private Image fileTXTPath;
        private Image fileImagePath;
        #endregion image

        private bool showError = true;
        private bool showWarning = true;
        private bool showInfo = false;
        private bool exportNew = false;

        private bool debugOffuscate = false;

        public Form1(string project = null)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            InitializeComponent();
            CommandParser.loadDict();
            Formatter.loadDict();

            if (project != null)
            {
                OpenFile(project);
            }

            minusPath = Image.FromFile(path + "assets/folder_open.png");
            plusPath = Image.FromFile(path + "assets/folder_closed.png");
            filePath = Image.FromFile(path + "assets/file.png");
            fileCSVPath = Image.FromFile(path + "assets/file_csv.png");
            fileINIPath = Image.FromFile(path + "assets/file_ini.png");
            fileTXTPath = Image.FromFile(path + "assets/file_txt.png");
            fileImagePath = Image.FromFile(path + "assets/file_image.png");
        }

        private void recallFile()
        {
            try
            {
                if (resourceSelected == "res")
                {
                    resources[previous].SetContent(CodeBox.Text);
                }
                else if (resourceSelected == "src")
                {
                    code[previous].SetContent(CodeBox.Text);
                }
                else if (resourceSelected == "respack")
                {
                    string dir = Path.GetDirectoryName(projectPath) + "/resourcespack/";
                    File.WriteAllText(dir + previous, CodeBox.Text);
                }
            }
            catch { }
        }
        public void ReloadCodeBoxFileCode(string file)
        {
            noReformat = true;

            recallFile();
            resourceSelected = "src";

            PreviousText.Clear();

            index = 0;
            if (code.ContainsKey(file))
            {
                CodeBox.Text = code[file].content;
                CodeBox.ClearUndo();
            }
            else
            {
                CodeBox.Text = "";
                CodeBox.ClearUndo();
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
            resourceSelected = "res";

            PreviousText.Clear();

            index = 0;
            if (resources.ContainsKey(file))
            {
                CodeBox.Text = resources[file].content;
                CodeBox.ClearUndo();
            }
            else
            {
                CodeBox.Text = "";
                CodeBox.ClearUndo();
            }
            PreviousText.Add(CodeBox.Text);
            previous = file;
            noReformat = false;
            UpdateCodeBox();
        }
        public void ReloadCodeBoxFileResPack(string file)
        {
            string dir = Path.GetDirectoryName(projectPath) + "/resourcespack/";

            noReformat = true;

            recallFile();
            resourceSelected = "respack";

            PreviousText.Clear();

            index = 0;

            CodeBox.Text = File.ReadAllText(dir + file);
            CodeBox.ClearUndo();

            PreviousText.Add(CodeBox.Text);
            previous = file;
            noReformat = false;
            UpdateCodeBox();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int nb = 0;
            while (lastSeen < debugMSGs.Count - 1 && nb < 100)
            {
                lastSeen++;
                nb++;
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

        #region Form
        private void Form1_Load(object sender, EventArgs e)
        {
            if (projectPath == null)
                NewProject();
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (!ignorNextListboxUpdate)
            {
                CheckFileModdification();
                ReloadTree();
            }
            else
            {
                ignorNextListboxUpdate = false;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !TryClose();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CompileThread != null)
                CompileThread.Abort();
        }
        #endregion

        public void Debug(object text, Color color)
        {
            debugMSGsShowned.Add(new DebugMessage(text.ToString(), color));
            DebugDisplay(text.ToString(), color);
        }
        public void DebugDisplay(string text, Color color)
        {
            bool show = true;
            if (color == Color.Red && !showError) show = false;
            if (color == Color.Yellow && !showWarning) show = false;
            if (color != Color.Red && color != Color.Yellow && color != Color.Aqua && !showInfo) show = false;

            if (show)
            {
                ErrorBox.SelectionStart = ErrorBox.Text.Length;
                ErrorBox.SelectionColor = color;
                ErrorBox.AppendText("[" + DateTime.Now.ToString() + "] " + text + "\n");
            }
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
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
            string dirLib = Path.GetDirectoryName(projectPath) + "/lib";
            
            if (!Directory.Exists(dirLib) && project.compilationSetting.isLibrary)
                Directory.CreateDirectory(dirLib);

            foreach (string file in codeOrder)
            {
                if (project.compilationSetting.isLibrary)
                {
                    if (code[file].wasModdified || settingChanged)
                    {
                        if (!file.Contains('.'))
                        {
                            SafeWriteFile(dir + file + ".bps", code[file].content);
                            code[file].date = DateTime.Now.AddSeconds(5);
                            code[file].Save();
                        }
                        else
                        {
                            SafeWriteFile(dir + file, code[file].content);
                            code[file].date = DateTime.Now.AddSeconds(5);
                            code[file].Save();
                        }
                    }
                    project.compileOrder.Add(file);
                }
                else
                {
                    lst.Add(new ProjectSave.FileSave(file, code[file].content, i));
                    code[file].Save();
                }
            }
            foreach (string file in resourceOrder)
            {
                if (project.compilationSetting.isLibrary)
                {
                    if (resources[file].wasModdified || settingChanged)
                    {
                        SafeWriteFile(dirRes + file, resources[file].content);
                        resources[file].date = DateTime.Now.AddSeconds(5);
                        resources[file].Save();
                    }
                }
                else
                {
                    lstRes.Add(new ProjectSave.FileSave(file, resources[file].content, i));
                    resources[file].Save();
                }
            }
            project.files = lst.ToArray();
            project.resources = lstRes.ToArray();
            project.description = projectDescription;

            File.WriteAllText(projectPath, JsonConvert.SerializeObject(project));
            settingChanged = false;
        }
        public void OpenFile(string name)
        {
            try
            {
                if (TryClose())
                {
                    projectPath = name;
                    Open(name, File.ReadAllText(name));
                    Text = projectName + " - BluePhoenix";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Open(string name, string data)
        {
            Reset();

            ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);
            codeOrder.Clear();
            ResetTabBar();
            
            code = new Dictionary<string, FormFile>();
            resources = new Dictionary<string, FormFile>();
            rpmoddate = new Dictionary<string, DateTime>();
            TagsList = project.TagsList;
            MCTagsList = project.mcTagsList;
            projectVersion = project.version;

            projectName = project.projectName;
            currentDataPack = project.datapackDirectory;
            currentResourcesPack = project.resourcesPackDirectory;
            project.compilationSetting.isLibrary = project.isLibrary;
            compilerSetting = project.compilationSetting;

            if (compilerSetting.libraryFolder.Count == 0)
            {
                compilerSetting.libraryFolder.Add("./lib/1_16_5/");
                compilerSetting.libraryFolder.Add("./lib/shared/");
            }
            previous = "$$$$$$$$$";

            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
            foreach (var file in project.files)
            {
                ignorNextListboxUpdate = true;
                try
                {
                    code.Add(file.name, new FormFile(file.name, file.content.Replace("\r", ""), false));
                    code[file.name].date = DateTime.Now;
                    project.compilationSetting.isLibrary = false;
                }
                catch
                {
                    Debug("Duplicated " + file.name + "-" + file.content + "////" + code[file.name], Color.Red);
                }

                codeOrder.Add(file.name);
            }

            if (project.resources != null)
            {
                foreach (var file in project.resources)
                {
                    ignorNextListboxUpdate = true;
                    try
                    {
                        resources.Add(file.name, new FormFile(file.name, file.content.Replace("\r", ""), false));
                        resources[file.name].date = DateTime.Now;
                    }
                    catch
                    {
                        Debug("Duplicated " + file.name + "-" + file.content + "////" + resources[file.name], Color.Red);
                    }

                    resourceOrder.Add(file.name);
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
                            code.Add(file, new FormFile(file, File.ReadAllText(dir + file + ".bps").Replace("\r", ""), false));
                        else
                            code.Add(file, new FormFile(file, File.ReadAllText(dir + file).Replace("\r", ""), false));
                        code[file].date = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        Debug("Exception while reading " + e.ToString(), Color.Red);
                    }

                    codeOrder.Add(file);
                }
            }
            FetchFilesInDirectory();

            ReloadTree();

            Debug("Project Loaded: " + projectPath, Color.Aqua);
            noReformat = false;
            exporting = false;
            Compile();
            UpdateCodeBox();
            ignorNextListboxUpdate = false;
            projectDescription = project.description;

            SelectFullPath("src/" + codeOrder[0]);
        }
        public bool TryClose()
        {
            if (code.Values.Any(x => x.wasModdified) || resources.Values.Any(x => x.wasModdified) || settingChanged)
            {
                string lst = code.Values.Concat(resources.Values).Where(x => x.wasModdified).Select(x => x.name).Aggregate("", (x, y) => (x==""?y:x + ", " + y));
                DialogResult res = MessageBox.Show("You have unsaved work. Do you want to save?\n"+ lst, "Are you sure?", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                {
                    return false;
                }
                else if (res == DialogResult.Yes)
                {
                    return ForceSave(true);
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        private void FetchFilesInDirectory()
        {
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                {
                    string fname;
                    if (file.EndsWith(".bps"))
                        fname = file.Substring(dir.Length, file.Length - dir.Length - Path.GetExtension(file).Length);
                    else
                        fname = file.Substring(dir.Length, file.Length - dir.Length);

                    if (!code.ContainsKey(fname.ToLower()) && fname != "desktop.ini")
                    {
                        try
                        {
                            code.Add(fname.ToLower(), new FormFile(fname, File.ReadAllText(file), false));
                            code[fname].date = DateTime.Now;
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

                    if (!resources.ContainsKey(fname.ToLower()))
                    {
                        try
                        {
                            resources.Add(fname.ToLower(), new FormFile(fname.ToLower(), File.ReadAllText(file), false));
                            resources[fname].date = DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            Debug("Exception while reading " + e.ToString(), Color.Red);
                        }

                        resourceOrder.Add(fname.ToLower());
                    }
                }
            }
        }
        public void UpdateRPTimes()
        {
            rpmoddate.Clear();
            string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
            if (Directory.Exists(rpdir))
            {
                Directory.EnumerateFiles(rpdir, "*.*", SearchOption.AllDirectories).ToList().ForEach(x => rpmoddate.Add(x, File.GetLastWriteTime(x)));
            }
        }
        public bool WasRPChanged()
        {
            string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
            if (Directory.Exists(rpdir))
            {
                bool res = rpmoddate.Any(x => !File.Exists(x.Key) || File.GetLastWriteTime(x.Key) > x.Value) ||
                        Directory.EnumerateFiles(rpdir, "*.*", SearchOption.AllDirectories).Any(x => !rpmoddate.ContainsKey(x))
                        || !File.Exists(currentResourcesPack);
                return res;
            }
            else
                return false;
        }
        public bool ForceSave(bool save = false)
        {
            if (projectPath == "" || projectPath == null)
            {
                if (ProjectSave.ShowDialog() == DialogResult.OK)
                {
                    projectPath = ProjectSave.FileName;
                }
                else
                    return false;
                
                Save(projectPath);
                UpdateProjectList();
                Debug("Project save as " + projectPath, Color.Aqua);
            }
            if (projectPath != "" && projectPath != null)
            {
                if (save)
                {
                    Save(projectPath);
                    UpdateProjectList();
                }
                return true;
            }
            return false;
        }
        public void GenerateResourcesPackFolder()
        {
            if (ForceSave())
            {
                if (!Directory.Exists(Path.GetDirectoryName(projectPath) + "/resourcespack"))
                {
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/textures/block/generator.bps","");
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/textures/item/generator.bps","");
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/sounds/generator.bps", "");
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/models/item/generator.bps","");
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/models/block/generator.bps", "");
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/font/generator.bps", "");
                    SafeWriteFile(Path.GetDirectoryName(projectPath) + "/resourcespack/assets/minecraft/blockstates/generator.bps", "");


                    string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
                    SafeCopy(path + "/assets/pack.png", Path.GetDirectoryName(projectPath) + "/resourcespack/pack.png");

                    FetchFilesInDirectory();
                    ReloadTree();
                }
            }
        }
        public void CheckFileModdification()
        {
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
            UpdateFile.Result prev = UpdateFile.Result.No;
            List<string> keys = new List<string>();
            keys.AddRange(code.Keys);
            foreach (string key in keys)
            {
                if (File.Exists(dir + key + ".bps") && code[key].date < File.GetLastWriteTime(dir + key + ".bps"))
                {
                    if (prev == UpdateFile.Result.NoForAll)
                    {
                        code[key].date = File.GetLastWriteTime(dir + key + ".bps");
                    }
                    else if (prev == UpdateFile.Result.YesForAll)
                    {
                        code[key].SetContent(File.ReadAllText(dir + key + ".bps"));
                        code[key].date = File.GetLastWriteTime(dir + key + ".bps");
                        if (previous == key)
                        {
                            CodeBox.Text = code[key].content;
                            CodeBox.ClearUndo();
                        }
                    }
                    else
                    {
                        var res = UpdateFile.ShowDialog("The File \"" + key + "\" has been moddify. Do you want to reload it?");
                        prev = res;
                        if (res == UpdateFile.Result.YesForAll || res == UpdateFile.Result.Yes)
                        {
                            code[key].SetContent(File.ReadAllText(dir + key + ".bps"));
                            code[key].date = File.GetLastWriteTime(dir + key + ".bps");
                            if (previous == key)
                            {
                                CodeBox.Text = code[key].content;
                                CodeBox.ClearUndo();
                            }
                        }
                        else
                        {
                            code[key].date = File.GetLastWriteTime(dir + key + ".bps");
                        }
                    }
                }
            }
            keys = new List<string>();
            keys.AddRange(resources.Keys);
            foreach (string key in keys)
            {
                if (File.Exists(dir + key) && resources[key].date < File.GetLastWriteTime(dir + key))
                {
                    if (prev == UpdateFile.Result.NoForAll)
                    {
                        resources[key].date = File.GetLastWriteTime(dir + key);
                    }
                    else if (prev == UpdateFile.Result.YesForAll)
                    {
                        resources[key].SetContent(File.ReadAllText(dirRes + key));
                        resources[key].date = File.GetLastWriteTime(dir + key);
                        if (previous == key)
                        {
                            CodeBox.Text = resources[key].content;
                            CodeBox.ClearUndo();
                        }
                    }
                    else
                    {
                        var res = UpdateFile.ShowDialog("The Resource File \"" + key + "\" has been moddify. Do you want to reload it?");
                        prev = res;
                        if (res == UpdateFile.Result.YesForAll || res == UpdateFile.Result.Yes)
                        {
                            resources[key].SetContent(File.ReadAllText(dir + key));
                            resources[key].date = File.GetLastWriteTime(dirRes + key);
                            if (previous == key)
                            {
                                CodeBox.Text = resources[key].content;
                                CodeBox.ClearUndo();
                            }
                        }
                        else
                        {
                            resources[key].date = File.GetLastWriteTime(dir + key);
                        }
                    }
                }
            }

            FetchFilesInDirectory();
        }
        public void UpdateProjectList()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";

            List<string> lst = new List<string>();
            if (File.Exists(path + "project.old"))
            {
                foreach (string s in File.ReadAllLines(path + "project.old")) lst.Add(s);
            }

            if (lst.Contains(projectPath))
                lst.Remove(projectPath);

            lst.Insert(0, projectPath);

            if (lst.Count > 20)
            {
                lst.RemoveAt(20);
            }

            File.WriteAllLines(path + "project.old", lst.ToArray());
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
        public void NewFile(string value = "", JSharp.NewFile.Type? type = null)
        {
            NewFile form = new NewFile();
            ignorNextListboxUpdate = true;
            if (value != "" || (form.ShowDialog() == DialogResult.OK && !code.Keys.Contains(form.filename)))
            {
                if (value != "")
                {
                    form.filename = value;
                }
                if (type != null)
                {
                    form.type = (JSharp.NewFile.Type)type;
                }
                string dir = treeView1.SelectedNode != null ? treeView1.SelectedNode.FullPath : "src/";
                if (treeView1.SelectedNode != null && treeView1.SelectedNode.Checked && dir.Contains("/"))
                {
                    dir = dir.Substring(0, dir.LastIndexOf("/"));
                }
                string grp = dir.Contains("/") ? dir.Substring(0, dir.IndexOf("/")) : dir;
                dir = dir.Contains("/") ? dir.Substring(dir.IndexOf("/") + 1, dir.Length - dir.IndexOf("/") - 1) : "";
                string path = dir == "" ? "" : dir + "/";

                if (grp == "structures")
                {
                    if (form.type == JSharp.NewFile.Type.FOLDER)
                    {
                        if (ForceSave())
                        {
                            string gloDir = Path.GetDirectoryName(projectPath) + "/structures/";
                            Directory.CreateDirectory(gloDir + dir + "/" + form.filename);
                        }
                    }
                }
                else if (grp == "resources" || form.type == JSharp.NewFile.Type.RESOURCE)
                {
                    if (form.type == JSharp.NewFile.Type.FOLDER)
                    {
                        if (ForceSave())
                        {
                            string gloDir = Path.GetDirectoryName(projectPath) + "/resources/";
                            Directory.CreateDirectory(gloDir + dir + "/" + form.filename);
                        }
                    }
                    else
                    {
                        resourceOrder.Add(path + form.filename);
                        resources.Add(path + form.filename, new FormFile(path+form.filename, ""));
                    }
                }
                else if (grp == "resourcespack")
                {
                    if (form.type == JSharp.NewFile.Type.FOLDER)
                    {
                        if (ForceSave())
                        {
                            string gloDir = Path.GetDirectoryName(projectPath) + "/resourcespack/";
                            Directory.CreateDirectory(gloDir + dir + "/" + form.filename);
                        }
                    }
                    else
                    {
                        string gloDir = Path.GetDirectoryName(projectPath) + "/resourcespack/";
                        File.WriteAllText(gloDir + dir + "/" + form.filename, "");
                    }
                }
                else if (grp == "src")
                {
                    if (form.type == JSharp.NewFile.Type.FOLDER)
                    {
                        if (ForceSave())
                        {
                            string gloDir = Path.GetDirectoryName(projectPath) + "/scripts/";
                            Directory.CreateDirectory(gloDir + dir + "/" + form.filename);
                        }
                    }
                    else if (form.type == JSharp.NewFile.Type.EXTERNAL)
                    {
                        if (DatapackOpen.ShowDialog() == DialogResult.OK)
                        {
                            codeOrder.Add(path + form.filename);
                            code.Add(path + form.filename.ToLower(), new FormFile(path+form.filename.ToLower(), GenerateDatapackLink(DatapackOpen.FileName)));
                        }
                    }
                    else
                    {
                        if (form.filename != "import")
                            codeOrder.Add(path + form.filename);
                        else
                            codeOrder.Insert(0, path + form.filename);
                        if (form.type == JSharp.NewFile.Type.STRUCTURE)
                        {
                            code.Add(path + form.filename, new FormFile(path + form.filename, "package " + (path + form.filename).Replace("/", ".") + "\n\nstruct " + (path + form.filename).Replace("/", ".") + "{\n\tdef __init__(){\n\n\t}\n}"));
                        }
                        else if (form.type == JSharp.NewFile.Type.SUBPROGRAMME)
                        {
                            code.Add(path + form.filename, new FormFile(path + form.filename, "package " + (path + form.filename).Replace("/", ".") + "\n\nstatic class "+ (path + form.filename) + " extends process{\n\tdef main(){\n\t\t\n\t}\n\tdef onStart(){\n\t\t\n\t}\n\tdef onStop(){\n\t\t\n\t}\n}"));
                        }
                        else
                        {
                            code.Add(path + form.filename, new FormFile(path + form.filename, "package " + (path + form.filename).Replace("/", ".")));
                        }
                    }
                }
                ReloadTree();
            }
        }
        public void NewProject(string path = null)
        {
            NewProjectForm form = new NewProjectForm();
            var res = DialogResult.None;
            if (path == null)
                res = form.ShowDialog();
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                Reset();
            }
            if (res == DialogResult.OK || path != null)
            {
                projectName = path==null?form.ProjectName.ToLower().Replace(" ",""): path+"/project.tbms";
                compilerSetting = new Compiler.CompilerSetting();
                compilerSetting.packformat = 10;
                compilerSetting.rppackformat = 9;
                compilerSetting.MCVersion = form.MCVersion;
                compilerSetting.libraryFolder.Add("./lib/1_19/");
                compilerSetting.libraryFolder.Add("./lib/1_18/");
                compilerSetting.libraryFolder.Add("./lib/1_17/");
                compilerSetting.libraryFolder.Add("./lib/1_16_5/");
                compilerSetting.libraryFolder.Add("./lib/shared/");
                compilerSetting.randomLambdaID = true;

                Text = projectName + " - TBMScript";

                code.Clear();
                LibImport libForm2 = new LibImport(compilerSetting.libraryFolder);
                libForm2.ShowDialog();
                string libs = libForm2.import.Count() > 0 ? libForm2.import.Select(x => $"import {x}").Aggregate((x, y) => x + "\n" + y) : "";
                code.Add("import", new FormFile("import", libs));
                code.Add(projectName.ToLower(), new FormFile(projectName.ToLower(), "package " + projectName.ToLower() + "\n"));
                SelectFullPath("src/" + projectName.ToLower());

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
                    MCTagsList = new Dictionary<string, Dictionary<string, TagsList>>();
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

        private void Reset()
        {
            ResetTabBar();
            codeOrder.Clear();
            code.Clear();
            resources.Clear();
            resourceOrder.Clear();
            projectVersion = new ProjectVersion();
            projectDescription = "";
            compilerSetting = new Compiler.CompilerSetting();
            structures.Clear();
            MCTagsList.Clear();
            TagsList.Clear();
            CodeBox.Text = "";
            CodeBox.ClearUndo();
        }

        public void UpdateCodeBox()
        {

        }
        public static void SafeCopy(string src, string dest)
        {
            try
            {
                string filePath = Path.GetDirectoryName(dest);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                File.Copy(src, dest, true);
            }
            catch(Exception e)
            {
                throw new Exception($"Failed to copy: {src} -> {dest} res:{e}");
            }
        }
        public int SafeWriteFile(string fileName, string content)
        {
            var filedir = fileName.Replace("\\", "/");
            string filePath = filedir.Substring(0, filedir.LastIndexOf('/'));
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            try
            {
                File.WriteAllText(fileName, content);
                return 0;
            }
            catch (System.IO.PathTooLongException e)
            {
                throw new Exception($"Path Too long: {fileName}. Try using exporting as a zip.");
                return 1;
            }
            catch (Exception e)
            {
                Debug(e, Color.Red);
                return -1;
            }
        }

        public void ChangeCompileOrder()
        {
            CompileOrder form = new CompileOrder(codeOrder, codeOrder.Contains("import") ? 1 : 0);
            if (form.ShowDialog() == DialogResult.OK)
            {
                ignorNextListboxUpdate = true;
                codeOrder.Clear();
                codeOrder.AddRange(form.Content);
                Debug("Compile Order Changed", Color.Aqua);
            }
        }
        public void Compile(bool showForm = false)
        {
            if (isCompiling > 0)
            {
                CompileThread.Abort();
                isCompiling = 0;
            }
            if (isCompiling == 0)
            {
                this.showForm = showForm;
                projectVersion.Build();
                GetCompileFiles();
                if (CompileThread != null && CompileThread.IsAlive)
                    CompileThread.Abort();

                CompileThread = new Thread(CompileThreaded);
                CompileThread.Priority = ThreadPriority.Highest;
                CompileThread.Start();
            }
        }

        private void GetCompileFiles()
        {
            compileFile = new List<Compiler.File>();

            recallFile();

            foreach (string f in codeOrder)
            {
                compileFile.Add(new Compiler.File(f, code[f].content.Replace('\t' + "", "")));
            }

            bool rp = WasRPChanged();

            string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
            if (currentDataPack != null && Directory.Exists(Path.GetDirectoryName(currentDataPack) + "/"))
            {
                foreach (var file in Directory.GetFiles(Path.GetDirectoryName(currentDataPack) + "/", "*.bpi", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file) != Path.GetFileName(currentDataPack) + ".bpi")
                    {
                        var f = new Compiler.File("outside", File.ReadAllText(file).Replace('\t' + "", ""));
                        compileFile.Insert(0, f);
                    }
                }
            }
            if (Directory.Exists(rpdir) && rp)
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
                compileResource.Add(new Compiler.File(f, resources[f].content.Replace('\t' + "", "")));
            }
        }

        public void GetCallStackTrace()
        {
            if (isCompiling == 0)
            {
                this.showForm = true;
                projectVersion.Build();

                GetCompileFiles();

                if (CompileThread != null && CompileThread.IsAlive)
                    CompileThread.Abort();
                CompileThread = new Thread(GetCallStackTraceThreaded);
                CompileThread.Start();
            }
        }

        public void CompileThreaded()
        {
            try
            {
                isCompiling = 1;
                
                if (compilerSetting.CompilerCoreName == "java") { core = new CompilerCoreJava(); }
                else if (compilerSetting.CompilerCoreName == "bedrock") { core = new CompilerCoreBedrock(); }
                else { throw new Exception("Unknown Compiler Core"); }
                if (exporting)
                {
                    compileFiled = Compiler.compile(core, projectName, compileFile, compileResource,
                        DebugThread, compilerSetting, projectVersion,
                        Path.GetDirectoryName(projectPath));
                }
                else
                {
                    compileFiled = Compiler.compile(core, projectName, compileFile, compileResource,
                        DebugThread, debugOffuscate ? compilerSetting : compilerSetting.withoutOffuscation(), projectVersion,
                        Path.GetDirectoryName(projectPath));
                }
                Formatter.getAutoComplete(autocompleteMenu1, previous, CodeBox.Text);
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
                ShowGraphviz(file);

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
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
                CodeBox.DescriptionFile = "";
                CodeBox.DescriptionFile = path + "formating.xml";
                Formatter.getAutoComplete(autocompleteMenu1, previous, CodeBox.Text);

                Regex reg = new Regex(@"(?s)^\s*((if)|(for)|(while))\b");
                Stack<char> chars = new Stack<char>();
                string[] textArr = CodeBox.Text.Split('\n');
                string text = "";

                for (int i = 0; i < textArr.Length; i++)
                {
                    int shift = 0;
                    if (Compiler.smartExtract(textArr[i]).StartsWith("}"))
                        shift += textArr[i].Split('}').Length - textArr[i].Split('{').Length;
                    /*
                    if (textArr[i].Contains("}"))
                        shift += textArr[i].Split('}').Length - textArr[i].Split('{').Length;

                    if (textArr[i].Contains(")"))
                        shift += textArr[i].Split(')').Length - textArr[i].Split('(').Length;

                    if (textArr[i].Contains("]"))
                        shift += textArr[i].Split(']').Length - textArr[i].Split('[').Length;
                    */
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
                CodeBox.ClearUndo();
            }
            catch (Exception e)
            {
                Debug(e, Color.Red);
            }
        }
        public string ProjectFolder()
        {
            return projectPath.Replace(Path.GetFileName(projectPath), "");
        }
        private void ReShowError()
        {
            ErrorBox.Text = "";
            debugMSGsShowned.ForEach(x => DebugDisplay(x.msg, x.color));
        }
        private void ShowGraphviz(string data)
        {
            try
            {
                string ProjectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
                File.WriteAllText(ProjectPath + "input.dot", data);
                var p = System.Diagnostics.Process.Start($"dot", $"-Tsvg {ProjectPath}input.dot -o {ProjectPath}tmp.svg");
                p.WaitForExit();
                System.Diagnostics.Process.Start($"{ProjectPath}tmp.svg");
                File.Delete(ProjectPath + "input.dot");
                showForm = false;
            }
            catch(Exception e)
            {
                showForm = true;
                compileFiled = new List<Compiler.File>();
                compileFiled.Add(new Compiler.File("Call Stacks", data));
                System.Diagnostics.Process.Start("https://dreampuf.github.io/GraphvizOnline/");
                DebugThread(e, Color.Red);
            }
        }

        public void DebugThread(object msg, Color c)
        {
            debugMSGs.Add(new DebugMessage(msg.ToString(), c));
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
                recallFile();

                string ProjectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
                string writePath = compilerSetting.ExportAsZip ? "C:/bpep/" : path;

                if (File.Exists(writePath + "/pack.mcmeta"))
                {
                    Directory.Delete(writePath, true);
                }
                
                exportRP = WasRPChanged() || exportNew;
                exportNew = false;
                projectVersion.Build();
                
                ExportFiles(writePath);
                ExportTags(writePath);
                ExportStructures(writePath);
                ExportReadMe(writePath);
                SafeCopy(ProjectPath + "/assets/pack.png", writePath + "/pack.png");
                SafeWriteFile(writePath + "/pack.mcmeta",
                            JsonConvert.SerializeObject(new DataPackMeta(projectName + " - " + projectDescription,
                            compilerSetting.packformat)));

                if (compilerSetting.exportInterface)
                {
                    ExportInterface(path + ".bpi");
                }
                if (compilerSetting.ExportAsZip)
                {
                    if (!path.EndsWith(".zip")) path += ".zip";
                    if (File.Exists(path)) { File.Delete(path); }
                    ZipFile.CreateFromDirectory(writePath, path);
                    Directory.Delete(writePath, true);
                }
                Formatter.getAutoComplete(autocompleteMenu1, previous, CodeBox.Text);

                DebugThread("Datapack successfully exported!", Color.Aqua);
                ExportResourcePack(rpPath);

                if (Directory.Exists(ProjectPath + "imported_dp")) Directory.Delete(ProjectPath + "imported_dp", true);
                if (Directory.Exists(ProjectPath + "imported_rp")) Directory.Delete(ProjectPath + "imported_rp", true);
                if (Directory.Exists(ProjectPath + "unzip")) Directory.Delete(ProjectPath + "unzip", true);
            }
            catch (Exception er)
            {
                DebugThread(er, Color.Red);
                string ProjectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
                if (Directory.Exists(ProjectPath + "imported_dp")) Directory.Delete(ProjectPath + "imported_dp", true);
                if (Directory.Exists(ProjectPath + "imported_rp")) Directory.Delete(ProjectPath + "imported_rp", true);
                if (Directory.Exists(ProjectPath + "unzip")) Directory.Delete(ProjectPath + "unzip", true);
            }
        }
        public void ExportInterface(string path)
        {
            try
            {
                core = new CompilerCoreJava();
                string GetName(Compiler.Function f)
                {
                    return f.gameName.Replace(":", ".").Replace("/", ".");
                }
                string GetArg(Compiler.Function f)
                {
                    string str = "";
                    for (int i = 0; i < f.args.Count; i++)
                    {
                        str += i == 0 ? $"int ${i}" : $", int ${i}";
                    }
                    return str;
                }
                string GetContent(Compiler.Function f)
                {
                    string str = "";
                    for (int i = 0; i < f.args.Count; i++)
                    {
                        str += $"/scoreboard players operation {f.args[i].scoreboard()} = ${i}.scoreboard\n";
                    }
                    str += $"/function {f.gameName}\n";
                    return str;
                }

                var functions = Compiler.functions
                    .SelectMany(x => x.Value)
                    .Where(f => !f.isLambda && f.isPublic && !f.lazy)
                    .Select(f => $"def lazy {GetName(f)}({GetArg(f)}){{\n{GetContent(f)}}}\n").ToList();
                if (functions.Count != 0)
                {
                    var text = functions.Aggregate((x, y) => x + y);

                    SafeWriteFile(path, $"package .\n{functions}");
                }
            }
            catch(Exception e)
            {

            }
        }
        public void ExportFiles(string path)
        {
            string ProjectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";

            GetCompileFiles();

            CompilerCore core;
            if (compilerSetting.CompilerCoreName == "java") { core = new CompilerCoreJava(); }
            else if (compilerSetting.CompilerCoreName == "bedrock") { core = new CompilerCoreBedrock(); }
            else { throw new Exception("Unknown Compiler Core"); }

            List<Compiler.File> cFiles = Compiler.compile(core, projectName, compileFile, compileResource,
                                        DebugThread, compilerSetting, projectVersion,
                                        Path.GetDirectoryName(projectPath));

            foreach (Compiler.File f in cFiles)
            {
                string fileName;
                if (f.type == "json" && f.name.Contains("json"))
                    fileName = path + core.GetJsonPath(projectName, f.name);
                else if (f.type == "json" && !f.name.Contains("json"))
                    fileName = path + core.GetJsonPath(projectName, f.name + ".json");
                else
                    fileName = path + core.GetFunctionPath(projectName, f.name);

                try
                {
                    SafeWriteFile(fileName, f.content);
                }
                catch (Exception e)
                {
                    throw new Exception(fileName + " " + e.ToString());
                }
            }

            if (Directory.Exists(rpdir) && exportRP)
            {
                string rpPath = "C:/bprp/";
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
                if (Directory.Exists(ProjectPath + "imported_rp/"))
                {
                    foreach (var file in Directory.GetFiles(ProjectPath + "imported_rp/", "*.*", SearchOption.AllDirectories))
                    {
                        if (!file.EndsWith(".bps"))
                        {
                            string fileName = file.Replace("\\", "/").Replace(ProjectPath.Replace("\\", "/") + "imported_rp/", rpPath);
                            SafeCopy(file, fileName);
                        }
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
                rpmoddate.Clear();
                UpdateRPTimes();
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
            if (compilerSetting.tagsFolder)
            {
                foreach (string key in Compiler.blockTags.Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/blocks/" + key.Substring(key.IndexOf(".") + 1, key.Length - key.IndexOf(".") - 1).Replace(".", "/") + ".json",
                            JsonConvert.SerializeObject(Compiler.blockTags[key]));
                }
                foreach (string key in Compiler.entityTags.Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/entity_types/" + key.Substring(key.IndexOf(".") + 1, key.Length - key.IndexOf(".") - 1).Replace(".", "/") + ".json",
                            JsonConvert.SerializeObject(Compiler.entityTags[key]));
                }
                foreach (string key in Compiler.itemTags.Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/items/" + key.Substring(key.IndexOf(".") + 1, key.Length - key.IndexOf(".") - 1).Replace(".", "/") + ".json",
                            JsonConvert.SerializeObject(Compiler.itemTags[key]));
                }
            }
            else
            {
                foreach (string key in Compiler.blockTags.Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/blocks/" + key.Replace(".", "/") + ".json",
                            JsonConvert.SerializeObject(Compiler.blockTags[key]));
                }
                foreach (string key in Compiler.entityTags.Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/entity_types/" + key.Replace(".", "/") + ".json",
                            JsonConvert.SerializeObject(Compiler.entityTags[key]));
                }
                foreach (string key in Compiler.itemTags.Keys)
                {
                    SafeWriteFile(path + "/data/" + projectName.ToLower() + "/tags/items/" + key.Replace(".", "/") + ".json",
                            JsonConvert.SerializeObject(Compiler.itemTags[key]));
                }
            }
        }
        public void ExportReadMe(string path)
        {
            if (compilerSetting.generateMAPSFile || compilerSetting.generateREADMEFile)
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
                    if (compilerSetting.generateREADMEFile)
                        SafeWriteFile(fileNameReadMe, readme);
                    if (compilerSetting.generateMAPSFile)
                        SafeWriteFile(fileNameOffuscation, offuscation);
                }
                catch (Exception e)
                {
                    throw new Exception("Notice " + e.ToString());
                }
            }
        }
        public void ExportStructures(string path)
        {
            if (projectPath != null)
            {
                SyncStructure();
            }

            string ProjectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            if (projectPath != null && Directory.Exists(path + "imported_dp/structure/"))
            {
                if (!Directory.Exists(path + "/data/" + projectName.ToLower() + "/structures/"))
                    Directory.CreateDirectory(path + "/data/" + projectName.ToLower() + "/structures/");

                foreach (string file in Directory.GetFiles(path + "imported_dp/structure/"))
                {
                    File.Copy(file, path + "/data/" + projectName.ToLower() + "/structures/" + Path.GetFileName(file));
                }
            }
            if (projectPath != null && Directory.Exists(ProjectFolder() + "/structures"))
            {
                if (!Directory.Exists(path + "/data/" + projectName.ToLower() + "/structures/"))
                    Directory.CreateDirectory(path + "/data/" + projectName.ToLower() + "/structures/");

                foreach (string file in Directory.GetFiles(ProjectFolder() + "/structures"))
                {
                    File.Copy(file, path + "/data/" + projectName.ToLower() + "/structures/" + Path.GetFileName(file));
                }
            }
        }
        public void ExportResourcePack(string path)
        {
            try
            {
                if (path != null && path != "" && exportRP)
                {
                    string rpPath = "C:/bprp/";
                    SafeWriteFile(rpPath + "/pack.mcmeta",
                                    JsonConvert.SerializeObject(
                                        new ResourcePackMeta(projectName + " - " + projectDescription,
                                        compilerSetting.rppackformat)));

                    if (Directory.Exists(rpPath))
                    {
                        if (File.Exists(path)) { File.Delete(path); }
                        ZipFile.CreateFromDirectory(rpPath, path);
                    }
                    Directory.Delete(rpPath, true);

                    DebugThread("Resources Pack successfully exported!", Color.Aqua);
                }
            }
            catch(Exception e)
            {
                DebugThread(e, Color.Red);
            }
        }
        #endregion

        #region Preview Button
        private void button1_Click(object sender, EventArgs e)
        {
            exporting = false;
            debugOffuscate = ModifierKeys == Keys.Shift;
            Compile(true);
            UpdateCodeBox();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            NewFile();
        }
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
        private void ParticlePreview_Click(object sender, EventArgs e)
        {
            FunctionPreview fp = new FunctionPreview(CommandParser.particles);
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
            Compile(true);
            UpdateCodeBox();
        }
        private void LibraryButton_Click(object sender, EventArgs e)
        {
            List<string> imported = code["import"].content.Split('\n').Where(x => x.StartsWith("import")).Select(x => x.Replace("import ", "")).ToList();
            LibImport libForm2 = new LibImport(compilerSetting.libraryFolder, imported);
            if (libForm2.ShowDialog() == DialogResult.OK)
            {
                string libs = libForm2.import.Count() > 0 ? libForm2.import.Select(x => $"import {x}").Aggregate((x, y) => x + "\n" + y) : "";
                code["import"].SetContent(libs);
                if (previous == "import")
                {
                    CodeBox.Text = code["import"].content;
                    CodeBox.ClearUndo();
                }
            }
        }
        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            ErrorBox.Text = "";
            debugMSGsShowned.Clear();
        }
        private void ErrorButton_Click(object sender, EventArgs e)
        {
            showError = !showError;
            ShowErrorButton.FlatAppearance.BorderSize = showError ? 3 : 1;
            ShowErrorButton.ForeColor = showError ? Color.FromArgb(255, 0, 165, 255) : Color.Gray;
            ReShowError();
        }
        private void WarningButton_Click(object sender, EventArgs e)
        {
            showWarning = !showWarning;
            ShowWarningButton.FlatAppearance.BorderSize = showWarning ? 3 : 1;
            ShowWarningButton.ForeColor = showWarning ? Color.FromArgb(255, 0, 165, 255) : Color.Gray;
            ReShowError();
        }
        private void InfoButton_Click(object sender, EventArgs e)
        {
            showInfo = !showInfo;
            ShowInfoButton.FlatAppearance.BorderSize = showInfo ? 3 : 1;
            ShowInfoButton.ForeColor = showInfo ? Color.FromArgb(255, 0, 165, 255) : Color.Gray;
            ReShowError();
        }
        private void SaveButton_Click(object sender, EventArgs e)
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
        #endregion

        #region Menu Strip
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenFile(ProjectOpen.FileName);
                }
                catch (Exception error)
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
                if (compilerSetting.autoSave && projectPath != "" && projectPath != null) Save(projectPath);
                string path = (currentDataPack == null) ? ExportSave.FileName : currentDataPack;
                currentDataPack = path;
                string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
                if (!Directory.Exists(rpdir))
                {
                    if (isCompiling == 0)
                    {
                        Thread t = new Thread(new ThreadStart(ExportDataPackThread));
                        t.Start();
                    }
                }
                else if (currentResourcesPack != null || ExportRP.ShowDialog() == DialogResult.OK)
                {
                    string rpPath = currentResourcesPack == null ? ExportRP.FileName : currentResourcesPack;
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
                        t.Priority = ThreadPriority.Highest;
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
                        t.Priority = ThreadPriority.Highest;
                        t.Start();
                    }
                }
            }
        }
        private void structuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StructureImport(projectPath.Replace(Path.GetFileName(projectPath), "")).ShowDialog();
        }
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Find_and_Replace far = new Find_and_Replace(CodeBox);
            //far.Show();
        }
        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Find_and_Replace far = new Find_and_Replace(CodeBox);
            //far.Show();
        }
        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectSetting settingForm = new ProjectSetting(projectName, projectVersion, projectDescription, compilerSetting, Compiler.variables);
            settingForm.ShowDialog();

            projectName = settingForm.ProjectName;
            projectVersion = settingForm.version;
            projectDescription = settingForm.description;
            settingChanged = true;
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
            Compile(true);
            UpdateCodeBox();
        }
        private void getCallStackTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetCallStackTrace();
        }
        private void generateResourcesPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateResourcesPackFolder();
        }
        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ignorNextListboxUpdate = true;
            string dir = treeView1.SelectedNode != null ? treeView1.SelectedNode.FullPath : "src/";
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Checked && dir.Contains("/"))
            {
                dir = dir.Substring(0, dir.LastIndexOf("/"));
            }
            string grp = dir.Contains("/") ? dir.Substring(0, dir.IndexOf("/")) : dir;
            dir = dir.Contains("/") ? dir.Substring(dir.IndexOf("/") + 1, dir.Length - dir.IndexOf("/") - 1) : "";
            string path = dir == "" ? "" : dir + "/";
            if (grp == "src")
            {
                NewFile();
            }
            else
            {
                NewItem f = new NewItem();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    NewFile(f.FileName);
                    ignorNextListboxUpdate = false;
                }
            }
        }
        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ignorNextListboxUpdate = true;
            NewItem f = new NewItem();
            if (f.ShowDialog() == DialogResult.OK)
            {
                NewFile(f.FileName, JSharp.NewFile.Type.FOLDER);
                ignorNextListboxUpdate = false;
            }
        }
        private void animatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = treeView1.SelectedNode != null ? treeView1.SelectedNode.FullPath : "src/";

            string grp = dir.Contains("/") ? dir.Substring(0, dir.IndexOf("/")) : dir;
            dir = dir.Contains("/") ? dir.Substring(dir.IndexOf("/") + 1, dir.Length - dir.IndexOf("/") - 1) : "";

            string gloDir = Path.GetDirectoryName(projectPath) + "/resourcespack/";
            if (grp == "resourcespack")
            {
                SafeWriteFile(gloDir + dir + ".mcmeta", "{\"animation\":{\"frametime\": 1}}");
                FetchFilesInDirectory();
                ReloadTree();
            }
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedFile();
        }
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedFile();
            }
        }
        private void libraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> imported = code["import"].content.Split('\n').Where(x => x.StartsWith("import")).Select(x => x.Replace("import ", "")).ToList();
            LibImport libForm2 = new LibImport(compilerSetting.libraryFolder, imported);
            if (libForm2.ShowDialog() == DialogResult.OK)
            {
                string libs = libForm2.import.Count() > 0 ? libForm2.import.Select(x => $"import {x}").Aggregate((x, y) => x + "\n" + y) : "";
                code["import"].SetContent(libs);
                if (previous == "import")
                {
                    CodeBox.Text = code["import"].content;
                    CodeBox.ClearUndo();
                }
            }
        }
        private void customPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomPaste inst = new CustomPaste();
            if (inst.ShowDialog() == DialogResult.OK)
            {
                customPaste = inst.customPasteReplace;
            }
        }
        private void inspectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InspectorForm inst = new InspectorForm(currentDataPack);
            inst.Show();
        }
        private void structureToCMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StructureToCMD inst = new StructureToCMD();
            inst.Show();
        }
        private void tilemapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ForceSave())
            {
                if (!Directory.Exists(Path.GetDirectoryName(projectPath) + "/tilemaps"))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(projectPath) + "/tilemaps");
                }
                TilemapGenerator t = new TilemapGenerator(Path.GetDirectoryName(projectPath) + "/tilemaps");
                t.Show();
            }
        }
        private void forgenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = treeView1.SelectedNode != null ? treeView1.SelectedNode.FullPath : "src/";

            string grp = dir.Contains("/") ? dir.Substring(0, dir.IndexOf("/")) : dir;
            dir = dir.Contains("/") ? dir.Substring(dir.IndexOf("/") + 1, dir.Length - dir.IndexOf("/") - 1) : "";

            string gloDir = Path.GetDirectoryName(projectPath) + "/resourcespack/";
            if (grp == "resourcespack")
            {
                string data = File.ReadAllText(gloDir + dir);
                SafeWriteFile(gloDir + dir.Replace(".json", ".bps"), 
                    "forgenerate($i, ){\njsonfile "+dir.Replace(".json", "")+"{\n" + data+"\n}\n}");
                DeleteSelectedFile();
                FetchFilesInDirectory();
                ReloadTree();
            }
        }
        #endregion

        #region codetree
        private void ReloadTree()
        {
            HashSet<string> allPath = new HashSet<string>();
            var paths = codeOrder.Select(x => x.Replace("\\", "/")).ToList();
            var root = GetNodeByName(treeView1.Nodes, "", "src").Nodes;

            paths.ForEach(x => { if (!allPath.Contains(x)) allPath.Add("src/" + x); });
            BuildTree(paths, "src", root);

            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            if (Directory.Exists(dir))
            {
                paths = Directory.EnumerateDirectories(dir, "*.*", SearchOption.AllDirectories)
                        .Select(x => x.Replace(dir, "").Replace("\\", "/")).ToList();
            }
            else
            {
                paths.Clear();
                paths.Add("src");
            }

            paths.ForEach(x => { if (!allPath.Contains(x)) allPath.Add(x); });
            BuildTree(paths, "src", root, false);

            paths = resourceOrder.Select(x => x.Replace("\\", "/")).ToList();
            root = GetNodeByName(treeView1.Nodes, "", "resources").Nodes;

            paths.ForEach(x => { if (!allPath.Contains(x)) allPath.Add("resources/" + x); });
            BuildTree(paths, "resources", root);

            dir = Path.GetDirectoryName(projectPath) + "/resources/";
            if (Directory.Exists(dir))
            {
                paths = Directory.EnumerateDirectories(dir, "*.*", SearchOption.AllDirectories)
                        .Select(x => x.Replace(dir, "").Replace("\\", "/")).ToList();
            }
            else
            {
                paths.Clear();
                paths.Add("resources");
            }

            paths.ForEach(x => { if (!allPath.Contains(x)) allPath.Add(x); });
            BuildTree(paths, "resources", root, false);


            string[] folders = { "resourcespack", "structures", "tilemaps" };
            folders.ToList().ForEach(folder =>
            {
                dir = Path.GetDirectoryName(projectPath) + $"/{folder}/";
                root = GetNodeByName(treeView1.Nodes, "", folder).Nodes;
                if (Directory.Exists(dir))
                {
                    paths = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                            .Select(x => x.Replace(dir, "").Replace("\\", "/")).ToList();
                }
                else
                {
                    paths.Clear();
                    paths.Add(folder);
                }
                paths.ForEach(x => { if (!allPath.Contains(x)) allPath.Add(folder + "/" + x); });
                BuildTree(paths, folder, root);

                if (Directory.Exists(dir))
                {
                    paths = Directory.EnumerateDirectories(dir, "*.*", SearchOption.AllDirectories)
                            .Select(x => x.Replace(dir, "").Replace("\\", "/")).ToList();
                }
                else
                {
                    paths.Clear();
                    paths.Add(folder);
                }
                paths.ForEach(x => { if (!allPath.Contains(x)) allPath.Add(x); });
                BuildTree(paths, folder, root, false);
            });

            TreeRM(allPath, treeView1.Nodes);
        }
        private void BuildTree(List<string> paths, string parent, TreeNodeCollection addInMe, bool isFile = true, int rec = 0)
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
                         TreeNode curNode = null;

                         foreach (TreeNode n in addInMe)
                         {
                             if (n.FullPath == parent + "/" + x.Key)
                             {
                                 curNode = n;
                             }
                         }
                         if (curNode == null)
                             curNode = addInMe.Add(x.Key);
                         BuildTree(x.Select(z => z.Last())
                                                 .ToList(),
                                                 parent + "/" + x.Key, curNode.Nodes, isFile, rec + 1);

                     });
                paths.Where(x => !x.Contains("/")).ToList().ForEach(x =>
                {
                    TreeNode curNode = null;
                    foreach (TreeNode n in addInMe)
                    {
                        if (n.FullPath == parent + "/" + x)
                        {
                            curNode = n;
                        }
                    }
                    if (curNode == null)
                        addInMe.Add(x).Checked = isFile;
                });
            }
        }
        private void TreeRM(HashSet<string> paths, TreeNodeCollection addInMe, int height = 0)
        {
            foreach (TreeNode n in addInMe)
            {
                if (n != null && n.Nodes != null)
                    TreeRM(paths, n.Nodes, height++);
                if (n != null && n.FullPath != null && !paths.Contains(n.FullPath) && n.Nodes.Count == 0 && height > 0)
                {
                    addInMe.Remove(n);
                }
            }
        }
        private TreeNode GetNodeByName(TreeNodeCollection addInMe, string path, string key)
        {
            TreeNode curNode = null;

            foreach (TreeNode n in addInMe)
            {
                if ((path == "" && n.FullPath == key) || n.FullPath == path + "/" + key)
                {
                    curNode = n;
                }
            }
            if (curNode == null)
                curNode = addInMe.Add(key);
            return curNode;
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
                SelectFullPath(fullPath);
            }
        }
        private Image DisplayImage(Image Image, float scale)
        {
            int wid = (int)(Image.Width * scale);
            int hgt = (int)(Image.Height * scale);
            Bitmap bm = new Bitmap(wid, hgt);

            // Draw the image onto the new bitmap.
            using (Graphics gr = Graphics.FromImage(bm))
            {
                // No smoothing.
                gr.InterpolationMode = InterpolationMode.NearestNeighbor;

                Point[] dest =
                {
            new Point(0, 0),
            new Point(wid, 0),
            new Point(0, hgt),
        };
                Rectangle source = new Rectangle(
                    0, 0,
                    Image.Width,
                    Image.Height);
                gr.DrawImage(Image,
                    dest, source, GraphicsUnit.Pixel);
            }

            return (Image)bm;
        }
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            ChangeCompileOrder();
        }
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Rectangle nodeRect = e.Node.Bounds;

            /*--------- 1. draw expand/collapse icon ---------*/
            if (!(nodeRect.Location.X == 0 && nodeRect.Location.Y == 0)) {
                Point ptExpand = new Point(nodeRect.Location.X - 10, nodeRect.Location.Y - 2);
                Image expandImg = null;

                if (e.Node.IsExpanded)
                    expandImg = minusPath;
                else
                    expandImg = plusPath;

                if (!e.Node.Checked)
                {
                    e.Graphics.DrawImage(expandImg, ptExpand);
                }


                /*--------- 2. draw node icon ---------*/
                Point ptNodeIcon = new Point(nodeRect.Location.X - 4, nodeRect.Location.Y - 2);
                Image nodeImg = filePath;
                if (e.Node.FullPath.EndsWith(".csv"))
                {
                    nodeImg = fileCSVPath;
                }
                if (e.Node.FullPath.EndsWith(".ini"))
                {
                    nodeImg = fileINIPath;
                }
                if (e.Node.FullPath.EndsWith(".txt") || e.Node.FullPath.EndsWith(".mcmeta"))
                {
                    nodeImg = fileTXTPath;
                }
                if (e.Node.FullPath.EndsWith(".json"))
                {
                    nodeImg = fileINIPath;
                }
                if (e.Node.FullPath.EndsWith(".png"))
                {
                    nodeImg = fileImagePath;
                }

                if (e.Node.Checked)
                {
                    e.Graphics.DrawImage(nodeImg, ptNodeIcon);
                }

                /*--------- 3. draw node text ---------*/
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null)
                    nodeFont = ((TreeView)sender).Font;
                Brush textBrush = new SolidBrush(Color.White);
                //to highlight the text when selected
                if ((e.State & TreeNodeStates.Focused) != 0)
                    textBrush = new SolidBrush(Color.Aqua);
                //Inflate to not be cut
                Rectangle textRect = nodeRect;
                //need to extend node rect
                textRect.Width += 40;
                e.Graphics.DrawString(e.Node.Text, nodeFont, textBrush, Rectangle.Inflate(textRect, -12, 0));
            }
        }
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));
            treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);
            if (treeView1.SelectedNode.Checked)
                treeView1.SelectedNode = treeView1.SelectedNode.Parent;

            if (ForceSave())
            {
                string dir = treeView1.SelectedNode != null ? treeView1.SelectedNode.FullPath : "src/";
                if (treeView1.SelectedNode != null && treeView1.SelectedNode.Nodes.Count == 0 && dir.Contains("/"))
                {
                    dir = dir.Substring(0, dir.LastIndexOf("/"));
                }
                string grp = dir.Contains("/") ? dir.Substring(0, dir.IndexOf("/")) : dir;
                dir = dir.Contains("/") ? dir.Substring(dir.IndexOf("/") + 1, dir.Length - dir.IndexOf("/") - 1) : "";
                string path = dir == "" ? "" : dir + "/";
                string gloDir = Path.GetDirectoryName(projectPath) + "/";
                string final = "";
                if (grp == "src")
                {
                    final = gloDir + "scripts/" + path + "/";
                }
                if (grp == "resources")
                {
                    final = gloDir + "resources/" + path + "/";
                }
                if (grp == "resourcespack")
                {
                    final = gloDir + "resourcespack/" + path + "/";
                }
                if (grp == "structures")
                {
                    final = gloDir + "structures/" + path + "/";
                }
                foreach (string f in files)
                {
                    var newP = f.Replace("\\", "/");
                    newP = newP.Substring(newP.LastIndexOf("/") + 1, newP.Length - newP.LastIndexOf("/") - 1);
                    SafeCopy(f, final + newP);
                    Debug("Imported " + newP, Color.DarkGreen);
                }
                CheckFileModdification();
                ReloadTree();
            }
        }
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }
        private void CollapseRec(TreeNode node, int rec = 0)
        {
            if (rec < 100)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    CollapseRec(n, rec + 1);
                }
                node.Collapse();
            }
        }
        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                CollapseRec(e.Node);
            }
        }
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                e.Node.ExpandAll();
            }
        }
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Point targetPoint = treeView1.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);

            string gloDir = Path.GetDirectoryName(projectPath) + "/";
            string path = treeView1.SelectedNode.FullPath;
            if (path.StartsWith("src/"))
            {
                path = path.Replace("src/", "scripts/")+".bps";
            }
            MessageBox.Show(gloDir + path);
            if (File.Exists(gloDir+path))
                System.Diagnostics.Process.Start(gloDir + path);
            else
                System.Diagnostics.Process.Start("explorer.exe", gloDir + path);

        }
        public void ShowCodeBox()
        {
            pictureBox1.Visible = false;
            CodeBox.Visible = true;
        }
        public void ShowImageBox()
        {
            pictureBox1.Visible = true;
            CodeBox.Visible = false;
        }
        public void ShowMediaBox()
        {
            pictureBox1.Visible = false;
            CodeBox.Visible = false;
        }
        private void SelectFullPath(string fullPath)
        {
            if (fullPath.StartsWith("src") && fullPath.Length > 4)
            {
                string path = fullPath.Substring(4, fullPath.Length - 4);

                if (code.ContainsKey(path))
                {
                    ShowCodeBox();
                    ReloadCodeBoxFileCode(path);
                    AddTabBar(fullPath);
                }
            }
            if (fullPath.StartsWith("resources") && fullPath.Length > 10)
            {
                string path = fullPath.Substring(10, fullPath.Length - 10);

                if (resources.ContainsKey(path))
                {
                    ShowCodeBox();
                    ReloadCodeBoxFileRes(path);
                    AddTabBar(fullPath);
                }
            }
            if (fullPath.StartsWith("resourcespack") && fullPath.Length > 14)
            {
                string path = fullPath.Substring(14, fullPath.Length - 14);

                string dir = Path.GetDirectoryName(projectPath) + "/resourcespack/";

                if (File.Exists(dir + path))
                {
                    if (path.EndsWith(".json") || path.EndsWith(".bps") || path.EndsWith(".txt") || path.EndsWith(".mcmeta")
                        || !path.Contains("."))
                    {
                        ShowCodeBox();
                        ReloadCodeBoxFileResPack(path);
                        AddTabBar(fullPath);
                    }
                    if (path.EndsWith(".png"))
                    {
                        ShowImageBox();
                        var img = Image.FromFile(dir + path);
                        float ratio = Math.Min(pictureBox1.Width / (float)img.Size.Width,
                                                pictureBox1.Height / (float)img.Size.Height);
                        pictureBox1.Image = DisplayImage(img, ratio);
                        AddTabBar(fullPath);
                    }
                    if (path.EndsWith(".ogg"))
                    {
                        ShowMediaBox();
                    }
                }
            }
        }
        private void ResetTabBar()
        {
            openedFullPath.Clear();
            tabControl1.TabPages.Clear();
        }
        private void AddTabBar(string fullPath)
        {
            if (!openedFullPath.Contains(fullPath))
            {
                if (openedFullPath.Count > 20)
                {
                    openedFullPath.RemoveAt(20);
                }
                openedFullPath.Insert(0, fullPath);
                tabControl1.TabPages.Clear();
                openedFullPath.ForEach(name =>
                {
                    tabControl1.TabPages.Add(new TabPage(name.Substring(name.LastIndexOf("/") + 1, name.Length - name.LastIndexOf("/") - 1)));
                });
            }
            tabControl1.SelectedIndex = openedFullPath.IndexOf(fullPath);
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex > -1 && tabControl1.SelectedIndex < openedFullPath.Count)
            {
                SelectFullPath(openedFullPath[tabControl1.SelectedIndex]);
            }
        }
        public void DeleteSelectedFile()
        {
            string dir = treeView1.SelectedNode != null ? treeView1.SelectedNode.FullPath : "src/";
            string fullPath = dir;
            string grp = dir.Contains("/") ? dir.Substring(0, dir.IndexOf("/")) : dir;
            dir = dir.Contains("/") ? dir.Substring(dir.IndexOf("/") + 1, dir.Length - dir.IndexOf("/") - 1) : "";
            if (MessageBox.Show($"Are you sure you want to delete {dir} from {grp}?", "Are you sure?", MessageBoxButtons.OKCancel)
                == DialogResult.OK)
            {
                if (openedFullPath.Contains(fullPath))
                {
                    openedFullPath.Remove(fullPath);
                    tabControl1.TabPages.Clear();
                    openedFullPath.ForEach(name =>
                    {
                        tabControl1.TabPages.Add(new TabPage(name.Substring(name.LastIndexOf("/") + 1, name.Length - name.LastIndexOf("/") - 1)));
                    });
                }

                string gloDir = Path.GetDirectoryName(projectPath) + "/resourcespack/";
                if (grp == "resourcespack")
                {
                    if (File.Exists(gloDir + dir))
                    {
                        File.Delete(gloDir + dir);
                    }
                }
                gloDir = Path.GetDirectoryName(projectPath) + "/structures/";
                if (grp == "structures")
                {
                    if (File.Exists(gloDir + dir))
                    {
                        File.Delete(gloDir + dir);
                    }
                }
                gloDir = Path.GetDirectoryName(projectPath) + "/resources/";
                if (grp == "resources")
                {
                    if (File.Exists(gloDir + dir))
                    {
                        File.Delete(gloDir + dir);
                    }
                    if (resources.ContainsKey(dir))
                    {
                        resourceOrder.Remove(dir);
                        resources.Remove(dir);
                    }
                }
                gloDir = Path.GetDirectoryName(projectPath) + "/scripts/";
                if (grp == "src")
                {
                    if (File.Exists(gloDir + dir + ".bps"))
                    {
                        File.Delete(gloDir + dir + ".bps");
                    }
                    if (code.ContainsKey(dir))
                    {
                        codeOrder.Remove(dir);
                        code.Remove(dir);
                    }
                }
                FetchFilesInDirectory();
                ReloadTree();
            }
        }
        #endregion

        #region CodeBox
        private void CodeBox_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void autocompleteMenu1_Selected(object sender, AutocompleteMenuNS.SelectedEventArgs e)
        {
            if (e.Item.Text.Contains("^"))
            {
                string text = "";
                for (int i = 0; i < e.Item.Text.Length - e.Item.Text.IndexOf('^'); i++)
                {
                    text += "{LEFT}";
                }
                text += "{DELETE}";
                SendKeys.Send(text);
            }
        }
        private void CodeBox_CustomAction(object sender, FastColoredTextBoxNS.CustomActionEventArgs e)
        {
            string text = Clipboard.GetText();
            // Special Paste
            if (e.Action == FastColoredTextBoxNS.FCTBAction.CustomAction1)
            {
                if (customPaste != null && customPaste.Get(text) != text)
                {
                    CodeBox.InsertText(customPaste.Get(text));
                }
                else if (new Regex(@"/setblock [\-\d]+ [\-\d]+ [\-\d]+").Match(text).Success)
                {
                    Regex reg2 = new Regex(@"[\-\d]+ [\-\d]+ [\-\d]+");
                    CodeBox.InsertText(reg2.Match(text).Value.Replace(" ", ","));
                }
                else if (new Regex(@"^/summon [\w\.\:]+ [\-\d\.~]+ [\-\d\.~]+ [\-\d\.~]+ \{.+\}").Match(text).Success)
                {
                    Regex reg2 = new Regex(@"\{.+\}");
                    CodeBox.InsertText(reg2.Match(text).Value);
                }
                else if (new Regex(@"tp @\w [\-\d\.]+ [\-\d\.]+ [\-\d\.]+ [\-\d\.]+ [\-\d\.]+").Match(text).Success)
                {
                    Regex reg2 = new Regex(@"[\-\d\.]+ [\-\d\.]+ [\-\d\.]+ [\-\d\.]+ [\-\d\.]+");
                    CodeBox.InsertText(reg2.Match(text).Value.Replace(" ", ","));
                }
                else if (new Regex(@"([\-\d\.]+\s)+").Match(text).Success)
                {
                    Regex reg2 = new Regex(@"([\-\d\.]+\s)+");
                    CodeBox.InsertText(reg2.Match(text).Value.Replace(" ", ","));
                }
                else
                {
                    CodeBox.InsertText(text);
                }
            }
        }
        private void CodeBox_ToolTipNeeded(object sender, FastColoredTextBoxNS.ToolTipNeededEventArgs e)
        {
            Regex reg = new Regex(@"[\w\._\:]+");
            string word = "";
            foreach (Match m in reg.Matches(CodeBox.Text.Split('\n')[e.Place.iLine]))
            {
                if (m.Index <= e.Place.iChar && m.Index + m.Length >= e.Place.iChar)
                {
                    word = m.Value;
                }
            }
            if (word != "")
            {
                string text = "";
                try
                {
                    text = Compiler.functions
                            ?.Keys
                            .Where(x => x.EndsWith(word.ToLower()))
                            .Select(x => Compiler.functions[x])
                            .Aggregate((x, y) => x.Concat(y).ToList())
                            .Where(x => !x.gameName.Contains("__struct__"))
                            .Select(x => $"{((x.outputs.Count > 0) ? (x.outputs.Select(arg => arg.GetFancyTypeString()).Aggregate((s1, s2) => s1 + ", " + s2)) : "void")} {x.gameName.Replace("/", ".").Replace(":", ".")}({((x.args.Count > 0) ? (x.args.Select(arg => arg.GetFancyTypeString() + " " + arg.name).Aggregate((s1, s2) => s1 + ", " + s2)) : "")})\n{x.desc}\n\n")
                            .Aggregate((x, y) => x + y);
                    e.ToolTipText = text;
                    e.ToolTipTitle = "Function";
                    e.ToolTipIcon = ToolTipIcon.None;
                }
                catch { }
            }
        }
        private void CodeBox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (!noReformat)
            {
                try
                {
                    if (resourceSelected == "res")
                    {
                        resources[previous].wasModdified = true;
                    }
                    else if (resourceSelected == "src")
                    {
                        code[previous].wasModdified = true;
                    }
                }
                catch { }
            }
        }
        private void CodeBox_Load(object sender, EventArgs e)
        {

        }
        private void fastColoredTextBox1_Enter(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            CodeBox.DescriptionFile = "";
            CodeBox.DescriptionFile = path + "formating.xml";
            Formatter.getAutoComplete(autocompleteMenu1, previous, CodeBox.Text);
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
            }
        }
        private void CodeBox_Leave(object sender, EventArgs e)
        {
            recallFile();
        }
        #endregion

        

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
        public class FormFile
        {
            public string name;
            public string content;
            public DateTime date;
            public bool wasModdified;

            public FormFile(string name, string content)
            {
                this.name = name;
                this.content = content;
                this.wasModdified = true;
            }
            public FormFile(string name, string content, bool wasModdified)
            {
                this.name = name;
                this.content = content;
                this.wasModdified = wasModdified;
            }

            public void SetContent(string content)
            {
                if (this.content.Replace("\r", "") != content.Replace("\r",""))
                {
                    this.content = content.Replace("\r", "");
                    wasModdified = true;
                }
            }
            public void Save()
            {
                wasModdified = false;
            }
        }

        private void showDebugStackTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (openDebugFile.ShowDialog() == DialogResult.OK)
            {
                ShowGraphviz(DebugFunctioParser.Parse(File.ReadAllText(openDebugFile.FileName)));
            }
        }

        private void InvalidateRP_Click(object sender, EventArgs e)
        {
            exportNew = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            compilerSetting.structuresSource = Directory.GetParent(currentDataPack).Parent.FullName + "/generated/minecraft/structures";
            SyncStructure();
        }

        private void SyncStructure()
        {
            try
            {
                if (compilerSetting.structuresSource != null)
                {
                    var projectDirectory = ProjectFolder() + "/structures/";
                    if (!Directory.Exists(projectDirectory))
                        Directory.CreateDirectory(projectDirectory);

                    if (Directory.Exists(compilerSetting.structuresSource))
                    {
                        var p = compilerSetting.structuresSource.Replace("\\\\", "/").Replace("\\", "/");
                        Directory.EnumerateFiles(compilerSetting.structuresSource, "*.*", SearchOption.AllDirectories)
                            .Where(x => !compilerSetting.structuresSources.ContainsKey(x) || File.GetLastWriteTime(x) != compilerSetting.structuresSources[x])
                            .ToList()
                            .ForEach(x =>
                            {
                                compilerSetting.structuresSources[x] = File.GetLastWriteTime(x);
                                Debug($"Import Structure: {x}", Color.Yellow);
                                File.Copy(x, ProjectFolder() + "/structures/" + x.Replace("\\\\", "/").Replace("\\", "/").Replace(p, ""), true);
                            });
                    }
                }
            }
            catch
            {
                Debug("Failled To Sync Structure", Color.Yellow);
            }
        }

        private void showOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isCompiling == 0) isCompiling = 2;
        }

        private void ShowOutput_Click(object sender, EventArgs e)
        {
            if (isCompiling == 0) isCompiling = 2;
        }
    }
}
