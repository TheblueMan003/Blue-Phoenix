using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSharp
{
    public class Compiler
    {
        #region dictonary
        public static Dictionary<string, List<Function>> functions;
        public static HashSet<Function> abstractFunctionsNeeded;
        public static Dictionary<string, Scoreboard> scoreboards;
        public static Dictionary<string, Variable> variables;
        private static Dictionary<int, Variable> constants;
        private static Dictionary<int, Variable> constantsF;
        public static Dictionary<string, Enum> enums;
        public static Dictionary<string, Structure> structs;
        public static Dictionary<string, TagsList> blockTags;
        public static Dictionary<string, TagsList> entityTags;
        public static Dictionary<string, TagsList> itemTags;
        public static Dictionary<string, List<Predicate>> predicates;
        public static Dictionary<string, List<string>> functionTags;
        public static Dictionary<int, List<File>> dedupFiles;
        #endregion

        public static Dictionary<string, string> offuscationMap;
        public static Dictionary<string, string> classOffuscationMap;
        private static Dictionary<string, string> packageMap;
        private static List<Dictionary<string, string>> lazyEvalVar;
        private static Dictionary<string, List<Function>> functDelegated;
        private static Dictionary<string, List<File>> functDelegatedFile;
        private static Dictionary<string, string> resourceFiles;
        private static Dictionary<string, Regex> compRegexCache = new Dictionary<string, Regex>();
        public static List<string> packages;
        private static HashSet<string> offuscationSet;
        private static HashSet<string> classOffuscationSet;
        private static HashSet<string> imported;

        private static Stack<Switch> switches;
        private static List<string> stringSet;
        private static Stack<Structure> structStack;
        private static Stack<string> ExtensionClassStack;
        private static Dictionary<string, List<Function>> ExtensionMethod;
        private static List<File> files;
        private static List<File> jsonFiles;
        public static List<File> resourcespackFiles;

        public static Context context;
        private static File loadFile;
        private static File mainFile;

        private static int tmpID;
        public static string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
        private static bool isInStructMethod;
        private static bool isInStaticMethod;
        private static bool forcedUnsed = false;

        private static Stack<Function> lazyCall = new Stack<Function>();
        private static Stack<List<Variable>> lazyOutput;
        private static File structMethodFile;
        private static File stringPool;
        private static Stack<string> thisDef = new Stack<string>();
        private static string currentFile;
        private static int currentLine;
        public delegate void Debug(object o, Color c);
        public static Debug GlobalDebug;
        public static string Project;
        public static int autoIndented = 0;
        public static bool inGenericStruct;
        private static Stack<Dictionary<string, string>> typeMaps = new Stack<Dictionary<string, string>>();
        private static string currentPackage;
        private static Stack<string> adjPackage;
        private static string functionDesc = "";
        private static bool isInFunctionDesc;
        private static Stack<If> LastConds;
        private static If LastCond;
        private static string projectFolder;
        private static long[] pow64 = new long[11];
        private static string dirVar;
        public static CompilerCore Core;
        private static ProjectVersion projectVersion;
        private static int jsonIndent = 0;

        private static List<string> funcDef;
        private static List<string> funcDefF;
        private static List<string> funcDefM;
        private static List<string> funcDefL;
        private static Dictionary<string, List<string>> varWord;
        private static Dictionary<string, List<string>> objectFunc;
        private static string callTrace = "digraph G {\nmain\nload\nhelper\n";
        private static string callingFunctName = "loading";
        private static bool structInstCompVar;
        private static Dictionary<string, string> structCompVarPointer;
        private static List<string> attributes;
        private static bool structGenerating;

        #region Regexs
        private static Regex funcReg = new Regex(@"^(@?[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])*\s+)+(?<function_name>[\w\.=\?<>\+\/\*\-\^\&\|\#]+)\s*\((.+\s+.+)*\)");
        private static Regex nullReg = new Regex(@"\s*null\s*");
        private static Regex enumsDesugarReg = new Regex(@"(?s)(enum\s+\w+\s*(\([a-zA-Z0-9\- ,_=:/\\\.""'!\[\]]*\))?\s*\{(\s*\w*(\([a-zA-Z0-9/\\\- ,_=:\.""'!:\[\]\(\)]*\))?,?\s*)*\}|enum\s+\w+\s*=\s*(\([a-zA-Z0-9/\\\- ,_=""'\[\]!:\(\)]*\))?\s*\{(\s*\w*(\([a-zA-Z0-9/\\\- ,_=:\.""'\[\]!\(\)]*\))?,?\s*)*\})");
        private static Regex blocktagsDesugarReg = new Regex(@"(?s)(blocktags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|blocktags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex entitytagsDesugarReg = new Regex(@"(?s)(entitytags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|entitytags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex itemtagsDesugarReg = new Regex(@"(?s)(itemtags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|itemtags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex funArgTypeReg = new Regex(@"^([@\w\.=\?<>\+\/\*\-\^\&\|\#]*\s*(<\(?\w*\)?,?\(?\w*\)?>)?(\[\d+\])?)*\(");
        private static Regex arraySizeReg = new Regex(@"(?:\[)\d+(?:\])");
        private static Regex opReg = new Regex(@"((#=)|(\+=)|(\-=)|(\*=)|(/=)|(\%=)|(\&=)|(\|=)|(\^=)|(:=)|(=))");
        private static Regex elsifReg = new Regex(@"^el?s?e?\s*ifs?\s?\(");
        private static Regex ifReg = new Regex(@"^if\s*\(");
        private static Regex jsonFileReg = new Regex(@"^jsonfile\s+[\w\\/\-\.]+\{?");
        private static Regex predicateFileReg = new Regex(@"^predicate\s+[\w\\/\-\(\)\$ ]+\{?");
        private static Regex ifsReg = new Regex(@"^ifs\s*\(");
        private static Regex switchReg = new Regex(@"^switch\s*\(");
        private static Regex caseReg = new Regex(@"^case\s*\(");
        private static Regex withReg = new Regex(@"^((with)|(as))s*\(");
        private static Regex atReg = new Regex(@"^at\s*\(");
        private static Regex positonedReg = new Regex(@"^positioned\s*\(");
        private static Regex alignReg = new Regex(@"^align\s*\(");
        private static Regex whileReg = new Regex(@"^while\s*\(");
        private static Regex forgenerateReg = new Regex(@"^forgenerate\s*\(");
        private static Regex forReg = new Regex(@"^for\s*\(");
        private static Regex enumReg = new Regex(@"^(\w+\s+)*enum\s+\w+\s*(\([a-zA-Z0-9 ,_=]*\))?\s*=");
        private static Regex enumFileReg = new Regex(@"^(\w+\s+)*enum\s*(\([a-zA-Z0-9\. ,_=" + "\"" + @"]*\))\s*");
        private static Regex blocktagReg = new Regex(@"^blocktags\s+\w+\s*=");
        private static Regex entitytagReg = new Regex(@"^entitytags\s+\w+\s*=");
        private static Regex itemtagReg = new Regex(@"^itemtags\s+\w+\s*=");
        private static Regex varInstReg = new Regex(@"^[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])*\s+[\w\$\.]+\s*");
        private static Regex compVarInstReg = new Regex(@"^[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])?\s+\$[\w\$\.]+\s*=");
        private static Regex elseReg = new Regex(@"^else\s*");
        private static Regex regEval = new Regex(@"\$eval\([0-9a-zA-Z\-\+\*/% \.\^]*\)");
        private static Regex regEval2 = new Regex(@"\$eval\([0-9a-zA-Z\-\+\*/% \.\(\)\s\^]*\)eval\$");
        private static Regex forgenInLineReg = new Regex(@"forgenerate\([^\(\)]*\)\{[^\{\}]*\}");
        private static Regex dualCompVar = new Regex(@"^\$[\w\.\$]+\s*=\s*\$?[\w\.\$]+\s*");
        private static Regex requireReg = new Regex(@"^require\s+\$?\w+\s+[\w\=\<\>]+");
        private static Regex indexedReg = new Regex(@"^indexed\s+\$?\w+\s+\w+");
        private static Regex functionTypeReg = new Regex(@"^(\([\w\,\s=>]+\)|\w+)\s*=>\s*(\([\w\,\s=>]+\)|\w+)");
        private static Regex functionTypeRegRelaxed = new Regex(@"(\([\w\,\s=>]+\)|\w+)\s*=>\s*(\([\w\,\s=>]+\)|\w+)");
        private static Regex thisReg = new Regex("\\bthis\\.");
        private static Regex thisReg2 = new Regex("\\$this\\.");
        private static Regex curriedReg = new Regex(@"([\w\.]*\(.*\)){2,100}");
        private static Regex valExtReg = new Regex(@"((\bval\b)|(\bvar\b)|(\blet\b))\s+\w+");
        private static Regex valReg = new Regex(@"((\bval\b)|(\bvar\b)|(\blet\b))");
        private static Regex entityTagsRpReg = new Regex(@"type=#[\w\.:/]+");
        private static Regex shortFuncReg = new Regex(@"\b[\w\.]+\{");
        private static Regex classReg = new Regex(@"^(\w+\s+)*((class)|(struct)|(interface))\s+\w+");
        private static Regex defineReg = new Regex(@"(?s)^\s*define ");
        private static Regex arrayVarReg = new Regex(@"\w+\[.+\]");
        private static Regex arrayReg = new Regex(@"\[.+\]");
        private static Regex arrayFuncReg = new Regex(@"\[.+\]\.");
        private static Regex arrayFunc2Reg = new Regex(@"\[.+\]");
        private static Regex arrayFunc3Reg = new Regex(@"\[.+\]\(.*\)");
        private static Regex attributeReg = new Regex(@"^\[.+\]");
        private static Regex entityReg = new Regex(@"^@[aspre](\[.+\])?");
        private static string[] operators_base = { "+", "*", "/", "-", "%" };
        private static string[] operators_bool = { "&", "|", "^"};
        private static string[] operators_comp = { "<=", "==", "!=","<", ">", ">=" };
        private static string[] illagal_op = { "=", "+=", "-=", "*=", ":=", "/=" };

        private static string ConditionAlwayTrue = "=$=TRUE=$=";
        private static string ConditionAlwayFalse = "=$=False=$=";
        #endregion

        private static int isInLazyCompile;
        public static CompilerSetting compilerSetting;
        private static int maxRecCall = 100;
        private static bool muxAdding;
        private static bool callStackDisplay;
        private static File currentParsedFile;
        private static int totalCodeLines;
        private static int totalCodeFiles;
        private static long totalCodeChar;
        private static long totalCodeCharNoLib;

        private Compiler() { }

        public static List<File> compile(CompilerCore core, string project, List<File> codes, List<File> resources, Debug debug,
                                            CompilerSetting setting, ProjectVersion version, string pctFolder)
        {
            DateTime startTime = DateTime.Now;

            callTrace = "digraph " + project + " {\nmain\nload\nhelper\n";
            Core = core;
            projectVersion = version;
            muxAdding = false;
            totalCodeLines = 0;
            totalCodeFiles = 0;
            totalCodeChar = 0;
            totalCodeCharNoLib = 0;
            for (int i = 0; i < 11; i++)
            {
                pow64[i] = IntPow(alphabet.Length, i);
            }
            foreach (File f in resources)
            {
                f.content = f.content.Replace("\r", "");
            }
            foreach (File f in codes)
            {
                f.content = f.content.Replace("\r", "");
                totalCodeCharNoLib += f.content.Length;
            }
            dirVar = project.Substring(0, Math.Min(4, project.Length));

            compilerSetting = setting;


            offuscationMap = new Dictionary<string, string>();
            classOffuscationMap = new Dictionary<string, string>();
            offuscationSet = new HashSet<string>();

            foreach (string key in setting.forcedOffuscation.Keys)
            {
                OffuscationMapAdd(key, setting.forcedOffuscation[key]);
            }

            functionTags = new Dictionary<string, List<string>>();
            functionTags.Add("ticking", new List<string>());
            functionTags.Add("loading", new List<string>());
            projectFolder = pctFolder;
            GlobalDebug = debug;
            Project = project;
            funcDef = new List<string>();
            funcDefM = new List<string>();
            funcDefL = new List<string>();
            funcDefF = new List<string>();
            varWord = new Dictionary<string, List<string>>();
            objectFunc = new Dictionary<string, List<string>>();

            structGenerating = false;

            try
            {
                Variable.INIT();
                Switch.INIT();
                If.INIT();
                While.INIT();
                For.INIT();
                Forgenerate.INIT();
                With.INIT();
                At.INIT();
                Lambda.INIT();

                scoreboards = new Dictionary<string, Scoreboard>();
                functions = new Dictionary<string, List<Function>>();
                variables = new Dictionary<string, Variable>();
                enums = new Dictionary<string, Enum>();
                structs = new Dictionary<string, Structure>();
                predicates = new Dictionary<string, List<Predicate>>();
                switches = new Stack<Switch>();
                constants = new Dictionary<int, Variable>();
                constantsF = new Dictionary<int, Variable>();
                functDelegated = new Dictionary<string, List<Function>>();
                functDelegatedFile = new Dictionary<string, List<File>>();
                classOffuscationSet = new HashSet<string>();
                imported = new HashSet<string>();
                thisDef = new Stack<string>();
                files = new List<File>();
                jsonFiles = new List<File>();
                resourcespackFiles = new List<File>();
                stringSet = new List<string>();
                packages = new List<string>();
                adjPackage = new Stack<string>();
                structStack = new Stack<Structure>();
                blockTags = new Dictionary<string, TagsList>();
                entityTags = new Dictionary<string, TagsList>();
                itemTags = new Dictionary<string, TagsList>();
                resourceFiles = new Dictionary<string, string>();
                dedupFiles = new Dictionary<int, List<File>>();
                ExtensionClassStack = new Stack<string>();
                ExtensionMethod = new Dictionary<string, List<Function>>();

                foreach (var f in resources)
                {
                    resourceFiles.Add(f.name, f.content);
                    GlobalDebug("Added resource: " + f.name, Color.Green);
                }

                isInStructMethod = false;
                isInStaticMethod = false;
                forcedUnsed = false;
                structMethodFile = null;

                loadFile = new File("load", "");
                loadFile.AddScoreboardDefLine(Core.LoadBase());
                new Scoreboard(compilerSetting.scoreboardValue, "dummy");
                new Scoreboard(compilerSetting.scoreboardConst, "dummy");
                new Scoreboard(compilerSetting.scoreboardTmp, "dummy");
                variables.Add("__class__", new Variable("__class__", "__class__", Type.INT));
                variables.Add("__class_pointer__", new Variable("__class_pointer__", "__class_pointer__", Type.INT));
                variables.Add("__CLASS__", new Variable("__CLASS__", "__CLASS__", Type.INT, true));

                files.Add(loadFile);
                mainFile = new File("main", "");
                mainFile.AddScoreboardDefLine(Core.MainBase());
                files.Add(mainFile);

                stringInit();

                compileFiles(codes);

                FunctionCreate();
                ConstCreate();
                ScoreboardCreate();
                StringPoolCreate();
                MuxClose();

                if (setting.opti_FunctionTagsReplace)
                    FunctionOptimisation();

                loadFile.Close();
                mainFile.Close();

                updateFormater();

                List<File> returnFiles = new List<File>();
                int lineCount = 0;
                long charCount = 0;

                foreach (File f in files)
                {
                    if (!compilerSetting.removeUselessFile)
                    {
                        f.content += $"not used:{f.notUsed}, f.isLazy: {f.isLazy}, line: {f.lineCount} valid: {f.valid}\n";
                        f.content += f.parsed.Count > 0 ? f.parsed.Aggregate((x, y) => x + "\n" + y) : "";
                        returnFiles.Add(f);
                    }
                    else if (!f.notUsed && !f.isLazy && f.lineCount > 0 && f.valid)
                    {
                        returnFiles.Add(f);
                        foreach (string line in f.content.Split('\n'))
                        {
                            if (line != "" && !line.StartsWith("#"))
                            {
                                lineCount++;
                                charCount += f.content.Length;
                            }
                        }
                    }
                }

                GlobalDebug("================[Datapack Stats]================", Color.LimeGreen);
                GlobalDebug("\t" + totalCodeFiles.ToString() + " TBMS Files", Color.LimeGreen);
                GlobalDebug("\t" + totalCodeLines.ToString() + " TBMS Lines", Color.LimeGreen);
                GlobalDebug("\t" + totalCodeChar.ToString() + " TBMS Char", Color.LimeGreen);
                GlobalDebug("\t" + returnFiles.Count.ToString() + " MCfunction Files", Color.LimeGreen);
                GlobalDebug("\t" + lineCount.ToString() + " MC Commands", Color.LimeGreen);
                GlobalDebug("\t" + charCount.ToString() + " MC Char", Color.LimeGreen);
                GlobalDebug("\t" + jsonFiles.Count.ToString() + " Json files", Color.LimeGreen);
                GlobalDebug("\t" + variables.Count.ToString() + " TBMS variables", Color.LimeGreen);
                GlobalDebug("\t" + functions.Count.ToString() + " TBMS functions", Color.LimeGreen);
                GlobalDebug("\t" + (((double)charCount) / totalCodeChar).ToString() + " Ratio MC/TBMS (With Lib)", Color.LimeGreen);
                GlobalDebug("\t" + (((double)charCount) / totalCodeCharNoLib).ToString() + " Ratio MC/TBMS (No Lib)", Color.LimeGreen);
                GlobalDebug("================================================", Color.LimeGreen);

                foreach (File item in jsonFiles)
                {
                    if (!item.notUsed)
                    {
                        returnFiles.Add(item);
                    }
                }

                callTrace += "}";

                GlobalDebug("Total Compile Time: " + (DateTime.Now - startTime).TotalMilliseconds.ToString() + "ms", Color.Aqua);

                return returnFiles;
            }
            catch (Exception e)
            {
                GlobalDebug("Dumped Core", Color.Yellow);
                updateFormater();
                throw new Exception("Error in " + currentFile + " on line " + currentLine.ToString() + ": " + e.ToString());
            }
        }
        public static string getStackCall(CompilerCore core, string project, List<File> codes, List<File> resources, Debug debug, CompilerSetting setting, ProjectVersion version, string pctFolder)
        {
            callStackDisplay = true;
            compile(core, project, codes, resources, debug, setting, projectVersion, pctFolder);
            callStackDisplay = false;
            return callTrace;
        }

        private static void compileFiles(List<File> files, bool notUsed = false, bool import = false)
        {
            List<Task<string[]>> tasks = new List<Task<string[]>>();
            foreach (File f in files)
            {
                tasks.Add(Task<string[]>.Factory.StartNew(() => (f.valid) ? desugar(f.content).Split('\n') : null));
            }
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].valid)
                {
                    currentParsedFile = files[i];
                    compileFile(files[i], tasks[i].Result, notUsed, import);
                }
            }
            currentParsedFile = null;
        }
        private static void compileFile(File f, string[] desugaredContent, bool notUsed = false, bool import = false)
        {
            string preFile = currentFile;
            structStack = new Stack<Structure>();
            packageMap = new Dictionary<string, string>();
            LastConds = new Stack<If>();
            lazyOutput = new Stack<List<Variable>>();
            lazyCall = new Stack<Function>();
            lazyEvalVar = new List<Dictionary<string, string>>();
            typeMaps = new Stack<Dictionary<string, string>>();
            attributes = new List<string>();
            LastCond = new If(-1);
            currentFile = f.name;
            currentLine = 1;
            inGenericStruct = false;
            isInFunctionDesc = false;
            isInStaticMethod = false;
            isInLazyCompile = 0;
            jsonIndent = 0;
            totalCodeFiles++;
            totalCodeChar += f.content.Length;
            if (!varWord.ContainsKey(currentFile))
            {
                varWord[currentFile] = new List<string>();
                objectFunc[currentFile] = new List<string>();
            }

            ParenthiseError parenthiseError = checkParenthisation(f.content.Split('\n'));
            if (parenthiseError != null)
                parenthiseError.throwException();

            File fFile = new File(f.name);
            fFile.notUsed = notUsed;
            files.Add(fFile);

            bool isMcFunction = false;

            if (f.name == "main" && !import)
            {
                context = new Context(Project, mainFile);
                callingFunctName = "main";
            }
            else if (f.name == "load" && !import)
            {
                context = new Context(Project, loadFile);
                callingFunctName = "load";
            }
            else
            {
                context = new Context(Project, f.name, fFile);
                callingFunctName = "load";
            }

            string[] lines = desugaredContent;

            currentPackage = fFile.name;
            adjPackage = new Stack<string>();

            if (f.name.EndsWith(".mcfunction"))
            {
                context = new Context(Project, mainFile);
                isMcFunction = true;
                preparseLine("def " + f.name.Substring(0, f.name.IndexOf('.')) + "(){");
            }

            foreach (string line2 in lines)
            {
                preparseLine((isMcFunction ? "/" : "") + line2);

                currentLine += 1;
            }
            if (isMcFunction)
            {
                preparseLine("}");
            }
            if (inGenericStruct)
                context.currentFile().Close();

            //getEnvironment(fFile.name);
            fFile.Close();
            if (fFile.lineCount == 0)
            {
                files.Remove(fFile);
            }
            else
            {
                if (!f.resourcespack)
                {
                    if (fFile.name != "load" && fFile.name != "main" && !import)
                    {
                        loadFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                    }
                    else
                    {
                        if (import)
                        {
                            loadFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                        }
                    }
                }
            }
            
            totalCodeLines += currentLine - 1;
            GlobalDebug("Succefully Compiled " + currentFile + " (" + (currentLine - 1).ToString() + " Lines)", Color.Lime);
            currentFile = preFile!=null?preFile:"____";
        }

        public static void updateFormater()
        {
            try
            {
                Formatter.setEnum(new List<string>(enums.Keys));
                List<string> formEnums = new List<string>();

                foreach (Enum s in enums.Values)
                {
                    foreach (Enum.EnumValue v in s.values)
                    {
                        formEnums.Add(v.value);
                    }
                }
                Formatter.setEnumValue(formEnums.Distinct().ToList());
                Formatter.setStructs(structs.Keys.Where(x => !structs[x].isStatic).ToList());
                Formatter.setpackage(packages.Concat(structs.Keys.Where(x => structs[x].isStatic)).Distinct().ToList());
                Formatter.setTags(functionTags.Keys.ToList());
                Formatter.setDefWord(funcDef);
                Formatter.defWordMore1F = funcDefF.Distinct().ToList();
                Formatter.defWordMore1L = funcDefL.Distinct().ToList();
                Formatter.defWordMore1M = funcDefM.Distinct().ToList();
                Formatter.objectFunc = objectFunc;
                Formatter.varWord = varWord;
                Formatter.loadDict();
            }
            catch { }
        }

        public static void preparseLine(string line2, File limit = null, bool lazyEval = false)
        {
            string line = line2;

            if (context.compVal.Count > 0 && line.Contains("$") && !(dualCompVar.Match(line).Success && structInstCompVar) && !defineReg.Match(line).Success)
            {
                line = compVarReplace(line);
            }

            if (jsonIndent > 0 && isInLazyCompile == 0)
            {
                parseLine(line);
            }
            else
            {
                line = smartExtract(line);
                if (line != "")
                {
                    if (!lazyEval)
                    {
                        if (inGenericStruct)
                            structStack.Peek().genericFile.addParsedLine(line);
                    }

                    if (isInLazyCompile > 0)
                    {
                        if (smartContains(line, '{'))
                            isInLazyCompile += 1;
                        if (smartContains(line, '}'))
                            isInLazyCompile -= 1;
                    }

                    if (isInLazyCompile > 0 && !inGenericStruct)
                    {
                        if (isInStructMethod)
                        {
                            structMethodFile.addParsedLine(line);
                        }
                        else
                        {
                            context.currentFile().addParsedLine(line);
                        }
                    }

                    string res;
                    if (isInLazyCompile == 0)
                    {
                        res = "";
                        if (smartContains(line, ';'))
                        {
                            foreach (string subline in smartSplit(line, ';'))
                            {
                                res += parseLine(subline) + "\n";
                            }
                        }
                        else
                        {
                            res = parseLine(line);
                        }
                    }
                    else
                    {
                        res = "";
                    }


                    if (res != "")
                        context.currentFile().AddLine(res);

                    if ((line == "}" || autoIndented == 1) && !inGenericStruct && isInLazyCompile == 0 && limit != context.currentFile())
                    {
                        context.currentFile().Close();
                    }
                    if (autoIndented > 0)
                    {
                        autoIndented--;
                    }
                }
            }
        }
        public static string parseLine(string text)
        {
            text = evalDesugar(text);

            try
            {
                text = smartExtract(text);

                if (isInFunctionDesc)
                {
                    return InstFuncDesc(text);
                }
                if (jsonIndent == 0 && text.StartsWith("//"))
                {
                    return InstComment(text, true);
                }
                if (jsonIndent == 0 && text.StartsWith("/"))
                {
                    return text.Substring(1, text.Length - 1);
                }
                if (jsonIndent == 0 && text.StartsWith("import"))
                {
                    return import(text);
                }
                if (jsonIndent == 0 && text.StartsWith("using"))
                {
                    return instUsing(text);
                }
                if (jsonIndent == 0 && text.StartsWith("alias"))
                {
                    return InstAlias(text);
                }
                if (jsonIndent == 0 && text.StartsWith("package"))
                {
                    return InstPackage(text);
                }
                if (jsonIndent == 0 && text.Contains("\"\"\""))
                {
                    return InstFuncDesc(text);
                }
                if (jsonIndent == 0 && text.StartsWith("{"))
                {
                    autoIndented = 0;
                    return parseLine(text.Substring(1, text.Length - 1));
                }
                if (requireReg.Match(text).Success)
                {
                    return Require(text);
                }
                if (indexedReg.Match(text).Success)
                {
                    return Indexed(text);
                }
                //return
                if ((text.StartsWith("return") && text.Contains("(") && text.Contains(")")) || text.StartsWith("return "))
                {
                    string[] args;
                    if (text.Contains("(") && text.Contains(")"))
                    {
                        args = getArgs(text);
                    }
                    else
                    {
                        args = smartSplit(text.Substring(7, text.Length - 7), ',');
                    }

                    return functionReturn(args);
                }
                if (jsonIndent == 0 && jsonFileReg.Match(text).Success)
                {
                    return InstJsonFile(text);
                }
                if (jsonIndent == 0 && predicateFileReg.Match(text).Success)
                {
                    return InstPredicateFile(text);
                }
                //condition
                if (ifsReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstIf(arg, text, 1);
                }
                if (ifReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstIf(arg, text);
                }
                if (elsifReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstElseIf(arg, text);
                }
                if (elseReg.Match(text).Success)
                {
                    return InstElse(text);
                }
                //class
                if (jsonIndent == 0 && classReg.Match(text).Success)
                {
                    return InstStruct(text);
                }
                //class
                if (jsonIndent == 0 && text.StartsWith("extension "))
                {
                    return InstExtension(text);
                }
                //switch
                if (jsonIndent == 0 && switchReg.Match(text).Success)
                {
                    string[] arg = getArgs(text);

                    return InstSwitch(arg, text);
                }
                //case
                if (jsonIndent == 0 && caseReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstCase(arg);
                }
                //smart case
                if (jsonIndent == 0 && text.Contains("->") && switches.Count > 0)
                {
                    return InstSmartCase(text);
                }
                //with
                if (jsonIndent == 0 && withReg.Match(text).Success)
                {
                    string[] args = getArgs(text);

                    return InstWith(args, text);
                }
                //at
                if (jsonIndent == 0 && atReg.Match(text).Success)
                {
                    string[] args = getArgs(text);

                    return InstAt(args, text);
                }
                //positioned
                if (jsonIndent == 0 && positonedReg.Match(text).Success)
                {
                    string args = getArg(text);

                    return InstPositioned(args, text);
                }
                //aligned
                if (jsonIndent == 0 && alignReg.Match(text).Success)
                {
                    string args = getArg(text);

                    return InstAligned(args, text);
                }
                //while
                if (jsonIndent == 0 && whileReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstWhile(arg, text);
                }
                //forgenerate
                if (forgenerateReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstForGenerate(arg, text);
                }
                //for
                if (jsonIndent == 0 && forReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstFor(arg, text);
                }
                //enum set
                if (jsonIndent == 0 && enumReg.Match(text).Success)
                {
                    return InstEnum(text);
                }
                //enum file
                if (jsonIndent == 0 && enumFileReg.Match(text).Success)
                {
                    return InstEnumFile(text);
                }
                //function def
                Match funMatch = funcReg.Match(text);
                if (jsonIndent == 0 && funMatch.Success && illagal_op.All(x => x != funMatch.Groups["function_name"].Value))
                {
                    return InstFunc(text);
                }
                //blocktag set
                if (jsonIndent == 0 && blocktagReg.Match(text).Success)
                { 
                    return InstBlockTag(text);
                }
                //entitytag set
                if (jsonIndent == 0 && entitytagReg.Match(text).Success)
                {
                    return InstEntityTag(text);
                }
                //itemTag set
                if (jsonIndent == 0 && itemtagReg.Match(text).Success)
                {
                    return InstItemTag(text);
                }
                //comp int set
                if (compVarInstReg.Match(text).Success || dualCompVar.Match(text).Success || defineReg.Match(text).Success)
                {
                    return InstCompilerVar(text);
                }
                //int set
                if (jsonIndent == 0 && (varInstReg.Match(text).Success || functionTypeReg.Match(text).Success))
                {
                    return InstVar(text);
                }
                //int add
                if (jsonIndent == 0 && smartContains(text, '='))
                {
                    return modVar(text);
                }
                //int add
                if (jsonIndent == 0 && (text.Contains("++") || text.Contains("--")))
                {
                    return modVar(text.Replace("++", "+=1").Replace("--", "-=1"));
                }
                //function call
                if (text.Contains("(") && text.Contains(")") || context.IsFunction(text) || shortFuncReg.Match(text).Success)
                {
                    return functionEval(text);
                }
                if (jsonIndent == 0 && attributeReg.Match(text).Success)
                {
                    return InstAttribute(text);
                }
                if (jsonIndent > 0)
                {
                    return AddToJsonFile(text);
                }
                
                if (text != "" && !text.StartsWith("}"))
                {
                    GlobalDebug("Unparsed line:'" + text + "'", Color.Yellow);
                }
                return "";
            }
            catch (Exception e)
            {
                if (!inGenericStruct)
                    throw new Exception(text + "\n" + e.ToString());
                else
                    return "";
            }
        }
        public static ParenthiseError checkParenthisation(string[] file)
        {
            Stack<char> chars = new Stack<char>();
            int lineIndex = 0;
            foreach (string line in file)
            {
                lineIndex++;
                bool inComment = false;
                bool inString = false;
                char cPrev = '\n';

                int columns = 0;

                if (!line.Replace(" ", "").StartsWith("/"))
                {
                    foreach (char c in line)
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
                            return new ParenthiseError(lineIndex, columns, c);
                        }
                        else if (c == '(' && !inComment && !inString)
                        {
                            chars.Push(c);
                        }
                        else if (c == ')' && !inComment && !inString && chars.Pop() != '(')
                        {
                            return new ParenthiseError(lineIndex, columns, c);
                        }
                        else if (c == '[' && !inComment && !inString)
                        {
                            chars.Push(c);
                        }
                        else if (c == ']' && !inComment && !inString && chars.Pop() != '[')
                        {
                            return new ParenthiseError(lineIndex, columns, c);
                        }
                        cPrev = c;
                        columns++;
                    }
                }
            }
            if (chars.Count == 0)
                return null;
            else
                return new ParenthiseError(++lineIndex, 0, chars.Pop());
        }

        #region desugar
        public static string regReplace(string text, Match match, string value)
        {
            return text.Substring(0, match.Index) +
                                value +
                                text.Substring(match.Index + match.Length, text.Length - match.Index - match.Length);
        }
        public static string regReplace(string text, MatchCustom match, string value)
        {
            return text.Substring(0, match.Index) +
                                value +
                                text.Substring(match.Index + match.Length, text.Length - match.Index - match.Length);
        }
        public static string compVarReplace(string line)
        {
            if (!requireReg.Match(line).Success && !indexedReg.Match(line).Success)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                List<string> keys = new List<string>();
                foreach (Dictionary<string, string> v in context.compVal)
                {
                    foreach (string key in v.Keys)
                    {
                        if (dic.ContainsKey(key))
                        {
                            dic[key] = v[key];
                        }
                        else
                        {
                            dic.Add(key, v[key]);
                            keys.Add(key);
                        }
                    }
                }

                keys.Sort();
                keys.Reverse();

                foreach (string key in keys)
                {
                    Regex reg;
                    if (compRegexCache.ContainsKey(key))
                    {
                        reg = compRegexCache[key];
                    }
                    else
                    {
                        reg = new Regex(@"\\b" + key + "\\b");
                        if (key.Contains("$"))
                            reg = new Regex("\\" + key);
                        compRegexCache[key] = reg;
                    }

                    Match match = reg.Match(line);
                    while (match != null && dic[key] != key && match.Value != "" && match.Value != null)
                    {
                        line = regReplace(line, match, dic[key]);
                        match = reg.Match(line);
                    }
                }
            }
            return line;
        }
        public static void compVarChange(string name, string value)
        {
            Dictionary<string, string> dic = null;

            foreach (Dictionary<string, string> v in context.compVal)
            {
                if (v.ContainsKey(name))
                {
                    dic = v;
                }
            }
            if (dic == null)
                throw new Exception("Comp Var Not found: " + name);
            dic[name] = value;
        }
        public static string[] addCompVar(Argument a, string[] args, int i)
        {
            if (a.type == Type.ENTITY)
            {
                if (!context.isEntity(smartExtract(args[i])))
                {
                    throw new Exception("Entity is required!");
                }
                context.compVal[context.compVal.Count - 1].Add(a.name, context.GetEntitySelector(smartExtract(args[i])));
            }
            else if (a.type == Type.INT || a.type == Type.FUNCTION || a.type == Type.FLOAT)
            {
                Variable valVar = GetVariableByName(smartExtract(args[i]), true);
                if (valVar != null)
                {
                    context.compVal[context.compVal.Count - 1].Add(a.name + ".enums", valVar.enums);
                    context.compVal[context.compVal.Count - 1].Add(a.name + ".type", valVar.GetTypeString());
                    context.compVal[context.compVal.Count - 1].Add(a.name + ".name", valVar.gameName);
                    context.compVal[context.compVal.Count - 1].Add(a.name + ".scoreboard", valVar.scoreboard());
                    context.compVal[context.compVal.Count - 1].Add(a.name + ".scoreboardname", valVar.scoreboard().Split(' ')[1]);
                }
                if (a.type == Type.FUNCTION)
                {
                    try
                    {
                        context.compVal[context.compVal.Count - 1].Add(a.name + ".name", functions[context.GetFunctionName(smartExtract(args[i]))][0].gameName);
                    }
                    catch { }
                }
                context.compVal[context.compVal.Count - 1].Add(a.name, smartExtract(args[i]));
            }
            else if (a.type == Type.JSON)
            {
                string[] json = Compiler.Core.FormatJson(args, context, i);
                string init = json[1];
                string clear = json[2];
                context.compVal[context.compVal.Count - 1].Add(a.name, json[0]);
                return new string[] { init, clear };
            }
            else if (a.type == Type.PARAMS)
            {
                string param = "(";
                int c = 0;
                for (int j = i; j < args.Length; j++)
                {
                    param += args[j] + ",";
                    c++;
                }

                context.compVal[context.compVal.Count - 1].Add(a.name, param + ")");
                context.compVal[context.compVal.Count - 1].Add(a.name + ".count", c.ToString());
            }
            else if (a.type == Type.STRING)
                context.compVal[context.compVal.Count - 1].Add(a.name, smartExtract(args[i]));
            else
            {
                Variable valVar = GetVariableByName(smartExtract(args[i]));
                context.compVal[context.compVal.Count - 1].Add(a.name + ".scoreboard", valVar.scoreboard());
                context.compVal[context.compVal.Count - 1].Add(a.name + ".enums", valVar.enums);
                context.compVal[context.compVal.Count - 1].Add(a.name + ".type", valVar.GetTypeString());
                context.compVal[context.compVal.Count - 1].Add(a.name + ".name", valVar.gameName);
                context.compVal[context.compVal.Count - 1].Add(a.name, valVar.gameName);
            }
            return null;
        }
        public static string desugar(string text)
        {
            text = desugarParenthis(text).Replace("$projectName", Project.ToLower())
                                         .Replace("$projectVersion", projectVersion.ToString());
            Match match;
            text = ifelseDetect(text);

            match = enumsDesugarReg.Match(text);
            while (match != null && match.Value != "")
            {
                text = regReplace(text, match,
                    match.Value.Replace("{", "=").Replace("\n", "").Replace("}", ""));

                match = enumsDesugarReg.Match(text);
            }

            match = blocktagsDesugarReg.Match(text);
            while (match != null && match.Value != "")
            {
                text = regReplace(text, match,
                    match.Value.Replace("{", "=").Replace("\n", "").Replace("}", ""));

                match = blocktagsDesugarReg.Match(text);
            }

            match = entitytagsDesugarReg.Match(text);
            while (match != null && match.Value != "")
            {
                text = regReplace(text, match,
                    match.Value.Replace("{", "=").Replace("\n", "").Replace("}", ""));

                match = entitytagsDesugarReg.Match(text);
            }

            match = itemtagsDesugarReg.Match(text);
            while (match != null && match.Value != "")
            {
                text = regReplace(text, match,
                    match.Value.Replace("{", "=").Replace("\n", "").Replace("}", ""));

                match = itemtagsDesugarReg.Match(text);
            }

            return text;
        }
        public static string desugarParenthis(string text)
        {
            int ind = 0;
            int parInd = 0;
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            bool inString = false;
            bool ignoreNext = false;
            bool needSplitter = false;
            Stack<bool> isFunkyLamba = new Stack<bool>();
            Stack<bool> isFunkyLamba2 = new Stack<bool>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(' && !inString && !ignoreNext)
                {
                    parInd++;
                    stringBuilder.Append(text[i]);
                    isFunkyLamba2.Push(false);
                }
                else if (text[i] == ')' && !inString && !ignoreNext)
                {
                    parInd--;
                    stringBuilder.Append(text[i]);
                    isFunkyLamba2.Pop();
                }
                else if (text[i] == '{' && !inString && !ignoreNext && parInd > 0)
                {
                    ind++;
                    stringBuilder.Append(text[i]);
                    isFunkyLamba.Push(true);
                    isFunkyLamba2.Push(true);
                    needSplitter = false;
                }
                else if (text[i] == '}' && !inString && !ignoreNext && parInd > 0)
                {
                    ind--;
                    isFunkyLamba.Pop();
                    isFunkyLamba2.Pop();
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == ':' && !inString && !ignoreNext && ind > 0)
                {
                    isFunkyLamba.Pop();
                    isFunkyLamba.Push(false);
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '\\' && !ignoreNext)
                {
                    ignoreNext = true;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '"' && !ignoreNext)
                {
                    if (inString)
                    {
                        stringBuilder.Append(text[i]);
                        inString = false;
                    }
                    else
                    {
                        stringBuilder.Append(text[i]);
                        inString = true;
                    }
                }
                else if ((text[i] == '\n' || text[i] == '\r') && parInd > 0)
                {
                    if (ind > 0)
                    {
                        bool funky = isFunkyLamba2.Peek();
                        foreach (bool b in isFunkyLamba)
                        {
                            funky = b && funky;
                        }
                        if (funky && needSplitter)
                        {
                            stringBuilder.Append(";");
                        }
                    }
                }
                else if (ignoreNext)
                {
                    ignoreNext = false;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == ' ' || text[i] == '\t' || text[i] == '\n' || text[i] == '\r')
                {
                    stringBuilder.Append(text[i]);
                }
                else
                {
                    stringBuilder.Append(text[i]);
                    needSplitter = true;
                }
            }
            return stringBuilder.ToString();
        }
        public static string functionDesugar(string text)
        {
            bool entity = text.ToUpper() == text;
            Match m = functionTypeReg.Match(text);
            while (m.Success)
            {
                string[] part = smartSplit(m.Value.Replace("=>", "§"), '§', 1);
                part[0] = smartExtract(part[0]).Replace("§", "=>");
                part[1] = smartExtract(part[1]).Replace("§", "=>");

                if (!part[0].StartsWith("(")) { part[0] = "(" + part[0] + ")"; }
                if (!part[1].StartsWith("(")) { part[1] = "(" + part[1] + ")"; }
                text = regReplace(text, m, (entity ? "FUNCTION<" : "function<") + part[0].ToLower() + "," + part[1].ToLower() + ">");
                m = functionTypeRegRelaxed.Match(text);
            }
            return text;
        }
        public static string evalDesugar(string text)
        {
            Match match2 = regEval.Match(text);
            while (match2 != null && match2.Value != "" && match2.Value != null)
            {
                text = regReplace(text, match2, Calculator.Calculate(getArg(match2.Value)).ToString());
                match2 = regEval.Match(text);
            }

            match2 = regEval2.Match(text);
            while (match2 != null && match2.Value != "" && match2.Value != null)
            {
                text = regReplace(text, match2, Calculator.Calculate(getArg(match2.Value)).ToString());
                match2 = regEval2.Match(text);
            }

            match2 = forgenInLineReg.Match(text);
            while (match2 != null && match2.Value != "" && match2.Value != null)
            {
                text = regReplace(text, match2, InstInLineForgenerate(match2.Value));
                match2 = forgenInLineReg.Match(text);
            }
            return text;
        }
        public static string ifelseDetect(string text)
        {
            string lastWord = "";
            int startIndent = 0;
            int start = 0;
            int indent = 0;
            bool isInString = false;
            int i = 0;
            Stack<int> ifIndex = new Stack<int>();
            Stack<int> ifIndentIndex = new Stack<int>();
            List<int> ifsIndex = new List<int>();

            foreach (char c in text)
            {
                if (c == ' ' || c == '\t' || c == '\n')
                {
                    lastWord = "";
                }
                else if (c == '{' && !isInString)
                {
                    indent++;
                    lastWord = "";
                }
                else if (c == '}' && !isInString)
                {
                    indent--;
                    lastWord = "";
                    if (ifIndentIndex.Count > 0 && ifIndentIndex.Peek() == indent)
                    {
                        startIndent = ifIndentIndex.Pop();
                        start = ifIndex.Pop();
                    }
                }
                else if (c == '"')
                {
                    isInString = !isInString;
                    lastWord = "";
                }
                else if (!isInString)
                {
                    lastWord += c;
                }
                if (lastWord == "if")
                {
                    ifIndentIndex.Push(indent);
                    ifIndex.Push(i + 1);
                }
                if (lastWord == "else" && startIndent == indent)
                {
                    ifsIndex.Add(start);
                }
                i++;
            }
            ifsIndex.Sort();
            for (int j = ifsIndex.Count - 1; j >= 0; j--)
            {
                text = text.Insert(ifsIndex[j], "s");
            }

            return text;
        }
        #endregion

        #region import
        public static string import(string text)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";

            text = smartExtract(text.Replace("import", "").Replace(" ", "").Replace(".", "/"));
            bool fu = true;
            if (text.EndsWith("/*"))
            {
                fu = false;
                text = text.Replace("/*", "");
            }
            if (imported.Contains(text))
            {
                return "";
            }
            string output = "";
            string subPath = compilerSetting.libraryFolder[0];
            string libpath = subPath.StartsWith("./") ? (path + subPath.Replace("./", "")) : subPath;
            int pathIndex = 0;
            bool fail = true;
            while ((fail = !tryImport(libpath, text, fu, out output)) && pathIndex < compilerSetting.libraryFolder.Count)
            {
                pathIndex++;
                if (pathIndex < compilerSetting.libraryFolder.Count)
                {
                    subPath = compilerSetting.libraryFolder[pathIndex];
                    libpath = subPath.StartsWith("./") ? (path + subPath.Replace("./", "")) : subPath;
                }
            }

            if (fail && !tryImport(projectFolder + "/lib/", text, fu, out output))
            {
                throw new Exception($"Unknown library: \"{text}\"");
            }

            return "";
        }
        public static bool tryImport(string folder, string lib, bool fu, out string msg)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            string[] split = lib.Split('.');
            List<string> paths = new List<string>();
            paths.Add(split[0]);
            for (int i = 1; i < lib.Length; i++)
            {
                paths.Add(paths[paths.Count - 1] + "." + lib[i]);
            }
            paths.Reverse();
            foreach (string text in paths)
            {
                if (System.IO.File.Exists(folder + text + ".tbms"))
                {
                    string fileName = lib.Replace(text, "");
                    Context c = context;
                    context = new Context(Project, new File("", ""));

                    forcedUnsed = fu;
                    string data = System.IO.File.ReadAllText(folder + text + ".tbms");
                    ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);
                    msg = "#Using TBMS Library: " + text + " v." + project.version.ToString() + "\n";

                    if (project.resources != null)
                    {
                        foreach (var f in project.resources)
                        {
                            if (!resourceFiles.ContainsKey(f.name))
                                resourceFiles.Add(f.name, f.content.Replace("\r", ""));
                        }
                    }

                    List<File> nFiles = new List<File>();
                    foreach (var file in project.files)
                    {
                        if (fileName == "" || fileName == file.name)
                        {
                            nFiles.Add(new File(text + "." + file.name, file.content.Replace("\r", "")));
                        }
                    }
                    compileFiles(nFiles, fu, true);

                    imported.Add(text);
                    forcedUnsed = false;

                    context = c;

                    return true;
                }
                if (System.IO.File.Exists(folder + text + ".dpo"))
                {
                    Dictionary<string, List<string>> save = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(System.IO.File.ReadAllText(folder + text + ".dpo"));
                    if (save["version"][0] == "0" && System.IO.File.Exists(folder + text + ".zip") && System.IO.File.GetLastWriteTime(folder + text + ".zip").ToString() == save["version"][1])
                    {
                        save["functions"].ForEach(x => preparseLine($"def external {x}()"));
                        msg = "#Using TBMS Library: " + text + " (datapack object)\n";
                        return true;
                    }
                }
                if (System.IO.File.Exists(folder + text + ".zip"))
                {
                    if (Directory.Exists(path+ "unzip")) Directory.Delete(path + "unzip", true);
                    ZipFile.ExtractToDirectory(folder + text + ".zip", path + "unzip");
                    List<string> functions = Directory.EnumerateFiles(path + "unzip", "*.mcfunction", SearchOption.AllDirectories)
                             .Select(x => x.Replace("\\","/").Replace(path.Replace("\\", "/") + "unzip/data", "").Replace("functions/", "").Replace("/",".").Replace(".mcfunction",""))
                             .Where(x => !x.Contains("__"))
                             .ToList();
                    functions.ForEach(x => preparseLine($"def external {x}()"));
                    
                    Dictionary<string, List<string>> save = new Dictionary<string, List<string>>();
                    save.Add("version", new List<string>() { "0", System.IO.File.GetLastWriteTime(folder + text + ".zip").ToString() });
                    save.Add("functions", functions);
                    System.IO.File.WriteAllText(folder + text + ".dpo", JsonConvert.SerializeObject(save));


                    Directory.Delete(path + "unzip", true);
                    
                    msg = "#Using TBMS Library: " + text + " (datapack)\n";
                    return true;
                }
            }

            msg = "";
            return false;
        }
        public static string instUsing(string text)
        {
            try
            {
                InstAlias(text);
            }
            catch
            {
                string[] part = text.Replace("using", "").Replace(" as ", "/").Replace(" ", "").Split('/');
                if (!packages.Contains(smartEmpty(part[0])))
                    throw new Exception("Unknown Package " + part[0]);

                packageMap.Add(smartEmpty(part[1]), smartEmpty(part[0]));
            }
            return "";
        }
        #endregion

        #region variable
        private static void AddVariable(string key, Variable variable)
        {
            if (variables.ContainsKey(key))
                throw new Exception(key + " already defined!");

            variables.Add(key, variable);
        }
        public static Variable GetVariable(string key)
        {
            if (variables.ContainsKey(key))
            {
                return variables[key];
            }
            throw new Exception(key + " not in dic.");
        }
        public static Variable GetVariableByName(string name, bool safe = false)
        {
            if (name.StartsWith("@") && context.isEntity(smartSplitJson(name, '.', 1)[0]))
            {
                string[] val = smartSplitJson(name, '.', 1);
                if (val.Length == 2)
                {
                    var tmp = GetVariableByName(val[1], true);
                    if (tmp == null && safe)
                    {
                        return null;
                    }
                    return tmp.Select(val[0]);
                }
            }
            string key = context.GetVariable(name, safe);
            if (key == null && safe)
            {
                return null;
            }
            if (variables.ContainsKey(key))
            {
                return variables[key];
            }
            if (safe)
            {
                return null;
            }
            throw new Exception(key + " not in dic.");
        }
        public static Variable GetConstant(int value)
        {
            if (!constants.ContainsKey(value))
            {
                string name = "c." + value.ToString();
                Variable var = new Variable(name, name, Type.INT, false);
                var.isConst = true;
                variables.Add(name, var);
                loadFile.AddStartLine(Core.VariableOperation(var, value, "="));
                constants.Add(value, var);
            }
            return constants[value];
        }
        public static Variable GetConstant(double value)
        {
            int inValue = (int)(value * compilerSetting.FloatPrecision);
            if (!constantsF.ContainsKey(inValue))
            {
                string name = $"c.{value}f";
                Variable var = new Variable(name, name, Type.FLOAT, false);
                var.isConst = true;
                variables.Add(name, var);
                loadFile.AddStartLine(Core.VariableOperation(var, inValue, "="));
                constantsF.Add(inValue, var);
            }
            return constantsF[inValue];
        }
        public static Variable GetConstant(Variable v, string value)
        {
            int intValue;
            double floatValue;
            if (int.TryParse(value, out intValue))
            {
                return GetConstant(intValue);
            }
            else if (double.TryParse(value, out floatValue))
            {
                return GetConstant(floatValue);
            }
            else if (v != null && v.type == Type.FUNCTION)
            {
                string name = context.GetFunctionName(value);

                return GetConstant(GetFunctionIndex(v, GetFunction(name, v.args)));
            }
            else if (v != null && v.type == Type.ENUM)
            {
                return GetConstant(enums[v.enums].IndexOf(value));
            }
            else
            {
                Context c = context;
                context = new Context(Project, new File("",""));
                string name = $"c.{context.GetVar()}{Variable.GetID(context.GetVar())}";
                preparseLine($"val .{name} = {value}");
                var var = GetVariable(name);
                var.use();
                context = c;
                return var;
            }
        }
        #endregion

        #region lazy val
        public static string getLazyVal(string val)
        {
            foreach (var lst in lazyEvalVar)
            {
                if (lst.ContainsKey(val))
                {
                    return lst[val];
                }
            }
            return null;
        }
        public static void addLazyVal(string key, string val)
        {
            lazyEvalVar[lazyEvalVar.Count - 1].Add(key, val);
        }
        public static void popLazyVal()
        {
            lazyEvalVar.RemoveAt(lazyEvalVar.Count - 1);
        }
        public static bool containLazyVal(string val)
        {
            return getLazyVal(val) != null;
        }
        public static List<Dictionary<string,string>> GetLazyValStack()
        {
            return lazyEvalVar?.ToList();
        }
        public static void SetLazyValStack(List<Dictionary<string, string>> stack)
        {
            lazyEvalVar = stack;
        }
        #endregion

        #region eval
        public static string modVar(string text)
        {
            if (text.StartsWith("$"))
                return InstCompilerVar(text);

            string op = "";
            string[] left;
            string[] right;

            op = opReg.Match(text).Value;

            if (op == "=")
            {
                string[] splitted = smartSplit(text, '=', 1);
                left = smartSplit(splitted[0], ',').Select(x => smartExtract(x)).ToArray();
                right = smartSplit(splitted[1], ',').Select(x => smartExtract(x)).ToArray();
            }
            else if (op == ":=")
            {
                string[] splitted = smartSplit(text.Replace(op, "="), '=', 1);
                left = smartSplit(splitted[0], ',').Select(x => smartExtract(x)).ToArray();
                right = smartSplit(splitted[1], ',').Select(x => smartExtract(x)).ToArray();

                string output = "";
                for (int i = 0; i < left.Length; i++)
                {
                    output += parseLine("if (" + left[i] + "==null){" + left[i] + "=" + right[i % right.Length] + "}") + "\n";
                }
                return output;
            }
            else
            {
                string[] splitted = smartSplit(text.Replace(op, "="), '=', 1);
                left  = smartSplit(splitted[0], ',').Select(x => smartExtract(x)).ToArray();
                right = smartSplit(splitted[1], ',').Select(x => smartExtract(x)).ToArray();
            }

            if (right[0].Contains("(") && context.IsFunction(right[0].Substring(0, right[0].IndexOf('(')))
                && !smartContains(right[0], '+') && !smartContains(right[0], '-') && !smartContains(right[0], '*')
                && !smartContains(right[0], '%') && !smartContains(right[0], '/') && !smartContains(right[0], '|')
                && !smartContains(right[0], '&') && !smartContains(right[0], '^'))
            {
                if (curriedReg.Match(right[0]).Success)
                {
                    return Decurry(right[0], left, op);
                }
                return functionEval(right[0], left, op);
            }
            else
            {
                string output = "";

                if (left.Length == 1 && right.Length > 1)
                {
                    if (arrayVarReg.Match(left[0]).Success && !context.isEntity(left[0]))
                    {
                        var mat = getArrayBlock(left[0]);
                        string vara = regReplace(left[0], mat, "");
                        string para = mat.Value.Substring(1, mat.Value.Length - 2);
                        return parseLine($"{smartExtract(vara)}.set({para},{right.Aggregate((x, y) => x + "," + y)})");
                    }

                    string rights = smartExtract(smartSplit(text, '=', 1)[1]);
                    Variable var;
                    string[] splitted = smartSplit(left[0], '.').Select(x => smartExtract(x)).ToArray();
                    if (context.isEntity(left[0]) && splitted.Length == 2)
                    {
                        var = new Variable(context.GetEntityName(left[0]), left[0].Split('.')[1], Type.ENTITY_COMPONENT);
                    }
                    else if (left[0].StartsWith("@"))
                    {
                        for (int i = 0; i < right.Length; i++)
                        {
                            string funcName = context.GetFunctionName(right[i]);
                            Function fun = GetFunction(funcName, new List<Argument>());
                            AddToFunctionTag(fun, left[0].Substring(1, left[0].Length - 1));
                        }
                        return "";
                    }
                    else
                    {
                        var = GetVariableByName(left[0]);
                    }

                    if (right.Length > 1)
                        output += eval(rights, var, var.type, op);
                    else
                        output += eval(rights, var, var.type, op);
                    return output;
                }
                for (int i = 0; i < left.Length; i++)
                {
                    if (arrayVarReg.Match(left[i]).Success && !context.isEntity(left[i]))
                    {
                        var mat = getArrayBlock(left[i]);
                        string vara = regReplace(left[i], mat, "");
                        string para = mat.Value.Substring(1, mat.Value.Length - 2);
                        output += parseLine($"{smartExtract(vara)}.set({para},{(right.Length > 1 ? right[i] : right[0])})");
                        continue;
                    }

                    Variable var;
                    string[] splitted = smartSplit(left[i], '.').Select(x => smartExtract(x)).ToArray();
                    if (context.isEntity(left[i]) && splitted.Length == 2)
                    {
                        var = new Variable(context.GetEntityName(left[i]), splitted[1], Type.ENTITY_COMPONENT);
                    }
                    else if (left[i].StartsWith("@"))
                    {
                        string funcName = context.GetFunctionName(right.Length > 1 ? right[i] : right[0]);
                        Function fun = GetFunction(funcName, new List<Argument>());
                        AddToFunctionTag(fun, left[i].Substring(1, left[i].Length - 1));
                        return "";
                    }
                    else
                    {
                        var = GetVariableByName(left[i]);
                    }

                    if (right.Length > 1)
                        output += eval(right[i], var, var.type, op);
                    else
                        output += eval(right[0], var, var.type, op);
                }
                return output;
            }
        }
        private static string eval(string val, Variable variable, Type ca, string op = "=", int recCall = 0)
        {
            variable.use();
            if (variable.isConst && variable.wasSet)
                throw new Exception("Cannot moddify Constant!");
            variable.wasSet = true;

            if (recCall > maxRecCall)
            {
                throw new Exception("Stack Overflow");
            }


            if (containLazyVal(val))
            {
                return eval(getLazyVal(val), variable, ca, op, recCall + 1);
            }
            if (curriedReg.Match(val).Success)
            {
                return Decurry(val, variable, ca, op);
            }
            if (context.IsFunction(val.Replace(" ", "")) && !val.Contains("(") && context.GetVariable(val, true) == null
                && ca != Type.FUNCTION && !(ca == Type.ENUM && enums[variable.enums].Contains(val.ToLower())))
            {
                return eval(val + "()", variable, ca, op, recCall + 1);
            }
            string output = "";

            int tmpI;
            double tmpF;
            string[] spaceSplitted = smartSplitJson(val, ' ');
            
            bool containsOP = (spaceSplitted.Length > 2 && (operators_base.Any(x => spaceSplitted[1] == x))||
                                (spaceSplitted.Length <= 2 && operators_base.Any(x => smartContains(val,x[0]))));
            float simflied = 0;
            Variable valVar = GetVariableByName(smartEmpty(val), true);

            if (nullReg.Matches(val).Count > 0)
            {
                if (op != "=")
                    throw new Exception("Invalid Operation " + op + " with null");
                if (ca != Type.STRUCT && ca != Type.FUNCTION)
                    return Core.VariableSetNull(variable);
                if (ca == Type.FUNCTION)
                    return Core.VariableOperation(variable, -1, "=");
                else
                {
                    Structure stru1 = structs[variable.enums];
                    if (stru1.isClass)
                    {
                        Structure.DerefObject(variable);
                        return Core.VariableSetNull(variable);
                    }
                    else
                    {
                        foreach (Variable struV in structs[variable.enums].fields)
                        {
                            output += parseLine(variable.gameName + "." + struV.name + "= null");
                        }
                    }
                }
            }
            else if (val.Contains("?") && val.Contains(":"))
            {
                string[] a = val.Split('?');
                string[] b = a[1].Split(':');
                string cond = getCondition(a[0]);
                string invCond = cond.Contains("if") ? cond.Replace("if", "unless") : cond.Replace("unless", "if");

                output += cond + eval(b[0], variable, ca, op, recCall + 1);
                output += invCond + eval(b[1], variable, ca, op, recCall + 1);
            }
            else if (operators_bool.Any(x => smartContains(val, x[0])) && ca == Type.BOOL)
            {
                return splitEval(val, variable, ca, op, recCall + 1);
            }
            else if (containsOP && Calculator.TryCalculate(val, out simflied))
            {
                return eval(simflied.ToString(), variable, ca, op, recCall + 1);
            }
            else if (containsOP && ca != Type.FUNCTION && ca != Type.BOOL && ca != Type.STRING)
            {
                return splitEval(val, variable, ca, op, recCall + 1);
            }
            else if (val.Contains("(") && context.IsFunction(val.Substring(0, val.IndexOf('('))))
            {
                return functionEval(val, new string[] { variable.gameName }, op);
            }
            else if (spaceSplitted.Length >= 3 && operators_comp.All(x => spaceSplitted[1] != x) && !(spaceSplitted[1] == "=>" && ca == Type.FUNCTION))
            {
                var desugared = DesugarOperator(spaceSplitted);
                if (desugared == null && operators_base.Any(x => smartContains(val, x[0]))){
                    return splitEval(val, variable, ca, op, recCall + 1);
                }
                if (desugared == null)
                {
                    throw new Exception("Unknow operator");
                }
                return eval(desugared, variable, ca, op, recCall);
            }
            else if (!context.isEntity(val) && smartContains(val, '[') && arrayVarReg.Match(val).Success && arrayFunc3Reg.Match(val).Success)
            {
                return functionEval(val, new string[] { variable.gameName }, op);
            }
            else if (!context.isEntity(val) && smartContains(val, '[') && arrayVarReg.Match(val).Success && !arrayFuncReg.Match(val).Success)
            {
                var mat = getArrayBlock(val);
                string vara = regReplace(val, mat, "");
                string para = mat.Value.Substring(1, mat.Value.Length - 2);
                return parseLine($"{variable.gameName}={smartExtract(vara)}.get({para})");
            }
            else if (!context.isEntity(val) && smartContains(val, '[') && arrayVarReg.Match(val).Success && arrayFuncReg.Match(val).Success)
            {
                return functionEval(val, new string[] { variable.gameName }, op);
            }
            else if (ca == Type.ARRAY)
            {
                val = smartExtract(val);

                if (val.StartsWith("[") || val.StartsWith("{"))
                {
                    string[] nValue = smartSplit(val.Substring(1, val.Length - 2), ',');

                    if (nValue.Length != variable.arraySize)
                        throw new Exception("Array don't the same size");

                    output = "";
                    for (int i = 0; i < variable.arraySize; i++)
                    {
                        output += parseLine(variable.gameName + "." + i.ToString() + op + nValue[i]);
                    }
                }
                else if (context.GetVariable(val, true) != null)
                {
                    var val2 = GetVariableByName(val);
                    if (val2.type == Type.ARRAY)
                    {
                        if (val2.arraySize != variable.arraySize)
                            throw new Exception("Array don't the same size");

                        output = "";
                        for (int i = 0; i < variable.arraySize; i++)
                        {
                            output += parseLine(variable.gameName + "." + i.ToString() + op + val + "." + i.ToString());
                        }
                        return output;
                    }
                    else
                    {
                        output = "";
                        for (int i = 0; i < variable.arraySize; i++)
                        {
                            output += parseLine(variable.gameName + "." + i.ToString() + op + val);
                        }
                        return output;
                    }
                }
                else
                {
                    output = "";
                    for (int i = 0; i < variable.arraySize; i++)
                    {
                        output += parseLine(variable.gameName + "." + i.ToString() + op + val);
                    }
                    return output;
                }
            }
            else if (ca == Type.FUNCTION)
            {
                int intVal = 0;
                if (val.Contains("=>"))
                {
                    return InstLamba(val, variable);
                }
                else if (val.StartsWith("{"))
                {
                    return InstLamba("=>" + val, variable);
                }
                else if (op == "#=" && int.TryParse(val, out intVal))
                {
                    return Core.VariableOperation(variable, intVal, "=");
                }
                else if (valVar == null && context.GetVariable(val, true) != null)
                {
                    return eval(context.GetVariable(val, true), variable, ca, op, recCall++);
                }
                else if (valVar == null || (op == "#="))
                {
                    Function func = GetFunction(context.GetFunctionName(val), variable.args);
                    if (!func.isStructMethod)
                    {
                        if (func.lazy)
                        {
                            throw new Exception("Can not put a lazy function inside a variable");
                        }

                        return Core.VariableOperation(variable, GetFunctionIndex(variable, func), "=");
                    }
                }
                else
                {
                    return Core.VariableOperation(variable, valVar, "=");
                }
            }
            else if (ca == Type.STRUCT)
            {
                Structure stru1 = structs[variable.enums];
                if (variable == valVar || (valVar != null && valVar.gameName == variable.gameName))
                    return "";
                if (val.StartsWith("new "))
                {
                    val = val.Substring(4, val.Length - 4);
                }
                if (getStruct(val.Replace("(", " ")) != null)
                {
                    if (stru1.isClass)
                    {
                        Structure.DerefObject(variable);
                    }
                    string stru = getStruct(val.Replace("(", " "));
                    if (structs[stru] == stru1)
                    {
                        if (structs[stru].isStatic)
                            throw new Exception($"Can not Instantiate Static Class/Struct {stru}");
                        if (structs[stru].isAbstract)
                            throw new Exception($"Can not Instantiate Abstract Class/Struct {stru}");
                        output += parseLine(variable.gameName + ".__init__" + val.Substring(val.IndexOf('('), val.LastIndexOf(')') - val.IndexOf('(') + 1));
                    }
                    else
                    {
                        if (structs[stru].isStatic)
                            throw new Exception($"Can not Instantiate Static Class/Struct {stru}");
                        if (structs[stru].isAbstract)
                            throw new Exception($"Can not Instantiate Abstract Class/Struct {stru}");
                        int id = tmpID++;
                        preparseLine(stru + " tmp_" + id.ToString() + " = " + stru + "(" + getArg(val) + ")");
                        preparseLine(variable.gameName + " = tmp_" + id.ToString());
                    }
                }
                else
                {
                    if (op == "#=")
                    {
                        return Core.VariableOperation(variable, valVar, "=");
                    }
                    else if (op == "=" && variable != valVar)
                    {
                        if (stru1.isClass && valVar != null)
                        {
                            Structure stru2 = structs[valVar.enums];

                            output = "";

                            Structure.RefObject(valVar);
                            Structure.DerefObject(variable);

                            output += Core.VariableOperation(variable, valVar, "=");
                        }
                        else if (stru1.methodsName.Contains("__set__") &&
                            (valVar == null || valVar.type != Type.STRUCT || valVar.enums != stru1.name))
                        {
                            output += parseLine(variable.gameName + ".__set__(" + val + ")");
                        }
                        else
                        {
                            Structure stru2 = structs[valVar.enums];
                            if (!stru2.canBeAssignIn(stru1))
                            {
                                throw new Exception("Cannot use " + op + " between " + stru2.name + " inside " + stru1.name);
                            }
                            HashSet<string> set = new HashSet<string>();
                            foreach (Variable struV in structs[variable.enums].fields)
                            {
                                if (!set.Contains(struV.name))
                                {
                                    preparseLine(variable.gameName + "." + struV.name + "=" + val + "." + struV.name);
                                    set.Add(struV.name);
                                }
                            }
                        }
                    }
                    else if (op == "*=")
                    {
                        output += parseLine(variable.gameName + ".__mult__(" + val + ")");
                    }
                    else if (op == "+=")
                    {
                        output += parseLine(variable.gameName + ".__add__(" + val + ")");
                    }
                    else if (op == "-=")
                    {
                        output += parseLine(variable.gameName + ".__sub__(" + val + ")");
                    }
                    else if (op == "/=")
                    {
                        output += parseLine(variable.gameName + ".__div__(" + val + ")");
                    }
                    else if (op == "&=")
                    {
                        output += parseLine(variable.gameName + ".__and__(" + val + ")");
                    }
                    else if (op == "|=")
                    {
                        output += parseLine(variable.gameName + ".__or__(" + val + ")");
                    }
                    else if (op == "^=")
                    {
                        output += parseLine(variable.gameName + ".__xor__(" + val + ")");
                    }
                    else if (op == "%=")
                    {
                        output += parseLine(variable.gameName + ".__mod__(" + val + ")");
                    }
                }
            }
            else if (ca == Type.ENTITY_COMPONENT)
            {
                if (op == "=")
                {
                    if (val.Contains("@"))
                    {
                        if (!NBT_Data.isSameAs(variable.name + "." + variable.gameName, val))
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1) + NBT_Data.parseGet(val, 1) + '\n';
                    }
                    else if (context.isEntity(val))
                    {
                        if (!NBT_Data.isSameAs(variable.name + "." + variable.gameName, val))
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1) + NBT_Data.parseGet(context.ConvertEntity(val), 1) + '\n';
                    }
                    else if (int.TryParse(val, out tmpI))
                    {
                        if (NBT_Data.getType(variable.gameName) == "int")
                        {
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1f) + "scoreboard players get " + GetConstant(tmpI).scoreboard() + '\n';
                        }
                        else
                        {
                            tmpI *= compilerSetting.FloatPrecision;
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1f / compilerSetting.FloatPrecision) + "scoreboard players get " + GetConstant(tmpI).scoreboard() + '\n';
                        }
                    }
                    else if (double.TryParse(val, out tmpF))
                    {
                        tmpI = (int)(tmpF * compilerSetting.FloatPrecision);
                        output += NBT_Data.parseSet(variable.name, variable.gameName, 1f / compilerSetting.FloatPrecision) + "scoreboard players get " + GetConstant(tmpI).scoreboard() + '\n';
                    }
                    else if (context.IsFunction(val))
                    {
                        return functionEval(val, new string[] { variable.name + "." + variable.gameName });
                    }
                    else if (valVar != null)
                    {
                        if (valVar.type == Type.FLOAT)
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1f / compilerSetting.FloatPrecision) + "scoreboard players get " + valVar.scoreboard() + '\n';
                        else
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1) + "scoreboard players get " + valVar.scoreboard() + '\n';
                    }
                }
                else
                {
                    output += parseLine(NBT_Data.getType(variable.gameName) + " tmp.0 = " + variable.name + "." + variable.gameName + op[0] + "(" + val + ")");
                    output += eval("tmp.0", variable, ca, "=", recCall + 1);
                }
            }
            else if (ca == Type.INT || ca == Type.ENUM)
            {
                val = smartEmpty(val);
                if (variable.enums != null && enums[variable.enums].Contains(val.ToLower()))
                {
                    if (op != "=")
                        throw new Exception("Unsupported Operator: " + op);
                    return Core.VariableOperation(variable, enums[variable.enums].IndexOf(val.ToLower()), "=");
                }
                else if (val.Contains("/"))
                {
                    return "execute store result score " + variable.scoreboard() + " run " +
                        val.Substring(val.IndexOf("/") + 1, val.Length - val.IndexOf("/") - 1) + '\n';
                }
                else if (CommandParser.canBeParse(val))
                {
                    return "execute store result score " + variable.scoreboard() + " run " +
                        CommandParser.parse(val, context);
                }
                else if (val.Contains("@"))
                {
                    if (op == "=")
                        return "execute store result score " + variable.scoreboard() + " run " + NBT_Data.parseGet(val, 1) + '\n';
                    else
                    {
                        output += parseLine("int tmp.0 = " + val);
                        output += eval("tmp.0", variable, ca, op, recCall + 1);
                        return output;
                    }
                }
                else if (context.isEntity(val))
                {
                    if (op == "=")
                        return "execute store result score " + variable.scoreboard() + " run " + NBT_Data.parseGet(context.ConvertEntity(val), 1) + '\n';
                    else
                    {
                        output += parseLine("int tmp.0 = " + val);
                        output += eval("tmp.0", variable, ca, op, recCall + 1);
                        return output;
                    }
                }
                else if (int.TryParse(val, out tmpI))
                {
                    return Core.VariableOperation(variable, tmpI, op);
                }
                else if (double.TryParse(val, out tmpF))
                {
                    tmpI = (int)(tmpF * compilerSetting.FloatPrecision);
                    if (op == "#=")
                        return Core.VariableOperation(variable, tmpI, "=");
                    else
                    {
                        output += Core.VariableOperation(variable, (int)(tmpF), op);
                        return output;
                    }
                }
                else
                {
                    try
                    {
                        if (op != "=" || valVar != variable)
                        {
                            if (op == "#=")
                                return Core.VariableOperation(variable, valVar, "=");
                            else
                            {
                                if (valVar != null && valVar.type != Type.FLOAT)
                                    return Core.VariableOperation(variable, valVar, op);
                                if (op == "=")
                                {
                                    output += Core.VariableOperation(variable, GetVariableByName(val), op);
                                    output += Core.VariableOperation(variable, compilerSetting.FloatPrecision, "/=");
                                    return output;
                                }
                                else
                                {
                                    output += parseLine("float tmp.0 = " + val + "/" + compilerSetting.FloatPrecision.ToString());
                                    output += Core.VariableOperation(variable, GetVariableByName("tmp.0"), op);
                                    return output;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error on var " + variable.gameName + " (" + variable.GetInternalTypeString() + "," + variable.enums + ") " + e.ToString());
                    }
                }
            }
            else if (ca == Type.FLOAT)
            {
                if (val.Contains("@"))
                {
                    if (op == "=")
                        return "execute store result score " + variable.scoreboard() + " run " + NBT_Data.parseGet(val, compilerSetting.FloatPrecision) + '\n';
                    else
                    {
                        output += parseLine("float tmp.0 = " + val);
                        output += eval("tmp.0", variable, ca, op, recCall + 1);
                        return output;
                    }
                }
                else if (context.isEntity(val))
                {
                    if (op == "=")
                        return "execute store result score " + variable.scoreboard() + " run " + NBT_Data.parseGet(context.ConvertEntity(val), compilerSetting.FloatPrecision) + '\n';
                    else
                    {
                        output += parseLine("float tmp.0 = " + val);
                        output += eval("tmp.0", variable, ca, op, recCall + 1);
                        return output;
                    }
                }
                else if (int.TryParse(val, out tmpI))
                {
                    int val2 = tmpI * ((op != "*=" && op != "/=") ? compilerSetting.FloatPrecision : 1);
                    return Core.VariableOperation(variable, val2, op);
                }
                else if (double.TryParse(val, out tmpF))
                {
                    int iVal = ((int)(tmpF * compilerSetting.FloatPrecision));
                    val = iVal.ToString();

                    if (op == "#=")
                        return Core.VariableOperation(variable, iVal, "=");
                    else if (op == "=" || op == "+=" || op == "-=")
                        return Core.VariableOperation(variable, iVal, op);
                    else if (op == "*=")
                    {
                        output += Core.VariableOperation(variable, iVal, op);
                        return output + Core.VariableOperation(variable, compilerSetting.FloatPrecision, "/=");
                    }
                    else if (op == "/=")
                    {
                        output += Core.VariableOperation(variable, iVal, op);
                        return output + Core.VariableOperation(variable, compilerSetting.FloatPrecision, "*=");
                    }
                    else
                    {
                        return Core.VariableOperation(variable, iVal, op);
                    }
                }
                else
                {
                    if (op != "=" || valVar != variable)
                    {
                        if (valVar != null && valVar.type == Type.INT && (op == "+=" || op == "-="))
                        {
                            output += parseLine("int tmp.1 = " + compilerSetting.FloatPrecision.ToString() + "*" + val);
                            return output + Core.VariableOperation(variable, GetVariableByName("tmp.1"), op);
                        }
                        else if (valVar != null && valVar.type == Type.INT && op == "#=")
                        {
                            return output + Core.VariableOperation(variable, GetVariableByName(val), "=");
                        }
                        else if (valVar != null && valVar.type == Type.INT && op == "=")
                        {
                            output += Core.VariableOperation(variable, valVar, op);
                            return output + Core.VariableOperation(variable, compilerSetting.FloatPrecision, "*=");
                        }
                        else if (valVar != null && valVar.type == Type.FLOAT && op == "*=")
                        {
                            output += Core.VariableOperation(variable, valVar, op);
                            return output + Core.VariableOperation(variable, compilerSetting.FloatPrecision, "/=");
                        }
                        else if (valVar != null && valVar.type == Type.FLOAT && op == "/=")
                        {
                            output += Core.VariableOperation(variable, compilerSetting.FloatPrecision, "*=");
                            return output + Core.VariableOperation(variable, valVar, op);
                        }
                        else if (valVar != null)
                        {
                            return Core.VariableOperation(variable, valVar, op);
                        }
                        else
                        {
                            throw new Exception("Unkowned: \"" + smartEmpty(val) + "\" in context " + context.GetVar());
                        }
                    }
                }
            }
            else if (ca == Type.BOOL)
            {
                string ve = smartEmpty(val);
                if (op == "&=" || op == "*=")
                {
                    if (valVar != null)
                    {
                        return Core.VariableOperation(variable, valVar, "*=");
                    }
                    else
                    {
                        string cond = getCondition("!" + val);
                        return cond + Core.VariableOperation(variable, 0, "=");
                    }
                }
                else if (op == "|=" || op == "+=")
                {
                    if (valVar != null)
                    {
                        output += Core.VariableOperation(variable, valVar, "+=");
                        string cond = getCondition(val + ">= 2");
                        return output + cond + Core.VariableOperation(variable, 1, "=");
                    }
                    else
                    {
                        string cond = getCondition(val);
                        return cond + Core.VariableOperation(variable, 1, "=");
                    }
                }
                else if (op == "^=")
                {
                    if (valVar != null)
                    {
                        output += Core.VariableOperation(variable, valVar, "+=");
                        string cond = getCondition(val + ">= 2");
                        output += cond + Core.VariableOperation(variable, 0, "=");
                        return output;
                    }
                    else
                    {
                        string cond = getCondition(val);
                        string cond2 = getCondition(val + "&&" + variable.gameName);

                        output += cond + Core.VariableOperation(variable, 1, "=");
                        output += cond2 + Core.VariableOperation(variable, 0, "=");
                        return output;
                    }
                }
                else if (ve.ToLower() == "true" || ve == "1")
                {
                    return Core.VariableOperation(variable, 1, "=");
                }
                else if (ve.ToLower() == "false" || ve == "0")
                {
                    return Core.VariableOperation(variable, 0, "=");
                }
                else if (valVar != null)
                {
                    if (op != "=" || valVar != variable)
                        return Core.VariableOperation(variable, valVar, "=");
                }
                else if (context.isEntity(val) && smartContains(val, '.'))
                {
                    if (op == "=")
                        return "execute store result score " + variable.scoreboard() + " run " + NBT_Data.parseGet(context.ConvertEntity(val), 1) + '\n';
                    else
                    {
                        output += parseLine("bool tmp.0 = " + val);
                        output += eval("tmp.0", variable, ca, op, recCall + 1);
                        return output;
                    }
                }
                else
                {
                    output += Core.VariableOperation(variable, 0, "=");
                    string cond = getCondition(val);
                    return output + cond + Core.VariableOperation(variable, 1, "=");
                }
            }
            else if (ca == Type.ENTITY)
            {
                if (val.Contains("/"))
                {
                    return "execute store result score " + variable.scoreboard() + " run " +
                        val.Substring(val.IndexOf("/") + 1, val.Length - val.IndexOf("/") - 1);
                }
                else if (val.Contains("@"))
                {
                    output += "tag @e remove " + variable.gameName + '\n' +
                        "tag " + smartEmpty(val) + " add " + variable.gameName + '\n';
                }
                else
                {
                    output += "tag @e remove " + variable.gameName + '\n' +
                        "tag " + context.GetEntitySelector(val) + " add " + variable.gameName + '\n';
                }
            }
            else if (ca == Type.STRING)
            {
                if (val.Contains("\""))
                {
                    Core.VariableOperation(variable, getStringID(val), "=");
                }
                else
                {
                    try
                    {
                        if (op != "=" || valVar != variable)
                            Core.VariableOperation(variable, valVar, "=");
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error on var " + variable.gameName + " (" + variable.GetInternalTypeString() + "," + variable.enums + ") " + e.ToString());
                    }
                }
            }
            else if (ca == Type.VOID)
                throw new Exception("Cannot moddifie type void");

            if (valVar != null && valVar.type == Type.VOID)
                throw new Exception("Cannot get value type void");

            return output;
        }
        private static string splitEval(string val, Variable variable, Type ca, string op, int recCall = 0)
        {
            string[] part;
            string output = "";
            string[] operations = new string[] { "^", "|", "&", "+", "-", "%", "/", "*" };
            string val2 = getParenthis(val, 1);

            foreach (string xop in operations)
            {
                part = smartSplit(val2, xop[0]);

                if (part.Length > 1)
                {
                    float valLeft = 0;
                    
                    for (int i = 0; i < part.Length; i++)
                    {
                        if (Calculator.TryCalculate(part[i], out valLeft))
                        {
                            part[i] = valLeft.ToString();
                        }
                    }

                    if (xop[0] == op[0])
                    {
                        if (op[0] == '-')
                        {
                            output += eval(getParenthis(part[0], 1), variable, ca, op, recCall + 1);
                            for (int i = 1; i < part.Length; i++)
                            {
                                string value = part[i];
                                output += eval(getParenthis(value, 1), variable, ca, "+=", recCall + 1);
                            }
                        }
                        else if (op[0] == '/')
                        {
                            output += eval(getParenthis(part[0], 1), variable, ca, op);
                            for (int i = 1; i < part.Length; i++)
                            {
                                string value = part[i];
                                output += eval(getParenthis(value, 1), variable, ca, "*=", recCall + 1);
                            }
                        }
                        else
                        {
                            foreach (string value in part)
                            {
                                output += eval(getParenthis(value, 1), variable, ca, op, recCall + 1);
                            }
                        }

                        return output;
                    }
                    else
                    {
                        int id = tmpID;

                        Variable v1 = new Variable(id.ToString(), context.GetVar() + "__eval__" + id.ToString(), ca);
                        v1.enums = variable.enums;

                        if (op != "=")
                        {
                            AddVariable(context.GetVar() + "__eval__" + id.ToString(), v1);
                            tmpID++;

                            if (ca == Type.STRUCT)
                            {
                                structs[variable.enums].generate("__eval__" + id.ToString(), false, v1);
                            }
                        }
                        else
                        {
                            v1 = variable;
                        }

                        if (part[0] == "")
                            part[0] = "0";

                        output += eval(getParenthis(part[0], 1), v1, ca, "=", recCall + 1);
                        for (int i = 1; i < part.Length; i++)
                        {
                            string value = part[i];
                            if (xop[0] == '-' && op[0] == '-')
                            {
                                output += eval(getParenthis(value, 1), v1, ca, "+=", recCall + 1);
                            }
                            else
                            {
                                output += eval(getParenthis(value, 1), v1, ca, xop[0] + "=", recCall + 1);
                            }
                        }

                        if (op != "=")
                            output += eval("__eval__" + (id).ToString(), variable, ca, op, recCall + 1);
                        return output;
                    }
                }
            }

            throw new Exception("Unsportted operation: " + val);
        }
        private static string Decurry(string val, Variable variable, Type ca, string op = "=")
        {
            string left = getFunctionName(val);
            string subName1 = !left.EndsWith(")") ? left.Substring(0, left.LastIndexOf(')')+1) : left;
            string subName2 = left.EndsWith(")") ? left.Substring(left.LastIndexOf(')') + 1, left.Length - left.LastIndexOf(')') - 1) : left.Substring(left.LastIndexOf(')')+1);
            preparseLine("var __decurry__ = " + subName1);
            preparseLine(variable.gameName + $" = __decurry__{(subName2 == null ? "" : "." + subName2)}({val.Replace(left, "")})");
            return "";
        }
        private static string Decurry(string val, string[] variables, string op = "=")
        {
            string left = getFunctionName(val);
            string subName1 = !left.EndsWith(")") ? left.Substring(0, left.LastIndexOf(')')+1) : left;
            string subName2 = left.EndsWith(")") ? left.Substring(left.LastIndexOf(')')+1, left.Length- left.LastIndexOf(')')-1) : left.Substring(left.LastIndexOf(')') + 1);
            string varStr = "";
            for (int i = 0; i < variables.Length; i++)
            {
                varStr += variables[i];
                if (i < variables.Length - 1)
                {
                    varStr += ",";
                }
            }
            preparseLine("var __decurry__ = " + subName1);
            preparseLine(varStr + $" = __decurry__{(subName2==null?"":"."+subName2)}({val.Replace(left, "")})");
            return "";
        }
        private static string DesugarOperator(string[] sp)
        {
            if (sp.Length <= 1)
            {
                return null;
            }
            string op = sp[1];
            int index = 1;
            for (int i = 1; i < sp.Length; i+=2)
            {
                if (op.CompareTo(sp[i]) > 0)
                {
                    op = sp[i];
                    index = i;
                }
            }
            string[] lefts = sp.Take(index).ToArray();
            string[] rights = sp.Skip(index + 1).ToArray();

            string l = lefts.Length  == 1 ? lefts[0]  : DesugarOperator(lefts);
            string r = rights.Length == 1 ? rights[0] : DesugarOperator(rights);
            if (l == null || r == null)
                return null;
            Function func;
            try
            {
                func = GetFunction(context.GetFunctionName(op), new string[] { l, r });
            }
            catch { return null; }
            if (func != null && func.isInfix)
                return $"{FunctionNameSimple(op)}({l},{r})";
            return null;
        }
        #endregion

        #region condition
        public static string getCondition(string text)
        {
            string[] v = getConditionSplit(text);
            return v[0] + Core.Condition(v[1]);
        }
        private static string[] getConditionSplit(string text)
        {
            string[] arg = smartSplit(text.Replace("&&", "&"), '&');
            string output = "";
            string cond = "";

            for (int i = 0; i < arg.Length; i++)
            {
                if (smartEmpty(arg[i]) == "")
                {
                }
                else if (arg[i].Contains("||"))
                {
                    string[] in1 = getCondOr(arg[i]);
                    cond += in1[0];
                    output += in1[1];
                }
                else
                {
                    while (arg[i].StartsWith(" "))
                    {
                        arg[i] = arg[i].Substring(1, arg[i].Length - 1);
                    }
                    if (arg[i].StartsWith("!"))
                    {
                        string[] in1 = getCond(arg[i].Substring(1, arg[i].Length - 1));

                        if (in1[0] == ConditionAlwayTrue)
                        {
                            cond += ConditionAlwayFalse;
                        }
                        else if (in1[0] == ConditionAlwayFalse)
                        {
                            
                        }
                        else
                        {
                            cond += Core.ConditionInverse(in1)[0];

                            output += in1[1];
                        }
                    }
                    else
                    {
                        string[] in1 = getCond(arg[i]);
                        if (in1[0] == ConditionAlwayTrue)
                        {

                        }
                        else if (in1[0] == ConditionAlwayFalse)
                        {
                            cond += ConditionAlwayFalse;
                        }
                        else
                        {
                            cond += in1[0];

                            output += in1[1];
                        }
                    }
                }
            }
            return new string[] { output, cond };
        }
        private static string[] getCondOr(string text)
        {
            string out1 = "";
            string out2 = "";
            int idVal = If.GetEval(context.GetFun());

            out2 += parseLine("bool __eval" + idVal.ToString() + "__ = " + text.Replace("&&", "&").Replace("||", "|"));

            string[] part = getCond("__eval" + idVal.ToString() + "__");

            out2 += part[1];

            out1 = part[0];
            return new string[] { out1, out2 };
        }
        private static string[] getCond(string text)
        {
            int tmpI;
            double tmpF;
            int idVal = If.GetEval(context.GetFun());

            if (context.isEntity(text) && !smartContains(text, '.'))
            {
                return Core.ConditionEntity(context.GetEntitySelector(text));
            }
            else if (text.StartsWith("tag("))
            {
                return Core.ConditionEntity("@s[tag=" + getArg(text) + "]");
            }
            else if (text.StartsWith("block("))
            {
                string[] args = getArgs(text);
                string output = args[0];
                if (args[0].Contains("#") && !args[0].Contains(":"))
                {
                    if (compilerSetting.tagsFolder)
                    {
                        string[] part = args[0].Split(' ');
                        string tag = Core.FormatTagsPath(context.GetBlockTags(part[3].Replace("#", "")));
                        output = part[0] + " " + part[1] + " " + part[2] + " #" + tag;
                    }
                    else
                    {
                        string[] part = args[0].Split(' ');
                        output = part[0] + " " + part[1] + " " + part[2] + " #" + Project + ":" + part[3].Replace("#", "");
                    }
                }
                return Core.ConditionBlock(output);
            }
            else if (text.StartsWith("blocks("))
            {
                string[] args = getArgs(text);
                return Core.ConditionBlocks(args[0]);
            }
            else if (text.ToLower().StartsWith("__isvar("))
            {
                string[] arg = getArgs(text);
                if (arg.All(x => GetVariableByName(x, true) != null))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                else
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
            }
            else if (text.ToLower().StartsWith("__isnb("))
            {
                string[] arg = getArgs(text);
                if (arg.All(x => double.TryParse(smartExtract(x), out double _)))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                else
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
            }
            else if (text.ToLower().StartsWith("__isint("))
            {
                string[] arg = getArgs(text);

                if (arg.All(x => int.TryParse(smartExtract(x), out int _)))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                else
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
            }
            else if (text.ToLower().StartsWith("__isstring("))
            {
                string[] arg = getArgs(text);

                if (arg.All(x => isString(x)))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                else
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
            }
            else if (text.ToLower().StartsWith("__isentity("))
            {
                string[] arg = getArgs(text);
                if (arg.All(x => context.isEntity(x)))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                else
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
            }
            else if (text.ToLower().StartsWith("__hasspace("))
            {
                string[] arg = getArgs(text);
                if (arg.Any(x => x.Contains(" ")))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                else
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
            }
            else if (text.ToLower().StartsWith("__debug"))
            {
                return new string[] { compilerSetting.opti_ShowDebug?ConditionAlwayTrue: ConditionAlwayFalse, "" };
            }
            else if (text.ToLower().StartsWith("__exception"))
            {
                return new string[] { compilerSetting.opti_ShowException ? ConditionAlwayTrue : ConditionAlwayFalse, "" };
            }
            else if (context.GetPredicate(text, true) != null && text.Contains("(") && text.Contains(")"))
            {
                Predicate pred = GetPredicate(context.GetPredicate(text), getArgs(text));
                return new string[] { "if predicate " + pred.get(getArg(text)) + " ", "" };
            }
            else if (text.Contains("=="))
            {
                string[] arg = text.Replace("==", "=").Split('=');

                if (smartEmpty(arg[0]) == smartEmpty(arg[1]) && !IsFunction(smartEmpty(arg[0])))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                double ta = 0, tb = 0;
                if (double.TryParse(smartEmpty(arg[0]), out ta) && double.TryParse(smartEmpty(arg[1]), out tb))
                {
                    if (ta == tb)
                    {
                        return new string[] { ConditionAlwayTrue, "" };
                    }
                    else
                    {
                        return new string[] { ConditionAlwayFalse, "" };
                    }
                }

                string selector1 = "";
                string selector2 = "";
                if (arg[0].StartsWith("@") && smartContains(arg[0], '.'))
                {
                    string[] sarg0 = smartSplit(arg[0], '.', 1);
                    selector1 = sarg0[0];
                    arg[0] = sarg0[1];
                }
                if (arg.Length > 1 && arg[1].StartsWith("@") && smartContains(arg[1], '.'))
                {
                    string[] sarg0 = smartSplit(arg[1], '.', 1);
                    selector2 = sarg0[0];
                    arg[1] = sarg0[1];
                }


                for (int i = 0; i < arg.Length; i++)
                {
                    if (containLazyVal(smartEmpty(arg[i])))
                    {
                        arg[i] = getLazyVal(smartEmpty(arg[i]));
                    }
                }

                Variable var;
                string pre = "";
                bool entity1 = false;
                if (context.GetVariable(arg[0], true) != null)
                {
                    var = GetVariableByName(arg[0]);
                }
                else
                {
                    int idVal3 = If.GetEval(context.GetFun());

                    pre += parseLine(getExprTypeStr(arg[0], selector1!="").ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + (selector1!= "" ? selector1 + "." : "") + arg[0]);
                    entity1 = selector1 != "";
                    selector1 = "";
                    var = GetVariableByName("cond_" + idVal3);
                }


                arg[1] = smartEmpty(arg[1]);

                Type t = var.type;

                if (arg[1] == "null")
                {
                    return Core.ConditionInverse(
                        appendPreCond(Core.CompareVariable(GetVariableByName(arg[0]), GetVariableByName(arg[0]), "=", selector1, selector1), pre));
                }

                if (t == Type.STRUCT)
                {
                    return getCond(var + ".__equal__(" + var.gameName + ")");
                }
                else if ((t == Type.INT || t == Type.ENUM || t == Type.FUNCTION))
                {
                    if (arg[1].Contains(".."))
                    {
                        string[] part = arg[1].Replace("..", ",").Split(',');
                        int p1 = int.MinValue;
                        int p2 = int.MaxValue;

                        if (part[0] != "")
                            p1 = ((int)(int.Parse(part[0])));
                        if (part[1] != "")
                            p2 = ((int)(int.Parse(part[1])));

                        return appendPreCond(Core.CompareVariable(var, p1, p2, selector1), pre);
                    }
                    else if (int.TryParse(arg[1], out tmpI))
                    {
                        return appendPreCond(Core.CompareVariable(var, tmpI, "=", selector1), pre);
                    }

                    if (var.enums != null && enums[var.enums].Contains(smartEmpty(arg[1].ToLower())))
                    {
                        return appendPreCond(Core.CompareVariable(var, enums[var.enums].IndexOf(smartEmpty(arg[1].ToLower())), "=", selector1), pre);
                    }
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2), pre);
                }
                else if (t == Type.FLOAT && arg[1].Contains(".."))
                {
                    string[] part = arg[1].Replace("..", ",").Split(',');
                    int p1 = int.MinValue;
                    int p2 = int.MaxValue;

                    if (part[0] != "")
                        p1 = ((int)(double.Parse(part[0]) * compilerSetting.FloatPrecision));
                    if (part[1] != "")
                        p2 = ((int)(double.Parse(part[1]) * compilerSetting.FloatPrecision));

                    return appendPreCond(Core.CompareVariable(var, p1, p2, selector1), pre);
                }
                else if (t == Type.FLOAT && double.TryParse(arg[1], out tmpF))
                {
                    return appendPreCond(Core.CompareVariable(var, (int)(tmpF * compilerSetting.FloatPrecision), "=", selector1), pre);
                }
                else if (t == Type.BOOL)
                {
                    if (arg[1] == "true")
                        return appendPreCond(Core.CompareVariable(var, 1, "="), pre);
                    else if (arg[1] == "false")
                        return appendPreCond(Core.CompareVariable(var, 0, "="), pre);
                    else
                        return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2), pre);
                }
                else if (t == Type.STRING && arg[1].Contains("\""))
                {
                    return appendPreCond(Core.CompareVariable(var, getStringID(arg[1]), "=", selector1), pre);
                }
                else if (context.GetVariable(arg[1], true) != null)
                {
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2), pre);
                }
                else
                {
                    int idVal2 = If.GetEval(context.GetFun());
                    pre += parseLine(getExprTypeStr(arg[1], selector2 != "").ToString().ToLower() + " cond_" + idVal2.ToString() + " = " + (selector2!=""?selector2+".":"")+arg[1]);
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName("cond_" + idVal2), "=", selector1), pre);
                }
            }
            else if (text.Contains("!="))
            {
                string[] arg = text.Replace("!=", "=").Split('=');
                string selector1 = "";

                if (smartEmpty(arg[0]) == smartEmpty(arg[1]) && !IsFunction(smartEmpty(arg[0])))
                {
                    return new string[] { ConditionAlwayFalse, "" };
                }
                double ta = 0, tb = 0;
                if (double.TryParse(smartEmpty(arg[0]), out ta) && double.TryParse(smartEmpty(arg[1]), out tb))
                {
                    if (ta == tb)
                    {
                        return new string[] { ConditionAlwayFalse, "" };
                    }
                    else
                    {
                        return new string[] { ConditionAlwayTrue, "" };
                    }
                }

                if (arg[0].StartsWith("@") && smartContains(arg[0], '.'))
                {
                    string[] sarg0 = smartSplit(arg[0], '.', 1);
                    selector1 = sarg0[0];
                    arg[0] = sarg0[1];
                }

                if (arg[1].Replace(" ", "") == "null")
                {
                    Variable var;
                    string pre = "";

                    if (context.GetVariable(arg[0], true) != null)
                    {
                        var = GetVariableByName(arg[0]);
                    }
                    else
                    {
                        int idVal3 = If.GetEval(context.GetFun());
                        pre += parseLine(getExprTypeStr(arg[0]).ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + arg[0]);
                        var = GetVariableByName("cond_" + idVal3);
                    }

                    return appendPreCond(Core.CompareVariable(var, var, "==", selector1), pre);
                }
                else
                {
                    return Core.ConditionInverse(getCond(text.Replace("!=", "==")));
                }
            }
            else if ((context.isEntity(text) || text.StartsWith("@")) && !smartContains(text, '.'))
            {
                return Core.ConditionEntity(context.GetEntitySelector(text));
            }
            else if (text.Contains("(") && context.IsFunction(text.Substring(0, text.IndexOf('('))) && !text.Contains("<") && !text.Contains("=") && !text.Contains(">"))
            {
                string arg = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
                string[] args = smartSplitJson(arg, ',');
                string func;
                if (smartContains(text, '='))
                {
                    func = smartEmpty(smartSplitJson(text, '=', 1)[1]).Substring(text.IndexOf(' ') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
                }
                else
                {
                    func = text.Substring(0, text.IndexOf('('));
                }

                string funcName = context.GetFunctionName(func);

                Function funObj = GetFunction(funcName, args);
                if (!funObj.lazy)
                    return appendPreCond(Core.CompareVariable(funObj.outputs[0], 1, ">="), parseLine(text));
                else
                {
                    int idVal3 = If.GetEval(context.GetFun());
                    string line = parseLine(funObj.outputs[0].GetInternalTypeString() + " cond_" + idVal3.ToString() + "=" + text);
                    return appendPreCond(Core.CompareVariable(GetVariableByName("cond_" + idVal3.ToString()), 1, ">="), line);
                }
            }
            else
            {
                string op = "";
                if (smartContains(text,"<=")) op = "<=";
                else if (smartContains(text,"<")) op = "<";
                else if (smartContains(text,">=")) op = ">=";
                else if (smartContains(text,">")) op = ">";

                if (op == "")
                {
                    if (smartEmpty(text) == "true") { return new string[] { ConditionAlwayTrue, "" }; }
                    if (smartEmpty(text) == "false") { return new string[] { ConditionAlwayFalse, "" }; }
                    double a = 0;
                    if (double.TryParse(smartEmpty(text), out a) && a > 0) { return new string[] { ConditionAlwayTrue, "" }; }
                    if (double.TryParse(smartEmpty(text), out a) && a <= 0) { return new string[] { ConditionAlwayFalse, "" }; }

                    if (GetVariableByName(text.Replace("!", ""), true) != null)
                    {
                        if (text.StartsWith("!"))
                            return Core.CompareVariable(GetVariableByName(text.Replace("!", "")), 0, "<=");
                        else
                            return Core.CompareVariable(GetVariableByName(text.Replace("!", "")), 1, ">=");
                    }
                    else
                    {
                        int idVal3 = If.GetEval(context.GetFun());

                        string pre2 = parseLine("bool cond_" + idVal3.ToString() + " = " + text);
                        Variable var2 = GetVariableByName("cond_" + idVal3);

                        if (text.StartsWith("!"))
                            return appendPreCond(Core.CompareVariable(var2, 0, "<="), pre2);
                        else
                            return appendPreCond(Core.CompareVariable(var2, 1, ">="), pre2);
                    }
                }

                string[] arg = smartEmpty(text).Replace(op, "=").Split('=');

                #region smartCompile
                double ta = 0, tb = 0;
                bool tc = false;
                bool td = smartEmpty(arg[0]) == smartEmpty(arg[1]) && !IsFunction(smartEmpty(arg[0]));

                tc = double.TryParse(smartEmpty(arg[0]), out ta) && double.TryParse(smartEmpty(arg[1]), out tb);
                if (op == "<=")
                {
                    if (td) { return new string[] { ConditionAlwayTrue, "" }; }
                    if (tc && ta <= tb) { return new string[] { ConditionAlwayTrue, "" }; } else if (tc) { return new string[] { ConditionAlwayFalse, "" }; }
                }
                if (op == "<")
                {
                    if (td) { return new string[] { ConditionAlwayFalse, "" }; }
                    if (tc && ta <= tb) { return new string[] { ConditionAlwayTrue, "" }; } else if (tc) { return new string[] { ConditionAlwayFalse, "" }; }
                }
                if (op == ">=")
                {
                    if (td) { return new string[] { ConditionAlwayTrue, "" }; }
                    if (tc && ta >= tb) { return new string[] { ConditionAlwayTrue, "" }; } else if (tc) { return new string[] { ConditionAlwayFalse, "" }; }
                }
                if (op == ">")
                {
                    if (td) { return new string[] { ConditionAlwayFalse, "" }; }
                    if (tc && ta > tb) { return new string[] { ConditionAlwayTrue, "" }; } else if (tc) { return new string[] { ConditionAlwayFalse, "" }; }
                }
                #endregion

                string selector1 = "";
                string selector2 = "";
                if (arg[0].StartsWith("@") && smartContains(arg[0], '.'))
                {
                    string[] sarg0 = smartSplit(arg[0], '.', 1);
                    selector1 = sarg0[0];
                    arg[0] = sarg0[1];
                }
                if (arg.Length > 1 && arg[1].StartsWith("@") && smartContains(arg[1], '.'))
                {
                    string[] sarg0 = smartSplit(arg[1], '.', 1);
                    selector2 = sarg0[0];
                    arg[1] = sarg0[1];
                }

                for (int i = 0; i < arg.Length; i++)
                {
                    if (containLazyVal(smartEmpty(arg[i])))
                    {
                        arg[i] = getLazyVal(smartEmpty(arg[i]));
                    }
                }

                Variable var;
                string pre = "";
                bool entity1 = false;
                
                if (context.GetVariable(arg[0], true) != null)
                {
                    var = GetVariableByName(arg[0]);
                }
                else
                {
                    int idVal3 = If.GetEval(context.GetFun());
                    
                    pre += parseLine(getExprTypeStr(arg[0], selector1 != "").ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + (selector1 != "" ? selector1 + "." : "") + arg[0]);
                    entity1 = selector1 != "";
                    selector1 = "";
                    var = GetVariableByName("cond_" + idVal3);
                }

                Type t = getExprType(arg[0], entity1);

                if ((t == Type.INT || t == Type.ENUM) && int.TryParse(arg[1], out tmpI))
                {
                    return appendPreCond(Core.CompareVariable(var, tmpI, op, selector1), pre);
                }
                if ((t == Type.ENUM) && enums[var.enums].valuesName.Contains(smartEmpty(arg[1]).ToLower()))
                {
                    return appendPreCond(Core.CompareVariable(var, enums[var.enums].valuesName.IndexOf(smartEmpty(arg[1]).ToLower()), op, selector1), pre);
                }
                else if (t == Type.FLOAT && double.TryParse(arg[1], out tmpF))
                {
                    int tmpL = ((int)(tmpF * compilerSetting.FloatPrecision));
                    return appendPreCond(Core.CompareVariable(var, tmpL, op, selector1), pre);
                }
                else if (context.GetVariable(arg[1], true) != null)
                {
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), op, selector1, selector2), pre);
                }
                else
                {
                    int idVal2 = If.GetEval(context.GetFun());
                    pre += parseLine(getExprTypeStr(arg[1], selector2 != "").ToString().ToLower() + " cond_" + idVal2.ToString() + " = " + (selector2 != "" ? selector2 + "." : "") + arg[1]);
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName("cond_" + idVal2), op, selector1), pre);
                }
            }
        }
        private static string[] appendPreCond(string[] text, string val)
        {
            return new string[] { text[0], text[1] + "\n" + val };
        }
        #endregion

        #region function
        public static void AddFunction(string name, Function func)
        {
            if (functions.ContainsKey(name))
                functions[name].Add(func);
            else
                functions.Add(name, new List<Function>() { func });
        }
        public static Function GetFunction(string funcName, string[] input_args, bool lambda = false)
        {
            if (functions[funcName].Count == 1)
            {
                return functions[funcName][0];
            }
            else
            {
                Function funObj = null;
                bool numericalOnly = true;
                //lambda = false;

                string[] args = new string[input_args.Length + (lambda ? 1 : 0)];
                for (int i = 0; i < input_args.Length; i++)
                {
                    args[i] = input_args[i];
                }

                if (lambda)
                    args[args.Length - 1] = "__lambda__";

                bool argIsString = args.Length > 0 ? isString(args[0]) : false;

                if (argIsString)
                {
                    foreach (string arg in smartSplitJson(extractString(args[0]), ' '))
                    {
                        numericalOnly = numericalOnly && (double.TryParse(arg, out double _) || arg.StartsWith("~") || arg.StartsWith("^"));
                    }
                }
                else
                {
                    foreach (string arg in args)
                    {
                        numericalOnly = numericalOnly && (double.TryParse(arg, out double _) || arg.StartsWith("~") || arg.StartsWith("^"));
                    }
                }

                Type[] argType = new Type[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    try
                    {
                        argType[i] = getExprType(args[i]);
                    }
                    catch
                    {
                        argType[i] = Type.UNKNOWN;
                    }
                }

                foreach (Function f in functions[funcName])
                {
                    if (funObj == null)
                    {
                        bool isGood = f.argNeeded <= args.Length && f.maxArgNeeded >= args.Length;

                        if (!numericalOnly && f.lazy && f.tags.Contains("__numerical_only__"))
                            isGood = false;

                        for (int i = 0; i < args.Length; i++)
                        {
                            int ad = (f.isExtensionMethod ? 1 : 0);
                            if (i >= f.args.Count - ad || argType[i] != f.args[i + ad].type ||
                                (f.lazy && f.args[i + ad].isLazy && !f.tags.Contains("__numerical_only__") && f.args[i + ad].type != Type.ENTITY)
                                || (f.lazy && f.args[i + ad].type == Type.ENTITY && !context.isEntity(args[i])))
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            funObj = f;
                            break;
                        }
                    }
                }
                if (funObj != null) { return funObj; }

                foreach (Function f in functions[funcName])
                {
                    if (funObj == null)
                    {
                        bool isGood = f.argNeeded <= args.Length && f.maxArgNeeded >= args.Length;

                        if (!numericalOnly && f.lazy && f.tags.Contains("__numerical_only__"))
                            isGood = false;

                        for (int i = 0; i < args.Length; i++)
                        {
                            int ad = (f.isExtensionMethod ? 1 : 0);
                            int index = i + ad;
                            if (i >= f.args.Count - ad ||
                                (argType[i] != f.args[index].type &&
                                !((argType[i] == Type.INT || argType[i] == Type.FLOAT) &&
                                (f.args[index].type == Type.FLOAT)))
                                || (f.lazy && f.args[index].isLazy && !f.tags.Contains("__numerical_only__")
                                && f.args[index].type != Type.ENTITY)
                                || (f.lazy && f.args[index].type == Type.ENTITY && !context.isEntity(args[i])))
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            funObj = f;
                            break;
                        }
                    }
                }
                foreach (Function f in functions[funcName])
                {
                    if (funObj == null)
                    {
                        bool isGood = f.argNeeded <= args.Length && f.maxArgNeeded >= args.Length;

                        if (!numericalOnly && f.lazy && f.tags.Contains("__numerical_only__"))
                            isGood = false;

                        for (int i = 0; i < args.Length; i++)
                        {
                            int ad = (f.isExtensionMethod ? 1 : 0);
                            int index = i + ad;
                            if (i >= f.args.Count - ad ||
                                (argType[i] != f.args[index].type &&
                                !((argType[i] == Type.INT || argType[i] == Type.FLOAT) &&
                                (f.args[index].type == Type.INT || f.args[index].type == Type.FLOAT)))
                                || (f.lazy && f.args[index].isLazy && !f.tags.Contains("__numerical_only__")
                                && f.args[index].type != Type.ENTITY)
                                || (f.lazy && f.args[index].type == Type.ENTITY && !context.isEntity(args[i])))
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            funObj = f;
                            break;
                        }
                    }
                }
                if (funObj != null) { return funObj; }

                foreach (Function f in functions[funcName])
                {
                    if (funObj == null && f.lazy && args.Length == f.args.Count)
                    {
                        bool isGood = f.argNeeded <= args.Length && f.maxArgNeeded >= args.Length;

                        for (int i = 0; i < args.Length; i++)
                        {
                            int ad = (f.isExtensionMethod ? 1 : 0);
                            int index = i + ad;
                            if (argType[i] != Type.UNKNOWN && i < f.args.Count - ad &&
                                ((argType[i] != f.args[index].type &&
                                !((argType[i] == Type.INT || argType[i] == Type.FLOAT) &&
                                (f.args[index].type == Type.INT || f.args[index].type == Type.FLOAT)))
                                || (f.lazy && f.args[index].isLazy && !f.tags.Contains("__numerical_only__"))))
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            funObj = f;
                            break;
                        }
                    }
                }
                if (funObj != null) { return funObj; }

                bool wasEmpty = (funObj == null);

                int cutOutArgsLength = (args.Length > 0) ? smartSplitJson(argIsString ? (extractString(args[0])) : args[0], ' ').Length : 0;

                foreach (Function f in functions[funcName])
                {
                    int ad = (f.isExtensionMethod ? 1 : 0);

                    bool entityFalse = false;
                    bool lambdaFalse = false;
                    for (int i = 0; i < args.Length; i++)
                    {
                        int index = i + ad;
                        if (f.lazy && f.args.Count > index && f.args[index].type == Type.ENTITY && !context.isEntity(args[i]))
                        {
                            entityFalse = true;
                        }
                    }
                    if (f.lazy && f.args.All(x => x.type != Type.FUNCTION) && lambda)
                    {
                        lambdaFalse = true;
                    }

                    if (!lambdaFalse && !entityFalse && funObj == null && f.lazy && args.Length >= f.args.Count - ad && args.Length <= f.args.Count - ad)
                    {
                        funObj = f;
                    }
                    if (!lambdaFalse && !entityFalse && wasEmpty && f.lazy && args.Length == 1 && f.args.Count - ad > 1
                        && cutOutArgsLength >= f.args.Count - ad
                        && cutOutArgsLength <= f.args.Count - ad)
                    {
                        funObj = f;
                    }
                }
                if (funObj != null) { return funObj; }
                if (funObj == null)
                {
                    foreach (Function f in functions[funcName])
                    {
                        foreach(Argument a in f.args)
                        {
                            if (a.type == Type.JSON || a.type == Type.PARAMS)
                            {
                                return f;
                            }
                        }
                    }
                }
                if (funObj == null)
                {
                    string a = "";
                    foreach (string ar in args)
                    {
                        try
                        {
                            a += getExprType(ar) + " " + ar + ", ";
                        }
                        catch
                        {
                            a += "???" + ar;
                        }
                    }
                    throw new Exception("No function Found for " + funcName + " with args:" + a);
                }

                return funObj;
            }
        }
        private static Function GetFunction(string funcName, List<Argument> args)
        {
            Function funObj = null;
            if (functions[funcName].Count == 1)
            {
                funObj = functions[funcName][0];
            }
            else
            {
                foreach (Function f in functions[funcName])
                {
                    if (funObj == null)
                    {
                        bool isGood = true;
                        for (int i = 0; i < args.Count; i++)
                        {
                            if (args[i].type != f.args[i].type)
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            funObj = f;
                            break;
                        }
                    }
                }
                foreach (Function f in functions[funcName])
                {
                    if (funObj == null)
                    {
                        bool isGood = true;
                        for (int i = 0; i < args.Count; i++)
                        {
                            if (args[i].type != f.args[i].type && args[i].type != Type.INT &&
                                f.args[i].type != Type.INT && args[i].type != Type.FLOAT && f.args[i].type != Type.FLOAT)
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            funObj = f;
                            break;
                        }
                    }
                }
                if (funObj == null)
                {
                    throw new Exception("No function Found");
                }
            }
            return funObj;
        }
        private static Predicate GetPredicate(string name, string[] args)
        {
            List<Predicate> preds = predicates[name];
            foreach (Predicate p in preds)
            {
                if (p.args.Length == args.Length)
                {
                    return p;
                }
            }
            throw new Exception("No Predicate Found for " + name + " " + args.Length.ToString());
        }
        private static bool IsFunction(string text)
        {
            return text.Contains("(") && context.IsFunction(text.Substring(0, text.IndexOf('('))) && !text.Contains("<") && !text.Contains("=") && !text.Contains(">");
        }

        public static string GetFunctionGRP(Function func)
        {
            string grp = func.GetInternalFunctionTypeString();
            return grp;
        }
        public static string GetFunctionGRP(Variable func)
        {
            string grp = func.GetInternalFunctionTypeString();
            return grp;
        }
        public static void CreateFunctionGRP(Variable variable, string grp)
        {
            File newfFile = new File("__multiplex__/" + grp, "");
            functDelegated.Add(grp, new List<Function>());
            functDelegatedFile.Add(grp, new List<File>() { newfFile });
            files.Add(newfFile);

            if (!variables.ContainsKey("__mux__" + grp))
            {
                AddVariable("__mux__" + grp, new Variable("__mux__" + grp, "__mux__" + grp, Type.INT));
            }

            int k = 0;
            foreach (Argument arg in variable.args)
            {
                arg.CopyTo("__mux__." + grp + "." + k.ToString(), "", false);
                k++;
            }
            k = 0;
            foreach (Variable arg in variable.outputs)
            {
                arg.CopyTo("__mux__." + grp + ".ret_" + k.ToString(), "", false);
                k++;
            }
        }
        public static void CreateFunctionGRP(Function variable, string grp)
        {
            File newfFile = new File("__multiplex__/" + grp, "");
            functDelegated.Add(grp, new List<Function>());
            functDelegatedFile.Add(grp, new List<File>() { newfFile });
            files.Add(newfFile);

            if (!variables.ContainsKey("__mux__" + grp))
            {
                AddVariable("__mux__" + grp, new Variable("__mux__" + grp, "__mux__" + grp, Type.INT));
            }

            int k = 0;
            foreach (Argument arg in variable.args)
            {
                arg.CopyTo("__mux__." + grp + "." + k.ToString(), "", false);
                k++;
            }
            k = 0;
            foreach (Variable arg in variable.outputs)
            {
                arg.CopyTo("__mux__." + grp + ".ret_" + k.ToString(), "", false);
                k++;
            }
        }
        public static void AddFunctionToGRPFile(Variable variable, Function func, string grp, File fFile)
        {
            string args = "";
            int k = 0;
            foreach (Argument arg in variable.args)
            {
                args += "__mux__." + grp + "." + k.ToString() + ",";
            }
            if (args.Length > 0)
                args = args.Substring(0, args.Length - 1);

            if (!variables.ContainsKey("__mux__" + grp))
            {
                AddVariable("__mux__" + grp, new Variable("__mux__" + grp, "__mux__" + grp, Type.INT));
            }
            Variable mux = GetVariable("__mux__" + grp);
            int id = functDelegated[grp].IndexOf(func);
            string cond = getCondition(mux.gameName + "== " + id.ToString());
            foreach (string line in parseLine(func.gameName.Replace("/", ".").Replace(":", ".") + "(" + args + ")").Split('\n'))
            {
                if (line != "" && !line.StartsWith("#"))
                {
                    fFile.AddLine(cond + line);
                }
            }
            if (func.outputs.Count == 0)
            {
                if (func.file.UnparsedFunctionFile || func.isAbstract)
                {
                    func.file.addParsedLine("if (" + mux.gameName + "== " + id.ToString() + "){" + mux.gameName + "=-1}");
                }
                else
                {
                    func.file.AddLine(cond + Core.VariableOperation(mux, -1, "="));
                }
            }
            int i = 0;
            foreach (Variable outputVar in variable.outputs)
            {
                string line = parseLine("__mux__." + grp + ".ret_" + i.ToString() + "=" + func.outputs[i].gameName);
                if (line != "" && !line.StartsWith("#"))
                {
                    fFile.AddLine(cond + line);
                }
                i++;
            }
        }
        public static void AddFunctionToGRP(Variable variable, Function func, string grp)
        {
            functDelegated[grp].Add(func);
            List<File> fFiles = functDelegatedFile[grp];
            muxAdding = true;
            if ((functDelegated[grp].Count - 1) % (compilerSetting.TreeMaxSize) == 0 && functDelegated[grp].Count > 1)
            {
                Variable mux = GetVariable("__mux__" + grp);
                if (fFiles.Count == 1)
                {
                    string muxedFileNameInit = "__multiplex__/" + grp + "/__splitted_" + (fFiles.Count - 1).ToString();
                    int lowerBound1 = (compilerSetting.TreeMaxSize * (fFiles.Count - 1));
                    int upperBound1 = (compilerSetting.TreeMaxSize * fFiles.Count) - 1;

                    File newfFileInit = new File(muxedFileNameInit, "");
                    newfFileInit.AddLine(fFiles[0].content);
                    fFiles.Add(newfFileInit);
                    files.Add(newfFileInit);

                    fFiles[0].content = "";


                    string cond1 = getCondition(mux.gameName + "== " + lowerBound1.ToString() + ".." + upperBound1.ToString());
                    fFiles[0].AddLine(cond1 + "function " + context.getRoot() + Core.FileNameSplitter()[0] + muxedFileNameInit);
                }

                string muxedFileName = "__multiplex__/" + grp + "/__splitted_" + (fFiles.Count - 1).ToString();
                int lowerBound = (compilerSetting.TreeMaxSize * (fFiles.Count - 1));
                int upperBound = (compilerSetting.TreeMaxSize * fFiles.Count) - 1;

                File newfFile = new File(muxedFileName, "");
                fFiles.Add(newfFile);
                files.Add(newfFile);

                string cond = getCondition(mux.gameName + "== " + lowerBound.ToString() + ".." + upperBound.ToString());
                fFiles[0].AddLine(cond + "function " + context.getRoot() + Core.FileNameSplitter()[0] + muxedFileName);
            }

            File fFile = fFiles[fFiles.Count - 1];
            AddFunctionToGRPFile(variable, func, grp, fFile);
            muxAdding = false;
        }
        public static int GetFunctionIndex(Variable variable, Function func)
        {
            string grp = GetFunctionGRP(func);
            func.file.use();
            if (!functDelegated.ContainsKey(grp))
            {
                CreateFunctionGRP(variable, grp);
            }
            if (!functDelegated[grp].Contains(func))
            {
                AddFunctionToGRP(variable, func, grp);
            }
            return functDelegated[grp].IndexOf(func);
        }
        public static string CallFunctionGRP(Variable funObj, string func, string grp, string[] args, string[] outVar = null)
        {
            string output = parseLine("__mux__" + grp + " = " + func);

            int i = 0;
            foreach (Argument a in funObj.args)
            {
                if (args.Length <= i)
                {
                    output += eval(a.defValue, GetVariable("__mux__." + grp + "." + i.ToString()), a.type, "=");
                }
                else
                {
                    output += eval(args[i], GetVariable("__mux__." + grp + "." + i.ToString()), a.type, "=");
                }
                i++;
            }

            output += getCondition($"{funObj.gameName}!=null") + "function " + context.getRoot() + Core.FileNameSplitter()[0] + "__multiplex__/" + grp + '\n';
            if (outVar != null)
            {
                for (int j = 0; j < outVar.Length; j++)
                {
                    var v = GetVariableByName(outVar[j]);
                    output += eval("__mux__." + grp + ".ret_" + j.ToString(), GetVariableByName(outVar[j]), v.type, "=");
                }
            }
            return output;
        }
        public static string GetLambdaFunctionArgs(string[] args1, List<Argument> args)
        {
            string anonymusFuncNameArg = "(";
            int i = 0;
            foreach (Argument v in args)
            {
                if (args1 != null && i < args1.Length)
                {
                    args1[i] = smartExtract(args1[i]);
                    string[] s = smartSplit(args1[i], ' ');
                    anonymusFuncNameArg += v.GetInternalTypeString() + " " + s[s.Length - 1];
                }
                else
                {
                    anonymusFuncNameArg += v.GetInternalTypeString() + " " + v.name;
                }

                if (i < args.Count - 1)
                    anonymusFuncNameArg += ", ";
                i++;
            }
            anonymusFuncNameArg += ")";
            return anonymusFuncNameArg;
        }
        public static string GetLambdaFunctionReturn(List<Variable> outputs)
        {
            int i = 0;
            string func = "";
            foreach (Variable v in outputs)
            {
                func += v.GetInternalTypeString();
                if (i < outputs.Count - 1)
                    func += ", ";
            }
            return func;
        }

        public static File CreateFunctionTag(string tag)
        {
            if (tag == "ticking")
            {
                return mainFile;
            }
            else if (tag == "loading")
            {
                return loadFile;
            }
            else if (!functions.ContainsKey(Project + ".__tags__." + tag.ToLower()))
            {
                functionTags.Add(tag.ToLower(), new List<string>());

                File tagFile = new File("__tags__/" + tag.Replace(".", "/").ToLower());
                tagFile.notUsed = true;
                Function tagFunc = new Function(tag.ToLower(), Project + ":__tags__/" + tag.Replace(".", "/").ToLower(), tagFile);
                files.Add(tagFile);
                List<Function> f = new List<Function>();
                f.Add(tagFunc);
                functions.Add(Project + ".__tags__." + tag.ToLower(), f);

                return tagFile;
            }
            else
            {
                return GetFunction(context.GetFunctionName("__tags__." + tag.ToLower()), new string[] { }).file;
            }
        }
        public static void AddToFunctionTag(Function func, string tag, string className = null)
        {
            bool wasUsed = func.file.notUsed;

            File f = CreateFunctionTag(tag);
            if (className == null)
            {
                AddLineToFileWithFunctionPriority(func, f, parseLine(func.gameName.Replace(":", ".").Replace("/", ".") + "()"));
            }
            else
            {
                AddLineToFileWithFunctionPriority(func, f, Core.As("@e[tag=" + className + "]") + "function " + func.gameName + "/w_0");
            }

            functionTags[tag.ToLower()].Add(func.gameName.Replace(":", ".").Replace("/", "."));
            func.file.notUsed = wasUsed;
            f.addChild(func.file);
        }
        public static void AddLineToFileWithFunctionPriority(Function func, File file, string line)
        {
            if (func.attributes?.Contains("startfile")==true)
            {
                file.AddStartLine(line);
            }
            else if (func.attributes?.Contains("endfile")==true)
            {
                file.AddEndLine(line);
            }
            else
            {
                file.AddLine(line);
            }
        }
        #endregion

        #region string
        private static void stringInit()
        {
            File fFile = new File("__multiplex__/sstring");
            stringPool = fFile;
            fFile.notUsed = true;
            Function function = new Function("sstring", Project + ":__multiplex__/sstring", fFile);
            List<Function> lst = new List<Function>();
            lst.Add(function);
            functions.Add((Project + ".__multiplex__.sstring").ToLower(), lst);
            files.Add(fFile);

            Argument b = new Argument("__strSelector__", "__multiplex__.sstring.__strSelector__", Type.STRING);
            Variable c = new Variable("__strSelector__", "__multiplex__.sstring.__strSelector__", Type.STRING);
            variables.Add("__multiplex__.sstring.__strSelector__", c);
            function.args.Add(b);
        }
        public static bool isStringVar(string val)
        {
            return context.GetVariable(val, true) != null && variables[context.GetVariable(val)].type == Type.STRING;
        }
        public static string getString(string val)
        {
            Variable v = variables[context.GetVariable(val)];
            stringPool.use();
            return parseLine("__multiplex__.sSTRING(" + v.gameName + ")");
        }
        private static int getStringID(string text)
        {
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            while (text.EndsWith(" "))
                text = text.Substring(0, text.Length - 1);
            if (text.StartsWith("\""))
                text = text.Substring(1, text.Length - 2);

            if (!stringSet.Contains(text))
                stringSet.Add(text);

            return stringSet.IndexOf(text);
        }
        public static string extractString(string text)
        {
            string tmp = smartExtract(text);
            StringBuilder sb = new StringBuilder(tmp.Length-2);
            bool ignoreNext = false;
            for (int i = 1; i < tmp.Length - 1; i++)
            {
                if (tmp[i] == '\\' && ignoreNext)
                {
                    sb.Append(tmp[i]);
                    ignoreNext = false;
                }
                else if (tmp[i] == '\\' && !ignoreNext)
                {
                    ignoreNext = true;
                }
                else
                {
                    sb.Append(tmp[i]);
                    ignoreNext = false;
                }
            }

            return sb.ToString();
        }
        public static bool isString(string text)
        {
            string tmp = smartEmpty(text);
            return tmp.StartsWith("\"") && tmp.EndsWith("\"");
        }
        #endregion

        #region instantiation
        public static string InstComment(string text, bool inLine)
        {
            if (compilerSetting.opti_ExportComment)
            {
                return "#" + text.Substring(2, text.Length - 2);
            }
            else
            {
                return "";
            }
        }
        public static string InstCompilerVar(string text)
        {
            string[] operations = new string[] { "^", "|", "&", "+", "-", "%", "/", "*" };
            string op = "=";
            string[] splited = smartSplit(text, '=', 1);
            foreach (string c in operations)
            {
                if (text.Contains(c) && text.IndexOf(c) == text.IndexOf("=") - 1)
                {
                    splited = smartSplit(text.Replace(c + "=", "="), '=', 1);
                    op = c;
                }
            }
            string[] field = smartSplit(smartExtract(splited[0].Replace("define ", "")), ' ');
            string name = field[field.Length - 1];
            string value = smartExtract(splited[1]);
            
            if (value.StartsWith("regex"))
            {
                if (text.ToLower().StartsWith("define"))
                    value = compVarReplace(value);
                string[] argget = getArgs(value);
                Func<string, string> destringyfie = x => isString(x) ? extractString(x) : x;

                Regex reg = new Regex(destringyfie(argget[1]));
                string newvalue = reg.Replace(compVarReplace(argget[0]), destringyfie(argget[2]));
                context.compVal[context.compVal.Count - 1].Add(name, newvalue);
                return "";
            }
            else if (value.StartsWith("indexfromenum"))
            {
                if (text.ToLower().StartsWith("define"))
                    value = compVarReplace(value);
                string[] argget = getArgs(value);
                Enum enu = enums[getEnum(argget[0])];
                
                context.compVal[context.compVal.Count - 1].Add(name, enu.IndexOf(smartExtract(argget[1].ToLower())).ToString());
                
                return "";
            }
            else if (value.StartsWith("fromenum"))
            {
                if (text.ToLower().StartsWith("define"))
                    value = compVarReplace(value);
                string[] argget = getArgs(value);
                Enum enu = enums[getEnum(argget[0])];
                List<string> sortedField = new List<string>();
                sortedField.AddRange(enu.fieldsName);
                sortedField.Sort((a, b) => b.Length.CompareTo(a.Length));
                context.compVal[context.compVal.Count - 1].Add(name, enu.GetValueOf(smartExtract(argget[1].ToLower())));
                foreach (string f in sortedField)
                {
                    context.compVal[context.compVal.Count - 1].Add(name + "." + f, enu.GetFieldOf(smartExtract(argget[1].ToLower()), f));
                }
                return "";
            }
            else if (value.StartsWith("fromconst"))
            {
                if (text.ToLower().StartsWith("define"))
                    value = compVarReplace(value);
                string[] argget = getArgs(value);
                var var = GetVariableByName(argget[0]);
                context.compVal[context.compVal.Count - 1].Add(name, var.constValue);
                return "";
            }
            else if (value.StartsWith("(") && field[0] != "json")
            {
                if (text.ToLower().StartsWith("define"))
                    value = compVarReplace(value);
                string[] argget = getArgs(value);
                for (int i = 0; i < argget.Length; i++)
                {
                    context.compVal[context.compVal.Count - 1].Add(name + "." + i.ToString(), smartExtract(argget[i]));
                }
                context.compVal[context.compVal.Count - 1].Add(name + ".count", argget.Length.ToString());
                context.compVal[context.compVal.Count - 1].Add(name, value);
                
                return "";
            }
            else if (value.StartsWith("$") && ((structStack.Count > 0 && !isInStructMethod) || structCompVarPointer != null) && op == "=")
            {
                if (structCompVarPointer != null)
                {
                    value = compVarReplace(value);

                    structCompVarPointer[name] = compVarReplace(value);
                }
                else if (structStack.Count > 0 && !isInStructMethod)
                {
                    structStack.Peek().compField.Add(name, value);
                }
                
                return "";
            }
            else if (value.StartsWith("$"))
            {
                value = compVarReplace(value);
            }

            Type type;
            try
            {
                type = getType(text.ToLower().Replace("define ", ""));
            }
            catch
            {
                type = Type.DEFINE;
                GlobalDebug(text.ToLower().Replace("define ", ""), Color.Yellow);
            }

            if (structStack.Count > 0 && !isInStructMethod)
            {
                structStack.Peek().compField.Add(name, value);
                return "";
            }
            else
            {
                if (context.compVal.Count == 0)
                {
                    context.compVal.Add(new Dictionary<string, string>());
                }
                
                if (type == Type.INT || type == Type.FUNCTION || type == Type.FLOAT || type == Type.DEFINE)
                {
                    Variable valVar = GetVariableByName(value, true);
                    if (valVar != null)
                    {
                        context.compVal[context.compVal.Count - 1].Add(name + ".enums", valVar.enums);
                        context.compVal[context.compVal.Count - 1].Add(name + ".type", valVar.GetTypeString());
                        context.compVal[context.compVal.Count - 1].Add(name + ".name", valVar.gameName);
                        context.compVal[context.compVal.Count - 1].Add(name + ".scoreboard", valVar.scoreboard());
                        context.compVal[context.compVal.Count - 1].Add(name + ".scoreboardname", valVar.scoreboard().Split(' ')[1]);
                    }
                    if (type == Type.FUNCTION)
                    {
                        context.compVal[context.compVal.Count - 1].Add(name + ".name", functions[context.GetFunctionName(value)][0].gameName);
                    }
                    context.compVal[context.compVal.Count - 1].Add(name, smartExtract(value));
                }
                else if (type == Type.JSON)
                {
                    if (op == "=")
                        context.compVal[context.compVal.Count - 1].Add(name, compVarReplace(value));
                    if (op == "+")
                    {
                        for (int i = context.compVal.Count - 1; i >= 0; i--)
                        {
                            if (context.compVal[i].ContainsKey(name))
                            {
                                string src = context.compVal[i][name];
                                context.compVal[i][name] = jsonAppend(src, compVarReplace(value));
                                break;
                            }
                        }
                    }
                }
                else if (type == Type.STRING)
                {
                    if (op == "=")
                        context.compVal[context.compVal.Count - 1].Add(name, value);
                    if (op == "+")
                    {
                        for (int i = context.compVal.Count - 1; i >= 0; i--)
                        {
                            if (context.compVal[i].ContainsKey(name))
                            {
                                string src = context.compVal[i][name];
                                context.compVal[i][name] = src + value;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Variable valVar = GetVariableByName(value);
                    if (valVar != null)
                    {
                        context.compVal[context.compVal.Count - 1].Add(name + ".scoreboard", valVar.scoreboard());
                        context.compVal[context.compVal.Count - 1].Add(name + ".enums", valVar.enums);
                        context.compVal[context.compVal.Count - 1].Add(name + ".type", valVar.GetTypeString());
                        context.compVal[context.compVal.Count - 1].Add(name + ".name", valVar.gameName);
                    }
                    context.compVal[context.compVal.Count - 1].Add(name, valVar.gameName);
                }
                return "";
            }
        }
        public static string InstVar(string text)
        {
            string origText = text;
            string vari;
            Type ca = Type.INT;
            bool isConst = false;
            bool isPrivate = false;
            bool isStatic = false;

            if (text.StartsWith("private "))
            {
                isPrivate = true;
                text = text.Substring("private".Length, text.Length - "private".Length);
                while (text.StartsWith(" "))
                {
                    text = text.Substring(1, text.Length - 1);
                }
            }
            if (text.StartsWith("static "))
            {
                isStatic = true;
                text = text.Substring("static".Length, text.Length - "static".Length);
                while (text.StartsWith(" "))
                {
                    text = text.Substring(1, text.Length - 1);
                }
            }
            if (text.StartsWith("const "))
            {
                isConst = true;
                text = text.Substring(5, text.Length - 5);
                while (text.StartsWith(" "))
                {
                    text = text.Substring(1, text.Length - 1);
                }
            }
            if (valExtReg.Match(origText).Success)
            {
                if (text.StartsWith("val") || text.StartsWith("let"))
                    isConst = true;

                if (smartContains(origText, '='))
                {
                    string[] textPart = smartSplit(text, '=', 2);
                    if (smartContains(textPart[1], ','))
                    {
                        textPart[1] = smartSplit(textPart[1], '=')[0];
                    }
                    string evalType = getExprTypeStr(smartExtract(textPart[textPart.Length - 1]));
                    var match = valReg.Match(origText);

                    bool varEntity = textPart[0].ToUpper().Substring(0, 3) == textPart[0].Substring(0, 3);
                    if (varEntity)
                    {
                        evalType = evalType.ToUpper();
                    }
                    if (isConst)
                    {
                        evalType = "const " + evalType;
                    }
                    var outputInst = regReplace(origText, match, evalType);
                    GlobalDebug(outputInst, Color.Yellow);
                    return InstVar(outputInst);
                }
                else
                {
                    throw new Exception("val must be asigned");
                }
            }
            if (text.StartsWith("implicite "))
            {
                text = text.Substring("implicite".Length, text.Length - "implicite".Length);
                while (text.StartsWith(" "))
                {
                    text = text.Substring(1, text.Length - 1);
                }
            }
            if (text.StartsWith("out "))
            {
                text = text.Substring("out".Length, text.Length - "out".Length);
                while (text.StartsWith(" "))
                {
                    text = text.Substring(1, text.Length - 1);
                }
            }
            string orgText = text;
            text = functionDesugar(text);

            ca = getType(text);

            bool entity = orgText.ToUpper().Substring(0, 3) == orgText.Substring(0, 3);

            string op = opReg.Match(text).Value;

            Variable variable;
            string def = "dummy";

            string[] splited = op != "" ?
                                smartSplit(text.Replace(op, "="), '=', entity ? 2 : 1) :
                                smartSplit(text, '=', entity ? 2 : 1);
            string[] left = smartSplit(splited[0], ' ', 1);
            vari = smartEmpty(left[left.Length - 1]);

            if (splited.Length > 1)
                def = splited[1].Replace(" ", "");

            bool entityFormatVar = false;
            if (double.TryParse(def, out double tmp) || ca == Type.STRUCT)
            {
                def = "dummy";
                entityFormatVar = true;
            }

            int part = splited.Length;

            string output = "";
            int index = 0;
            string[] defValue = null;

            if (!entity && splited.Length > 1)
            {
                defValue = smartSplit(splited[1], ',');
            }
            else if (entity && part == 3)
            {
                defValue = smartSplit(splited[2], ',');
            }
            else if (entity && part == 2 && entityFormatVar)
            {
                defValue = smartSplit(splited[1], ',');
            }
            bool instantiated = false;

            foreach (string v in smartSplit(vari, ','))
            {
                string prefix = context.GetVar();
                string name = v.StartsWith(".") ? v.Substring(1, v.Length - 1) : prefix + v;
                if (varWord.ContainsKey(currentFile))
                    varWord[currentFile].Add(v);

                if (isStatic)
                {
                    name = name.Replace("__struct__", "");
                    prefix = prefix.Replace("__struct__", "");
                }

                if (variables.ContainsKey(name))
                {
                    variables.Remove(name);
                }

                if (ca == Type.ARRAY)
                {
                    var mat = arraySizeReg.Matches(text).Cast<Match>().Last();
                    string arraySizeS = mat.Value.Replace("[", "").Replace("]", "");
                    int arraySize = int.Parse(arraySizeS);

                    variable = new Variable(v, name, ca, entity, def);
                    variable.isConst = isConst;
                    variable.arraySize = arraySize;
                    variable.isPrivate = isPrivate;
                    variable.isStatic = isStatic;
                    variable.privateContext = context.GetVar();
                    
                    AddVariable(name, variable);

                    string typeArray = smartExtract(text.Substring(0, mat.Index));
                    variable.typeArray = typeArray;

                    if (structStack.Count > 0 && !isInStructMethod && !isStatic)
                    {
                        structStack.Peek().addField(variable);
                    }
                    else
                    {
                        variable.CreateArray();
                    }
                }
                else if (ca != Type.STRUCT)
                {
                    variable = new Variable(v, name, ca, entity, def);
                    variable.isConst = isConst;
                    variable.isPrivate = isPrivate;
                    variable.isStatic = isStatic;
                    variable.privateContext = prefix;

                    AddVariable(name, variable);
                    if (ca == Type.ENUM)
                    {
                        string enu = getEnum(text);
                        variable.SetEnum(enu);
                        enums[enu].GenerateVariable(variable.name);
                    }
                    if (structStack.Count > 0 && !isInStructMethod && !isStatic)
                    {
                        structStack.Peek().addField(variable);
                    }

                    if (ca == Type.FUNCTION && text.Contains(">") && text.Contains("<"))
                    {
                        InstFunctionVar(variable, splited[0], name);
                    }
                }
                else
                {
                    variable = new Variable(v, name, ca, entity, "__class_id__");
                    variable.isConst = isConst;
                    variable.isPrivate = isPrivate;
                    variable.isStatic = isStatic;
                    variable.privateContext = prefix;
                    
                    AddVariable(name, variable);

                    if (getStruct(text) == null)
                        throw new Exception("No struct in " + text);

                    variable.SetEnum(getStruct(text));

                    if (structStack.Count > 0 && !isInStructMethod && !isStatic)
                    {
                        structStack.Peek().addField(variable);
                    }

                    if (structStack.Count == 0)
                    {
                        string strucName = getStruct(text);
                        if (strucName.Contains("["))
                        {
                            strucName = strucName.Substring(0, strucName.IndexOf("["));
                        }
                        if (part == 2 && defValue != null && defValue.Length > index && defValue[index] != null
                            && (smartExtract(defValue[index]).ToLower().StartsWith(strucName) ||
                            smartExtract(defValue[index]).ToLower().StartsWith("new " + strucName)))
                        {
                            string instArg = $"({getArg(defValue[index])})";

                            output += structs[getStruct(text)].generate(v, entity, variable, instArg);
                            instantiated = true;
                        }
                        else
                        {
                            output += structs[getStruct(text)].generate(v, entity, variable);
                        }
                    }
                }

                string typeStr = variable.GetInternalTypeString();
                if (ExtensionMethod.ContainsKey(typeStr))
                {
                    foreach (Function fun in ExtensionMethod[typeStr])
                    {
                        fun.LinkCopyTo(variable.name, true);
                    }
                }

                if (isConst)
                {
                    variable.constValue = splited[1];
                    variable.UnparsedFunctionFileContext = prefix;
                    variable.wasSet = true;
                }

                if (ca == Type.STRUCT && part == 2 && defValue[index].Replace(" ", "").ToLower().StartsWith(getStruct(text)))
                {
                    index = (index + 1) % defValue.Length;
                }
            }
            if (!structGenerating)
                attributes = new List<string>();
            if (context.currentFile().type != "struct" && ((!entity && splited.Length > 1) ||
                     (entity && part == 2 && entityFormatVar)) && !instantiated && !isConst)
            {
                output += modVar(vari + op + splited[1]);
            }
            if (context.currentFile().type != "struct" && ((entity && part == 3)) && !instantiated)
            {
                output += modVar(vari + op + splited[2]);
            }
            
            return output;
        }
        public static void InstFunctionVar(Variable variable, string text, string name)
        {
            string[] typeArg = smartSplit(text.Substring(text.IndexOf("<") + 1, text.LastIndexOf(">") - text.IndexOf("<") - 1), ',');
            if (typeArg.Length > 0)
            {
                int i = 0;
                foreach (string s in getArgs(typeArg[0]))
                {
                    Type type = getType(s + " ");
                    if (type == Type.VOID)
                    {
                        break;
                    }
                    Argument arg = new Argument(i.ToString(), name + "." + i.ToString(), type);
                    Variable var2 = new Variable(i.ToString(), name + "." + i.ToString(), type);
                    if (type == Type.FUNCTION)
                    {
                        InstFunctionVar(var2, s, name + "." + i.ToString());
                    }
                    AddVariable(name + "." + i.ToString(), var2);
                    variable.args.Add(arg);

                    if (type == Type.STRUCT)
                    {
                        var2.SetEnum(getStruct(s + " "));
                        arg.SetEnum(getStruct(s + " "));
                    }
                    if (type == Type.ENUM)
                    {
                        var2.SetEnum(getEnum(s + " "));
                        arg.SetEnum(getEnum(s + " "));
                    }

                    i++;
                }
            }
            if (typeArg.Length > 1)
            {
                int i = 0;

                foreach (string s in getArgs(typeArg[1]))
                {
                    Type type = getType(s + " ");
                    if (type == Type.VOID)
                    {
                        break;
                    }
                    Variable var2 = new Variable(i.ToString(), name + ".ret_" + i.ToString(), type);
                    if (type == Type.FUNCTION)
                    {
                        InstFunctionVar(var2, s, name + "." + i.ToString());
                    }
                    AddVariable(name + ".ret_" + i.ToString(), var2);
                    variable.outputs.Add(var2);

                    if (type == Type.STRUCT)
                    {
                        var2.SetEnum(getStruct(s + " "));
                    }
                    if (type == Type.ENUM)
                    {
                        var2.SetEnum(getEnum(s + " "));
                    }

                    i++;
                }
            }
        }
        public static string InstFuncDesc(string text)
        {
            if (isInFunctionDesc)
            {
                functionDesc += "#" + text + "\n";
                if (text.Contains("\"\"\""))
                {
                    isInFunctionDesc = false;
                }
            }
            else
            {
                isInFunctionDesc = true;
                functionDesc = "#" + text + "\n";
            }
            return "";
        }
        public static string InstFunc(string text)
        {
            if (!text.StartsWith("def"))
                text = "def " + text;

            string[] funArgType;// = smartSplit(text.Substring(text.IndexOf("def ") + 4, text.IndexOf('(') - text.IndexOf("def ") - 4).ToLower(),' ');

            funArgType = smartSplit(funArgTypeReg.Match(text).Value, ' ');

            string func = funArgType[funArgType.Length - 1];
            func = func.Substring(0, func.Length - 1).ToLower();
            if (func.EndsWith(" ") || func.EndsWith("("))
            {
                func = func.Substring(0, func.Length - 1);
            }
            func = FunctionNameSimple(func);

            bool isStatic = false;
            bool lazy = false;
            bool isAbstract = false;
            bool isTicking = false;
            bool isLoading = false;
            bool isHelper = false;
            bool isPrivate = false;
            bool isLambda = false;
            bool isExternal = false;
            bool isVirtual = false;
            bool isOverride = false;
            bool isStackSafe = false;
            bool isInvisible = false;
            bool isInfix = false;
            bool isPrefix = false;

            string arg = getArg(smartSplit(text, ':')[0]);
            string[] args = smartSplit(arg, ',');


            List<string> outputType = new List<string>();
            List<string> tags = new List<string>();
            if (funArgType.Length > 1)
            {
                for (int i = 0; i < funArgType.Length - 1; i++)
                {
                    if (funArgType[i] == "def") { }
                    else if (funArgType[i] == "static")
                    {
                        isStatic = true;
                    }
                    else if (funArgType[i] == "helper")
                    {
                        isHelper = true;
                    }
                    else if (funArgType[i] == "ticking")
                    {
                        isTicking = true;
                    }
                    else if (funArgType[i] == "loading")
                    {
                        isLoading = true;
                    }
                    else if (funArgType[i] == "external")
                    {
                        isExternal = true;
                    }
                    else if (funArgType[i] == "invisible")
                    {
                        isInvisible = true;
                    }
                    else if (funArgType[i] == "__lambda__")
                    {
                        isLambda = true;
                    }
                    else if (funArgType[i] == "lazy")
                    {
                        lazy = true;
                    }
                    else if (funArgType[i].ToLower() == "stacksafe")
                    {
                        isStackSafe = true;
                    }
                    else if (funArgType[i] == "abstract")
                    {
                        isAbstract = true;
                        if (structStack.Count > 0)
                        {
                            isVirtual = true;
                        }
                    }
                    else if (funArgType[i] == "private")
                    {
                        isPrivate = true;
                    }
                    else if (funArgType[i] == "public")
                    {
                        isPrivate = false;
                    }
                    else if (funArgType[i] == "virtual")
                    {
                        isVirtual = true;
                    }
                    else if (funArgType[i] == "override")
                    {
                        isOverride = true;
                    }
                    else if (funArgType[i] == "infix")
                    {
                        isInfix = true;
                    }
                    else if (funArgType[i] == "prefix")
                    {
                        isPrefix = true;
                    }
                    else if (funArgType[i].StartsWith("@"))
                    {
                        tags.Add(funArgType[i].Replace("@", ""));
                    }
                    else
                    {
                        getType(funArgType[i]);
                        outputType.Add(funArgType[i]);
                    }
                }
            }
            isAbstract = isExternal || isAbstract;

            string fullName = (isExternal ? "" : context.GetFun().Replace(":", "/")) + func.Replace(".", "/");

            if (fullName.Contains("/"))
                fullName = fullName.Substring(0, fullName.IndexOf("/")) + ":" +
                            fullName.Substring(fullName.IndexOf("/") + 1, fullName.Length - fullName.IndexOf("/") - 1);

            string funcID = fullName.Replace(':', '.').Replace('/', '.');
            string subName = fullName.Substring(fullName.IndexOf(":") + 1, fullName.Length - fullName.IndexOf(":") - 1);

            if (isStatic)
            {
                fullName = fullName.Replace("__struct__", "");
                funcID = funcID.Replace("__struct__", "");
                subName = subName.Replace("__struct__", "");
            }

            File fFile = new File(subName);
            Function function = new Function(func, fullName, fFile);

            function.desc = functionDesc;
            functionDesc = "";
            fFile.function = function;

            function.isLoading = isLoading;
            function.isTicking = isTicking;
            function.isHelper = isHelper;
            function.tags = tags;
            function.isPrivate = isPrivate;
            function.isLambda = isLambda;
            function.isExternal = isExternal;
            function.isVirtual = isVirtual;
            function.isOverride = isOverride;
            function.isExtensionMethod = ExtensionClassStack.Count > 0;
            function.isStatic = true;
            function.isStackSafe = isStackSafe;
            function.attributes = attributes;
            function.isInfix = isInfix;
            function.isPrefix = isPrefix;

            if (!structGenerating)
                attributes = new List<string>();

            if (ExtensionClassStack.Count > 0)
            {
                if (!ExtensionMethod.ContainsKey(ExtensionClassStack.Peek()))
                    ExtensionMethod[ExtensionClassStack.Peek()] = new List<Function>();
                ExtensionMethod[ExtensionClassStack.Peek()].Add(function);
            }

            function.privateContext = context.GetVar();

            if (isOverride)
            {
                if (!functions.ContainsKey(funcID))
                {
                    if (structStack.Count > 0)
                    {
                        List<Function> lst = new List<Function>();
                        lst.Add(function);
                        functions.Add(funcID, lst);
                        if (structStack.Peek().isClass)
                            function.isVirtual = true;
                    }
                    else
                    {
                        throw new Exception("Nothing to Override for " + funcID + "!");
                    }
                }
                else
                {
                    bool contain = true;
                    while (contain)
                    {
                        function.gameName = function.gameName + "_";
                        fFile.name = fFile.name + "_";
                        func = func + "_";

                        contain = false;
                        for (int j = 0; j < functions[funcID].Count; j++)
                        {
                            Function f = functions[funcID][j];
                            if (f.gameName == function.gameName && f != function)
                            {
                                contain = true;

                                if (f.args.Count == 0 && args.Length == 0)
                                {
                                    function.gameName = f.gameName;
                                    fFile.name = f.file.name;
                                    func = f.name;
                                    functions[funcID].Remove(f);
                                    break;
                                }
                            }
                        }
                    }
                    functions[funcID].Add(function);
                }
            }
            else if (!functions.ContainsKey(funcID))
            {
                List<Function> lst = new List<Function>();
                lst.Add(function);
                functions.Add(funcID, lst);
            }
            else if (functions[funcID][0].isAbstract)
            {
                Function prev = functions[funcID][0];
                fFile.notUsed = prev.file.notUsed;

                function.isLoading = prev.isLoading || isLoading;
                function.isTicking = prev.isTicking || isTicking;
                function.isHelper = prev.isHelper || isHelper;
                function.tags.AddRange(prev.tags);
                function.isPrivate = prev.isPrivate || isPrivate;
                function.isLambda = prev.isLambda || isLambda;
                function.isExternal = prev.isExternal || isExternal;
                fFile.parsed_end = prev.file.parsed;
            }
            else
            {
                functions[funcID].Add(function);
                bool contain = true;
                while (contain)
                {
                    function.gameName = function.gameName + "_";
                    fFile.name = fFile.name + "_";
                    func = func + "_";

                    contain = false;
                    foreach (Function f in functions[funcID])
                    {
                        if (f.gameName == function.gameName && f != function)
                        {
                            contain = true;

                            if (f.args.Count == 0 && args.Length == 0)
                            {
                                throw new Exception(funcID + " already exists");
                            }
                        }
                    }
                }
            }
            function.lazy = lazy;
            function.isAbstract = isAbstract;

            structMethodFile = fFile;
            fFile.isLazy = true;

            if (!isAbstract)
            {
                files.Add(fFile);
            }
            context.Sub(func, fFile);

            fFile.UnparsedFunctionFile = !lazy && structStack.Count == 0 && !isAbstract;
            fFile.UnparsedFunctionFileContext = context.GetVar();

            if (structStack.Count > 0 && !isStatic)
            {
                function.file.type = "strucMethod";
                isInStructMethod = true;
                structMethodFile = fFile;
                fFile.notUsed = true;
                function.isStructMethod = true;
                isInLazyCompile++;
            }
            if (structStack.Count > 0 && isStatic)
            {
                function.file.type = "staticMethod";
                isInStaticMethod = true;
            }


            string[] tmp = text.Split(':');
            if (tmp.Length == 2)
            {
                if (outputType.Count > 0)
                    throw new Exception("Illegal Syntax");
                string[] ret = smartSplit(tmp[1].Replace(" ", ""),',');
                int i = 0;
                foreach (string a in ret)
                {
                    string r = functionDesugar(a);
                    Type ca = getType(r + " ");
                    if (ca != Type.VOID)
                    {
                        parseLine(smartEmpty(r.Replace("{", "")) + " ret_" + i.ToString());
                        function.outputs.Add(GetVariableByName("ret_" + i.ToString()));
                    }
                    i++;
                }
            }
            if (outputType.Count > 0)
            {
                int i = 0;
                foreach (string a in outputType)
                {
                    string r = functionDesugar(a);
                    Type ca = getType(r + " ");
                    if (ca != Type.VOID)
                    {
                        parseLine(smartEmpty(r.Replace("{", "")) + " ret_" + i.ToString());
                        function.outputs.Add(GetVariableByName("ret_" + i.ToString()));
                    }
                    i++;
                }
            }

            foreach (string z in args)
            {
                string a = functionDesugar(smartExtract(z));
                string c = a.Replace("=", " ");

                while (c.StartsWith(" "))
                {
                    c = c.Substring(1, c.Length - 1);
                }
                while (c.Contains("  "))
                {
                    c = c.Replace("  ", " ");
                }
                bool implicite = false;
                if (c.StartsWith("implicite "))
                {
                    implicite = true;
                    c = c.Substring(10, c.Length - 10);
                }


                if (c.Contains(' '))
                {
                    string t = smartSplit(c, ' ')[0];
                    string name = smartSplit(c, ' ')[1];

                    Type type = getType(a + " ");

                    Argument b = new Argument(name, context.GetInput() + name, type);
                    b.isImplicite = implicite;
                    if (a.Contains("="))
                    {
                        b.defValue = smartSplit(a, '=', 1)[1];
                    }
                    else
                    {
                        function.argNeeded++;
                    }
                    function.maxArgNeeded++;

                    if (type == Type.ENUM)
                    {
                        b.SetEnum(getEnum(t + " "));
                    }
                    if (type == Type.STRUCT)
                    {
                        b.SetEnum(getStruct(t + " "));
                    }
                    
                    parseLine(a.Replace("implicite ", ""));
                    b.variable = GetVariableByName(name, true);
                    if (type == Type.ARRAY)
                    {
                        Variable variable = GetVariable(context.GetInput() + name);
                        b.arraySize = variable.arraySize;
                    }
                    
                    function.args.Add(b);
                }
            }

            if (structStack.Count > 0 && !isStatic)
            {
                structStack.Peek().addMethod(function);
            }

            if (isAbstract)
            {
                context.Parent();
            }
            if (!(structStack.Count > 0 && !lazy) && !isAbstract)
            {
                isInLazyCompile = 1;
            }

            if (!isAbstract)
                autoIndent(text);

            if (function.args.Count > 0 && ((tags.Count > 0 && !tags[0].StartsWith("__")) || isTicking || isLoading))
            {
                throw new Exception("Tagged Function cannot have arguments.");
            }

            if (function.args.Count == 1 && function.args[0].type == Type.FUNCTION)
                funcDefF.Add(subName.Replace("/", "."));
            else if (function.args.Count > 1 && function.args[function.args.Count-1].type == Type.FUNCTION)
                funcDefM.Add(subName.Replace("/", "."));
            else if (function.lazy)
                funcDefL.Add(subName.Replace("/", "."));
            else
                funcDef.Add(subName.Replace("/", "."));

            if (!(structStack.Count > 0 && !isStatic))
            {
                if (isLoading)
                {
                    AddToFunctionTag(function, "loading");
                    fFile.use();
                    if (callStackDisplay)
                        callTrace += "\"load\"->\"" + function.gameName + "\"\n";
                }

                if (isTicking)
                {
                    AddToFunctionTag(function, "ticking");
                    fFile.use();
                    if (callStackDisplay)
                        callTrace += "\"main\"->\"" + function.gameName + "\"\n";
                }

                if (isHelper)
                {
                    fFile.use();
                    if (callStackDisplay)
                        callTrace += "\"helper\"->\"" + function.gameName + "\"\n";
                }


                foreach (string tag in tags)
                {
                    if (!tag.StartsWith("__"))
                    {
                        AddToFunctionTag(function, tag);
                        if (callStackDisplay)
                            callTrace += "\"@" + tag.ToLower() + "\"->\"" + function.gameName + "\"\n";
                    }
                }
            }
            fFile.notUsed |= isInvisible;
            return "";
        }
        public static string InstWhile(string text, string fText)
        {
            string loop = getCondition(text);

            int wID = While.GetID(context.GetFun());
            string funcName = context.GetFun() + "u_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().AddLine(loop + cmd);

            File fFile = new File(context.GetFile() + "u_" + wID, "", "while");
            fFile.AddEndLine(loop + cmd);
            context.Sub("u_" + wID, fFile);
            files.Add(fFile);

            autoIndent(fText);

            return "";
        }
        public static string InstIf(string text, string fText, int mult = 0, string extra = "")
        {
            string loop = getCondition(((mult >= 2 && (LastCond.wasAlwayFalse || LastCond.wasAlwayTrue)) ? "" : extra) + text);

            If wID = new If();
            If wID2 = new If(-1);
            If wID_else = new If(-1);

            wID.wasAlwayTrue = loop == "";
            wID.wasAlwayFalse = (loop.Contains(ConditionAlwayFalse) || (LastConds.Count > 0 && LastCond.wasAlwayTrue && LastCond.id > -1));

            wID2.wasAlwayFalse = wID.wasAlwayFalse;
            wID2.wasAlwayTrue = wID.wasAlwayTrue;
            
            wID_else.wasElseAlwayTrue = mult == 3 && (wID.wasAlwayTrue || LastCond.wasAlwayFalse);

            string funcName = context.GetFun() + "i_" + wID.id.ToString();

            string cmd = "function " + funcName + '\n';
            if (mult == 2 && LastCond.wasAlwayFalse && !wID.wasAlwayFalse && !wID.wasAlwayTrue)
            {
                context.currentFile().AddLine(parseLine("int __elseif_" + LastCond.id.ToString() + " = 0"));
                LastConds.Pop();
                LastConds.Push(wID2);
            }
            else if (mult == 2)
            {
                wID2 = new If(LastCond.id);

                wID2.wasAlwayFalse = wID.wasAlwayFalse;
                wID2.wasAlwayTrue = wID.wasAlwayTrue;
                LastConds.Pop();
                LastConds.Push(wID2);
            }
            else if (mult == 1)
            {
                wID2 = new If();

                wID2.wasAlwayFalse = wID.wasAlwayFalse;
                wID2.wasAlwayTrue = wID.wasAlwayTrue;

                if (!wID.wasAlwayFalse && !wID.wasAlwayTrue)
                    context.currentFile().AddLine(parseLine("int __elseif_" + wID2.id.ToString() + " = 0"));
                LastConds.Push(wID2);
            }
            else if (mult == 0)
            {
                LastConds.Push(wID2);
            }
            else if (mult == 3)
            {
                LastConds.Push(wID_else);
            }
            if (wID.wasAlwayTrue) {  }
            else if (wID.wasAlwayFalse || (mult >= 2 && LastCond.wasAlwayTrue)) { }
            else { context.currentFile().AddLine(loop + cmd); }

            context.currentFile().cantMergeWith = true;
            if (wID.wasAlwayFalse)
                isInLazyCompile++;

            File fFile = new File(context.GetFile() + "i_" + wID.id, "", "if");
            context.Sub("i_" + wID.id, fFile);


            if (wID.wasAlwayFalse || (mult >= 2 && LastCond.wasAlwayTrue))
            {
                context.currentFile().notUsed = true;
                context.currentFile().type = "if_empty";
            }
            if (mult == 1 && !wID.wasAlwayFalse && !wID.wasAlwayTrue)
            {
                context.currentFile().AddLine(parseLine("__elseif_" + wID2.id.ToString() + " = 1"));
            }
            if (mult == 2 && !wID.wasAlwayFalse && !wID.wasAlwayTrue)
            {
                context.currentFile().AddLine(parseLine("__elseif_" + LastCond.id.ToString() + " = 1"));
            }

            files.Add(fFile);

            autoIndent(fText);

            return "";
        }
        public static string InstElseIf(string text, string fText)
        {
            if (LastCond.id < 0)
                throw new Exception("No if statement to apply else");
            LastConds.Push(LastCond);
            return InstIf(text, fText, 2, "__elseif_" + LastCond.id.ToString() + " == 0 &&");
        }
        public static string InstElse(string fText)
        {
            if (LastCond.id < 0)
                throw new Exception("No if statement to apply else");

            string ret = InstIf("", fText, 3, "__elseif_" + LastCond.id.ToString() + " == 0");
            LastCond.id = -1;
            return ret;
        }
        public static string InstFor(string text, string fText)
        {
            string[] args = smartSplit(text.Replace(",", ";"), ';');

            File f = context.currentFile();

            int wID = For.GetID(context.GetFun());
            string funcName = context.GetFun() + "f_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            File fFile = new File(context.GetFile() + "f_" + wID, "", "while");
            context.Sub("f_" + wID, fFile);
            files.Add(fFile);

            f.AddLine(InstVar(args[0]));
            string loop = getCondition(args[1]);
            f.AddLine(loop + cmd);
            fFile.AddEndLine(parseLine(args[2]) + loop + cmd);

            autoIndent(fText);

            return "";
        }
        public static string InstForGenerate(string text, string fText)
        {
            if (!isInStructMethod)
            {
                string[] args = smartSplit(text.Replace(";", ","), ',');

                File f = context.currentFile();

                int wID = Forgenerate.GetID(context.GetFun());

                File fFile = new File(context.GetFile() + "g_" + wID, "", "forgenerate");

                fFile.var = smartEmpty(args[0]);
                string args1Extra = smartExtract(args[1]);
                if (enums.ContainsKey(args1Extra.ToLower()))
                {
                    fFile.enumGen = args1Extra.ToLower();
                }
                else if (args1Extra.StartsWith("files("))
                {
                    fFile.enumGen = args1Extra;
                }
                else if (args1Extra.StartsWith("("))
                {
                    fFile.enumGen = args1Extra;
                }
                else if (args.Length == 2)
                {
                    fFile.enumGen = args1Extra;
                }
                else
                {
                    fFile.min = double.Parse(smartEmpty(args[1]));
                    fFile.max = double.Parse(smartEmpty(args[2]));
                    if (args.Length > 3)
                        fFile.step = double.Parse(smartEmpty(args[3]));
                    else
                        fFile.step = 1;
                }

                context.Sub("g_" + wID, fFile);
                files.Add(fFile);
                isInLazyCompile = 1;
            }
            return "";
        }
        public static string InstWith(string[] text, string fText)
        {
            if (context.isEntity(text[0]))
            {
                string pre;
                if (text.Length > 2)
                {
                    string[] v = getConditionSplit(text[2]);
                    if (smartEmpty(text[1]) == "true")
                        pre = v[0] + Core.AsAt(context.GetEntitySelector(text[0]), v[1]);
                    else
                        pre = v[0] + Core.As(context.GetEntitySelector(text[0]), v[1]);
                }
                else
                {
                    if (text.Length > 1 && smartEmpty(text[1]) == "true")
                        pre = Core.AsAt(context.GetEntitySelector(text[0]));
                    else
                        pre = Core.As(context.GetEntitySelector(text[0]));
                }

                int wID = With.GetID(context.GetFun());
                string funcName = context.GetFun() + "w_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "w_" + wID, "", "with");
                context.Sub("w_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                int wID = With.GetID(context.GetFun());
                File fFile = new File(context.GetFile() + "w_" + wID, "", "withContext");
                context.Sub("w_" + wID, fFile);
                files.Add(fFile);
                List<ImpliciteVar> lst = new List<ImpliciteVar>();
                foreach (var l in text)
                {
                    lst.AddRange(getImpliciteFromExpr(l));
                }
                autoIndent(fText);
                //getExprType(text[0])
                //ImpliciteVar var = new ImpliciteVar();
                context.addImpliciteVar(lst);
                return "";
            }
        }
        public static string InstAt(string[] text, string fText)
        {
            if (context.isEntity(text[0]))
            {
                string pre = "";

                if (text.Length > 1)
                {
                    string[] v = getConditionSplit(text[1]);
                    pre = v[0] + Core.At(context.GetEntitySelector(text[0]), v[1]);
                }
                else
                {
                    pre = Core.At(context.GetEntitySelector(text[0]));
                }


                int wID = At.GetID(context.GetFun());
                string funcName = context.GetFun() + "a_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "a_" + wID, "", "at");
                context.Sub("a_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else if (text[0].Split(' ').Length == 3
                && (double.TryParse(text[0].Split(' ')[0], out double _a) || text[0].Split(' ')[0].StartsWith("~") || text[0].Split(' ')[0].StartsWith("^"))
                && (double.TryParse(text[0].Split(' ')[1], out double _b) || text[0].Split(' ')[1].StartsWith("~") || text[0].Split(' ')[1].StartsWith("^"))
                && (double.TryParse(text[0].Split(' ')[2], out double _c) || text[0].Split(' ')[2].StartsWith("~") || text[0].Split(' ')[2].StartsWith("^")))
            {
                string pre = Core.Positioned(text[0]);

                int wID = At.GetID(context.GetFun());
                string funcName = context.GetFun() + "a_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "a_" + wID, "", "at");
                context.Sub("a_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string InstPositioned(string text, string fText)
        {
            if (text.Split(' ').Length == 3 && !text.Contains(",")
                && (double.TryParse(text.Split(' ')[0], out double _a) || text.Split(' ')[0].StartsWith("~") || text.Split(' ')[0].StartsWith("^"))
                && (double.TryParse(text.Split(' ')[1], out double _b) || text.Split(' ')[1].StartsWith("~") || text.Split(' ')[1].StartsWith("^"))
                && (double.TryParse(text.Split(' ')[2], out double _c) || text.Split(' ')[2].StartsWith("~") || text.Split(' ')[2].StartsWith("^")))
            {

                if (isString(text))
                    text = extractString(text);

                string pre = Core.Positioned(text);

                int wID = At.GetID(context.GetFun());
                string funcName = context.GetFun() + "a_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "a_" + wID, "", "at");
                context.Sub("a_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string InstAligned(string text, string fText)
        {
            if (text.Split(' ').Length == 1)
            {
                string pre = Core.Align(smartEmpty(text));

                int wID = At.GetID(context.GetFun());
                string funcName = context.GetFun() + "a_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "a_" + wID, "", "at");
                context.Sub("a_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string InstEnum(string text)
        {
            string[] field = smartSplit(text, '=');
            string[] subField1 = smartSplit(field[0], ' ');
            bool final = false;
            bool overriding = false;
            bool isPrivate = false;

            string name = "";
            string contextName = context.GetVar();
            string[] fields = null;
            for (int i = 0; i < subField1.Length; i++)
            {
                if (subField1[i] == "final")
                {
                    final = true;
                }
                else if (subField1[i] == "override")
                {
                    overriding = true;
                }
                else if (subField1[i] == "private" || subField1[i] == "")
                {
                    isPrivate = true;
                }
                else if (subField1[i] == "public")
                {
                    isPrivate = false;
                }
                else if (subField1[i] == "enum" || subField1[i] == "")
                {

                }
                else if (name == "")
                {
                    name = subField1[i];
                    if (name.Contains("("))
                    {
                        fields = getArgs(name);
                        name = name.ToLower().Substring(0, name.IndexOf('('));
                    }
                    else
                    {
                        name = name.ToLower();
                    }
                }
                else
                {
                    throw new Exception("Unknown keyword: " + subField1[i]);
                }
            }
            if (overriding && !enums.ContainsKey(contextName + name))
                throw new Exception("Nothing to Override for Enum:" + contextName + name);

            if (overriding && enums.ContainsKey(contextName + name))
                enums.Remove(name);

            if (enums.ContainsKey(contextName + name))
            {
                foreach (string s in smartSplit(field[1], ','))
                {
                    enums[contextName + name].Add(s);
                }
            }
            else
            {
                Enum e = null;
                if (fields != null)
                {
                    e = new Enum(contextName + name, fields, smartSplit(field[1], ','), final, isPrivate);
                }
                else
                {
                    e = new Enum(contextName + name, smartSplit(field[1], ','), final, isPrivate);
                }
                string dir = "";
                for (int i = contextName.Split('.').Length - 2; i >= 0; i--)
                {
                    enums[dir + name] = e;
                    dir = contextName.Split('.')[i] + "." + dir;
                }
                enums[dir + name] = e;
            }


            string varNames = (contextName + name).ToLower();
            if (!variables.ContainsKey(varNames + ".length"))
            {
                AddVariable(varNames + ".length", new Variable("length", varNames + ".length", Type.INT, false));
                variables[varNames + ".length"].isConst = true;
                variables[varNames + ".length"].UnparsedFunctionFileContext = varNames;
            }

            variables[varNames + ".length"].constValue = enums[contextName + name].values.Count.ToString();

            return "";
        }
        public static string InstEnumFile(string text)
        {
            string[] field = smartSplit(text, '=');
            string[] subField1 = smartSplit(field[0], ' ');
            bool final = false;
            bool overriding = false;
            bool isPrivate = false;
            string contextName = context.GetVar();

            for (int i = 0; i < subField1.Length; i++)
            {
                if (subField1[i] == "final")
                {
                    final = true;
                }
                else if (subField1[i] == "override")
                {
                    overriding = true;
                }
                else if (subField1[i] == "private")
                {
                    isPrivate = true;
                }
                else if (subField1[i] == "public")
                {
                    isPrivate = false;
                }
                else if (subField1[i].StartsWith("enum") || subField1[i] == "")
                {

                }
                else
                {
                    throw new Exception("Unknown keyword: " + subField1[i]);
                }
            }
            string[] args = getArgs(text);
            string name = extractString(args[0]);

            string file = extractString(args[1]);
            string type = "???";

            if (args.Length > 2)
            {
                type = extractString(args[2]);
            }
            else
            {
                if (name.EndsWith(".init"))
                    type = "init";
                if (name.EndsWith(".csv"))
                    type = "csv";
            }

            if (overriding && enums.ContainsKey(name))
                enums.Remove(name);

            if (!resourceFiles.ContainsKey(file))
                throw new Exception("Unknown resource " + file);
            var e = EnumConverter.GetEnum(name, resourceFiles[file], type, final, isPrivate);

            string dir = "";
            for (int i = contextName.Split('.').Length - 2; i >= 0; i--)
            {
                enums[dir + name] = e;
                dir = contextName.Split('.')[i] + "." + dir;
            }
            enums[dir + name] = e;

            try
            {
                InstCompilerVar("int $" + name + ".length=" + enums[name].values.Count.ToString());
            }
            catch { }
            string varNames = (contextName + name).ToLower();
            if (!variables.ContainsKey(varNames + ".length"))
            {
                AddVariable(varNames + ".length", new Variable("length", varNames + ".length", Type.INT, false));
                variables[varNames + ".length"].isConst = true;
                variables[varNames + ".length"].UnparsedFunctionFileContext = varNames;
            }

            variables[varNames + ".length"].constValue = enums[contextName + name].values.Count.ToString();

            return "";
        }
        public static string InstBlockTag(string text)
        {
            string[] field = smartSplit(smartEmpty(text), '=');

            string name = (compilerSetting.tagsFolder ? context.GetVar() : "") +
                            smartEmpty(field[0].ToLower().Replace("blocktags", ""));

            if (field.Length > 1)
            {
                if (blockTags.ContainsKey(name))
                {
                    foreach (string value in smartSplit(field[1].ToLower(), ','))
                    {
                        if (value.StartsWith("-"))
                        {
                            if (blockTags[name].values.Contains(value.Substring(1, value.Length - 1)))
                                blockTags[name].values.Remove(value.Substring(1, value.Length - 1));
                        }
                        else if (value.StartsWith("+"))
                        {
                            if (!blockTags[name].values.Contains(value.Substring(1, value.Length - 1)))
                                blockTags[name].values.Add(value.Substring(1, value.Length - 1));
                        }
                        else
                        {
                            if (!blockTags[name].values.Contains(value))
                                blockTags[name].values.Add(value);
                        }
                    }
                }
                else
                {
                    blockTags.Add(name, new TagsList(new List<string>(smartSplit(field[1].ToLower(), ','))));
                }
            }
            else if (!blockTags.ContainsKey(name))
            {
                blockTags.Add(name, new TagsList(new List<string>()));
            }
            return "";
        }
        public static string InstEntityTag(string text)
        {
            string[] field = smartSplit(smartEmpty(text), '=');

            string name = (compilerSetting.tagsFolder ? context.GetVar() : "") +
                            smartEmpty(field[0].ToLower().Replace("entitytags", ""));

            if (field.Length > 1)
            {
                if (entityTags.ContainsKey(name))
                {
                    foreach (string value in smartSplit(field[1].ToLower(), ','))
                    {
                        if (value.StartsWith("-"))
                        {
                            if (entityTags[name].values.Contains(value.Substring(1, value.Length - 1)))
                                entityTags[name].values.Remove(value.Substring(1, value.Length - 1));
                        }
                        else if (value.StartsWith("+"))
                        {
                            if (!entityTags[name].values.Contains(value.Substring(1, value.Length - 1)))
                                entityTags[name].values.Add(value.Substring(1, value.Length - 1));
                        }
                        else
                        {
                            if (!entityTags[name].values.Contains(value))
                                entityTags[name].values.Add(value);
                        }
                    }
                }
                else
                {
                    entityTags.Add(name, new TagsList(new List<string>(smartSplit(field[1].ToLower(), ','))));
                }
            }
            else if (!entityTags.ContainsKey(name))
            {
                entityTags.Add(name, new TagsList(new List<string>()));
            }
            return "";
        }
        public static string InstItemTag(string text)
        {
            string[] field = smartSplit(smartEmpty(text), '=');

            string name = smartEmpty(field[0].ToLower().Replace("itemtags", ""));
            if (field.Length > 1)
            {
                if (itemTags.ContainsKey(name))
                {
                    foreach (string value in smartSplit(field[1].ToLower(), ','))
                    {
                        if (value.StartsWith("-"))
                        {
                            if (itemTags[name].values.Contains(value.Substring(1, value.Length - 1)))
                                itemTags[name].values.Remove(value.Substring(1, value.Length - 1));
                        }
                        else if (value.StartsWith("+"))
                        {
                            if (!itemTags[name].values.Contains(value.Substring(1, value.Length - 1)))
                                itemTags[name].values.Add(value.Substring(1, value.Length - 1));
                        }
                        else
                        {
                            if (!itemTags[name].values.Contains(value))
                                itemTags[name].values.Add(value);
                        }
                    }
                }
                else
                {
                    itemTags.Add(name, new TagsList(new List<string>(smartSplit(field[1].ToLower(), ','))));
                }
            }
            else if (!itemTags.ContainsKey(name))
            {
                itemTags.Add(name, new TagsList(new List<string>()));
            }
            return "";
        }
        public static string InstSwitch(string[] text, string fText)
        {
            if (text.Length >= 1)
            {
                int wID = Switch.GetID(context.GetFun());

                if (text.Length == 2)
                {
                    switches.Push(new Switch(text[0], text, wID));
                }
                else
                {
                    switches.Push(new Switch(text[0], wID));
                }

                string funcName = context.GetFun() + "s_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                File fFile = new File(context.GetFile() + "s_" + wID, "", "switch");
                context.Sub("s_" + wID, fFile);
                files.Add(fFile);
                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string InstCase(string text)
        {
            if (switches.Count == 0)
            {
                throw new Exception("Invalide use of case statement.");
            }
            int wID = switches.Peek().Count();
            string funcName = context.GetFun() + "i_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().cantMergeWith = true;
            File fFile = new File(context.GetFile() + "i_" + wID, "", "case");
            fFile.switchcase = switches.Peek().Add(text, cmd);
            context.Sub("i_" + wID, fFile);
            files.Add(fFile);

            return "";
        }
        public static string InstSmartCase(string text)
        {
            text = text.Replace("->", "§");
            string[] part = smartSplit(text, '§', 1);

            if (switches.Count == 0)
            {
                throw new Exception("Invalide use of case statement.");
            }

            int wID = switches.Peek().Count();
            string funcName = context.GetFun() + "i_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().cantMergeWith = true;
            File fFile = new File(context.GetFile() + "i_" + wID, "", "case");
            fFile.switchcase = switches.Peek().Add(part[0], cmd);
            context.Sub("i_" + wID, fFile);
            files.Add(fFile);

            fFile.AddLine(parseLine(part[1]));
            fFile.Close();

            return "";
        }
        public static string InstStruct(string text)
        {
            string[] split = smartSplit(text.ToLower().Replace("{", ""), ' ');
            string name = "";
            string functionBase = null;
            File functionBaseFile = null;
            bool isPrivate = false;
            Structure parent = null;
            bool isClass = false;
            bool isStatic = false;
            bool isAbstract = false;
            bool isInterface = false;
            List<Structure> interfaces = new List<Structure>();

            int mode = 0;
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] == "private" && mode == 0)
                {
                    isPrivate = true;
                }
                else if (split[i] == "public" && mode == 0)
                {
                    isPrivate = false;
                }
                else if (split[i] == "abstract" && mode == 0)
                {
                    isAbstract = false;
                }
                else if (split[i] == "static" && mode == 0)
                {
                    isStatic = false;
                }
                else if (split[i] == "extends")
                {
                    mode = 1;
                }
                else if (split[i] == "implements")
                {
                    mode = 3;
                }
                else if (split[i] == "initer")
                {
                    mode = -1;
                }
                else if (split[i] == "struct" && mode == 0)
                {
                    isClass = false;
                    mode = 2;
                }
                else if (split[i] == "class" && mode == 0)
                {
                    isClass = true;
                    mode = 2;
                }
                else if (split[i] == "interface" && mode == 0)
                {
                    isInterface = true;
                    isAbstract = true;
                    isClass = true;
                    mode = 2;
                }
                else if (mode == 1)
                {
                    parent = structs[split[i]];
                }
                else if (mode == 3)
                {
                    if (!structs[split[i]].isInterface)
                        throw new Exception($"{split[i]} is not an interface.");
                    interfaces.Add(structs[split[i]]);
                }
                else if (mode == 2)
                {
                    mode = 0;
                    name = split[i];
                }
                else if (mode == -1)
                {
                    functionBase = split[i];
                }
            }
            if (name == "")
                throw new Exception("No Name for struct/class");
            if (isClass && functionBase != null)
            {
                if (functionBase.StartsWith("minecraft:"))
                {
                    functionBaseFile = new File("", "");
                    functionBaseFile.addParsedLine("/summon " + functionBase + " ~ ~ ~ {Tags:[\"__class__\",\"cls_trg\"]}");
                    functionBaseFile.addParsedLine("with(@e[tag=cls_trg]){");
                    functionBaseFile.addParsedLine("/tag @s remove cls_trg");
                    functionBaseFile.addParsedLine("__CLASS__ = __class__");
                    functionBaseFile.addParsedLine("__ref++");
                    functionBaseFile.addParsedLine("}");
                }
                else
                {
                    functionBaseFile = GetFunction(context.GetFunctionName(functionBase), new List<Argument>()).file;
                }
            }

            string generics = "";
            if (name.Contains("<"))
            {
                inGenericStruct = true;
                generics = name.Substring(name.IndexOf("<") + 1, name.LastIndexOf(">") - name.IndexOf("<") - 1);
                name = smartEmpty(name.Substring(0, name.IndexOf("<")));
            }

            string[] contextName = context.GetVar().Split('.');
            context.Sub("__struct__" + name, new File("struct/" + name, "", "struct"));

            Structure stru = new Structure(name, parent, interfaces, isPrivate);

            stru.isClass = isClass;
            stru.classInitBase = functionBaseFile;
            stru.isAbstract = isAbstract;
            stru.isStatic = isStatic;
            stru.isInterface = isInterface;

            try
            {
                string dir = "";
                for (int i = contextName.Length - 2; i >= 0; i--)
                {
                    structs[dir + name] = stru;
                    dir = contextName[i] + "." + dir;
                }
                structs[dir + name] = stru;
                structStack.Push(stru);
            }
            catch (Exception e)
            {
                throw new Exception(name + " already exists? " + e.ToString());
            }

            foreach (string gen in smartSplit(generics, ','))
            {
                stru.addGeneric(smartEmpty(gen));
                stru.isGeneric = true;
            }

            thisDef.Push(context.GetVar());

            return "";
        }
        public static string InstExtension(string text)
        {
            string[] split = smartSplit(text.ToLower().Replace("{", ""), ' ');
            string name = split[1].ToLower();

            string generics = "";
            if (name.Contains("<"))
            {
                inGenericStruct = true;
                generics = name.Substring(name.IndexOf("<") + 1, name.LastIndexOf(">") - name.IndexOf("<") - 1);
                name = smartEmpty(name.Substring(0, name.IndexOf("<")));
            }

            Type type = getType(name + " ");
            if (type == Type.ENUM)
            {
                name = getEnum(name + " ");
            }
            if (type == Type.STRUCT)
            {
                name = getStruct(name + " ");
            }

            string[] contextName = context.GetVar().Split('.');
            context.Sub("__extension__" + name, new File("__extension__/" + name, "", "extension"));
            ExtensionClassStack.Push(name);

            return "";
        }
        public static string InstAlias(string text)
        {
            string[] arg = smartSplit(text, ' ');
            string f1 = smartEmpty(arg[3]);
            string f2 = smartEmpty(arg[1]);
            if (arg[2] != "as")
            {
                throw new Exception("Syntax Error");
            }
            if (context.IsFunction(f1))
            {
                string funcID = (context.GetFun() + f2).Replace(':', '.').Replace('/', '.').ToLower();
                if (!functions.ContainsKey(funcID))
                    functions.Add(funcID, functions[context.GetFunctionName(f1.ToLower())]);
                else if (functions[funcID][0].isAbstract)
                {
                    GlobalDebug(functions[funcID][0].gameName + " was overrided", Color.Yellow);
                }
                else
                {
                    throw new Exception(funcID + " already exists");
                }
            }
            else if (context.IsFunction(f2))
            {
                string funcID = (context.GetFun() + f1).Replace(':', '.').Replace('/', '.').ToLower();
                if (!functions.ContainsKey(funcID))
                    functions.Add(funcID, functions[context.GetFunctionName(f2)]);
                else if (functions[funcID][0].isAbstract)
                {
                    GlobalDebug(functions[funcID][0].gameName + " was overrided", Color.Yellow);
                }
                else
                {
                    throw new Exception(funcID + " already exists");
                }
            }
            else if (context.GetVariable(f1, true) != null)
            {
                if (!variables.ContainsKey(context.GetVar() + f2))
                {
                    AddVariable(context.GetVar() + f2, variables[context.GetVariable(f1)]);
                }
            }
            else if (context.GetVariable(f2, true) != null)
            {
                if (!variables.ContainsKey(context.GetVar() + f1))
                {
                    AddVariable(context.GetVar() + f1, variables[context.GetVariable(f2)]);
                }
            }
            else
            {
                string generics = "";
                if (f1.Contains("<"))
                {
                    inGenericStruct = true;
                    generics = f1.Substring(f1.IndexOf("<") + 1, f1.LastIndexOf(">") - f1.IndexOf("<") - 1);
                    string name = smartEmpty(f1.Substring(0, f1.IndexOf("<"))).ToLower();

                    try
                    {
                        structs[name].createGeneric(f2, smartSplit(smartEmpty(generics), ','));
                    }
                    catch (Exception e)
                    {
                        GlobalDebug(e.ToString(), Color.Red);
                    }
                }
                else
                {
                    throw new Exception("Unkown function/variable " + f2);
                }
            }

            return "";
        }
        public static string InstLamba(string text, Variable variable)
        {
            string[] para = text.Replace("=>", "\\").Split('\\');
            string lambda = "lambda_" + Lambda.GetID(context.GetFun()).ToString();
            string func = "def " + lambda;
            string content = "return(";

            if (smartEmpty(para[0]).StartsWith("("))
            {
                para[0] = getArg(para[0]);
            }
            if (smartEmpty(para[1]).StartsWith("("))
            {
                para[1] = getArg(para[1]);
            }
            while (para[0].StartsWith(" "))
            {
                para[0] = para[0].Substring(1, para[0].Length - 1);
            }
            string[] args = smartSplit(para[0], ',');
            if (args.Length > variable.args.Count)
            {
                throw new Exception("To Much argument for Lambda");
            }

            func += GetLambdaFunctionArgs(args, variable.args);
            if (variable.outputs.Count > 0) { func += ":"; }
            func += GetLambdaFunctionReturn(variable.outputs);

            func += "{";

            if (para[1].Contains("return") || variable.outputs.Count == 0)
            {
                content = smartExtract(para[1]);
            }
            else
            {
                content += smartExtract(para[1]) + ")";
            }

            preparseLine(func);
            if (content.StartsWith("{"))
                preparseLine(getCodeBlock(content));
            else
                preparseLine(content);

            preparseLine("}");


            return eval(lambda, variable, variable.type, "=");
        }
        public static string InstPackage(string text)
        {
            string pck = smartEmpty(text.Substring(8, text.Length - 8).Replace(" ", ""));
            currentPackage = pck;
            if (currentPackage != ".")
            {
                File pckFile = context.Package(pck);
                packages.Add(pck);
                if (pckFile != null) files.Add(pckFile);
            }
            else
            {
                context.GoRoot();
            }
            return "";
        }
        public static string InstInLineForgenerate(string text)
        {
            string block = getCodeBlock(text);

            string[] arg = getArgs(text.Substring(0, text.IndexOf('{')));
            string output = "";
            string var = arg[0];
            if (double.TryParse(arg[1], out double _))
            {
                if (arg.Length > 3)
                {
                    int j = 0;
                    int max = (int)Math.Ceiling((double.Parse(arg[1]) - double.Parse(arg[2])) / double.Parse(arg[3]));

                    for (double i = double.Parse(arg[1]); i <= double.Parse(arg[2]); i += double.Parse(arg[3]))
                    {
                        string line = block.Replace(var + ".index", j.ToString())
                        .Replace(var + ".length", max.ToString())
                        .Replace(var, i.ToString());

                        output += line;
                    }
                }
                else
                {
                    int j = 0;
                    int max = (int)Math.Ceiling((double.Parse(arg[1]) - double.Parse(arg[2])));

                    for (double i = double.Parse(arg[1]); i <= double.Parse(arg[2]); i++)
                    {
                        string line = block.Replace(var + ".index", j.ToString())
                        .Replace(var + ".length", max.ToString())
                        .Replace(var, i.ToString());

                        output += line;
                    }
                }
            }
            else
            {
                if (enums != null && getEnum(arg[1]) != null)
                {
                    var en = enums[getEnum(arg[1])];
                    foreach (var value in en.Values())
                    {
                        string line = block;
                        foreach (string field in en.fieldsName)
                        {
                            line = line.Replace(var + "." + field, en.GetFieldOf(value, field));
                        }
                        line = line.Replace(var, value);
                        output += line;
                    }
                }
                else if (enums != null && arg[1].StartsWith("("))
                {
                    string[] values = getArgs(arg[1]);
                    foreach (var value in values)
                    {
                        string line = block;

                        if (value.StartsWith("("))
                        {
                            string[] splitted = getArgs(value);
                            for (int i = splitted.Length - 1; i >= 0; i--)
                            {
                                line = line.Replace(var + "." + i.ToString(),
                                    splitted[i]);
                            }
                        }

                        line = line.Replace(var, value);
                        output += line;
                    }
                }
            }
            return output;
        }
        public static string InstJsonFile(string text)
        {
            string name = "";
            if (text.Contains("\""))
            {
                name = text.Substring(text.IndexOf("\"") + 1, text.LastIndexOf("\"") - text.IndexOf("\"") - 1);
            }
            else
            {
                name = smartSplit(text, ' ', 1)[1].Replace("{", "");
            }
            if (currentParsedFile != null && currentParsedFile.resourcespack)
            {
                string nFn = currentParsedFile.name.Replace("\\", "/");
                if (nFn.Contains("/"))
                {
                    name = nFn.Substring(0, nFn.LastIndexOf("/") + 1) + name;
                }
                resourcespackFiles.Add(new File(name, "", "json"));
                if (text.Contains("{"))
                {
                    resourcespackFiles[resourcespackFiles.Count - 1].AddLine(text.Substring(text.IndexOf("{"), text.Length - text.IndexOf("{")));
                }
                jsonIndent = text.Split('{').Length - text.Split('}').Length;
                return "";
            }
            else
            {
                jsonFiles.Add(new File(name, "", "json"));
                if (text.Contains("{"))
                {
                    jsonFiles[jsonFiles.Count - 1].AddLine(text.Substring(text.IndexOf("{"), text.Length - text.IndexOf("{")));
                }
                jsonIndent = text.Split('{').Length - text.Split('}').Length;
                return "";
            }
        }
        public static string AddToJsonFile(string text)
        {
            if (context.currentFile().type != "forgenerate")
            {
                if (currentParsedFile != null && currentParsedFile.resourcespack)
                {
                    resourcespackFiles[resourcespackFiles.Count - 1].AddLine(text);
                    jsonIndent += text.Split('{').Length - text.Split('}').Length;
                }
                else
                {
                    jsonFiles[jsonFiles.Count - 1].AddLine(text);
                    jsonIndent += text.Split('{').Length - text.Split('}').Length;
                }
            }
            return "";
        }
        public static string InstPredicateFile(string text)
        {
            string name = smartSplit(text, ' ', 1)[1].Replace("{", "");
            name = name.Substring(0, name.IndexOf("("));
            string[] args = getArgs(text);

            File f = new File("predicates/" + context.GetFun().ToLower() + name.ToLower(), "", "json");
            Predicate p = new Predicate(context.GetFun().ToLower() + name.ToLower(), args, f);
            string key = context.GetFun().Replace(":", ".").Replace("/", ".").ToLower() + name.ToLower();

            if (predicates.ContainsKey(key))
            {
                predicates[key].Add(p);
            }
            else
            {
                predicates.Add(key, new List<Predicate>() { p });
            }

            f.notUsed = true;

            jsonFiles.Add(f);
            if (text.Contains("{"))
            {
                jsonFiles[jsonFiles.Count - 1].AddLine(text.Substring(text.IndexOf("{"), text.Length - text.IndexOf("{")));
            }
            jsonIndent = text.Split('{').Length - text.Split('}').Length;
            return "";
        }
        public static string InstAttribute(string text)
        {
            string v = getArrayBlock(text).Value;
            v = v.Substring(1, v.Length - 2);
            attributes.Add(v);
            return "";
        }
        #endregion

        #region Compiler Exception
        public static string Require(string text)
        {
            string[] args = smartSplit(text, ' ');
            if (args.Length > 2)
            {
                string comVar = args[1];
                string value = compVarReplace(comVar);
                string msg = args.Length > 4 ? args[4] : "";

                if (args[2] == "in")
                {
                    if (getEnum(args[3]) == null)
                    {
                        throw new Exception("No such enum: " + args[3]);
                    }
                    if (int.TryParse(value, out int _))
                    {
                        compVarChange(comVar, enums[getEnum(args[3])].valuesName[int.Parse(value)]);
                    }
                    else if (!enums[getEnum(args[3])].valuesName.Contains(value.ToLower()))
                    {
                        throw new Exception("Fail requirement: " + value + " must be in enum " + args[3] + " Message: " + msg);
                    }
                }
                else if (args[2] == "<" || args[2] == "<=" || args[2] == ">=" || args[2] == ">" || args[2] == "==")
                {
                    double valRight = double.Parse(smartExtract(evalDesugar(compVarReplace(args[3]))));
                    double valLeft = double.Parse(smartExtract(evalDesugar(value)));
                    bool valid = false;
                    if (args[2] == "<" && valLeft < valRight) { valid = true; }
                    if (args[2] == "<=" && valLeft <= valRight) { valid = true; }
                    if (args[2] == ">" && valLeft > valRight) { valid = true; }
                    if (args[2] == ">=" && valLeft >= valRight) { valid = true; }
                    if (args[2] == "==" && valLeft == valRight) { valid = true; }

                    if (!valid)
                    {
                        throw new Exception("Fail requirement: " + value + " " + args[2] + " " + args[3] + " Message: " + msg);
                    }
                }
                else if (args[2] == "valid_tag")
                {
                    string extracted = isString(value) ? extractString(value) : value;
                    Regex reg = new Regex(@"\s \(\)\[\]");
                    if (reg.Match(extracted).Success)
                        throw new Exception("Invalid Tags");
                    compVarChange(comVar, extracted);
                }
                else
                {
                    if (getEnum(args[2]) == null)
                    {
                        throw new Exception("No such enum: " + args[2]);
                    }
                    if (int.TryParse(value, out int _))
                    {
                        compVarChange(comVar, enums[getEnum(args[2])].valuesName[int.Parse(value)]);
                    }
                    else if (!enums[getEnum(args[2])].valuesName.Contains(value.ToLower()))
                    {
                        throw new Exception("Fail requirement: " + value + " must be in enum " + args[2]);
                    }
                }
            }
            else
            {
                throw new Exception("No Enough argument for require");
            }
            return "";
        }
        public static string Indexed(string text)
        {
            string[] args = smartSplit(text, ' ');
            if (args.Length > 2)
            {
                string comVar = args[1];
                string value = compVarReplace(comVar);

                if (args[2] == "in")
                {
                    if (getEnum(args[3]) == null)
                    {
                        throw new Exception("No such enum: " + args[3]);
                    }
                    int a;
                    if (int.TryParse(value, out a))
                    {
                        if (a >= enums[getEnum(args[3])].valuesName.Count || a < 0)
                        {
                            throw new Exception("Index Out of Bound");
                        }
                    }
                    else if (enums[getEnum(args[3])].valuesName.Contains(value.ToLower()))
                    {
                        compVarChange(comVar, enums[getEnum(args[3])].valuesName.IndexOf(value).ToString());
                    }
                    else
                    {
                        throw new Exception("Fail requirement: " + value + " must be in enum " + args[3]);
                    }
                }
                else
                {
                    if (getEnum(args[2]) == null)
                    {
                        throw new Exception("No such enum: " + args[2]);
                    }
                    int a;
                    if (int.TryParse(value, out a))
                    {
                        if (a >= enums[getEnum(args[2])].valuesName.Count || a < 0)
                        {
                            throw new Exception("Index Out of Bound");
                        }
                    }
                    else if (enums[getEnum(args[2])].valuesName.Contains(value.ToLower()))
                    {
                        compVarChange(comVar, enums[getEnum(args[2])].valuesName.IndexOf(value).ToString());
                    }
                    else
                    {
                        throw new Exception("Fail requirement: " + value + " must be in enum " + args[3]);
                    }
                }
            }
            else
            {
                throw new Exception("No Enough argument for require");
            }
            return "";
        }
        #endregion

        #region type
        public static Type getType(string t, int recCall = 0)
        {
            t = t.Replace("{", "") + " ";

            while (t.StartsWith(" "))
                t = t.Substring(1, t.Length - 1);

            if (recCall > maxRecCall)
            {
                throw new Exception("Stack Overflow");
            }

            if (typeMaps.Count > 0)
            {
                foreach (string key in typeMaps.Peek().Keys)
                {
                    Regex typeCheck = new Regex($"\\b{key}\\b");
                    if (typeCheck.Match(t).Success)
                        return getType(typeMaps.Peek()[key], recCall + 1);
                }
            }

            Type type;
            
            if (t.Split('=')[0].Contains("[") && t.Split('=')[0].Contains("]"))
            {
                type = Type.ARRAY;
            }
            else if (t.ToLower().StartsWith("int "))
            {
                type = Type.INT;
            }
            else if (t.ToLower().StartsWith("define ") && t.Contains("$"))
            {
                type = Type.DEFINE;
            }
            else if (t.ToLower().StartsWith("json ") && t.Contains("$"))
            {
                type = Type.JSON;
            }
            else if (t.ToLower().StartsWith("params ") && t.Contains("$"))
            {
                type = Type.PARAMS;
            }
            else if (t.ToLower().StartsWith("float "))
            {
                type = Type.FLOAT;
            }
            else if (t.ToLower().StartsWith("bool "))
            {
                type = Type.BOOL;
            }
            else if (t.ToLower().StartsWith("string "))
            {
                type = Type.STRING;
            }
            else if (t.ToLower().StartsWith("function<") || t.ToLower().StartsWith("function "))
            {
                type = Type.FUNCTION;
            }
            else if (t.StartsWith("entity ") && t.Contains("$"))
            {
                type = Type.ENTITY;
            }
            else if (t.StartsWith("void "))
            {
                type = Type.VOID;
            }
            else if (containLazyVal(smartEmpty(t)))
            {
                return getType(getLazyVal(smartEmpty(t)), recCall + 1);
            }
            else if (containEnum(t + " "))
            {
                type = Type.ENUM;
            }
            else if (containStruct(t + " "))
            {
                type = Type.STRUCT;
            }
            else if ((t.ToLower().StartsWith("var ") || t.ToLower().StartsWith("let ") || t.ToLower().StartsWith("val ")))
            {
                if (smartContains(t, '='))
                {
                    string[] part = smartSplit(t, '=', 1);
                    if (smartContains(part[1], ','))
                    {
                        part[1] = smartSplit(part[1], '=')[0];
                    }
                    return getExprType(smartExtract(part[1]));
                }
                else
                {
                    throw new Exception("Let & Var must have a value after.");
                }
            }
            else
            {
                throw new Exception("Unknown type: " + t);
            }
            return type;
        }
        public static Type getExprType(string t, bool entity = false, int recCall = 0)
        {
            if (t == null)
                throw new ArgumentNullException();
            if (recCall > maxRecCall)
            {
                throw new Exception("Stack Overflow!");
            }
            if (t.StartsWith("__lambda__"))
            {
                return Type.FUNCTION;
            }
            if (t.StartsWith("~"))
            {
                return Type.FLOAT;
            }
            if (t.StartsWith("(") && t.EndsWith(")"))
            {
                return getExprType(getParenthis(t), entity, recCall + 1);
            }
            if (t.ToLower() == "true" || t.ToLower() == "false")
            {
                return Type.BOOL;
            }
            if (int.TryParse(t, out int i))
            {
                return Type.INT;
            }

            if (double.TryParse(t, out double f))
            {
                return Type.FLOAT;
            }
            if (t.StartsWith("\""))
            {
                return Type.STRING;
            }
            if (context.GetVariable(t, true) != null)
            {
                return variables[context.GetVariable(t)].type;
            }
            if (t.Contains("("))
            {
                string s = t.Substring(0, t.IndexOf("("));

                if (context.IsFunction(s))
                {
                    string text = t;
                    string arg = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
                    string[] args = smartSplit(arg, ',');
                    string func;
                    if (smartContains(text, '='))
                    {
                        func = smartEmpty(smartSplit(text, '=', 1)[1]).Substring(text.IndexOf(' ') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
                    }
                    else
                    {
                        func = text.Substring(0, text.IndexOf('('));
                    }


                    string funcName = context.GetFunctionName(func);

                    return GetFunction(funcName, args).outputs[0].type;
                }
            }
            if (context.IsFunction(t) && variables.ContainsKey(context.GetFunctionName(t)))
            {
                return variables[context.GetFunctionName(t)].outputs[0].type;
            }
            if (context.IsFunction(t) && functions.ContainsKey(context.GetFunctionName(t)))
            {
                return Type.FUNCTION;
            }
            string ext = smartExtract(t).ToLower();
            foreach (var enu in enums.Values)
            {
                foreach (string val in enu.Values())
                {
                    if (val == ext)
                    {
                        return Type.ENUM;
                    }
                }
            }
            if (smartEmpty(t).StartsWith("-"))
            {
                string v = smartEmpty(t);
                return getExprType(v.Substring(1, v.Length - 1));
            }
            if (t.Contains("*") || t.Contains("+") || t.Contains("-") || t.Contains("/") ||
                t.Contains("^") || t.Contains("|") || t.Contains("&") || t.Contains("%"))
            {
                string[] part;
                string[] operations = new string[] { "^", "|", "&", "+", "-", "%", "/", "*" };
                string val2 = getParenthis(t, 1);
                foreach (string xop in operations)
                {
                    part = smartSplit(val2, xop[0], 2);

                    if (part.Length > 1)
                    {
                        Type a = getExprType(part[0]);
                        Type b = getExprType(part[1]);

                        if (a == b)
                        {
                            return a;
                        }
                        if ((a == Type.FLOAT && b == Type.INT) || (a == Type.INT && b == Type.FLOAT))
                        {
                            return Type.FLOAT;
                        }
                        if ((a == Type.BOOL && (b == Type.INT || b == Type.FLOAT)))
                        {
                            return b;
                        }
                        if ((b == Type.BOOL && (a == Type.INT || a == Type.FLOAT)))
                        {
                            return a;
                        }
                    }
                }
            }

            if (containLazyVal(smartEmpty(t)))
            {
                return getExprType(getLazyVal(smartEmpty(t)));
            }
            if (!context.isEntity(t) && smartContains(t, '[') && arrayVarReg.Match(t).Success && !arrayFuncReg.Match(t).Success)
            {
                var mat = arrayReg.Matches(t).Cast<Match>().Last();
                string vara = regReplace(t, mat, "");
                string para = mat.Value.Substring(1, mat.Value.Length - 2);
                return getExprType($"{smartExtract(vara)}.get({para})");
            }
            string[] spaceSplitted = smartSplitJson(t, ' ');
            if (spaceSplitted.Length >= 3)
            {
                try
                {
                    return getExprType(DesugarOperator(spaceSplitted), entity, recCall+1);
                }
                catch
                {

                }
            }
            if (entity)
            {
                return getType(NBT_Data.getType(t));
            }
            context.GetVariable(t, true, false, 0, true);
            throw new Exception("Unparsable: " + t);
        }
        public static string getExprTypeStr(string t, bool entity = false, int recCall = 0)
        {
            if (recCall > maxRecCall)
            {
                throw new Exception("Stack Overflow!");
            }
            if (t.StartsWith("__lambda__"))
            {
                return Type.FUNCTION.ToString();
            }
            if (t.StartsWith("~"))
            {
                return "float";
            }
            if (t.StartsWith("(") && t.EndsWith(")"))
            {
                return getExprTypeStr(getParenthis(t), entity, recCall + 1);
            }
            if (t.ToLower() == "true" || t.ToLower() == "false")
            {
                return "bool";
            }
            if (int.TryParse(t, out int i))
            {
                return "int";
            }

            if (double.TryParse(t, out double f))
            {
                return "float";
            }
            if (t.StartsWith("\""))
            {
                return "string";
            }
            if (context.GetVariable(t, true) != null)
            {
                return variables[context.GetVariable(t)].GetInternalTypeString();
            }
            if (t.Contains("("))
            {
                string s = t.Substring(0, t.IndexOf("("));

                if (context.IsFunction(s))
                {
                    string text = t;
                    string arg = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
                    string[] args = smartSplit(arg, ',');
                    string func;
                    if (smartContains(text, '='))
                    {
                        func = smartEmpty(smartSplit(text, '=', 1)[1]).Substring(text.IndexOf(' ') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
                    }
                    else
                    {
                        func = text.Substring(0, text.IndexOf('('));
                    }


                    string funcName = context.GetFunctionName(func);

                    return GetFunction(funcName, args).outputs[0].GetInternalTypeString();
                }
            }
            if (context.IsFunction(t) && variables.ContainsKey(context.GetFunctionName(t)))
            {
                return variables[context.GetFunctionName(t)].outputs[0].GetInternalTypeString();
            }
            if (context.IsFunction(t) && functions.ContainsKey(context.GetFunctionName(t)))
            {
                return functions[context.GetFunctionName(t)][0].GetInternalTypeString();
            }
            string ext = smartExtract(t).ToLower();
            foreach (var enu in enums.Values)
            {
                foreach (string val in enu.Values())
                {
                    if (val == ext)
                    {
                        return enu.name;
                    }
                }
            }
            if (smartEmpty(t).StartsWith("-"))
            {
                string v = smartEmpty(t);
                return getExprTypeStr(v.Substring(1, v.Length - 1), entity, recCall+1);
            }
            if (t.Contains("*") || t.Contains("+") || t.Contains("-") || t.Contains("/") ||
                t.Contains("^") || t.Contains("|") || t.Contains("&") || t.Contains("%"))
            {
                string[] part;
                string[] operations = new string[] { "^", "|", "&", "+", "-", "%", "/", "*" };
                string val2 = getParenthis(t, 1);
                foreach (string xop in operations)
                {
                    part = smartSplit(val2, xop[0], 2);

                    if (part.Length > 1)
                    {
                        string a = getExprTypeStr(part[0]);
                        string b = getExprTypeStr(part[1]);

                        if (a == b)
                        {
                            return a;
                        }
                        if ((a == "float" && b == "int") || (a == "int" && b == "float"))
                        {
                            return "float";
                        }
                        if ((a == "bool" && (b == "int" || b == "float")))
                        {
                            return b;
                        }
                        if ((b == "bool" && (a == "int" || a == "float")))
                        {
                            return a;
                        }
                    }
                }
            }

            if (containLazyVal(smartEmpty(t)))
            {
                return getExprTypeStr(getLazyVal(smartEmpty(t)), entity);
            }
            if (!context.isEntity(t) && smartContains(t, '[') && arrayVarReg.Match(t).Success && !arrayFuncReg.Match(t).Success)
            {
                var mat = arrayReg.Matches(t).Cast<Match>().Last();
                string vara = regReplace(t, mat, "");
                string para = mat.Value.Substring(1, mat.Value.Length - 2);
                return getExprTypeStr($"{smartExtract(vara)}.get({para})");
            }
            string[] spaceSplitted = smartSplitJson(t, ' ');
            if (spaceSplitted.Length >= 3)
            {
                try
                {
                    return getExprTypeStr(DesugarOperator(spaceSplitted), entity, recCall + 1);
                }
                catch
                {

                }
            }
            if (entity)
            {
                return NBT_Data.getType(t);
            }
            
            context.GetVariable(t, true, false, 0, true);
            throw new Exception("Unparsable: " + t);
        }
        public static string getExprEnum(string t)
        {
            if (context.GetVariable(t, true) != null)
            {
                return variables[context.GetVariable(t)].enums;
            }
            if (t.Contains("("))
            {
                string s = t.Substring(0, t.IndexOf("("));

                if (context.IsFunction(s))
                {
                    string text = t;
                    string arg = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
                    string[] args = smartSplit(arg, ',');
                    string func;
                    if (smartContains(text, '='))
                    {
                        func = smartEmpty(smartSplit(text, '=', 1)[1]).Substring(text.IndexOf(' ') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
                    }
                    else
                    {
                        func = text.Substring(0, text.IndexOf('('));
                    }


                    string funcName = context.GetFunctionName(func);

                    return GetFunction(funcName, args).outputs[0].enums;
                }
            }
            if (context.IsFunction(t))
            {
                return variables[context.GetFunctionName(t)].outputs[0].enums;
            }
            throw new Exception("Unable to Infer enum");
        }
        public static string getEnum(string text)
        {
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            text = text.Replace("{", " ") + " ";
            if (typeMaps.Count > 0)
            {
                foreach (string key in typeMaps.Peek().Keys)
                {
                    if (text.ToLower().StartsWith(key.ToLower() + " "))
                    {
                        return getEnum(typeMaps.Peek()[key]);
                    }
                }
            }

            foreach (string key in enums.Keys)
            {
                if (text.ToLower().StartsWith(key + " ") &&
                    !(enums[key].isPrivate && !context.GetVar().StartsWith(enums[key].privateContext)))
                {
                    return key;
                }
            }
            return null;
        }
        public static bool containEnum(string text)
        {
            return getEnum(text) != null;
        }
        public static string getStruct(string text)
        {
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            text = text.Replace("{", " ") + " ";
            if (text.Contains("<"))
            {
                if (!structs.ContainsKey(text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]").ToLower()))
                {
                    string generics = text.Substring(text.IndexOf("<") + 1, getCloseCharIndex(text, '>') - text.IndexOf("<") - 1);
                    string name = smartEmpty(text.Substring(0, text.IndexOf("<"))).ToLower();

                    structs[name].createGeneric(text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]"),
                        smartSplit(smartEmpty(generics), ','));
                }
                return text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]").ToLower();
            }
            if (typeMaps.Count > 0)
            {
                foreach (string key in typeMaps.Peek().Keys)
                {
                    if (text.ToLower().StartsWith(key.ToLower() + " ") || text.ToLower().StartsWith(key.ToLower() + "["))
                    {
                        return getStruct(typeMaps.Peek()[key]);
                    }
                }
            }

            foreach (string key in structs.Keys)
            {
                if ((text.ToLower().StartsWith(key.ToLower() + " ") || text.ToLower().StartsWith(key.ToLower() + "[")) &&
                    !(structs[key].isPrivate && !context.GetVar().StartsWith(structs[key].privateContext)))
                {
                    return key;
                }
            }
            return null;
        }
        public static bool containStruct(string text)
        {
            return getStruct(text) != null;
        }
        #endregion

        #region function eval
        public static string functionEval(string text, string[] outVar = null, string op = "=")
        {
            Match _m = shortFuncReg.Match(text);
            if (_m.Success && !text.Substring(0, text.IndexOf('{')).Contains("("))
            {
                text = regReplace(text, _m, _m.Value.Replace("{", "(){"));
            }
            string funcVar = smartExtract(text.Substring(0, text.IndexOf('(')));
            if (CommandParser.canBeParse(funcVar))
            {
                return CommandParser.parse(text, context);
            }

            var var = GetVariableByName(funcVar, true);
            if (var != null && var.type == Type.FUNCTION && !muxAdding)
            {
                return functionVarEval(text, outVar, op);
            }
            else
            {
                string arg = getArg(text);
                string[] args = smartSplitJson(arg, ',');

                string func;
                bool anonymusFunc = false;
                string anonymusFuncName = "";
                string anonymusFuncNameArg = "()";

                if (smartContains(text, '='))
                {
                    func = smartEmpty(smartSplit(text, '=', 1)[1]).Substring(text.IndexOf(' ') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
                }
                else
                {
                    func = text.Substring(0, text.IndexOf('('));
                }

                if (containLazyVal(func))
                    func = getLazyVal(func);

                string output = "";
                if (func[0] == '@')
                {
                    string tag = func.Substring(1, func.Length - 1);
                    CreateFunctionTag(tag);

                    func = "__tags__." + tag.ToLower();
                }

                if (smartContains(text, '[') && arrayFuncReg.Match(text).Success)
                {
                    var mat = arrayFuncReg.Matches(text).Cast<Match>().Last();
                    string vara = text.Substring(0, mat.Index);
                    string newfunc = text.Substring(mat.Index + mat.Length, text.Length - mat.Index - mat.Length);
                    string index = mat.Value.Substring(1, mat.Value.Length - 3);
                    string aparg = getArg(newfunc);
                    string reciepe = outVar == null ? "" : (outVar.Length == 0 ? "" : outVar.Aggregate((x, y) => x + "," + y) + " = ");
                    newfunc = newfunc.Substring(0, newfunc.IndexOf('('));
                    index = index + (aparg == "" ? "" : ("," + aparg));
                    
                    return functionEval($"{reciepe}{vara}.dot_{newfunc}({index})", outVar, op);
                }
                if (smartContains(text, '['))
                {
                    var mat = arrayFunc2Reg.Matches(text).Cast<Match>().Last();
                    string vara = text.Substring(0, mat.Index+mat.Length);
                    preparseLine($"var __tmp = {vara}");
                    return functionEval($"__tmp({getArg(text)})", outVar, op);
                }

                string funcName = context.GetFunctionName(func);

                Function funObj = GetFunction(funcName, args, smartContains(text, '{'));

                if (lazyCall.Contains(funObj) && !funObj.isStackSafe)
                    throw new Exception("Cannot have recursive Lazy Recursive Function.");

                if (funObj.isPrivate)
                {
                    string cont = context.GetVar();
                    bool inPrivateContext = false;
                    foreach (string adj in adjPackage)
                    {
                        if (adj != null && !cont.StartsWith(adj))
                            inPrivateContext = true;
                    }

                    if (!context.GetVar().StartsWith(funObj.privateContext) && !inPrivateContext)
                    {
                        throw new Exception("can not call private function " + funObj.name);
                    }
                }


                if (funObj.isExtensionMethod)
                {
                    string[] args2 = new string[args.Length + 1];
                    args2[0] = func.Replace("." + funObj.name, "");
                    for (int i = 0; i < args.Length; i++)
                    {
                        args2[i + 1] = args[i];
                    }
                    args = args2;
                }

                if (structStack.Count == 0)
                {
                    if (!context.currentFile().notUsed)
                        funObj.file.use();
                }

                context.currentFile().addChild(funObj.file);
                if (callStackDisplay)
                    callTrace += "\"" + callingFunctName + "\"->\"" + funObj.gameName + "\"\n";

                int functionCount = 0;
                foreach (var a in funObj.args)
                {
                    if (a.type == Type.FUNCTION)
                    {
                        functionCount++;
                    }
                }

                //short notation
                if (!funObj.lazy)
                {
                    if (args.Length == 1)
                    {
                        string[] spt = smartSplit(args[0], ' ');
                        if ((spt.Length >= funObj.args.Count - functionCount ||
                            spt.Length >= funObj.argNeeded) && spt.Length <= funObj.maxArgNeeded)
                        {

                            args = spt;
                        }
                    }
                }
                else
                {
                    if (args.Length == 1 && funObj.args.Count > 1 && isString(args[0])
                        && smartSplit(extractString(args[0]), ' ').Length >= funObj.args.Count - functionCount)
                    {
                        args = smartSplit(extractString(args[0]), ' ');
                    }
                    else if (args.Length == 1 && funObj.args.Count > 1 &&
                        smartSplit(args[0], ' ').Length >= funObj.args.Count - functionCount)
                    {
                        args = smartSplit(args[0], ' ');
                    }
                }

                HashSet<string> contextVar = new HashSet<string>();
                bool endWithAccollade = (smartEmpty(text).EndsWith("{") || smartEmpty(text).EndsWith("}"));
                if (funObj.lazy)
                {
                    lazyOutput.Push(new List<Variable>());
                    adjPackage.Push(funObj.package);

                    lazyEvalVar.Add(new Dictionary<string, string>());
                    context.compVal.Add(new Dictionary<string, string>());

                    if (outVar != null)
                    {
                        for (int j = 0; j < outVar.Length; j++)
                        {
                            lazyOutput.Peek().Add(GetVariableByName(outVar[j]));
                        }
                    }


                    lazyCall.Push(funObj);
                    string clear = "";
                    string init = "";
                    if (funObj.varOwner != null)
                    {
                        thisDef.Push(funObj.varOwner.gameName + ".");
                    }
                    int i = 0;

                    var Condstack = LastConds;
                    var CondLast = LastCond;
                    LastConds = new Stack<If>();
                    LastCond = new If(-1);

                    if (args.Length > funObj.args.Count && !funObj.args.Any(x => x.type == Type.PARAMS || x.type == Type.JSON))
                        throw new Exception("To much Argument: recieve " + args.Length.ToString() + " expected: " + funObj.args.Count.ToString());
                    
                    bool inplace = funObj.attributes != null && funObj.attributes.Contains("inplace");
                    
                    string lazyconte = context.GetVar() + (inplace?"":"c_" + funObj.name + ".");
                    
                    foreach (Argument a in funObj.args)
                    {
                        if (args.Length <= i)
                        {
                            if (a.defValue == null && !endWithAccollade && a.type == Type.FUNCTION)
                            {
                                string val = context.getImpliciteVar(a);
                                if (val != null && a.isImplicite)
                                {
                                    output += parseLine(a.GetInternalTypeString() + a.name + "=" + val);
                                    if (!contextVar.Contains(a.GetTypeString()))
                                    {
                                        contextVar.Add(a.GetTypeString());
                                    }
                                    else
                                    {
                                        throw new Exception("Not Enought argument for " + funObj.gameName + "(" + arg + ")");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Not Enought argument for " + funObj.gameName + "(" + arg + ")");
                                }
                            }
                            else if (a.name.StartsWith("$") && a.defValue != null)
                            {
                                string[] o = addCompVar(a, new string[] { a.defValue }, 0);
                                if (o != null)
                                {
                                    init += o[0];
                                    clear += o[1];
                                }
                            }
                            else if (a.defValue != null)
                                addLazyVal(lazyconte + a.name, a.defValue);
                            else if (endWithAccollade && a.type == Type.FUNCTION)
                            {
                                anonymusFuncName = "lambda_" + Lambda.GetID(context.GetFun()).ToString();

                                anonymusFuncNameArg = GetLambdaFunctionArgs(null, a.variable.args);

                                parseLine("def abstract __lambda__ " + anonymusFuncName + anonymusFuncNameArg);

                                context.compVal[context.compVal.Count - 1].Add(a.name + ".name", functions[context.GetFunctionName(anonymusFuncName)][0].gameName);
                                if (a.name.StartsWith("$"))
                                    context.compVal[context.compVal.Count - 1].Add(a.name, anonymusFuncName);
                                else
                                    addLazyVal(lazyconte+a.name, anonymusFuncName);
                                
                                anonymusFunc = true;
                            }
                        }
                        else
                        {
                            if (endWithAccollade && a.type == Type.FUNCTION && !anonymusFunc)
                            {
                                anonymusFuncName = "lambda_" + Lambda.GetID(context.GetFun()).ToString();

                                anonymusFuncNameArg = GetLambdaFunctionArgs(null, a.variable.args);

                                parseLine("def abstract __lambda__ " + anonymusFuncName + anonymusFuncNameArg);

                                context.compVal[context.compVal.Count - 1].Add(a.name + ".name", functions[context.GetFunctionName(anonymusFuncName)][0].gameName);
                                if (a.name.StartsWith("$"))
                                    context.compVal[context.compVal.Count - 1].Add(a.name, anonymusFuncName);
                                else
                                    addLazyVal(lazyconte+a.name, anonymusFuncName);

                                anonymusFunc = true;
                                i--;
                            }
                            else if (a.name.StartsWith("$"))
                            {
                                string[] o = addCompVar(a, args, i);
                                if (o != null)
                                {
                                    init += o[0];
                                    clear += o[1];
                                }
                            }
                            else
                            {
                                Variable valVar = GetVariableByName(smartExtract(args[i]), true);
                                if (valVar != null)
                                {
                                    addLazyVal(lazyconte + a.name, valVar.gameName);

                                    if (a.type == Type.STRUCT)
                                    {
                                        foreach (Variable v in structs[a.enums].fields)
                                        {
                                            addLazyVal(lazyconte + a.name + "." + v.name, valVar.gameName + "." + v.name);
                                        }
                                    }
                                }
                                else if (a.type == Type.FUNCTION)
                                {
                                    if (context.GetFunctionName(args[i], true) == null)
                                    {
                                        output += parseLine(a.variable.GetInternalTypeString() + " " + a.name + "=" + args[i]);
                                    }
                                    else
                                    {
                                        addLazyVal(lazyconte + a.name, GetConstant(a.variable, args[i]).gameName);
                                    }
                                }
                                else if (IsFunction(args[i]))
                                {
                                    output += parseLine(a.variable.GetInternalTypeString() + " " + a.name + "=" + args[i]);
                                }
                                else
                                {
                                    addLazyVal(lazyconte + a.name, GetConstant(a.variable, args[i]).gameName);
                                }
                            }
                        }
                        i++;
                    }

                    if (!inplace)
                    {
                        context.Sub("c_" + funObj.name, new File("c_" + funObj.name, "", "lazyfunctioncall"));
                    }
                    File tFile = context.currentFile();

                    i = 0;
                    autoIndented = 0;
                    if (funObj.variableStruct != null)
                        adjPackage.Push(funObj.variableStruct);
                    string prevFunction = callingFunctName;
                    callingFunctName = funObj.gameName;
                    
                    context.currentFile().AddLine(init);
                    foreach (string line in funObj.file.parsed)
                    {
                        preparseLine(line, tFile, true);

                        i++;
                    }
                    context.currentFile().AddLine(clear);
                    if (!inplace)
                    {
                        tFile.Close();
                    }

                    callingFunctName = prevFunction;
                    if (funObj.variableStruct != null)
                        adjPackage.Pop();

                    context.compVal.RemoveAt(context.compVal.Count - 1);
                    if (funObj.varOwner != null)
                    {
                        thisDef.Pop();
                    }

                    adjPackage.Pop();
                    lazyOutput.Pop();
                    lazyCall.Pop();
                    popLazyVal();
                    
                    if (anonymusFunc)
                    {
                        parseLine("def __lambda__ " + anonymusFuncName + anonymusFuncNameArg + "{");
                        if (smartEmpty(text).EndsWith("}"))
                        {
                            preparseLine(getCodeBlock(text));
                            preparseLine("}");
                        }
                    }

                    LastConds = Condstack;
                    LastCond = CondLast;
                    
                    return "";
                }
                else
                {
                    HashSet<Argument> assignedArg = new HashSet<Argument>();

                    if (args.Length > funObj.args.Count)
                        throw new Exception("To much Argument: recieve " + args.Length.ToString() + " expected: " + funObj.args.Count.ToString());

                    for (int i = 0; i < args.Length; i++)
                    {
                        if ((args[i].Contains("=") && !args[i].Contains("=>")) || args[i].Split('=').Length == 3)
                        {
                            string[] part = smartSplit(args[i], '=', 1);
                            bool found = false;
                            foreach (Argument a in funObj.args)
                            {
                                if (a.name == smartEmpty(part[0]))
                                {
                                    if (assignedArg.Contains(a))
                                        throw new Exception(a.name + " already assigned!");

                                    output += parseLine(a.gameName + "=" + part[1]) + "\n";
                                    assignedArg.Add(a);
                                    found = true;
                                }
                            }
                            if (!found)
                                throw new Exception("Unknown argument " + part[0]);
                        }
                        else
                        {
                            output += parseLine(funObj.args[i].gameName + "=" + args[i]) + "\n";
                            assignedArg.Add(funObj.args[i]);
                        }
                    }
                    if (funObj.args.Count != args.Length)
                    {
                        foreach (Argument a in funObj.args)
                        {
                            if (!assignedArg.Contains(a))
                            {
                                if (a.defValue == null && !endWithAccollade && a.type == Type.FUNCTION)
                                {
                                    string val = context.getImpliciteVar(a);
                                    if (val != null && a.isImplicite)
                                    {
                                        output += parseLine(a.gameName + "=" + val);
                                        if (!contextVar.Contains(a.GetTypeString()))
                                        {
                                            contextVar.Add(a.GetTypeString());
                                        }
                                        else
                                        {
                                            throw new Exception("Not Enought argument for " + funObj.gameName + "(" + arg + ")");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Not Enought argument for " + funObj.gameName + "(" + arg + ")");
                                    }
                                }
                                else if (endWithAccollade && a.type == Type.FUNCTION)
                                {
                                    anonymusFuncName = "lambda_" + Lambda.GetID(context.GetFun()).ToString();

                                    anonymusFuncNameArg = GetLambdaFunctionArgs(null, a.variable.args);

                                    parseLine("def abstract " + anonymusFuncName + anonymusFuncNameArg);
                                    output += parseLine(a.gameName + "=" + anonymusFuncName) + "\n";
                                    anonymusFunc = true;
                                }
                                else if (a.defValue != null)
                                    output += parseLine(a.gameName + "=" + a.defValue);
                            }
                        }
                    }

                    output += Core.CallFunction(funObj) + '\n';
                    if (outVar != null)
                    {
                        Variable valVar = GetVariableByName(outVar[0], true);

                        if (funObj.outputs.Count == 0 && outVar.Length > 0)
                        {
                            throw new Exception("Function " + funObj.gameName + " do not return any value. ");
                        }
                        else if (valVar != null && valVar.type == Type.ARRAY && funObj.outputs[0].type != Type.ARRAY)
                        {
                            if (valVar.arraySize != funObj.outputs.Count)
                                throw new Exception("Cannot cast function output into array");

                            for (int j = 0; j < funObj.outputs.Count; j++)
                            {
                                string v = context.GetVariable(outVar[0] + "." + j.ToString());
                                output += parseLine(v + " " + op + " " + funObj.outputs[j].gameName) + '\n';
                            }
                        }
                        else
                        {
                            for (int j = 0; j < outVar.Length; j++)
                            {
                                output += parseLine(outVar[j] + " " + op + " " + funObj.outputs[j].gameName) + '\n';
                            }
                        }
                    }
                    if (anonymusFunc)
                    {
                        context.currentFile().AddLine(output);
                        parseLine("def __lambda__ " + anonymusFuncName + anonymusFuncNameArg + "{");
                        if (smartEmpty(text).EndsWith("}"))
                        {
                            preparseLine(getCodeBlock(text));
                            preparseLine("}");
                        }
                        return "";
                    }
                    else
                    {
                        return output;
                    }
                }
            }
        }
        public static string functionReturn(string[] arg)
        {
            string ouput = "";
            if (lazyEvalVar.Count > 0 && lazyOutput.Count > 0)
            {
                int i = 0;

                foreach (Variable v in lazyOutput.Peek())
                {
                    ouput += parseLine(v.gameName + "=" + arg[i]);
                    i++;
                }

                if (lazyOutput.Peek().Count == 0)
                {
                    foreach (string ar in arg)
                        ouput += parseLine(ar);
                }
            }
            else
            {
                bool hadOutput = context.currentFile().function.outputs.Count > 0;
                if (context.currentFile().function.outputs.Count == 0)
                {
                    ouput += parseLine(arg[0]);
                    return ouput;
                }
                for (int i = 0; i < arg.Length; i++)
                {
                    if (context.GetVariable("ret_" + i.ToString(), true) == null)
                    {
                        if (hadOutput)
                        {
                            throw new Exception("Wrong Number of Output for " + context.currentFile().function.gameName);
                        }
                        else
                        {
                            string name = context.GetVar() + "ret_" + i.ToString();
                            Variable v = new Variable("ret_" + i.ToString(), name, getExprType(arg[i]));

                            if (getExprType(arg[i]) == Type.STRUCT)
                            {
                                v.enums = getStruct(arg[i]);
                                enums[v.enums].GenerateVariable(v.name);
                            }

                            variables.Add(name, v);
                            context.currentFile().function.outputs.Add(v);
                        }
                    }
                    ouput += parseLine("ret_" + i.ToString() + "=" + arg[i]);
                }
            }
            return ouput;
        }
        public static string functionVarEval(string text, string[] outVar = null, string op = "=")
        {
            string arg = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
            string[] args = smartSplit(arg, ',');
            string func;
            if (text.Contains('='))
            {
                func = text.Substring(text.IndexOf('=') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
            }
            else
            {
                func = text.Substring(0, text.IndexOf('('));
            }

            string output = "";

            string funcName;
            if (containLazyVal(func))
                funcName = context.GetVariable(getLazyVal(func));
            else
                funcName = context.GetVariable(func);

            Variable funObj = GetVariable(funcName);
            string grp = GetFunctionGRP(funObj);

            if (!functDelegated.ContainsKey(grp))
            {
                CreateFunctionGRP(funObj, grp);
            }

            if (args.Length > funObj.args.Count)
                throw new Exception("To much Argument: recieve " + args.Length.ToString() + " expected: " + funObj.args.Count.ToString());

            output = CallFunctionGRP(funObj, func, grp, args, outVar);

            return output;
        }
        public static string getFunctionName(string text)
        {
            int opIndex = getOpenCharIndex(text, '(');
            return text.Substring(0, opIndex);
        }
        #endregion

        #region generate info
        private static string GenerateInfoPackage(string package)
        {
            string output = "#=================================================#" + '\n';
            output += "#Package name: " + package + '\n';
            output += "#=================================================#" + '\n';

            return output;
        }
        private static string GenerateInfo(Function func)
        {
            string output = "#=================================================#" + '\n';
            output += functionDesc.Replace("\"\"\"", "") + "\n";
            output += "#" + func.gameName + "\n";
            functionDesc = "";
            output += "#Argument:" + '\n';

            foreach (Argument arg in func.args)
            {
                output += "# - " + arg.gameName + " (" + arg.name + "): " + arg.GetTypeString() + '\n';
            }
            output += "#Output:" + '\n';

            foreach (Variable arg in func.outputs)
            {
                output += "# - " + arg.gameName + ": " + arg.GetTypeString() + '\n';
            }
            output += "#=================================================#" + '\n';

            return output;
        }
        #endregion

        #region tools
        public static string[] smartSplit(string text, char c, int max = -1)
        {
            List<string> output = new List<string>();
            int ind = 0;
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            bool inString = false;
            bool skipNext = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == ')' && !inString)
                {
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '[' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == ']' && !inString)
                {
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '<' && c == ',' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '>' && c == ',' && !inString)
                {
                    if (!(i > 0 && text[i - 1] == '='))
                        ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '"')
                {
                    if (inString)
                    {
                        if (!skipNext)
                        {
                            ind -= 1;
                            stringBuilder.Append(text[i]);
                            inString = false;
                        }
                        else
                        {
                            skipNext = false;
                        }
                    }
                    else
                    {
                        ind += 1;
                        stringBuilder.Append(text[i]);
                        inString = true;
                    }
                }
                else if (text[i] == c && ind == 0 && max != 0 && !inString)
                {
                    output.Add(stringBuilder.ToString());
                    stringBuilder = new StringBuilder(text.Length);
                    max--;
                    
                    if (max == 0)
                    {
                        i++;
                        string a = text.Substring(i, text.Length - i);
                        if (a != null && a != "")output.Add(a);
                        break;
                    }
                }
                else if (inString && !skipNext && text[i] == '\\')
                {
                    skipNext = true;
                    if (text[i + 1] != '\"')
                    {
                        stringBuilder.Append(text[i]);
                    }
                }
                else
                {
                    skipNext = false;
                    stringBuilder.Append(text[i]); 
                }
            }
            string str = stringBuilder.ToString();
            if (str != "" && str != null)
                output.Add(str);

            return output.ToArray();
        }
        public static string[] smartSplitJson(string text, char c, int max = -1)
        {
            List<string> output = new List<string>();
            int ind = 0;
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            bool inString = false;
            bool skipNext = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == ')' && !inString)
                {
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '{' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '}' && !inString)
                {
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '[' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == ']' && !inString)
                {
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '<' && c == ',' && !inString)
                {
                    ind += 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '>' && c == ',' && !inString)
                {
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '"')
                {
                    if (inString)
                    {
                        if (!skipNext)
                        {
                            ind -= 1;
                            stringBuilder.Append(text[i]);
                            inString = false;
                        }
                        else
                        {
                            skipNext = false;
                        }
                    }
                    else
                    {
                        ind += 1;
                        stringBuilder.Append(text[i]);
                        inString = true;
                    }
                }
                else if (text[i] == c && ind == 0 && max != 0 && !inString)
                {
                    output.Add(stringBuilder.ToString());
                    stringBuilder = new StringBuilder(text.Length);
                    max--;
                }
                else if (inString && !skipNext && text[i] == '\\')
                {
                    skipNext = true;
                    if (text[i+1] != '\"')
                    {
                        stringBuilder.Append(text[i]);
                    }
                }
                else 
                {
                    skipNext = false;
                    stringBuilder.Append(text[i]); 
                }
            }
            if (stringBuilder.ToString() != "")
                output.Add(stringBuilder.ToString());

            return output.ToArray();
        }
        public static bool smartContains(string text, char c, int max = -1)
        {
            int ind = 0;
            bool inString = false;
            if (text == null || text.Length == 0) return false;
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            if (text[0] == '(' && text.Contains(c))
            {
                return true;
            }

            if (c == '-' && text.StartsWith("-") && int.TryParse(text[1].ToString(), out int xxx))
            {
                return false;
            }
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(')
                {
                    ind += 1;
                }
                else if (text[i] == ')')
                {
                    ind -= 1;
                }
                else if (text[i] == '[' && c != '[' && c != ']')
                {
                    ind += 1;
                }
                else if (text[i] == ']' && c != '[' && c != ']')
                {
                    ind -= 1;
                }
                else if (text[i] == '{' && c != '{' && c != '}')
                {
                    ind += 1;
                }
                else if (text[i] == '}' && c != '}' && c != '{')
                {
                    ind -= 1;
                }
                else if (text[i] == '"' && c != '"')
                {
                    inString = !inString;
                }
                else if (text[i] == '<' && c == ',')
                {
                    ind += 1;
                }
                else if (text[i] == '>' && c == ',')
                {
                    ind -= 1;
                }
                else if (text[i] == c && ind == 0 && max != 0 && !inString)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool smartContains(string text, string c, int max = -1)
        {
            int ind = 0;
            bool inString = false;
            if (text == null || text.Length == 0) return false;
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            if (text[0] == '(' && text.Contains(c))
            {
                return true;
            }
            int j = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(')
                {
                    ind += 1;
                    j = 0;
                }
                else if (text[i] == ')')
                {
                    ind -= 1;
                    j = 0;
                }
                else if (text[i] == '[' && c[j] != '[' && c[j] != ']')
                {
                    ind += 1;
                    j = 0;
                }
                else if (text[i] == ']' && c[j] != '[' && c[j] != ']')
                {
                    ind -= 1;
                    j = 0;
                }
                else if (text[i] == '{' && c[j] != '{' && c[j] != '}')
                {
                    ind += 1;
                    j = 0;
                }
                else if (text[i] == '}' && c[j] != '}' && c[j] != '{')
                {
                    ind -= 1;
                    j = 0;
                }
                else if (text[i] == '"' && c[j] != '"')
                {
                    inString = !inString;
                    j = 0;
                }
                else if (text[i] == '<' && c[j] == ',')
                {
                    ind += 1;
                    j = 0;
                }
                else if (text[i] == '>' && c[j] == ',')
                {
                    ind -= 1;
                    j = 0;
                }
                else if (text[i] == c[j] && ind == 0 && max != 0 && !inString)
                {
                    j++;
                    if (j >= c.Length)
                    {
                        return true;
                    }
                }
                else
                {
                    j = 0;
                }
            }

            return false;
        }

        public static bool smartEndWith(string text, string c)
        {
            while (text.EndsWith(" ") || text.EndsWith("\n") || text.EndsWith("  "))
            {
                text = text.Substring(0, text.Length - 1);
            }

            return text.EndsWith(c);
        }
        public static string smartEmpty(string text)
        {
            int ind = 0;
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                {
                    ind = 1 - ind;
                    stringBuilder.Append(text[i]);
                }
                else if ((text[i] == ' ' || text[i] == '\t') && ind == 0)
                {

                }
                else { stringBuilder.Append(text[i]); }
            }

            return stringBuilder.ToString();
        }
        public static string smartExtract(string text)
        {
            while (text.StartsWith(" ") || text.StartsWith("\t"))
            {
                text = text.Substring(1, text.Length - 1);
            }
            while (text.EndsWith(" ") || text.EndsWith("\t") || text.EndsWith("\n") || text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
            }
            return text;
        }
        public static string getArg(string text)
        {
            try
            {
                int opIndex = getOpenCharIndex(text, '(');
                return text.Substring(opIndex + 1, getCloseCharIndex(text, ')') - opIndex - 1);
            }
            catch(Exception e)
            {
                int o = getOpenCharIndex(text, '(');
                int c = getCloseCharIndex(text, ')');
                throw new Exception($"Invalid arg: {text} open: {o}/{c}"+e.ToString());
            }
        }
        public static string[] getArgs(string text)
        {
            string[] args = smartSplitJson(getArg(text), ',');
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = smartExtract(args[i]);
            }
            return args;
        }
        public static string getParenthis(string text, int max = -1, int recCall = 0)
        {
            if (recCall > maxRecCall)
            {
                throw new Exception("Stackoverflow");
            }
            while (text.StartsWith(" "))
            {
                text = text.Substring(1, text.Length - 1);
            }
            while (text.EndsWith(" "))
            {
                text = text.Substring(0, text.Length - 1);
            }

            while (text.StartsWith("(") && text.EndsWith(")") && getCloseCharIndex(text, ')') == text.Length - 1)
            {
                return getParenthis(text.Substring(text.IndexOf('(') + 1, getCloseCharIndex(text, ')') - text.IndexOf('(') - 1), max - 1, recCall + 1);
            }

            return text;
        }
        public static string getCodeBlock(string text)
        {
            while (text.StartsWith(" "))
            {
                text = text.Substring(1, text.Length - 1);
            }

            return text.Substring(getOpenCharIndex(text, '{') + 1, text.LastIndexOf('}') - getOpenCharIndex(text, '{') - 1);

            //return text;
        }
        public static MatchCustom getArrayBlock(string text)
        {
            while (text.StartsWith(" "))
            {
                text = text.Substring(1, text.Length - 1);
            }
            string value = text.Substring(getOpenCharIndex(text, '['), text.LastIndexOf(']') - getOpenCharIndex(text, '[') + 1);
            return new MatchCustom(getOpenCharIndex(text, '['), value);
        }
        public static void autoIndent(string text)
        {
            if (text.Contains("{") && smartEndWith(text, "}"))
            {
                preparseLine(getCodeBlock(text));
                preparseLine("}");
                autoIndented = 0;
            }
            else if (text.Contains("{") && !smartEndWith(text, "{") && !smartEndWith(text, "}"))
            {
                preparseLine(getCodeBlock(text + "}"));
                autoIndented = 2;
            }
            else if (!smartEndWith(text, "{"))
            {
                autoIndented = 2;
            }
        }
        public static int getOpenCharIndex(string text, char d)
        {
            int indent = 0;
            int index = 0;
            int returnVal = -1;
            bool genInd = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '{' && d != c)
                {
                    indent++;
                }
                else if (c == '{' && d == c)
                {
                    if (indent == 0)
                    {
                        returnVal = index;
                        indent++;
                    }
                    else
                        indent++;
                }
                else if (c == '}')
                {
                    indent--;
                }
                if (c == '(' && d != c)
                {
                    indent++;
                }
                else if (c == '(' && d == c)
                {
                    if (indent == 0)
                    {
                        returnVal = index;
                        indent++;
                    }
                    else
                        indent++;
                }
                else if (c == ')')
                {
                    indent--;
                }
                else if (c == '<' && d == c)
                {
                    if (indent == 0)
                    {
                        returnVal = index;
                        indent++;
                    }
                }
                else if (c == '<' && indent == 0)
                {
                    indent++;
                    genInd = true;
                }
                else if (c == '>' && d == '<')
                {
                    indent--;
                }
                else if (c == '>' && indent == 1 && genInd)
                {
                    indent--;
                    genInd = false;
                }
                if (c == '[' && d != c)
                {
                    indent++;
                }
                else if (c == '[' && d == c)
                {
                    if (indent == 0)
                    {
                        returnVal = index;
                        indent++;
                    }
                    else
                        indent++;
                }
                else if (c == ']')
                {
                    indent--;
                }

                index++;
            }
            return returnVal;
        }
        public static int getCloseCharIndex(string text, char d)
        {
            int indent = 0;
            int index = 0;
            bool genInd = false;
            foreach (char c in text)
            {
                if (c == '{')
                {
                    indent++;
                }
                else if (c == '}' && d != c)
                {
                    indent--;
                }
                else if (c == '}' && d == c)
                {
                    if (indent == 1)
                        return index;
                    else
                        indent--;
                }
                if (c == '(')
                {
                    indent++;
                }
                else if (c == ')' && d != c)
                {
                    indent--;
                }
                else if (c == ')' && d == c)
                {
                    if (indent == 1)
                        return index;
                    else
                        indent--;
                }
                if (c == '<' && d == '>')
                {
                    indent++;
                }
                else if (c == '<' && indent == 0)
                {
                    indent++;
                    genInd = true;
                }
                else if (c == '>' && d == c)
                {
                    if (indent == 1)
                        return index;
                    else
                        indent--;
                }
                else if (c == '>' && indent == 1 && genInd)
                {
                    indent--;
                    genInd = false;
                }
                if (c == '[')
                {
                    indent++;
                }
                else if (c == ']' && d != c)
                {
                    indent--;
                }
                else if (c == ']' && d == c)
                {
                    if (indent == 1)
                        return index;
                    else
                        indent--;
                }

                index++;
            }
            return -1;
        }
        public static string FunctionNameSimple(string name)
        {
            return name.Replace("=", "__equals__")
                       .Replace(">", "__bigger__")
                       .Replace("<", "__smaller__")
                       .Replace("+", "__add__")
                       .Replace("-", "__sub__")
                       .Replace("/", "__div__")
                       .Replace("*", "__mul__")
                       .Replace("^", "__hat__")
                       .Replace("&", "__and__")
                       .Replace("|", "__or__");
        }

        public static List<ImpliciteVar> getImpliciteFromExpr(string text)
        {
            List<ImpliciteVar> lst = new List<ImpliciteVar>();

            string empty = smartEmpty(text);
            string lower = empty.ToLower();

            foreach (var key in enums.Keys)
            {
                foreach (var val in enums[key].Values())
                {
                    if (val == lower)
                        lst.Add(new ImpliciteVar(key, Type.ENUM, text));
                }
            }

            if (context.GetVariable(empty, true) != null)
            {
                var var = GetVariableByName(empty);
                lst.Add(new ImpliciteVar(var.enums, var.type, text));
            }

            if (empty.StartsWith("\"") && empty.EndsWith("\""))
            {
                lst.Add(new ImpliciteVar("$val", Type.STRING, empty.Substring(1, empty.Length - 2)));
            }

            return lst;
        }

        public static string jsonAppend(string src, string text)
        {
            Regex jsonListEmpty = new Regex(@"\[\s*\]");
            src = smartExtract(src);
            text = smartExtract(text);
            if (src.StartsWith("["))
            {
                if (jsonListEmpty.Match(src).Success)
                    return "[" + text + "]";
                else
                    return src.Substring(0, src.Length - 1) + "," + text + "]";
            }
            if (src == "" || src == null || src == "{}")
            {
                if (text.StartsWith("{"))
                    return text;
                else
                    return "{" + text + "}";
            }
            if (text.StartsWith("{"))
            {
                if (text == "{}")
                    return src;
                JObject o1 = JObject.Parse(src);
                JObject o2 = JObject.Parse(text);
                o1.Merge(o2, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                return o1.ToString().Replace("\n", "").Replace("\r","").Replace("\t","");
            }
            else
                return src.Substring(0, src.Length - 1) + "," + text + "}";
        }
        #endregion

        #region offuscation
        public static long IntPow(long x, long pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
        public static string OffuscationMapAdd(string text)
        {
            if (offuscationMap.ContainsKey(text))
            {
                if (!compilerSetting.offuscate)
                    return text;
                return offuscationMap[text];
            }

            string rText = text.Reverse().Select(x => x+"").Aggregate((x,y)=>(x+y));

            long hash = text.GetHashCode() + rText.GetHashCode() * (long)(int.MaxValue);
            long c = Math.Abs(hash % pow64[10]);

            string map = GetOffuscationUUID(c);

            if (offuscationSet.Contains(map))
            {
                return OffuscationMapAdd(text + "'");
            }

            offuscationSet.Add(map);
            offuscationMap.Add(text, map);
            if (!compilerSetting.offuscate)
                return text;
            return map;
        }
        public static string OffuscationMapAdd(string text, string forced)
        {
            if (offuscationMap.ContainsKey(text))
            {
                if (!compilerSetting.offuscate)
                    return text;
                return offuscationMap[text];
            }

            if (offuscationSet.Contains(forced))
            {
                return OffuscationMapAdd(text + "_");
            }

            offuscationSet.Add(forced);
            offuscationMap.Add(text, forced);
            if (!compilerSetting.offuscate)
                return text;
            return forced;
        }
        public static string GetOffuscationUUID(long c)
        {
            StringBuilder s = new StringBuilder(dirVar, 16);
            s.Append(".");
            s.Append(alphabet[(int)((c >> 52) & 63)]);
            s.Append(alphabet[(int)((c >> 48) & 63)]);
            s.Append(alphabet[(int)((c >> 42) & 63)]);
            s.Append(alphabet[(int)((c >> 36) & 63)]);
            s.Append(alphabet[(int)((c >> 30) & 63)]);
            s.Append(alphabet[(int)((c >> 24) & 63)]);
            s.Append(alphabet[(int)((c >> 18) & 63)]);
            s.Append(alphabet[(int)((c >> 12) & 63)]);
            s.Append(alphabet[(int)((c >> 6) & 63)]);
            s.Append(alphabet[(int)(c & 63)]);
            return s.ToString();
        }

        public static string classOffuscationMapAdd(string text)
        {
            if (!compilerSetting.offuscate)
                return text;

            if (classOffuscationMap.ContainsKey(text))
                return classOffuscationMap[text];

            string rText = "";
            foreach (char ch in text.Reverse())
            {
                rText += ch;
            }
            long hash = text.GetHashCode() + rText.GetHashCode() * (long)(int.MaxValue);
            long c = Math.Abs(hash % pow64[10]);

            string map = GetOffuscationUUID(c);

            if (classOffuscationSet.Contains(map))
            {
                return classOffuscationMapAdd(text + "'");
            }

            classOffuscationSet.Add(map);
            classOffuscationMap.Add(text, map);
            return map;
        }
        #endregion

        #region Finishing
        public static void ConstCreate()
        {
            DateTime startTime = DateTime.Now;

            bool change = true;
            do
            {
                change = false;
                foreach (Variable var in variables.Values.Reverse())
                {
                    if (var.wasUsed && !var.wasAdded && var.isConst && var.constValue != null)
                    {
                        if (var.UnparsedFunctionFileContext != null)
                            context.GoTo(var.UnparsedFunctionFileContext);
                        else
                            context.GoTo(Project.ToLower() + ".const");

                        var.wasAdded = true;
                        var.isConst = false;
                        change = true;

                        try
                        {
                            loadFile.AddStartLine(eval(var.constValue, var, var.type));
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Exception while calculating const value: " + e.ToString());
                        }
                        var.isConst = true;
                    }
                }
            } while (change);

            if (compilerSetting.advanced_debug)
                GlobalDebug($">> Const created in {((DateTime.Now-startTime).TotalMilliseconds)}ms", Color.Yellow);
        }
        public static void ScoreboardCreate()
        {
            DateTime startTime = DateTime.Now;
            bool change = true;
            do
            {
                change = false;
                foreach (Scoreboard var in scoreboards.Values)
                {
                    if (var.wasUsed && !var.wasAdded)
                    {
                        var.wasAdded = true;
                        change = true;

                        loadFile.AddScoreboardDefLine(Core.DefineScoreboard(var));
                    }
                }
            } while (change);

            if (compilerSetting.advanced_debug)
                GlobalDebug($">> Scoreboard Created in {((DateTime.Now-startTime).TotalMilliseconds)}ms", Color.Yellow);
        }
        public static void FunctionCreate()
        {
            DateTime startTime = DateTime.Now;
            bool changed = true;
            int pass = 1;
            while (changed)
            {
                GlobalDebug("Funciton Pass: " + pass.ToString(), Color.YellowGreen);
                pass++;
                int fi = 0;
                changed = false;
                while (fi < files.Count)
                {
                    if ((files[fi].UnparsedFunctionFile && !files[fi].notUsed))
                    {
                        files[fi].Compile();
                        changed = true;
                    }
                    fi++;
                }
            }
            if (compilerSetting.advanced_debug)
                GlobalDebug($">> Functions Created in {((DateTime.Now-startTime).TotalMilliseconds)}ms", Color.Yellow);
        }
        public static void StringPoolCreate()
        {
            DateTime startTime = DateTime.Now;
            int i = 0;
            
            foreach (string s in stringSet)
            {
                Variable mux = GetVariable("__multiplex__.sstring.__strSelector__");
                stringPool.AddLine("execute if score " + mux.scoreboard() + " matches " + i.ToString() +
                    " run summon minecraft:area_effect_cloud ~ ~ ~ { CustomName: '{\"text\":\"" + s + "\"}'},Tags:[\"__str__\"]}");
                i++;
            }
            if (compilerSetting.advanced_debug)
                GlobalDebug($">> String Pool in {((DateTime.Now-startTime).TotalMilliseconds)}ms", Color.Yellow);
        }
        public static void MuxClose()
        {
            DateTime startTime = DateTime.Now;
            foreach (var grp in functDelegatedFile)
            {
                foreach (var f in grp.Value)
                {
                    f.Close();
                }
            }
            if (compilerSetting.advanced_debug)
                GlobalDebug($">> Mux Closed in {((DateTime.Now-startTime).TotalMilliseconds)}ms", Color.Yellow);
        }
        public static void FunctionOptimisation()
        {
            DateTime startTime = DateTime.Now;
            foreach (var f in functionTags.Keys.Select(x => CreateFunctionTag(x)))
            {
                var lineCount = f.content.Split('\n').Length - 1;
                if (lineCount == 1 && !f.content.StartsWith("#") && f.name != "load" && f.name != "main")
                {
                    foreach (File f2 in files)
                    {
                        f2.content = f2.content.Replace(Core.CallFunction(f), f.content);
                    }
                    f.valid = false;
                }
            }
            if (compilerSetting.advanced_debug)
                GlobalDebug($">> Function Optimisation in {((DateTime.Now-startTime).TotalMilliseconds)}ms", Color.Yellow);
        }
        #endregion

        #region Data Class
        public abstract class CompilerObject
        {
            public List<string> attributes = new List<string>();
        }
        public class Function : CompilerObject
        {
            public string name;
            public string gameName;
            public string desc;

            public bool lazy;
            public bool isExtensionMethod;
            public string package;
            public string structure;
            public string variableStruct;

            public List<Argument> args = new List<Argument>();
            public List<Variable> outputs = new List<Variable>();
            public HashSet<Variable> moddifiedVar = new HashSet<Variable>();
            public Dictionary<string, int> SwitchNumber;
            public List<string> tags = new List<string>();
            public File file;
            public Variable varOwner;
            public bool isAbstract;
            public bool isTicking;
            public bool isLoading;
            public bool isHelper;
            public bool isLambda;
            public bool isExternal;
            public bool isPrivate = false;
            public bool isVirtual = false;
            public bool isOverride = false;
            public bool isStatic = false;
            public bool isStackSafe = false;
            public bool isInfix = false;
            public bool isPrefix = false;
            public string privateContext;
            public bool isStructMethod = false;
            public int argNeeded = 0;
            public int maxArgNeeded = 0;

            public Function(string name, string gameName, File file)
            {
                this.name = name;
                this.gameName = gameName;
                this.file = file;
                this.package = context != null ? context.GetVar() : currentPackage;

                if (adjPackage.Count > 0)
                {
                    package = adjPackage.Peek();
                }
            }

            public void LinkCopyTo(string v, bool simple = false)
            {
                AddFunction(v + "." + name, this);
                if (!simple)
                {
                    foreach (Variable var in args)
                    {
                        try
                        {
                            var.LinkCopyTo(v);
                        }
                        catch { }
                    }
                    foreach (Variable var in outputs)
                    {
                        try
                        {
                            var.LinkCopyTo(v);
                        }
                        catch { }
                    }
                }
            }
            public Function CopyTo(string gameName, string name, File f)
            {
                Function newfunc = new Function(name, gameName, f);

                newfunc.gameName = gameName;
                newfunc.name = name;
                newfunc.file = f;
                newfunc.args = args;
                newfunc.outputs = outputs;
                newfunc.isPrivate = isPrivate;
                newfunc.isStructMethod = isStructMethod;
                newfunc.isStructMethod = isStructMethod;

                newfunc.desc = desc;
                newfunc.lazy = lazy;
                newfunc.isAbstract = isAbstract;
                newfunc.isPrivate = isPrivate;
                newfunc.argNeeded = argNeeded;
                newfunc.maxArgNeeded = maxArgNeeded;
                newfunc.package = package;
                newfunc.isOverride = isOverride;
                newfunc.isVirtual = isVirtual;

                newfunc.argNeeded = argNeeded;
                newfunc.maxArgNeeded = maxArgNeeded;

                return newfunc;
            }

            public override string ToString()
            {
                return gameName;
            }

            public string GetInternalFunctionTypeString()
            {
                string args2 = "";
                int i = 0;
                foreach (Argument arg in args)
                {
                    if (i < args.Count - 1)
                        args2 += arg.GetInternalFunctionTypeString() + "_and_";
                    else
                        args2 += arg.GetInternalFunctionTypeString();
                    i++;
                }
                string outs = "";
                i = 0;
                foreach (Variable arg in outputs)
                {
                    if (i < outputs.Count - 1)
                        outs += arg.GetInternalFunctionTypeString() + "_and_";
                    else
                        outs += arg.GetInternalFunctionTypeString();
                    i++;
                }
                return "function_from_" + args2 + "_to_" + outs + "_func";
            }
            public string GetInternalTypeString()
            {
                string args2 = "";
                int i = 0;
                foreach (Argument arg in args)
                {
                    if (i < args.Count - 1)
                        args2 += arg.GetInternalTypeString() + ",";
                    else
                        args2 += arg.GetInternalTypeString();
                    i++;
                }
                string outs = "";
                i = 0;
                foreach (Variable arg in outputs)
                {
                    if (i < outputs.Count - 1)
                        outs += arg.GetInternalTypeString() + ",";
                    else
                        outs += arg.GetInternalTypeString();
                    i++;
                }
                
                return "function<(" + args2 + "),(" + outs + ")>";
            }
            public void ModVar(Variable var)
            {
                if (!moddifiedVar.Contains(var))
                    moddifiedVar.Add(var);
            }
            public int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
        }
        public class Structure : CompilerObject
        {
            public string name;

            public List<Variable> fields = new List<Variable>();
            public List<Function> methods = new List<Function>();
            public List<string> methodsName = new List<string>();
            public List<string> generic = new List<string>();
            public Dictionary<string, string> compField = new Dictionary<string, string>();
            public Dictionary<string, string> typeMapContext = new Dictionary<string, string>();

            public string package;
            public string structureContext = context.GetVar();
            public bool isGeneric;
            public Structure parent;
            public List<Structure> interfaces;
            public bool isLazy;
            public bool isClass;
            public File classInitBase;

            public File genericFile = new File("____", "");
            public File initBase = new File("", "");
            private Variable representative;

            public bool isAbstract = false;
            public bool isStatic = false;
            public bool isPrivate = false;
            public bool isInterface = false;
            public string privateContext;

            public Structure(string name, Structure parent, List<Structure> interfaces, bool isPrivate)
            {
                this.name = name;
                this.parent = parent;
                this.package = currentPackage;
                foreach (var inter in interfaces)
                {
                    foreach (Variable v in inter.fields)
                    {
                        string varName = context.toInternal(context.GetVar() + v.name);
                        Variable variable = new Variable(v.name, varName, v.type, v.entity, v.def);

                        variable.isConst = v.isConst;
                        AddVariable(varName, variable);
                        if (v.type == Type.ENUM)
                            variable.SetEnum(v.enums);

                        fields.Add(variable);
                    }
                    foreach (Function f in inter.methods)
                    {
                        Function newFun = f.CopyTo(f.gameName, f.name, f.file);
                        methods.Add(newFun);
                    }
                    methodsName.AddRange(inter.methodsName);
                }
                if (parent != null)
                {
                    foreach (Variable v in parent.fields)
                    {
                        string varName = context.toInternal(context.GetVar() + v.name);
                        Variable variable = new Variable(v.name, varName, v.type, v.entity, v.def);

                        variable.isConst = v.isConst;
                        AddVariable(varName, variable);
                        if (v.type == Type.ENUM)
                            variable.SetEnum(v.enums);

                        fields.Add(variable);
                    }
                    foreach (Function f in parent.methods)
                    {
                        Function newFun = f.CopyTo(f.gameName, f.name, f.file);
                        methods.Add(newFun);
                    }
                    methodsName.AddRange(parent.methodsName);
                }
                if (typeMaps.Count > 0)
                {
                    foreach (string key in typeMaps.Peek().Keys)
                    {
                        typeMapContext.Add(key, typeMaps.Peek()[key]);
                    }
                }
                this.isPrivate = isPrivate;
                this.privateContext = context.GetVar();
            }

            public void addGeneric(string v)
            {
                generic.Add(v);
            }
            public void createGeneric(string name, string[] mType)
            {
                typeMaps.Push(new Dictionary<string, string>());
                for (int i = 0; i < generic.Count; i++)
                {
                    typeMaps.Peek().Add(generic[i].ToLower(), mType[i]);
                }
                context.Sub("_", new File("", ""));
                preparseLine("struct " + name + "{");
                structInstCompVar = true;
                foreach (string line in genericFile.parsed)
                {
                    preparseLine(line);
                }
                structInstCompVar = false;
                context.Parent();

                typeMaps.Pop();
            }

            public void addField(Variable variable)
            {
                fields.Insert(0, variable);
            }
            public void addMethod(Function element)
            {
                if (isStatic && !element.isStatic)
                    throw new Exception("Static Class/Struct Can only have static method.");
                if (!element.isVirtual)
                {
                    methods.Add(element);
                    methodsName.Add(element.name);
                }
                else
                {
                    string name = element.name;
                    element.name = "__virutal__" + element.name;
                    methods.Add(element);
                    methodsName.Add(element.name);

                    Variable var = new Variable(element.name, element.gameName.Replace("/", "").Replace(":", ""), Type.FUNCTION);
                    var.args = element.args;
                    var.outputs = element.outputs;
                    addField(var);

                    initBase.addParsedLine(element.name + "#=" + element.name);

                    File f = new File("f", "");
                    Function newFunc = element.CopyTo(name, name, f);
                    string args = "";
                    for (int i = 0; i < newFunc.args.Count; i++)
                    {
                        args += newFunc.args[i].name + ((i == newFunc.args.Count - 1) ? "" : ",");
                    }
                    if (element.outputs.Count == 0)
                        f.parsed.Add(element.name + "(" + args + ")");
                    else
                        f.parsed.Add("return " + element.name + "(" + args + ")");

                    methods.Add(newFunc);
                }
            }
            public void generateFunction(Function fun, Variable varOwner, string v, string cont, bool parentClass)
            {
                if (!parentClass || fun.tags.Count > 0 || fun.isTicking || fun.isLoading)
                {
                    string cont2 = context.GetVar();
                    string funcName = fun.name;
                    string contextStruct = null;
                    if (isClass)
                        contextStruct = representative.gameName;
                    else
                        contextStruct = context.GetVar().ToLower();

                    File fFile = new File(context.GetFile() + funcName);

                    if (!objectFunc.ContainsKey(currentFile))
                        objectFunc.Add(currentFile, new List<string>());
                    objectFunc[currentFile].Add(v+"."+ fun.name);
                    Function function = new Function(fun.name, context.GetFun() + funcName, fFile);
                    fFile.function = function;
                    function.desc = fun.desc;
                    function.lazy = fun.lazy;
                    function.isAbstract = fun.isAbstract;
                    function.varOwner = varOwner;
                    function.isTicking = fun.isTicking;
                    function.isLoading = fun.isLoading;
                    function.isHelper = fun.isHelper;
                    function.isPrivate = fun.isPrivate;
                    function.privateContext = context.GetVar();
                    function.variableStruct = varOwner.gameName;
                    function.argNeeded = fun.argNeeded;
                    function.maxArgNeeded = fun.maxArgNeeded;
                    function.package = package;
                    function.structure = contextStruct;
                    function.isOverride = fun.isOverride;
                    function.isStatic = fun.isStatic;
                    function.isStackSafe = fun.isStackSafe;
                    function.attributes = fun.attributes.ToList();

                    varOwner.associatedFunction.Add(function);

                    function.argNeeded = fun.argNeeded;
                    function.maxArgNeeded = fun.maxArgNeeded;

                    fFile.isLazy = fun.lazy;

                    if (structStack.Count > 0)
                        function.isStructMethod = true;

                    if (fun.name == "__init__" && isClass)
                    {
                        fFile.parsed.Add("__class__++");
                        fFile.parsed.Add(varOwner.gameName + " #= __class__");
                        foreach (string line in GetClassInitBase().parsed)
                        {
                            fFile.addParsedLine(line);
                        }
                        if (varOwner.entity)
                        {
                            fFile.parsed.Add("with(@e[tag=__class__],false,__CLASS__==__class__){");
                            fFile.parsed.Add(varOwner.gameName + " #= __class__");
                            fFile.parsed.Add("}");
                        }
                    }

                    #region tags
                    if (fun.isLoading)
                    {
                        if (isClass && parentClass)
                        {
                            string map = classOffuscationMapAdd(name);
                            AddLineToFileWithFunctionPriority(fun, loadFile, Core.As("@e[tag=" + map + "]") + "function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower() + "/w_0");
                        }
                        else if (!isClass)
                        {
                            AddLineToFileWithFunctionPriority(fun, loadFile, "function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                        }

                        if (callStackDisplay)
                            callTrace += "\"load\"->\"" + function.gameName + "\"\n";
                    }
                    if (fun.isTicking)
                    {
                        if (isClass && parentClass)
                        {
                            string map = classOffuscationMapAdd(name);
                            AddLineToFileWithFunctionPriority(fun, mainFile, Core.As("@e[tag=" + map + "]") + "function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower() + "/w_0");
                        }
                        else if (!isClass)
                        {
                            AddLineToFileWithFunctionPriority(fun, mainFile, "function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                        }
                        if (callStackDisplay)
                            callTrace += "\"main\"->\"" + function.gameName + "\"\n";
                    }
                    if (fun.isHelper)
                    {
                        fFile.use();
                        if (callStackDisplay)
                            callTrace += "\"helper\"->\"" + function.gameName + "\"\n";
                    }
                    #endregion
                    #region add to dic
                    files.Add(fFile);
                    string key = (context.GetFun() + funcName).Replace(':', '.').Replace('/', '.');
                    if (fun.isOverride)
                    {
                        bool contain = true;
                        while (contain)
                        {
                            contain = false;
                            for (int j = 0; j < functions[key].Count; j++)
                            {
                                Function f = functions[key][j];
                                if (f.gameName == function.gameName && f != function)
                                {
                                    contain = true;

                                    if (f.args.Count == 0 && fun.args.Count == 0)
                                    {
                                        fun.gameName = f.gameName;
                                        fFile.name = f.file.name;
                                        fun.name = f.name;
                                        functions[key].Remove(f);
                                        break;
                                    }
                                }
                            }
                        }
                        functions[key].Add(function);
                    }
                    else if (functions.ContainsKey(key))
                    {
                        functions[key].Add(function);
                        bool contain = true;
                        while (contain)
                        {
                            function.gameName = function.gameName + "_";
                            function.file.name = function.file.name + "_";
                            contain = false;
                            foreach (Function f in functions[key])
                            {
                                if (f.gameName == function.gameName && f != function)
                                {
                                    contain = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        List<Function> lst = new List<Function>();
                        lst.Add(function);
                        functions.Add(key, lst);
                    }
                    #endregion
                    context.Sub(fun.name, fFile);

                    fFile.UnparsedFunctionFile = !fun.lazy && structStack.Count == 0 && !fun.isAbstract;
                    fFile.UnparsedFunctionFileContext = context.GetVar();


                    foreach (string tag in fun.tags)
                    {
                        function.tags.Add(tag);
                        if (isClass && parentClass)
                        {
                            string map = classOffuscationMapAdd(name);
                            AddToFunctionTag(function, tag, map);
                        }
                        else if (!isClass)
                        {
                            AddToFunctionTag(function, tag);
                        }

                    }

                    fFile.notUsed = !fun.isLoading && !fun.isHelper && !fun.isTicking && fun.tags.Count == 0;

                    int i = 0;
                    foreach (Variable output in fun.outputs)
                    {
                        string varName = context.GetVar() + "ret_" + i.ToString();
                        Variable variable = new Variable(output.name, varName, output.type, output.entity, output.def);
                        variable.isConst = output.isConst;
                        if (variables.ContainsKey(varName))
                        {
                            variables.Remove(varName);
                        }
                        AddVariable(varName, variable);
                        if (output.type == Type.ENUM)
                            variable.SetEnum(output.enums);
                        if (output.type == Type.STRUCT)
                            variable.SetEnum(output.enums);
                        if (output.type == Type.STRUCT)
                        {
                            if (variable.enums != varOwner.enums)
                            {
                                structs[variable.enums].generate(variable.name, false, variable);
                            }
                            else
                            {
                                foreach (Variable strVar in structs[output.enums].fields)
                                {
                                    string varName2 = context.toInternal(context.GetInput() + output.name + "." + strVar.name);
                                    Variable variable2 = new Variable(strVar.name, varName2, strVar.type, false, strVar.def);
                                    variable2.isConst = strVar.isConst;
                                    AddVariable(varName2, variable2);
                                    if (strVar.type == Type.ENUM)
                                        variable2.SetEnum(strVar.enums);
                                }
                            }
                        }

                        if (output.type == Type.FUNCTION)
                        {
                            int j = 0;
                            foreach (Argument s in output.args)
                            {
                                Type type = s.type;
                                Argument arg = new Argument(i.ToString(), context.GetVar() + v + "." + j.ToString(), type);
                                arg.defValue = s.defValue;
                                Variable var2 = new Variable(i.ToString(), context.GetVar() + v + "." + j.ToString(), type);
                                AddVariable(context.GetVar() + v + "." + j.ToString(), var2);
                                variable.args.Add(arg);
                                j++;
                            }

                            j = 0;
                            foreach (Variable s in output.outputs)
                            {
                                Type type = s.type;
                                Variable var2 = new Variable(i.ToString(), context.GetVar() + v + ".ret_" + j.ToString(), type);
                                AddVariable(context.GetVar() + v + ".ret_" + j.ToString(), var2);
                                variable.outputs.Add(var2);
                                j++;
                            }
                        }

                        function.outputs.Add(variable);
                        i++;
                    }
                    foreach (Argument arg in fun.args)
                    {
                        Argument variable = new Argument(arg.name, context.GetInput() + arg.name, arg.type);
                        variable.enums = arg.enums;
                        variable.defValue = arg.defValue;
                        variable.variable = variable;
                        if (variables.ContainsKey(context.GetInput() + arg.name))
                        {
                            variables.Remove(context.GetInput() + arg.name);
                        }
                        AddVariable(context.GetInput() + arg.name, variable);

                        if (arg.type == Type.ENUM)
                        {
                            variable.SetEnum(getEnum(arg.enums));
                        }

                        if (arg.type == Type.STRUCT)
                            variable.SetEnum(arg.enums);

                        if (arg.type == Type.STRUCT)
                        {
                            if (variable.enums != varOwner.enums)
                            {
                                structs[variable.enums].generate(variable.name, false, variable);
                            }
                            else
                            {
                                foreach (Variable strVar in structs[arg.enums].fields)
                                {
                                    string varName = context.toInternal(context.GetInput() + arg.name + "." + strVar.name);
                                    Variable variable2 = new Variable(strVar.name, varName, strVar.type, false, strVar.def);
                                    variable2.isConst = strVar.isConst;
                                    AddVariable(varName, variable2);
                                    if (strVar.type == Type.ENUM)
                                        variable2.SetEnum(strVar.enums);
                                }
                            }
                        }
                        if (arg.type == Type.FUNCTION)
                        {
                            int j = 0;
                            foreach (Argument s in arg.variable.args)
                            {
                                Type type = s.type;
                                Argument arg2 = new Argument(i.ToString(), context.GetVar() + v + "." + j.ToString(), type);
                                arg2.defValue = s.defValue;
                                Variable var2 = new Variable(i.ToString(), context.GetVar() + v + "." + j.ToString(), type);
                                AddVariable(context.GetVar() + v + "." + j.ToString(), var2);
                                variable.args.Add(arg2);
                                j++;
                            }

                            j = 0;
                            foreach (Variable s in arg.variable.outputs)
                            {
                                Type type = s.type;
                                Variable var2 = new Variable(i.ToString(), context.GetVar() + v + ".ret_" + j.ToString(), type);
                                AddVariable(context.GetVar() + v + ".ret_" + j.ToString(), var2);
                                variable.outputs.Add(var2);
                                j++;
                            }
                        }

                        function.args.Add(variable);
                    }

                    context.Parent();

                    if (isClass)
                    {
                        if (varOwner.entity)
                        {
                            fFile.parsed.Add("__class_pointer__ #= " + varOwner.gameName);
                            fFile.parsed.Add("with(@e[tag=__class__],false,__CLASS__==__class_pointer__){");
                        }
                        else
                        {
                            fFile.parsed.Add("with(@e[tag=__class__],false,__CLASS__==" + varOwner.gameName + "){");
                        }
                    }
                    if (fun.name == "__init__")
                    {
                        foreach (string line in initBase.parsed)
                        {
                            fFile.addParsedLine(line);
                        }
                        if (isClass)
                        {
                            Structure tagGiver = this;
                            while (tagGiver != null)
                            {
                                string map = classOffuscationMapAdd(tagGiver.name);
                                fFile.addParsedLine("tag(" + map + ")");
                                tagGiver = tagGiver.parent;
                            }
                        }
                    }
                    foreach (string line in fun.file.parsed)
                    {
                        if (thisReg2.Match(line).Success)
                        {
                            if (context.compVal.Count > 0 && !(dualCompVar.Match(line).Success && structInstCompVar))
                            {
                                fFile.parsed.Add(compVarReplace(line));
                            }
                            else
                            {
                                fFile.parsed.Add(line);
                            }
                        }
                        else
                        {
                            if (context.compVal.Count > 0 && !(dualCompVar.Match(line).Success && structInstCompVar))
                            {
                                fFile.parsed.Add(compVarReplace(thisReg.Replace(line, "." + context.GetVar())));
                            }
                            else
                            {
                                fFile.parsed.Add(thisReg.Replace(line, "." + context.GetVar()));
                            }
                        }
                    }
                    if (isClass)
                    {
                        fFile.parsed.Add("}");
                    }
                }
            }
            public string generate(string v, bool entity, Variable varOwner, string instArg = null, bool parentClass = false)
            {
                structGenerating = true;
                if (isClass && parent == null && name != "object")
                {
                    parent = structs["object"];
                }
                if (isClass && !parentClass)
                {
                    if (representative == null)
                    {
                        Context c = context;
                        context = new Context(Project, new File("", ""));

                        string fName = context.GetVar() + "__class__";
                        representative = new Variable("__class__", fName, Type.STRUCT, false, "__class_id__");
                        if (variables.ContainsKey(fName))
                        {
                            variables.Remove(fName);
                        }
                        AddVariable(fName, representative);
                        representative.SetEnum(name);
                        generate("__class__", false, representative, instArg, true);

                        context = c;
                    }
                    foreach (Variable strVar in fields)
                    {
                        try
                        {
                            GetVariableByName(representative.gameName + "." + strVar.name).LinkCopyTo(context.GetVar() + v + "." + strVar.name);
                        }
                        catch
                        {

                        }
                    }
                }

                string cont = context.GetVar();
                string output = "";

                thisDef.Push(context.GetVar() + v + ".");
                context.Sub(v, new File("", ""));

                context.currentFile().notUsed = true;
                if (!isClass || parentClass)
                {
                    foreach (Variable strVar in fields)
                    {
                        try
                        {
                            string varName = context.toInternal(context.GetVar() + strVar.name);
                            var tmpV = strVar.CopyTo(varName, v, entity || strVar.entity || isClass, varOwner.isPrivate);
                            tmpV.parent = varOwner;
                            tmpV.inClass = isClass;
                        }
                        catch
                        {

                        }
                    }
                }
                if (parentClass)
                {
                    context.Sub(name.Replace(".", "_"), new File("", ""));
                }
                context.compVal[context.compVal.Count - 1]["$this"] = varOwner.uuid;
                context.compVal[context.compVal.Count - 1]["$this.lower"] = varOwner.uuid.ToLower();
                context.compVal[context.compVal.Count - 1]["$this.upper"] = varOwner.uuid.ToUpper();
                context.compVal[context.compVal.Count - 1]["$this.enums"] = varOwner.enums;
                context.compVal[context.compVal.Count - 1]["$this.type"] = varOwner.GetTypeString();
                context.compVal[context.compVal.Count - 1]["$this.name"] = varOwner.gameName;
                context.compVal[context.compVal.Count - 1]["$this.scoreboard"] = varOwner.scoreboard();
                context.compVal[context.compVal.Count - 1]["$this.scoreboardname"] = varOwner.scoreboard().Split(' ')[1];

                typeMaps.Push(new Dictionary<string, string>());
                foreach (string key in typeMapContext.Keys)
                {
                    typeMaps.Peek().Add(key, typeMapContext[key]);
                }
                foreach (string c in compField.Keys)
                {
                    context.compVal[context.compVal.Count - 1].Add(c, compField[c]);
                }

                bool prev = isInStructMethod;
                isInStructMethod = false;
                adjPackage.Push(package);

                if (instArg != null && !parentClass)
                {
                    structInstCompVar = true;

                    foreach (Function fun in methods)
                    {
                        if (fun.name == "__init__")
                        {
                            generateFunction(fun, varOwner, v, cont, parentClass);
                        }
                    }

                    structCompVarPointer = context.compVal[context.compVal.Count - 1];

                    context.Parent();
                    if (isClass)
                        Structure.DerefObject(varOwner);

                    if (isStatic)
                        throw new Exception($"Can not Instantiate Static Class/Struct {name}");

                    if (isAbstract)
                        throw new Exception($"Can not Instantiate Abstract Class/Struct {name}");

                    output += parseLine(v + ".__init__" + instArg);
                    context.Sub(v, new File("", ""));
                    context.compVal[context.compVal.Count - 1] = structCompVarPointer;
                    structCompVarPointer = null;
                    structInstCompVar = false;
                }

                foreach (Function fun in methods)
                {
                    if (fun.name != "__init__" || instArg == null)
                    {
                        generateFunction(fun, varOwner, v, cont, parentClass);
                    }
                }
                isInStructMethod = prev;
                int ci = 0;
                if (parentClass)
                {
                    context.Parent();
                }
                if (cont != context.GetVar())
                {
                    context.Parent();
                    ci++;
                }

                if (ci > 1)
                {
                    GlobalDebug(name, Color.Red);
                }

                thisDef.Pop();
                adjPackage.Pop();
                typeMaps.Pop();
                structGenerating = false;
                return output;
            }

            public bool isPolyTo(Structure stru)
            {
                if (name == stru.name)
                {
                    return true;
                }
                else if (parent == null)
                {
                    return false;
                }
                else
                {
                    return parent.isPolyTo(stru);
                }
            }

            public bool canBeAssignIn(Structure stru)
            {
                return isPolyTo(stru);
            }
            public File GetClassInitBase()
            {
                if (isClass && parent == null && name != "object")
                {
                    parent = structs["object"];
                }

                if (classInitBase == null)
                {
                    return parent.GetClassInitBase();
                }
                else
                {
                    return classInitBase;
                }
            }
            public static void DerefObject(Variable variable)
            {
                preparseLine("__class_pointer__ #= " + variable.gameName);
                preparseLine("with(@e[tag=__class__],false,__CLASS__==__class_pointer__){");
                preparseLine("__ref--");
                preparseLine("if (__ref <= 0){");
                if (IsFunction(variable.gameName.ToLower() + ".__destroy__"))
                {
                    preparseLine(variable.gameName + ".__destroy__()");
                }
                else
                {
                    preparseLine("/kill @s");
                }
                preparseLine("}");
                preparseLine("}");
            }
            public static void RefObject(Variable variable)
            {
                preparseLine("__class_pointer__ #= " + variable.gameName);
                preparseLine("with(@e[tag=__class__],false,__CLASS__==__class_pointer__){");
                preparseLine("__ref++");
                preparseLine("}");
            }
        }
        public class Variable : CompilerObject
        {
            public string name;
            public string gameName;
            public string enums;
            public Type type;
            public bool entity;
            public bool isConst = false;
            public bool wasSet = false;
            public bool isStatic = false;
            public bool inClass = false;
            public string def;
            public Scoreboard scoreboardObj;
            public bool isPrivate = false;
            public string privateContext;
            public bool isStructureVar;
            public int arraySize;
            private string score = null;
            public string uuid = "";
            public string typeArray;
            public string constValue;
            public string UnparsedFunctionFileContext;
            public bool wasUsed;
            public bool wasAdded;
            public Variable parent;
            public int accessTime = 0;

            public List<Argument> args = new List<Argument>();
            public List<Variable> outputs = new List<Variable>();
            public List<Function> associatedFunction = new List<Function>();


            public static Dictionary<string, int> ConstNumber;
            public static void INIT()
            {
                ConstNumber = new Dictionary<string, int>();
            }
            public static int GetID(string context)
            {
                if (ConstNumber.ContainsKey(context))
                {
                    int val = ConstNumber[context];
                    ConstNumber[context]++;
                    return val;
                }
                else
                {
                    ConstNumber.Add(context, 1);
                    return 0;
                }
            }

            private Variable() { }

            public Variable(string name, string gameName, Type type, bool entity = false, string def = "dummy")
            {
                this.name = name;
                this.gameName = gameName;
                this.type = type;
                this.entity = entity;
                this.def = def;
                this.attributes = Compiler.attributes;

                if (structStack.Count > 0 && !isInStaticMethod)
                {
                    isStructureVar = true;
                }

                if (entity && type == Type.STRUCT && def == "__class_id__")
                {
                    uuid = OffuscationMapAdd(gameName);
                    score = "@s " + uuid;

                    scoreboardObj = new Scoreboard(uuid, "dummy");
                }
                else if (entity && type != Type.STRUCT)
                {
                    uuid = OffuscationMapAdd(gameName);
                    score = "@s " + uuid;

                    scoreboardObj = new Scoreboard(uuid, def);
                }
                else
                {
                    uuid = OffuscationMapAdd(gameName);
                    score = uuid + " " + (isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue);

                    scoreboardObj = scoreboards[isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue];
                }

                if (attributes?.Contains("offuscationsaved")==true && compilerSetting.offuscate)
                {
                    if (!compilerSetting.forcedOffuscation.ContainsKey(gameName))
                    {
                        compilerSetting.forcedOffuscation.Add(gameName, uuid);
                    }
                }
            }
            public virtual void SetEnum(string enums)
            {
                this.enums = enums;
            }
            public virtual string scoreboard()
            {
                use();
                File cF = context.currentFile();
                if (cF != null && cF.function != null)
                {
                    cF.function.ModVar(this);
                }
                else
                {
                    accessTime++;
                }
                if (isPrivate)
                {
                    string cont = context.GetVar();
                    bool inPrivateContext = false;
                    foreach (string adj in adjPackage)
                    {
                        if (!cont.StartsWith(adj))
                            inPrivateContext = true;
                    }

                    if (!cont.StartsWith(privateContext) && !inPrivateContext)
                        throw new Exception("can not asign private variable in context: " + context.GetVar() + " from " + privateContext);
                }

                if (score != null)
                    return score;

                if (entity)
                {
                    score = "@s " + OffuscationMapAdd(gameName);
                    return score;
                }
                else
                {
                    score = OffuscationMapAdd(gameName) + " " + (isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue);
                    return score;
                }
            }

            public void CreateArray()
            {
                bool wasGenerating = structGenerating;
                structGenerating = true;
                string prefix = "";
                if (isPrivate) { prefix += "private "; }
                if (isStatic) { prefix += "static "; }
                if (isConst) { prefix += "const "; }

                context.Sub(name, new File("", "", ""));
                parseLine(prefix + "int " + "length = " + arraySize.ToString());

                for (int i = 0; i < arraySize; i++)
                {
                    parseLine(prefix + typeArray + " " + i.ToString());
                }

                preparseLine($"def invisible __get_mux__(int index):{typeArray}" + "{");
                preparseLine("switch(index){");
                preparseLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                preparseLine("case($_i){");
                preparseLine("return(" + name + ".$_i)");
                preparseLine("}");
                preparseLine("}");
                preparseLine("}");
                preparseLine("}");
                associatedFunction.Add(GetFunction(context.GetFunctionName("__get_mux__"), null));

                preparseLine("def invisible __set_mux__(int index, " + typeArray + " value){");
                preparseLine("switch(index){");
                preparseLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                preparseLine("case($_i){");
                preparseLine(name + ".$_i = value");
                preparseLine("}");
                preparseLine("}");
                preparseLine("}");
                preparseLine("}");
                associatedFunction.Add(GetFunction(context.GetFunctionName("__set_mux__"), null));

                parseLine("def lazy stacksafe set(int $a," + typeArray + " $b){");
                context.currentFile().addParsedLine($"ifs (__isint($a))" + "{");
                context.currentFile().addParsedLine(name + ".$a = $b");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine($"else" + "{");
                context.currentFile().addParsedLine($"{name}.__set_mux__($a,$b)");
                context.currentFile().addParsedLine("}");
                context.currentFile().Close();
                isInLazyCompile -= 1;
                associatedFunction.Add(GetFunction(context.GetFunctionName("set"), null));

                parseLine($"def lazy stacksafe get(int $a):{typeArray}" + "{");
                context.currentFile().addParsedLine($"ifs (__isint($a))" + "{");
                context.currentFile().addParsedLine("return(" + name + ".$a)");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine($"else" + "{");
                context.currentFile().addParsedLine("return(" + name + ".__get_mux__($a))");
                context.currentFile().addParsedLine("}");
                context.currentFile().Close();
                isInLazyCompile -= 1;
                associatedFunction.Add(GetFunction(context.GetFunctionName("get"), null));

                parseLine($"def invisible contains({typeArray} value):bool" + "{");
                context.currentFile().addParsedLine("bool res = false");
                context.currentFile().addParsedLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                context.currentFile().addParsedLine($"if ("+name + ".$_i == value)" + "{");
                context.currentFile().addParsedLine("res = true");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine("return(res)");
                context.currentFile().Close();
                isInLazyCompile -= 1;
                associatedFunction.Add(GetFunction(context.GetFunctionName("contains"), null));

                parseLine($"def invisible indexof({typeArray} value):int" + "{");
                context.currentFile().addParsedLine("int res = -1");
                context.currentFile().addParsedLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                context.currentFile().addParsedLine($"if (" + name + ".$_i == value && res == -1)" + "{");
                context.currentFile().addParsedLine("res = $_i");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine("return(res)");
                context.currentFile().Close();
                isInLazyCompile -= 1;
                associatedFunction.Add(GetFunction(context.GetFunctionName("indexof"), null));

                parseLine($"def invisible lastindexof({typeArray} value):int"+"{");
                context.currentFile().addParsedLine("int res = -1");
                context.currentFile().addParsedLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                context.currentFile().addParsedLine($"if (" + name + ".$_i == value)" + "{");
                context.currentFile().addParsedLine("res = $_i");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine("}");
                context.currentFile().addParsedLine("return(res)");
                context.currentFile().Close();
                isInLazyCompile -= 1;
                associatedFunction.Add(GetFunction(context.GetFunctionName("lastindexof"), null));



                foreach (Function f in GetVariableByName("0").associatedFunction)
                {
                    string outs = (f.outputs.Count > 0 ? f.outputs.Select(x => x.GetInternalTypeString()).Aggregate((x, y) => x + " " + y) : "void");
                    string ins = (f.args.Count > 0 ? "," + f.args.Select(x => x.GetInternalTypeString() + " " + x.name).Aggregate((x, y) => x + ", " + y) : "");
                    string insName = (f.args.Count > 0 ? f.args.Select(x => x.name).Aggregate((x, y) => x + ", " + y) : "");
                    string insName2 = (f.args.Count > 0 ? "," + f.args.Select(x => x.name).Aggregate((x, y) => x + ", " + y) : "");

                    parseLine($"def {outs} __dot_mux_{f.name}__(int _i {ins.Replace("$", "")})" + "{");
                    preparseLine("switch(_i){");
                    preparseLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                    preparseLine("case($_i){");
                    preparseLine($"{name}.$_i.{f.name}({insName.Replace("$", "")})");
                    preparseLine("}");
                    preparseLine("}");
                    preparseLine("}");
                    preparseLine("}");
                    associatedFunction.Add(GetFunction(context.GetFunctionName($"__dot_mux_{f.name}__"), null));

                    parseLine($"def lazy stacksafe {outs} dot_{f.name}(int $_i {ins})" + "{");
                    context.currentFile().addParsedLine($"ifs (__isint($_i))" + "{");
                    context.currentFile().addParsedLine($"return({name}.$_i.{f.name}({insName}))");
                    context.currentFile().addParsedLine("}");
                    context.currentFile().addParsedLine($"else" + "{");
                    context.currentFile().addParsedLine($"return({name}.__dot_mux_{f.name}__($_i{insName2}))");
                    context.currentFile().addParsedLine("}");
                    context.currentFile().Close();
                    isInLazyCompile -= 1;
                    associatedFunction.Add(GetFunction(context.GetFunctionName($"dot_{f.name}"), null));
                }

                context.Parent();
                structGenerating = wasGenerating;
            }

            public Variable CopyTo(string newName, string v, bool entity, bool copiedIsPrivate = false)
            {
                Variable variable = new Variable(this.name, newName, this.type, entity, this.def);
                variable.isConst = this.isConst;
                variable.isPrivate = copiedIsPrivate || isPrivate;
                variable.privateContext = context.GetVar();
                variable.attributes = attributes.ToList();

                AddVariable(newName, variable);

                if (this.type == Type.ENUM)
                {
                    variable.SetEnum(this.enums);
                    var c = context;
                    context = new Context(Project, new File("", ""));
                    var dest = variable.gameName.Substring(0, variable.gameName.LastIndexOf('.'));
                    context.GoTo(dest + ".", true);
                    Compiler.enums[this.enums].GenerateVariable(variable.name);
                    context = c;
                }

                if (this.type == Type.ARRAY)
                {
                    variable.arraySize = arraySize;
                    variable.typeArray = typeArray;
                    variable.CreateArray();
                }

                if (this.type == Type.STRUCT)
                {
                    variable.SetEnum(this.enums);
                    if (this.enums != name)
                    {
                        var c = context;
                        context = new Context(Project, new File("", ""));
                        var dest = variable.gameName.Substring(0, variable.gameName.LastIndexOf('.'));
                        context.GoTo(dest + ".", true);
                        structs[this.enums].generate(this.name, entity, variable);
                        context = c;
                    }
                }

                if (this.type == Type.FUNCTION)
                {
                    int i = 0;
                    foreach (Argument s in this.args)
                    {
                        Type type = s.type;
                        Argument arg = new Argument(i.ToString(), newName + "." + i.ToString(), type);
                        Variable var2 = new Variable(i.ToString(), newName + "." + i.ToString(), type);
                        AddVariable(newName + "." + i.ToString(), var2);
                        variable.args.Add(arg);
                        i++;
                    }

                    i = 0;
                    foreach (Variable s in this.outputs)
                    {
                        Type type = s.type;
                        Variable var2 = new Variable(i.ToString(), newName + ".ret_" + i.ToString(), type);
                        AddVariable(newName + ".ret_" + i.ToString(), var2);
                        variable.outputs.Add(var2);
                        i++;
                    }
                }
                return variable;
            }
            public void LinkCopyTo(string newName)
            {
                AddVariable(newName, this);

                if (this.type == Type.ARRAY)
                {
                    for (int i = 0; i < arraySize; i++)
                    {
                        GetVariableByName(gameName + "." + i.ToString()).LinkCopyTo(newName + "." + i.ToString());
                    }
                    GetVariableByName(gameName + ".length").LinkCopyTo(newName + ".length");
                }

                if (this.type == Type.STRUCT)
                {
                    foreach (Variable field in structs[this.enums].fields)
                    {
                        GetVariableByName(gameName + "." + field.name).LinkCopyTo(newName + "." + field.name);
                    }
                }

                if (this.type == Type.FUNCTION)
                {
                    int i = 0;
                    foreach (Argument s in this.args)
                    {
                        GetVariableByName(gameName + "." + i.ToString()).LinkCopyTo(newName + "." + i.ToString());
                        i++;
                    }

                    i = 0;
                    foreach (Variable s in this.outputs)
                    {
                        GetVariableByName(gameName + ".ret_" + i.ToString()).LinkCopyTo(newName + ".ret_" + i.ToString());
                        i++;
                    }
                }
                foreach (Function f in associatedFunction)
                {
                    f.LinkCopyTo(newName + "." + name);
                }
            }
            public Variable Select(string entitySelector)
            {
                wasUsed = true;

                Variable var = new Variable();
                var.name = name;
                var.gameName = gameName;
                var.enums = enums;
                var.isConst = isConst;
                var.type = type;
                var.entity = entity;
                var.isConst = isConst;
                var.def = def;
                var.isPrivate = isPrivate;
                var.privateContext = privateContext;
                var.arraySize = arraySize;
                var.isStructureVar = isStructureVar;
                var.score = score.Replace("@s", entitySelector);
                var.scoreboardObj = scoreboardObj;
                var.args = args;
                var.outputs = outputs;

                return var;
            }
            public string GetTypeString()
            {
                return GetInternalTypeString();
            }
            public string GetInternalTypeString()
            {
                if (type == Type.ENUM || type == Type.STRUCT)
                {
                    return enums;
                }
                else if (type == Type.FUNCTION)
                {
                    string args2 = "";
                    int i = 0;
                    foreach (Argument arg in args)
                    {
                        if (i < args.Count - 1)
                            args2 += arg.GetInternalTypeString() + ",";
                        else
                            args2 += arg.GetInternalTypeString();
                        i++;
                    }
                    string outs = "";
                    i = 0;
                    foreach (Variable arg in outputs)
                    {
                        if (i < outputs.Count - 1)
                            outs += arg.GetInternalTypeString() + ",";
                        else
                            outs += arg.GetInternalTypeString();
                        i++;
                    }
                    return type.ToString().ToLower() + "<(" + args2 + "),(" + outs + ")>";
                }
                else
                    return type.ToString().ToLower();
            }
            public string GetFancyTypeString()
            {
                if (type == Type.ENUM || type == Type.STRUCT)
                {
                    return enums;
                }
                else if (type == Type.FUNCTION)
                {
                    string args2 = "";
                    int i = 0;
                    foreach (Argument arg in args)
                    {
                        if (i < args.Count - 1)
                            args2 += arg.GetFancyTypeString() + ",";
                        else
                            args2 += arg.GetFancyTypeString();
                        i++;
                    }
                    string outs = "";
                    i = 0;
                    foreach (Variable arg in outputs)
                    {
                        if (i < outputs.Count - 1)
                            outs += arg.GetFancyTypeString() + ",";
                        else
                            outs += arg.GetFancyTypeString();
                        i++;
                    }
                    string output = "";
                    if (args2.Contains(","))
                    {
                        output += $"({args2})";
                    }
                    else if (args2 == "")
                    {
                        output += "void";
                    }
                    else
                    {
                        output += args2;
                    }
                    output += "=>";
                    if (outs.Contains(","))
                    {
                        output += $"({outs})";
                    }
                    else if (outs == "")
                    {
                        output += "void";
                    }
                    else
                    {
                        output += outs;
                    }
                    return output;
                }
                else
                    return type.ToString().ToLower();
            }
            public string GetInternalFunctionTypeString()
            {
                if (type == Type.ENUM || type == Type.STRUCT)
                {
                    return enums;
                }
                else if (type == Type.FUNCTION)
                {
                    string args2 = "";
                    int i = 0;
                    foreach (Argument arg in args)
                    {
                        if (i < args.Count - 1)
                            args2 += arg.GetInternalFunctionTypeString() + "_and_";
                        else
                            args2 += arg.GetInternalFunctionTypeString();
                        i++;
                    }
                    string outs = "";
                    i = 0;
                    foreach (Variable arg in outputs)
                    {
                        if (i < outputs.Count - 1)
                            outs += arg.GetInternalFunctionTypeString() + "_and_";
                        else
                            outs += arg.GetInternalFunctionTypeString();
                        i++;
                    }
                    return type.ToString().ToLower() + "_from_" + args2 + "_to_" + outs + "_func";
                }
                else
                    return type.ToString().ToLower();
            }

            public void use()
            {
                if (!entity)
                {
                    scoreboardObj = scoreboards[isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue];
                }

                wasUsed = true;
                if (parent != null)
                    parent.use();
                scoreboardObj.use();
            }
        }
        public class Argument : Variable
        {
            public string defValue = null;
            public bool isImplicite;
            public bool isLazy;
            public Argument(string name, string gameName, Type type) : base(name, gameName, type)
            {
                isLazy = name.StartsWith("$");
            }
            public Variable variable;

        }
        public class Scoreboard
        {
            public string name;
            public string property;
            public bool wasUsed;
            public bool wasAdded;

            public Scoreboard(string name, string property)
            {
                this.name = name;
                this.property = property;

                scoreboards.Add(name, this);
            }
            public void use()
            {
                wasUsed = true;
            }
        }
        public class Enum
        {
            public class EnumValue
            {
                public string value;
                public Dictionary<string, string> fields = new Dictionary<string, string>();

                public EnumValue(string value)
                {
                    this.value = value;
                }
            }
            public class EnumField
            {
                public Enum parent;
                public string type;
                public string name;
                public string defaultVal;
                public Function functionGet;
                private string funcName;
                private Variable multiplexer;
                private Variable output;

                public EnumField(Enum parent, string type, string name, string defaultVal)
                {
                    this.parent = parent;
                    this.type = type.ToLower();
                    this.name = name.ToLower();
                    this.defaultVal = defaultVal;

                    if (type == "json" && defaultVal.StartsWith("(("))
                    {
                        defaultVal = getArg(defaultVal);
                    }
                    if (type != "json")
                    {
                        funcName = "__getEnumField__." + parent.name + "." + name + "";

                        preparseLine("def __getEnumField__." + parent.name + "." + name + "(int value):" + type + "{");
                        multiplexer = GetVariableByName(funcName.ToLower() + ".value");
                        output = GetVariableByName(funcName.ToLower() + ".ret_0");
                        preparseLine("}");

                        functionGet = GetFunction(context.GetFunctionName(funcName), new string[] { "0" });
                        functionGet.file.isLazy = false;
                        functionGet.file.notUsed = true;
                        functionGet.file.UnparsedFunctionFile = false;
                    }
                }

                public void AddValue(int index, EnumValue value)
                {
                    if (type != "json")
                    {
                        if (value.fields.ContainsKey(name))
                        {
                            string cond = getCondition(multiplexer.gameName + "==" + index.ToString());
                            string line = output.gameName + "=" + value.fields[name];
                            functionGet.file.AddLine(cond + parseLine(line));
                        }
                        else if (defaultVal != "")
                        {
                            string cond = getCondition(multiplexer.gameName + "==" + index.ToString());
                            string line = output.gameName + "=" + defaultVal;
                            functionGet.file.AddLine(cond + parseLine(line));
                        }
                        else
                            throw new Exception("No value for field \"" + name + "\" in enum value " + value.value);
                    }
                }
            }

            public List<EnumValue> values = new List<EnumValue>();
            public List<EnumField> fields = new List<EnumField>();
            public List<string> fieldsName = new List<string>();
            public List<string> valuesName = new List<string>();
            public bool final = false;
            public string name;

            public bool isPrivate = false;
            public string privateContext;

            public Enum(string name, string[] values, bool final = false, bool isPrivate = false)
            {
                this.name = name;
                foreach (string value in values)
                {
                    Add(value);
                }
                this.final = final;
                this.isPrivate = isPrivate;
                this.privateContext = context.GetVar();
            }
            public Enum(string name, string[] fields, string[] values, bool final = false, bool isPrivate = false)
            {
                this.name = name;
                foreach (string field in fields)
                {
                    string type = smartSplit(field, ' ')[0];
                    string fname = smartSplit(field, ' ')[1];

                    string defaultVal = "";
                    if (field.Contains("="))
                        defaultVal = smartSplit(field, '=')[1];

                    fieldsName.Add(fname.ToLower());
                    this.fields.Add(new EnumField(this, type, fname, defaultVal));
                }
                foreach (string value in values)
                {
                    Add(value);
                }
                this.final = final;
                this.isPrivate = isPrivate;
                this.privateContext = context.GetVar();
            }

            public bool Contains(string value)
            {
                foreach (EnumValue v in values)
                {
                    if (v.value.ToLower() == value.ToLower())
                    {
                        return true;
                    }
                }
                return false;
            }

            public int IndexOf(string value)
            {
                int i = 0;
                foreach (EnumValue v in values)
                {
                    if (v.value.ToLower() == value.ToLower())
                    {
                        return i;
                    }
                    i++;
                }
                return -1;
            }
            public void Add(string value)
            {
                value = smartEmpty(value);
                string[] fields = new string[] { };
                string name = value;
                if (value.Contains("("))
                {
                    fields = getArgs(value);
                    name = value.Substring(0, value.IndexOf('(')).ToLower();
                }
                int index = values.Count;

                EnumValue enumvalue = new EnumValue(name);
                int i = 0;
                foreach (string field in fields)
                {
                    if (field.Contains("="))
                    {
                        string[] subField = smartSplit(field, '=');
                        string fieldName = smartEmpty(subField[0]).ToLower();
                        string val = smartExtract(subField[1]);
                        string type = this.fields[fieldsName.IndexOf(fieldName)].type;
                        if (type == "json" && smartExtract(val).StartsWith("("))
                        {
                            val = getArg(val);
                        }
                        if (fieldsName.Contains(fieldName))
                            enumvalue.fields.Add(fieldName, val);
                        else
                            throw new Exception("Unknown field " + fieldName);
                    }
                    else
                    {
                        string val = smartExtract(field);
                        string type = this.fields[i].type;
                        if (type == "json" && smartExtract(val).StartsWith("("))
                        {
                            val = getArg(val);
                        }
                        enumvalue.fields.Add(fieldsName[i], val);
                    }
                    i++;
                }

                foreach (EnumField enumField in this.fields)
                {
                    enumField.AddValue(index, enumvalue);
                }

                if (final)
                {
                    throw new Exception("Cannot add To final enum");
                }
                if (!Contains(name))
                {
                    valuesName.Add(name);
                    values.Add(enumvalue);
                }
            }
            public string GetFieldOf(string value, string field)
            {
                int n = -1;
                EnumValue v;
                if (int.TryParse(value, out n))
                {
                    v = values[n];
                }
                else
                {
                    v = values[valuesName.IndexOf(value)];
                }
                if (v.fields.ContainsKey(field))
                {
                    return v.fields[field];
                }
                else
                {
                    return fields[fieldsName.IndexOf(field)].defaultVal;
                }
            }
            public string GetValueOf(string value)
            {
                if (valuesName.Contains(value))
                {
                    return value;
                }
                else
                {
                    return valuesName[int.Parse(value)];
                }
            }

            public void GenerateVariable(string name)
            {
                if (structStack.Count == 0)
                {
                    context.Sub(name, new File("", ""));
                    foreach (EnumField field in fields)
                    {
                        if (field.type != "json")
                        {
                            preparseLine("def lazy " + field.name + "():" + field.type + "{");
                            preparseLine("return(" + field.functionGet.gameName.Replace("/", ".").Replace(":", ".") + "(" + name + "))");
                            preparseLine("}");
                        }
                    }
                    context.Parent();
                }
            }
            public List<string> Values()
            {
                List<string> lst = new List<string>();
                foreach (EnumValue v in values)
                {
                    lst.Add(v.value);
                }
                return lst;
            }
        }
        public interface Component
        {
            string Compile();
        }
        public enum Type
        {
            UNKNOWN,
            INT,
            FLOAT,
            BOOL,
            ENTITY,
            ENTITY_COMPONENT,
            ENUM,
            FUNCTION,
            STRUCT,
            STRING,
            VOID,
            ARRAY,
            JSON,
            PARAMS,
            DEFINE
        }
        #endregion

        #region Structure Class
        public class Switch : Component
        {
            List<Case> casesUnit = new List<Case>();
            List<Case> casesRange = new List<Case>();
            string type;
            public Variable variable;
            Variable copyFrom;
            string text;
            int treeBottom;
            string[] sizes = new string[0];
            int id;

            public static Dictionary<string, int> SwitchNumber;
            
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }

            public Switch(string text, int id)
            {
                this.text = text;
                this.id = id;
                CreateVariable(text);
                treeBottom = compilerSetting.TreeMaxSize;
            }
            public Switch(Variable var, int id)
            {
                this.text = var.name;
                this.id = id;
                variable = var;
                treeBottom = compilerSetting.TreeMaxSize;
            }
            public Switch(string text, string[] sizes, int id)
            {
                this.text = text;
                this.id = id;
                CreateVariable(text);
                this.treeBottom = int.Parse(sizes[1]);
                sizes = new string[sizes.Length - 1];
                for (int i = 1; i < sizes.Length; i++)
                {
                    this.sizes[i - 1] = sizes[i];
                }
            }
            public Switch(Variable variable, List<Case> cases, string[] sizes)
            {
                this.casesUnit.AddRange(cases);
                this.variable = variable;
                if (sizes.Length == 0)
                {
                    treeBottom = compilerSetting.TreeMaxSize;
                }
                else
                {
                    this.treeBottom = int.Parse(sizes[0]);
                    this.sizes = sizes;
                }
                type = variable.GetInternalTypeString();
            }

            public void CreateVariable(string text)
            {
                type = getExprTypeStr(text);

                string name = "_s." + id.ToString();

                parseLine(type + " " + name);


                copyFrom = GetVariableByName(smartExtract(text), true);

                variable = GetVariableByName(name);
            }
            public string Compile()
            {
                bool createVar = casesUnit.TrueForAll(x => !x.cmd.Contains("function") 
                                                        && !(x.cmd.StartsWith("scoreboard") && x.cmd.Contains(copyFrom.scoreboard()))) &&
                                 casesRange.TrueForAll(x => !x.cmd.Contains("function")
                                                        && !(x.cmd.StartsWith("scoreboard") && x.cmd.Contains(copyFrom.scoreboard()))) &&
                                 copyFrom != null;
                
                if (createVar)
                    variable = copyFrom;
                return Compile(!createVar);
            }
            public string Compile(bool createVariable)
            {
                if (treeBottom < 1)
                {
                    treeBottom = (int)Math.Max(Math.Sqrt(casesUnit.Count), compilerSetting.TreeMaxSize);
                }
                List<Case> subList(List<Case> cases, int min, int max)
                {
                    List<Case> outp = new List<Case>();
                    for (int i = min; i <= max; i++)
                    {
                        outp.Add(cases[i]);
                    }
                    return outp;
                }
                string[] tail = new string[Math.Max(sizes.Length - 1, 0)];
                for (int i = 1; i < sizes.Length; i++)
                {
                    tail[i - 1] = sizes[i];
                }

                casesUnit.Sort((a, b) => a.value.CompareTo(b.value));
                int subTreeSize = (int)Math.Max(treeBottom, Math.Pow(treeBottom, Math.Log(casesUnit.Count, treeBottom)-1));
                if (casesUnit.Count > subTreeSize)
                {
                    string text = "";
                    if (createVariable)
                        text += eval(this.text, variable, variable.type, "=");
                    for (int i = 0; i < Math.Ceiling((casesUnit.Count * 1.0) / subTreeSize); i++)
                    {
                        string contName = "__splitted_" + i.ToString();
                        string funcName = (context.GetFun() + contName);
                        string c = Core.FileNameSplitter()[0];
                        string subName = funcName.Substring(funcName.IndexOf(c) + 1, funcName.Length - funcName.IndexOf(c) - 1);
                        File f = new File(subName);
                        files.Add(f);
                        context.currentFile().addChild(f);

                        int iMin = i * subTreeSize;
                        int iMax = Math.Min((i + 1) * subTreeSize - 1, casesUnit.Count - 1);

                        Switch s = new Switch(variable, subList(casesUnit, iMin, iMax), tail);
                        context.Sub(contName, f);
                        f.AddLine(s.Compile(false));
                        context.Parent();
                        string cmd = "function " + funcName + '\n';
                        text += getCondition(variable.gameName + "==" +
                            casesUnit[iMin].value.ToString() + ".." +
                            casesUnit[iMax].value.ToString())
                            + cmd + "\n";
                    }
                    foreach (Case c in casesRange)
                    {
                        text += getCondition(variable.gameName + "==" + c.valueStr) + c.cmd + "\n";
                    }
                    return text.Replace("\n\n", "\n");
                }
                else
                {
                    string text = "";
                    if (createVariable)
                        text = eval(this.text, variable, variable.type, "=");
                    foreach (Case c in casesUnit)
                    {
                        if (variable.type == Type.FLOAT)
                            text += getCondition(variable.gameName + "==" + ((c.value * 1d) / compilerSetting.FloatPrecision).ToString()) + c.cmd + "\n";
                        else
                            text += getCondition(variable.gameName + "==" + c.value.ToString()) + c.cmd + "\n";
                    }
                    foreach (Case c in casesRange)
                    {
                        text += getCondition(variable.gameName + "==" + c.valueStr) + c.cmd + "\n";
                    }
                    return text.Replace("\n\n", "\n");
                }
            }
            public Case Add(string cond, string cmd)
            {
                if (variable.type == Type.ENUM)
                {
                    if (variable.enums != null && enums[variable.enums].valuesName.Contains(smartEmpty(cond).ToLower()))
                    {
                        casesUnit.Add(new Case(enums[variable.enums].valuesName.IndexOf(smartEmpty(cond).ToLower()), cmd));
                        return casesUnit[casesUnit.Count - 1];
                    }
                    else
                    {
                        casesRange.Add(new Case(cond, cmd));
                        return casesRange[casesRange.Count - 1];
                    }
                }
                else if (variable.type == Type.INT)
                {
                    if (int.TryParse(cond, out int _))
                    {
                        casesUnit.Add(new Case(int.Parse(cond), cmd));
                        return casesUnit[casesUnit.Count - 1];
                    }
                    else
                    {
                        casesRange.Add(new Case(cond, cmd));
                        return casesRange[casesRange.Count - 1];
                    }
                }
                else if (variable.type == Type.FLOAT)
                {
                    if (double.TryParse(cond, out double _))
                    {
                        casesUnit.Add(new Case((int)(double.Parse(cond) * compilerSetting.FloatPrecision), cmd));
                        return casesUnit[casesUnit.Count - 1];
                    }
                    else
                    {
                        casesRange.Add(new Case(cond, cmd));
                        return casesRange[casesRange.Count - 1];
                    }
                }
                else
                {
                    casesRange.Add(new Case(cond, cmd));
                    return casesRange[casesRange.Count - 1];
                }
            }
            public int Count()
            {
                return casesUnit.Count + casesRange.Count;
            }

            public class Case
            {
                public int value;
                public string valueStr;
                public string cmd;

                public Case(int value, string cmd)
                {
                    this.value = value;
                    this.cmd = cmd;
                }

                public Case(string valueStr, string cmd)
                {
                    this.valueStr = valueStr;
                    this.cmd = cmd;
                }
            }
        }
        public class If
        {
            public static Dictionary<string, int> SwitchNumber;
            public static Dictionary<string, int> EvalNumber;
            public static Dictionary<string, Stack<int>> conditionsIDStack;
            public static Dictionary<string, int> conditionsID;

            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
                EvalNumber = new Dictionary<string, int>();
                conditionsIDStack = new Dictionary<string, Stack<int>>();
                conditionsID = new Dictionary<string, int>();
            }

            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static int GetEval(string context)
            {
                if (EvalNumber.ContainsKey(context))
                {
                    int val = EvalNumber[context];
                    EvalNumber[context]++;
                    return val;
                }
                else
                {
                    EvalNumber.Add(context, 1);
                    return 0;
                }
            }


            public int id;
            public bool wasAlwayTrue;
            public bool wasAlwayFalse;
            public bool wasElseAlwayTrue;

            public If(int forced)
            {
                id = forced;
            }
            public If()
            {
                id = GetID(context.GetFun());
            }

            public override string ToString()
            {
                throw new Exception("That's illegal!");
            }
        }
        public class While
        {
            public static Dictionary<string, int> SwitchNumber;
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
        }
        public class For
        {
            public static Dictionary<string, int> SwitchNumber;
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
        }
        public class Forgenerate
        {
            public static Dictionary<string, int> SwitchNumber;
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
        }
        public class With
        {
            public static Dictionary<string, int> SwitchNumber;
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
        }
        public class At
        {
            public static Dictionary<string, int> SwitchNumber;
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
        }
        public class Lambda
        {
            public static Dictionary<string, int> SwitchNumber;
            public static int GetID(string context)
            {
                if (SwitchNumber.ContainsKey(context))
                {
                    int val = SwitchNumber[context];
                    SwitchNumber[context]++;
                    return val;
                }
                else
                {
                    SwitchNumber.Add(context, 1);
                    return 0;
                }
            }
            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
            }
        }
        public class Predicate
        {
            public string name;
            public string[] args;
            public File baseFile;
            public List<string> generated = new List<string>();

            public Predicate(string name, string[] args, File file)
            {
                this.name = name;
                this.args = args;
                baseFile = file;
            }

            public string get(string arg)
            {
                arg = smartEmpty(arg);
                string[] args2 = smartSplitJson(arg, ',');
                if (!generated.Contains(arg))
                {
                    string text = baseFile.content;
                    for (int i = 0; i < args.Length; i++)
                    {
                        text = text.Replace(args[i], args2[i].Replace("\"", "\\\""));
                    }
                    string filename = name.Substring(name.IndexOf(":") + 1, name.Length - name.IndexOf(":") - 1);
                    File f = new File("predicates/" + filename + "_" + args2.Length.ToString() + "_" + generated.Count(), "", "json");
                    f.AddLine(text);
                    f.use();
                    generated.Add(arg);
                    files.Add(f);
                }
                return name + "_" + args2.Length.ToString() + "_" + generated.IndexOf(arg);
            }
        }
        #endregion

        [Serializable]
        public class CompilerSetting
        {
            public int TreeMaxSize = 20;
            public int FloatPrecision = 1000;
            public bool removeUselessFile = true;
            public bool offuscate = true;
            public string scoreboardValue = "tbms.value";
            public string scoreboardConst = "tbms.const";
            public string scoreboardTmp = "tbms.tmp";
            public bool tagsFolder = true;
            public Dictionary<string, string> forcedOffuscation = new Dictionary<string, string>();
            public List<string> libraryFolder = new List<string>();
            public string MCVersion = "1.17";
            public bool ExportAsZip = false;
            public int packformat = 7;
            public int rppackformat = 7;
            public string CompilerCoreName = "java";
            public string Authors = "";
            public bool generateMAPSFile = true;
            public bool generateREADMEFile = true;

            public bool opti_FunctionTagsReplace = true;
            public bool opti_ExportComment = false;
            public bool opti_ShowException = true;
            public bool opti_ShowDebug = true;
            public bool advanced_debug = false;

            public bool isLibrary = true;

            public CompilerSetting()
            {
                packformat = 6;
            }

            public CompilerSetting withoutOffuscation()
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, this);
                    ms.Position = 0;

                    CompilerSetting newSetting = (CompilerSetting)formatter.Deserialize(ms);
                    newSetting.offuscate = false;
                    newSetting.forcedOffuscation = forcedOffuscation;
                    return newSetting;
                }
            }
        }

        public class ParenthiseError
        {
            public int line;
            public int column;
            public char c;
            public bool expected;

            public ParenthiseError(int line, int column, char c, bool expected = false)
            {
                this.line = line;
                this.column = column;
                this.c = c;
                this.expected = expected;
            }

            public void throwException()
            {
                if (expected)
                {
                    throw new Exception("Unexcepted " + c + " at line " + line.ToString() + " column " + column.ToString());
                }
                throw new Exception("Excepted " + c + " at line " + line.ToString() + " column " + column.ToString());
            }
        }
        public class ImpliciteVar
        {
            public string enums;
            public Type type;
            public string value;

            public ImpliciteVar(string enums, Type type, string value)
            {
                this.enums = enums;
                this.type = type;
                this.value = value;
            }
        }
        public class MatchCustom
        {
            public int Index;
            public int Length;
            public string Value;

            public MatchCustom(int index, string Value)
            {
                this.Index = index;
                this.Length = Value.Length;
                this.Value = Value;
            }
        }

        public class File
        {
            public string name;
            public string content;
            public string abstractContent;
            public string ending;
            public string start = "";
            public bool valid = true;
            public StringBuilder scoreboardDef = new StringBuilder();
            public List<string> parsed = new List<string>();
            public List<string> parsed_end = new List<string>();
            public bool isLazy;
            public string type;
            public int lineCount = 0;
            public bool cantMergeWith;
            public string var;
            public double min;
            public double max;
            public double step;
            public string enumGen;
            private int genIndex;
            private int genAmount;
            public bool UnparsedFunctionFile;
            public string UnparsedFunctionFileContext;
            public bool resourcespack;
            public Dictionary<string, string> typeMapContext = new Dictionary<string, string>();
            public bool multiUsed = false;
            public List<Dictionary<string, string>> lazyVarStack;

            public Function function;
            public Switch.Case switchcase;

            public bool notUsed = false;
            public List<File> childs = new List<File>();
            public File parent;

            public File(string name, string content = "", string type = "")
            {
                this.name = name;
                this.content = content;
                this.type = type;
                lazyVarStack = GetLazyValStack();

                if (typeMaps.Count > 0)
                {
                    foreach (string key in typeMaps.Peek().Keys)
                    {
                        typeMapContext.Add(key, typeMaps.Peek()[key]);
                    }
                }
            }

            public File AddLine(string cont)
            {
                if (cont != "")
                {
                    if (!cont.EndsWith("\n"))
                        content += cont + '\n';
                    else
                        content += cont.Replace("\n\n", "\n");

                    if (cont.Contains('\n'))
                        lineCount += cont.Split('\n').Length - 1;
                    else
                        lineCount++;
                }
                return this;
            }
            public void AddScoreboardDefLine(string cont)
            {
                scoreboardDef.Append(cont);
                scoreboardDef.Append('\n');
                //scoreboardDef = cont + '\n' + scoreboardDef;
                if (cont.Contains('\n'))
                    lineCount += cont.Split('\n').Length - 1;
                else
                    lineCount++;
            }
            public void AddStartLine(string cont)
            {
                start = cont + '\n' + start;
                if (cont.Contains('\n'))
                    lineCount += cont.Split('\n').Length - 1;
                else
                    lineCount++;
            }

            public void AddEndLine(string cont)
            {
                ending = cont + '\n' + ending;
                if (cont.Contains('\n'))
                    lineCount += cont.Split('\n').Length - 1;
                else
                    lineCount++;
            }
            public void generate(string value, Enum enums = null, string file = null)
            {
                isInLazyCompile = 0;
                var dic = new Dictionary<string, string>();

                if (!structInstCompVar)
                {
                    context.compVal.Add(dic);
                }

                if (file != null)
                {
                    string fullName = value;
                    value = value.Replace(file, "");
                    if (fullName.EndsWith(".png"))
                    {
                        Image img = Bitmap.FromFile(fullName);
                        context.compVal[context.compVal.Count - 1].Add(var + ".width", img.Width.ToString());
                        context.compVal[context.compVal.Count - 1].Add(var + ".height", img.Height.ToString());
                    }
                }
                File f = context.currentFile();
                foreach (string l in parsed)
                {
                    string line = l;
                    if (enums != null)
                    {
                        List<string> sortedField = new List<string>();
                        sortedField.AddRange(enums.fieldsName);
                        sortedField.Sort((a, b) => b.Length.CompareTo(a.Length));
                        foreach (string field in sortedField)
                        {
                            line = line.Replace(var + "." + field, enums.GetFieldOf(value, field));
                        }
                    }
                    else if (value.StartsWith("("))
                    {
                        string[] argget = getArgs(value);
                        for (int i = argget.Length - 1; i >= 0; i--)
                        {
                            line = line.Replace(var + "." + i.ToString(), smartExtract(argget[i]));
                        }
                        line = line.Replace(var + ".count", argget.Length.ToString());
                    }

                    line = line.Replace(var + ".index", genIndex.ToString())
                        .Replace(var + ".length", genAmount.ToString())
                        .Replace(var, value);

                    preparseLine(line, f);
                }
                if (jsonIndent == 1)
                {
                    preparseLine("}");
                }
                if (context.compVal.Count > 0 && !structInstCompVar)
                {
                    context.compVal.RemoveAt(context.compVal.Count - 1);
                }
                genIndex++;
            }
            public void forgenerate()
            {
                string tmp = content;
                content = "";
                genIndex = 0;
                if (enumGen != null && enumGen.StartsWith("("))
                {
                    string[] values = getArgs(enumGen);
                    genAmount = values.Length;
                    foreach (string value in values)
                    {
                        if (value != "")
                            generate(value);
                    }
                }
                else if (enumGen != null && enums.ContainsKey(enumGen))
                {
                    genAmount = enums[enumGen].values.Count;
                    foreach (string value in enums[enumGen].Values())
                    {
                        generate(value, enums[enumGen]);
                    }
                }
                else if (enumGen != null && structs.ContainsKey(enumGen))
                {
                    genAmount = structs[enumGen].fields.Count;
                    foreach (Variable value in structs[enumGen].fields)
                    {
                        generate(value.name);
                    }
                }
                else if (enumGen != null && enumGen.StartsWith("files("))
                {
                    string[] args = getArgs(enumGen);
                    string filter = "*.*";
                    if (args.Length > 1) { filter = "*." + extractString(args[1]); }

                    string[] files = Directory.GetFiles(projectFolder + "/resourcespack/" + extractString(args[0]), "*.*", SearchOption.AllDirectories);
                    genAmount = files.Length;
                    foreach (string value in files)
                    {
                        generate(value.Replace("\\", "/"), null, (projectFolder + "/resourcespack/" + extractString(args[0]) + "/").Replace("\\", "/"));
                    }
                }
                else if (enumGen != null && functionTags.ContainsKey(enumGen.Replace("@", "")))
                {
                    genAmount = functionTags[enumGen.Replace("@", "")].Count;
                    foreach (string value in functionTags[enumGen.Replace("@", "")])
                    {
                        generate(value);
                    }
                }
                else if (enumGen != null)
                {
                    throw new Exception("Unknown generator:" + enumGen);
                }
                else
                {
                    genAmount = (int)(Math.Ceiling((max - min) / step));
                    for (double i = min; i != max + step; i += step)
                    {
                        generate(i.ToString());
                    }
                }
            }
            public virtual void Close(bool debug = false)
            {
                content = content.Replace("run  execute", "run execute");
                content = scoreboardDef.ToString() + start + content + ending;
                ending = "";
                start = "";

                while (content.Contains("\n\n"))
                {
                    content = content.Replace("\n\n", "\n");
                }
                lineCount = content.Split('\n').Length - 1;

                if (type == "struct")
                {
                    structStack.Pop();
                    context.currentFile().unuse();
                }

                context.Parent();

                if (type == "forgenerate")
                {
                    forgenerate();
                }

                if (type == "if" || type == "if_empty")
                {
                    if (LastConds.Count > 0)
                        LastCond = LastConds.Pop();
                }

                if (function != null && function.isLambda && (parent == null || parent.function != function) && lineCount == 1 && !content.StartsWith("#"))
                {
                    File f = context.currentFile();
                    string tmp = f.content.Replace("function " + function.gameName, content);
                    f.content = tmp;
                    valid = false;
                }
                if (type == "if" && ((LastCond.wasAlwayTrue || LastCond.wasElseAlwayTrue) && lineCount > 1))
                {
                    File f = context.currentFile();
                    string tmp = f.content + content;
                    f.content = tmp;
                    valid = false;
                }
                if (type == "lazyfunctioncall")
                {
                    File f = context.currentFile();
                    string tmp = f.content + content;
                    f.content = tmp;
                    valid = false;
                }
                if ((type == "if" || (type == "with" && !cantMergeWith) || type == "at") && lineCount == 1 && !content.StartsWith("#"))
                {
                    if ((LastCond.wasAlwayTrue || LastCond.wasElseAlwayTrue) && type == "if")
                    {
                        File f = context.currentFile();
                        string tmp = f.content + content;
                        f.content = tmp;
                        valid = false;
                    }
                    else
                    {
                        File f = context.currentFile();
                        string tmp = f.content.Replace(Core.CallFunction(this), content);
                        f.content = tmp;
                        valid = false;
                    }
                }
                if (type == "case" && lineCount == 1 && !content.StartsWith("#"))
                {
                    switchcase.cmd = content;
                    valid = false;
                }
                if (type == "switch")
                {
                    File f = context.currentFile();
                    string tmp = switches.Pop().Compile();
                    while (tmp.Contains("\n\n"))
                    {
                        tmp = tmp.Replace("\n\n", "\n");
                    }
                    f.AddLine(tmp);

                    valid = false;
                }
                if (type == "forgenerate")
                {
                    File f = context.currentFile();
                    f.AddLine(content);
                    valid = false;
                }
                if (type == "withContext")
                {
                    File f = context.currentFile();
                    string tmp = f.content;
                    tmp += content;
                    f.content = tmp;
                    valid = false;
                    context.popImpliciteVar();
                }

                if (lineCount >= 65536)
                {
                    GlobalDebug("Warning! maxCommandChainLength exceeded in " + name, Color.Yellow);
                }
                if (type == "staticMethod" && context.currentFile().type != "staticMethod")
                {
                    isInStaticMethod = false;
                }
                if (type == "strucMethod")
                {
                    abstractContent = content;
                    content = "";
                    isInStructMethod = false;
                }
                if (type == "extension")
                {
                    ExtensionClassStack.Pop();
                }

                if (type == "if" || type == "case" || type == "with")
                {
                    bool found = false;
                    if (dedupFiles.ContainsKey(content.GetHashCode()))
                    {
                        var lst = dedupFiles[content.GetHashCode()];

                        foreach (File file in lst)
                        {
                            if (file.content == content && file.valid && !file.notUsed)
                            {
                                if ((type == "if" || type == "with") && lineCount != 1)
                                {
                                    context.currentFile().content =
                                            context.currentFile()
                                            .content
                                            .Replace(Core.CallFunction(this), Core.CallFunction(file));
                                    found = true;
                                    valid = false;
                                    file.multiUsed = true;
                                    break;
                                }
                                if (type == "case" && lineCount != 1)
                                {
                                    switchcase.cmd = switchcase.cmd
                                                                .Replace(Core.CallFunction(this), Core.CallFunction(file));
                                    found = true;
                                    valid = false;
                                    file.multiUsed = true;
                                    break;
                                }
                            }

                        }
                    }
                    if (!found)
                    {
                        if (!dedupFiles.ContainsKey(content.GetHashCode()))
                        {
                            dedupFiles.Add(content.GetHashCode(), new List<File>());
                        }
                        dedupFiles[content.GetHashCode()].Add(this);
                    }
                }
            }

            public void addParsedLine(string a)
            {
                if (a.Contains("\\compiler\\"))
                {
                    a = a.Replace("\\compiler\\", "");
                }
                parsed.Add(a);
            }

            public void addChild(File file)
            {
                if (!(file.function != null && (file.function.isLoading || file.function.isTicking ||
                    file.function.tags.Count > 0 || file.function.isHelper))
                    && !((type == "if" || type == "if_empty") && file.function != null))
                    file.notUsed = notUsed;
                if (file.function == null)
                    file.function = function;
                file.parent = this;

                childs.Add(file);
            }
            public void use()
            {
                if (notUsed && !forcedUnsed)
                {
                    notUsed = false;

                    foreach (File f in childs)
                    {
                        f.use();
                    }
                }
            }

            public void unuse()
            {
                notUsed = true;

                foreach (File f in childs)
                {
                    if (!f.notUsed && !(f.function != null && (f.function.isLoading || f.function.isTicking ||
                        f.function.tags.Count > 0 || f.function.isHelper))
                        && !((type == "if" || type == "if_empty") && f.function != null))
                    {
                        f.unuse();
                    }
                }
            }

            public override string ToString()
            {
                return name;
            }

            public void Compile()
            {
                DateTime startTime = DateTime.Now;

                addChild(context.GoTo(UnparsedFunctionFileContext));

                context.currentFile().notUsed = notUsed;
                structStack = new Stack<Structure>();
                packageMap = new Dictionary<string, string>();
                LastConds = new Stack<If>();
                lazyOutput = new Stack<List<Variable>>();
                lazyCall = new Stack<Function>();
                lazyEvalVar = new List<Dictionary<string, string>>();
                LastCond = new If(-1);
                currentFile = name;
                currentLine = 1;
                inGenericStruct = false;
                isInFunctionDesc = false;
                isInStaticMethod = false;
                isInLazyCompile = 0;
                callingFunctName = function.gameName;

                typeMaps.Push(new Dictionary<string, string>());

                foreach (string key in typeMapContext.Keys)
                {
                    typeMaps.Peek().Add(key, typeMapContext[key]);
                }
                adjPackage.Push(function.package);
                adjPackage.Push(function.structure);
                SetLazyValStack(lazyVarStack);
                
                if (function != null && function.args.Count == 1)
                {
                    switches.Push(new Switch(function.args[0], -1));
                }
                int i = 0;
                try
                {
                    foreach (string line in parsed)
                    {
                        currentLine = i;
                        
                        try
                        {
                            preparseLine(line);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("adj package: " + UnparsedFunctionFileContext + "///" + context.GetVar() + "\n" + e.ToString());
                        }
                        i++;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("adj package: " + UnparsedFunctionFileContext + "///" + context.GetVar() + "\n" + e.ToString()+"\n"+
                            "\t"+parsed.Aggregate((x,y)=>$"{x}\n\t{y}"));
                }
                
                foreach (string line in parsed_end)
                {
                    currentLine = i;
                    try
                    {
                        preparseLine(line);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("adj package: " + UnparsedFunctionFileContext + "///" + context.GetVar() + "\n" + e.ToString());
                    }
                    i++;
                }
                

                if (function != null && function.args.Count == 1)
                {
                    Switch s = switches.Pop();
                }

                typeMaps.Pop();
                context.currentFile().Close();
                adjPackage.Pop();
                adjPackage.Pop();
                UnparsedFunctionFile = false;

                if (compilerSetting.advanced_debug)
                    GlobalDebug($">> Compiled {name} in {((DateTime.Now - startTime).TotalMilliseconds)}ms", Color.Yellow);
            }
        }
        public class Context
        {
            public List<Dictionary<string, string>> compVal = new List<Dictionary<string, string>>();
            public List<string> directories = new List<string>();
            public List<File> files = new List<File>();
            public Dictionary<string, File> fileDir = new Dictionary<string, File>();
            public List<string> import = new List<string>();
            public string fakeContext;
            public List<List<ImpliciteVar>> impliciteVars = new List<List<ImpliciteVar>>();
            private string[] funcSplitter = Core.FileNameSplitter();
            public Context(string project, File f)
            {
                directories.Add(project);
                files.Add(f);

                fileDir[GetFile()] = f;
                compVal.Add(new Dictionary<string, string>());
            }
            public Context(string project, string file, File f)
            {
                directories.Add(project);
                File root = loadFile;
                files.Add(root);
                fileDir[GetFile()] = root;

                directories.Add(file);
                files.Add(f);

                fileDir[GetFile()] = f;
                compVal.Add(new Dictionary<string, string>());
            }

            public void Sub(string file, File f)
            {
                File cur = null;
                cur = currentFile();
                directories.Add(file);
                files.Add(f);
                cur.addChild(f);
                if (!structInstCompVar)
                {
                    context.compVal.Add(new Dictionary<string, string>());
                }
            }
            public void Parent()
            {
                if (directories.Count > 1)
                {
                    directories.RemoveAt(directories.Count - 1);
                    files.RemoveAt(files.Count - 1);
                    if (context.compVal.Count > 0 && !structInstCompVar)
                    {
                        context.compVal.RemoveAt(context.compVal.Count - 1);
                    }
                }
            }
            public void GoRoot()
            {
                while (directories.Count > 1)
                {
                    directories.RemoveAt(directories.Count - 1);
                    files.RemoveAt(files.Count - 1);
                }
                if (files.Count == 0)
                {
                    files.Add(new File(".", "", ""));
                }
            }
            public File GoTo(string path, bool strict = false)
            {
                GoRoot();
                File fOut = null;
                if (!path.Contains("."))
                {
                    File f = new File(path, "", "");
                    Compiler.files.Add(f);
                    Sub(path, f);
                    return f;
                }
                else
                {
                    if (strict)
                    {
                        directories[0] = path.Substring(0, path.IndexOf('.'));
                    }
                    foreach (string p in path.Substring(path.IndexOf('.'), path.LastIndexOf('.') - path.IndexOf('.')).Split('.'))
                    {
                        if (p != "")
                        {
                            string fullName = context.GetFun() + p;
                            string c = Core.FileNameSplitter()[0];
                            string subName = fullName.Substring(fullName.IndexOf(c) + 1, fullName.Length - fullName.IndexOf(c) - 1);
                            File f = new File(subName, "", "");
                            Compiler.files.Add(f);
                            Sub(p, f);
                            fOut = f;
                        }
                    }
                    return fOut;
                }
            }
            public File Package(string package)
            {
                while (directories.Count > 1)
                {
                    directories.RemoveAt(directories.Count - 1);
                }
                foreach (string s in package.Split('.')) if (s != "") directories.Add(s);
                string file = GetFile();
                file = file.Substring(0, file.Length - 1);

                return null;
            }

            public string GetFile()
            {
                string output = "";

                for (int i = 1; i < directories.Count; i++)
                {
                    output += directories[i].ToLower() + "/";
                }

                return output;
            }

            public string GetFun()
            {
                string output = directories[0].ToLower() + funcSplitter[0];

                for (int i = 1; i < directories.Count; i++)
                {
                    output += directories[i].ToLower() + funcSplitter[1];
                }

                return output;
            }

            public string getRoot()
            {
                return directories[0].ToLower();
            }

            public string GetVar()
            {
                if (fakeContext != null && fakeContext != "")
                    return fakeContext;
                string output = directories[0] + ".";

                for (int i = 1; i < directories.Count; i++)
                {
                    output += directories[i] + ".";
                }

                return output;
            }

            public string GetInput()
            {
                return GetVar();
            }

            public File currentFile()
            {
                return files[files.Count - 1];
            }
            public string toInternal(string value)
            {
                string[] v = smartEmpty(value).Split('.');

                StringBuilder stringBuilder = new StringBuilder(512);
                for (int i = 0; i < v.Length - 1; i++)
                {
                    if (packageMap.ContainsKey(v[i]))
                    {
                        stringBuilder.Append(packageMap[v[i]]);
                        stringBuilder.Append(".");
                    }
                    else
                    {
                        stringBuilder.Append(v[i]);
                        stringBuilder.Append(".");
                    }
                }
                stringBuilder.Append(v[v.Length - 1]);
                return FunctionNameSimple(stringBuilder.ToString());
            }

            public string GetFunctionName(string func, bool safe = false, bool bottleneck = false)
            {
                func = toInternal(smartEmpty(func).ToLower());
                
                if (func.StartsWith("."))
                {
                    func = func.Substring(1, func.Length - 1);
                    if (functions.ContainsKey(func))
                    {
                        return func;
                    }
                    else if (safe)
                    {
                        return null;
                    }
                    else
                    {
                        throw new Exception($"No Such Function as .{func}");
                    }
                }
                string[] splited = func.Split('.');
                if (containLazyVal(splited[0]) && splited.Length == 2)
                {
                    var varfunc = GetVariable(getLazyVal(splited[0]), safe, bottleneck) + '.' + splited[1];
                    if (functions.ContainsKey(varfunc))
                    {
                        return varfunc;
                    }
                }
                if (functions.ContainsKey(func))
                {
                    return func;
                }
                string dir = "";
                string output = null;



                foreach (string co in directories)
                {
                    dir += co.ToLower() + ".";
                    if (functions.ContainsKey(dir + func))
                    {
                        output = dir + func;
                    }
                }
                if (output != null)
                    return output;
                
                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string dir1 = "";
                        if (pack != null)
                        {
                            foreach (string co in pack.Split('.').Where(x => x != ""))
                            {
                                dir1 += co + ".";
                                dir = "";
                                if (functions.ContainsKey((dir1 + func).ToLower()))
                                {
                                    output = (dir1 + func).ToLower();
                                    return output;
                                }
                                foreach (string co2 in directories)
                                {
                                    dir += co2 + ".";
                                    if (functions.ContainsKey((dir + dir1 + func).ToLower()))
                                    {
                                        output = (dir + dir1 + func).ToLower();
                                        return output;
                                    }
                                }
                            }
                        }
                    }
                }

                if (safe)
                    return null;
                throw new Exception($"UNKNOW FUNCTION ({dir}/{func}) or in {((adjPackage!=null&&adjPackage.Count > 0)?adjPackage?.Aggregate((x,y)=>x+","+y):"")}");
            }

            public bool IsFunction(string func, bool bottleneck = false)
            {
                if (!bottleneck)
                    func = toInternal(smartEmpty(func).ToLower());

                if (func.StartsWith("."))
                {
                    func = smartExtract(func.Substring(1, func.Length - 1));
                    if (functions.ContainsKey(func))
                    {
                        return true;
                    }
                    return false;
                }

                string var = GetVariable(func, true);
                if (var != null && GetVariableByName(var, true) != null)
                {
                    if (Compiler.GetVariable(var).type == Type.FUNCTION)
                    {
                        return true;
                    }
                }

                if (functions.ContainsKey(func))
                {
                    return true;
                }


                string dir = "";
                foreach (string co in directories)
                {
                    dir += co.ToLower() + ".";

                    if (functions.ContainsKey(dir + func))
                    {
                        return true;
                    }
                }
                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string dir1 = "";
                        if (pack != null)
                        {
                            foreach (string co in pack.Split('.').Where(x => x != ""))
                            {
                                dir1 += co + ".";
                                dir = "";
                                if (functions.ContainsKey((dir1 + func).ToLower()))
                                {
                                    return true;
                                }
                                foreach (string co2 in directories)
                                {
                                    dir += co2 + ".";
                                    if (functions.ContainsKey((dir + dir1 + func).ToLower()))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }

            public string GetVariable(string func, bool safe = false, bool bottleneck = false, int recCall = 0, bool debug = false)
            {
                if (func.StartsWith("."))
                {
                    func = smartExtract(func.Substring(1, func.Length - 1));
                    if (variables.ContainsKey(func))
                    {
                        return func;
                    }
                    else if (safe)
                    {
                        return null;
                    }
                    else
                    {
                        throw new Exception($"No Such Variables as .{func}");
                    }
                }
                if (func == "_" && switches.Count > 0)
                {
                    return switches.Peek().variable.gameName;
                }
                if (recCall == 0)
                    func = toInternal(func.Replace(" ", ""));

                if (recCall > maxRecCall)
                {
                    throw new Exception("Stack Overflow");
                }
                if (containLazyVal(func))
                {
                    return getLazyVal(func);
                }

                if (variables.ContainsKey(func))
                {
                    return func;
                }

                string adj = "";


                string dir = "";
                string output = null;
                foreach (string co in directories)
                {
                    dir += co + ".";

                    if (containLazyVal(dir + func))
                    {
                        return getLazyVal(dir + func);
                    }
                    if (variables.ContainsKey(dir + func))
                    {
                        output = dir + func;
                    }
                }

                if (output != null)
                    return output;

                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string dir1 = "";
                        if (pack != null)
                        {
                            foreach (string co in pack.Split('.').Where(x => x != ""))
                            {
                                dir1 += co + ".";
                                dir = "";
                                if (containLazyVal(dir1 + func))
                                { 
                                    return getLazyVal(dir1 + func);
                                }
                                if (variables.ContainsKey(dir1 + func))
                                {
                                    output = dir1 + func;
                                    return output;
                                }
                                foreach (string co2 in directories)
                                {
                                    dir += co2 + ".";

                                    if (containLazyVal(dir + dir1 + func))
                                    {
                                        return getLazyVal(dir + dir1 + func);
                                    }
                                    if (variables.ContainsKey(dir + dir1 + func))
                                    {
                                        output = dir + dir1 + func;
                                        return output;
                                    }
                                }
                            }

                            adj += pack + ", ";
                        }
                    }
                }
                if (!safe)
                    throw new Exception("UNKNOW Variable (" + dir + "/" + func + ") with package: " + adj);
                else
                    return null;
            }

            public string GetBlockTags(string func, bool safe = false, bool bottleneck = false, int recCall = 0, bool debug = false)
            {
                return GetTags(blockTags, func, safe, bottleneck, recCall, debug);
            }
            public string GetEntityTags(string func, bool safe = false, bool bottleneck = false, int recCall = 0, bool debug = false)
            {
                return GetTags(entityTags, func, safe, bottleneck, recCall, debug);
            }
            public string GetItemTags(string func, bool safe = false, bool bottleneck = false, int recCall = 0, bool debug = false)
            {
                return GetTags(itemTags, func, safe, bottleneck, recCall, debug);
            }
            public string GetTags(Dictionary<string, TagsList> dic, string func, bool safe = false, bool bottleneck = false, int recCall = 0, bool debug = false)
            {
                if (func.StartsWith("."))
                {
                    func = smartExtract(func.Substring(1, func.Length - 1));
                    if (dic.ContainsKey(func))
                    {
                        return func;
                    }
                    else if (safe)
                    {
                        return null;
                    }
                    else
                    {
                        throw new Exception($"No Such Tags as .{func}");
                    }
                }
                if (recCall == 0)
                    func = toInternal(func.Replace(" ", ""));

                if (recCall > maxRecCall)
                {
                    throw new Exception("Stack Overflow");
                }

                if (dic.ContainsKey(func))
                {
                    return func;
                }

                string adj = "";


                string dir = "";
                string output = null;
                foreach (string co in directories)
                {
                    dir += co + ".";

                    if (dic.ContainsKey(dir + func))
                    {
                        output = dir + func;
                    }
                }

                if (output != null)
                    return output;

                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string dir1 = "";
                        if (pack != null)
                        {
                            foreach (string co in pack.Split('.').Where(x => x != ""))
                            {
                                dir1 += co + ".";
                                dir = "";
                                if (dic.ContainsKey(dir1 + func))
                                {
                                    output = dir1 + func;
                                    return output;
                                }
                                foreach (string co2 in directories)
                                {
                                    dir += co2 + ".";
                                    if (dic.ContainsKey(dir + dir1 + func))
                                    {
                                        output = dir + dir1 + func;
                                        return output;
                                    }
                                }
                            }
                            adj += pack + ", ";
                        }
                    }
                }
                if (!safe)
                    throw new Exception("UNKNOW Tags (" + dir + "/" + func + ") with package: " + adj);
                else
                    return null;
            }

            public string GetPredicate(string func, bool safe = false, bool bottleneck = false)
            {
                if (func.Contains("("))
                {
                    func = smartEmpty(func.Substring(0, func.IndexOf("(")));
                }
                func = toInternal(func.Replace(" ", "")).ToLower();
                if (func.StartsWith("."))
                {
                    func = smartExtract(func.Substring(1, func.Length - 1));
                    if (predicates.ContainsKey(func))
                    {
                        return func;
                    }
                    else if (safe)
                    {
                        return null;
                    }
                    else
                    {
                        throw new Exception($"No Such Predicate as .{func}");
                    }
                }

                if (func.StartsWith("this.") && predicates.ContainsKey(func.Replace("this.", thisDef.Peek())))
                {
                    return func.Replace("this.", thisDef.Peek());
                }

                if (variables.ContainsKey(func))
                {
                    return func;
                }
                string dir = "";
                string output = null;
                foreach (string co in directories)
                {
                    dir += co.ToLower() + ".";
                    if (predicates.ContainsKey(dir + func))
                    {
                        output = dir + func;
                    }
                }

                if (output != null)
                    return output;

                string adj = "";
                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string var = GetPredicate(pack + "." + func, true, true);
                        if (var != null)
                        {
                            return var;
                        }
                        adj += pack + ", ";
                    }
                }

                if (!safe)
                    throw new Exception("UNKNOW Predicate (" + dir + "/" + func + ") with package: " + adj);
                else
                    return null;
            }

            public string GetEntityName(string value)
            {
                if (value.Contains("@"))
                {
                    if (!Core.isValidSelector(value.Split('.')[0]))
                        throw new Exception("Invalid Selctor " + value.Split('.')[0]);
                    value = ReplaceEntityTags(value);
                    return smartSplit(value, '.')[0];
                }
                if (value.Contains('.'))
                {
                    string ent = value.Split('.')[0];
                    return (GetVariableByName(ent)).scoreboard().Split(' ')[0];
                }
                else
                    throw new Exception("UNKNOW Entity (" + value + ")");
            }
            public string ReplaceEntityTags(string value)
            {
                var m = entityTagsRpReg.Match(value);
                if (m.Success && !m.Value.Contains(":"))
                {
                    string tag = GetEntityTags(m.Value.Split('#')[1]);
                    value = regReplace(value, m, "type=#" + Core.FormatTagsPath(tag));
                }
                return value;
            }

            public string ConvertEntity(string value, bool single = true)
            {
                value = ReplaceEntityTags(value);
                if (single)
                {
                    string t = context.GetVariable(value.Split('.')[0]);
                    if (t.Contains("@"))
                    {
                        if (!Core.isValidSelector(value))
                            throw new Exception("Invalid Selctor " + value);
                        return t;
                    }
                    else
                    {
                        return "@e[tag=" + t + "]"
                        + value.Substring(value.IndexOf('.'), value.Length - value.IndexOf('.'));
                    }
                }
                else
                {
                    string t = context.GetVariable(value.Split('.')[0]);
                    if (t.Contains("@"))
                    {
                        if (!Core.isValidSelector(value))
                            throw new Exception("Invalid Selctor " + value);
                        return t;
                    }
                    else
                    {
                        return "@e[limit=1,tag=" + t + "]"
                        + value.Substring(value.IndexOf('.'), value.Length - value.IndexOf('.'));
                    }

                }
            }

            public string GetEntitySelector(string value, bool single = true)
            {
                value = ReplaceEntityTags(value);
                if (value.Contains("@"))
                {
                    if (!Core.isValidSelector(value))
                        throw new Exception("Invalid Selctor " + value);

                    return smartEmpty(value);
                }
                else if (single)
                    return "@e[tag=" + GetVariableByName(value.Split('.')[0]).uuid + "]";
                else
                    return "@e[limit=1,tag=" + GetVariableByName(value.Split('.')[0]).uuid + "]";

            }
            public Type GetVarType(string value)
            {
                return Compiler.GetVariable(GetVariable(value)).type;
            }
            public bool isEntity(string value)
            {
                if (entityReg.Match(smartEmpty(value)).Success)
                {
                    return true;
                }
                else if (value.Contains('.'))
                {
                    string ent = value.Split('.')[0];

                    return (GetVariable(ent, true) != null && (Compiler.GetVariable(GetVariable(ent, true)).type == Type.ENTITY));
                }
                else
                    return false;
            }

            public string Last()
            {
                return directories[directories.Count - 1];
            }

            public void addImpliciteVar(List<ImpliciteVar> var)
            {
                impliciteVars.Add(var);
            }
            public void popImpliciteVar()
            {
                impliciteVars.RemoveAt(impliciteVars.Count - 1);
            }
            public string getImpliciteVar(string enums, Type type)
            {
                string value = null;
                foreach (var lst in impliciteVars)
                {
                    foreach (ImpliciteVar var in lst)
                    {
                        if (var.type == type && var.enums == enums)
                            value = var.value;
                    }
                }

                return value;
            }
            public string getImpliciteVar(Variable trg)
            {
                string value = null;
                foreach (var lst in impliciteVars)
                {
                    foreach (ImpliciteVar var in lst)
                    {
                        if (var.type == trg.type && var.enums == trg.enums)
                            value = var.value;
                    }
                }

                return value;
            }
        }
    }
}
