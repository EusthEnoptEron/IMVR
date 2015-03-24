using UnityEngine;
using System.Collections;
using Gestures;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class RingMenuItem : UIBehaviour {
    public FingerType fingerType = FingerType.Index;
    public float pixelsRatio = 1000;
    public Color color = Color.blue;
    public float heightDifference = 0;

    private GameObject torus;
    //private UILineRenderer lineRenderer;
    private LineRenderer lineRenderer;

    public float Progress { get; set; }

	// Use this for initialization
	protected virtual void Awake () {
        torus = GameObject.Instantiate<GameObject>(Resources.Load("Prefabs/Torus") as GameObject);
        torus.transform.SetParent(transform);

        //torus.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);
        torus.GetComponentInChildren<Image>().color = color;

        lineRenderer = new GameObject().AddComponent<LineRenderer>();
        //lineRenderer.transform.SetParent(transform);

        lineRenderer.transform.localPosition = Vector3.zero;
        lineRenderer.transform.localRotation = Quaternion.identity;
        lineRenderer.transform.localScale = Vector3.one;

      //  InitLineRenderer();
	}


	// Update is called once per frame
	protected virtual void Update () {
        var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
        if (hand != null)
        {
            var finger = hand.GetFinger(fingerType);
            var bone   = finger.GetBone(BoneType.Intermediate);

            // Place myself
            Vector3 distance = new Vector3(0.1f, heightDifference, 0f);

            transform.position = finger.GetBone(BoneType.Distal).Position;//hand.PalmPosition + Vector3.ProjectOnPlane(finger.GetBone(BoneType.Intermediate).Position - hand.PalmPosition, Camera.main.transform.forward) * 2f;// + Camera.main.transform.TransformDirection(distance);
            transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized);

            // Place torus
            torus.GetComponentInChildren<Image>().transform.localRotation *= Quaternion.Euler(0, 0, (180 + Progress * 1000) * Time.deltaTime);

            BoneType startBone = Progress > 0.5f ? BoneType.Intermediate : BoneType.Distal;
            BoneType endBone   = startBone - 1;
            var v1 = finger.GetBone(startBone).Position;
            var v2 = finger.GetBone(endBone).Position;

            torus.transform.position = Vector3.Lerp(v1, v2, (Progress * 2) % 1 );

            if (Progress > 0)
                torus.transform.rotation = Quaternion.LookRotation((v1 - v2).normalized);
            else
                torus.transform.rotation = Quaternion.LookRotation((torus.transform.position - Camera.main.transform.position).normalized);
            //torus.transform.localScale = Vector3.one * 0.02f;
            
           // UpdateLineRenderer(torus.transform.position, transform.position);
        }
	}


    void OnEnable()
    {
        torus.SetActive(true);
        //lineRenderer.gameObject.SetActive(true); 
    }

    void OnDisable()
    {
        torus.SetActive(false);
        //lineRenderer.gameObject.SetActive(false);
    }

    private void InitLineRenderer()
    {
        //lineRenderer.color = color;
        //lineRenderer.LineThickness = 3;

        lineRenderer.material = Resources.Load("Materials/Line Material") as Material;
        lineRenderer.material.color = color;

        lineRenderer.SetColors(color, color);
        lineRenderer.SetWidth(0.005f, 0.005f);
        lineRenderer.SetVertexCount(3);
    }

    private void UpdateLineRenderer(Vector3 p1, Vector3 p2)
    {
        var diff = p2 - p1;

        p1 = p1 + diff * 0f;
        p2 = p2 - diff * 0f;

        diff = Vector3.ProjectOnPlane(p2 - p1, Camera.main.transform.right);

        //lineRenderer.Points = new Vector2[] {
        //    lineRenderer.transform.InverseTransformPoint(p1),
        //    lineRenderer.transform.InverseTransformPoint(new Vector3(p1.x + diff.x / 10f, p2.y, p1.z + diff.z / 10f)),
        //    lineRenderer.transform.InverseTransformPoint(p2)
        //};
        lineRenderer.SetPosition(0, p1);
        lineRenderer.SetPosition(1, p1 + diff);
        lineRenderer.SetPosition(2, p2);
    }
}
