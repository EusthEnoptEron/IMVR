using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Leap;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System.IO;

namespace Gestures
{
    public class AvatarHandIntegrator : HandProvider
    {
        public HandProvider controller;
        public float returnSpeed = 1;
            private Animator animator;
        private Dictionary<AvatarIKGoal, float> weights;
        private Dictionary<AvatarIKGoal, GenericHand> handCache;

        private HumanBodyBones[,,] LUT;
        private Dictionary<HumanBodyBones, Quaternion> offsetTable;
        public Transform eyePosition;


        void Awake()
        {
            animator = GetComponent<Animator>();
            weights = new Dictionary<AvatarIKGoal, float>();
            handCache = new Dictionary<AvatarIKGoal, GenericHand>();

            weights[AvatarIKGoal.LeftHand] = 0;
            weights[AvatarIKGoal.RightHand] = 0;
            handCache[AvatarIKGoal.LeftHand] = null;
            handCache[AvatarIKGoal.RightHand] = null;

            #if UNITY_EDITOR
                var metaFile = new MetaFile( AssetDatabase.GetAssetPath( animator.avatar ));
                metaFile.EnforceTPose(gameObject);
            #endif

            buildLUT();
            buildOffsets();

           // EditorApplication.isPaused = true;
        }

        private void buildOffsets()
        {
            offsetTable = new Dictionary<HumanBodyBones, Quaternion>();
            //string assetPath = AssetDatabase.GetAssetPath(animator.avatar);
            
            foreach (bool isLeft in new bool[]{false, true})
            {

                foreach (FingerType fingerType in (FingerType[])Enum.GetValues(typeof(FingerType)))
                {
                    Vector3 fwd = (isLeft ? -transform.right : transform.right);
                    if (fingerType == FingerType.Thumb)
                    {
                        fwd = transform.forward;
                    }

                    var from = getBoneTransform(isLeft, fingerType, BoneType.Proximal);
                    var to   = getBoneTransform(isLeft, fingerType, BoneType.Intermediate);

                    if (from != null && to != null)
                    {
                        fwd = (to.position - from.position).normalized;
                    }

                    foreach (BoneType boneType in (BoneType[])Enum.GetValues(typeof(BoneType)))
                    {
                        var boneTransform = getBoneTransform(isLeft, fingerType, boneType);
                        var name = getBoneMapping(isLeft, fingerType, boneType);
                        if (boneTransform != null)
                        {
                            // We know that transform.up points upward and that fwd is the (absolute) direction the fingers point toward, so we can reorientate them accordingly
                            // Using the inverse quaternions, we convert them into relative directions
                            var rotatedUp = Quaternion.Inverse(boneTransform.rotation) * transform.up;
                            var rotatedFwd = Quaternion.Inverse(boneTransform.rotation) * fwd;
                            //offsetTable[name] = Quaternion.Inverse(baseRotation * (Quaternion.Inverse(boneTransform.rotation) * baseRotation));
                            offsetTable[name] = Quaternion.Inverse(Quaternion.LookRotation(rotatedFwd, rotatedUp));
                            
                        }
                    }
                }
            }


        }

        // Update is called once per frame
        void OnAnimatorIK()
        {
            var leftHand = controller.LeftHand;
            var rightHand = controller.RightHand;

            setIK(leftHand, AvatarIKGoal.LeftHand);
            setIK(rightHand, AvatarIKGoal.RightHand);

            animator.SetLookAtWeight(1, 0.2f, 0.8f);
            animator.SetLookAtPosition(eyePosition.position + Camera.main.transform.forward * 1f + Vector3.down * 0.15f);
            //animator.SetLookAtPosition(Camera.main.transform.position + Vector3.down * 0.15f);
        }

        void LateUpdate()
        {
            foreach(var hand in handCache.Values) {
                if (hand != null)
                {
                    positionHand(hand);
                    
                }
            }
        }

        private void setIK(GenericHand hand, AvatarIKGoal goal)
        {
            HumanBodyBones target = goal == AvatarIKGoal.LeftHand ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand;

            if (hand == null)
            {
                weights[goal] = Mathf.Max(weights[goal] - Time.deltaTime * returnSpeed, 0);

                if (handCache[goal] != null)
                {
                    animator.SetIKPosition(goal, handCache[goal].WristPosition);
                    animator.SetIKRotation(goal, handCache[goal].PalmRotation);// * Reorientation());
                }
            }
            else
            {
                weights[goal] = 1;
                // Set IKs
                animator.SetIKPosition(goal, hand.WristPosition);
                animator.SetIKRotation(goal, hand.PalmRotation); //* Reorientation());

                handCache[goal] = hand;
            }
            //Debug.Log("IK..." + weights[goal]);

            // Set weights
            animator.SetIKPositionWeight(goal, weights[goal]);
            animator.SetIKRotationWeight(goal, weights[goal]);
        }


