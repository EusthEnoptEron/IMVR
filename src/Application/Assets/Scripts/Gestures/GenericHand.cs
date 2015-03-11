using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gestures
{
    [Serializable]
    public class TransformContext {
        public Vector3 Translation { get { return _translation.ToUnity(); } set { _translation = new SerializableVector3(value); Invalidate();  } }
        public Vector3 Scale  { get { return _scale.ToUnity(); } }
        public Quaternion Rotation  { get { return _rotation.ToUnity(); }
            set
            {
                _rotation = new SerializableQuaternion(value);
                Invalidate();
            }
        }

        private SerializableVector3 _translation;
        private SerializableVector3 _scale;
        private SerializableQuaternion _rotation;

        [NonSerialized]
        private Matrix4x4 _matrix = Matrix4x4.zero;

        public TransformContext() : this(Vector3.zero, new Vector3(1, 1, 1), Quaternion.identity)
        {
            
        }

        public TransformContext(TransformContext context) 
            : this(context.Translation, context.Scale, context.Rotation) 
        { }

        public TransformContext(Vector3 translation, Vector3 scale, Quaternion rotation) {
            _translation = new SerializableVector3(translation);
            _scale = new SerializableVector3(scale);
            _rotation = new SerializableQuaternion(rotation);
        }

        public Matrix4x4 Matrix {
            get {
                if (_matrix == Matrix4x4.zero)
                    _matrix = Matrix4x4.TRS(Translation, Rotation, Scale);

                return _matrix;
            }
        }

        public Vector3 TransformPoint(Vector3 point) {
            return Matrix.MultiplyPoint(point);
        }

        public Vector3 InverseTransformPoint(Vector3 point) {
            return Matrix.inverse.MultiplyPoint(point);
        }

        public Vector3 TransformVector(Vector3 vector) {
            return Matrix.MultiplyVector(vector);
        }
        public Vector3 InverseTransformVector(Vector3 vector) {
            return Matrix.inverse.MultiplyVector(vector);
        }

        public Quaternion Transform(Quaternion rotation) {
            return Rotation * rotation;
        }

        public void RotateAround(Vector3 localPoint, Quaternion quaternion)
        {
            var T = Matrix4x4.TRS(-localPoint, Quaternion.identity, new Vector3(1, 1, 1));
            var R = Matrix4x4.TRS(Vector3.zero, quaternion, new Vector3(1, 1, 1));
            var TBack = Matrix4x4.TRS(localPoint, Quaternion.identity, new Vector3(1, 1, 1));

            var M = this.Matrix * TBack * R * T;

            Rotation = Quaternion.LookRotation(
                 M.GetColumn(2), // Z = Forward
                 M.GetColumn(1)  // Y = Up
             );

            Translation = M.GetColumn(3);

            OculusDebug.Log(this.Matrix);
        }

        private void Invalidate()
        {
            _matrix = Matrix4x4.zero;
        }

    }

    [Serializable]
    public class GenericHand
    {
        private static int ID_COUNTER = 0;
        
        //public TransformContext Transform { 
        //    get { return _transform; }
        //    private set
        //    {
        //        _transform = value;
        //    }
        //}
        public TransformContext Transform { get; set; }
        //private TransformContext _transform = new TransformContext();

        public enum NormalizationGrade {
            None = 0,
            Position = 1,
            Rotation = 2
        }

        public int Id { get; private set; }


        public GenericHand Normalized
        {
            get { return this.Normalize(NormalizationGrade.Position); }
        }



        public Vector3 PalmDirection
        {
            get { return Transform.TransformVector(LocalPalmDirection).normalized; }
        }
        public Vector3 PalmNormal
        {
            get { return Transform.TransformVector(LocalPalmNormal).normalized; }
        }
        public Vector3 PalmPosition
        {
            get { return Transform.TransformPoint(LocalPalmPosition); }
            //set { Transform.Translation += (value - PalmPosition); }
        }
        public Vector3 WristPosition
        {
            get { return Transform.TransformPoint(LocalWristPosition); }
        }
        public Quaternion PalmRotation
        {
            get { return Transform.Transform(LocalPalmRotation); }
        }

        public Vector3 LocalFingerDirection
        {
            get
            {
                var dir = Vector3.zero;
                foreach (var finger in Fingers)
                {
                    if (finger.Type != FingerType.Thumb)
                    {
                        dir += finger.LocalDirection;
                    }
                }
                return dir / 4;
            }
        }


        public Vector3 LocalPalmDirection
        {
            get { return palmDirection.ToUnity(); }
            set { palmDirection = new SerializableVector3(value); }
        }
        public Vector3 LocalPalmNormal
        {
            get { return palmNormal.ToUnity(); }
            set { palmNormal = new SerializableVector3(value); }
        }
        public Vector3 LocalPalmPosition
        {
            get { return palmPosition.ToUnity(); }
            set { palmPosition = new SerializableVector3(value); }
        }
        public Vector3 LocalWristPosition
        {
            get { return wristPosition.ToUnity(); }
            set { wristPosition = new SerializableVector3(value); }
        }
        public Quaternion LocalPalmRotation
        {
            get { return palmRotation.ToUnity(); }
            set { palmRotation = new SerializableQuaternion(value); }
        }

        public float Confidence;

        private SerializableVector3 palmDirection;
        private SerializableVector3 palmNormal;
        private SerializableVector3 palmPosition;
        private SerializableVector3 wristPosition;
        private SerializableQuaternion palmRotation;

        public NormalizationGrade Normalization { get; private set; }

        public GenericFinger[] Fingers;

        public bool IsLeft;

        public bool IsRight { get { return !IsLeft; } }

        public GenericHand()
        {
            Id = ID_COUNTER++;
            Fingers = new GenericFinger[Enum.GetValues(typeof(FingerType)).Length];

            for (int i = 0; i < Fingers.Length; i++)
                Fingers[i] = new GenericFinger((FingerType)i, this);

            Transform = new TransformContext();
        }

        public GenericHand(HandModel model) : this(model.GetLeapHand(), model.GetController().transform)
        {
            //Id = model.GetInstanceID();
            //PalmDirection = model.GetPalmDirection();
            //PalmNormal = model.GetPalmNormal();
            //PalmPosition = model.GetPalmPosition();
            //PalmRotation = model.GetPalmRotation();
            //WristPosition = model.GetWristPosition();
            //IsLeft = model.GetLeapHand().IsLeft;
            

            //Fingers = new GenericFinger[model.fingers.Length];

            //for (int i = 0; i < Fingers.Length; i++)
            //    Fingers[i] = new GenericFinger(model.fingers[i]);


            //Init();
        }

        public GenericHand(Hand hand, Vector3 translation, Vector3 scale, Quaternion orientation, Quaternion orientationCorrecture)
        {
            Id = hand.Id;

            Transform = new TransformContext(translation, scale, orientation);

            var correcture = Matrix4x4.TRS(Vector3.zero, orientationCorrecture, new Vector3(1, 1, 1));

            LocalPalmDirection = correcture.MultiplyVector(hand.Direction.ToUnity()).normalized;
            LocalPalmNormal = correcture.MultiplyVector(hand.PalmNormal.ToUnity()).normalized;
            LocalPalmPosition = correcture.MultiplyPoint(hand.PalmPosition.ToUnityScaled());
            LocalPalmRotation = orientationCorrecture * hand.Basis.Rotation();
            LocalWristPosition = correcture.MultiplyPoint(hand.WristPosition.ToUnityScaled());
            IsLeft = hand.IsLeft;
            Confidence = hand.Confidence;

            Fingers = new GenericFinger[hand.Fingers.Count];

            for (int i = 0; i < Fingers.Length; i++)
                Fingers[i] = new GenericFinger(hand.Fingers[i], this, orientationCorrecture);

        }


        public GenericHand(Hand hand, Vector3 position, Vector3 scale, Quaternion orientation) : this(hand, position, scale, orientation, Quaternion.identity)
        {}

        public GenericHand(Hand hand, Transform transform) : this(hand, transform.position, transform.localScale, transform.rotation)
        {
        }

        public GenericHand(GenericHand hand)
        {
            Id = hand.Id;
            Transform = new TransformContext(hand.Transform);

            LocalPalmDirection = hand.LocalPalmDirection;
            LocalPalmNormal = hand.LocalPalmNormal;
            LocalPalmPosition = hand.LocalPalmPosition;
            LocalPalmRotation = hand.LocalPalmRotation;
            LocalWristPosition = hand.LocalWristPosition;
            IsLeft = hand.IsLeft;
            Confidence = hand.Confidence;

            Fingers = new GenericFinger[hand.Fingers.Length];

            for (int i = 0; i < Fingers.Length; i++)
                Fingers[i] = new GenericFinger(hand.Fingers[i], this);
        }

        public GenericHand(Hand model, Transform transform, Quaternion correcture)
            : this(model, transform.position, transform.localScale, transform.rotation, correcture)
        {
        }

        public GenericFinger GetFinger(FingerType type)
        {
            return Fingers[(int)type];
        }

        public GenericHand Normalize(NormalizationGrade grade)
        {
            var hand = new GenericHand(this);
            if ((grade & NormalizationGrade.Position) == NormalizationGrade.Position &&
                (Normalization & NormalizationGrade.Position) != NormalizationGrade.Position)
            {
                // Move wrist to the center.
                var T = -LocalWristPosition;
                hand.LocalPalmPosition += T;
                hand.LocalWristPosition += T;
                foreach (var finger in hand.Fingers)
                {
                    finger.LocalTipPosition += T;
                    foreach (var bone in finger.Bones)
                    {
                        bone.LocalPosition += T;
                    }
                }
            }

            if ((grade & NormalizationGrade.Rotation) == NormalizationGrade.Rotation &&
               (Normalization & NormalizationGrade.Rotation) != NormalizationGrade.Rotation)
            {
                var R = Quaternion.Inverse(LocalPalmRotation);
                hand.LocalPalmRotation *= R;
                hand.LocalPalmPosition = R * hand.LocalPalmPosition;
                //hand.WristPosition += T;

                foreach (var finger in hand.Fingers)
                {
                    finger.LocalTipPosition = R * finger.LocalTipPosition;
                    foreach (var bone in finger.Bones)
                    {
                        bone.LocalRotation *= R;
                        bone.LocalPosition = R * bone.LocalPosition;
                    }
                }
            }

            return hand;
        }

        public GenericHand Move(Vector3 offset)
        {
            var hand = new GenericHand(this);
            hand.move(offset);

            return hand;
        }

        private void move(Vector3 offset)
        {
            this.LocalPalmPosition += offset;
            this.LocalWristPosition += offset;
            foreach (var finger in this.Fingers)
            {
                finger.LocalTipPosition += offset;
                foreach (var bone in finger.Bones)
                {
                    bone.LocalPosition += offset;
                }
            }
        }
    }
}
