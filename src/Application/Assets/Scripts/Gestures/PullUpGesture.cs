using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;
using System.Linq;


public class PullUpGesture : TwoHandGesture
{
    private bool finished = false;
    private bool active = false;
    private Vector3 startPosition;

    public float requiredDistance = 0.05f;

    public override int BacktrackLength
    {
        get { return 1; }
    }

    public override string Name
    {
        get { return "Pull Up"; }
    }

    public override bool Stateful
    {
        get { return false; }
    }

    protected override bool Process(HandPair pair, LinkedList<HandPair> cache, GestureState state)
    {
        if (HandsAreValid(pair))
        {
            if (finished) return false;

            if (!active)
            {
                // Initialize
                active = true;
                startPosition = pair.LeftHand.PalmPosition;
            }

            var pullEndPosition = pair.LeftHand.PalmPosition;
            var axis = -Camera.main.transform.up;
            var distance = Vector3.Dot(pullEndPosition - startPosition, axis);

            if (distance > requiredDistance)
            {
                finished = true;

                return true;
            }
        }
        else
        {
            active = false;
            finished = false;
        }

        return false;
    }

    private bool HandsAreValid(HandPair pair)
    {
        var camera = Camera.main.transform;
        var diff = pair.LeftHand.LocalPalmPosition - pair.RightHand.LocalPalmPosition;

        bool allFingersExtended = pair.LeftHand.Fingers.All(f => f.Extended)
                                && pair.RightHand.Fingers.All(f => f.Extended);

        bool correctDirection = Vector3.Dot(pair.LeftHand.PalmNormal, camera.up) > 0.7f
                            && Vector3.Dot(pair.RightHand.PalmNormal, camera.up) > 0.7f;


        bool correctHeight = Mathf.Abs(diff.y) < 0.1f && Mathf.Abs(diff.z) < 0.1f;

        return allFingersExtended && correctDirection && correctHeight;
    }
}
