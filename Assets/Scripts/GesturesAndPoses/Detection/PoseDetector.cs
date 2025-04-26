using UnityEngine;
using TMPro;

// This script is reponsible for constantly detecting the hand poses of the user
// It uses the MLClassifier to classify the hand poses based on the data from the OVRSkeleton and OVRHand
// The script also handles the logging of the detected poses to the UI if enabled
[System.Serializable]
public class PoseDetector : MonoBehaviour
{
    [Header("Hand Pose classes")]
    [SerializeField] private HandPoseCalculations poseGetter;

    [Header("Classifier")]
    [SerializeField] private MLClassifier poseClassifier;

    [Header("Hand objects")]
    [SerializeField] private OVRSkeleton rightHandSkeleton;
    [SerializeField] private OVRSkeleton leftHandSkeleton;
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private OVRHand leftHand;

    [Header("Enabled Hands")]
    [SerializeField] private bool enableRight = true;
    [SerializeField] private bool enableLeft = true;

    [Header("Logging")]
    [SerializeField] private TMP_Text statusTextRight;
    [SerializeField] private TMP_Text statusTextLeft;
    [SerializeField] private bool logData = true;

    [Header("Confidence values")]
    [SerializeField] private float confidenceThreshold = 0.9f;

    private string defaultTextRight = "None";
    private string defaultTextLeft = "None";

    private string leftPose = "None";
    private string rightPose = "None";


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
        if(enableRight) DetectHandPose(rightHandSkeleton, rightHand, statusTextRight, defaultTextRight, ref rightPose);
        if(enableLeft) DetectHandPose(leftHandSkeleton, leftHand, statusTextLeft, defaultTextLeft, ref leftPose);
    }

    // Runs the full detection process for the hand pose
    private void DetectHandPose(OVRSkeleton handSkeleton, OVRHand hand, TMP_Text statusText, string defaultText, ref string poseField)
    {
        // Can't run detection if there is less than two poses in the dataset
        if (!DataInterface.moreThanOnePose())
        {
            if(logData) statusText.text = "Not enough poses for classification";
        }
        else if (hand.IsTracked)
        {
            // Get the mapped pose data from the hand skeleton and run inference on it
            float[] handPoseData = poseGetter.getPose(handSkeleton);
            if (handPoseData == null) return;
            float[] output = poseClassifier.RunInference(handPoseData);

            // Run softmax on the output to get the probabilities of each pose
            float sumExp = 0f;
            float[] softmaxOutput = new float[output.Length];
            for (int i = 0; i < output.Length; i++)
            {
                softmaxOutput[i] = Mathf.Exp(output[i]);
                sumExp += softmaxOutput[i];
            }
            for (int i = 0; i < output.Length; i++)
            {
                softmaxOutput[i] /= sumExp;
            }
            // Round the output to 2 decimal places for better readability when logging
            float[] roundedOutput = new float[softmaxOutput.Length];
            for (int i = 0; i < softmaxOutput.Length; i++)
            {
                roundedOutput[i] = Mathf.Round(softmaxOutput[i] * 100f) / 100f;
            }

            // Calculate the index of the maximum value in the output array
            int maxIndex = System.Array.IndexOf(output, Mathf.Max(output));
            string poseName = DataInterface.whichPose(maxIndex);

            // Check if the maximum value is above the confidence threshold
            // If it is, set the pose field to the pose name and log it
            if (roundedOutput[maxIndex] > confidenceThreshold)
            {
                poseField = poseName;
                if (logData) statusText.text = defaultText + " " + poseName;
            }
            else if (logData) statusText.text = defaultText + " None";
        }
    }

    // Functions used by external classes to get the current detected pose
    // and to reset the pose if needed
    public string getLeftPose()
    {
        return leftPose;
    }

    public string getRightPose()
    {
        return rightPose;
    }

    public void resetLeftPose()
    {
        leftPose = "None";
    }

    public void resetRightPose()
    {
        rightPose = "None";
    }
}