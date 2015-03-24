using UnityEngine;
using System.Collections;
using Gestures;
using Assets.Scripts;
using System.Collections.Generic;

public class QuaternionComparer : HandComparer
{

    private GenericHand originalHand;
    private HandSimilarityVector2 similarityVector;

    public override float GetSimilarity(Gestures.GenericHand hand, bool rotationIndependent = true)
    {
        int i = 0;
        float sum = 0;

        CompareBones(originalHand, hand, (GenericBone b1, GenericBone b2) =>
        {
            var q1 = rotationIndependent ? b1.NormalizedRotation : b1.LocalRotation;
            var q2 = rotationIndependent ? b2.NormalizedRotation : b2.LocalRotation;
            sum += Mathf.Pow(Quaternion.Dot(q1, q2), 10);

            i++;
        });

        float similarity = sum / i;

        return Mathf.Pow(similarity, 10);
    }

    public override void SetOriginalHand(Gestures.GenericHand hand)
    {
        this.originalHand = hand;
    }
}

