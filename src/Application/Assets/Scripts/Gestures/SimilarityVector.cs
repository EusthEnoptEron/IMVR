using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gestures
{
    public abstract class SimilarityVector
    {
        protected float[] values;

        public float Magnitude
        {
            get
            {
                return Mathf.Sqrt(MagnitudeSquared);
            }
        }

        public float MagnitudeSquared
        {
            get
            {
                float sum = 0;
                foreach (float v in values)
                {
                    sum += v * v;
                }
                return sum;
            }
        }


        public static float Dot(SimilarityVector v1, SimilarityVector v2)
        {
            float sum = 0;

            for (int i = 0; i < v1.values.Length; i++)
            {
                sum += v1[i] * v2[i];
            }

            return sum;
        }

        public static float Similarity(SimilarityVector v1, SimilarityVector v2)
        {
            if (v1.Length != v2.Length) return 0;

            return SimilarityVector.Dot(v1, v2) / (v1.Magnitude * v2.Magnitude);
        }

        public float this[int index]
        {
            get
            {
                return values[index];
            }
        }

        public int Length
        {
            get
            {
                return values.Length;
            }
        }
    }



    public class HandSimilarityVector : SimilarityVector
    {
        public HandSimilarityVector(GenericHand hand)
        {
            var vals = new List<float>();

            foreach (var finger in hand.Fingers)
            {
                float weight = 1;

                foreach (var bone in finger.Bones)
                {
                    var q = bone.LocalRotation * Quaternion.Inverse(hand.LocalPalmRotation);
                    vals.Add(q.x * weight);
                    vals.Add(q.y * weight);
                    vals.Add(q.z * weight);
                    vals.Add(q.w * weight);

                    weight++;
                }
            }

            values = vals.ToArray();
        }
    }

    public class HandSimilarityVector2 : ClassifierVector<Quaternion>
    {
        public HandSimilarityVector2(GenericHand hand)
        {
            var vals = new List<Quaternion>();

            foreach (var finger in hand.Fingers)
            {
                float weight = 1;

                foreach (var bone in finger.Bones)
                {
                    vals.Add(bone.NormalizedRotation);
                }
            }

            values = vals.ToArray();
        }

        public static float Classify(Quaternion q1, Quaternion q2)
        {

            // Angle approach
            // [-1, 1], max is 180
            //return Mathf.Pow( 1 - (Quaternion.Angle(q1, q2) / 90f), 3);


            // Dotproduct appraoch
            return Mathf.Pow(Quaternion.Dot(q1, q2), 5);
           /* if (Mathf.Abs(Quaternion.Dot(q1, q2)) > 0.8)
                return 1;
            else return -1;*/

        }

        public static float Similarity(HandSimilarityVector2 v1, HandSimilarityVector2 v2)
        {
            return Similarity(v1, v2, Classify);
        }
    }


    public class ClassifierVector<T>
    {
        public delegate float Classifier(T lhs, T rhs);

        protected T[] values;

        public ClassifierVector()
        {
            values = new T[0];
        }
        public ClassifierVector(IEnumerable<T> values)
        {
            this.values = values.ToArray();
        }

        public static float Similarity(ClassifierVector<T> v1, ClassifierVector<T> v2, Classifier comparer)
        {
            float sum = 0;
            if (v1.values.Length == v2.values.Length)
            {
                int counter = 0;
                for (int i = 0; i < v1.values.Length; i++)
                {
                    sum += Mathf.Clamp(comparer(v1.values[i], v2.values[i]), -1, 1);
                    counter++;
                }

                return sum / counter;
            }
            else
            {
                return -1;
            }
        }

    }

}
