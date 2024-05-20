using UnityEditor.Android;
using UnityEngine;
using System.IO;

class GradleBuildHax : IPostGenerateGradleAndroidProject
{
    private const string ToReplace = "def unityProjectPath = $/file:////$.replace(\"\\\\\", \"/\")";
	
    public int callbackOrder { get { return 0; } }
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("GradleBuildHax at path " + path);
		
        var projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace("\\", "/");
		
        Debug.Log("GradleBuildHax projectPath " + projectPath);
        
        var gradlePath = Path.Combine(path, "build.gradle");
		
        var targetValue = "def unityProjectPath = \"file:///" + projectPath + "\"";
        
        var text = File.ReadAllText(gradlePath);
        text = text.Replace(ToReplace, targetValue);
        File.WriteAllText(gradlePath, text);
		
        Debug.Log("GradleBuildHax gradle:\n" + text);
    }
}