using UnityEngine;
using System.Collections;

public class EnableOculusDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.logMessageReceived += HandleLog;
	}

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Warning)
        {
            OculusDebug.LogWarning(condition);
        }
        else if (type == LogType.Error || type == LogType.Exception)
        {
            OculusDebug.LogError(condition);
        }
        else
        {
            OculusDebug.Log(condition);
        }
    }
	
}
