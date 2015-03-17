using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Linq;


[AddComponentMenu("Debug/Oculus Debug")]
[RequireComponent(typeof(Canvas))]
public class OculusDebug : MonoBehaviour {
    private static OculusDebug _instance;

    private Text console;

    private const int maxLines = 28;
    private List<string> lines = new List<string>(maxLines);


    void Start()
    {        
        console = gameObject.GetComponentInChildren<Text>();
        console.text = "";
    }

    void Update()
    {
        var camera = GameObject.Find("OVRCameraRig") ?? GameObject.Find("LeapOVRCameraRig");
          
        // Orientate
        gameObject.transform.position = camera.transform.position + camera.transform.forward * 0.5f + camera.transform.right * 0.2f;
        gameObject.transform.rotation =
            Quaternion.LookRotation(gameObject.transform.position - camera.transform.position, camera.transform.up);

        console.text = string.Join("\n", lines.Select(l => "> " + l).ToArray());

    }


    public static OculusDebug Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject holder = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Console")) as GameObject;
                _instance = holder.AddComponent<OculusDebug>();

                // Start deactivated
                //holder.SetActive(false);
            }
            return _instance;
        }
    }

    private void log(object obj)
    {
            
        if (lines.Count >= maxLines)
        {
            lines.RemoveAt(0);
        }
        lines.Add(obj.ToString());
            
    }


    public static void Log(object obj)
    {
        OculusDebug.Instance.log(obj);
    }
    public static void LogError(object obj)
    {
        OculusDebug.Instance.log("<color=red>" + obj + "</color>");
    }

    public static void LogWarning(object obj)
    {
        OculusDebug.Instance.log("<color=yellow>" + obj + "</color>");
    }
}
