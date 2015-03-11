using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Gestures
{
    public class LeapAnimationVector : SimilarityVector
    {
        public delegate float Mapper(GenericHand hand, GenericHand prevHand);
        public delegate float Classifier(float v1, float v2);
        public readonly int HandCount;
        public const float M_TO_MM = 1000;

        private Mapper[] mappers = new Mapper[]{
            (h1, h2) => h1.PalmDirection.x,
            (h1, h2) => h1.PalmDirection.y,
            (h1, h2) => h1.PalmDirection.z,

            (h1, h2) => (h2.PalmPosition.x - h1.PalmPosition.x) * M_TO_MM,
            (h1, h2) => (h2.PalmPosition.y - h1.PalmPosition.y) * M_TO_MM,
            (h1, h2) => (h2.PalmPosition.z - h1.PalmPosition.z) * M_TO_MM,

            (h1, h2) => h1.PalmNormal.x,
            (h1, h2) => h1.PalmNormal.y,
            (h1, h2) => h1.PalmNormal.z,

            (h1, h2) => (h2.Fingers[0].TipPosition.x - h1.Fingers[0].TipPosition.x) * M_TO_MM,
            (h1, h2) => (h2.Fingers[0].TipPosition.y - h1.Fingers[0].TipPosition.y) * M_TO_MM,
            (h1, h2) => (h2.Fingers[0].TipPosition.z - h1.Fingers[0].TipPosition.z) * M_TO_MM,

        
            (h1, h2) => (h2.Fingers[1].TipPosition.x - h1.Fingers[1].TipPosition.x) * M_TO_MM,
            (h1, h2) => (h2.Fingers[1].TipPosition.y - h1.Fingers[1].TipPosition.y) * M_TO_MM,
            (h1, h2) => (h2.Fingers[1].TipPosition.z - h1.Fingers[1].TipPosition.z) * M_TO_MM,

            (h1, h2) => (h2.Fingers[2].TipPosition.x - h1.Fingers[2].TipPosition.x) * M_TO_MM,
            (h1, h2) => (h2.Fingers[2].TipPosition.y - h1.Fingers[2].TipPosition.y) * M_TO_MM,
            (h1, h2) => (h2.Fingers[2].TipPosition.z - h1.Fingers[2].TipPosition.z) * M_TO_MM,

            (h1, h2) => (h2.Fingers[3].TipPosition.x - h1.Fingers[3].TipPosition.x) * M_TO_MM,
            (h1, h2) => (h2.Fingers[3].TipPosition.y - h1.Fingers[3].TipPosition.y) * M_TO_MM,
            (h1, h2) => (h2.Fingers[3].TipPosition.z - h1.Fingers[3].TipPosition.z) * M_TO_MM,

            (h1, h2) => (h2.Fingers[4].TipPosition.x - h1.Fingers[4].TipPosition.x) * M_TO_MM,
            (h1, h2) => (h2.Fingers[4].TipPosition.y - h1.Fingers[4].TipPosition.y) * M_TO_MM,
            (h1, h2) => (h2.Fingers[4].TipPosition.z - h1.Fingers[4].TipPosition.z) * M_TO_MM
        };




        public LeapAnimationVector(IEnumerable<GenericHand> hands)
                : base()
        {
            HandCount = hands.Count();

            values = new float[mappers.Length * (hands.Count() - 1)];
            int i = 0;

            GenericHand prevHand = null;
            foreach (var hand in hands)
            {
                if (prevHand != null)
                {
                    foreach (var classify in mappers)
                    {
                        values[i++] = classify(hand, prevHand);
                    }
                }
                prevHand = hand;
            }
        }
    }



    [Serializable]
    public struct SerializableVector3
    {
        float x, y, z;

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToUnity()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public struct SerializableQuaternion
    {
        float x, y, z, w;

        public SerializableQuaternion(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quaternion ToUnity()
        {
            return new Quaternion(x, y, z, w);
        }

    }

}
