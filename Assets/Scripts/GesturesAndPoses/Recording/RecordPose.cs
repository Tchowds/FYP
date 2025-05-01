using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecordPose : Recorder
{
    
    // The OVRSkeleton component is used to get the hand skeleton data
    [SerializeField] private OVRSkeleton handSkeleton;

    // Defines how often and how long to record the pose data
    [SerializeField] private float recordInterval = 1.0f;
    [SerializeField] private float endTime = 10f;

    // This class is responsible for recording the pose data of the hand
    [SerializeField] private HandPoseCalculations poseCalculator;
    private float nextRecordTime;
    private float startTime;
    private List<float[]> newPoseData;


    protected override void Initialize()
    {
        startTime = Time.time;
        nextRecordTime = startTime;
        newPoseData = new List<float[]>();
    }

    protected override void Update()
    {
        // If next record time has passed, record the pose data
        if (Time.time >= nextRecordTime)
        {
            float[] poseInstance = poseCalculator.getPose(handSkeleton);
            newPoseData.Add(poseInstance);
            nextRecordTime = Time.time + recordInterval;
        }
        // If the end time has passed, add all the pose data using the DataInterface and stop recording
        else if (Time.time - startTime >= endTime)
        {
            if (recordingMode == RecordingMode.NEW) DataInterface.addNewPoseData(nextToRecordName, newPoseData.ToArray());
            else if (recordingMode == RecordingMode.RETRAIN) DataInterface.retrainPose(nextToRecordName, newPoseData.ToArray());
            else if (recordingMode == RecordingMode.ENHANCE) DataInterface.enhancePose(nextToRecordName, newPoseData.ToArray());
            // DataInterface.addNewPoseData(nextToRecordName, newPoseData.ToArray());
            StopRecording();
        }
    }
}