using UnityEngine;
using System.Collections;
using Gestures;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class RingMenuItem : MonoBehaviour, IPointerClickHandler {
    public Canvas ui;
    public Text text;
    public Image circle;

    public FingerType fingerType = FingerType.Index;
    public float width = 0.03f;
    public Color color = Color.blue;
    public float heightDifference = 0;

    [System.Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    public ButtonClickedEvent OnClick = new ButtonClickedEvent();

    public float Progress { get; set; }

    private float fullScale;

	// Use this for initialization
	protected virtual void Awake () {
        //torus.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);
        circle.color = color;

        fullScale = width / ui.GetComponent<RectTransform>().rect.width;

        ui.transform.localScale = Vector3.zero;
      //  InitLineRenderer();
	}

    //protected virtual void Start() {
    //    foreach (var img in GetComponentsInChildren<Image>())
    //    {
    //        img.material = Resources.Load<Material>("Materials/UI-Material");
    //    }
    //    foreach (var text in GetComponentsInChildren<Text>())
    //    {
    //        text.material = Resources.Load<Material>("Materials/UI-Text");
    //    }

    //}

	// Update is called once per frame
	protected virtual void Update () {
        var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
        if (hand != null)
        {
            var finger = hand.GetFinger(fingerType);
            var bone   = finger.GetBone(BoneType.Intermediate);

            // Place myself
            Vector3 distance = new Vector3(0.1f, heightDifference, 0f);

            ui.transform.position = finger.GetBone(BoneType.Distal).Position;//hand.PalmPosition + Vector3.ProjectOnPlane(finger.GetBone(BoneType.Intermediate).Position - hand.PalmPosition, Camera.main.transform.forward) * 2f;// + Camera.main.transform.TransformDirection(distance);
            ui.transform.rotation = Quaternion.LookRotation((ui.transform.position - Camera.main.transform.position).normalized);

            // Place torus
            circle.transform.localRotation *= Quaternion.Euler(0, 0, (180 + Progress * 1000) * Time.deltaTime);

            BoneType startBone = Progress > 0.5f ? BoneType.Intermediate : BoneType.Distal;
            BoneType endBone   = startBone - 1;
            var v1 = finger.GetBone(startBone).Position;
            var v2 = finger.GetBone(endBone).Position;

            circle.transform.position = Vector3.Lerp(v1, v2, (Progress * 2) % 1);

            //if (Progress > 0)
            //    circle.transform.rotation = Quaternion.LookRotation((v1 - v2).normalized);
            //else
            //    circle.transform.rotation = Quaternion.LookRotation((circle.transform.position - Camera.main.transform.position).normalized);
            //torus.transform.localScale = Vector3.one * 0.02f;
            
           // UpdateLineRenderer(torus.transform.position, transform.position);
        }
	}


    void OnEnable()
    {
        //torus.SetActive(true);
        //lineRenderer.gameObject.SetActive(true); 
    }

    void OnDisable()
    {
        //torus.SetActive(false);
        //lineRenderer.gameObject.SetActive(false);
    }


    public virtual void SetVisibility(bool visible)    {
        var targetSize = visible ? Vector3.one * fullScale : Vector3.zero;
        ui.transform.DOKill();
        var transition = ui.transform.DOScale(targetSize, 0.5f);

        if (!visible) transition.OnComplete(delegate
            {
                // TODO: Enable when this strange bug is fixed...
                ui.gameObject.SetActive(false);
                enabled = false;
            });
        else
        {
            ui.gameObject.SetActive(true);
            enabled = true;
        }

    }

    private void Press()
    {
        if (!gameObject.activeInHierarchy || !enabled)
            return;

        OnClick.Invoke();
    }

    // Trigger all registered callbacks.
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }
}
