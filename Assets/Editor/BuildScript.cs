using UnityEditor;
using System.IO;

public class BuildScript
{
    public static void PerformBuild()
    {
        string[] scenes = { "Assets/Scenes/Intro.unity", "Assets/Scenes/Main.unity" };

        // ºôµå ´ë»ó ÇÃ·§Æû¿¡ µû¶ó ºôµå °æ·Î¿Í Å¸°ÙÀ» ¼³Á¤
        string buildPath = "Builds/";
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        if (target == BuildTarget.WebGL)
        {
            buildPath += "WebGL";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);
        }
        else if (target == BuildTarget.Android)
        {
            buildPath += "Android";
            BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
        }
        else
        {
            throw new System.Exception("Unsupported build target.");
        }
    }
}
