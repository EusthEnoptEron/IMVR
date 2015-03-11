using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Gestures
{
    class HandPosition
    {
        public static HandPosition DuckHandLeft = new HandPosition("DuckHandLeft");
        public static HandPosition DuckHandRight = new HandPosition("DuckHandRight");
        public static HandPosition PinchRight = new HandPosition("PinchRight");


        private string filename;
        private GenericHand hand = null;

        public HandPosition(string name)
        {
            filename = name;
        }

        public GenericHand GetHand()
        {
            if (hand == null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                string path = Application.dataPath + "/Snapshots/" + filename + ".bin";
                GenericHand[] hands;

                using (var stream = File.OpenRead(path))
                {
                    hands = bf.Deserialize(stream) as GenericHand[];
                }
                hand = hands.First();
            }

            return hand;
        }

        public bool IsActive(GenericHand hand, float threshold = 0.8f)
        {
            var v1 = new HandSimilarityVector2(hand);
            var v2 = new HandSimilarityVector2(GetHand());

            //Debug.Log((hand.GetFinger(FingerType.Thumb).Bones[1].LocalRotation * Quaternion.Inverse(hand.LocalPalmRotation)).eulerAngles);
            //OculusDebug.Log(HandSimilarityVector2.Similarity(v1, v2));
            return HandSimilarityVector2.Similarity(v1, v2) > threshold;
        }

        public static bool IsActive(HandPosition position, GenericHand hand, float threshold = 0.8f)
        {
            return position.IsActive(hand, threshold);
        }
    }
}
