using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSharp
{
    public class Compiler
    {
        public static Dictionary<string, List<Function>> functions;
        public static HashSet<Function> abstractFunctionsNeeded;
        public static Dictionary<string, Scoreboard> scoreboards;
        public static Dictionary<string, Variable> variables;
        private static Dictionary<int, Variable> constants;
        public static Dictionary<string, Enum> enums;
        public static Dictionary<string, Structure> structs;
        public static Dictionary<string, TagsList> blockTags;
        public static Dictionary<string, TagsList> entityTags;
        public static Dictionary<string, TagsList> itemTags;
        public static Dictionary<string, List<Predicate>> predicates;
        public static Dictionary<string, List<string>> functionTags;


        public static Dictionary<string, string> offuscationMap;
        private static Dictionary<string, string> packageMap;
        private static List<Dictionary<string, string>> lazyEvalVar;
        private static List<Dictionary<string,string>> compVal;
        private static Dictionary<string, List<Function>> functDelegated;
        private static Dictionary<string, List<File>> functDelegatedFile;
        private static Dictionary<string, string> resourceFiles;
        private static Dictionary<string, Regex> compRegexCache = new Dictionary<string, Regex>();
        public static List<string> packages;
        private static HashSet<string> offuscationSet;
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
        private static CompilerCore Core;
        private static ProjectVersion projectVersion;
        private static int jsonIndent = 0;

        private static List<string> funcDef;
        private static string callTrace = "digraph G {\nmain\nload\nhelper\n";
        private static string callingFunctName = "loading";
        private static bool structInstCompVar;
        private static Dictionary<string, string> structCompVarPointer;
        
        #region Regexs
        private static Regex funcReg = new Regex(@"^(@?[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])*\s+)+[\w\.]+\s*\(.*\)");
        private static Regex nullReg = new Regex(@"\s*null\s*");
        private static Regex outterReg = new Regex(@"(?s)\[.*\]");
        private static Regex getReg = new Regex(@"\w*\s*=[ a-z\.A-Z0-9]*\[.*\]");
        private static Regex oppReg = new Regex(@"[a-zA-Z0-9\._]+\[.*\]\s*[+\-/\*%]=");
        private static Regex setReg = new Regex(@"\[.*\]\s*=\s*.*");
        private static Regex enumsDesugarReg = new Regex(@"(?s)(enum\s+\w+\s*(\([a-zA-Z0-9\- ,_=:/\\\.""'!\[\]]*\))?\s*\{(\s*\w*(\([a-zA-Z0-9/\\\- ,_=:\.""'!:\[\]\(\)]*\))?,?\s*)*\}|enum\s+\w+\s*=\s*(\([a-zA-Z0-9/\\\- ,_=""'\[\]!:\(\)]*\))?\s*\{(\s*\w*(\([a-zA-Z0-9/\\\- ,_=:\.""'\[\]!\(\)]*\))?,?\s*)*\})");
        private static Regex blocktagsDesugarReg = new Regex(@"(?s)(blocktags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|blocktags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex entitytagsDesugarReg = new Regex(@"(?s)(entitytags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|entitytags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex itemtagsDesugarReg = new Regex(@"(?s)(itemtags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|itemtags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex entitytagsReplaceReg = new Regex(@"type=#\w+");
        private static Regex entitytagsReplaceReg2 = new Regex(@"type=!#\w+");
        private static Regex ifsDesugarReg = new Regex(@"(?s)^(if\s*\(.*\)\{.*\}\s*else)|(if\s*\(.*\).*\s*else)");
        private static Regex funArgTypeReg = new Regex(@"^([@\w\.]*\s*(<\(?\w*\)?,?\(?\w*\)?>)?(\[\d+\])?)*\(");
        private static Regex arraySizeReg = new Regex(@"(?:\[)\d+(?:\])");
        private static Regex arrayTypeReg = new Regex(@"\b\w+(?:\[)");
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
        private static Regex varInstReg = new Regex(@"^[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])?\s+[\w\$\.]+\s*");
        private static Regex compVarInstReg = new Regex(@"^[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])?\s+\$[\w\$\.]+\s*=");
        private static Regex elseReg = new Regex(@"^else\s*");
        private static Regex regEval = new Regex(@"\$eval\([0-9a-zA-Z\-\+\*/% \.]*\)");
        private static Regex regEval2 = new Regex(@"\$eval\([0-9a-zA-Z\-\+\*/% \.\(\)\s]*\)eval\$");
        private static Regex forgenInLineReg = new Regex(@"forgenerate\([^\(\)]*\)\{[^\{\}]*\}");
        private static Regex dualCompVar = new Regex(@"^\$[\w\.\$]+\s*=\s*\$?[\w\.\$]+\s*");
        private static Regex requireReg = new Regex(@"^require\s+\$?\w+\s+[\w\=\<\>]+");
        private static Regex indexedReg = new Regex(@"^indexed\s+\$?\w+\s+\w+");
        private static Regex functionTypeReg = new Regex(@"^(\([\w\,\s=>]+\)|\w+)\s*=>\s*(\([\w\,\s=>]+\)|\w+)");
        private static Regex functionTypeRegRelaxed = new Regex(@"(\([\w\,\s=>]+\)|\w+)\s*=>\s*(\([\w\,\s=>]+\)|\w+)");
        private static Regex thisReg = new Regex("\\bthis\\.");
        private static Regex thisReg2 = new Regex("\\$this\\.");
        private static Regex curriedReg = new Regex(@"([\w\.]*\(.*\)){2,100}");

        private static string ConditionAlwayTrue = "=$=TRUE=$=";
        private static string ConditionAlwayFalse = "=$=False=$=";
        #endregion

        private static int isInLazyCompile;
        private static CompilerSetting compilerSetting;
        private static int maxRecCall = 300;
        private static bool muxAdding;
        private static bool callStackDisplay;
        private static File currentParsedFile;

        private Compiler() { }

        public static List<File> compile(CompilerCore core,string project, List<File> codes, List<File> resources, Debug debug,
                                            CompilerSetting setting, ProjectVersion version, string pctFolder)
        {
            DateTime startTime = DateTime.Now;

            callTrace = "digraph "+project+" {\nmain\nload\nhelper\n";
            Core = core;
            projectVersion = version;
            muxAdding = false;
            for (int i = 0; i < 11; i++)
            {
                pow64[i] = IntPow(alphabet.Length, i);
            }
            foreach(File f in resources)
            {
                f.content = f.content.Replace("\r", "");
            }
            foreach (File f in codes)
            {
                f.content = f.content.Replace("\r", "");
            }
            dirVar = project.Substring(0, Math.Min(4, project.Length));

            compilerSetting = setting;

            
            offuscationMap = new Dictionary<string, string>();

            foreach(string key in setting.forcedOffuscation.Keys)
            {
                offuscationMapAdd(key, setting.forcedOffuscation[key]);
            }

            functionTags = new Dictionary<string, List<string>>();
            projectFolder = pctFolder;
            GlobalDebug = debug;
            Project = project;
            funcDef = new List<string>();

            try
            {
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
                functDelegated = new Dictionary<string, List<Function>>();
                functDelegatedFile = new Dictionary<string, List<File>>();
                offuscationSet = new HashSet<string>();
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
                compVal = new List<Dictionary<string, string>>();
                ExtensionClassStack = new Stack<string>();
                ExtensionMethod = new Dictionary<string, List<Function>>();

                foreach (var f in resources)
                {
                    resourceFiles.Add(f.name, f.content);
                    GlobalDebug("Added resource: "+f.name, Color.Green);
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

                loadFile.Close();
                mainFile.Close();

                updateFormater();

                List<File> returnFiles = new List<File>();
                int lineCount=0;
                
                foreach(File f in files)
                {
                    if (!compilerSetting.removeUselessFile)
                    {
                        returnFiles.Add(f);
                    }
                    else if (!f.notUsed && !f.isLazy && f.lineCount > 0)
                    {
                        returnFiles.Add(f);
                        foreach(string line in f.content.Split('\n'))
                        {
                            if (line != "" && !line.StartsWith("#"))
                            {
                                lineCount++;
                            }
                        }
                    }
                }

                GlobalDebug("================[Datapack Stats]================", Color.LimeGreen);
                GlobalDebug("\t" + returnFiles.Count.ToString() + " MCfunction Files", Color.LimeGreen);
                GlobalDebug("\t" + lineCount.ToString()       + " MC Commands", Color.LimeGreen);
                GlobalDebug("\t" + jsonFiles.Count.ToString() + " Json files", Color.LimeGreen);
                GlobalDebug("\t" + variables.Count.ToString() + " TBMS variables", Color.LimeGreen);
                GlobalDebug("\t" + functions.Count.ToString() + " TBMS functions", Color.LimeGreen);
                GlobalDebug("================================================", Color.LimeGreen);

                foreach (File item in jsonFiles)
                {
                    if (!item.notUsed)
                    {
                        returnFiles.Add(item);
                    }
                }
                
                callTrace += "}";

                GlobalDebug("Total Compile Time: " + (DateTime.Now - startTime).TotalMilliseconds.ToString()+"ms",Color.Aqua);
                
                return returnFiles;
            }
            catch (Exception e)
            {
                GlobalDebug("Dumped Core",Color.Yellow);
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
            foreach(File f in files)
            {
                tasks.Add(Task<string[]>.Factory.StartNew(() => desugar(f.content).Split('\n')));
            }
            for (int i = 0; i < files.Count; i++)
            {
                currentParsedFile = files[i];
                compileFile(files[i], tasks[i].Result, notUsed, import);
            }
            currentParsedFile = null;
        }
        private static void compileFile(File f, string[] desugaredContent, bool notUsed = false, bool import = false)
        {
            structStack = new Stack<Structure>();
            packageMap = new Dictionary<string, string>();
            LastConds = new Stack<If>();
            lazyOutput = new Stack<List<Variable>>();
            lazyCall = new Stack<Function>();
            lazyEvalVar = new List<Dictionary<string, string>>();
            compVal = new List<Dictionary<string,string>>();
            typeMaps = new Stack<Dictionary<string, string>>();
            LastCond = new If(-1);
            currentFile = f.name;
            currentLine = 1;
            inGenericStruct = false;
            isInFunctionDesc = false;
            isInStaticMethod = false;
            isInLazyCompile = 0;
            jsonIndent = 0;

            compVal.Add(new Dictionary<string, string>());

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
                preparseLine("def " + f.name.Substring(0, f.name.IndexOf('.'))+"(){");
            }

            foreach (string line2 in lines)
            {
                if (!f.resourcespack)
                {
                    preparseLine((isMcFunction ? "/" : "") + line2);
                }
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
            GlobalDebug("Succefully Compiled " + currentFile + " (" + (currentLine - 1).ToString() + " Lines)", Color.Lime);
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
                Formatter.setEnumValue(formEnums);
                Formatter.setStructs(new List<string>(structs.Keys));
                Formatter.setpackage(packages);
                Formatter.setTags(functionTags.Keys.ToList());
                Formatter.setDefWord(funcDef);
                Formatter.loadDict();
            }
            catch { }
        }

        public static void preparseLine(string line2, File limit = null, bool lazyEval = false)
        {
            string line = line2;

            if (compVal.Count > 0 && line.Contains("$") && !(dualCompVar.Match(line).Success && structInstCompVar)) {
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
                                res += parseLine(subline)+"\n";
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
                else if (text.StartsWith("//") && jsonIndent == 0)
                    return "#" + text.Substring(2, text.Length - 2);

                else if (text.StartsWith("/") && jsonIndent == 0)
                    return text.Substring(1, text.Length - 1);
                else if (text.StartsWith("import") && jsonIndent == 0)
                {
                    return import(text);
                }
                else if (text.StartsWith("using") && jsonIndent == 0)
                {
                    return instUsing(text);
                }
                else if (text.StartsWith("alias") && jsonIndent == 0)
                {
                    return InstAlias(text);
                }
                else if (text.StartsWith("package") && jsonIndent == 0)
                {
                    return InstPackage(text);
                }
                else if (CommandParser.canBeParse(text) && jsonIndent == 0)
                {
                    return CommandParser.parse(text, context);
                }
                else if (text.Contains("\"\"\"") && jsonIndent == 0)
                {
                    return InstFuncDesc(text);
                }
                else if (text.StartsWith("{") && jsonIndent == 0)
                {
                    autoIndented = 0;
                    return parseLine(text.Substring(1, text.Length - 1));
                }
                else if (requireReg.Match(text).Success)
                {
                    return Require(text);
                }
                else if (indexedReg.Match(text).Success)
                {
                    return Indexed(text);
                }
                //return
                else if ((text.StartsWith("return") && text.Contains("(") && text.Contains(")")) || text.StartsWith("return "))
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
                else if (jsonFileReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstJsonFile(text);
                }
                else if (predicateFileReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstPredicateFile(text);
                }
                //condition
                else if (ifsReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstIf(arg, text, 1);
                }
                else if (ifReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstIf(arg, text);
                }
                else if (elsifReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstElseIf(arg, text);
                }
                else if (elseReg.Match(text).Success)
                {
                    return InstElse(text);
                }
                //structure
                else if (text.StartsWith("struct ") && jsonIndent == 0)
                {
                    return InstStruct(text, false);
                }
                //class
                else if (text.StartsWith("class ") && jsonIndent == 0)
                {
                    return InstStruct(text, true);
                }
                //class
                else if (text.StartsWith("extension ") && jsonIndent == 0)
                {
                    return InstExtension(text);
                }
                //switch
                else if (switchReg.Match(text).Success && jsonIndent == 0)
                {
                    string[] arg = getArgs(text);

                    return InstSwitch(arg, text);
                }
                //case
                else if (caseReg.Match(text).Success && jsonIndent == 0)
                {
                    string arg = getArg(text);

                    return InstCase(arg);
                }
                //smart case
                else if (text.Contains("->") && switches.Count > 0 && jsonIndent == 0)
                {
                    return InstSmartCase(text);
                }
                //with
                else if (withReg.Match(text).Success && jsonIndent == 0)
                {
                    string[] args = getArgs(text);

                    return InstWith(args, text);
                }
                //at
                else if (atReg.Match(text).Success && jsonIndent == 0)
                {
                    string[] args = getArgs(text);

                    return InstAt(args, text);
                }
                //positioned
                else if (positonedReg.Match(text).Success && jsonIndent == 0)
                {
                    string args = getArg(text);

                    return InstPositioned(args, text);
                }
                //aligned
                else if (alignReg.Match(text).Success && jsonIndent == 0)
                {
                    string args = getArg(text);

                    return InstAligned(args, text);
                }
                //while
                else if (whileReg.Match(text).Success && jsonIndent == 0)
                {
                    string arg = getArg(text);

                    return InstWhile(arg, text);
                }
                //forgenerate
                else if (forgenerateReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return InstForGenerate(arg, text);
                }
                //for
                else if (forReg.Match(text).Success && jsonIndent == 0)
                {
                    string arg = getArg(text);

                    return InstFor(arg, text);
                }
                //enum set
                else if (enumReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstEnum(text);
                }
                //enum file
                else if (enumFileReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstEnumFile(text);
                }
                //function def
                else if (funcReg.Matches(text).Count > 0 && jsonIndent == 0)
                {
                    return InstFunc(text);
                }
                //blocktag set
                else if (blocktagReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstBlockTag(text);
                }
                //entitytag set
                else if (entitytagReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstEntityTag(text);
                }
                //itemTag set
                else if (itemtagReg.Match(text).Success && jsonIndent == 0)
                {
                    return InstItemTag(text);
                }
                //comp int set
                else if (compVarInstReg.Match(text).Success || dualCompVar.Match(text).Success)
                {
                    return InstCompilerVar(text);
                }
                //int set
                else if ((varInstReg.Match(text).Success || functionTypeReg.Match(text).Success) && jsonIndent == 0)
                {
                    return InstVar(text);
                }
                //int add
                else if (smartContains(text, '=') && jsonIndent == 0)
                {
                    return modVar(text);
                }
                //int add
                else if ((text.Contains("++") || text.Contains("--")) && jsonIndent == 0)
                {
                    return modVar(text.Replace("++", "+=1").Replace("--", "-=1"));
                }
                //function call
                else if (text.Contains("(") && text.Contains(")") || context.IsFunction(text))
                {
                    return functionEval(text);
                }
                else if (jsonIndent > 0)
                {
                    return AddToJsonFile(text);
                }
                else
                {
                    if (text != "" && !text.StartsWith("}"))
                    {
                        GlobalDebug("Unparsed line:'" + text + "'", Color.Yellow);
                    }
                    return "";
                }
            }
            catch(Exception e)
            {
                if (!inGenericStruct)
                    throw new Exception(text + "\n" + e.ToString());
                else
                    return "";
            }
        }
        public static string regReplace(string text, Match match, string value)
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
                foreach (Dictionary<string, string> v in compVal)
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
                        reg = new Regex(@"\b" + key + "\b");
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

            foreach (Dictionary<string, string> v in compVal)
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
        public static string desugar(string text)
        {
            text = desugarParenthis(text).Replace("$projectName", Project.ToLower()).Replace("$projectVersion", projectVersion.ToString());
            Match match;
            text = ifelseDetect(text);
            
            match = oppReg.Match(text);
            while (match != null && match.Value != "")
            {
                string op = match.Value[match.Value.Length - 2]+"";

                text = regReplace(text, match, match.Value.Replace(op + "=", "= " + match.Value.Substring(0, match.Value.Length - 2) + op));
                match = oppReg.Match(text);
            }

            match = getReg.Match(text);
            while (match != null && match.Value != "")
            {
                string v = outterReg.Match(match.Value).Value;
                
                text = regReplace(text, match, 
                    regReplace(match.Value, outterReg.Match(match.Value),
                    ".get("+v.Substring(1, v.Length-2)+")"));

                match = getReg.Match(text);
            }
            
            match = setReg.Match(text);
            while (match != null && match.Value != "")
            {
                string v = outterReg.Match(match.Value).Value;
                
                text = regReplace(text, match,
                    ".set(" + v.Substring(1, v.Length - 2)+","+ smartSplit(match.Value, '=',1)[1] + ")");

                match = setReg.Match(text);
            }
            
            match = enumsDesugarReg.Match(text);
            while (match != null && match.Value != "")
            {
                text = regReplace(text, match,
                    match.Value.Replace("{","=").Replace("\n","").Replace("}",""));

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
                else if (text[i] == '\n' && parInd > 0)
                {
                    if (ind > 0) {
                        bool funky = isFunkyLamba2.Peek();
                        foreach(bool b in isFunkyLamba)
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
                else if (text[i] == ' ' || text[i] == '\t' || text[i] == '\n')
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
                string[] part = smartSplit(m.Value.Replace("=>", "§"),'§',1);
                part[0] = smartExtract(part[0]).Replace("§","=>");
                part[1] = smartExtract(part[1]).Replace("§", "=>");

                if (!part[0].StartsWith("(")) { part[0] = "(" + part[0] + ")"; }
                if (!part[1].StartsWith("(")) { part[1] = "(" + part[1] + ")"; }
                text = regReplace(text, m, (entity?"FUNCTION<": "function<") + part[0].ToLower() + "," + part[1].ToLower() + ">");
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

            foreach(char c in text)
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
            for (int j  = ifsIndex.Count - 1; j >= 0; j--)
            {
                text = text.Insert(ifsIndex[j], "s");
            }
 
            return text;
        }

        public static string import(string text)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";

            text = text.Replace("import", "").Replace(" ", "").Replace(".","/");
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
            if (!tryImport(path+"lib/", text, fu, out output))
            {
                if (!tryImport(path+"lib/" + Core.getLibraryFolder() + "/", text, fu, out output))
                {
                    if (!tryImport(projectFolder + "/", text, fu, out output))
                    {
                        throw new Exception("Unknown library: " + text);
                    }
                }
            }
            //context.currentFile().AddStartLine(output);
            return "";
        }
        public static bool tryImport(string folder, string text, bool fu, out string msg)
        {
            if (System.IO.File.Exists(folder + text + ".tbms"))
            {
                forcedUnsed = fu;
                string data = System.IO.File.ReadAllText(folder + text + ".tbms");
                ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);
                msg = "#Using TBMS Library: " + text + " v." + project.version.ToString()+"\n";

                context.GoRoot();
                if (project.resources != null)
                {
                    foreach (var f in project.resources)
                    {
                        if (!resourceFiles.ContainsKey(f.name))
                            resourceFiles.Add(f.name, f.content);
                    }
                }

                List<File> nFiles = new List<File>();
                foreach (var file in project.files)
                {
                    nFiles.Add(new File(text + "." + file.name, file.content));
                }
                compileFiles(nFiles, fu, true);

                imported.Add(text);
                forcedUnsed = false;
                return true;
            }
            else
            {
                msg = "";
                return false;
            }
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
            throw new Exception(key + " not in dic.");
        }
        public static Variable GetConstant(int value)
        {
            if (!constants.ContainsKey(value))
            {
                string name = "c." + value.ToString();
                Variable var = new Variable(name, name,Type.INT,false);
                var.isConst = true;
                variables.Add(name, var);
                loadFile.AddStartLine(Core.VariableOperation(var, value, "="));
                constants.Add(value, var);
            }
            return constants[value];
        }

        public static string getLazyVal(string val)
        {
            foreach(var lst in lazyEvalVar)
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

        private static void stringInit()
        {
            File fFile = new File("__multiplex__/sstring");
            stringPool = fFile;
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
            return context.GetVariable(val) != null && variables[context.GetVariable(val)].type == Type.STRING;
        }
        public static string getString(string val)
        {
            Variable v = variables[context.GetVariable(val)];
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
                left = smartSplit(splitted[0], ',');
                right = smartSplit(splitted[1], ',');
            }
            else if (op == ":=")
            {
                string[] splitted = smartSplit(text.Replace(op, "="), '=', 1);
                left = smartSplit(splitted[0], ',');
                right = smartSplit(splitted[1], ',');

                string output = "";
                for (int i = 0; i < left.Length; i++)
                {
                    output+=parseLine("if (" + left[i] + "==null){" +left[i] +"="+ right[i%right.Length]+"}")+"\n";
                }
                return output;
            }
            else
            {
                string[] splitted = smartSplit(text.Replace(op, "="), '=', 1);
                left = smartSplit(splitted[0], ',');
                right = smartSplit(splitted[1], ',');
            }
            
            if (right[0].Contains("(") && context.IsFunction(right[0].Substring(0, right[0].IndexOf('(')))
                && !smartContains(right[0], '+') && !smartContains(right[0], '-') && !smartContains(right[0], '*')
                && !smartContains(right[0], '%') && !smartContains(right[0], '/'))
            {
                return functionEval(right[0], left, op);
            }
            else
            {
                string output = "";
                if (left.Length == 1 && right.Length > 1)
                {
                    string rights = smartExtract(smartSplit(text, '=', 1)[1]);
                    Variable var;
                    if (context.isEntity(left[0]))
                    {
                        var = new Variable(context.GetEntityName(left[0]), left[0].Split('.')[1], Type.ENTITY_COMPONENT);
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
                    Variable var;
                    if (context.isEntity(left[i]))
                    {
                        var = new Variable(context.GetEntityName(smartExtract(left[i])), smartExtract(left[i].Split('.')[1]), Type.ENTITY_COMPONENT);
                    }
                    else
                    {
                        var = GetVariableByName(smartExtract(left[i]));
                    }

                    if (right.Length > 1)
                        output += eval(smartExtract(right[i]), var, var.type, op);
                    else
                        output += eval(smartExtract(right[0]), var, var.type, op);
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
            float tmpF;
            bool containsOP = (smartContains(val, '+') || smartContains(val, '-') || smartContains(val, '*') || smartContains(val, '%')
                || smartContains(val, '/'));
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
                    foreach (Variable struV in structs[variable.enums].fields)
                    {
                        output += parseLine(variable.gameName + "." + struV.name + "= null");
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
            else if ((smartContains(val,'^') || smartContains(val, '&') || smartContains(val,'|')) && ca == Type.BOOL)
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
            else if (ca == Type.ARRAY)
            {
                val = smartExtract(val);
                    
                if (val.StartsWith("[") || val.StartsWith("{"))
                {
                    string[] nValue = smartSplit(val.Substring(1, val.Length - 2),',');

                    if (nValue.Length != variable.arraySize)
                        throw new Exception("Array don't the same size");

                    output = "";
                    for (int i = 0; i < variable.arraySize; i++)
                    {
                        output += parseLine(variable.gameName + "." + i.ToString() + op+  nValue[i]);
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
                        output += parseLine(variable.gameName + "." + i.ToString() +op+val);
                    }
                    return output;
                }
            }
            else if (ca == Type.FUNCTION)
            {
                int intVal=0;
                if (val.Contains("=>"))
                {
                    return InstLamba(val, variable);
                }
                else if (val.StartsWith("{"))
                {
                    return InstLamba("=>"+val, variable);
                }
                else if (op == "#=" && int.TryParse(val, out intVal))
                {
                    return Core.VariableOperation(variable, intVal, "=");
                }
                else if (valVar == null ||(op == "#="))
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
                
                if (getStruct(val.Replace("(", " ")) != null)
                {
                    if (stru1.isClass)
                    {
                        Structure.DerefObject(variable);
                    }
                    string stru = getStruct(val.Replace("(", " "));
                    if (structs[stru] == stru1)
                    {
                        output += parseLine(variable.gameName + ".__init__" + val.Substring(val.IndexOf('('), val.LastIndexOf(')') - val.IndexOf('(') + 1));
                    }
                    else
                    {
                        int id = tmpID++;
                        preparseLine(stru + " tmp." + id.ToString() + " = " + stru + "(" + getArg(val) + ")");
                        preparseLine(variable.gameName +" = tmp." + id.ToString());
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
                        if (stru1.isClass)
                        {
                            Structure stru2 = structs[valVar.enums];

                            output = "";

                            Structure.RefObject(valVar);
                            Structure.DerefObject(variable);

                            output += Core.VariableOperation(variable, valVar, "=");
                        }
                        else if (stru1.methodsName.Contains("__set__"))
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
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1f/compilerSetting.FloatPrecision) + "scoreboard players get " + GetConstant(tmpI).scoreboard() + '\n';
                        }
                    }
                    else if (float.TryParse(val, out tmpF))
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
                    output += parseLine(NBT_Data.getType(variable.gameName) +" tmp.0 = " + variable.name + "." + variable.gameName + op[0] + "(" + val + ")");
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
                else if (float.TryParse(val, out tmpF))
                {
                    tmpI = (int)(tmpF * compilerSetting.FloatPrecision);
                    if (op == "#=")
                        return Core.VariableOperation(variable, tmpI, "=");
                    else
                    {
                        output += Core.VariableOperation(variable,  (int)(tmpF), op);
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
                                else
                                {
                                    output += parseLine("float tmp.0 = " + val+"/"+ compilerSetting.FloatPrecision.ToString());
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
                    int val2 = tmpI * ((op != "*=" && op != "/=")? compilerSetting.FloatPrecision : 1);
                    return Core.VariableOperation(variable, val2, op);
                }
                else if (float.TryParse(val, out tmpF))
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
                            output += parseLine("int tmp.1 = "+ compilerSetting.FloatPrecision.ToString()+ "*" + val);
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
                            throw new Exception("Unkowned: \"" + smartEmpty(val)+"\" in context "+context.GetVar());
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
                        string cond = getCondition("!"+val);
                        return cond + Core.VariableOperation(variable, 0, "=");
                    }
                }
                else if (op == "|=" || op == "+=")
                {
                    if (valVar != null)
                    {
                        output += Core.VariableOperation(variable, valVar, "+=");
                        string cond = getCondition(val +">= 2");
                        return output+cond + Core.VariableOperation(variable,1, "=");
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
                        string cond2 = getCondition(val+"&&"+variable.gameName);

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
                    output += "tag @e remove " + variable.gameName + '\n'+
                        "tag " + smartEmpty(val) + " add " + variable.gameName + '\n';
                }
                else
                {
                    output += "tag @e remove " + variable.gameName + '\n'+
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
            string[] operations = new string[] { "^", "|","&" ,"+", "-", "%", "/", "*" };
            string val2 = getParenthis(val, 1);
            
            foreach (string xop in operations)
            {
                part = smartSplit(val2, xop[0], 2);

                if (part.Length > 1)
                {
                    float valLeft = 0;
                    float valRight = 0;
                    if (Calculator.TryCalculate(part[0], out valLeft))
                    {
                        part[0] = valLeft.ToString();
                    }
                    if (Calculator.TryCalculate(part[1], out valRight))
                    {
                        part[1] = valRight.ToString();
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
                        
                        Variable v1 = new Variable(id.ToString(), context.GetVar()+"__eval__" + id.ToString(), ca);
                        v1.enums = variable.enums;
                       
                        if (op != "=")
                        {
                            AddVariable("__eval__" + id.ToString(), v1);
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
            
            throw new Exception("Unsportted operation: "+val);
        }
        private static string Decurry(string val, Variable variable, Type ca, string op = "=", int recCall = 0)
        {
            string left = getFunctionName(val);
            throw new Exception("Decurring not Implemented Yet");
            int id = tmpID++;
            Variable v1 = new Variable(id.ToString(), context.GetVar() + "__eval__" + id.ToString(), getExprType(val));
            v1.enums = variable.enums;
            AddVariable("__eval__" + id.ToString(), v1);

            if (curriedReg.Match(val).Success)
            {
                Decurry(getFunctionName(val), variable, ca, op);
            }
            return "";
        }

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
                if (smartEmpty(arg[i]) == "") {
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

            out2 += parseLine("bool __eval" + idVal.ToString() + "__ = " + text.Replace("&&","&").Replace("||", "|"));

            string[] part = getCond("__eval" + idVal.ToString() + "__");

            out2 += part[1];

            out1 = part[0];
            return new string[] { out1, out2 };
        }
        private static string[] getCond(string text)
        {
            int tmpI;
            float tmpF;
            int idVal = If.GetEval(context.GetFun());
            
            if (context.isEntity(text) && !smartContains(text,'.'))
            {
                return Core.ConditionEntity(context.GetEntitySelector(text));
            }
            else if (text.StartsWith("tag("))
            {
                return Core.ConditionEntity("@s[tag="+ getArg(text) + "]");
            }
            else if (text.StartsWith("block("))
            {
                string[] args = getArgs(text);
                string output = args[0];
                if (args[0].Contains("#") && !args[0].Contains(":"))
                {
                    string[] part = args[0].Split(' ');
                    output = part[0] + " " + part[1] + " " + part[2] + " #" + Project + ":" + part[3].Replace("#","");
                }
                return Core.ConditionBlock(output);
            }
            else if (text.StartsWith("blocks("))
            {
                string[] args = getArgs(text);
                return Core.ConditionBlocks(args[0]);
            }
            else if (context.GetPredicate(text, true) != null)
            {
                Predicate pred = GetPredicate(context.GetPredicate(text), getArgs(text));
                return new string[] { "if predicate " + pred.get(getArg(text)) +" " ,""};
            }
            else if (text.Contains("=="))
            {
                string[] arg = text.Replace("==", "=").Split('=');

                if (smartEmpty(arg[0]) == smartEmpty(arg[1]) && !IsFunction(smartEmpty(arg[0])))
                {
                    return new string[] { ConditionAlwayTrue, "" };
                }
                float ta = 0, tb = 0;
                if (float.TryParse(smartEmpty(arg[0]), out ta) && float.TryParse(smartEmpty(arg[1]), out tb))
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
                if (arg[0].StartsWith("@") && smartContains(arg[0], '.')){
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
                if (context.GetVariable(arg[0], true) != null)
                {
                    var = GetVariableByName(arg[0]);
                }
                else
                {
                    int idVal3 = If.GetEval(context.GetFun());
                    
                    pre += parseLine(getExprType(arg[0]).ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + arg[0]);
                    var = GetVariableByName("cond_" + idVal3);
                }


                arg[1] = smartEmpty(arg[1]);

                Type t = var.type;

                if (arg[1] == "null")
                {
                    return Core.ConditionInverse(
                        appendPreCond(Core.CompareVariable(GetVariableByName(arg[0]), GetVariableByName(arg[0]), "=", selector1, selector1),pre));
                }

                if (t == Type.STRUCT)
                {
                    return getCond(var + ".__equal__("+var.gameName+")");
                }
                else if ((t == Type.INT || t == Type.ENUM || t == Type.FUNCTION))
                {
                    if (arg[1].Contains(".."))
                    {
                        string[] part = arg[1].Replace("..", ",").Split(',');
                        int p1 = int.MinValue;
                        int p2 = int.MaxValue;

                        if (part[0]!= "")
                            p1 = ((int)(int.Parse(part[0])));
                        if (part[1] != "")
                            p2 = ((int)(int.Parse(part[1])));

                        return appendPreCond(Core.CompareVariable(var, p1,p2, selector1),pre);
                    }
                    else if(int.TryParse(arg[1], out tmpI))
                    {
                        return appendPreCond(Core.CompareVariable(var, tmpI, "=", selector1),pre);
                    }
                    
                    if (var.enums != null && enums[var.enums].Contains(smartEmpty(arg[1].ToLower())))
                    {
                        return appendPreCond(Core.CompareVariable(var, enums[var.enums].IndexOf(smartEmpty(arg[1].ToLower())), "=", selector1),pre);
                    }
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2),pre);
                }
                else if (t == Type.FLOAT && arg[1].Contains(".."))
                {
                    string[] part = arg[1].Replace("..", ",").Split(',');
                    int p1 = int.MinValue;
                    int p2 = int.MaxValue;

                    if (part[0] != "")
                        p1 = ((int)(float.Parse(part[0])* compilerSetting.FloatPrecision));
                    if (part[1] != "")
                        p2 = ((int)(float.Parse(part[1])* compilerSetting.FloatPrecision));

                    return appendPreCond(Core.CompareVariable(var, p1, p2, selector1),pre);
                }
                else if (t == Type.FLOAT && float.TryParse(arg[1], out tmpF))
                {
                    return appendPreCond(Core.CompareVariable(var, (int)(tmpF * compilerSetting.FloatPrecision), "=", selector1),pre);
                }
                else if (t == Type.BOOL)
                {
                    if (arg[1] == "true")
                        return appendPreCond(Core.CompareVariable(var, 1, "="),pre);
                    else if (arg[1] == "false")
                        return appendPreCond(Core.CompareVariable(var, 0, "="),pre);
                    else
                        return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2),pre);
                }
                else if (t == Type.STRING && arg[1].Contains("\""))
                {
                    return appendPreCond(Core.CompareVariable(var, getStringID(arg[1]), "=", selector1),pre);
                }
                else if (context.GetVariable(arg[1], true) != null)
                {
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2),pre);
                }
                else
                {
                    int idVal2 = If.GetEval(context.GetFun());
                    pre += parseLine(getExprType(arg[1]).ToString().ToLower() + " cond_" + idVal2.ToString() + " = " + arg[1]);
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
                float ta = 0, tb = 0;
                if (float.TryParse(smartEmpty(arg[0]), out ta) && float.TryParse(smartEmpty(arg[1]), out tb))
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

                if (arg[1].Replace(" ","") == "null")
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
                        pre += parseLine(getExprType(arg[0]).ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + arg[0]);
                        var = GetVariableByName("cond_" + idVal3);
                    }

                    return appendPreCond(Core.CompareVariable(var, var, "==", selector1),pre);
                }
                else
                {
                    return Core.ConditionInverse(getCond(text.Replace("!=", "==")));
                }
            }
            else if ((context.isEntity(text) || text.StartsWith("@")) && !smartContains(text,'.'))
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
                if (text.Contains("<=")) op = "<=";
                else if (text.Contains("<")) op = "<";
                else if (text.Contains(">=")) op = ">=";
                else if (text.Contains(">")) op = ">";

                if (op == "")
                {
                    if (smartEmpty(text) == "true"){return new string[] { ConditionAlwayTrue, "" };}
                    if (smartEmpty(text) == "false") { return new string[] { ConditionAlwayFalse, "" }; }
                    float a = 0;
                    if (float.TryParse(smartEmpty(text), out a) && a > 0) { return new string[] { ConditionAlwayTrue, "" }; }
                    if (float.TryParse(smartEmpty(text), out a) && a <= 0) { return new string[] { ConditionAlwayFalse, "" }; }

                    if (text.StartsWith("!"))
                        return Core.CompareVariable(GetVariableByName(text.Replace("!", "")), 0, "<=");
                    else
                        return Core.CompareVariable(GetVariableByName(text.Replace("!", "")), 1, ">=");
                }

                string[] arg = smartEmpty(text).Replace(op, "=").Split('=');

                #region smartCompile
                float ta = 0, tb = 0;
                bool tc = false;
                bool td = smartEmpty(arg[0]) == smartEmpty(arg[1]) && !IsFunction(smartEmpty(arg[0]));

                tc = float.TryParse(smartEmpty(arg[0]), out ta) && float.TryParse(smartEmpty(arg[1]), out tb);
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
                if (context.GetVariable(arg[0], true) != null){
                    var = GetVariableByName(arg[0]);
                }
                else
                {
                    int idVal3 = If.GetEval(context.GetFun());
                    
                    pre += parseLine(getExprType(arg[0]).ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + arg[0]);
                    var = GetVariableByName("cond_" + idVal3);
                }

                Type t = getExprType(arg[0]);

                if ((t == Type.INT || t == Type.ENUM) && int.TryParse(arg[1], out tmpI))
                {
                    return appendPreCond(Core.CompareVariable(var, tmpI, op, selector1), pre);
                }
                if ((t == Type.ENUM) && enums[var.enums].valuesName.Contains(smartEmpty(arg[1]).ToLower()))
                {
                    return appendPreCond(Core.CompareVariable(var, enums[var.enums].valuesName.IndexOf(smartEmpty(arg[1]).ToLower()), op, selector1), pre);
                }
                else if (t == Type.FLOAT && float.TryParse(arg[1], out tmpF))
                {
                    int tmpL = ((int)(tmpF * compilerSetting.FloatPrecision));
                    return appendPreCond(Core.CompareVariable(var, tmpL, op, selector1), pre);
                }
                else if (context.GetVariable(arg[1],true)!=null)
                {
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName(arg[1]), op, selector1, selector2), pre);
                }
                else
                {
                    int idVal2 = If.GetEval(context.GetFun());
                    pre += parseLine(getExprType(arg[1]).ToString().ToLower() + " cond_" + idVal2.ToString() + " = " + arg[1]);
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName("cond_" + idVal2), op, selector1), pre);
                }
            }
        }
        private static string[] appendPreCond(string[] text, string val)
        {
            return new string[] { text[0], text[1] + "\n" + val };
        }


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
                lambda = false;

                string[] args = new string[input_args.Length + (lambda ? 1 : 0)];
                for (int i = 0; i < input_args.Length; i++)
                {
                    args[i] = input_args[i];
                }
                bool argIsString = args.Length > 0 ? isString(args[0]) : false;


                if (lambda)
                    args[args.Length - 1] = "__lambda__";

                if (argIsString)
                {
                    foreach (string arg in smartSplitJson(extractString(args[0]), ' '))
                    {
                        numericalOnly = numericalOnly && (float.TryParse(arg, out float _) || arg.StartsWith("~") || arg.StartsWith("^"));
                    }
                }
                else
                {
                    foreach (string arg in args)
                    {
                        numericalOnly = numericalOnly && (float.TryParse(arg, out float _) || arg.StartsWith("~") || arg.StartsWith("^"));
                    }
                }

                Type[] argType = new Type[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    try
                    {
                        argType[i] = getExprType(args[i]);
                    }
                    catch {
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
                            if (i >= f.args.Count - (f.isExtensionMethod ? 1 : 0) || argType[i] != f.args[i + (f.isExtensionMethod ? 1 : 0)].type ||
                                (f.lazy && f.args[i + (f.isExtensionMethod ? 1 : 0)].isLazy && !f.tags.Contains("__numerical_only__")))
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
                            if (i >= f.args.Count- (f.isExtensionMethod ? 1 : 0) ||
                                (argType[i] != f.args[i + (f.isExtensionMethod ? 1 : 0)].type &&
                                !((argType[i] == Type.INT || argType[i] == Type.FLOAT) &&
                                (f.args[i+ (f.isExtensionMethod ? 1 : 0)].type == Type.INT || f.args[i+ (f.isExtensionMethod ? 1 : 0)].type == Type.FLOAT)))
                                || (f.lazy && f.args[i+ (f.isExtensionMethod ? 1 : 0)].isLazy && !f.tags.Contains("__numerical_only__")))
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
                            if (argType[i] != Type.UNKNOWN && i < f.args.Count - (f.isExtensionMethod ? 1 : 0) &&
                                ((argType[i] != f.args[i+ (f.isExtensionMethod ? 1 : 0)].type &&
                                !((argType[i] == Type.INT || argType[i] == Type.FLOAT) &&
                                (f.args[i+ (f.isExtensionMethod ? 1 : 0)].type == Type.INT || f.args[i+ (f.isExtensionMethod ? 1 : 0)].type == Type.FLOAT)))
                                || (f.lazy && f.args[i+ (f.isExtensionMethod ? 1 : 0)].isLazy && !f.tags.Contains("__numerical_only__"))))
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

                int cutOutArgsLength = (args.Length > 0)?smartSplitJson(argIsString?(extractString(args[0])):args[0], ' ').Length:0;

                foreach (Function f in functions[funcName])
                {
                    int functionCount = 0;
                    foreach (var arg in f.args)
                    {
                        if (arg.type == Type.FUNCTION)
                        {
                            functionCount++;
                        }
                    }

                    if (funObj == null && f.lazy && args.Length >= f.args.Count - functionCount- (f.isExtensionMethod ? 1 : 0) && args.Length <= f.args.Count- (f.isExtensionMethod ? 1 : 0))
                    {
                        funObj = f;
                    }
                    if (wasEmpty && f.lazy && args.Length == 1 && f.args.Count- (f.isExtensionMethod ? 1 : 0) > 1
                        && cutOutArgsLength >= f.args.Count - functionCount - (f.isExtensionMethod ? 1 : 0)
                        && cutOutArgsLength <= f.args.Count- (f.isExtensionMethod ? 1 : 0))
                    {
                        funObj = f;
                    }
                }
                if (funObj != null) { return funObj; }

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
        public static void CreateFunctionGRP(Variable variable,string grp)
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
            if (func.file.UnparsedFunctionFile)
                func.file.addParsedLine("if ("+mux.gameName + "== " + id.ToString()+"){" + mux.gameName +"=-1}");
            else
                func.file.AddLine(cond + Core.VariableOperation(mux, -1, "="));
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
            if ((functDelegated[grp].Count-1) % (compilerSetting.TreeMaxSize) == 0 && functDelegated[grp].Count > 1)
            {
                Variable mux = GetVariable("__mux__" + grp);
                if (fFiles.Count == 1)
                {
                    string muxedFileNameInit = "__multiplex__/" + grp + "/__splitted_" + (fFiles.Count - 1).ToString();
                    int lowerBound1 = (compilerSetting.TreeMaxSize * (fFiles.Count - 1));
                    int upperBound1 = (compilerSetting.TreeMaxSize * fFiles.Count) - 1;

                    File newfFileInit = new File(muxedFileNameInit,"");
                    newfFileInit.AddLine(fFiles[0].content);
                    fFiles.Add(newfFileInit);
                    files.Add(newfFileInit);

                    fFiles[0].content = "";
                    

                    string cond1 = getCondition(mux.gameName + "== " + lowerBound1.ToString() + ".." + upperBound1.ToString());
                    fFiles[0].AddLine(cond1 + "function " + context.getRoot() + ":" + muxedFileNameInit);
                }

                string muxedFileName = "__multiplex__/" + grp + "/__splitted_" + (fFiles.Count - 1).ToString();
                int lowerBound = (compilerSetting.TreeMaxSize * (fFiles.Count - 1));
                int upperBound = (compilerSetting.TreeMaxSize * fFiles.Count) - 1;

                File newfFile = new File(muxedFileName,"");
                fFiles.Add(newfFile);
                files.Add(newfFile);

                string cond = getCondition(mux.gameName + "== " + lowerBound.ToString()+ ".." + upperBound.ToString());
                fFiles[0].AddLine(cond + "function "+ context.getRoot() + ":"+ muxedFileName);
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
                CreateFunctionGRP(variable,grp);
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

            output += "function " + context.getRoot() + ":__multiplex__/" + grp + '\n';
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
            if (!functions.ContainsKey(Project + ".__tags__." + tag.ToLower()))
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
        public static void AddToFunctionTag(Function func, string tag)
        {
            bool wasUsed = func.file.notUsed;

            File f = CreateFunctionTag(tag)
                .AddLine(parseLine(func.gameName.Replace(":", ".").Replace("/", ".") + "()"));
            functionTags[tag.ToLower()].Add(func.gameName.Replace(":", ".").Replace("/", "."));
            func.file.notUsed = wasUsed;
            f.addChild(func.file);
        }
        #endregion

        #region instantiation
        public static string InstCompilerVar(string text)
        {
            string[] splited = smartSplit(text, '=', 1);
            string[] field = smartSplit(smartExtract(splited[0]), ' ');
            string name = field[field.Length - 1];
            string value = smartExtract(splited[1]);

            if (value.StartsWith("fromenum"))
            {
                string[] argget = getArgs(value);
                Enum enu = enums[getEnum(argget[0])];
                List<string> sortedField = new List<string>();
                sortedField.AddRange(enu.fieldsName);
                sortedField.Sort((a, b) => b.Length.CompareTo(a.Length));
                compVal[compVal.Count - 1].Add(name, enu.GetValueOf(smartExtract(argget[1].ToLower())));
                foreach (string f in sortedField)
                {
                    compVal[compVal.Count - 1].Add(name + "." + f, enu.GetFieldOf(smartExtract(argget[1].ToLower()), f));
                }
                return "";
            }
            else if (value.StartsWith("fromconst"))
            {
                string[] argget = getArgs(value);
                var var = GetVariableByName(argget[0]);
                compVal[compVal.Count - 1].Add(name, var.constValue);
                return "";
            }
            else if (value.StartsWith("("))
            {
                string[] argget = getArgs(value);
                for (int i = 0; i < argget.Length; i++)
                {
                    compVal[compVal.Count - 1].Add(name+"."+i.ToString(), smartExtract(argget[i]));
                }
                compVal[compVal.Count - 1].Add(name+".count", argget.Length.ToString());
                compVal[compVal.Count - 1].Add(name, value);
                return "";
            }
            else if (value.StartsWith("$"))
            {
                if (structCompVarPointer != null)
                {
                    value = compVarReplace(value);

                    structCompVarPointer[name] = compVarReplace(value);
                }
                else if(structStack.Count > 0 && !isInStructMethod)
                {
                    structStack.Peek().compField.Add(name, value);
                }
                return "";
            }
            
            Type type;
            try
            {
                type = getType(text);
            }
            catch
            {
                type = Type.DEFINE;
            }

            if (structStack.Count > 0 && !isInStructMethod)
            {
                structStack.Peek().compField.Add(name,value);
                return "";
            }
            else
            {
                if (compVal.Count == 0)
                {
                    compVal.Add(new Dictionary<string, string>());
                }

                if (type == Type.INT || type == Type.FUNCTION || type == Type.FLOAT || type == Type.DEFINE)
                {
                    Variable valVar = GetVariableByName(value, true);
                    if (valVar != null)
                    {
                        compVal[compVal.Count - 1].Add(name + ".enums", valVar.enums);
                        compVal[compVal.Count - 1].Add(name + ".type", valVar.GetTypeString());
                        compVal[compVal.Count - 1].Add(name + ".name", valVar.gameName);
                        compVal[compVal.Count - 1].Add(name + ".scoreboard", valVar.scoreboard());
                        compVal[compVal.Count - 1].Add(name + ".scoreboardname", valVar.scoreboard().Split(' ')[1]);
                    }
                    if (type == Type.FUNCTION)
                    {
                        compVal[compVal.Count - 1].Add(name + ".name", functions[context.GetFunctionName(value)][0].gameName);
                    }
                    compVal[compVal.Count - 1].Add(name, smartExtract(value));
                }
                else if (type == Type.JSON)
                {
                    compVal[compVal.Count - 1].Add(name, value);
                }
                else if (type == Type.STRING)
                    compVal[compVal.Count - 1].Add(name, value);
                else
                {
                    Variable valVar = GetVariableByName(value);
                    if (valVar != null)
                    {
                        compVal[compVal.Count - 1].Add(name + ".scoreboard", valVar.scoreboard());
                        compVal[compVal.Count - 1].Add(name + ".enums", valVar.enums);
                        compVal[compVal.Count - 1].Add(name + ".type", valVar.GetTypeString());
                        compVal[compVal.Count - 1].Add(name + ".name", valVar.gameName);
                    }
                    compVal[compVal.Count - 1].Add(name, valVar.gameName);
                }
                return "";
            }
        }
        public static string InstVar(string text)
        {
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
                while(text.StartsWith(" "))
                {
                    text = text.Substring(1, text.Length - 1);
                }
            }
            if (text.StartsWith("val "))
            {
                isConst = true;
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

            text = functionDesugar(text);

            ca = getType(text);

            bool entity = text.ToUpper().Substring(0, 3) == text.Substring(0, 3);

            string op = opReg.Match(text).Value;

            Variable variable;
            string def = "dummy";

            string[] splited = op!="" ? 
                                smartSplit(text.Replace(op,"="), '=', entity?2:1) : 
                                smartSplit(text, '=', entity ? 2 : 1);
            string[] left = smartSplit(splited[0], ' ',1);
            vari = smartEmpty(left[left.Length-1]);

            if (splited.Length > 1)
                def = splited[1].Replace(" ", "");

            bool entityFormatVar = false;
            if (float.TryParse(def, out float tmp) || ca == Type.STRUCT)
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
                string name = prefix+v;

                if (isStatic)
                {
                    name = name.Replace("__struct__", "");
                    prefix = prefix.Replace("__struct__", "");
                }

                if (variables.ContainsKey(name))
                {
                    variables.Remove(name);
                    GlobalDebug(name + " was shadowed.", Color.Yellow);
                }

                if (ca == Type.ARRAY)
                {
                    string arraySizeS = arraySizeReg.Match(text).Value.Replace("[", "").Replace("]", "");
                    int arraySize = int.Parse(arraySizeS);

                    variable = new Variable(v, name, ca, entity, def);
                    variable.isConst = isConst;
                    variable.arraySize = arraySize;
                    variable.isPrivate = isPrivate;
                    variable.isStatic = isStatic;
                    variable.privateContext = context.GetVar();
                    AddVariable(name, variable);

                    string typeArray = arrayTypeReg.Match(text).Value.Replace("[", "");
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
                        if (part == 2 && defValue[index].Replace(" ", "").ToLower().StartsWith(strucName))
                        {
                            string instArg = defValue[index].Substring(defValue[index].IndexOf('('),
                            defValue[index].LastIndexOf(')') - defValue[index].IndexOf('(') + 1);

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
                    foreach(Function fun in ExtensionMethod[typeStr])
                    {
                        fun.LinkCopyTo(variable.name, true);
                    }
                }
                
                if (isConst)
                {
                    //float val;
                    //string valStr = smartExtract(splited[1]);
                    //if (Calculator.TryCalculate(valStr, out val)) { valStr = val.ToString(); }
                    variable.constValue = splited[1];
                    variable.UnparsedFunctionFileContext = prefix;
                    variable.wasSet = true;
                }

                if (ca == Type.STRUCT && part == 2 && defValue[index].Replace(" ","").ToLower().StartsWith(getStruct(text)))
                {
                    index = (index + 1) % defValue.Length;
                }
            }

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
                functionDesc = "#"+text + "\n";
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
            string arg = getArg(text);
            string[] args = smartSplit(arg, ',');
            

            List<string> outputType = new List<string>();
            List<string> tags = new List<string>();
            if (funArgType.Length > 1)
            {
                for (int i = 0; i < funArgType.Length - 1; i++)
                {
                    if (funArgType[i] == "def"){}
                    else if (funArgType[i] == "static")
                    {
                        isStatic = true;
                    }
                    else if (funArgType[i] == "helper")
                    {
                        isHelper = true;
                    }
                    else if(funArgType[i] == "ticking")
                    {
                        isTicking = true;
                    }
                    else if(funArgType[i] == "loading")
                    {
                        isLoading = true;
                    }
                    else if (funArgType[i] == "external")
                    {
                        isExternal = true;
                    }
                    else if (funArgType[i] == "__lambda__")
                    {
                        isLambda = true;
                    }
                    else if(funArgType[i] == "lazy")
                    {
                        lazy = true;
                    }
                    else if(funArgType[i] == "abstract")
                    {
                        isAbstract = true;
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

            string fullName = (isExternal?"":context.GetFun().Replace(":", "/")) + func.Replace(".","/");

            if (fullName.Contains("/"))
                fullName = fullName.Substring(0, fullName.IndexOf("/")) + ":" +
                            fullName.Substring(fullName.IndexOf("/") + 1, fullName.Length - fullName.IndexOf("/") - 1);

            string funcID = fullName.Replace(':', '.').Replace('/', '.');
            string subName = fullName.Substring(fullName.IndexOf(":") + 1, fullName.Length - fullName.IndexOf(":") - 1);

            if (isStatic) {
                fullName = fullName.Replace("__struct__", "");
                funcID = funcID.Replace("__struct__", "");
                subName = subName.Replace("__struct__", "");
            }

            File fFile = new File(subName);
            Function function = new Function(func, fullName, fFile);

            funcDef.Add(subName.Replace("/","."));
            function.desc = functionDesc;
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
                string[] ret = tmp[1].Replace(" ", "").Split(',');
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
                string c = a.Replace("="," ");

                while (c.StartsWith(" "))
                {
                    c = c.Substring(1, c.Length - 1);
                }
                while (c.Contains("  ")) {
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

                    parseLine(a.Replace("implicite ",""));
                    b.variable = GetVariableByName(name);
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


            if (!(structStack.Count > 0 && !isStatic))
            {
                if (isLoading)
                {
                    loadFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                    fFile.use();
                    if (callStackDisplay)
                        callTrace += "\"load\"->\"" + function.gameName + "\"\n";
                }

                if (isTicking)
                {
                    mainFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
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

            return "";
        }
        public static string InstWhile(string text, string fText)
        {
            string loop = getCondition(text);

            int wID = While.GetID(context.GetFun());
            string funcName = context.GetFun() + "w_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().AddLine(loop + cmd);

            File fFile = new File(context.GetFile() + "w_" + wID, "", "while");
            fFile.AddEndLine(loop + cmd);
            context.Sub("w_" + wID, fFile);
            files.Add(fFile);

            autoIndent(fText);

            return "";
        }
        public static string InstIf(string text, string fText, int mult = 0, string extra = "")
        {
            string loop = getCondition(((mult >= 2 && (LastCond.wasAlwayFalse || LastCond.wasAlwayTrue)) ? "" : extra) + text);

            If wID = new If();
            If wID2 = new If(-1);
            wID.wasAlwayTrue = loop == "";
            wID.wasAlwayFalse = (loop.Contains(ConditionAlwayFalse) || (LastConds.Count > 0 && LastCond.wasAlwayTrue));

            wID2.wasAlwayFalse = wID.wasAlwayFalse;
            wID2.wasAlwayTrue = wID.wasAlwayTrue;
            
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
            else if (mult == 0 || mult == 3)
            {
                LastConds.Push(wID2);
            }

            if (wID.wasAlwayTrue){ }
            else if (wID.wasAlwayFalse || (mult >= 2 && LastCond.wasAlwayTrue)) {  }
            else { context.currentFile().AddLine(loop + cmd); }

            context.currentFile().cantMergeWith = true;

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
                else
                {
                    fFile.min = float.Parse(smartEmpty(args[1]));
                    fFile.max = float.Parse(smartEmpty(args[2]));
                    if (args.Length > 3)
                        fFile.step = float.Parse(smartEmpty(args[3]));
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
                foreach(var l in text){
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
                && (float.TryParse(text[0].Split(' ')[0], out float _a) || text[0].Split(' ')[0].StartsWith("~") || text[0].Split(' ')[0].StartsWith("^"))
                && (float.TryParse(text[0].Split(' ')[1], out float _b) || text[0].Split(' ')[1].StartsWith("~") || text[0].Split(' ')[1].StartsWith("^"))
                && (float.TryParse(text[0].Split(' ')[2], out float _c) || text[0].Split(' ')[2].StartsWith("~") || text[0].Split(' ')[2].StartsWith("^")))
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
        public static string InstPositioned(string text,string fText)
        {
            if (text.Split(' ').Length == 3 && !text.Contains(",")
                && (float.TryParse(text.Split(' ')[0], out float _a) || text.Split(' ')[0].StartsWith("~") || text.Split(' ')[0].StartsWith("^"))
                && (float.TryParse(text.Split(' ')[1], out float _b) || text.Split(' ')[1].StartsWith("~") || text.Split(' ')[1].StartsWith("^"))
                && (float.TryParse(text.Split(' ')[2], out float _c) || text.Split(' ')[2].StartsWith("~") || text.Split(' ')[2].StartsWith("^")))
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

            if (overriding && enums.ContainsKey(contextName+name))
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
                    e = new Enum(contextName + name, fields, smartSplit(field[1], ','), final);
                }
                else
                {
                    e = new Enum(contextName + name, smartSplit(field[1], ','), final);
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
            enums.Add(name, EnumConverter.GetEnum(name, resourceFiles[file], type, final));
            try
            {
                InstCompilerVar("int $" + name + ".length=" + enums[name].values.Count.ToString());
            }
            catch { }
            return "";
        }
        public static string InstBlockTag(string text)
        {
            string[] field = smartSplit(smartEmpty(text), '=');

            string name = smartEmpty(field[0].ToLower().Replace("blocktags", ""));
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

            string name = smartEmpty(field[0].ToLower().Replace("entitytags", ""));
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
        public static string InstStruct(string text, bool isClass)
        {
            string[] split = smartSplit(text.ToLower().Replace("{", ""), ' ');
            string name = split[1];
            string functionBase = null;
            File functionBaseFile = null;

            Structure parent = null;
            int mode = 0;
            for (int i = 2; i < split.Length; i++)
            {
                if (split[i] == "extends")
                {
                    mode = 1;
                }
                else if (split[i] == "initer")
                {
                    mode = -1;
                }
                else if (mode == 1)
                {
                    parent = structs[split[i]];
                }
                else if (mode == -1)
                {
                    functionBase = split[i];
                }
            }
            if (isClass && functionBase != null)
            {
                functionBaseFile = GetFunction(context.GetFunctionName(functionBase),new List<Argument>()).file;
            }
            
            string generics = "";
            if (name.Contains("<"))
            {
                inGenericStruct = true;
                generics = name.Substring(name.IndexOf("<") + 1, name.LastIndexOf(">") - name.IndexOf("<") - 1);
                name = smartEmpty(name.Substring(0, name.IndexOf("<")));
            }
            
            string[] contextName = context.GetVar().Split('.');
            context.Sub("__struct__"+name, new File("struct/"+name,"", "struct"));
            
            Structure stru = new Structure(name, parent);
           
            stru.isClass = isClass;
            stru.classInitBase = functionBaseFile;

            try
            {
                string dir = "";
                for (int i = contextName.Length-2; i >= 0; i--)
                {
                    structs[dir+name] = stru;
                    dir = contextName[i] + "."+ dir;
                }
                structs[dir + name] = stru;
                structStack.Push(stru);
            }
            catch(Exception e)
            {
                throw new Exception(name + " already exists? " + e.ToString());
            }

            foreach(string gen in smartSplit(generics,','))
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
            string lambda = "lamba_" + Lambda.GetID(context.GetFun()).ToString();
            string func = "def "+lambda;
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

            func += GetLambdaFunctionArgs(args,variable.args);
            if (variable.outputs.Count > 0){func += ":";}
            func += GetLambdaFunctionReturn(variable.outputs);

            func += "{";

            if (para[1].Contains("return") || variable.outputs.Count==0)
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
            if (float.TryParse(arg[1],out float _))
            {
                if (arg.Length > 3)
                {
                    int j = 0;
                    int max = (int)Math.Ceiling((float.Parse(arg[1]) - float.Parse(arg[2])) / float.Parse(arg[3]));

                    for (float i = float.Parse(arg[1]); i <= float.Parse(arg[2]); i += float.Parse(arg[3]))
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
                    int max = (int)Math.Ceiling((float.Parse(arg[1]) - float.Parse(arg[2])));

                    for (float i = float.Parse(arg[1]); i <= float.Parse(arg[2]); i ++)
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
                if (enums != null)
                {
                    var en = enums[getEnum(arg[1])];
                    foreach (var value in en.Values())
                    {
                        string line = block;
                        foreach (string field in en.fieldsName)
                        {
                            line = line.Replace(var + "." + field, en.GetFieldOf(value, field));
                        }
                        output += line;
                    }
                }
            }
            return output;
        }
        public static string InstJsonFile(string text)
        {
            string name = "";
            if (text.Contains("\"")){
                name = text.Substring(text.IndexOf("\"") + 1, text.LastIndexOf("\"") - text.IndexOf("\"") - 1);
            }
            else
            {
                name = smartSplit(text, ' ', 1)[1].Replace("{", "");
            }
            if (currentParsedFile != null && currentParsedFile.resourcespack)
            {
                if (currentParsedFile.name.Contains("/"))
                {
                    name = currentParsedFile.name.Substring(0, currentParsedFile.name.LastIndexOf("/") + 1) + name;
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
        #endregion

        #region Compiler Exception
        public static string Require(string text)
        {
            string[] args = smartSplit(text, ' ');
            if (args.Length > 2)
            {
                string comVar = args[1];
                string value = compVarReplace(comVar);
                string msg = args.Length > 4?args[4]:"";
                
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
                        throw new Exception("Fail requirement: " + value + " must be in enum " + args[3]+" Message: "+ msg);
                    }
                }
                else if (args[2] == "<" || args[2] == "<=" || args[2] == ">=" || args[2] == ">" || args[2] == "==")
                {
                    float valRight = float.Parse(smartExtract(evalDesugar(compVarReplace(args[3]))));
                    float valLeft = float.Parse(smartExtract(evalDesugar(value)));
                    bool valid = false;
                    if (args[2] == "<" && valLeft < valRight) { valid = true; }
                    if (args[2] == "<=" && valLeft <= valRight) { valid = true; }
                    if (args[2] == ">" && valLeft > valRight) { valid = true; }
                    if (args[2] == ">=" && valLeft >= valRight) { valid = true; }
                    if (args[2] == "==" && valLeft == valRight) { valid = true; }
                    
                    if (!valid)
                    {
                        throw new Exception("Fail requirement: " + value + " "+args[2]+" " + args[3] + " Message: " + msg);
                    }
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

        private static bool containType(string text)
        {
            if (typeMaps.Count > 0)
            {
                foreach (string key in typeMaps.Peek().Keys)
                {
                    if (text.ToLower().StartsWith(key))
                        return true;
                }
            }
            return false;
        }
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
                    if (t.ToLower().StartsWith(key))
                        return getType(typeMaps.Peek()[key], recCall + 1);
                }
            }

            Type type;
            if (t.Split('=')[0].Contains("[") && t.Split('=')[0].Contains("]"))
            {
                type = Type.ARRAY;
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
            else if (t.ToLower().StartsWith("int "))
            {
                type = Type.INT;
            }
            else if (t.ToLower().StartsWith("define "))
            {
                type = Type.DEFINE;
            }
            else if (t.ToLower().StartsWith("json "))
            {
                type = Type.JSON;
            }
            else if (t.ToLower().StartsWith("params "))
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
            else if (containEnum(t + " "))
            {
                type = Type.ENUM;
            }
            else if (t.ToLower().StartsWith("function<") || t.ToLower().StartsWith("function "))
            {
                type = Type.FUNCTION;
            }/*
            else if (containClass(t + " "))
            {
                type = Type.CLASS;
            }*/
            else if (containStruct(t + " "))
            {
                type = Type.STRUCT;
            }
            else if (t.StartsWith("entity "))
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
            else
            {
                throw new Exception("Unknown type: " + t);
            }
            return type;
        }
        public static Type getExprType(string t, int recCall = 0)
        {
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
                return getExprType(getParenthis(t), recCall+1);
            }
            if (t.ToLower() == "true" || t.ToLower() == "false")
            {
                return Type.BOOL;
            }
            if (int.TryParse(t, out int i))
            {
                return Type.INT;
            }

            if (float.TryParse(t, out float f))
            {
                return Type.FLOAT;
            }
            if (t.StartsWith("\""))
            {
                return Type.STRING;
            }
            if (context.GetVariable(t, true)!= null)
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

                    return GetFunction(funcName,args).outputs[0].type;
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
                return getExprType(v.Substring(1,v.Length-1));
            }
            if (t.Contains("*") || t.Contains("+") || t.Contains("-") || t.Contains("/")||
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
            context.GetVariable(t, true, false, 0, true);
            throw new Exception("Unparsable: "+t);
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
        
        public static string extractString(string text)
        {
            string tmp = smartEmpty(text);
            return tmp.Substring(1, tmp.Length - 2);
        }
        public static bool isString(string text)
        {
            string tmp = smartEmpty(text);
            return tmp.StartsWith("\"") && tmp.EndsWith("\"");
        }

        public static string functionEval(string text, string[] outVar = null, string op = "=")
        {
            string funcVar = smartExtract(text.Substring(0, text.IndexOf('(')));
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

                if (smartContains(text,'='))
                {
                    func = smartEmpty(smartSplit(text,'=',1)[1]).Substring(text.IndexOf(' ') + 1, text.IndexOf('(') - text.IndexOf('=') - 1);
                }
                else
                {
                    func = text.Substring(0, text.IndexOf('('));
                }
                if (containLazyVal(func))
                    func = getLazyVal(func);

                string output = "";
                if (func[0]=='@')
                {
                    string tag = func.Substring(1, func.Length - 1);
                    CreateFunctionTag(tag);

                    func = "__tags__." + tag.ToLower();
                }

                string funcName = context.GetFunctionName(func);


                Function funObj = GetFunction(funcName, args, smartContains(text,'{'));
                if (lazyCall.Contains(funObj))
                    throw new Exception("Cannot have recursive Lazy Recursive Function.");

                if (funObj.isPrivate)
                {
                    string cont = context.GetVar();
                    bool inPrivateContext = false;
                    foreach (string adj in adjPackage)
                    {
                        if (!cont.StartsWith(adj))
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
                    args2[0] = func.Replace("." + funObj.name,"");
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
                    callTrace += "\""+callingFunctName+"\"->\"" + funObj.gameName + "\"\n";

                //short notation
                if (!funObj.lazy)
                {
                    if (args.Length == 1 && funObj.args.Count == smartSplit(args[0],' ').Length)
                    {
                        args = smartSplit(args[0], ' ');
                    }
                }
                else
                {
                    int functionCount = 0;
                    foreach (var a in funObj.args)
                    {
                        if (a.type == Type.FUNCTION)
                        {
                            functionCount++;
                        }
                    }
                    if (args.Length == 1 && funObj.args.Count > 1 && isString(args[0]) 
                        && smartSplit(extractString(args[0]), ' ').Length >= funObj.args.Count - functionCount)
                    {
                        args = smartSplit(extractString(args[0]), ' ');
                    }
                    else if (args.Length == 1 && funObj.args.Count > 1 && smartSplit(args[0], ' ').Length >= funObj.args.Count - functionCount)
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
                    compVal.Add(new Dictionary<string, string>());

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
                    if (funObj.varOwner!= null)
                    {
                        thisDef.Push(funObj.varOwner.gameName+".");
                    }
                    int i = 0;


                    foreach (Argument a in funObj.args)
                    {
                        if (args.Length <= i)
                        {
                            if (a.defValue == null && !endWithAccollade && a.type == Type.FUNCTION)
                            {
                                string val = context.getImpliciteVar(a);
                                if (val != null && a.isImplicite)
                                {
                                    output += parseLine(a.GetInternalTypeString()+a.name + "=" + val);
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
                            if (a.defValue != null)
                                output += parseLine(a.gameName + "=" + a.defValue);
                            else if (endWithAccollade && a.type == Type.FUNCTION)
                            {
                                anonymusFuncName = "lamba_" + Lambda.GetID(context.GetFun()).ToString();

                                anonymusFuncNameArg = GetLambdaFunctionArgs(null, a.variable.args);

                                parseLine("def abstract __lambda__ " + anonymusFuncName + anonymusFuncNameArg);

                                compVal[compVal.Count-1].Add(a.name + ".name", functions[context.GetFunctionName(anonymusFuncName)][0].gameName);
                                if (a.name.StartsWith("$"))
                                    compVal[compVal.Count - 1].Add(a.name, anonymusFuncName);
                                else
                                    addLazyVal(a.name, anonymusFuncName);

                                anonymusFunc = true;
                            }
                        }
                        else
                        {
                            if (a.name.StartsWith("$"))
                            {
                                if (a.type == Type.ENTITY)
                                {
                                    if (!context.isEntity(smartExtract(args[i])))
                                    {
                                        throw new Exception("Entity is required!");
                                    }
                                    compVal[compVal.Count - 1].Add(a.name, context.GetEntitySelector(smartExtract(args[i])));
                                }
                                else if (a.type == Type.INT || a.type == Type.FUNCTION || a.type == Type.FLOAT)
                                {
                                    Variable valVar = GetVariableByName(smartExtract(args[i]), true);
                                    if (valVar != null){
                                        compVal[compVal.Count-1].Add(a.name + ".enums", valVar.enums);
                                        compVal[compVal.Count-1].Add(a.name + ".type", valVar.GetTypeString());
                                        compVal[compVal.Count-1].Add(a.name + ".name", valVar.gameName);
                                        compVal[compVal.Count-1].Add(a.name + ".scoreboard", valVar.scoreboard());
                                        compVal[compVal.Count - 1].Add(a.name + ".scoreboardname", valVar.scoreboard().Split(' ')[1]);
                                    }
                                    if (a.type == Type.FUNCTION)
                                    {
                                        compVal[compVal.Count - 1].Add(a.name + ".name", functions[context.GetFunctionName(smartExtract(args[i]))][0].gameName);
                                    }
                                    compVal[compVal.Count - 1].Add(a.name, smartExtract(args[i]));
                                }
                                else if (a.type == Type.JSON)
                                {
                                    string[] json = CommandParser.jsonFormat(args, context, i);
                                    init += json[1];
                                    clear += json[2];
                                    compVal[compVal.Count - 1].Add(a.name, json[0]);
                                }
                                else if (a.type == Type.PARAMS)
                                {
                                    string param = "(";
                                    for (int j = i; j < args.Length; j++)
                                    {
                                        param += args[j] + ",";
                                    }

                                    compVal[compVal.Count - 1].Add(a.name, param + ")");
                                }
                                else if (a.type == Type.STRING)
                                    compVal[compVal.Count - 1].Add(a.name, smartExtract(args[i]));
                                else
                                {
                                    Variable valVar = GetVariableByName(smartExtract(args[i]));
                                    compVal[compVal.Count-1].Add(a.name + ".scoreboard", valVar.scoreboard());
                                    compVal[compVal.Count-1].Add(a.name + ".enums", valVar.enums);
                                    compVal[compVal.Count-1].Add(a.name + ".type", valVar.GetTypeString());
                                    compVal[compVal.Count-1].Add(a.name + ".name", valVar.gameName);
                                    compVal[compVal.Count - 1].Add(a.name, valVar.gameName);
                                }
                            }
                            else
                            {
                                Variable valVar = GetVariableByName(smartExtract(args[i]), true);
                                if (valVar != null)
                                {
                                    addLazyVal(a.name, valVar.gameName);

                                    if (a.type == Type.STRUCT)
                                    {
                                        foreach (Variable v in structs[a.enums].fields)
                                        {
                                            addLazyVal(a.name + "." + v.name, valVar.gameName + "." + v.name);
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
                                        addLazyVal(a.name, args[i]);
                                    }
                                }
                                else
                                {
                                    addLazyVal(a.name, args[i]);
                                }
                            }
                        }
                        i++;
                    }

                    context.Sub("c_" + funObj.name, new File("c_" + funObj.name, "", "lazyfunctioncall"));

                    File tFile = context.currentFile();

                    i = 0;
                    autoIndented = 0;
                    if (funObj.variableStruct != null)
                        adjPackage.Push(funObj.variableStruct);
                    string prevFunction = callingFunctName;
                    callingFunctName = funObj.gameName;
                    foreach (string line in funObj.file.parsed)
                    {
                        preparseLine(line, tFile, true);
                        
                        i++;
                    }
                    tFile.Close();
                    
                    callingFunctName = prevFunction;
                    if (funObj.variableStruct != null)
                        adjPackage.Pop();

                    compVal.RemoveAt(compVal.Count - 1);
                    if (funObj.varOwner != null)
                    {
                        thisDef.Pop();
                    }
                    
                    output = init+output+"\n"+clear;
                    adjPackage.Pop();
                    lazyOutput.Pop();
                    lazyCall.Pop();
                    popLazyVal();
                    
                    if (anonymusFunc)
                    {
                        parseLine("def " + anonymusFuncName + anonymusFuncNameArg +"{");
                        if (smartEmpty(text).EndsWith("}"))
                        {
                            preparseLine(getCodeBlock(text));
                            preparseLine("}");
                        }
                        return "";
                    }
                    else
                    {
                        return "";
                    }
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
                                        throw new Exception(a.name+" already assigned!");
                                    
                                    output += parseLine(a.gameName + "=" + part[1])+"\n";
                                    assignedArg.Add(a);
                                    found = true;
                                }
                            }
                            if (!found)
                                throw new Exception("Unknown argument "+part[0]);
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
                                else if (a.defValue != null)
                                    output += parseLine(a.gameName + "=" + a.defValue);
                                else if (endWithAccollade && a.type == Type.FUNCTION)
                                {
                                    anonymusFuncName = "lamba_" + Lambda.GetID(context.GetFun()).ToString();

                                    anonymusFuncNameArg = GetLambdaFunctionArgs(null, a.variable.args);

                                    parseLine("def abstract " + anonymusFuncName + anonymusFuncNameArg);
                                    output += parseLine(a.gameName + "=" + anonymusFuncName) + "\n";
                                    anonymusFunc = true;
                                }
                            }
                        }
                    }

                    output += Core.CallFunction(funObj) + '\n';
                    if (outVar != null)
                    {
                        Variable valVar = GetVariableByName(outVar[0], true);

                        if (funObj.outputs.Count == 0 && outVar.Length > 0)
                        {
                            throw new Exception("Function "+ funObj.gameName+" do not return any value. ");
                        }
                        else if (valVar != null && valVar.type == Type.ARRAY && funObj.outputs[0].type != Type.ARRAY)
                        {
                            if (valVar.arraySize != funObj.outputs.Count)
                                throw new Exception("Cannot cast function output into array");

                            for (int j = 0; j < funObj.outputs.Count; j++)
                            {
                                string v = context.GetVariable(outVar[0]+"."+j.ToString());
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
                        parseLine("def " + anonymusFuncName + anonymusFuncNameArg+"{");
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
            if (lazyEvalVar.Count > 0)
            {
                int i = 0;

                foreach(Variable v in lazyOutput.Peek())
                {
                    ouput += parseLine(v.gameName + "=" + arg[i]);
                    i++;
                }
            }
            else
            {
                bool hadOutput = context.currentFile().function.outputs.Count > 0;
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
            output += functionDesc.Replace("\"\"\"","")+"\n";
            output += "#"+func.gameName+"\n";
            functionDesc = "";
            output += "#Argument:" + '\n';

            foreach (Argument arg in func.args)
            {
                output += "# - " + arg.gameName +" ("+arg.name+"): " + arg.GetTypeString() + '\n';
            }
            output += "#Output:" + '\n';

            foreach (Variable arg in func.outputs)
            {
                output += "# - " + arg.gameName + ": " + arg.GetTypeString() + '\n';
            }
            output += "#=================================================#" + '\n';

            return output;
        }

        public static string[] smartSplit(string text, char c, int max = -1)
        {
            List<string> output = new List<string>();
            int ind = 0;
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            bool inString = false;

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
                    ind -= 1;
                    stringBuilder.Append(text[i]);
                }
                else if (text[i] == '"')
                {
                    if (inString)
                    {
                        ind -= 1;
                        stringBuilder.Append(text[i]);
                        inString = false;
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
                else { stringBuilder.Append(text[i]); }
            }
            if (stringBuilder.ToString() != "")
                output.Add(stringBuilder.ToString());

            return output.ToArray();
        }
        public static string[] smartSplitJson(string text, char c, int max = -1)
        {
            List<string> output = new List<string>();
            int ind = 0;
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            bool inString = false;

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
                        ind -= 1;
                        stringBuilder.Append(text[i]);
                        inString = false;
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
                else { stringBuilder.Append(text[i]); }
            }
            if (stringBuilder.ToString() != "")
                output.Add(stringBuilder.ToString());

            return output.ToArray();
        }
        public static bool smartContains(string text, char c, int max = -1)
        {
            int ind = 0;
            bool inString = false;

            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            if (text.Contains(c) && text.StartsWith("("))
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
                else if (text[i] == '[')
                {
                    ind += 1;
                }
                else if (text[i] == ']')
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
        public static bool smartEndWith(string text, string c)
        {
            while(text.EndsWith(" ") || text.EndsWith("\n") || text.EndsWith("  "))
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
            while (text.EndsWith(" ") || text.EndsWith("\t"))
            {
                text = text.Substring(0, text.Length - 1);
            }
            return text;
        }
        public static string getArg(string text)
        {
            int opIndex = getOpenCharIndex(text, '(');
            return text.Substring(opIndex + 1, getCloseCharIndex(text,')') - opIndex - 1);
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
        public static string getFunctionName(string text)
        {
            int opIndex = getOpenCharIndex(text, '(');
            return text.Substring(0,opIndex);
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
            
            while (text.StartsWith("(") && text.EndsWith(")") && getCloseCharIndex(text, ')') == text.Length-1) {
                return getParenthis(text.Substring(text.IndexOf('(') + 1, getCloseCharIndex(text,')') - text.IndexOf('(') - 1), max-1, recCall+1);
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
                preparseLine(getCodeBlock(text+"}"));
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
            foreach(char c in text)
            {
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
                else if (c == '<' && d == c )
                {
                    if (indent == 0)
                    {
                        returnVal = index;
                        indent++;
                    }
                }
                else if (c == '>' && d == '<')
                {
                    indent--;
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
                else if (c == '>' && d == c)
                {
                    if (indent == 1)
                        return index;
                    else
                        indent--;
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

            foreach (string key in enums.Keys) {
                if (text.ToLower().StartsWith(key+" "))
                {
                    return key;
                }
            }
            return null;
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
                lst.Add(new ImpliciteVar("$val", Type.STRING, empty.Substring(1, empty.Length-2)));
            }

            return lst;
        }
        public static bool containEnum(string text)
        {
            return getEnum(text) != null;
        }

        public static string getStruct(string text)
        {
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            text = text.Replace("{", " ")+" ";
            if (text.Contains("<"))
            {
                if (!structs.ContainsKey(text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]").ToLower()))
                {
                    string generics = text.Substring(text.IndexOf("<") + 1, getCloseCharIndex(text,'>') - text.IndexOf("<") - 1);
                    string name = smartEmpty(text.Substring(0, text.IndexOf("<"))).ToLower();
                    
                    structs[name].createGeneric(text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<","[").Replace(">", "]"),
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
                if (text.ToLower().StartsWith(key.ToLower() + " ") || text.ToLower().StartsWith(key.ToLower() + "["))
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
        public static string offuscationMapAdd(string text)
        {
            if (!compilerSetting.offuscate)
                return text;

            if (offuscationMap.ContainsKey(text))
                return offuscationMap[text];

            string rText = "";
            foreach(char ch in text.Reverse())
            {
                rText += ch;
            }
            long hash = text.GetHashCode() + rText.GetHashCode()*(long)(int.MaxValue);
            long c = Math.Abs(hash % pow64[10]);
            
            string map = getMap(c);

            if (offuscationSet.Contains(map))
            {
                return offuscationMapAdd(text + "'");
            }

            offuscationSet.Add(map);
            offuscationMap.Add(text, map);
            return map;
        }
        public static string offuscationMapAdd(string text, string forced)
        {
            if (!compilerSetting.offuscate)
                return text;

            if (offuscationMap.ContainsKey(text))
                return offuscationMap[text];

            if (offuscationSet.Contains(forced))
            {
                return offuscationMapAdd(text + "_");
            }

            offuscationSet.Add(forced);
            offuscationMap.Add(text, forced);
            return forced;
        }
        public static string getMap(long c)
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

        public static void ConstCreate()
        {
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
                            context.GoTo(Project.ToLower()+".const");
                        
                        var.wasAdded = true;
                        var.isConst = false;
                        change = true;

                        try
                        {
                            loadFile.AddStartLine(eval(var.constValue, var, var.type));
                        }
                        catch(Exception e)
                        {
                            throw new Exception("Exception while calculating const value: " + e.ToString());
                        }
                        var.isConst = true;
                    }
                }
            } while (change);
        }
        public static void ScoreboardCreate()
        {
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
        }
        public static void FunctionCreate()
        {
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
                    if ((files[fi].UnparsedFunctionFile && !files[fi].notUsed) || !compilerSetting.removeUselessFile)
                    {
                        files[fi].Compile();
                        changed = true;
                    }
                    fi++;
                }
            }
        }
        private static void StringPoolCreate()
        {
            int i = 0;
            foreach (string s in stringSet)
            {
                Variable mux = GetVariable("__multiplex__.sstring.__strSelector__");
                stringPool.AddLine("execute if score " + mux.scoreboard() + " matches " + i.ToString() + " run summon minecraft:area_effect_cloud ~ ~ ~ { CustomName: \"\\\"" + s + "\\\"\",Tags:[\"__str__\"]}");
                i++;
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

        public class Function
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
            public string privateContext;
            public bool isStructMethod = false;
            public int argNeeded = 0;
            public int maxArgNeeded = 0;

            public Function(string name, string gameName, File file)
            {
                this.name = name;
                this.gameName = gameName;
                this.file = file;
                this.package = currentPackage;

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
                        var.LinkCopyTo(v);
                    }
                    foreach (Variable var in outputs)
                    {
                        var.LinkCopyTo(v);
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
        public class Structure
        {
            public string name;

            public List<Variable> fields = new List<Variable>();
            public List<Function> methods = new List<Function>();
            public List<string> methodsName = new List<string>();
            public List<string> generic = new List<string>();
            public Dictionary<string,string> compField = new Dictionary<string, string>();
            public Dictionary<string, string> typeMapContext = new Dictionary<string, string>();

            public string package;
            public string structureContext = context.GetVar();
            public bool isGeneric;
            public Structure parent;
            public bool isLazy;
            public bool isClass;
            public File classInitBase;

            public File genericFile = new File("____","");
            public File initBase = new File("", "");
            private Variable representative;

            public Structure(string name, Structure parent)
            {
                this.name = name;
                this.parent = parent;
                this.package = currentPackage;
                if (parent != null)
                {
                    foreach(Variable v in parent.fields)
                    {
                        string varName = context.toInternal(context.GetVar() + v.name);
                        Variable variable = new Variable(v.name, varName, v.type, v.entity, v.def);
                        
                        variable.isConst = v.isConst;
                        AddVariable(varName, variable);
                        if (v.type == Type.ENUM)
                            variable.SetEnum(v.enums);

                        fields.Add(variable);
                    }
                    foreach(Function f in parent.methods)
                    {
                        Function newFun = f.CopyTo(f.gameName, f.name, f.file);
                        methods.Add(newFun);
                    }
                }
                if (typeMaps.Count > 0)
                {
                    foreach (string key in typeMaps.Peek().Keys)
                    {
                        typeMapContext.Add(key, typeMaps.Peek()[key]);
                    }
                }
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
                if (!element.isVirtual && !element.isOverride)
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
            public void generateFunction(Function fun, Variable varOwner, string v, string cont)
            {
                string cont2 = context.GetVar();
                string funcName = fun.name;
                string contextStruct = null;
                if (isClass)
                    contextStruct = representative.gameName;

                File fFile = new File(context.GetFile() + funcName);

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
                    loadFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                    if (callStackDisplay)
                        callTrace += "\"load\"->\"" + function.gameName + "\"\n";
                }
                if (fun.isTicking)
                {
                    mainFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
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
                    AddToFunctionTag(function, tag);
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
                    if (arg.type == Type.FUNCTION)
                    {
                        int j = 0;
                        foreach (Argument s in arg.args)
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
                        foreach (Variable s in arg.outputs)
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
                        fFile.parsed.Add("__class_pointer__ #= "+ varOwner.gameName);
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
                }
                foreach (string line in fun.file.parsed)
                {
                    if (thisReg2.Match(line).Success)
                    {
                        if (compVal.Count > 0 && !(dualCompVar.Match(line).Success && structInstCompVar))
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
                        if (compVal.Count > 0 && !(dualCompVar.Match(line).Success && structInstCompVar))
                        {
                            fFile.parsed.Add(compVarReplace(thisReg.Replace(line, context.GetVar())));
                        }
                        else
                        {
                            fFile.parsed.Add(thisReg.Replace(line, context.GetVar()));
                        }
                    }
                }
                if (isClass)
                {
                    fFile.parsed.Add("}");
                }
            }
            public string generate(string v, bool entity, Variable varOwner, string instArg = null, bool parentClass = false)
            {
                if (isClass && parent == null && name != "object")
                {
                    parent = structs["object"];
                }
                if (isClass && !parentClass)
                {
                    if (representative == null)
                    {
                        Context c = context;
                        context = new Context(Project, new File("",""));

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
                            GetVariableByName(representative.gameName + "." + strVar.name).LinkCopyTo(context.GetVar() + v+"."+ strVar.name);
                        }
                        catch
                        {
                            
                        }
                    }
                }

                string cont = context.GetVar();
                string output = "";

                thisDef.Push(context.GetVar() + v+".");
                context.Sub(v, new File("", ""));
                
                context.currentFile().notUsed = true;
                if (!isClass || parentClass){
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
                compVal[compVal.Count - 1].Add("$this", varOwner.uuid);
                compVal[compVal.Count - 1].Add("$this.lower", varOwner.uuid.ToLower());
                compVal[compVal.Count - 1].Add("$this.upper", varOwner.uuid.ToUpper());
                compVal[compVal.Count - 1].Add("$this.enums", varOwner.enums);
                compVal[compVal.Count - 1].Add("$this.type", varOwner.GetTypeString());
                compVal[compVal.Count - 1].Add("$this.name", varOwner.gameName);
                compVal[compVal.Count - 1].Add("$this.scoreboard", varOwner.scoreboard());
                compVal[compVal.Count - 1].Add("$this.scoreboardname", varOwner.scoreboard().Split(' ')[1]);

                typeMaps.Push(new Dictionary<string, string>());
                foreach (string key in typeMapContext.Keys)
                {
                    typeMaps.Peek().Add(key, typeMapContext[key]);
                }
                foreach (string c in compField.Keys)
                {
                    compVal[compVal.Count - 1].Add(c, compField[c]);
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
                            generateFunction(fun, varOwner, v, cont);
                        }
                    }

                    structCompVarPointer = compVal[compVal.Count - 1];

                    context.Parent();
                    if (isClass)
                        Structure.DerefObject(varOwner);
                    output += parseLine(v+".__init__" + instArg);
                    context.Sub(v, new File("", ""));
                    compVal[compVal.Count - 1] = structCompVarPointer;

                    structInstCompVar = false;
                }

                foreach (Function fun in methods)
                {
                    if (fun.name != "__init__" || instArg == null)
                    {
                        generateFunction(fun, varOwner, v, cont);
                    }
                }
                isInStructMethod = prev;
                int ci=0;
                
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
        public class Variable
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

            private Variable() { }

            public Variable(string name, string gameName, Type type, bool entity = false, string def="dummy")
            {
                this.name = name;
                this.gameName = gameName;
                this.type = type;
                this.entity = entity;
                this.def = def;

                if (structStack.Count > 0 && !isInStaticMethod)
                {
                    isStructureVar = true;
                }

                if (entity && type == Type.STRUCT && def == "__class_id__")
                {
                    uuid = offuscationMapAdd(gameName);
                    score = "@s " + uuid;

                    scoreboardObj = new Scoreboard(uuid, "dummy");
                }
                else if (entity && type != Type.STRUCT)
                {
                    uuid = offuscationMapAdd(gameName);
                    score = "@s " + uuid;
                    
                    scoreboardObj = new Scoreboard(uuid, def);
                }
                else
                {
                    uuid = offuscationMapAdd(gameName);
                    score = uuid + " " + (isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue);

                    scoreboardObj = scoreboards[isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue];
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
                    score = "@s "+offuscationMapAdd(gameName);
                    return score;
                }
                else
                {
                    score = offuscationMapAdd(gameName) + " "+(isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue);
                    return score;
                }
            }

            public void CreateArray()
            {
                string prefix = "";
                if (isPrivate) { prefix += "private "; }
                if (isStatic) { prefix += "static "; }
                if (isConst) { prefix += "const "; }

                parseLine(prefix+"int " + name + ".length = "+arraySize.ToString());

                for (int i = 0; i < arraySize; i++)
                {
                    parseLine(prefix+typeArray + " " + name + "." + i.ToString());
                }

                context.Sub(name, new File("", "", ""));
                parseLine("def lazy @__numerical_only__ set(int $a," + typeArray + " $b){");
                context.currentFile().addParsedLine("\\compiler\\" + name + ".$a = $b");
                context.currentFile().Close();
                isInLazyCompile -= 1;

                parseLine("def lazy @__numerical_only__ get(int $a):" + typeArray + "{");
                context.currentFile().addParsedLine("\\compiler\\return(" + name + ".$a)");
                context.currentFile().Close();
                isInLazyCompile -= 1;

                preparseLine("def get(int index):" + typeArray + "{");
                preparseLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                preparseLine("if(index==$_i){");
                preparseLine("return(" + name + ".$_i)");
                preparseLine("}");
                preparseLine("}");
                preparseLine("}");


                preparseLine("def set(int index, " + typeArray + " value){");
                preparseLine("forgenerate($_i,0," + (arraySize - 1).ToString() + "){");
                preparseLine("if(index==$_i){");
                preparseLine(name + ".$_i = value");
                preparseLine("}");
                preparseLine("}");
                preparseLine("}");

                context.Parent();
            }

            public Variable CopyTo(string newName, string v, bool entity, bool copiedIsPrivate = false)
            {
                Variable variable = new Variable(this.name, newName, this.type, entity, this.def);
                variable.isConst = this.isConst;
                variable.isPrivate = copiedIsPrivate || isPrivate;
                variable.privateContext = context.GetVar();

                AddVariable(newName, variable);
                
                if (this.type == Type.ENUM)
                    variable.SetEnum(this.enums);

                if (this.type == Type.ARRAY)
                {
                    variable.arraySize = arraySize;
                    variable.CreateArray();
                }

                if (this.type == Type.STRUCT)
                {
                    variable.SetEnum(this.enums);
                    if (this.enums != name)
                    {
                        structs[this.enums].generate(this.name, entity, variable);
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
                var.score = score.Replace("@s",entitySelector);
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
                        if (i < args.Count-1)
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

            public void use(){
                if (!entity)
                {
                    scoreboardObj = scoreboards[isConst ? compilerSetting.scoreboardConst : compilerSetting.scoreboardValue];
                }

                wasUsed = true;
                if (parent!=null)
                    parent.use();
                scoreboardObj.use();
            }
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

                        preparseLine("def __getEnumField__." + parent.name+"." + name + "(int value):"+type+"{");
                        multiplexer = GetVariableByName(funcName.ToLower() + ".value");
                        output = GetVariableByName(funcName.ToLower() + ".ret_0");
                        preparseLine("}");

                        functionGet = GetFunction(context.GetFunctionName(funcName),new string[] { "0" });
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

            public Enum(string name,string[] values, bool final = false)
            {
                this.name = name;
                foreach (string value in values)
                {
                    Add(value);
                }
                this.final = final;
            }
            public Enum(string name,string[] fields, string[] values, bool final = false)
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
                    this.fields.Add(new EnumField(this,type, fname, defaultVal));
                }
                foreach (string value in values)
                {
                    Add(value);
                }
                this.final = final;
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
                foreach (string field in fields) {
                    if (field.Contains("="))
                    {
                        string[] subField = smartSplit(field,'=');
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

                foreach(EnumField enumField in this.fields)
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
                context.Sub(name, new File("",""));
                foreach(EnumField field in fields)
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

        public class Switch: Component
        {
            List<Case> casesUnit = new List<Case>();
            List<Case> casesRange = new List<Case>();
            Type type;
            public Variable variable;
            Variable copyFrom;
            int copyFromAccessTime;
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

            public Switch(string text,int id)
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
            public Switch(string text,string[] sizes,int id)
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
                type = variable.type;
            }

            public void CreateVariable(string text)
            {
                type = getExprType(text);

                string name = "_s." + id.ToString();
                if (type == Type.STRUCT)
                    parseLine(GetVariableByName(text).enums.ToLower() + " " + name);
                else if (type == Type.ENUM)
                    parseLine(getExprEnum(text).ToLower() + " " + name);
                else
                    parseLine(type.ToString().ToLower() + " " + name);

                copyFrom = GetVariableByName(smartExtract(text), true);
                if (copyFrom != null)
                {
                    copyFromAccessTime = copyFrom.accessTime;
                }

                variable = GetVariableByName(name);
            }
            public string Compile() {
                return Compile(true);
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
                string[] tail = new string[Math.Max(sizes.Length-1,0)];
                for (int i = 1; i < sizes.Length; i++)
                {
                    tail[i - 1] = sizes[i];
                }
                
                casesUnit.Sort((a, b) => a.value.CompareTo(b.value));
                if (casesUnit.Count > treeBottom)
                {
                    string text = "";
                    if (createVariable)
                        text += eval(this.text, variable, type, "=");
                    for (int i = 0; i < Math.Ceiling((casesUnit.Count*1.0)/ treeBottom); i++)
                    {
                        string contName = "__splitted_" + i.ToString();
                        string funcName = (context.GetFun() + contName);
                        string subName = funcName.Substring(funcName.IndexOf(":") + 1, funcName.Length - funcName.IndexOf(":") - 1);
                        File f = new File(subName);
                        files.Add(f);
                        context.currentFile().addChild(f);

                        int iMin = i * treeBottom;
                        int iMax = Math.Min((i + 1) * treeBottom-1, casesUnit.Count-1);

                        Switch s = new Switch(variable, subList(casesUnit,iMin, iMax), tail);
                        context.Sub(contName, f);
                        f.AddLine(s.Compile(false));
                        context.Parent();
                        string cmd = "function " + funcName + '\n';
                        text += getCondition(variable.gameName + "==" + 
                            casesUnit[iMin].value.ToString()+".."+
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
                        text = eval(this.text, variable, type, "=");
                    foreach (Case c in casesUnit)
                    {
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
                if (type == Type.ENUM)
                {
                    if (getEnum(text) != null && enums[getEnum(text)].valuesName.Contains(smartEmpty(cond).ToLower()))
                    {
                        casesUnit.Add(new Case(enums[getEnum(text)].valuesName.IndexOf(smartEmpty(cond).ToLower()), cmd));
                        return casesUnit[casesUnit.Count - 1];
                    }
                    else
                    {
                        casesRange.Add(new Case(cond, cmd));
                        return casesRange[casesRange.Count - 1];
                    }
                }
                else if (type == Type.INT)
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
                else if (type == Type.FLOAT)
                {
                    if (float.TryParse(cond, out float _))
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

            public static void INIT()
            {
                SwitchNumber = new Dictionary<string, int>();
                EvalNumber = new Dictionary<string, int>();
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
                return base.ToString();
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

            public Predicate(string name, string[] args, File file) {
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
                        text = text.Replace(args[i], args2[i].Replace("\"","\\\""));
                    }
                    string filename = name.Substring(name.IndexOf(":") + 1, name.Length - name.IndexOf(":") - 1);
                    File f = new File("predicates/" + filename +"_"+args2.Length.ToString()+ "_" + generated.Count(), "","json");
                    f.AddLine(text);
                    f.use();
                    generated.Add(arg);
                    files.Add(f);
                }
                return name + "_" + args2.Length.ToString() + "_" + generated.IndexOf(arg);
            }
        }

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
            public Dictionary<string, string> forcedOffuscation = new Dictionary<string, string>();

            public CompilerSetting()
            {
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
                    return newSetting;
                }
            }
        }

        public class Argument: Variable
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
            CLASS,
            STRING,
            VOID,
            ARRAY,
            JSON,
            PARAMS,
            RANGE,
            DEFINE
        }
        public class File
        {
            public string name;
            public string content;
            public string abstractContent;
            public string ending;
            public string start = "";
            public StringBuilder scoreboardDef = new StringBuilder();
            public List<string> parsed = new List<string>();
            public bool isLazy;
            public string type;
            public int lineCount=0;
            public bool cantMergeWith;
            public string var;
            public float min;
            public float max;
            public float step;
            public string enumGen;
            private int genIndex;
            private int genAmount;
            public bool UnparsedFunctionFile;
            public string UnparsedFunctionFileContext;
            public bool resourcespack;
            public Dictionary<string, string> typeMapContext = new Dictionary<string, string>();

            public Function function;
            public Switch.Case switchcase;

            public bool notUsed = false;
            public List<File> childs = new List<File>();
            public File parent;

            public File(string name, string content = "", string type="")
            {
                this.name = name;
                this.content = content;
                this.type = type;

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

                if (!structInstCompVar)
                {
                    compVal.Add(new Dictionary<string, string>());
                }

                if (file != null)
                {
                    string fullName = value;
                    value = value.Replace(file,"");
                    if (fullName.EndsWith(".png")) {
                        Image img = Bitmap.FromFile(fullName);
                        compVal[compVal.Count - 1].Add(var + ".width", img.Width.ToString());
                        compVal[compVal.Count - 1].Add(var + ".height", img.Height.ToString());
                    }
                }

                foreach (string l in parsed)
                {
                    string line = l;
                    if (enums != null) {
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
                        for (int i = argget.Length-1; i >= 0; i--)
                        {
                            line = line.Replace(var + "." + i.ToString(), smartExtract(argget[i]));
                        }
                        line = line.Replace(var + ".count", argget.Length.ToString());
                    }

                    line = line.Replace(var + ".index", genIndex.ToString())
                        .Replace(var + ".length", genAmount.ToString())
                        .Replace(var, value);
                    
                    preparseLine(line);
                }

                if (compVal.Count > 0 && !structInstCompVar)
                {
                    compVal.RemoveAt(compVal.Count - 1);
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
                        generate(value.Replace("\\", "/"), null, (projectFolder + "/resourcespack/" + extractString(args[0])+"/").Replace("\\", "/"));
                    }
                }
                else if (enumGen != null && functionTags.ContainsKey(enumGen.Replace("@","")))
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
                    for (float i = min; i != max + step; i += step)
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
                    string tmp = f.content.Replace("function "+function.gameName, content);
                    f.content = tmp;
                    files.Remove(this);
                }
                if (type == "if" && LastCond.wasAlwayTrue && lineCount > 1)
                {
                    File f = context.currentFile();
                    string tmp = f.content + content;
                    f.content = tmp;
                    files.Remove(this);
                }
                if (type == "lazyfunctioncall")
                {
                    File f = context.currentFile();
                    string tmp = f.content + content;
                    f.content = tmp;
                    files.Remove(this);
                }
                if ((type == "if" || (type == "with" && !cantMergeWith) || type == "at") && lineCount == 1 && !content.StartsWith("#"))
                {
                    if (LastCond.wasAlwayTrue)
                    {
                        File f = context.currentFile();
                        string tmp = f.content + content;
                        f.content = tmp;
                        files.Remove(this);
                    }
                    else
                    {
                        File f = context.currentFile();
                        string tmp = f.content.Substring(0, f.content.LastIndexOf(' '));
                        tmp = tmp.Substring(0, tmp.LastIndexOf(' '));
                        tmp += " " + content;
                        f.content = tmp;
                        files.Remove(this);
                    }
                }
                if (type == "case" && lineCount == 1 && !content.StartsWith("#"))
                {
                    switchcase.cmd = content;
                    files.Remove(this);
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

                    files.Remove(this);
                }
                if (type == "forgenerate")
                {
                    File f = context.currentFile();
                    f.AddLine(content);
                    files.Remove(this);
                }
                if (type == "withContext")
                {
                    File f = context.currentFile();
                    string tmp = f.content;
                    tmp += content;
                    f.content = tmp;
                    files.Remove(this);
                    context.popImpliciteVar();
                }
                
                lineCount = content.Split('\n').Length - 1;
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
                if (!(file.function != null && (file.function.isLoading || file.function.isTicking || file.function.tags.Count > 0 || file.function.isHelper)))
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
                    if (!f.notUsed && !(f.function != null && (f.function.isLoading || f.function.isTicking || f.function.tags.Count > 0 || f.function.isHelper)))
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

                if (function != null && function.args.Count == 1)
                {
                    switches.Push(new Switch(function.args[0], -1));
                }
                int i = 0;
                foreach (string line in parsed)
                {
                    currentLine = i;
                    preparseLine(line);
                    i++;
                }

                if (function != null && function.args.Count == 1)
                {
                    Switch s = switches.Pop();
                    context.currentFile().AddLine(s.Compile());
                }

                typeMaps.Pop();
                context.currentFile().Close();
                adjPackage.Pop();
                adjPackage.Pop();
                UnparsedFunctionFile = false;
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
        
        public class Context
        {
            public List<string> directories = new List<string>();
            public List<File> files = new List<File>();
            public Dictionary<string, File> fileDir = new Dictionary<string, File>();
            public List<string> import = new List<string>();
            public string fakeContext;
            public List<List<ImpliciteVar>> impliciteVars = new List<List<ImpliciteVar>>();
            
            public Context(string project, File f)
            {
                directories.Add(project);
                files.Add(f);

                fileDir[GetFile()] = f;
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
                    compVal.Add(new Dictionary<string, string>());
                }
            }
            public void Parent()
            {
                if (directories.Count > 1)
                {
                    directories.RemoveAt(directories.Count - 1);
                    files.RemoveAt(files.Count - 1);
                    if (compVal.Count > 0 && !structInstCompVar)
                    {
                        compVal.RemoveAt(compVal.Count - 1);
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
            public File GoTo(string path)
            {
                GoRoot();
                File fOut=null;
                foreach(string p in path.Substring(path.IndexOf('.'), path.LastIndexOf('.')-path.IndexOf('.')).Split('.'))
                {
                    if (p != "")
                    {
                        string fullName = context.GetFun() + p;
                        string subName = fullName.Substring(fullName.IndexOf(":") + 1, fullName.Length - fullName.IndexOf(":") - 1);
                        File f = new File(subName, "", "");
                        Compiler.files.Add(f);
                        Sub(p, f);
                        fOut = f;
                    }
                }
                return fOut;
            }
            public File Package(string package)
            {
                while(directories.Count > 1)
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
                string output = directories[0].ToLower() + ":";

                for (int i = 1; i < directories.Count; i++)
                {
                    output += directories[i].ToLower() + "/";
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
                for (int i = 0; i < v.Length-1; i++)
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
                stringBuilder.Append(v[v.Length-1]);
                return stringBuilder.ToString();
            }

            public string GetFunctionName(string func, bool safe = false, bool bottleneck = false)
            {
                func = toInternal(smartEmpty(func).ToLower());
                if (functions.ContainsKey(func))
                {
                    return func;
                }
                string dir = "";
                string output = null;

                
                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string var = GetFunctionName(pack + "." + func, true, true);
                        if (var != null)
                        {
                            return var;
                        }
                    }
                }

                foreach (string co in directories)
                {
                    dir += co.ToLower() + ".";
                    if (functions.ContainsKey(dir+func))
                    {
                        output = dir+func;
                    }
                }
                if (output != null)
                    return output;

                if (safe)
                    return null;
                throw new Exception("UNKNOW FUNCTION (" + dir+func+ ")");
            }

            public bool IsFunction(string func, bool bottleneck = false)
            {
                if (!bottleneck)
                    func = toInternal(smartEmpty(func).ToLower());
                
                string var = GetVariable(func, true);
                if (var != null)
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

                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        bool val = IsFunction(pack + "." + func, true);
                        if (val)
                        {
                            return val;
                        }
                    }
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

                return false;
            }

            public string GetVariable(string func, bool safe = false, bool bottleneck = false, int recCall = 0, bool debug = false)
            {
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
                    return GetVariable(getLazyVal(func), safe, bottleneck, recCall+1);
                }

                if (variables.ContainsKey(func))
                {
                    return func;
                }
                
                string adj = "";
                
                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string var = GetVariable(pack + "." + func, true, true, recCall + 1);
                        
                        if (var != null)
                        {
                            return var;
                        }
                        adj += pack + ", ";
                    }
                }

                string dir = "";
                string output = null;
                foreach (string co in directories)
                {
                    dir += co + ".";

                    if (variables.ContainsKey(dir + func))
                    {
                        output = dir + func;
                    }
                }
                
                if (output != null)
                    return output;

                if (!safe)
                    throw new Exception("UNKNOW Variable (" + dir + "/" + func + ") with package: " + adj);
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
                    return value.Split('.')[0];
                }
                if (value.Contains('.'))
                {
                    string ent = value.Split('.')[0];
                    return (Compiler.GetVariableByName(ent)).scoreboard().Split(' ')[0];
                }
                else
                    throw new Exception("UNKNOW Entity (" + value + ")");
            }

            public string ConvertEntity(string value, bool single = true)
            {
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
                if (value.Contains("@"))
                {
                    if (!Core.isValidSelector(value))
                        throw new Exception("Invalid Selctor "+value);
                    Match m = entitytagsReplaceReg.Match(value);
                    if (m.Success)
                    {
                        value = regReplace(value, m, m.Value.Replace("=#", "=#" + Project));
                    }
                    m = entitytagsReplaceReg2.Match(value);
                    if (m.Success)
                    {
                        value = regReplace(value, m, m.Value.Replace("=!#", "=!#" + Project));
                    }
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
                if (smartEmpty(value).StartsWith("@"))
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
                impliciteVars.RemoveAt(impliciteVars.Count-1);
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
