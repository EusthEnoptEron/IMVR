using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;

public class VHBuild  {
    private static string[] filesToCopy = {
        "Database.s3db",
        "Plugins/sqlite3.dll",
        "Plugins/sqlite3.def"                      
    };

	/// <summary>
	/// Replaces the built standalone with an auto-patcher for the same architecture.
	/// </summary>
	[PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {
            var dataFolder = pathToBuiltProject.Replace(".exe", "_Data");

            foreach (var fileToCopy in filesToCopy)
            {
                string from = Path.Combine(Application.dataPath, fileToCopy);
                string to = Path.Combine(dataFolder, fileToCopy);
                //Debug.Log(System.String.Format("{0} -> {1}", from, to));

                if (File.Exists(from))
                {
                    File.Copy(from, to);
                }
            }
        }
    }
}
