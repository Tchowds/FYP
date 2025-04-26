using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Represents a hand pose, including 15 angles and 7 distances between important joints
[System.Serializable]
public struct HandPoseData
{
    // Joint angles for each finger
    public float thumbAngle1;
    public float thumbAngle2;
    
    public float indexAngle1;
    public float indexAngle2;
    
    public float middleAngle1;
    public float middleAngle2;
    
    public float ringAngle1;
    public float ringAngle2;
    
    public float pinkyAngle1;
    public float pinkyAngle2;

    // Knuckle angles for each finger
    public float thumbKnuckleAngle;
    public float indexKnuckleAngle;
    public float middleKnuckleAngle;
    public float ringKnuckleAngle;
    public float pinkyKnuckleAngle;

    // Distances between important hand joints
    public float thumbToIndexDistance;
    public float thumbToPinkyDistance;
    public float wristToIndexDistance;
    public float indexToMiddleDistance;
    public float middleToRingDistance;
    public float ringToPinkyDistance;
    public float wristToPinkyDistance;

    public int poseIndex;
}

// Performs all the calculations for the hand pose data
[System.Serializable]
public class HandPoseCalculations
{

    private OVRSkeleton hand;

    // Function to calculate the distance between two joints or positions
    private float CalculateDistance(OVRSkeleton.BoneId boneId1, OVRSkeleton.BoneId boneId2)
    {
        Vector3 joint1 = hand.Bones[(int)boneId1].Transform.position;
        Vector3 joint2 = hand.Bones[(int)boneId2].Transform.position;
        return Vector3.Distance(joint1, joint2);
    }

    // Function to calculate the angle between three joints or positions
    private float CalculateAngle(OVRSkeleton.BoneId boneId1, OVRSkeleton.BoneId boneId2, OVRSkeleton.BoneId boneId3)
    {
        Vector3 p1 = hand.Bones[(int)boneId1].Transform.position;
        Vector3 p2 = hand.Bones[(int)boneId2].Transform.position;
        Vector3 p3 = hand.Bones[(int)boneId3].Transform.position;
        return Vector3.Angle(p2 - p1, p3 - p2);
    }

    // Function to calculate the knuckle angle for a given finger
    private float CalculateKnuckleAngle(OVRSkeleton.BoneId boneIdBase, OVRSkeleton.BoneId boneId1, OVRSkeleton.BoneId boneId2)
    {
        // Get positions of the joints
        Vector3 palmBase = hand.Bones[(int)boneIdBase].Transform.position;
        Vector3 joint1 = hand.Bones[(int)boneId1].Transform.position;
        Vector3 joint2 = hand.Bones[(int)boneId2].Transform.position;

        // Create vectors for palm to joint1 and joint1 to joint2
        Vector3 palmToJoint1 = joint1 - palmBase;
        Vector3 joint1ToJoint2 = joint2 - joint1;

        // Calculate angle using dot product
        return Vector3.Angle(palmToJoint1, joint1ToJoint2);
    }


    // Calculates all the pose data and returns it as an array of floats
    public float[] getPose(OVRSkeleton handSkeleton)
    {
        hand = handSkeleton;
        
        try
        {
            float[] pose = new float[]
            {
                // Joint angles for thumb, index, middle, ring, and pinky
                CalculateAngle(OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3),
                CalculateAngle(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_ThumbTip),

                CalculateAngle(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3),
                CalculateAngle(OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_IndexTip),

                CalculateAngle(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3),
                CalculateAngle(OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MiddleTip),

                CalculateAngle(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3),
                CalculateAngle(OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_RingTip),

                CalculateAngle(OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3),
                CalculateAngle(OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_PinkyTip),

                // Knuckle angles for each finger
                CalculateAngle(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb2),
                CalculateAngle(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2),
                CalculateAngle(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2),
                CalculateAngle(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2),
                CalculateAngle(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2),

                // Distances between joints
                CalculateDistance(OVRSkeleton.BoneId.Hand_ThumbTip, OVRSkeleton.BoneId.Hand_IndexTip),
                CalculateDistance(OVRSkeleton.BoneId.Hand_ThumbTip, OVRSkeleton.BoneId.Hand_PinkyTip),
                CalculateDistance(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_IndexTip),
                CalculateDistance(OVRSkeleton.BoneId.Hand_IndexTip, OVRSkeleton.BoneId.Hand_MiddleTip),
                CalculateDistance(OVRSkeleton.BoneId.Hand_MiddleTip, OVRSkeleton.BoneId.Hand_RingTip),
                CalculateDistance(OVRSkeleton.BoneId.Hand_RingTip, OVRSkeleton.BoneId.Hand_PinkyTip),
                CalculateDistance(OVRSkeleton.BoneId.Hand_WristRoot, OVRSkeleton.BoneId.Hand_PinkyTip)
            };

            return pose;
        }
        catch (ArgumentOutOfRangeException e)
        {
            return null;
        }
    }

}
