using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

[ExecuteInEditMode]
public class EZBuild : ScriptableObject
{
    public string sceneFolderRelativePath = "Assets/Scenes";

    public string firstSceneRelativePath = "Assets/Scenes/DefaultScene.unity";

    static string assetPath = "Assets/Plugins/EZBuild/Editor/EZBuildData.asset";

    const string dataAssetName = "EZBuildData.asset";

    private void Awake()
    {
        UpdateDataAssetPath();

    }

    private void OnEnable()
    {
        UpdateDataAssetPath();
    }

    private void UpdateDataAssetPath()
    {
        MonoScript script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);

        FileInfo fi = new FileInfo(scriptPath);
        string assetFolder = fi.Directory.ToString();

        assetPath = Path.Combine(assetFolder, dataAssetName);
        assetPath = assetPath.Replace('\\', '/');
        assetPath = GetRelativePath(assetPath);
    }

    private static EZBuild GetDataInstance()
    {
        EZBuild dataInstance = null;

        string fullPath = (Application.dataPath + '/' + assetPath.Substring(assetPath.IndexOf('/') + 1)).Replace("/", "\\");
        Debug.Log("fullpath : " + fullPath);
        if (File.Exists(fullPath))
        {
            dataInstance = AssetDatabase.LoadAssetAtPath<EZBuild>(assetPath);
            Debug.Log("get data instance");
        }
        else
        {
            dataInstance = ScriptableObject.CreateInstance<EZBuild>();
            AssetDatabase.CreateAsset(dataInstance, assetPath);
            Debug.Log("create new instance");
        }
        return dataInstance;
    }

    // Retrieve the relative path (starting with "Assets") from the full path
    private static string GetRelativePath(string path)
    {
        string relativePath = "";
        if (path.Contains("Assets"))
        {
            relativePath = path.Substring(path.IndexOf("Assets"));
        }
        else
        {
            Debug.LogError("Folder should be somewhere within 'Assets/' folder");
        }

        return relativePath;
    }

    //public string GetCurrentScriptPath() {
    //    MonoScript script = MonoScript.FromScriptableObject(this);
    //    assetPath = AssetDatabase.GetAssetPath(script);

    //    return "";
    //}

    [MenuItem("EZBuild/Set First Scene")]
    public static void SetFirstScene()
    {
        EZBuild dataInstance = GetDataInstance();

        // TODO: handle when selecting nothing
        // Display file picker from OS
        string firstSceneFullPath = EditorUtility.OpenFilePanel("Select first scene to build",
                                                                 Path.Combine(Application.dataPath, dataInstance.firstSceneRelativePath),
                                                                 "unity");
        if (firstSceneFullPath == "")
        {
            return;
        }

        dataInstance.firstSceneRelativePath = GetRelativePath(firstSceneFullPath);
        EditorUtility.SetDirty(dataInstance);
        AssetDatabase.SaveAssets();

        Debug.Log("first scene for EZ building is set to " + dataInstance.firstSceneRelativePath);

    }

    [MenuItem("EZBuild/Set Scene Folder")]
    public static void SetSceneFolder()
    {
        EZBuild dataInstance = GetDataInstance();

        string[] foldersTab = dataInstance.sceneFolderRelativePath.Split("/".ToCharArray());

        string folderName = "";
        if (foldersTab.Length > 0)
        {
            folderName = foldersTab[foldersTab.Length - 1];
        }

        // Display folder picker from OS
        string sceneFolderFullPath = EditorUtility.OpenFolderPanel("Select folder with Scene files that need to be built",
                                                                    Path.Combine(Application.dataPath, dataInstance.sceneFolderRelativePath),
                                                                    folderName);

        if (sceneFolderFullPath == "")
        {
            return;
        }

        dataInstance.sceneFolderRelativePath = GetRelativePath(sceneFolderFullPath);
        EditorUtility.SetDirty(dataInstance);
        AssetDatabase.SaveAssets();

        Debug.Log("Scene folder for EZ building is set to " + dataInstance.sceneFolderRelativePath);
    }

    [MenuItem("EZBuild/EZ Build Current Scene Only")]
    public static void EZBuildCurrentSceneOnly()
    {
        BuildWithScenes(new[] { SceneManager.GetActiveScene().path }, GetBuildOptions());
    }

    [MenuItem("EZBuild/EZ Build Win64")]
    public static void EZBuildWin64()
    {
        Debug.Log("EZ Building for Windows 64bit...");

        EZBuild dataInstance = GetDataInstance();

        string[] scenesAssetsGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { dataInstance.sceneFolderRelativePath });

        List<string> scenesRelativePath = new List<string>(new[] { dataInstance.firstSceneRelativePath });

        foreach (string sceneAssetGUID in scenesAssetsGUIDs)
        {
            scenesRelativePath.Add(AssetDatabase.GUIDToAssetPath(sceneAssetGUID));
        }

        BuildWithScenes(scenesRelativePath.ToArray());
    }

    [MenuItem("EZBuild/EZ Build Win64 Script Only")]
    public static void EZBuildWin64ScriptOnly()
    {
        Debug.Log("EZ Script Only Building for Windows 64bit...");

        EZBuild dataInstance = GetDataInstance();

        string[] scenesAssetsGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { dataInstance.sceneFolderRelativePath });

        List<string> scenesRelativePath = new List<string>(new[] { dataInstance.firstSceneRelativePath });

        foreach (string sceneAssetGUID in scenesAssetsGUIDs)
        {
            scenesRelativePath.Add(AssetDatabase.GUIDToAssetPath(sceneAssetGUID));
        }

        BuildWithScenes(scenesRelativePath.ToArray(), GetBuildOptions(scriptOnly: true));
    }

    private static void BuildWithScenes(string[] scenesPaths, BuildOptions buildOptions = BuildOptions.ShowBuiltPlayer | BuildOptions.PatchPackage | BuildOptions.Development)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenesPaths;
        buildPlayerOptions.locationPathName = "Builds/Win64Build.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        // TODO: use the build options patchpackage if the game has already been built for faster building
        //buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer | BuildOptions.PatchPackage | BuildOptions.Development;
        buildPlayerOptions.options = buildOptions;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;


        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded in " + summary.totalTime + " of size " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed with " + summary.totalErrors + " errors");

        }
    }

    private static BuildOptions GetBuildOptions(bool scriptOnly = false)
    {
        BuildOptions buildOptions = BuildOptions.ShowBuiltPlayer | BuildOptions.PatchPackage | BuildOptions.Development;

        if (scriptOnly)
        {
            return buildOptions | BuildOptions.BuildScriptsOnly;
        }

        return buildOptions;

    }
}
