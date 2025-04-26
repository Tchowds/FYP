using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Completely symmetrical to GestureDetector
// Refer to GestureDetector for comments on the code
public class GestureDetectorController : MonoBehaviour
{
    [Header("Hand Gesture classes")]
    [SerializeField] private HandBufferController handBufferRight;
    [SerializeField] private HandBufferController handBufferLeft;
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

    void Start()
    {
        if (logData)
        {
            defaultTextRight = statusTextRight.text;
            statusTextRight.text = defaultTextRight + " None";
            defaultTextLeft = statusTextLeft.text;
            statusTextLeft.text = defaultTextLeft + " None";
        }
    }

    void Update()
    {
        // Ensure there is enough gesture data before attempting classification.
        if (!DataInterface.moreThanOneGesture())
        {
            if (logData)
            {
                statusTextRight.text = "Not enough gestures for classification";
                statusTextLeft.text = "Not enough gestures for classification";
            }
            return;
        }

        if (useInterpolation)
        {
            if (enableRight)
                ProcessHandGestureWithInterpolation(handBufferRight, statusTextRight, defaultTextRight, true);
            if (enableLeft)
                ProcessHandGestureWithInterpolation(handBufferLeft, statusTextLeft, defaultTextLeft, false);
        }
        else
        {
            if (enableRight)
                ProcessHandGesture(handBufferRight, statusTextRight, defaultTextRight, true);
            if (enableLeft)
                ProcessHandGesture(handBufferLeft, statusTextLeft, defaultTextLeft, false);
        }
    }

    private void ProcessHandGestureWithInterpolation(HandBufferController handBuffer, TMP_Text statusText, string defaultText, bool isRightHand)
    {
        Vector3[][] buffers = handBuffer.GetInterpolatedBuffers(interpolationInterval, minInterval, intervalStrategy);
        if (buffers == null) return;

        List<float[]> roundedOutputs = new List<float[]>();
        for (int i = 0; i < buffers.Length; i++)
        {
            if (buffers[i] == null) continue;
            float[] roundedOutput = ProcessBuffer(buffers[i]);
            roundedOutputs.Add(roundedOutput);
        }

        float[] bestOutput = new float[roundedOutputs[0].Length];
        float highestValue = float.MinValue;

        foreach (float[] array in roundedOutputs)
        {
            if (intervalStrategy == IntervalStrategy.BEST)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] > highestValue)
                    {
                        highestValue = array[i];
                        bestOutput = array;
                    }
                }

                if (highestValue > confidenceThreshold)
                {
                    string gestureName = DataInterface.whichGesture(System.Array.IndexOf(roundedOutputs.ToArray(), bestOutput));
                    if (logData)
                        statusText.text = defaultText + " " + gestureName + " (" + string.Join(", ", bestOutput) + ")";

                    if (isRightHand)
                        rightGesture = gestureName;
                    else
                        leftGesture = gestureName;

                    StartCoroutine(SkipUpdateForDuration(isRightHand));
                    return;
                }
            }
            else
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] > confidenceThreshold)
                    {
                        string gestureName = DataInterface.whichGesture(i);
                        if (logData)
                            statusText.text = defaultText + " " + gestureName + " (" + string.Join(", ", array) + ")";

                        if (isRightHand)
                            rightGesture = gestureName;
                        else
                            leftGesture = gestureName;

                        StartCoroutine(SkipUpdateForDuration(isRightHand));
                        return;
                    }
                }
            }
        }
    }

    private void ProcessHandGesture(HandBufferController handBuffer, TMP_Text statusText, string defaultText, bool isRightHand)
    {
        Vector3[] buffer = handBuffer.GetBufferWithDeadzone();
        if (buffer == null) return;

        float[] roundedOutput = ProcessBuffer(buffer);

        int maxIndex = System.Array.IndexOf(roundedOutput, Mathf.Max(roundedOutput));
        if (maxIndex == 0) return;
        string gestureName = DataInterface.whichGesture(maxIndex);

        if (roundedOutput[maxIndex] > confidenceThreshold)
        {
            Debug.Log("Buffer data: " + string.Join(", ", buffer));
            if (logData)
                statusText.text = defaultText + " " + gestureName + "\n" + string.Join(", ", roundedOutput);

            if (isRightHand)
                rightGesture = gestureName;
            else
                leftGesture = gestureName;

            StartCoroutine(SkipUpdateForDuration(isRightHand));
        }
    }

    // Applies all necessary transformations to the buffer and runs inference on the gesture classifier.
    private float[] ProcessBuffer(Vector3[] buffer)
    {
        float[] transformedData = HandGestureCalculations.runAllTransformations(buffer, centerEyeAnchor.rotation, boundingCubeSize);
        float[] output = gestureClassifier.RunInference(transformedData);

        float sumExp = 0f;
        float[] softmaxOutput = new float[output.Length];
        for (int i = 0; i < output.Length; i++)
        {
            softmaxOutput[i] = Mathf.Exp(output[i]);
            sumExp += softmaxOutput[i];
        }
        for (int i = 0; i < output.Length; i++)
            softmaxOutput[i] /= sumExp;

        float[] roundedOutput = new float[softmaxOutput.Length];
        for (int i = 0; i < softmaxOutput.Length; i++)
            roundedOutput[i] = Mathf.Round(softmaxOutput[i] * 100f) / 100f;

        return roundedOutput;
    }

    // Temporarily disables gesture processing for a given hand.
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
            // yield return new WaitForSeconds(handBufferLeft.GetBufferTimeLength());
            yield return new WaitForSeconds(1.0f);
            enableLeft = true;
        }
    }

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
