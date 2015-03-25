using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;

public class StillSnapshotGesture : Gesture {

    public string gestureName;
    public float threshold = 0.7f;

    private GenericHand originalHand;
    private HandComparer comparer = new QuaternionComparer();


    protected override void Awake()
    {
        base.Awake();
        originalHand = LoadGesture(gestureName)[0];
        comparer.SetOriginalHand(originalHand);
    }


    public override int BacktrackLength
    {
        get { return 1; }
    }

    public override string Name
    {
        get { return gestureName; }
    }

    public override bool Stateful
    {
        get { return true; }
    }

    protected override bool Process(GenericHand hand, LinkedList<GenericHand> cache, GestureState state)
    {
        var similarity = comparer.GetSimilarity(hand, true);

        if (similarity > threshold)
            return true;
        else
            return false;
    }

    protected override bool Filter(GenericHand hand)
    {
        return hand.IsLeft == originalHand.IsLeft;
    }
}
