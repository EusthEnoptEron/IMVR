using UnityEngine;
using System.Collections;
using Gestures;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class RingMenuItem : UIBehaviour, IPointerClickHandler {
    public FingerType fingerType = FingerType.Index;
    public float pixelsRatio = 1000;
    public Color color = Color.blue;
    public float heightDifference = 0;

    private GameObject torus;

    [System.Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    public float Progress { get; set; }

	// Use this for initialization
	protected virtual void Awake () {
        torus = GameObject.Instantiate<GameObject>(Resources.Load("Prefabs/Torus") as GameObject);
        torus.transform.SetParent(transform);

        //torus.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);
        torus.GetComponentInChildren<Image>().color = color;

        transform.localScale = Vector3.zero;
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
        //torus.SetActive(true);
        //lineRenderer.gameObject.SetActive(true); 
    }

    void OnDisable()
    {
        //torus.SetActive(false);
        //lineRenderer.gameObject.SetActive(false);
    }


    public virtual void SetVisibility(bool visible)    {
        var targetSize = visible ? Vector3.one : Vector3.zero;
        transform.DOKill();
        var transition = transform.DOScale(targetSize, 0.5f);

        if (!visible) transition.OnComplete(delegate
            {
                // TODO: Enable when this strange bug is fixed...
                //gameObject.SetActive(false);
            });
        else gameObject.SetActive(true);
    }

    private void Press()
    {
        if (!IsActive())
            return;

        m_OnClick.Invoke();
    }

    // Trigger all registered callbacks.
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }
}
