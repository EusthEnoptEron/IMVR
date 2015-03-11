using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gestures
{
    public enum BoneType
    {
        Metacarpal = 0,
        Proximal = 1,
        Intermediate = 2,
        Distal = 3
    }

    [Serializable]
    public class GenericBone
    {
        public BoneType Type;

        public Vector3 Position
        {
            get { return Finger.Hand.Transform.TransformPoint(LocalPosition); }
            set { LocalPosition = Finger.Hand.Transform.InverseTransformPoint(value); }
        }

        public Vector3 Direction
        {
            get { return Finger.Hand.Transform.TransformVector(LocalDirection).normalized; }
            set { LocalDirection = Finger.Hand.Transform.InverseTransformVector(LocalDirection); }
        }

        public Quaternion Rotation
        {
            get { return Finger.Hand.Transform.Transform(LocalRotation); }
        }

        public Vector3 LocalPosition
        {
            get { return position.ToUnity(); }
            set { position = new SerializableVector3(value); }
        }
        public Vector3 LocalDirection
        {
            get { return direction.ToUnity(); }
            set { direction = new SerializableVector3(value); }
        }
        public Quaternion LocalRotation
        {
            get { return rotation.ToUnity(); }
            set { rotation = new SerializableQuaternion(value); }
        }


        public Quaternion NormalizedRotation
        {
            get
            {
                return Quaternion.Inverse(Finger.Hand.LocalPalmRotation) * LocalRotation;
            }
        }
        public GenericFinger Finger;

        public GenericBone(GenericFinger finger)
        {
            Finger = finger;
        }

        private SerializableVector3 position;
        private SerializableVector3 direction;
        private SerializableQuaternion rotation;
    }

}
