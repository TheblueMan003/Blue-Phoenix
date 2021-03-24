using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public class Compiler
    {
        public static Dictionary<string, List<Function>> functions;
        public static HashSet<Function> abstractFunctionsNeeded;
        public static Dictionary<string, Variable> variables;
        public static Dictionary<string, Enum> enums;
        public static Dictionary<string, Structure> structs;
        public static Dictionary<string, Class> classes;
        public static Dictionary<string, TagsList> blockTags;
        public static HashSet<string> functionTags;


        private static Dictionary<string, string> structMap;
        public static Dictionary<string, string> offuscationMap;
        private static Dictionary<string, string> packageMap;
        private static List<Dictionary<string, string>> lazyEvalVar;
        private static List<Dictionary<string,string>> compVal;
        private static Dictionary<string, List<Function>> functDelegated;
        private static Dictionary<string, List<File>> functDelegatedFile;
        private static Dictionary<string, string> selector;
        private static Dictionary<string, string> resourceFiles;
        public static List<string> packages;
        private static HashSet<string> offuscationSet;
        private static HashSet<string> imported;

        private static Stack<Variable> switches;
        private static List<string> stringSet;
        private static Stack<Structure> structStack;
        private static Stack<int> condIDStack;
        private static Stack<Class> classDefStack;
        private static List<File> files;
        private static List<File> jsonFiles;
        private static Dictionary<int, Variable> constants;
        public static Context context;
        private static File loadFile;
        private static File mainFile;
        private static int condID;
        private static int whileID;
        private static int switchID;
        private static int tmpID;
        private static int lambdaID;
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
        public static Debug GobalDebug;
        public static int subDir = 256;
        public static string Project;
        public static int autoIndented = 0;
        public static bool inGenericStruct;
        private static Dictionary<string, string> typeMaps = new Dictionary<string, string>();
        private static string currentPackage;
        private static Stack<string> adjPackage;
        private static string functionDesc = "";
        private static bool isInFunctionDesc;
        private static Stack<int> LastConds;
        private static int LastCond;
        private static string projectFolder;
        private static long[] pow64 = new long[11];
        private static string dirVar;
        private static bool OffuscateNeed;
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
        private static Regex getReg = new Regex("\\w*\\s*=[ a-z\\.A-Z0-9]*\\[.*\\]");
        private static Regex oppReg = new Regex(@"[a-zA-Z0-9\._]+\[.*\]\s*[+\-/\*%]=");
        private static Regex setReg = new Regex("\\[.*\\]\\s*=\\s*.*");
        private static Regex enumsDesugarReg = new Regex(@"(?s)(enum\s+\w+\s*(\([a-zA-Z0-9 ,_=:\.""'!\[\]]*\))?\s*\{(\s*\w*(\([a-zA-Z0-9 ,_=:\.""'!:\[\]\(\)]*\))?,?\s*)*\}|enum\s+\w+\s*=\s*(\([a-zA-Z0-9 ,_=""'\[\]!:\(\)]*\))?\s*\{(\s*\w*(\([a-zA-Z0-9 ,_=:\.""'\[\]!\(\)]*\))?,?\s*)*\})");
        private static Regex blocktagsDesugarReg = new Regex(@"(?s)(blocktags\s+\w+\s*\{(\s*[^\}]+,?\s*)*\}|blocktags\s+\w+\s*=\s*\{(\s*[^\}]+,?\s*)*\})");
        private static Regex ifsDesugarReg = new Regex(@"(?s)^(if\s*\(.*\)\{.*\}\s*else)|(if\s*\(.*\).*\s*else)");
        private static Regex funArgTypeReg = new Regex(@"^([@\w\.]*\s*(<\(?\w*\)?,?\(?\w*\)?>)?(\[\d+\])?)*\(");
        private static Regex arraySizeReg = new Regex(@"(?:\[)\d+(?:\])");
        private static Regex arrayTypeReg = new Regex(@"\b\w+(?:\[)");
        private static Regex opReg = new Regex(@"((#=)|(\+=)|(\-=)|(\*=)|(/=)|(\%=)|(\&=)|(\|=)|(\^=)|(=))");
        private static Regex elsifReg = new Regex(@"^el?s?e?\s*ifs?\s?\(");
        private static Regex ifReg = new Regex(@"^if\s*\(");
        private static Regex jsonFileReg = new Regex(@"^jsonfile\s+[\w\\/\-]+\{?");
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
        private static Regex varInstReg = new Regex(@"^[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])?\s+[\w\$\.]+\s*");
        private static Regex compVarInstReg = new Regex(@"^[\w\.]+(<\(?[@\w]*\)?,?\(?\w*\)?>)?(\[\w+\])?\s+\$[\w\$\.]+\s*=");
        private static Regex elseReg = new Regex(@"^else\s*");
        private static Regex regEval = new Regex(@"\$eval\([0-9a-zA-Z\-\+\*/% \.]*\)");
        private static Regex regEval2 = new Regex(@"\$eval\([0-9a-zA-Z\-\+\*/% \.\(\)\s]*\)eval\$");
        private static Regex forgenInLineReg = new Regex(@"forgenerate\([^\(\)]*\)\{[^\{\}]*\}");
        private static Regex dualCompVar = new Regex(@"^\$[\w\.\$]+\s*=\s*\$?[\w\.\$]+");
        #endregion

        private static int isInLazyCompile;
        private static CompilerSetting compilerSetting = new CompilerSetting();

        private Compiler() { }

        public static List<File> compile(CompilerCore core,string project, List<File> codes, List<File> resources, Debug debug,
                                            bool offuscated, ProjectVersion version, string pctFolder)
        {
            callTrace = "digraph "+project+" {\nmain\nload\nhelper\n";
            Core = core;
            projectVersion = version;
            for (int i = 0; i < 11; i++)
            {
                pow64[i] = IntPow(alphabet.Length, i);
            }
            dirVar = project.Substring(0, Math.Min(4, project.Length));
            OffuscateNeed = offuscated;
            offuscationMap = new Dictionary<string, string>();
            functionTags = new HashSet<string>();
            projectFolder = pctFolder;
            GobalDebug = debug;
            Project = project;
            whileID = 0;
            condID = 0;
            switchID = -1;
            lambdaID = 0;
            funcDef = new List<string>();

            try
            {
                functions = new Dictionary<string, List<Function>>();
                variables = new Dictionary<string, Variable>();
                enums = new Dictionary<string, Enum>();
                structs = new Dictionary<string, Structure>();
                classes = new Dictionary<string, Class>();
                switches = new Stack<Variable>();
                constants = new Dictionary<int, Variable>();
                structMap = new Dictionary<string, string>();
                functDelegated = new Dictionary<string, List<Function>>();
                functDelegatedFile = new Dictionary<string, List<File>>();
                offuscationSet = new HashSet<string>();
                imported = new HashSet<string>();
                thisDef = new Stack<string>();
                files = new List<File>();
                jsonFiles = new List<File>();
                stringSet = new List<string>();
                packages = new List<string>();
                adjPackage = new Stack<string>();
                structStack = new Stack<Structure>();
                blockTags = new Dictionary<string, TagsList>();
                resourceFiles = new Dictionary<string, string>();
                foreach(var f in resources)
                {
                    resourceFiles.Add(f.name, f.content);
                    GobalDebug("Added resource: "+f.name, Color.Green);
                }

                isInStructMethod = false;
                isInStaticMethod = false;
                forcedUnsed = false;
                structMethodFile = null;
                loadFile = new File("load", "");
                loadFile.AddScoreboardDefLine(Core.LoadBase());
                
                files.Add(loadFile);
                mainFile = new File("main", "");
                mainFile.AddScoreboardDefLine(Core.MainBase());
                files.Add(mainFile);
                /*
                preparseLine("int __cls_pointer__");
                preparseLine("INT __ins_pointer__");
                */
                stringInit();
                foreach (File f in codes)
                {
                    compileFile(f);
                }
                
                bool changed = true;
                while (changed)
                {
                    int fi = 0;
                    changed = false;
                    while (fi < files.Count)
                    {
                        if (files[fi].UnparsedFunctionFile && !files[fi].notUsed)
                        {
                            files[fi].Compile();
                            changed = true;
                        }
                        fi++;
                    }
                }
                generateStringPool();
                loadFile.Close();
                mainFile.Close();

                updateFormater();

                List<File> returnFiles = new List<File>();
                int lineCount=0;
                
                foreach(File f in files)
                {
                    //returnFiles.Add(f);
                    if (!f.notUsed && !f.isLazy && f.lineCount > 0)
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
                returnFiles.AddRange(jsonFiles);
                callTrace += "}";
                GobalDebug("Datapack contains: " + returnFiles.Count.ToString() + " files & "+lineCount.ToString()+" lines.", Color.LimeGreen);
                
                return returnFiles;
            }
            catch (Exception e)
            {
                throw new Exception("Error in " + currentFile + " on line " + currentLine.ToString() + ": " + e.ToString());
            }
        }
        public static string getStackCall(CompilerCore core, string project, List<File> codes, List<File> resources, Debug debug, bool offuscated, ProjectVersion version, string pctFolder)
        {
            compile(core, project, codes, resources, debug, offuscated, projectVersion, pctFolder);
            return callTrace;
        }

        private static void compileFile(File f, bool notUsed = false, bool import = false)
        {
            structStack = new Stack<Structure>();
            packageMap = new Dictionary<string, string>();
            condIDStack = new Stack<int>();
            LastConds = new Stack<int>();
            lazyOutput = new Stack<List<Variable>>();
            lazyCall = new Stack<Function>();
            lazyEvalVar = new List<Dictionary<string, string>>();
            compVal = new List<Dictionary<string,string>>();
            LastCond = -1;
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
            
            string[] lines = desugar(f.content).Split('\n');

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
                preparseLine((isMcFunction?"/":"")+line2);
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
            GobalDebug("Succefully Compiled " + currentFile + " (" + (currentLine - 1).ToString() + " Lines)", Color.Lime);
        }

        public static void updateFormater()
        {
            Formatter.setEnum(new List<string>(enums.Keys));
            List<string> formEnums = new List<string>();
            
            foreach (Enum s in enums.Values)
            {
                foreach(Enum.EnumValue v in s.values)
                {
                    formEnums.Add(v.value);
                }
            }
            Formatter.setEnumValue(formEnums);
            Formatter.setStructs(new List<string>(structs.Keys));
            Formatter.setpackage(packages);
            Formatter.setTags(functionTags);
            Formatter.setDefWord(funcDef);
            Formatter.loadDict();
        }

        public static void preparseLine(string line2, File limit = null, bool lazyEval = false)
        {
            string line = line2;

            if (compVal.Count > 0 && !(dualCompVar.Match(line).Success && structInstCompVar)) {
                line = compVarReplace(line);
            }
            if (jsonIndent > 0)
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
                        /*
                        if (isInStructMethod)
                            structMethodFile.addParsedLine(line);*/

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
                        res = parseLine(line);
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
                if (jsonIndent > 0)
                {
                    return AddToJsonFile(text);
                }

                text = smartExtract(text);

                if (isInFunctionDesc)
                {
                    return instFuncDesc(text);
                }
                else if (text.StartsWith("//"))
                    return "#" + text.Substring(2, text.Length - 2);

                else if (text.StartsWith("/"))
                    return text.Substring(1, text.Length - 1);
                else if (text.StartsWith("import"))
                {
                    return import(text);
                }
                else if (text.StartsWith("using"))
                {
                    return instUsing(text);
                }
                else if (text.StartsWith("alias"))
                {
                    return instAlias(text);
                }
                else if (text.StartsWith("package"))
                {
                    return instPackage(text);
                }
                else if (CommandParser.canBeParse(text))
                {
                    return CommandParser.parse(text, context);
                }
                else if (text.Contains("\"\"\""))
                {
                    return instFuncDesc(text);
                }
                else if (text.StartsWith("{"))
                {
                    autoIndented = 0;
                    return parseLine(text.Substring(1, text.Length - 1));
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
                else if (jsonFileReg.Match(text).Success)
                {
                    return instJsonFile(text);
                }
                //condition
                else if (ifsReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instIf(arg, text, 1);
                }
                else if (ifReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instIf(arg, text);
                }
                else if (elsifReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instElseIf(arg, text);
                }
                else if (elseReg.Match(text).Success)
                {
                    return instElse(text);
                }
                //structure
                else if (text.StartsWith("struct "))
                {
                    return instStruct(text);
                }
                //class
                else if (text.StartsWith("class "))
                {
                    return instClass(text);
                }
                //switch
                else if (switchReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instSwitch(arg);
                }
                //case
                else if (caseReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instCase(arg);
                }
                //smart case
                else if (text.Contains("->") && switchID > -1)
                {
                    return instSmartCase(text);
                }
                //with
                else if (withReg.Match(text).Success)
                {
                    string[] args = getArgs(text);

                    return instWith(args, text);
                }
                //at
                else if (atReg.Match(text).Success)
                {
                    string[] args = getArgs(text);

                    return instAt(args, text);
                }
                //positioned
                else if (positonedReg.Match(text).Success)
                {
                    string args = getArg(text);

                    return instPositioned(args, text);
                }
                //aligned
                else if (alignReg.Match(text).Success)
                {
                    string args = getArg(text);

                    return instAligned(args, text);
                }
                //while
                else if (whileReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instWhile(arg, text);
                }
                //forgenerate
                else if (forgenerateReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instForGenerate(arg, text);
                }
                //for
                else if (forReg.Match(text).Success)
                {
                    string arg = getArg(text);

                    return instFor(arg, text);
                }
                //enum set
                else if (enumReg.Match(text).Success)
                {
                    return instEnum(text);
                }
                //enum file
                else if (enumFileReg.Match(text).Success)
                {
                    return instEnumFile(text);
                }
                //function def
                else if (funcReg.Matches(text).Count > 0)
                {
                    return instFunc(text);
                }
                //blocktag set
                else if (blocktagReg.Match(text).Success)
                {
                    return instBlockTag(text);
                }
                //comp int set
                else if (compVarInstReg.Match(text).Success || dualCompVar.Match(text).Success)
                {
                    return instCompilerVar(text);
                }
                //int set
                else if (varInstReg.Match(text).Success)
                {
                    return instVar(text);
                }
                //int add
                else if (smartContains(text, '='))
                {
                    return modVar(text);
                }
                //int add
                else if (text.Contains("++") || text.Contains("--"))
                {
                    return modVar(text.Replace("++", "+=1").Replace("--", "-=1"));
                }
                //function call
                else if (text.Contains("(") && text.Contains(")") || context.IsFunction(text))
                {
                    return functionEval(text);
                }
                else
                {
                    if (text != "" && !text.StartsWith("}"))
                    {
                        GobalDebug("Unparsed line:'" + text + "'", Color.Yellow);
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
                Regex reg = new Regex(@"\b" + key + "\b");
                if (key.Contains("$"))
                    reg = new Regex("\\" + key);

                Match match = reg.Match(line);
                while (match != null && dic[key] != key && match.Value != "" && match.Value != null)
                {
                    line = regReplace(line, match, dic[key]);
                    match = reg.Match(line);
                }
            }
            return line;
        }
        public static string desugar(string text)
        {
            text = text.Replace("$projectName", Project.ToLower()).Replace("$projectVersion", projectVersion.ToString());
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
                text = regReplace(text, match2, instInLineForgenerate(match2.Value));
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
            if (System.IO.File.Exists("lib/" + text + ".tbms"))
            {
                forcedUnsed = fu;
                string data = System.IO.File.ReadAllText("lib/" + text + ".tbms");
                ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);
                
                context.GoRoot();
                if (project.resources != null)
                {
                    foreach (var f in project.resources)
                    {
                        if (!resourceFiles.ContainsKey(f.name))
                            resourceFiles.Add(f.name, f.content);
                    }
                }

                foreach (var file in project.files)
                {
                    compileFile(new File(text+"."+file.name, file.content), fu, true);
                }

                imported.Add(text);
                forcedUnsed = false;
                return "";
            }
            if (System.IO.File.Exists(projectFolder+"/" + text + ".tbms"))
            {
                forcedUnsed = fu;
                string data = System.IO.File.ReadAllText(projectFolder+"/" + text + ".tbms");
                ProjectSave project = JsonConvert.DeserializeObject<ProjectSave>(data);

                context.GoRoot();

                if (project.resources != null)
                {
                    foreach (var f in project.resources)
                    {
                        if (!resourceFiles.ContainsKey(f.name))
                            resourceFiles.Add(f.name, f.content);
                    }
                }

                foreach (var file in project.files)
                {
                    compileFile(new File(text + "." + file.name, file.content), fu, true);
                }

                imported.Add(text);
                forcedUnsed = false;
                return "";
            }
            
            throw new Exception("Unknown library: " + text);
        }
        public static string instUsing(string text)
        {
            try
            {
                instAlias(text);
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
        private static void addVariable(string key, Variable variable)
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
        public static Variable GetVariableByName(string name)
        {
            if (name.StartsWith("@") && context.isEntity(smartSplitJson(name, '.', 1)[0]))
            {
                string[] val = smartSplitJson(name, '.', 1);
                if (val.Length == 2)
                {
                    return GetVariableByName(val[1]).Select(val[0]);
                }
            }
            string key = context.GetVariable(name);
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
        public static Function GetFunction(string funcName, string[] input_args, bool lambda = false)
        {
            Function funObj = null;
            bool numericalOnly = true;
            lambda = false;

            bool debug = false;

            string[] args = new string[input_args.Length + (lambda?1:0)];
            
            for (int i = 0; i < input_args.Length; i++)
            {
                args[i] = input_args[i];
            }
            if (lambda)
                args[args.Length - 1] = "__lambda__";
            
            if (args.Length == 1 && isString(args[0]))
            {
                foreach (string arg in smartSplitJson(extractString(args[0]),' '))
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
                        bool isGood = f.argNeeded <= args.Length && f.maxArgNeeded >= args.Length;

                        if (debug)
                            GobalDebug(f.gameName + "<1>" + isGood.ToString()+" in "+ f.argNeeded.ToString()+".."+ f.maxArgNeeded.ToString(), Color.Yellow);

                        if (!numericalOnly && f.lazy && f.tags.Contains("__numerical_only__"))
                            isGood = false;

                        for (int i = 0; i < args.Length; i++)
                        {
                            try
                            {
                                if (getExprType(args[i]) != f.args[i].type ||
                                    (f.lazy && f.args[i].name.StartsWith("$") && !f.tags.Contains("__numerical_only__")))
                                {
                                    isGood = false;
                                }
                            }
                            catch(Exception e){
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            if (debug)
                                GobalDebug("sel 1)"+f.gameName, Color.Yellow);
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
                            try
                            {
                                if ((getExprType(args[i]) != f.args[i].type && 
                                    !((getExprType(args[i]) == Type.INT || getExprType(args[i]) == Type.FLOAT) &&
                                    (f.args[i].type == Type.INT || f.args[i].type == Type.FLOAT)))
                                    || (f.lazy && f.args[i].name.StartsWith("$") && !f.tags.Contains("__numerical_only__")))
                                {
                                    isGood = false;
                                }
                            }
                            catch (Exception e)
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            if (debug)
                                GobalDebug("sel 2)" + f.gameName+"-", Color.Yellow);
                            funObj = f;
                            break;
                        }
                    }
                }
                foreach (Function f in functions[funcName])
                {
                    if (funObj == null && f.lazy && args.Length == f.args.Count)
                    {
                        bool isGood = f.argNeeded <= args.Length && f.maxArgNeeded >= args.Length;

                        for (int i = 0; i < args.Length; i++)
                        {
                            try
                            {
                                if ((getExprType(args[i]) != f.args[i].type &&
                                    !((getExprType(args[i]) == Type.INT || getExprType(args[i]) == Type.FLOAT) &&
                                    (f.args[i].type == Type.INT || f.args[i].type == Type.FLOAT)))
                                    || (f.lazy && f.args[i].name.StartsWith("$") && !f.tags.Contains("__numerical_only__")))
                                {
                                    isGood = false;
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        if (isGood)
                        {
                            if (debug)
                                GobalDebug("sel 3)" + f.gameName, Color.Yellow);
                            funObj = f;
                            break;
                        }
                    }
                }
                bool wasEmpty = (funObj == null);
                
                foreach (Function f in functions[funcName])
                {
                    int functionCount = 0;
                    foreach(var arg in f.args)
                    {
                        if (arg.type == Type.FUNCTION)
                        {
                            functionCount++;
                        }
                    }

                    if (funObj == null && f.lazy && args.Length >= f.args.Count - functionCount && args.Length <= f.args.Count)
                    {
                        funObj = f;
                    }
                    if (wasEmpty && f.lazy && args.Length == 1 && f.args.Count > 1
                        && smartSplitJson(args[0], ' ').Length >= f.args.Count - functionCount
                        && smartSplitJson(args[0], ' ').Length <= f.args.Count)
                    {
                        funObj = f;
                    }
                    if (wasEmpty && f.lazy && args.Length == 1 && f.args.Count > 1 && isString(args[0])
                        && smartSplitJson(extractString(args[0]), ' ').Length >= f.args.Count - functionCount
                        && smartSplitJson(extractString(args[0]), ' ').Length <= f.args.Count)
                    {
                        funObj = f;
                        if (debug)
                            GobalDebug("sel 4)" + f.gameName, Color.Yellow);
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
                    throw new Exception("No function Found for "+funcName+" with args:"+a);
                }
            }
            return funObj;
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
                            if (args[i].type != f.args[i].type && args[i].type != Type.INT && f.args[i].type != Type.INT && args[i].type != Type.FLOAT && f.args[i].type != Type.FLOAT)
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
        private static void generateStringPool()
        {
            int i = 0;
            foreach (string s in stringSet) {
                Variable mux = GetVariable("__multiplex__.sstring.__strSelector__");
                stringPool.AddLine("execute if score "+ mux.scoreboard() + " matches " + i.ToString() + " run summon minecraft:area_effect_cloud ~ ~ ~ { CustomName: \"\\\"" + s + "\\\"\",Tags:[\"__str__\"]}");
                i++;
            }
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

        public static string modVar(string text)
        {
            if (text.StartsWith("$"))
                return instCompilerVar(text);

            string op = "";
            string[] left;
            string[] right;

            op = opReg.Match(text).Value;

            if (op == "=")
            {
                left = smartSplit(smartExtract(smartSplit(text, '=', 1)[0]), ',');
                right = smartSplit(smartExtract(smartSplit(text, '=', 1)[1]), ',');
            }
            else
            {
                left = smartSplit(smartExtract(smartSplit(text.Replace(op, "§"), '§', 1)[0]), ',');
                right = smartSplit(smartExtract(smartSplit(text.Replace(op, "§"), '§', 1)[1]), ',');
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
                        var = new Variable(context.GetEntityName(left[i]), left[i].Split('.')[1], Type.ENTITY_COMPONENT);
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
        private static string eval(string val, Variable variable, Type ca, string op = "=")
        {
            if (variable.isConst && variable.wasSet)
                throw new Exception("Cannot moddify Constant!");
            variable.wasSet = true;

            if (containLazyVal(val))
            {
                return eval(getLazyVal(val), variable, ca, op);
            }
            
            if (context.IsFunction(val.Replace(" ", "")) && !val.Contains("(") && context.GetVariable(val, true) == null
                && ca != Type.FUNCTION && !(ca == Type.ENUM && enums[variable.enums].Contains(val.ToLower())))
            {
                return eval(val + "()", variable, ca, op);
            }
            string output = "";
            
            int tmpI;
            float tmpF;
            
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
                        output += parseLine(desugar(variable.gameName + "." + struV.name + "= null"));
                    }
                }
            }
            else if (val.Contains("?") && val.Contains(":"))
            {
                string[] a = val.Split('?');
                string[] b = a[1].Split(':');
                string cond = getCondition(a[0]);
                string invCond = cond.Contains("if") ? cond.Replace("if", "unless") : cond.Replace("unless", "if");

                output += cond + eval(b[0], variable, ca, op);
                output += invCond + eval(b[1], variable, ca, op);
            }
            else if ((smartContains(val,'^') || smartContains(val, '&') || smartContains(val,'|')) && ca == Type.BOOL)
            {
                return splitEval(val, variable, ca, op);
            }
            else if ((smartContains(val,'+') || smartContains(val,'-') || smartContains(val,'*') || smartContains(val,'%') 
                || smartContains(val, '/')) && ca != Type.FUNCTION && ca != Type.BOOL && ca != Type.STRING)
            {
                return splitEval(val, variable, ca, op);
            }
            else if (val.Contains("(") && context.IsFunction(val.Substring(0, val.IndexOf('('))))
            {
                return functionEval(val, new string[] { variable.gameName }, op);
            }
            else if (ca == Type.SELECTOR)
            {
                selector.Add(variable.gameName, smartEmpty(val));
            }
            else if (ca == Type.ARRAY)
            {
                while (val.StartsWith(" "))
                    val = val.Substring(1, val.Length - 1);

                while (val.EndsWith(" "))
                    val = val.Substring(0, val.Length - 1);
                    
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
                if (val.Contains("=>"))
                {
                    return instLamba(val, variable);
                }
                else if (context.GetVariable(val, true) == null)
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
                    return Core.VariableOperation(variable, GetVariableByName(val), "=");
                }
            }
            else if (ca == Type.STRUCT)
            {
                Structure stru1 = structs[variable.enums];
                
                if (getStruct(val.Replace("(", " ")) != null)
                {
                    output += parseLine(variable.gameName + ".__init__" + val.Substring(val.IndexOf('('), val.LastIndexOf(')') - val.IndexOf('(') + 1));
                }
                else
                {
                    if (op == "=")
                    {
                        if (stru1.methodsName.Contains("__set__"))
                        {
                            output += parseLine(variable.gameName + ".__set__(" + val + ")");
                        }
                        else
                        {
                            Structure stru2 = structs[GetVariableByName(smartEmpty(val)).enums];
                            if (!stru2.canBeAssignIn(stru1))
                            {
                                throw new Exception("Cannot use " + op + " between " + stru2.name + " inside " + stru1.name);
                            }
                            foreach (Variable struV in structs[variable.enums].fields)
                            {
                                preparseLine(variable.gameName + "." + struV.name + "=" + val + "." + struV.name);
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
                    else if (context.GetVariableName(val, true) != null)
                    {
                        Variable val2Obj = GetVariableByName(val);

                        if (val2Obj.type == Type.FLOAT)
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1f / compilerSetting.FloatPrecision) + "scoreboard players get " + val2Obj.scoreboard() + '\n';
                        else
                            output += NBT_Data.parseSet(variable.name, variable.gameName, 1) + "scoreboard players get " + val2Obj.scoreboard() + '\n';
                    }
                }
                else
                {
                    output += parseLine(NBT_Data.getType(variable.gameName) +" tmp.0 = " + variable.name + "." + variable.gameName + op[0] + "(" + val + ")");
                    output += eval("tmp.0", variable, ca, "=");
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
                        output += eval("tmp.0", variable, ca, op);
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
                        output += eval("tmp.0", variable, ca, op);
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
                        if (op != "=" || smartEmpty(context.GetVariableName(val)) != smartEmpty(variable.scoreboard()))
                        {
                            if (op == "#=")
                                return Core.VariableOperation(variable, GetVariableByName(val), "=");
                            else
                            {
                                var val2 = GetVariableByName(val);
                                if (val2.type != Type.FLOAT)
                                    return Core.VariableOperation(variable, GetVariableByName(val), op);
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
                        throw new Exception("Error on var " + variable.gameName + " (" + variable.type.ToString() + "," + variable.enums + ") " + e.ToString());
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
                        output += eval("tmp.0", variable, ca, op);
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
                        output += eval("tmp.0", variable, ca, op);
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
                    Variable var2 = GetVariableByName(val);

                    if (op != "=" || smartEmpty(context.GetVariableName(val)) != smartEmpty(variable.scoreboard()))
                    {
                        if (var2.type == Type.INT && (op == "+=" || op == "-="))
                        {
                            output += parseLine("int tmp.1 = "+ compilerSetting.FloatPrecision.ToString()+ "*" + val);
                            return output + Core.VariableOperation(variable, GetVariableByName("tmp.1"), op);
                        }
                        else if (var2.type == Type.INT && op == "#=")
                        {
                            return output + Core.VariableOperation(variable, GetVariableByName(val), "=");
                        }
                        else if (var2.type == Type.INT && op == "=")
                        {
                            output += Core.VariableOperation(variable, GetVariableByName(val), op);
                            return output + Core.VariableOperation(variable, compilerSetting.FloatPrecision, "*=");
                        }
                        else if (var2.type == Type.FLOAT && op == "*=")
                        {
                            output += Core.VariableOperation(variable, GetVariableByName(val), op);
                            return output + Core.VariableOperation(variable, compilerSetting.FloatPrecision, "/=");
                        }
                        else if (var2.type == Type.FLOAT && op == "/=")
                        {
                            output += Core.VariableOperation(variable, compilerSetting.FloatPrecision, "*=");
                            return output + Core.VariableOperation(variable, GetVariableByName(val), op);
                        }
                        else
                        {
                            return Core.VariableOperation(variable, GetVariableByName(val), op);
                        }
                    }
                }
            }
            else if (ca == Type.BOOL)
            {
                string ve = smartEmpty(val);
                if (op == "&=" || op == "*=")
                {
                    if (context.GetVariable(val, true) != null)
                    {
                        return Core.VariableOperation(variable, GetVariableByName(val), "*=");
                    }
                    else
                    {
                        string cond = getCondition("!"+val);
                        return cond + Core.VariableOperation(variable, 0, "=");
                    }
                }
                else if (op == "|=" || op == "+=")
                {
                    if (context.GetVariable(val, true) != null)
                    {
                        output += Core.VariableOperation(variable, GetVariableByName(val), "+=");
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
                    if (context.GetVariable(val, true) != null)
                    {
                        output += Core.VariableOperation(variable, GetVariableByName(val), "+=");
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
                else if (context.GetVariable(val, true) != null)
                {
                    if (op != "=" || smartEmpty(context.GetVariableName(val)) != smartEmpty(variable.scoreboard()))
                        return Core.VariableOperation(variable, GetVariableByName(val), "=");
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
                    output += "tag " + smartEmpty(val) + " add " + variable.gameName + '\n';
                    string end = "tag @e remove " + variable.gameName + '\n';
                    context.currentFile().AddEndLine(end);
                }
                else
                {
                    output += "tag " + context.GetEntitySelector(val) + " add " + variable.gameName + '\n';
                    string end = "tag @e remove " + variable.gameName + '\n';
                    context.currentFile().AddEndLine(end);
                }
            }
            else if (ca == Type.STRING)
            {
                if (val.Contains("\""))
                {
                    output += "scoreboard players set " + variable.scoreboard() + " " + getStringID(val).ToString() + '\n';
                }
                else
                {
                    try
                    {
                        if (op != "=" || smartEmpty(context.GetVariableName(val)) != smartEmpty(variable.scoreboard()))
                            output += "scoreboard players operation " + variable.scoreboard() + " " + op + " " + context.GetVariableName(val) + "" + '\n';
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error on var " + variable.gameName + " (" + variable.type.ToString() + "," + variable.enums + ") " + e.ToString());
                    }
                }
            }
            else if (ca == Type.VOID)
                throw new Exception("Cannot moddifie type void");

            if (context.GetVariable(val,true) != null && variables[context.GetVariable(val, true)].type == Type.VOID)
                throw new Exception("Cannot get value type void");

            return output;
        }
        private static string splitEval(string val, Variable variable, Type ca, string op)
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
                    if (xop[0] == op[0])
                    {
                        if (op[0] == '-')
                        {
                            output += eval(getParenthis(part[0], 1), variable, ca, op);
                            for (int i = 1; i < part.Length; i++)
                            {
                                string value = part[i];
                                output += eval(getParenthis(value, 1), variable, ca, "+=");
                            }
                        }
                        else if (op[0] == '/')
                        {
                            output += eval(getParenthis(part[0], 1), variable, ca, op);
                            for (int i = 1; i < part.Length; i++)
                            {
                                string value = part[i];
                                output += eval(getParenthis(value, 1), variable, ca, "*=");
                            }
                        }
                        else
                        {
                            foreach (string value in part)
                            {
                                output += eval(getParenthis(value, 1), variable, ca, op);
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
                            addVariable("__eval__" + id.ToString(), v1);
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

                        output += eval(getParenthis(part[0], 1), v1, ca);
                        for (int i = 1; i < part.Length; i++)
                        {
                            string value = part[i];
                            if (xop[0] == '-' && op[0] == '-')
                            {
                                output += eval(getParenthis(value, 1), v1, ca, "+=");
                            }
                            else
                            {
                                output += eval(getParenthis(value, 1), v1, ca, xop[0] + "=");
                            }
                        }

                        if (op != "=")
                            output += eval("__eval__" + (id).ToString(), variable, ca, op);
                        return output;
                    }
                }
            }
            
            throw new Exception("Unsportted operation: "+val);
        }
        private static string[] getConditionSplit(string text)
        {
            string[] arg = text.Replace("&&", "&").Split('&');
            string output = "";
            string cond = "";
            condID = 0;
            List<string> condList = new List<string>();
            for (int i = 0; i < arg.Length; i++)
            {
                if (arg[i].Contains("||"))
                {
                    string[] in1 = getCondOr(arg[i]);
                    cond += in1[0];
                    output += in1[1];
                    condList.Add(cond);
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

                        cond += Core.ConditionInverse(in1)[0];

                        output += in1[1];
                        condList.Add(cond);
                    }
                    else
                    {
                        string[] in1 = getCond(arg[i]);
                        
                        cond += in1[0];

                        output += in1[1];
                        condList.Add(cond);
                    }
                }
            }
            return new string[] { output, cond };
        }
        public static string getCondition(string text)
        {
            string[] v = getConditionSplit(text);
            condIDStack.Push(condID);
            return v[0] + Core.Condition(v[1]);
        }
        private static string[] getCondOr(string text)
        {
            string[] arg = text.Replace("||", "|").Split('|');
            string out1 = "";
            string out2 = "";
            int idVal = condID++;

            out2 += parseLine("bool __eval" + idVal.ToString() + "__ = " + text.Replace("&&","&").Replace("||", "|"));

            string[] part = getCond("__eval" + idVal.ToString() + "__");

            out2 += part[1];
            /*
            if (part[0].StartsWith("!"))
                out1 = "unless "+part[0].Substring(1, part[0].Length-1);
            else
                out1 = "if " + part[0];
                */
            out1 = part[0];
            return new string[] { out1, out2 };
        }
        private static string[] getCond(string text)
        {
            int tmpI;
            float tmpF;
            int idVal = condID++;
            
            if (context.isEntity(text) && !smartContains(text,'.'))
            {
                return Core.ConditionEntity(context.GetEntitySelector(text));
            }
            else if (text.Contains("block("))
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
            else if (text.Contains("blocks("))
            {
                string[] args = getArgs(text);
                return Core.ConditionBlocks(args[0]);
            }
            else if (text.Contains("=="))
            {
                string[] arg = text.Replace("==", "=").Split('=');
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
                    int idVal3 = condID++;
                    pre += parseLine(getExprType(arg[0]).ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + arg[0]);
                    var = GetVariableByName("cond_" + idVal3);
                }


                arg[1] = smartEmpty(arg[1]);

                Type t = var.type;

                if (arg[1] == "null")
                {
                    return Core.ConditionInverse(Core.CompareVariable(GetVariableByName(arg[0]), GetVariableByName(arg[0]), "=", selector1, selector1));
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

                        return Core.CompareVariable(var, p1,p2, selector1);
                    }
                    else if(int.TryParse(arg[1], out tmpI))
                    {
                        return Core.CompareVariable(var, tmpI, "=", selector1);
                    }
                    
                    if (var.enums != null && enums[var.enums].Contains(smartEmpty(arg[1].ToLower())))
                    {
                        return Core.CompareVariable(var, enums[var.enums].IndexOf(smartEmpty(arg[1].ToLower())), "=", selector1);
                    }
                    return Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2);
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

                    return Core.CompareVariable(var, p1, p2, selector1);
                }
                else if (t == Type.FLOAT && float.TryParse(arg[1], out tmpF))
                {
                    return Core.CompareVariable(var, (int)(tmpF * compilerSetting.FloatPrecision), "=", selector1);
                }
                else if (t == Type.BOOL)
                {
                    if (arg[1] == "true")
                        return Core.CompareVariable(var, 1, "=");
                    else if (arg[1] == "false")
                        return Core.CompareVariable(var, 0, "=");
                    else
                        return Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2);
                }
                else if (t == Type.STRING && arg[1].Contains("\""))
                {
                    return Core.CompareVariable(var, getStringID(arg[1]), "=", selector1);
                }
                else if (context.GetVariable(arg[1], true) != null)
                {
                    return Core.CompareVariable(var, GetVariableByName(arg[1]), "=", selector1, selector2);
                }
                else
                {
                    int idVal2 = condID++;
                    pre += parseLine(getExprType(arg[1]).ToString().ToLower() + " cond_" + idVal2.ToString() + " = " + arg[1]);
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName("cond_" + idVal2), "=", selector1), pre);
                }

            }
            else if (text.Contains("!="))
            {
                string[] arg = text.Replace("!=", "=").Split('=');
                string selector1 = "";
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
                        int idVal3 = condID++;
                        pre += parseLine(getExprType(arg[0]).ToString().ToLower() + " cond_" + idVal3.ToString() + " = " + arg[0]);
                        var = GetVariableByName("cond_" + idVal3);
                    }

                    return Core.CompareVariable(var, var, "==", selector1);
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
                    int idVal3 = condID++;
                    string line = parseLine(funObj.outputs[0].type.ToString().ToLower() + " cond_" + idVal3.ToString() + "=" + text);
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
                    if (text.StartsWith("!"))
                        return Core.CompareVariable(GetVariableByName(text.Replace("!", "")), 0, "<=");
                    else
                        return Core.CompareVariable(GetVariableByName(text.Replace("!", "")), 1, ">=");
                }

                string[] arg = smartEmpty(text).Replace(op, "=").Split('=');
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
                    int idVal3 = condID++;
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
                    int idVal2 = condID++;
                    pre += parseLine(getExprType(arg[1]).ToString().ToLower() + " cond_" + idVal2.ToString() + " = " + arg[1]);
                    return appendPreCond(Core.CompareVariable(var, GetVariableByName("cond_" + idVal2), op, selector1), pre);
                }
            }
        }
        private static string[] appendPreCond(string[] text, string val)
        {
            return new string[] { text[0], text[1] + "\n" + val };
        }
        private static int getStringID(string text)
        {
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length-1);
            while (text.EndsWith(" "))
                text = text.Substring(0, text.Length-1);
            if (text.StartsWith("\""))
                text = text.Substring(1, text.Length-2);

            if (!stringSet.Contains(text))
                stringSet.Add(text);
            return stringSet.IndexOf(text);
        }


        #region function GRP
        public static string GetFunctionGRP(Function func)
        {
            string grp = "m";

            foreach (Argument arg2 in func.args)
            {
                if (arg2.enums == null)
                    grp += arg2.type.ToString().ToLower();
                else
                    grp += arg2.enums.ToLower();
            }
            grp += ".m";
            foreach (Variable arg2 in func.outputs)
            {
                if (arg2.enums == null)
                    grp += arg2.type.ToString().ToLower();
                else
                    grp += arg2.enums.ToLower();
            }
            return grp;
        }
        public static string GetFunctionGRP(Variable func)
        {
            string grp = "m";

            foreach (Argument arg2 in func.args)
            {
                if (arg2.enums == null)
                    grp += arg2.type.ToString().ToLower();
                else
                    grp += arg2.enums.ToLower();
            }
            grp += ".m";
            foreach (Variable arg2 in func.outputs)
            {
                if (arg2.enums == null)
                    grp += arg2.type.ToString().ToLower();
                else
                    grp += arg2.enums.ToLower();
            }
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
                addVariable("__mux__" + grp, new Variable("__mux__" + grp, "__mux__" + grp, Type.INT));
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
                addVariable("__mux__" + grp, new Variable("__mux__" + grp, "__mux__" + grp, Type.INT));
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
            if ((functDelegated[grp].Count-1) % (compilerSetting.TreeMaxSize) == 0 && functDelegated[grp].Count > 1)
            {
                Variable mux = GetVariable("__mux__" + grp);
                if (fFiles.Count == 1)
                {
                    string muxedFileNameInit = "__multiplex__/" + grp + "/splitted_" + (fFiles.Count - 1).ToString();
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

                string muxedFileName = "__multiplex__/" + grp + "/splitted_" + (fFiles.Count - 1).ToString();
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
            string output = parseLine(desugar("__mux__" + grp + " = " + func));

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

        public static File CreateFunctionTag(string tag)
        {
            if (!functions.ContainsKey(Project + ".__tags__." + tag.ToLower()))
            {
                functionTags.Add(tag.ToLower());

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
            func.file.notUsed = wasUsed;
            f.addChild(func.file);
        }
        #endregion

        #region instantiation
        public static string instCompilerVar(string text)
        {
            string[] splited = smartSplit(text, '=', 1);
            string[] field = smartSplit(smartExtract(splited[0]), ' ');
            string name = field[field.Length - 1];
            string value = smartExtract(splited[1]);

            if (value.StartsWith("$"))
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
                type = Type.INT;
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

                if (type == Type.INT || type == Type.FUNCTION || type == Type.FLOAT)
                {
                    if (context.GetVariable(value, true) != null)
                    {
                        compVal[compVal.Count - 1].Add(name + ".enums", GetVariableByName(value).enums);
                        compVal[compVal.Count - 1].Add(name + ".type", GetVariableByName(value).GetTypeString());
                        compVal[compVal.Count - 1].Add(name + ".name", GetVariableByName(value).gameName);
                        compVal[compVal.Count - 1].Add(name + ".scoreboard", GetVariableByName(value).scoreboard());
                        compVal[compVal.Count - 1].Add(name + ".scoreboardname", GetVariableByName(value).scoreboard().Split(' ')[1]);
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
                    if (context.GetVariable(value, true) != null)
                    {
                        compVal[compVal.Count - 1].Add(name + ".scoreboard", GetVariableByName(value).scoreboard());
                        compVal[compVal.Count - 1].Add(name + ".enums", GetVariableByName(value).enums);
                        compVal[compVal.Count - 1].Add(name + ".type", GetVariableByName(value).GetTypeString());
                        compVal[compVal.Count - 1].Add(name + ".name", GetVariableByName(value).gameName);
                    }
                    compVal[compVal.Count - 1].Add(name, context.GetVariable(value));
                }
                return "";
            }
        }
        public static string instVar(string text)
        {
            string vari;
            Type ca = Type.INT;
            bool isConst = false;
            bool isPrivate = false;

            if (text.StartsWith("private "))
            {
                isPrivate = true;
                text = text.Substring("private".Length, text.Length - "private".Length);
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
            

            ca = getType(text);

            bool entity = text.ToUpper().Substring(0, 3) == text.Substring(0, 3);

            if (text.Contains("="))
            {
                if (ca == Type.FUNCTION && text.Contains("<"))
                {
                    int f = text.IndexOf(" ", text.IndexOf(">"))+1;
                    vari = smartEmpty(text.Substring(f, text.IndexOf("=") - f));
                    
                }
                else
                {
                    vari = smartEmpty(text.Substring(text.IndexOf(" ") + 1,
                        text.IndexOf('=') - text.IndexOf(" ") - 1).Replace(" ", ""));
                }
            }
            else
            {
                if (ca == Type.FUNCTION && text.Contains("<"))
                {
                    int f = text.IndexOf(" ", text.IndexOf(">"))+1;
                    vari = smartEmpty(text.Substring(f, text.Length-f));
                }
                else
                {
                    vari = smartEmpty(text.Substring(text.IndexOf(" ") + 1,
                    text.Length - text.IndexOf(" ") - 1).Replace(" ", ""));
                }
            }


            Variable variable;
            string def = "dummy";
            string[] splited = smartSplit(text, '=', entity?2:1);

            if (text.Contains("="))
                def = splited[1].Replace(" ", "");
            bool entityFormatVar = false;
            if (float.TryParse(def, out float tmp))
            {
                def = "dummy";
                entityFormatVar = true;
            }

            int part = splited.Length;

            string output = "";
            int index = 0;
            string[] defValue = null;

            
            if (!entity && text.Contains("="))
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
            foreach (string v in smartSplit(smartEmpty(vari), ','))
            {
                if (variables.ContainsKey(context.GetVar() + v))
                {
                    variables.Remove(context.GetVar() + v);
                }
                if (ca == Type.ARRAY)
                {
                    string arraySizeS = arraySizeReg.Match(text).Value.Replace("[", "").Replace("]", "");
                    int arraySize = int.Parse(arraySizeS);

                    variable = new Variable(v, context.GetVar() + v, ca, entity, def);
                    variable.isConst = isConst;
                    variable.arraySize = arraySize;
                    variable.isPrivate = isPrivate;
                    variable.privateContext = context.GetVar();
                    addVariable(context.GetVar() + v, variable);

                    string typeArray = arrayTypeReg.Match(text).Value.Replace("[", "");
                    variable.typeArray = typeArray;

                    if (structStack.Count > 0 && !isInStructMethod)
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
                    variable = new Variable(v, context.GetVar() + v, ca, entity, def);
                    variable.isConst = isConst;
                    variable.isPrivate = isPrivate;
                    variable.privateContext = context.GetVar();

                    addVariable(context.GetVar() + v, variable);
                    if (ca == Type.ENUM)
                    {
                        string enu = getEnum(text);
                        variable.SetEnum(enu);
                        enums[enu].GenerateVariable(variable.name);
                    }
                    if (structStack.Count > 0 && !isInStructMethod)
                    {
                        structStack.Peek().addField(variable);
                    }

                    if (ca == Type.FUNCTION && text.Contains(">") && text.Contains("<"))
                    {
                        string[] typeArg = smartSplit(text.Substring(text.IndexOf("<") + 1, text.IndexOf(" ", text.IndexOf(">"))- text.IndexOf("<") - 1), ',');
                        if (typeArg.Length > 0)
                        {
                            int i = 0;
                            foreach (string s in getArgs(typeArg[0]))
                            {
                                Type type = getType(s + " ");
                                Argument arg = new Argument(i.ToString(), context.GetVar() + v + "." + i.ToString(), type);
                                Variable var2 = new Variable(i.ToString(), context.GetVar() + v + "." + i.ToString(), type);
                                addVariable(context.GetVar() + v + "." + i.ToString(), var2);
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
                                Variable var2 = new Variable(i.ToString(), context.GetVar() + v + ".ret_" + i.ToString(), type);
                                addVariable(context.GetVar() + v + ".ret_" + i.ToString(), var2);
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
                }
                else
                {
                    variable = new Variable(v, context.GetVar() + v, ca, entity, def);
                    variable.isConst = isConst;
                    variable.isPrivate = isPrivate;
                    variable.privateContext = context.GetVar();
                    addVariable(context.GetVar() + v, variable);

                    if (getStruct(text) == null)
                        throw new Exception("No struct in " + text);

                    variable.SetEnum(getStruct(text));

                    if (structStack.Count > 0 && !isInStructMethod)
                    {
                        structStack.Peek().addField(variable);
                    }

                    if (structStack.Count == 0)
                    {
                        if (part == 2 && defValue[index].Replace(" ", "").ToLower().StartsWith(getStruct(text)))
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
                
                if (ca == Type.STRUCT && part == 2 && defValue[index].Replace(" ","").ToLower().StartsWith(getStruct(text)))
                {
                    index = (index + 1) % defValue.Length;
                }
            }
            if (context.currentFile().type != "struct" && ((!entity && text.Contains("=")) ||
                     (entity && part == 2 && entityFormatVar)) && !instantiated)
            {
                output += modVar(vari +"="+ splited[1]);
            }
            if (context.currentFile().type != "struct" && ((entity && part == 3)) && !instantiated)
            {
                output += modVar(vari + "=" + splited[2]);
            }

            return output;
        }
        public static string instFuncDesc(string text)
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
        public static string instFunc(string text)
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

            function.privateContext = context.GetVar();

            if (!functions.ContainsKey(funcID))
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
                foreach (string r in ret)
                {
                    Type ca = getType(r + " ");
                    parseLine(smartEmpty(r.Replace("{","")) + " ret_" + i.ToString());
                    function.outputs.Add(GetVariableByName("ret_" + i.ToString()));
                    i++;
                }
            }
            if (outputType.Count > 0)
            {
                int i = 0;
                foreach (string r in outputType)
                {
                    Type ca = getType(r + " ");
                    parseLine(smartEmpty(r.Replace("{", "")) + " ret_" + i.ToString());
                    function.outputs.Add(GetVariableByName("ret_" + i.ToString()));
                    i++;
                }
            }

            foreach (string a in args)
            {
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
                    callTrace += "\"load\"->\"" + function.gameName + "\"\n";
                }

                if (isTicking)
                {
                    mainFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                    fFile.use();
                    callTrace += "\"main\"->\"" + function.gameName + "\"\n";
                }

                if (isHelper)
                {
                    fFile.use();
                    callTrace += "\"helper\"->\"" + function.gameName + "\"\n";
                }

                
                foreach (string tag in tags)
                {
                    if (!tag.StartsWith("__"))
                    {
                        AddToFunctionTag(function, tag);

                        callTrace += "\"@" + tag.ToLower() + "\"->\"" + function.gameName + "\"\n";
                    }
                }
            }

            return "";
        }
        public static string instWhile(string text, string fText)
        {
            string loop = getCondition(text);

            int wID = whileID++;
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
        public static string instIf(string text, string fText, int mult = 0)
        {
            string loop = getCondition(text);

            int wID = whileID++;
            int wID2 = -1;
            
            string funcName = context.GetFun() + "i_" + wID.ToString();

            string cmd = "function " + funcName + '\n';
            if (mult == 1)
            {
                wID2 = whileID++;
                context.currentFile().AddLine(parseLine("int __elseif_" + wID2.ToString() + " = 0"));
                LastConds.Push(wID2);
            }
            else if (mult == 0)
            {
                LastConds.Push(-1);
            }

            context.currentFile().AddLine(loop + cmd);
            context.currentFile().cantMergeWith = true;
            File fFile = new File(context.GetFile() + "i_" + wID, "", "if");
            context.Sub("i_" + wID, fFile);
            if (mult == 1)
            {
                context.currentFile().AddLine(parseLine("__elseif_" + wID2.ToString() + " = 1"));
            }
            if (mult == 2)
            {
                context.currentFile().AddLine(parseLine("__elseif_" + LastCond.ToString() + " = 1"));
            }

            files.Add(fFile);

            autoIndent(fText);

            return "";
        }
        public static string instElseIf(string text, string fText)
        {
            if (LastCond < 0)
                throw new Exception("No if statement to apply else");
            LastConds.Push(LastCond);
            return instIf("__elseif_"+LastCond.ToString()+" == 0 && "+text, fText, 2);
        }
        public static string instElse(string fText)
        {
            if (LastCond < 0)
                throw new Exception("No if statement to apply else");

            string ret = instIf("__elseif_" + LastCond.ToString() + " == 0", fText);
            LastCond = -1;
            return ret;
        }
        public static string instFor(string text, string fText)
        {
            string[] args = smartSplit(text.Replace(",", ";"), ';');

            File f = context.currentFile();

            int wID = whileID++;
            string funcName = context.GetFun() + "f_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            File fFile = new File(context.GetFile() + "f_" + wID, "", "while");
            context.Sub("f_" + wID, fFile);
            files.Add(fFile);

            f.AddLine(instVar(args[0]));
            string loop = getCondition(args[1]);
            f.AddLine(loop + cmd);
            fFile.AddEndLine(parseLine(args[2]) + loop + cmd);

            autoIndent(fText);

            return "";
        }
        public static string instForGenerate(string text, string fText)
        {
            if (!isInStructMethod)
            {
                string[] args = smartSplit(text.Replace(";", ","), ',');

                File f = context.currentFile();

                int wID = whileID++;

                File fFile = new File(context.GetFile() + "g_" + wID, "", "forgenerate");
                fFile.var = smartEmpty(args[0]);

                if (enums.ContainsKey(smartEmpty(args[1]).ToLower()))
                {
                    fFile.enumGen = smartEmpty(args[1]).ToLower();
                }
                else if (smartEmpty(args[1]).StartsWith("("))
                {
                    fFile.enumGen = smartEmpty(args[1]);
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
        public static string instWith(string[] text, string fText)
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

                int wID = whileID++;
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
                int wID = whileID++;
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
        public static string instAt(string[] text, string fText)
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


                int wID = whileID++;
                string funcName = context.GetFun() + "w_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "w_" + wID, "", "at");
                context.Sub("w_" + wID, fFile);
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

                int wID = whileID++;
                string funcName = context.GetFun() + "w_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "w_" + wID, "", "at");
                context.Sub("w_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string instPositioned(string text,string fText)
        {
            if (text.Split(' ').Length == 3 && !text.Contains(",")
                && (float.TryParse(text.Split(' ')[0], out float _a) || text.Split(' ')[0].StartsWith("~") || text.Split(' ')[0].StartsWith("^"))
                && (float.TryParse(text.Split(' ')[1], out float _b) || text.Split(' ')[1].StartsWith("~") || text.Split(' ')[1].StartsWith("^"))
                && (float.TryParse(text.Split(' ')[2], out float _c) || text.Split(' ')[2].StartsWith("~") || text.Split(' ')[2].StartsWith("^")))
            {

                if (isString(text))
                    text = extractString(text);

                string pre = Core.Positioned(text);

                int wID = whileID++;
                string funcName = context.GetFun() + "w_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "w_" + wID, "", "at");
                context.Sub("w_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string instAligned(string text, string fText)
        {
            if (text.Split(' ').Length == 1)
            {
                string pre = Core.Align(smartEmpty(text));

                int wID = whileID++;
                string funcName = context.GetFun() + "w_" + wID.ToString();

                string cmd = "function " + funcName + '\n';

                context.currentFile().AddLine(pre + cmd);

                File fFile = new File(context.GetFile() + "w_" + wID, "", "at");
                context.Sub("w_" + wID, fFile);
                files.Add(fFile);

                autoIndent(fText);

                return "";
            }
            else
            {
                return functionEval(fText);
            }
        }
        public static string instEnum(string text)
        {
            string[] field = smartSplit(text, '=');
            string[] subField1 = smartSplit(field[0], ' ');
            bool final = false;
            bool overriding = false;

            string name = "";
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
            if (overriding && enums.ContainsKey(name))
                enums.Remove(name);
            if (fields != null) 
                enums.Add(name,new Enum(name,fields,smartSplit(field[1], ','),final));
            else
                enums.Add(name, new Enum(name,smartSplit(field[1], ','), final));

            return "";
        }
        public static string instEnumFile(string text)
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

            return "";
        }
        public static string instBlockTag(string text)
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
                            blockTags[name].values.Add(value.Substring(1, value.Length - 1));
                        else
                            blockTags[name].values.Add(value);
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
        public static string instSwitch(string text)
        {
            switchID++;
            Type t = getExprType(text);

            string name = "_s." + switchID.ToString();
            if (t == Type.STRUCT)
                parseLine(GetVariableByName(text).enums.ToLower()+" "+name);
            else if (t==Type.ENUM)
                parseLine(getExprEnum(text).ToLower() + " " + name);
            else
                parseLine(t.ToString().ToLower() + " " + name);

            Variable variable = GetVariableByName(name);
            
            switches.Push(variable);
            string init = eval(text, variable, t, "=");

            int wID = whileID++;
            string funcName = context.GetFun() + "s_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().AddLine(init);
            File fFile = new File(context.GetFile() + "s_" + wID, "", "switch");
            context.Sub("s_" + wID, fFile);
            files.Add(fFile);
            return "";
        }
        public static string instCase(string text)
        {
            if (switchID == -1)
            {
                throw new Exception("Invalide use of case statement.");
            }
            string loop = getCondition(switches.Peek().gameName + "==" + text);

            int wID = whileID++;
            string funcName = context.GetFun() + "i_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().AddLine(loop + cmd);
            context.currentFile().cantMergeWith = true;
            File fFile = new File(context.GetFile() + "i_" + wID, "", "case");
            context.Sub("i_" + wID, fFile);
            files.Add(fFile);
            
            return "";
        }
        public static string instSmartCase(string text)
        {
            text = text.Replace("->", "§");
            string[] part = smartSplit(text, '§', 1);
            if (switchID == -1)
            {
                throw new Exception("Invalide use of case statement.");
            }
            string loop = getCondition(switches.Peek().gameName + "==" + part[0]);

            int wID = whileID++;
            string funcName = context.GetFun() + "i_" + wID.ToString();

            string cmd = "function " + funcName + '\n';

            context.currentFile().AddLine(loop + cmd);
            context.currentFile().cantMergeWith = true;
            File fFile = new File(context.GetFile() + "i_" + wID, "", "case");
            context.Sub("i_" + wID, fFile);
            files.Add(fFile);

            fFile.AddLine(parseLine(part[1]));
            fFile.Close();

            return "";
        }
        public static string instStruct(string text)
        {
            string[] split = smartSplit(text.ToLower().Replace("{", ""), ' ',1);
            string name = split[1];
            Structure parent = null;
            if (split.Length > 3 && split[2] == "extends")
            {
                parent = structs[split[3]];
            }
            string generics = "";
            if (name.Contains("<"))
            {
                inGenericStruct = true;
                generics = name.Substring(name.IndexOf("<") + 1, name.LastIndexOf(">") - name.IndexOf("<") - 1);
                name = smartEmpty(name.Substring(0, name.IndexOf("<")));
            }

            context.Sub(name, new File("struct/"+name,"", "struct"));
            //context.currentFile().notUsed = true;
            Structure stru = new Structure(name, parent);
            try
            {
                structs.Add(name, stru);
                structStack.Push(stru);
            }
            catch
            {
                throw new Exception(name + " already exists");
            }

            foreach(string gen in smartSplit(generics,','))
            {
                stru.addGeneric(smartEmpty(gen));
                stru.isGeneric = true;
            }

            thisDef.Push(context.GetVar());

            return "";
        }
        public static string instClass(string text)
        {
            string[] split = smartSplit(text.ToLower().Replace("{", ""), ' ', 1);
            string name = split[1];
            Class c = new Class(name);
            classDefStack.Push(c);
            instStruct(text);
            classes.Add(name, c);
            c.structure = structs[name];
            return "";
        }
        public static string instAlias(string text)
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
                    GobalDebug(functions[funcID][0].gameName + " was overrided", Color.Yellow);
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
                    GobalDebug(functions[funcID][0].gameName + " was overrided", Color.Yellow);
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
                    variables.Add(context.GetVar() + f2, variables[context.GetVariable(f1)]);
                }
            }
            else if (context.GetVariable(f2, true) != null)
            {
                if (!variables.ContainsKey(context.GetVar() + f1))
                {
                    variables.Add(context.GetVar() + f1, variables[context.GetVariable(f2)]);
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
                        MessageBox.Show(e.ToString());
                    }
                }
                else
                {
                    throw new Exception("Unkown function/variable " + f2);
                }
            }

            return "";
        }
        public static string instLamba(string text, Variable variable)
        {
            string[] para = text.Replace("=>", "\\").Split('\\');
            string lambda = "lamba_" + lambdaID.ToString();
            string func = "def "+lambda + "(";
            string content = "return(";
            lambdaID++;

            if (para[0].Contains("("))
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
            string[] args = smartSplit(para[0], ' ');

            int i = 0;
            foreach (Argument v in variable.args)
            {
                func += v.type.ToString().ToLower() + " " + args[i];
                if (i < args.Length - 1)
                    func += ", ";
            }
            func += ")";
            if (variable.outputs.Count > 0)
            {
                func += ":";
            }
            i = 0;
            foreach (Variable v in variable.outputs)
            {
                func += v.type.ToString().ToLower();
                if (i < variable.outputs.Count - 1)
                    func += ", ";
            }
            func += "{";

            if (para[1].Contains("return") || variable.outputs.Count==0)
            {
                content = para[1];
            }
            else
            {
                content += para[1] + ")";
            }
            
            preparseLine(func);
            preparseLine(content);
            preparseLine("}");

            return eval(lambda, variable, variable.type, "=");
        }
        public static string instPackage(string text)
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
        public static string instInLineForgenerate(string text)
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
        public static string instJsonFile(string text)
        {
            string name = "";
            if (text.Contains("\"")){
                name = text.Substring(text.IndexOf("\"") + 1, text.LastIndexOf("\"") - text.IndexOf("\"") - 1);
            }
            else
            {
                name = smartSplit(text, ' ', 1)[1].Replace("{", "");
            }
            jsonFiles.Add(new File(name, "", "json"));
            if (text.Contains("{"))
            {
                jsonFiles[jsonFiles.Count - 1].AddLine(text.Substring(text.IndexOf("{"), text.Length - text.IndexOf("{")));
            }
            jsonIndent = text.Split('{').Length - text.Split('}').Length;
            return "";
        }
        public static string AddToJsonFile(string text)
        {
            jsonFiles[jsonFiles.Count - 1].AddLine(text);
            jsonIndent += text.Split('{').Length - text.Split('}').Length;
            return "";
        }
        #endregion

        private static bool containType(string text)
        {
            foreach (string key in typeMaps.Keys)
            {
                if (text.ToLower().StartsWith(key))
                    return true;
            }
            return false;
        }
        public static Type getType(string t)
        {
            t = t.Replace("{", "") + " ";
            
            while (t.StartsWith(" "))
                t = t.Substring(1, t.Length - 1);

            foreach (string key in typeMaps.Keys)
            {
                if (t.ToLower().StartsWith(key))
                    return getType(typeMaps[key]);
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
                return getType(getLazyVal(smartEmpty(t)));
            }
            else
            {
                throw new Exception("Unknown type: " + t);
            }
            return type;
        }
        public static Type getExprType(string t)
        {
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
                return getExprType(getParenthis(t));
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
            try
            {
                string func = smartExtract(text.Substring(0, text.IndexOf('(')));
                if (GetVariableByName(func).type == Type.FUNCTION)
                {
                    return functionVarEval(text, outVar, op);
                }
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                string arg = getArg(text);
                string[] args = smartSplitJson(arg, ',');

                string func;
                bool anonymusFunc = false;
                string anonymusFuncName = "";

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
                if (func.StartsWith("@"))
                {
                    string tag = func.Substring(1, func.Length - 1);
                    CreateFunctionTag(tag);

                    func = "__tags__." + tag.ToLower();
                }

                string funcName = context.GetFunctionName(func);


                Function funObj = GetFunction(funcName, args, text.EndsWith("{") || text.EndsWith("}"));
                if (lazyCall.Contains(funObj))
                    throw new Exception("Cannot have recursive Lazy Recursive Function.");
                if (funObj.isPrivate && !context.GetVar().StartsWith(funObj.privateContext))
                {
                    throw new Exception("can not call private function " + funObj.name);
                }
                if (structStack.Count == 0)
                {
                    if (!context.currentFile().notUsed)
                        funObj.file.use();
                }
                context.currentFile().addChild(funObj.file);
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
                            if (a.defValue == null && !(smartEmpty(text).EndsWith("{") && a.type == Type.FUNCTION)
                                                    && !(smartEmpty(text).EndsWith("}") && a.type == Type.FUNCTION))
                            {
                                string val = context.getImpliciteVar(a);
                                if (val != null && a.isImplicite)
                                {
                                    output += parseLine(desugar(a.gameName + "=" + val));
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
                                output += parseLine(desugar(a.gameName + "=" + a.defValue));
                            else if ((smartEmpty(text).EndsWith("}") || smartEmpty(text).EndsWith("{")) && a.type == Type.FUNCTION)
                            {
                                anonymusFuncName = "lamba_" + lambdaID.ToString();

                                parseLine("def abstract __lambda__ " + anonymusFuncName + "()");

                                compVal[compVal.Count-1].Add(a.name + ".name", functions[context.GetFunctionName(anonymusFuncName)][0].gameName);
                                if (a.name.StartsWith("$"))
                                    compVal[compVal.Count - 1].Add(a.name, anonymusFuncName);
                                else
                                    addLazyVal(a.name, anonymusFuncName);

                                anonymusFunc = true;
                                lambdaID++;
                            }
                        }
                        else
                        {
                            if (a.name.StartsWith("$"))
                            {
                                if (a.type == Type.INT || a.type == Type.FUNCTION || a.type == Type.FLOAT)
                                {
                                    if (context.GetVariable(smartExtract(args[i]),true) != null){
                                        compVal[compVal.Count-1].Add(a.name + ".enums", GetVariableByName(smartExtract(args[i])).enums);
                                        compVal[compVal.Count-1].Add(a.name + ".type", GetVariableByName(smartExtract(args[i])).GetTypeString());
                                        compVal[compVal.Count-1].Add(a.name + ".name", GetVariableByName(smartExtract(args[i])).gameName);
                                        compVal[compVal.Count-1].Add(a.name + ".scoreboard", GetVariableByName(smartExtract(args[i])).scoreboard());
                                        compVal[compVal.Count - 1].Add(a.name + ".scoreboardname", GetVariableByName(smartExtract(args[i])).scoreboard().Split(' ')[1]);
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
                                    compVal[compVal.Count-1].Add(a.name + ".scoreboard", GetVariableByName(smartEmpty(args[i])).scoreboard());
                                    compVal[compVal.Count-1].Add(a.name + ".enums", GetVariableByName(smartEmpty(args[i])).enums);
                                    compVal[compVal.Count-1].Add(a.name + ".type", GetVariableByName(smartEmpty(args[i])).GetTypeString());
                                    compVal[compVal.Count-1].Add(a.name + ".name", GetVariableByName(smartEmpty(args[i])).gameName);
                                    compVal[compVal.Count - 1].Add(a.name, context.GetVariable(args[i]));
                                }
                            }
                            else
                            {
                                if (context.GetVariable(args[i].Replace(" ", ""), true) != null)
                                {
                                    addLazyVal(a.name, context.GetVariable(args[i].Replace(" ", "")));

                                    if (a.type == Type.STRUCT)
                                    {
                                        foreach (Variable v in structs[a.enums].fields)
                                        {
                                            addLazyVal(a.name + "." + v.name, context.GetVariable(args[i].Replace(" ", "") + "." + v.name));
                                        }
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
                    
                    File tFile = context.currentFile();

                    i = 0;
                    autoIndented = 0;
                    if (funObj.variableStruct != null)
                        adjPackage.Push(funObj.variableStruct);
                    string prevFunction = callingFunctName;
                    callingFunctName = funObj.gameName;
                    foreach (string line in funObj.file.parsed)
                    {
                        string modLine = line;                        
                        
                        preparseLine(modLine, tFile, true);
                        
                        i++;
                    }
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
                        parseLine("def " + anonymusFuncName + "(){");
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
                                    
                                    output += parseLine(desugar(a.gameName + "=" + part[1]))+"\n";
                                    assignedArg.Add(a);
                                    found = true;
                                }
                            }
                            if (!found)
                                throw new Exception("Unknown argument "+part[0]);
                        }
                        else
                        {
                            output += parseLine(desugar(funObj.args[i].gameName + "=" + args[i])) + "\n";
                            assignedArg.Add(funObj.args[i]);
                        }
                    }
                    if (funObj.args.Count != args.Length)
                    {
                        foreach (Argument a in funObj.args)
                        {
                            if (!assignedArg.Contains(a))
                            {
                                if (a.defValue == null && !(smartEmpty(text).EndsWith("{") && a.type == Type.FUNCTION)
                                                        && !(smartEmpty(text).EndsWith("}") && a.type == Type.FUNCTION))
                                {
                                    string val = context.getImpliciteVar(a);
                                    if (val != null && a.isImplicite)
                                    {
                                        output += parseLine(desugar(a.gameName + "=" + val));
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
                                    output += parseLine(desugar(a.gameName + "=" + a.defValue));
                                else if ((smartEmpty(text).EndsWith("}") || smartEmpty(text).EndsWith("{")) && a.type == Type.FUNCTION)
                                {
                                    anonymusFuncName = "lamba_" + lambdaID.ToString();
                                    parseLine("def abstract " + anonymusFuncName + "()");
                                    output += parseLine(desugar(a.gameName + "=" + anonymusFuncName)) + "\n";
                                    anonymusFunc = true;
                                    lambdaID++;
                                }
                            }
                        }
                    }

                    output += Core.CallFunction(funObj) + '\n';
                    if (outVar != null)
                    {
                        if (funObj.outputs.Count == 0 && outVar.Length > 0)
                        {
                            throw new Exception("Function "+ funObj.gameName+" do not return any value. ");
                        }
                        else if (context.GetVariable(outVar[0],true)!=null && GetVariableByName(outVar[0]).type == Type.ARRAY && funObj.outputs[0].type != Type.ARRAY)
                        {
                            if (GetVariableByName(outVar[0]).arraySize != funObj.outputs.Count)
                                throw new Exception("Cannot cast function output into array");

                            for (int j = 0; j < funObj.outputs.Count; j++)
                            {
                                string v = context.GetVariable(outVar[0]+"."+j.ToString());
                                output += parseLine(desugar(v + " " + op + " " + funObj.outputs[j].gameName)) + '\n';
                            }
                        }
                        else
                        {
                            for (int j = 0; j < outVar.Length; j++)
                            {
                                output += parseLine(desugar(outVar[j] + " " + op + " " + funObj.outputs[j].gameName)) + '\n';
                            }
                        }
                    }
                    if (anonymusFunc)
                    {
                        context.currentFile().AddLine(output);
                        parseLine("def " + anonymusFuncName + "(){");
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
                    ouput += parseLine(desugar(v.gameName + "=" + arg[i]));
                    i++;
                }
            }
            else
            {
                bool hadOutput = context.currentFile().function.outputs.Count > 0;
                bool changeOutput = false;
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
                            changeOutput = true;
                        }
                    }
                    ouput += parseLine(desugar("ret_" + i.ToString() + "=" + arg[i]));
                }
                if (changeOutput)
                {
                    var m = context.currentFile().function.file;
                    int i = "#=================================================#".Length;
                    int j = m.content.IndexOf("#=================================================#",
                            m.content.IndexOf("#=================================================#")+1);
                    string t = m.content.Substring(j + i, m.content.Length - j - i);
                    m.content = GenerateInfo(context.currentFile().function) + t;
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
            string current = "";
            bool inString = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == ')' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '[' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == ']' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '<' && c == ',' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == '>' && c == ',' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '"')
                {
                    if (inString)
                    {
                        ind -= 1;
                        current += text[i];
                        inString = false;
                    }
                    else
                    {
                        ind += 1;
                        current += text[i];
                        inString = true;
                    }
                }
                else if (text[i] == c && ind == 0 && max != 0 && !inString)
                {
                    output.Add(current);
                    current = "";
                    max--;
                }
                else { current += text[i]; }
            }
            if (current != "")
                output.Add(current);

            return output.ToArray();
        }
        public static string[] smartSplitJson(string text, char c, int max = -1)
        {
            List<string> output = new List<string>();
            int ind = 0;
            string current = "";
            bool inString = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == ')' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '{' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == '}' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '[' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == ']' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '<' && c == ',' && !inString)
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == '>' && c == ',' && !inString)
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '"')
                {
                    if (inString)
                    {
                        ind -= 1;
                        current += text[i];
                        inString = false;
                    }
                    else
                    {
                        ind += 1;
                        current += text[i];
                        inString = true;
                    }
                }
                else if (text[i] == c && ind == 0 && max != 0 && !inString)
                {
                    output.Add(current);
                    current = "";
                    max--;
                }
                else { current += text[i]; }
            }
            if (current != "")
                output.Add(current);

            return output.ToArray();
        }
        public static bool smartContains(string text, char c, int max = -1)
        {
            List<string> output = new List<string>();
            int ind = 0;
            string current = "";
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
                    current += text[i];
                }
                else if (text[i] == ')')
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '[')
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == ']')
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == '"' && c != '"')
                {
                    inString = !inString;
                    current += text[i];
                }
                else if (text[i] == '<' && c == ',')
                {
                    ind += 1;
                    current += text[i];
                }
                else if (text[i] == '>' && c == ',')
                {
                    ind -= 1;
                    current += text[i];
                }
                else if (text[i] == c && ind == 0 && max != 0 && !inString)
                {
                    return true;
                }
                else { current += text[i]; }
            }
            if (current != "")
                output.Add(current);
            
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
            string current = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                {
                    ind = 1 - ind;
                    current += text[i];
                }
                else if ((text[i] == ' ' || text[i] == '\t') && ind == 0)
                {

                }
                else { current += text[i]; }
            }

            return current;
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
               return text.Substring(text.IndexOf('(') + 1, getCloseCharIndex(text,')') - text.IndexOf('(') - 1);
        }
        public static string getParenthis(string text, int max = -1)
        {
            while (text.StartsWith(" "))
            {
                text = text.Substring(1, text.Length - 1);
            }
            while (text.EndsWith(" "))
            {
                text = text.Substring(0, text.Length - 1);
            }
            
            while (text.StartsWith("(") && text.EndsWith(")") && getCloseCharIndex(text, ')') == text.Length-1) {
                return getParenthis(text.Substring(text.IndexOf('(') + 1, getCloseCharIndex(text,')') - text.IndexOf('(') - 1), max-1);
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
        public static string[] getArgs(string text)
        {
            string[] args = smartSplit(getArg(text), ',');
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = smartExtract(args[i]);
            }
            return args;
        }
        public static void autoIndent(string text)
        {
            if (!smartEndWith(text, "{"))
            {
                autoIndented = 2;
            }
            if (text.Contains("{") && smartEndWith(text, "}"))
            {
                preparseLine(getCodeBlock(text));
                preparseLine("}");
                autoIndented = 0;
            }
        }
        public static int getOpenCharIndex(string text, char d)
        {
            int indent = 0;
            int index = 0;
            foreach(char c in text)
            {
                if (c == '{' && d != c)
                {
                    indent++;
                }
                else if (c == '{' && d == c)
                {
                    if (indent == 0)
                        return index;
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
                        return index;
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
                        return index;
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
                        return index;
                    else
                        indent++;
                }
                else if (c == ']')
                {
                    indent--;
                }

                index++;
            }
            return -1;
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

            foreach (string key in typeMaps.Keys)
            {
                if (text.ToLower().StartsWith(key.ToLower() + " "))
                {
                    return getEnum(typeMaps[key]);
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

            foreach (string key in typeMaps.Keys)
            {
                if (text.ToLower().StartsWith(key.ToLower() + " ")|| text.ToLower().StartsWith(key.ToLower() + "["))
                {
                    return getStruct(typeMaps[key]);
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
        public static string getClass(string text)
        {
            while (text.StartsWith(" "))
                text = text.Substring(1, text.Length - 1);
            text = text.Replace("{", " ") + " ";
            if (text.Contains("<"))
            {
                if (!classes.ContainsKey(text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]").ToLower()))
                {
                    string generics = text.Substring(text.IndexOf("<") + 1, getCloseCharIndex(text, '>') - text.IndexOf("<") - 1);
                    string name = smartEmpty(text.Substring(0, text.IndexOf("<"))).ToLower();

                    classes[name].createGeneric(text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]"),
                        smartSplit(smartEmpty(generics), ','));
                }
                return text.Substring(0, getCloseCharIndex(text, '>') + 1).Replace("<", "[").Replace(">", "]").ToLower();
            }

            foreach (string key in typeMaps.Keys)
            {
                if (text.ToLower().StartsWith(key.ToLower() + " ") || text.ToLower().StartsWith(key.ToLower() + "["))
                {
                    return getClass(typeMaps[key]);
                }
            }

            foreach (string key in classes.Keys)
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
        public static bool containClass(string text)
        {
            return getClass(text) != null;
        }
        public static string structMapAdd(string text)
        {
            string map = alphabet[structMap.Count() / alphabet.Length] +""+ alphabet[structMap.Count() % alphabet.Length]; ;
            structMap.Add(text, map);
            return map;
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
            if (!OffuscateNeed)
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
            structMap.Add(text, map);
            offuscationMap.Add(text, map);
            return map;
        }
        public static string getMap(long c)
        {
            return dirVar + "."
                        + "" + alphabet[(int)((c >> 52) & 63)]
                        + "" + alphabet[(int)((c >> 48) & 63)]
                        + "" + alphabet[(int)((c >> 42) & 63)]
                        + "" + alphabet[(int)((c >> 36) & 63)]
                        + "" + alphabet[(int)((c >> 30) & 63)]
                        + "" + alphabet[(int)((c >> 24) & 63)]
                        + "" + alphabet[(int)((c >> 18) & 63)]
                        + "" + alphabet[(int)((c >> 12) & 63)]
                        + "" + alphabet[(int)((c >> 6) & 63)]
                        + "" + alphabet[(int)(c & 63)];
        }

        public static ParenthiseError checkParenthisation(string[] file)
        {
            Stack<char> chars = new Stack<char>();
            int lineIndex = 0;
            foreach(string line in file)
            {
                lineIndex++;
                bool inComment = false;
                bool inString = false;
                char cPrev = '\n';

                int columns = 0;
                if (!line.Replace(" ","").Replace(" ","").StartsWith("/"))
                    foreach(char c in line)
                {
                    if (c == '"' && cPrev!= '\\')
                    {
                        inString = !inString;
                    }
                    else if (c == '\\' && cPrev == '\\')
                    {
                        inComment = true;
                    }
                    else if(c == '{' && !inComment && !inString)
                    {
                        chars.Push(c);
                    }
                    else if(c == '}' && !inComment && !inString && chars.Pop() != '{')
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
                    columns++;
                }
            }
            if (chars.Count == 0)
                return null;
            else
                return new ParenthiseError(++lineIndex, 0, chars.Pop());
        }


        public class Class
        {
            public string name;
            public Structure structure;
            public Variable pointer;

            public Class(string name)
            {
                this.name = name;
            }

            public void createGeneric(string name, string[] mType)
            {
                structure.createGeneric(name, mType);

                Class c = new Class(name);
                c.structure = structs[name];

                classes.Add(name, c);
            }

            public void CreateVariable(Variable var)
            {
                context.Sub(var.name, new File("", ""));
                foreach(Function f in structure.methods)
                {
                    preparseLine("def lazy "+f.name+"(){");
                    preparseLine("with(@e[tag=__cls__],false,"+var.gameName+ "=__ins_pointer__){");

                    preparseLine("}");
                    preparseLine("}");
                }
                context.Parent();
            }

            public string InstantiateClass(Variable var)
            {
                return parseLine("__cls_pointer__ += 1")+parseLine(var.gameName+"=__cls_pointer__");
            }
        }
        public class Function
        {
            public string name;
            public string gameName;
            public string desc;

            public bool lazy;
            public string package;
            public string variableStruct;

            public List<Argument> args = new List<Argument>();
            public List<Variable> outputs = new List<Variable>();
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

            public override string ToString()
            {
                return gameName;
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

            public string package;
            public bool isGeneric;
            public Structure parent;
            public bool isLazy;

            public File genericFile = new File("____","");

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
                        addVariable(varName, variable);
                        if (v.type == Type.ENUM)
                            variable.SetEnum(v.enums);

                        fields.Add(variable);
                    }

                    methods.AddRange(parent.methods);
                }
            }

            public void addGeneric(string v)
            {
                generic.Add(v);
            }
            public void createGeneric(string name, string[] mType)
            {
                for (int i = 0; i < generic.Count; i++)
                {
                    typeMaps.Add(generic[i].ToLower(), mType[i]);
                }
                context.Sub("_", new File("", ""));
                preparseLine("struct " + name + "{");
                foreach (string line in genericFile.parsed)
                {
                    preparseLine(line);
                }
                context.Parent();

                typeMaps.Clear();
            }

            public void addField(Variable variable)
            {
                fields.Insert(0, variable);
            }
            public void addMethod(Function variable)
            {
                methods.Add(variable);
                methodsName.Add(variable.name);
            }
            public void generateFunction(Function fun, Variable varOwner, string v, string cont)
            {
                string cont2 = context.GetVar();

                string funcName = fun.name;

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

                fFile.isLazy = fun.lazy;

                if (structStack.Count > 0)
                    function.isStructMethod = true;

                if (fun.isLoading)
                {
                    loadFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                    callTrace += "\"load\"->\"" + function.gameName + "\"\n";
                }

                if (fun.isTicking)
                {
                    mainFile.AddLine("function " + Project.ToLower() + ":" + fFile.name.Replace(".", "/").ToLower());
                    callTrace += "\"main\"->\"" + function.gameName + "\"\n";
                }

                if (fun.isHelper)
                {
                    fFile.use();
                    callTrace += "\"helper\"->\"" + function.gameName + "\"\n";
                }

                files.Add(fFile);
                string key = (context.GetFun() + funcName).Replace(':', '.').Replace('/', '.');
                if (functions.ContainsKey(key))
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
                    addVariable(varName, variable);
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
                            addVariable(varName2, variable2);
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
                            addVariable(context.GetVar() + v + "." + j.ToString(), var2);
                            variable.args.Add(arg);
                            j++;
                        }

                        j = 0;
                        foreach (Variable s in output.outputs)
                        {
                            Type type = s.type;
                            Variable var2 = new Variable(i.ToString(), context.GetVar() + v + ".ret_" + j.ToString(), type);
                            addVariable(context.GetVar() + v + ".ret_" + j.ToString(), var2);
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
                    if (variables.ContainsKey(context.GetInput() + arg.name))
                    {
                        variables.Remove(context.GetInput() + arg.name);
                    }
                    addVariable(context.GetInput() + arg.name, variable);

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
                            addVariable(varName, variable2);
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
                            addVariable(context.GetVar() + v + "." + j.ToString(), var2);
                            variable.args.Add(arg2);
                            j++;
                        }

                        j = 0;
                        foreach (Variable s in arg.outputs)
                        {
                            Type type = s.type;
                            Variable var2 = new Variable(i.ToString(), context.GetVar() + v + ".ret_" + j.ToString(), type);
                            addVariable(context.GetVar() + v + ".ret_" + j.ToString(), var2);
                            variable.outputs.Add(var2);
                            j++;
                        }
                    }

                    function.args.Add(variable);
                }
                
                context.Parent();
                Regex reg = new Regex("\\bthis\\.");
                foreach(string line in fun.file.parsed)
                {
                    fFile.parsed.Add(reg.Replace(line, context.GetVar()));
                }
            }
            public string generate(string v, bool entity, Variable varOwner, string instArg = null)
            {
                //v = structMapAdd(v);
                string cont = context.GetVar();
                string output = "";

                thisDef.Push(context.GetVar() + v+".");
                context.Sub(v, new File("", ""));
                context.currentFile().notUsed = true;

                foreach (Variable strVar in fields)
                {
                    string varName = context.toInternal(context.GetVar() + strVar.name);
                    strVar.CopyTo(varName, v, entity || strVar.entity, varOwner.isPrivate);
                }
                compVal[compVal.Count - 1].Add("$this", varOwner.uuid);
                compVal[compVal.Count - 1].Add("$this.enums", varOwner.enums);
                compVal[compVal.Count - 1].Add("$this.type", varOwner.GetTypeString());
                compVal[compVal.Count - 1].Add("$this.name", varOwner.gameName);
                compVal[compVal.Count - 1].Add("$this.scoreboard", varOwner.scoreboard());
                compVal[compVal.Count - 1].Add("$this.scoreboardname", varOwner.scoreboard().Split(' ')[1]);
                
                foreach (string c in compField.Keys)
                {
                    compVal[compVal.Count - 1].Add(c, compField[c]);
                }

                bool prev = isInStructMethod;
                isInStructMethod = false;
                adjPackage.Push(package);
                
                if (instArg != null)
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
                    GobalDebug(name, Color.Red);
                }

                thisDef.Pop();
                adjPackage.Pop();
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
            public string def;
            public bool isPrivate = false;
            public string privateContext;
            public bool isStructureVar;
            public int arraySize;
            private string score = null;
            public string uuid = "";
            public string typeArray;

            public List<Argument> args = new List<Argument>();
            public List<Variable> outputs = new List<Variable>();

            private Variable() { }
            public Variable(string name, string gameName, Type type, bool entity = false, string def="dummy", string forcedOffuscation="")
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

                if (entity && type != Type.STRUCT)
                {
                    if (forcedOffuscation == "")
                    {
                        uuid = offuscationMapAdd(gameName);
                        score = "@s " + uuid;
                        loadFile.AddScoreboardDefLine(Core.DefineVariable(this));
                    }
                    else
                    {
                        uuid = forcedOffuscation;
                        score = "@s "+forcedOffuscation;
                        loadFile.AddScoreboardDefLine(Core.DefineVariable(this));
                    }
                }
                else
                {
                    if (forcedOffuscation != "")
                    {
                        uuid = offuscationMapAdd(gameName);
                        score = forcedOffuscation + (isConst ? " tbms.const" : " tbms.value");
                        loadFile.AddScoreboardDefLine(Core.DefineVariable(this));
                    }
                    else
                    {
                        uuid = offuscationMapAdd(gameName);
                    }
                }
            }
            public virtual void SetEnum(string enums)
            {
                this.enums = enums;
            }
            public virtual string scoreboard()
            {
                if (isPrivate && !context.GetVar().StartsWith(privateContext))
                    throw new Exception("can not asign private variable in context: "+ context.GetVar() +" from "+ privateContext);

                if (score != null)
                    return score;

                if (entity)
                {
                    score = "@s "+offuscationMapAdd(gameName);
                    return score;
                }
                else
                {
                    score = offuscationMapAdd(gameName) + (isConst ? " tbms.const" : " tbms.value");
                    return score;
                }
            }

            public void CreateArray()
            {
                if (isPrivate)
                    parseLine("private const int " + name + ".length = "+arraySize.ToString());
                else
                    parseLine("const int " + name + ".length = " + arraySize.ToString());

                for (int i = 0; i < arraySize; i++)
                {
                    if (isPrivate)
                        parseLine("private " + typeArray + " " + name + "." + i.ToString());
                    else
                        parseLine(typeArray + " " + name + "." + i.ToString());
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

                addVariable(newName, variable);
                
                if (this.type == Type.ENUM)
                    variable.SetEnum(this.enums);

                if (this.type == Type.ARRAY)
                {
                    variable.arraySize = arraySize;
                    variable.CreateArray();
                }

                /*
                if (structStack.Count > 0 && !isInStructMethod)
                    structStack.Peek().addField(variable);
                    */
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
                        addVariable(newName + "." + i.ToString(), var2);
                        variable.args.Add(arg);
                        i++;
                    }

                    i = 0;
                    foreach (Variable s in this.outputs)
                    {
                        Type type = s.type;
                        Variable var2 = new Variable(i.ToString(), newName + ".ret_" + i.ToString(), type);
                        addVariable(newName + ".ret_" + i.ToString(), var2);
                        variable.outputs.Add(var2);
                        i++;
                    }
                }
                return variable;
            }
            public Variable Select(string entitySelector)
            {
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
                var.args = args;
                var.outputs = outputs;

                return var;
            }
            public string GetTypeString()
            {
                if (type == Type.ENUM || type == Type.STRUCT)
                {
                    return type.ToString() + "(" + enums + ")";
                }
                else if (type == Type.FUNCTION)
                {
                    string args2 = "";
                    foreach(Argument arg in args)
                    {
                        args2 += arg.GetTypeString() + ",";
                    }
                    string outs = "";
                    foreach (Variable arg in outputs)
                    {
                        outs += arg.GetTypeString() + ",";
                    }
                    return type.ToString() + "<(" + args2 +"),("+ outs + ")>";
                }
                else
                    return type.ToString();
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
                        if (type == "json" && val.StartsWith("(("))
                        {
                            value = getArg(val);
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
                        if (type == "json" && val.StartsWith("(("))
                        {
                            value = getArg(val);
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
                EnumValue v = values[valuesName.IndexOf(value)];
                if (v.fields.ContainsKey(field))
                {
                    return v.fields[field];
                }
                else
                {
                    return fields[fieldsName.IndexOf(field)].defaultVal;
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
        public class CompilerSetting
        {
            public int TreeMaxSize = 20;
            public int FloatPrecision = 1000;
        }

        public class Argument : Variable
        {
            public string defValue = null;
            public bool isImplicite;
            public Argument(string name, string gameName, Type type) : base(name, gameName, type)
            {

            }
            public Variable variable;
        }
        public enum Type
        {
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
            SELECTOR,
            RANGE
        }
        public class File
        {
            public string name;
            public string content;
            public string abstractContent;
            public string ending;
            public string start = "";
            public string scoreboardDef = "";
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

            public Function function;

            public bool notUsed = false;
            public List<File> childs = new List<File>();
            public File parent;

            public File(string name, string content = "", string type="")
            {
                this.name = name;
                this.content = content;
                this.type = type;
            }

            public File AddLine(string cont)
            {
                if (cont != "")
                {
                    content += cont + '\n';

                    if (cont.Contains('\n'))
                        lineCount += cont.Split('\n').Length - 1;
                    else
                        lineCount++;
                }
                return this;
            }
            public void AddScoreboardDefLine(string cont)
            {
                scoreboardDef = cont + '\n' + scoreboardDef;
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
            public void generate(string value, Enum enums = null)
            {
                isInLazyCompile = 0;

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
                    line = line.Replace(var + ".index", genIndex.ToString())
                        .Replace(var + ".length", genAmount.ToString())
                        .Replace(var, value);

                    preparseLine(line);
                    
                    /*
                    if (isInLazyCompile > 0)
                    {
                        if (smartContains(line, '{'))
                            isInLazyCompile += 1;
                        if (smartContains(line, '}'))
                            isInLazyCompile -= 1;
                    }

                    if (isInLazyCompile > 0)
                        context.currentFile().addParsedLine(line);

                    string res;
                    if (isInLazyCompile == 0)
                        res = parseLine(line);
                    else
                    {
                        res = "";
                    }

                    context.currentFile().AddLine(res);

                    if (line == "}" && isInLazyCompile == 0)
                    {
                        context.currentFile().Close();
                    }*/
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
                    string[] values = enumGen.Replace("(", "").Replace(")", "").Split(',');
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
                content = scoreboardDef + start + content + ending;
                ending = "";
                start = "";

                while (content.Contains("\n\n"))
                {
                    content = content.Replace("\n\n", "\n");
                }
                lineCount = content.Split('\n').Length - 1;

                context.Parent();

                if (type == "forgenerate")
                {
                    forgenerate();
                }

                if (type == "if")
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
                if ((type == "if" || (type == "with" && !cantMergeWith) || type == "at" || type == "case") && lineCount == 1 && !content.StartsWith("#"))
                {
                    File f = context.currentFile();
                    string tmp = f.content.Substring(0, f.content.LastIndexOf(' '));
                    tmp = tmp.Substring(0, tmp.LastIndexOf(' '));
                    tmp += " "+content;
                    f.content = tmp;
                    files.Remove(this);
                }
                if (type == "switch" || type == "forgenerate")
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
                    GobalDebug("Warning! maxCommandChainLength exceeded in " + name, Color.Yellow);
                }
                if (type == "struct")
                {
                    structStack.Pop();
                    context.currentFile().unuse();
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
                condIDStack = new Stack<int>();
                LastConds = new Stack<int>();
                lazyOutput = new Stack<List<Variable>>();
                lazyCall = new Stack<Function>();
                lazyEvalVar = new List<Dictionary<string, string>>();
                LastCond = -1;
                currentFile = name;
                currentLine = 1;
                inGenericStruct = false;
                isInFunctionDesc = false;
                isInStaticMethod = false;
                isInLazyCompile = 0;
                callingFunctName = function.gameName;
                try
                {
                    ParenthiseError parenthiseError = checkParenthisation(parsed.ToArray());
                    if (parenthiseError != null)
                    {
                        parenthiseError.throwException();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                adjPackage.Push(function.package);
                foreach (string line in parsed)
                {
                    preparseLine(line);
                }
                context.currentFile().Close();
                adjPackage.Pop();
                UnparsedFunctionFile = false;
                //GobalDebug("Function: "+name+" Compiled ("+ parsed.Count.ToString()+" lines)", Color.LimeGreen);
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
                    compVal.Add(new Dictionary<string, string>());
            }
            public void Parent()
            {
                if (directories.Count > 1)
                {
                    directories.RemoveAt(directories.Count - 1);
                    files.RemoveAt(files.Count - 1);
                    if (compVal.Count > 0 && !structInstCompVar)
                        compVal.RemoveAt(compVal.Count-1);
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
                string output = directories[0].Substring(0,Math.Min(subDir, directories[0].Length)) + ".";

                for (int i = 1; i < directories.Count; i++)
                {
                    output += directories[i].Substring(0, Math.Min(subDir, directories[i].Length)) + ".";
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
            public string toInternal(string value, int maxC = 2)
            {
                if (maxC == 2)
                {
                    maxC = subDir;
                }
                string[] v = smartEmpty(value).Split('.');
                string output = "";
                for (int i = 0; i < v.Length-1; i++)
                {
                    if (packageMap.ContainsKey(v[i]))
                    {
                        output += packageMap[v[i]] + ".";
                    }
                    else if (structMap.ContainsKey(v[i]))
                    {
                        output += structMap[v[i]] + ".";
                    }
                    else
                    {
                        output += v[i].Substring(0, Math.Min(maxC, v[i].Length)) + ".";
                    }
                }
                output += v[v.Length-1];
                return output;
            }

            public string GetFunctionName(string func, bool safe = false, bool bottleneck = false)
            {
                func = toInternal(smartEmpty(func).ToLower(), 256);
                if (functions.ContainsKey(func))
                {
                    return func;
                }
                string dir = "";
                string output = null;
                
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
                if (safe)
                    return null;
                throw new Exception("UNKNOW FUNCTION (" + dir+func+ ")");
            }

            public bool IsFunction(string func, bool bottleneck = false)
            {
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
                        bool val = IsFunction(pack + "." + func, true);
                        if (val)
                        {
                            return val;
                        }
                    }
                }

                return false;
            }

            public string GetVariableName(string func, bool safe = false, bool bottleneck = false)
            {
                func = toInternal(smartEmpty(func));
                if (containLazyVal(func))
                {
                    return GetVariableName(getLazyVal(func), safe);
                }
                if (func.StartsWith("this.") && variables.ContainsKey(func.Replace("this.", thisDef.Peek())))
                {
                    return Compiler.GetVariable(func.Replace("this.", thisDef.Peek())).scoreboard();
                }

                if (variables.ContainsKey(func))
                {
                    return Compiler.GetVariable(func).scoreboard();
                }
                string dir = "";
                string output=null;
                foreach (string co in directories)
                {
                    dir += co.Substring(0, Math.Min(subDir, co.Length)) + ".";
                    if (variables.ContainsKey(dir + func))
                    {
                        output = Compiler.GetVariable(dir + func).scoreboard();
                    }
                }
                if (output != null)
                    return output;

                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach (string pack in adjPackage)
                    {
                        string var = GetVariableName(pack + "." + func, true, true);
                        if (var != null)
                        {
                            return var;
                        }
                    }
                }

                if (!safe)
                    throw new Exception("UNKNOW Variable (" + dir + func + ")");
                else
                    return null;
            }

            public string GetVariable(string func, bool safe = false, bool bottleneck = false)
            {
                func = toInternal(func.Replace(" ", ""));

                if (containLazyVal(func))
                {
                    return GetVariable(getLazyVal(func), safe);
                }
                if (func.StartsWith("this.") && variables.ContainsKey(func.Replace("this.", thisDef.Peek())))
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
                    dir += co.Substring(0, Math.Min(subDir, co.Length)) + ".";
                    if (variables.ContainsKey(dir + func))
                    {
                        output = dir + func;
                    }
                }
                
                if (output != null)
                    return output;

                string adj = "";
                if (adjPackage.Count > 0 && !bottleneck)
                {
                    foreach(string pack in adjPackage)
                    {
                        string var = GetVariable(pack + "." + func, true, true);
                        if (var != null)
                        {
                            return var;
                        }
                        adj += pack + ", ";
                    }
                }

                if (!safe)
                    throw new Exception("UNKNOW Variable (" + dir +"/"+ func + ") with package: "+adj);
                else
                    return null;
            }

            public string GetEntityName(string value)
            {
                if (value.Contains("@"))
                {
                    if (!CommandParser.isValidSelector(value.Split('.')[0]))
                        throw new Exception("Invalid Selctor " + value.Split('.')[0]);
                    return value.Split('.')[0];
                }
                if (value.Contains('.'))
                {
                    string ent = value.Split('.')[0];
                    return (GetVariableName(ent)).Split(' ')[0];
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
                        if (!CommandParser.isValidSelector(value))
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
                        if (!CommandParser.isValidSelector(value))
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
                    if (!CommandParser.isValidSelector(value))
                        throw new Exception("Invalid Selctor "+value);
                    return smartEmpty(value);
                }
                else if (single)
                    return "@e[tag=" + context.GetVariableName(value.Split('.')[0]).Split(' ')[0] + "]";
                else
                    return "@e[limit=1,tag=" + context.GetVariableName(value.Split('.')[0]).Split(' ')[0] + "]";

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

                    return (GetVariableName(ent, true) != null && (Compiler.GetVariable(GetVariable(ent, true)).type == Type.ENTITY || Compiler.GetVariable(GetVariable(ent, true)).type == Type.SELECTOR));
                }
                else if (GetVariable(value,true)!=null && GetVariableByName(value).type == Type.SELECTOR)
                {
                    return true;
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
