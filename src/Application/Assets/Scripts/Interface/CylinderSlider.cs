using UnityEngine;
using System.Collections;
using System.Linq;

public class CylinderSlider : MonoBehaviour {
    public Transform fill;

    private float _minValue = 0;
    private float _maxValue = 1;

    public UnityEngine.UI.Slider.SliderEvent onMinValueChanged = new UnityEngine.UI.Slider.SliderEvent();
    public UnityEngine.UI.Slider.SliderEvent onMaxValueChanged = new UnityEngine.UI.Slider.SliderEvent();
    public UnityEngine.UI.Slider.SliderEvent onValueChanged = new UnityEngine.UI.Slider.SliderEvent();

    public Vector3 direction = Vector3.up;

    public float minPos = -0.99f;
    public float maxPos = 0.99f;
    public float maxScale = 0.99f;

    private Vector3 _scaledAxis;
    private Vector3 _minPoint;

    public float MaxValue
    {
        get { return _maxValue; }
        set
        {
            _maxValue = Mathf.Clamp(value, _minValue, 1);

            onMaxValueChanged.Invoke(_maxValue);
        }
    }
    public float MinValue
    {
        get { return _minValue; }
        set
        {
            _minValue = Mathf.Clamp(value, 0, _maxValue);

            onMinValueChanged.Invoke(_minValue);
        }
    }

    private float _length;

	// Use this for initialization
	void Start () {
        _length = maxPos - minPos;

        _scaledAxis = direction * _length;
        _minPoint = minPos * direction;

        // Route detailed events to value changed event
        onMinValueChanged.AddListener((v) => onValueChanged.Invoke(v));
        onMaxValueChanged.AddListener((v) => onValueChanged.Invoke(v));

        onValueChanged.AddListener(OnValueChanged);

        OnValueChanged(0);
	}


    private void OnValueChanged(float arg0)
    {
        //if (MaxValue == 1 && MinValue == 0)
        //{
        //    fill.gameObject.SetActive(false);
        //}
        //else
        //{
        //    fill.gameObject.SetActive(true);

            fill.localPosition = Vector3.Max(_minPoint, _minPoint + _scaledAxis * MinValue);
            fill.localScale = new Vector3(
                0.99f,
                0f,
                0.99f
            ) + direction * (MaxValue - MinValue);

        //}
    }

	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerStay(Collider other)
    {
        var hand = other.GetComponentInParent<HandModel>();
        if (hand)
        {
            var palmNormal = transform.InverseTransformDirection(hand.GetPalmNormal());
            var dotProduct = Vector3.Dot(palmNormal, direction);
            bool eligible =
                (hand.GetLeapHand().IsLeft && dotProduct > 0)
            || (!hand.GetLeapHand().IsLeft && dotProduct < 0);

            eligible = eligible && hand.fingers.Count(
                f => f.GetLeapFinger().IsExtended
            ) >= 3;

            // Only let through when the hand is correctly oriented
            if (eligible)
            {
                var pos = transform.InverseTransformPoint(hand.GetPalmPosition());
                var posVector = (pos - _minPoint) / _length;

                float value = Mathf.Clamp01(Vector3.Dot(direction, posVector));

                if (hand.GetLeapHand().IsLeft)
                {
                    // change lower bound
                    MinValue = value < 0.01f ? 0 : value;
                }
                else
                {
                    // change upper bound
                    MaxValue = value > 0.99f ? 1 : value;
                }
            }
        }
    }
}
