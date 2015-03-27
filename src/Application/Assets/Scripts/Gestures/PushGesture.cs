using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;
using System.Linq;


public class PushGesture : TwoHandGesture {

    public override int BacktrackLength
    {
        get { return 1; }
    }

    public override string Name
    {
        get { return "Push"; }
    }

    public override bool Stateful
    {
        get { return true; }
    }

    protected override bool Process(HandPair pair, LinkedList<HandPair> cache, GestureState state)
    {
        var camera = Camera.main.transform;
        var diff = pair.LeftHand.LocalPalmPosition - pair.RightHand.LocalPalmPosition;

        bool allFingersExtended = pair.LeftHand.Fingers.All(f => f.Extended)
                                && pair.RightHand.Fingers.All(f => f.Extended);

        bool correctDirection = Vector3.Dot(pair.LeftHand.PalmNormal, camera.forward) > 0.7f
                            && Vector3.Dot(pair.RightHand.PalmNormal, camera.forward) > 0.7f;


        bool correctHeight = Mathf.Abs(diff.y) < 0.1f && Mathf.Abs(diff.z) < 0.1f;

        return allFingersExtended && correctDirection && correctHeight;
    }
}