        private Quaternion Reorientation(HumanBodyBones bone)
        {
            return offsetTable[bone];
            //return Quaternion.Inverse(Quaternion.LookRotation(forwardVector, upVector));
        }

        private void positionHand(GenericHand hand)
        {
            HumanBodyBones b = hand.IsLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand;
          //  animator.GetBoneTransform(b).rotation = hand.PalmRotation * Reorientation();
            if (animator.avatar.isHuman)
            {
                var avatar = animator.avatar;
                
            }
            foreach(var finger in hand.Fingers) {
                foreach (var bone in finger.Bones)
                {
                    var transform = getBoneTransform(hand, finger, bone);
                    
                    if (transform != null)
                    {
                        transform.rotation = bone.Rotation * Reorientation(getBoneMapping(hand, finger, bone));
                    }
                }
            }
        }


        private Transform getBoneTransform(GenericHand hand, GenericFinger finger, GenericBone bone)
        {
            return getBoneTransform(hand.IsLeft, finger.Type, bone.Type);
        }
        private Transform getBoneTransform(bool isLeft, FingerType fingerType, BoneType boneType)
        {
            HumanBodyBones empty = (HumanBodyBones)0;

            var uBoneType = LUT[Convert.ToInt32(isLeft), (int)fingerType, (int)boneType];
            //Debug.Log(String.Format("[{0}, {1}, {2}]", Convert.ToInt32(hand.IsLeft), (int)finger.FingerType, (int)bone.Type));
            if (uBoneType == empty) return null;
            else
            {
                return animator.GetBoneTransform(uBoneType);
            }
        }

        private HumanBodyBones getBoneMapping(GenericHand hand, GenericFinger finger, GenericBone bone)
        {
            return getBoneMapping(hand.IsLeft, finger.Type, bone.Type);
        }

        private HumanBodyBones getBoneMapping(bool isLeft, FingerType fingerType, BoneType boneType)
        {
            return LUT[Convert.ToInt32(isLeft), (int)fingerType, (int)boneType];
        }

        private void buildLUT()
        {
            var fingerTypes = (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType));
            var boneTypes   = (Finger.FingerType[])Enum.GetValues(typeof(Bone.BoneType));
            LUT = new HumanBodyBones[2, fingerTypes.Length, boneTypes.Length];


