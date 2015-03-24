using UnityEngine;
using System.Collections;
using Gestures;

public abstract class HandComparer
{

    private GenericHand originalHand;

    public abstract void SetOriginalHand(GenericHand hand);
    public abstract float GetSimilarity(GenericHand hand, bool rotationIndependent);


    protected delegate void FingerComparer(GenericFinger lhs, GenericFinger rhs);
    protected delegate void BoneComparer(GenericBone lhs, GenericBone rhs);


    protected void CompareFingers(GenericHand lhs, GenericHand rhs, FingerComparer comparer)
    {
        for (int f = 0; f < lhs.Fingers.Length; f++)
        {
            comparer(lhs.Fingers[f], rhs.Fingers[f]);
        }
    }

    protected void CompareBones(GenericHand lhs, GenericHand rhs, BoneComparer comparer)
    {
        for (int f = 0; f < lhs.Fingers.Length; f++)
        {
            for (int b = 0; b < lhs.Fingers[f].Bones.Length; b++)
            {
                comparer(lhs.Fingers[f].Bones[b], rhs.Fingers[f].Bones[b]);
            }
        }
    }
}