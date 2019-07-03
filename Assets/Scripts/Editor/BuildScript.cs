using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class BuildScript : Editor {
    private static String exportDir;

    static void Init()
    {
        exportDir = CommandLineReader.GetCustomArgument("exportDir");
        UnityEditor.PlayerSettings.applicationIdentifier = CommandLineReader.GetCustomArgument("applicationIdentifier");
        UnityEditor.PlayerSettings.companyName = CommandLineReader.GetCustomArgument("companyName");
        UnityEditor.PlayerSettings.productName = CommandLineReader.GetCustomArgument("productName");
        UnityEditor.PlayerSettings.bundleVersion = CommandLineReader.GetCustomArgument("bundleVersion");
        UnityEditor.PlayerSettings.iOS.buildNumber = CommandLineReader.GetCustomArgument("iOS.buildNumber");
        UnityEditor.PlayerSettings.Android.bundleVersionCode = int.Parse(CommandLineReader.GetCustomArgument("Android.bundleVersionCode"));
        UnityEditor.PlayerSettings.Android.keystorePass = CommandLineReader.GetCustomArgument("Android.keystorePass");
        UnityEditor.PlayerSettings.Android.keyaliasName = CommandLineReader.GetCustomArgument("Android.keyaliasName");
        UnityEditor.PlayerSettings.Android.keyaliasPass = CommandLineReader.GetCustomArgument("Android.keyaliasPass");
        UnityEditor.PlayerSettings.Android.useAPKExpansionFiles = bool.Parse(CommandLineReader.GetCustomArgument("Android.useAPKExpansionFiles"));

        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.BuildTargetGroup.iOS, CommandLineReader.GetCustomArgument("iOS.ScriptingDefineSymbols"));
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.BuildTargetGroup.Android, CommandLineReader.GetCustomArgument("Android.ScriptingDefineSymbols"));
    }

    static void BuildIOS()
    {
        Init();

        BuildPipeline.BuildPlayer(GetSceneNameArray(), exportDir, BuildTarget.iOS, BuildOptions.SymlinkLibraries);
    }

    static void BuildAndroid()
    {
        Init();

        BuildPipeline.BuildPlayer(GetSceneNameArray(), exportDir, BuildTarget.Android, BuildOptions.None);
    }

    private static string[] GetSceneNameArray()
    {
        List<string> sceneNameList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneNameList.Add(scene.path);
            }
        }
        return sceneNameList.ToArray();
    }

    public class CommandLineReader
    {
        //Config
        private const string CUSTOM_ARGS_PREFIX = "-CustomArgs:";
        private const char CUSTOM_ARGS_SEPARATOR = ';';

        public static string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }

        public static string GetCommandLine()
        {
            string[] args = GetCommandLineArgs();

            if (args.Length > 0)
            {
                return string.Join(" ", args);
            }
            else
            {
                Debug.LogError("CommandLineReader.cs - GetCommandLine() - Can't find any command line arguments!");
                return "";
            }
        }

        public static Dictionary<string, string> GetCustomArguments()
        {
            Dictionary<string, string> customArgsDict = new Dictionary<string, string>();
            string[] commandLineArgs = GetCommandLineArgs();
            string[] customArgs;
            string[] customArgBuffer;
            string customArgsStr = "";

            try
            {
                customArgsStr = commandLineArgs.Where(row => row.Contains(CUSTOM_ARGS_PREFIX)).Single();
            }
            catch (Exception e)
            {
                Debug.LogError("CommandLineReader.cs - GetCustomArguments() - Can't retrieve any custom arguments in the command line [" + commandLineArgs + "]. Exception: " + e);
                return customArgsDict;
            }

            customArgsStr = customArgsStr.Replace(CUSTOM_ARGS_PREFIX, "");
            customArgs = customArgsStr.Split(CUSTOM_ARGS_SEPARATOR);

            foreach (string customArg in customArgs)
            {
                customArgBuffer = customArg.Split('=');
                if (customArgBuffer.Length == 2)
                {
                    customArgsDict.Add(customArgBuffer[0], customArgBuffer[1]);
                }
                else
                {
                    Debug.LogWarning("CommandLineReader.cs - GetCustomArguments() - The custom argument [" + customArg + "] seem to be malformed.");
                }
            }

            return customArgsDict;
        }

        public static string GetCustomArgument(string argumentName)
        {
            Dictionary<string, string> customArgsDict = GetCustomArguments();

            if (customArgsDict.ContainsKey(argumentName))
            {
                return customArgsDict[argumentName];
            }
            else
            {
                Debug.LogError("CommandLineReader.cs - GetCustomArgument() - Can't retrieve any custom argument named [" + argumentName + "] in the command line [" + GetCommandLine() + "].");
                return "";
            }
        }
    }
}