            // Right
            LUT[0, (int)Finger.FingerType.TYPE_THUMB, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.RightThumbDistal;
            LUT[0, (int)Finger.FingerType.TYPE_THUMB, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.RightThumbIntermediate;
            LUT[0, (int)Finger.FingerType.TYPE_THUMB, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.RightThumbProximal;

            LUT[0, (int)Finger.FingerType.TYPE_INDEX, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.RightIndexDistal;
            LUT[0, (int)Finger.FingerType.TYPE_INDEX, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.RightIndexIntermediate;
            LUT[0, (int)Finger.FingerType.TYPE_INDEX, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.RightIndexProximal;

            LUT[0, (int)Finger.FingerType.TYPE_MIDDLE, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.RightMiddleDistal;
            LUT[0, (int)Finger.FingerType.TYPE_MIDDLE, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.RightMiddleIntermediate;
            LUT[0, (int)Finger.FingerType.TYPE_MIDDLE, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.RightMiddleProximal;

            LUT[0, (int)Finger.FingerType.TYPE_RING, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.RightRingDistal;
            LUT[0, (int)Finger.FingerType.TYPE_RING, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.RightRingIntermediate;
            LUT[0, (int)Finger.FingerType.TYPE_RING, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.RightRingProximal;

            LUT[0, (int)Finger.FingerType.TYPE_PINKY, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.RightLittleDistal;
            LUT[0, (int)Finger.FingerType.TYPE_PINKY, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.RightLittleIntermediate;
            LUT[0, (int)Finger.FingerType.TYPE_PINKY, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.RightLittleProximal;


            // Left
            LUT[1, (int)Finger.FingerType.TYPE_THUMB, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.LeftThumbDistal;
            LUT[1, (int)Finger.FingerType.TYPE_THUMB, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.LeftThumbIntermediate;
            LUT[1, (int)Finger.FingerType.TYPE_THUMB, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.LeftThumbProximal;

            LUT[1, (int)Finger.FingerType.TYPE_INDEX, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.LeftIndexDistal;
            LUT[1, (int)Finger.FingerType.TYPE_INDEX, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.LeftIndexIntermediate;
            LUT[1, (int)Finger.FingerType.TYPE_INDEX, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.LeftIndexProximal;

            LUT[1, (int)Finger.FingerType.TYPE_MIDDLE, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.LeftMiddleDistal;
            LUT[1, (int)Finger.FingerType.TYPE_MIDDLE, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.LeftMiddleIntermediate;
            LUT[1, (int)Finger.FingerType.TYPE_MIDDLE, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.LeftMiddleProximal;

            LUT[1, (int)Finger.FingerType.TYPE_RING, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.LeftRingDistal;
            LUT[1, (int)Finger.FingerType.TYPE_RING, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.LeftRingIntermediate;
            LUT[1, (int)Finger.FingerType.TYPE_RING, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.LeftRingProximal;

            LUT[1, (int)Finger.FingerType.TYPE_PINKY, (int)Bone.BoneType.TYPE_DISTAL] = HumanBodyBones.LeftLittleDistal;
            LUT[1, (int)Finger.FingerType.TYPE_PINKY, (int)Bone.BoneType.TYPE_INTERMEDIATE] = HumanBodyBones.LeftLittleIntermediate;
            LUT[1, (int)Finger.FingerType.TYPE_PINKY, (int)Bone.BoneType.TYPE_PROXIMAL] = HumanBodyBones.LeftLittleProximal;

        }


        public override GenericHand LeftHand
        {
            get {
                var hand = controller.LeftHand;
                if (hand == null) return null;
                return getHand(new GenericHand(hand)); 
            }
        }

        private GenericHand getHand(GenericHand hand)
        {
            //hand.WristPosition = animator.GetBoneTransform(hand.IsLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand).position;

            foreach (var finger in hand.Fingers)
            {
                GenericBone prevBone = null, lastBone = null;

                foreach (var bone in finger.Bones)
                {
                    var transform = getBoneTransform(hand, finger, bone);
                    if (transform != null)
                    {
                        bone.Position = transform.position;
                        prevBone = lastBone;
                        lastBone = bone;
                    }
                }
                if (prevBone != null && lastBone != null)
                {
                    finger.TipPosition = lastBone.Position + (lastBone.Position - prevBone.Position);

                }
            }

            return hand;
        }

        public override GenericHand RightHand
        {
            get {
                var hand = controller.RightHand;
                if (hand == null) return null;
                return getHand(new GenericHand(hand)); 
            }
        }


        public override GenericHand GetHand(HandType type, NoHandStrategy strategy = NoHandStrategy.SetNull)
        {
            // TODO: implement properly
            if (type == HandType.Left) return LeftHand;
            else return RightHand;
        }
    }


    class MetaFile
    {
        Dictionary<string, SkeletonBone> bones;
        public MetaFile(string path)
        {
            bones = new Dictionary<string, SkeletonBone>();

            if(!path.EndsWith(".meta")) path += ".meta";

            if (path != null && path != "" && File.Exists(path))
            {
                //Debug.Log("FOUND");
                using (var reader = new StreamReader(File.OpenRead(path)))
                {
                    readSkeleton(reader);
                }

                //Debug.Log(bones.Count);

            }
            else
            {
                //Debug.Log("NOT FOUND");
            }
        }

        private void readSkeleton(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int level = line.Length - line.TrimStart().Length;
                if (line.Trim() == "skeleton:")
                {
                    while (!reader.EndOfStream && readBone(reader));
                 
                    break;
                }
            }
        }

        private bool readBone(StreamReader reader)
        {
            if(reader.EndOfStream) return false;
            string line = reader.ReadLine().Trim();
            if(!line.StartsWith("-")) return false;
            
            SkeletonBone bone = new SkeletonBone();
            line = line.Substring(2);

            bone.name = line.Split(':')[1].Trim();

            int varsSet = 0;
            for (int i = 1; i < 5; i++)
            {
                var splittedLine = reader.ReadLine().Split(new char[]{':'}, 2);
                switch (splittedLine[0].Trim())
                {
                    case "position":
                        var position = SimpleJSON.JSON.Parse(splittedLine[1]);
                        bone.position = new Vector3(position["x"].AsFloat, position["y"].AsFloat, position["z"].AsFloat);
                        varsSet++;
                    break;
                    case "rotation":
                        var rotation = SimpleJSON.JSON.Parse(splittedLine[1]);
                        bone.rotation = new Quaternion(rotation["x"].AsFloat, rotation["y"].AsFloat, rotation["z"].AsFloat, rotation["w"].AsFloat);
                        varsSet++;
                    break;
                    case "scale":
                        var scale = SimpleJSON.JSON.Parse(splittedLine[1]);
                        bone.scale = new Vector3(scale["x"].AsFloat, scale["y"].AsFloat, scale["z"].AsFloat);
                        varsSet++;
                    break;
                    default:
                    break;
                }
            }
            if(varsSet == 3)
                bones[bone.name] = bone;

            return true;
        }

        public void EnforceTPose(GameObject gameObject)
        {
            traverseTransform(gameObject.transform, true);
        }

        private void traverseTransform(Transform go, bool start = false)
        {
            if (!start && bones.ContainsKey(go.name))
            {
                var bone = bones[go.name];

                go.localPosition = bone.position;
                go.localScale = bone.scale;
                go.localRotation = bone.rotation;
            }

            for (int i = 0; i < go.childCount; i++)
            {
                traverseTransform(go.GetChild(i));
            }
        }
    }

}