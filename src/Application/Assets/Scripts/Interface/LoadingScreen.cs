using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Gestures;

public class LoadingScreen : Singleton<LoadingScreen> {
    private Image fill;
    private CanvasGroup shutter;
    private int okCount = 0;

    public GameObject shimCamera;


	// Use this for initialization
	void Awake () {
        fill = transform.FindRecursively("Fill").GetComponent<Image>();
        shutter = transform.GetComponentInChildren<CanvasGroup>();

        //var materials = GetComponentsInChildren<CanvasRenderer>()
        //                .Select(c => c.GetMaterial())
        //                .Where(m => m != null);

        //Debug.LogFormat("Materials found: {0}", materials.Count());

	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(ImageAtlas.Progress);
        fill.fillAmount = ImageAtlas.Progress;

        if (!ImageAtlas.IsLoading) okCount++;
        else okCount = 0;

        if (okCount > 5)
        {
            okCount = 0;
            enabled = false;
        }
	}

    void OnEnable()
    {

        HandProvider.Instance.gameObject.SetActive(false);

        //shimCamera.SetActive(true);
        shutter.alpha = 1;
        foreach (var camera in shimCamera.GetComponentsInChildren<Camera>())
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
        }
    }

    void OnDisable()
    {
        HandProvider.Instance.gameObject.SetActive(true);

        OVRManager.capiHmd.RecenterPose();
       
        shutter.DOFade(0, 1).OnComplete(delegate
        {
            foreach (var camera in shimCamera.GetComponentsInChildren<Camera>())
            {
                camera.clearFlags = CameraClearFlags.Nothing;
            }
        });
    }

}
