using UnityEngine;
using System.Collections;
using System.Linq;

public class CylinderSlider : MonoBehaviour {
    public Transform fill;

    private float _minValue = 0;
    private float _maxValue = 1;

    private bool _leftHandUpdated = false;
    private bool _rightHandUpdated = false;

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
	void FixedUpdate () {
        _leftHandUpdated = false;
        _rightHandUpdated = false;
	}

    void OnTriggerStay(Collider other)
    {
        var finger = other.GetComponentInParent<FingerModel>();
        var hand = other.GetComponentInParent<HandModel>();
        if (hand && finger && finger.fingerType == Leap.Finger.FingerType.TYPE_MIDDLE)
        {

            bool isLeft = hand.GetLeapHand().IsLeft;

            // break condition to save CPU time
            if ((isLeft && _leftHandUpdated) || (!isLeft && _rightHandUpdated))
                return;


            var palmNormal = transform.InverseTransformDirection(hand.GetPalmNormal());
            var dotProduct = Vector3.Dot(palmNormal, direction);
            bool eligible =
                (isLeft && dotProduct > 0)
            || (!isLeft && dotProduct < 0);

            eligible = eligible && hand.fingers.Count(
                f => f.GetLeapFinger().IsExtended
            ) >= 3;

            // Only let through when the hand is correctly oriented
            if (eligible)
            {
                _leftHandUpdated = _leftHandUpdated || isLeft;
                _rightHandUpdated = _rightHandUpdated || !isLeft;

                var pos = transform.InverseTransformPoint(hand.GetPalmPosition());
                var posVector = (pos - _minPoint) / _length;

                float value = Mathf.Clamp01(Vector3.Dot(direction, posVector));
                

                if (isLeft)
                {
                    float lerpedValue = Mathf.Lerp(
                       MinValue,
                       value,
                       Time.deltaTime * 5);

                    // change lower bound
                    MinValue = value < 0.01f ? 0 : lerpedValue;

                }
                else
                {
                    float lerpedValue = Mathf.Lerp(
                       MaxValue,
                       value,
                       Time.deltaTime * 5);

                    // change upper bound
                    MaxValue = value > 0.99f ? 1 : lerpedValue;
                }
            }
        }
    }
}
