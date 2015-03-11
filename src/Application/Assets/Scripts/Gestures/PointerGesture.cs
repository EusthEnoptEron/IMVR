using UnityEngine;
using System.Collections;
using Leap;

//public class PointerGesture : HandPosition
//{
//    private GUIText text;

//    private HandController controller;
//    private Leap.Hand hand;
//    private float minAngle = 70;

//    private Hand leap_hand = null;

//    public PointerGesture()
//    {
//        Constraints.AddRange(new Constraint[] {
//            PointerConstraint
//        });
//    }

//    protected override void OnReset()
//    {
//        GameObject.Destroy(text);
//    }

//    private float PointerConstraint(Hand hand)
//    {
//        if (State.Active && hand.Confidence < 0.5f)
//        {
//            return 1;
//        }

//        var n = hand.PalmNormal.ToUnity().normalized;
//        float maxH = 0;

//        foreach (var f in hand.Fingers)
//        {
//            var v = f.Direction.ToUnity().normalized;
//            //var angle = Mathf.Acos( Vector3.Dot(n, v) );
//            float angle = Vector3.Angle(n, v);
//            float h = Mathf.Abs(angle - 90f);
//            maxH = Mathf.Max(h, maxH);
//        }

//        if (maxH > (90 - minAngle)) return 0;
//        else return 1;
//    }

//    protected override void OnActivateHand(Hand hand)
//    {
//        leap_hand = hand;
//    }

//    protected float GetFlatnessScore(Leap.Hand hand)
//    {
//        return 1;
//    }

//    public override void EnforcePosition(HandModel hand_model)
//    {
//        Debug.Log("ENFORCE");

//        hand_model.Copy(leap_hand);
//    }
//}
