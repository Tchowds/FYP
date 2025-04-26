using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecordingMode
{
    NEW,
    RETRAIN,
    ENHANCE
}

// Base class for recording gestures and poses
public abstract class Recorder : MonoBehaviour
{
    [SerializeField] protected RecordingMode recordingMode;
    protected string nextToRecordName = "";


    protected virtual void OnEnable()
    {
        Initialize();
    }

    protected abstract void Initialize();

    public virtual void StartRecording()
    {
        enabled = true;
    }

    public virtual void StopRecording()
    {
        enabled = false;
    }

    // Set the name of the gesture or pose to be recorded
    public virtual void SetNextToRecordName(string name)
    {
        nextToRecordName = name;
    }

    public void setToNEW()
    {
        recordingMode = RecordingMode.NEW;
    }

    public void setToRETRAIN()
    {
        recordingMode = RecordingMode.RETRAIN;
    }

    public void setToENHANCE()
    {
        recordingMode = RecordingMode.ENHANCE;
    }

    protected abstract void Update();
}