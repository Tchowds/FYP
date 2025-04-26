using UnityEngine;

public class VRControllerButtonDetector : MonoBehaviour
{
    [Tooltip("Set this to RTouch for right hand, LTouch for left hand")]
    public OVRInput.Controller controller;

    void Update()
    {
        // Face buttons (A/X and B/Y) are mapped the same for both hands.
        if (OVRInput.GetDown(OVRInput.Button.One, controller))
        {
            Debug.Log(controller + " Button One pressed");
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, controller))
        {
            Debug.Log(controller + " Button Two pressed");
        }
        
        // Thumbstick button: Primary for right hand, Secondary for left hand.
        if (controller == OVRInput.Controller.RTouch)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, controller))
            {
                Debug.Log(controller + " Thumbstick pressed");
            }
        }
        else if (controller == OVRInput.Controller.LTouch)
        {
            if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick, controller))
            {
                Debug.Log(controller + " Thumbstick pressed");
            }
        }
        
        // Index trigger: note that these are analog inputs but can be polled as buttons.
        if (controller == OVRInput.Controller.RTouch)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                Debug.Log(controller + " Index Trigger pressed");
            }
        }
        else if (controller == OVRInput.Controller.LTouch)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                Debug.Log(controller + " Index Trigger pressed");
            }
        }
        
        // Hand trigger.
        if (controller == OVRInput.Controller.RTouch)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            {
                Debug.Log(controller + " Hand Trigger pressed");
            }
        }
        else if (controller == OVRInput.Controller.LTouch)
        {
            if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger, controller))
            {
                Debug.Log(controller + " Hand Trigger pressed");
            }
        }

        // Optional: Check for Start and Back buttons if your controller supports them.
        if (OVRInput.GetDown(OVRInput.Button.Start, controller))
        {
            Debug.Log(controller + " Start button pressed");
        }
        if (OVRInput.GetDown(OVRInput.Button.Back, controller))
        {
            Debug.Log(controller + " Back button pressed");
        }
    }
}
