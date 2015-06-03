using UnityEngine;
using System.Collections;
using Gestures;
using DG.Tweening;
using System.Linq;
using Leap;

[RequireComponent(typeof(CanvasGroup))]
public class ArmWear : MonoBehaviour {
    public enum ArmWearPosition
    {
        Front,
        Back
    }

    public ArmWearPosition position = ArmWearPosition.Front;

    private bool _visible = true;
    private CanvasGroup _group;

    public Vector3 baseRotation = Vector3.zero;
    public Vector3 baseOffset = Vector3.zero;
	// Use this for initialization
	protected virtual void Start () {
        _group = GetComponent<CanvasGroup>();

        _group.alpha = 0;

        SetState(false);
	}
	
	// Update is called once per frame
	protected virtual void Update () {
        float lerpProgress = Time.deltaTime * 10;
        var hand = HandProvider.Instance.GetHand(HandType.Left);

        if (hand != null)
        {
            if (!IsVisible)
            {
                // Place immediately
                lerpProgress = 1;
            }

            var provider = HandProvider.Instance as LeapHandProvider;
            var leapHand = provider.GetFrame().Hands.FirstOrDefault(h => h.Id == hand.Id);

            if (leapHand != null)
            {
                var arm = leapHand.Arm;
                var armRotation = provider.transform.rotation * arm.Basis.Rotation() * Reorientation();

                // armNormal is the direction of the bottom of the arm
                var armNormal      = armRotation * Vector3.forward;
                var camDirection   = Camera.main.transform.forward;

                // [-1, 1]
                // x < 0 => Back side
                // x > 0 => Front side
                float dotProduct = Vector3.Dot(armNormal, camDirection);

                SetState(
                    (dotProduct < 0 && position == ArmWearPosition.Back)
                 || (dotProduct > 0 && position == ArmWearPosition.Front)
                );

                //if (_group.alpha > 0)
                {
                    if (position == ArmWearPosition.Back)
                    {
                        armRotation *= Quaternion.Euler(180, 0, 0) * Quaternion.Euler(baseRotation);
                    }

                    transform.position = Vector3.Lerp(transform.position,
                                            provider.transform.TransformPoint(arm.Center.ToUnityScaled())
                                            + armRotation * baseOffset,
                                            lerpProgress);
                    transform.rotation = Quaternion.Slerp(
                                            transform.rotation,
                                            armRotation,
                                            lerpProgress);
                }
            }
        }
        else 
        {
            SetState(false);
        }
	}

    private Quaternion Reorientation()
    {
        return Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, Vector3.back));
    }

    void SetState(bool newState)
    {
        if (_visible != newState)
        {
            _visible = newState;

            _group.Fade(newState ? 1 : 0, 0.1f).OnComplete(
                () => _group.interactable = _visible    
            );
        }
    }


    /// <summary>
    /// Gets whether or not the wear is visible.
    /// </summary>
    public bool IsVisible
    {
        get
        {
            return _visible;
        }
    }

}
