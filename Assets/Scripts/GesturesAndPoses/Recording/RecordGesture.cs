using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordGesture : Recorder
{
    // Hand Buffer to get trajectory data from
    [SerializeField] private HandBuffer handBuffer;
    [SerializeField] private float boundingCubeSize = 1.0f;
    [SerializeField] private Transform centerEyeAnchor;

    private float recordInterval;
    // End time used to only start reording after a certain time
    private float endTime;
    private Vector3[][] newGestureData;

    protected override void Initialize()
    {
        recordInterval = handBuffer.getBufferTimeLength();
        endTime = Time.time + recordInterval;
    }

    // Get the hand buffer data and run transformations on it
    // Then add the data to the DataInterface based on the recording mode
    protected override void Update()
    {
        if (Time.time > endTime)
        {
            newGestureData = handBuffer.getAllBuffers();
            float[][] transformedData = runTransformations();

            if (recordingMode == RecordingMode.NEW) DataInterface.addNewGestureData(nextToRecordName, transformedData);
            else if (recordingMode == RecordingMode.RETRAIN) DataInterface.retrainGesture(nextToRecordName, transformedData);
            else if (recordingMode == RecordingMode.ENHANCE) DataInterface.enhanceGesture(nextToRecordName, transformedData);

            StopRecording();
        }
    }

    // Use the HandGestureCalculations class to run all transformations on the hand buffer data
    private float[][] runTransformations()
    {
        float[][] transformedData = new float[newGestureData.Length][];
        for (int i = 0; i < newGestureData.Length; i++)
        {
            Vector3[] buffer = newGestureData[i];
            if (buffer != null) transformedData[i] = HandGestureCalculations.runAllTransformations(buffer, centerEyeAnchor.rotation, boundingCubeSize);               
            else Debug.Log("Buffer is null");
        }
        return transformedData;
    }
}
