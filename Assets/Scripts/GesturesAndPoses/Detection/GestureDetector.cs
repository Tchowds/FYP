using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Enumeration for the different strategies used for gesture interpolation
public enum IntervalStrategy
{
    BEST,
    SMALLEST,
    LARGEST
}

// This script is responsible for detecting hand gestures using the MLClassifier and the HandBuffer classes. 
// It processes the hand data and updates the UI with the detected gesture names.
public class GestureDetector : MonoBehaviour
{

    [Header("Hand Gesture classes")]
    [SerializeField] private HandBuffer handBufferRight;
    [SerializeField] private HandBuffer handBufferLeft;

    [Header("Classifier")]
    [SerializeField] private MLClassifier gestureClassifier;

    [Header("Enabled hands")]
    [SerializeField] private bool enableRight = true;
    [SerializeField] private bool enableLeft = true;


    [Header("Scaling Parameters")]
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private float boundingCubeSize = 1.0f;
    [SerializeField] private bool useInterpolation = false;


    [Header("Logging")]
    [SerializeField] private TMP_Text statusTextRight;
    [SerializeField] private TMP_Text statusTextLeft;
    [SerializeField] private bool logData = true;

    // These define the parameter for gesture interpolation through dynamic time warping
    [Header("Gesture Interpolation Intervals")]
    [SerializeField] private float interpolationInterval = 0.5f;
    [SerializeField] private float minInterval = 1.0f;
    [SerializeField] private IntervalStrategy intervalStrategy = IntervalStrategy.BEST;

    [Header("Confidence values")]
    [SerializeField] private float confidenceThreshold = 0.9f;
    
    private string defaultTextRight = "None";
    private string defaultTextLeft = "None";

    private string leftGesture = "None";
    private string rightGesture = "None"; 


    // Start is called before the first frame update
    void Start()
    {
        // Set default text for statusText
        if(logData)
        {
            defaultTextRight = statusTextRight.text;
            statusTextRight.text = defaultTextRight + " None";
            defaultTextLeft = statusTextLeft.text;
            statusTextLeft.text = defaultTextLeft + " None";            
        }

    }

    void Update()
    {
        if (!DataInterface.moreThanOneGesture())
        {
            if(logData)
            {
                statusTextRight.text = "Not enough gestures for classification";
                statusTextLeft.text = "Not enough gestures for classification";                
            }
            return;
        }

        if(useInterpolation)
        {
            if(enableRight) ProcessHandGestureWithInterpolation(handBufferRight, statusTextRight, defaultTextRight, true);
            if(enableLeft) ProcessHandGestureWithInterpolation(handBufferLeft, statusTextLeft, defaultTextLeft, false);
        }
        else
        {
            if(enableRight) ProcessHandGesture(handBufferRight, statusTextRight, defaultTextRight, true);
            if(enableLeft) ProcessHandGesture(handBufferLeft, statusTextLeft, defaultTextLeft, false);
        }


    }

    // Runs the full detection process for the hand gesture
    private void ProcessHandGestureWithInterpolation(HandBuffer handBuffer, TMP_Text statusText, string defaultText, bool isRightHand)
    {
        // Gets all the interpolated buffers from the handBuffer
        Vector3[][] buffers = handBuffer.getInterpolatedBuffers(interpolationInterval, minInterval, intervalStrategy);
        if (buffers == null) return;

        // Processes each buffer to get output individually
        List<float[]> roundedOutputs = new List<float[]>();
        for (int i = 0; i < buffers.Length; i++)
        {
            if(buffers[i] == null) continue;
            float[] roundedOutput = processBuffer(buffers[i]);
            if(roundedOutput == null) continue;
            roundedOutputs.Add(roundedOutput);
        }

        float[] bestOutput = new float[roundedOutputs[0].Length];
        float highestValue = float.MinValue;

        // Runs inference strategy on all the buffers
        foreach (float[] array in roundedOutputs)
        {
            // For BEST strategy, iterate through all the outputs and get the one with the highest value
            if (intervalStrategy == IntervalStrategy.BEST)
            {
                for(int i = 0; i < array.Length; i++)
                {
                    if(array[i] > highestValue)
                    {
                        highestValue = array[i];
                        bestOutput = array;
                    }
                }

                // If the highest value is above the confidence threshold, get the gesture name and update the status text
                if (highestValue > confidenceThreshold)
                {
                    string gestureName = DataInterface.whichGesture(System.Array.IndexOf(roundedOutputs.ToArray(), bestOutput));
                    if(logData) statusText.text = defaultText + " " + gestureName;

                    if (isRightHand) rightGesture = gestureName;
                    else leftGesture = gestureName;
                    
                    // Lock detection for a second to avoid multiple detections
                    StartCoroutine(SkipUpdateForDuration(isRightHand));
                    return;
                }
            }
            // For SMALLEST and LARGEST strategy, stop when the first value is above the confidence threshold
            // HandGestureCalculations handles ordering the buffers in the right order based on the strategy
            else
            {
                for (int i = 0; i < array.Length; i++)
                {
                    var val = array[i];
                    // Check if the value is above the confidence threshold
                    // If it is, get the gesture name and update the status text
                    if (val > confidenceThreshold)
                    {
                        string gestureName = DataInterface.whichGesture(i);
                        if(logData) statusText.text = defaultText + " " + gestureName;

                        if (isRightHand) rightGesture = gestureName;
                        else leftGesture = gestureName;

                        // Lock detection for a second to avoid multiple detections
                        StartCoroutine(SkipUpdateForDuration(isRightHand));
                        return;
                    }
                }                
            }
        }
    }

