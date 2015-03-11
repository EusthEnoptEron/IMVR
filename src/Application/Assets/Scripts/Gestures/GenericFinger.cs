using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gestures
{
    public enum FingerType
    {
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Pinky = 4,
    }

    [Serializable]
    public class GenericFinger
    {
        private static int ID_COUNTER = 0;

        [NonSerialized]
        public GenericHand Hand;

        public int Id { get; private set; }
        public const int NUM_BONES = 4;
        public const int NUM_JOINTS = 5;

        public FingerType Type { get; private set; }
        public bool Extended { get; set; }
        public Vector3 TipPosition
        {
            get { return Hand.Transform.TransformPoint(LocalTipPosition); }
            set { LocalTipPosition = Hand.Transform.InverseTransformPoint(value); }
        }

        public Vector3 LocalTipPosition
        {
            get { return tipPosition.ToUnity(); }
            set { tipPosition = new SerializableVector3(value); }
        }


        public Vector3 Direction
        {
            get { return Hand.Transform.TransformVector(LocalDirection).normalized; }
        }

        public Vector3 LocalDirection
        {
            get { return (LocalTipPosition - Bones[1].LocalPosition).normalized; }
            set { direction = new SerializableVector3(value); }
        }

        private SerializableVector3 tipPosition;
        private SerializableVector3 direction;

        public GenericBone[] Bones = new GenericBone[NUM_BONES];

        public GenericFinger(FingerModel model, GenericHand hand) : this(model.GetLeapFinger(), hand, model.GetController().transform.localRotation)
        {
        }

        public GenericFinger(Finger finger, GenericHand hand) : this(finger, hand, Quaternion.identity)
        {
        }

        public GenericFinger(GenericFinger finger, GenericHand hand)
        {
            Id = finger.Id;
            Hand = hand;
            Type = finger.Type;
            Extended = finger.Extended;
            LocalTipPosition = finger.LocalTipPosition;
            LocalDirection = finger.LocalDirection;

            for (int i = 0; i < NUM_BONES; i++)
            {
                Bones[i] = new GenericBone(this);
                Bones[i].LocalPosition = finger.Bones[i].LocalPosition;
                Bones[i].LocalDirection = finger.Bones[i].LocalDirection;
                Bones[i].LocalRotation = finger.Bones[i].LocalRotation;

                Bones[i].Type = (BoneType)i;
            }
        }

        public GenericFinger(Finger finger, GenericHand hand, Quaternion orientationCorrectur)
        {
            var correcture = Matrix4x4.TRS(Vector3.zero, orientationCorrectur, new Vector3(1,1,1));

            Id = finger.Id;
            Hand = hand;
            Type = (FingerType)finger.Type();
            Extended = finger.IsExtended;
            LocalDirection = correcture.MultiplyVector(finger.Direction.ToUnity()).normalized;
            LocalTipPosition = correcture.MultiplyPoint(finger.TipPosition.ToUnityScaled());

            for (int i = 0; i < NUM_BONES; i++)
            {
                var bone = finger.Bone((Bone.BoneType)i);
                Bones[i] = new GenericBone(this);
                Bones[i].LocalPosition = correcture.MultiplyPoint(bone.Center.ToUnityScaled());
                Bones[i].LocalDirection = correcture.MultiplyVector(bone.Direction.ToUnity()).normalized;
                Bones[i].LocalRotation = orientationCorrectur * bone.Basis.Rotation();
                Bones[i].Type = (BoneType)i;
            }
        }


        public GenericFinger(FingerType type, GenericHand hand)
        {
            Id = ID_COUNTER++;
            Type = type;
            this.Hand = hand;

            for (int i = 0; i < NUM_BONES; i++)
            {
                Bones[i] = new GenericBone(this);
                Bones[i].Type = (BoneType)i;
            }
        }

        public GenericBone GetBone(BoneType type)
        {
            return Bones[(int)type];
        }

    }
}
