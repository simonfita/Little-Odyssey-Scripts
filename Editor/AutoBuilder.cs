using Codice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class AutoBuilder
{
    const string buildPath = "D:/MerchantBuild";
    const string steamBatPath = "D:/MerchantBuild/steamworks_sdk_159/sdk/tools/ContentBuilder/";
    const string steamContentPath = "D:/MerchantBuild/steamworks_sdk_159/sdk/tools/ContentBuilder/content";

    static readonly string[] levels = new string[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Persistant.unity", "Assets/Scenes/TongsDelta.unity", "Assets/Scenes/DreamMinigame.unity","Assets/Scenes/DemoEnd.unity" };
    //static readonly string[] testLevels = new string[] { "Assets/Scenes/MainMenu.unity" };



    public static string GetCurrentCL()
    {
        string[] cmVersions = CmdRunner.ExecuteCommandWithResult("cm find changesets --format=\"{changesetid}\" --nototal".Split(' '), Environment.CurrentDirectory).Output.Split("\n");

        string currentVersion = cmVersions[cmVersions.Length - 2].Trim();

        return currentVersion;
    }

    private static bool Build( BuildTarget target, BuildOptions options,string fileSuffix, bool demo, string clSuffix, string buildBat)
    {
        string cl = GetCurrentCL();
        cl += clSuffix;
        BuildData data = Resources.Load<BuildData>("BuildData");
        data.changeset = cl;
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssetIfDirty(data);

        PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, !demo? new string[] { "UNITY_PIPELINE_URP" }: new string[] { "UNITY_PIPELINE_URP", "DEMO" });


        string path = buildPath + "/" + cl + "/LittleOdyssey";
        var output = BuildPipeline.BuildPlayer(levels, path + "/LittleOdyssey"+ fileSuffix, target, options);

        if (output.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            return false;

        FileUtil.DeleteFileOrDirectory(steamContentPath + "/LittleOdyssey");
        FileUtil.CopyFileOrDirectory(path, steamContentPath + "/LittleOdyssey");

        ProcessStartInfo info = new ProcessStartInfo(steamBatPath + buildBat);
        info.WorkingDirectory = steamBatPath;

        Process.Start(info);
        return true;

    }



    [MenuItem("File/AutoBuilder/AllReleaseBuilds", false)]
    public static void BuildsReleaseAll()
    {
        BuildRelease();
        BuildReleaseLinux();
    }
    
    [MenuItem("File/AutoBuilder/ReleaseBuild", false)]
    public static void BuildRelease()
    {
        Build(BuildTarget.StandaloneWindows, BuildOptions.None, ".exe", false, "","run_build.bat");
    }

    [MenuItem("File/AutoBuilder/ReleaseBuildLinux", false)]
    public static void BuildReleaseLinux()
    {
        Build(BuildTarget.StandaloneLinux64, BuildOptions.None, ".x86_64", false, "-linux", "run_build_linux.bat");
    }



    [MenuItem("File/AutoBuilder/DevelopmentBuild", false)]
    public static void BuildDevelopment()
    {
        Build(BuildTarget.StandaloneWindows, BuildOptions.Development, ".exe", false, "-dev", "run_build_dev.bat");

    }

    [MenuItem("File/AutoBuilder/DevelopmentLinuxBuild", false)]
    public static void BuildDevelopmentLinux()
    {
        Build(BuildTarget.StandaloneLinux64, BuildOptions.Development, ".x86_64", false, "-dev-linux", "run_build_dev_linux.bat");

    }



    [MenuItem("File/AutoBuilder/DemoBuild", false)]
    public static void BuildDemo()
    {
        Build(BuildTarget.StandaloneWindows, BuildOptions.None, ".exe", true, "-demo", "run_build_demo.bat");
    }

    [MenuItem("File/AutoBuilder/DemoLinuxBuild", false)]
    public static void BuildDemoLinux()
    {
        Build(BuildTarget.StandaloneLinux64, BuildOptions.None, ".x86_64", true, "-demo-linux", "run_build_demo_linux.bat");
    }



}
