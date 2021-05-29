using JSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluePhoenix
{
    public class CMD_Compile
    {
        private ProjectSave project;
        private string projectPath;
        private Dictionary<string, string> resources = new Dictionary<string, string>();
        private Dictionary<string, string> code = new Dictionary<string, string>();
        private string path;
        public static StringBuilder consoleText;
        public CMD_Compile(string project, string path)
        {
            consoleText = new StringBuilder();
            projectPath = project;
            this.project = JsonConvert.DeserializeObject<ProjectSave>(File.ReadAllText(project));
            
            this.path = path;
            
            OpenFile();
        }
        public string Export()
        {
            ExportDataPack(path, path+"rp.zip");
            return consoleText.ToString();
        }
        public void ExportDataPack(string path, string rpPath)
        {
            string ProjectPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            string writePath = project.compilationSetting.ExportAsZip ? ProjectPath + "tmp_dp" : path;

            if (File.Exists(writePath + "/pack.mcmeta"))
            {
                Directory.Delete(writePath, true);
            }
            project.version.Build();
            ExportFiles(writePath);
            ExportTags(writePath);
            ExportStructures(writePath);
            ExportReadMe(writePath);
            SafeCopy(ProjectPath + "/assets/pack.png", writePath + "/pack.png");
            SafeWriteFile(writePath + "/pack.mcmeta",
                        JsonConvert.SerializeObject(new DataPackMeta(
                            project.projectName + " - " + project.description,
                            project.compilationSetting.packformat)));

            if (project.compilationSetting.ExportAsZip)
            {
                if (!path.EndsWith(".zip")) path += ".zip";
                if (File.Exists(path)) { File.Delete(path); }
                ZipFile.CreateFromDirectory(writePath, path);
                Directory.Delete(writePath, true);
            }

            ExportResourcePack(rpPath);
            Debug("Datapack successfully exported!", Color.Aqua);
        }
        public static void Debug(object text, Color c)
        {
            if (text != null)
            {
                consoleText.Append(text);
                consoleText.Append("\n");
            }
        }
        public void ExportFiles(string path)
        {
            List<Compiler.File> files = new List<Compiler.File>();
            foreach (string f in code.Keys)
            {
                files.Add(new Compiler.File(f, code[f].Replace('\t' + "", "")));
            }
            string rpdir = Path.GetDirectoryName(projectPath) + "/resourcespack";
            if (Directory.Exists(rpdir))
            {
                foreach (var file in Directory.GetFiles(rpdir, "*.bps", SearchOption.AllDirectories))
                {
                    var f = new Compiler.File(file, File.ReadAllText(file).Replace('\t' + "", ""));
                    f.resourcespack = true;
                    files.Add(f);
                }
            }

            List<Compiler.File> resourcesfiles = new List<Compiler.File>();
            foreach (string f in resources.Keys)
            {
                resourcesfiles.Add(new Compiler.File(f, resources[f].Replace('\t' + "", "")));
            }
            List<Compiler.File> cFiles = Compiler.compile(new CompilerCoreJava(), project.projectName, files, resourcesfiles,
                                            CMD_Compile.Debug, project.compilationSetting, project.version,
                                            Path.GetDirectoryName(projectPath));
            
            foreach (Compiler.File f in cFiles)
            {
                string fileName;
                if (f.type == "json" && f.name.Contains("json"))
                    fileName = path + "/data/" + project.projectName.ToLower() + "/" + f.name;
                else if (f.type == "json" && !f.name.Contains("json"))
                    fileName = path + "/data/" + project.projectName.ToLower() + "/" + f.name + ".json";
                else
                    fileName = path + "/data/" + project.projectName.ToLower() + "/functions/" + f.name + ".mcfunction";
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
            foreach (string key in project.mcTagsList.Keys)
            {
                foreach (string file in project.mcTagsList[key].Keys)
                {
                    if (key == "functions")
                    {
                        SafeWriteFile(path + "/data/minecraft/tags/" + key + "/" + file + ".json",
                            JsonConvert.SerializeObject(project.mcTagsList[key][file].ToFunctions(project.projectName.ToLower())));
                    }
                    else
                    {
                        SafeWriteFile(path + "/data/minecraft/tags/" + key + "/" + file + ".json",
                            JsonConvert.SerializeObject(project.mcTagsList[key][file]));
                    }
                }
            }
            foreach (string key in project.TagsList.Keys)
            {
                foreach (string file in project.TagsList[key].Keys)
                {
                    SafeWriteFile(path + "/data/" + project.projectName.ToLower() + "/tags/" + key + "/" + file + ".json",
                        JsonConvert.SerializeObject(project.TagsList[key][file]));
                }
            }
            foreach (string key in Compiler.blockTags.Keys)
            {
                SafeWriteFile(path + "/data/" + project.projectName.ToLower() + "/tags/blocks/" + key + ".json",
                        JsonConvert.SerializeObject(Compiler.blockTags[key]));
            }
            foreach (string key in Compiler.entityTags.Keys)
            {
                SafeWriteFile(path + "/data/" + project.projectName.ToLower() + "/tags/entity_types/" + key + ".json",
                        JsonConvert.SerializeObject(Compiler.entityTags[key]));
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
            if (Directory.Exists(ProjectFolder() + "/structures"))
            {
                Directory.CreateDirectory(path + "/data/" + project.projectName.ToLower() + "/structures/");

                foreach (string file in Directory.GetFiles(ProjectFolder() + "/structures"))
                {
                    File.Copy(file, path + "/data/" + project.projectName.ToLower() + "/structures/" + Path.GetFileName(file));
                }
            }
        }
        public void ExportResourcePack(string path)
        {
            if (path != null && path != "")
            {
                string rpPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/tmp_rp";
                SafeWriteFile(rpPath + "/pack.mcmeta",
                                JsonConvert.SerializeObject(new ResourcePackMeta(project.projectName + " - " + project.description)));

                if (Directory.Exists(rpPath))
                {
                    if (File.Exists(path)) { File.Delete(path); }
                    ZipFile.CreateFromDirectory(rpPath, path);
                }
                Directory.Delete(rpPath, true);
            }
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
            string filePath = fileName.Substring(0, fileName.LastIndexOf('/'));
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            File.WriteAllText(fileName, content);
        }
        public string ProjectFolder()
        {
            return projectPath.Replace(Path.GetFileName(projectPath), "");
        }
        public void OpenFile()
        {
            string dir = Path.GetDirectoryName(projectPath) + "/scripts/";
            string dirRes = Path.GetDirectoryName(projectPath) + "/resources/";
            
            if (Path.GetDirectoryName(projectPath)== "")
            {
                dir = "." + dir;
                dirRes = "." + dirRes;
            }
            
            if (project.resources != null)
            {
                foreach (var file in project.resources)
                {
                    try
                    {
                        resources.Add(file.name, file.content);
                    }
                    catch
                    {
                        Debug("Duplicated " + file.name + "-" + file.content + "////" + resources[file.name], Color.Red);
                    }
                }
            }

            if (project.compileOrder != null)
            {
                foreach (var file in project.compileOrder)
                {
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
                }
            }
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
                            code.Add(fname.ToLower(), File.ReadAllText(file));
                        }
                        catch (Exception e)
                        {
                            Debug("Exception while reading " + e.ToString(), Color.Red);
                        }
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
                            resources.Add(fname.ToLower(), File.ReadAllText(file));
                        }
                        catch (Exception e)
                        {
                            Debug("Exception while reading " + e.ToString(), Color.Red);
                        }
                    }
                }
            }
        }
    }
}
