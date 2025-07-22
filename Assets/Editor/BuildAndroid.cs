using UnityEditor;
using System.IO;

public class BuildAndroid
{
    public static void Build()
    {
        string[] scenes = { "Assets/Scenes/Cards.unity" };//主场景路径
        string outPath = "Build/android_build.apk";       //构建产物路径

        //确保构建目录存在
        Directory.CreateDirectory("Build");

        BuildPipeline.BuildPlayer(scenes, outPath, BuildTarget.Android, BuildOptions.None);
    }
    
}
