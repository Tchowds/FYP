using UnityEngine;

[System.Serializable]

public class PoseDetectorSubController : MonoBehaviour
{
    private OVRInput.Controller rightController = OVRInput.Controller.RTouch;
    private OVRInput.Controller leftController = OVRInput.Controller.LTouch;


    private string leftPose = "None";
    private string rightPose = "None";


    void Update()
    {
        detectPoses(rightController, ref rightPose);
        detectPoses(leftController, ref leftPose);
    }

    public void detectPoses(OVRInput.Controller controller, ref string pose)
    {
        if (OVRInput.GetDown(OVRInput.Button.One, controller))
        {
            pose = "fist";
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two, controller))
        {
            pose = "thumbs";
        }
        else if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            pose = "gun";
        }
    }


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