    // If interpolation isn't used, this function runs the detection process for the hand gesture
    // This doesn't use the interpolation strategy since there is only one buffer to process
    private void ProcessHandGesture(HandBuffer handBuffer, TMP_Text statusText, string defaultText, bool isRightHand)
    {
        Vector3[] buffer = handBuffer.getBufferWithDeadzone();
        if (buffer == null) return;

        float[] roundedOutput = processBuffer(buffer);
        if(roundedOutput == null) return;
        
        // int maxIndex = System.Array.IndexOf(output, Mathf.Max(output));
        int maxIndex = System.Array.IndexOf(roundedOutput, Mathf.Max(roundedOutput));
        if (maxIndex == 0) return;
        string gestureName = DataInterface.whichGesture(maxIndex);

        if (roundedOutput[maxIndex] > confidenceThreshold)
        {
            Debug.Log("Buffer data: " + string.Join(", ", buffer));
            if (logData) statusText.text = defaultText + " " + gestureName;

            if (isRightHand) rightGesture = gestureName;
            else leftGesture = gestureName;

            StartCoroutine(SkipUpdateForDuration(isRightHand));
        }
    }

    // Runs all the transformations on the hand buffer data and runs inference on it
    private float[] processBuffer(Vector3[] buffer)
    {
        // Runs all transformations on the hand buffer data
        float[] transformedData = HandGestureCalculations.runAllTransformations(buffer, centerEyeAnchor.rotation, boundingCubeSize);
        // Runs inference on the transformed data
        float[] output = gestureClassifier.RunInference(transformedData);
        if(output == null) return null;

        // Run softmax on the output to get the probabilities of each gesture
        float sumExp = 0f;
        float[] softmaxOutput = new float[output.Length];
        for (int i = 0; i < output.Length; i++)
        {
            softmaxOutput[i] = Mathf.Exp(output[i]);
            sumExp += softmaxOutput[i];
        }
        for (int i = 0; i < output.Length; i++) softmaxOutput[i] /= sumExp;
        
        // Round the output to 2 decimal places for better readability when logging
        float[] roundedOutput = new float[softmaxOutput.Length];
        for (int i = 0; i < softmaxOutput.Length; i++) roundedOutput[i] = Mathf.Round(softmaxOutput[i] * 100f) / 100f;

        return roundedOutput;
    }

    // Delay to disable gesture detection for a certain duration
    private IEnumerator SkipUpdateForDuration(bool isRightHand)
    {
        if (isRightHand)
        {
            enableRight = false;
            yield return new WaitForSeconds(1.0f);
            enableRight = true;
        }
        else
        {
            enableLeft = false;
            yield return new WaitForSeconds(1.0f);
            enableLeft = true;
        }
    }

    // Functions used by external classes to get the current detected gesture
    // and to reset the pose if needed
    public string getLeftGesture()
    {
        return leftGesture;
    }

    public string getRightGesture()
    {
        return rightGesture;
    }

    public void resetLeftGesture()
    {
        leftGesture = "None";
    }

    public void resetRightGesture()
    {
        rightGesture = "None";
    }
}
