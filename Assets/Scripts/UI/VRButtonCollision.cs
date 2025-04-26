using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

// Script attached to all buttons in the floating menu
// This detects a button press by comparing the index fingertip position to the button collider
public class VRButtonCollision : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button button;

    // Hand objects to track to detect fingertip position
    [SerializeField] public OVRSkeleton rightHand;
    [SerializeField] public OVRSkeleton leftHand;

    // The button collider to check for fingertip collision
    [SerializeField] public Collider specificCollider;

    [SerializeField] private LayerMask buttonLayerMask;

    // The radius of the fingertip collision sphere
    private float fingertipCollisionRadius = 0.0001f;

    // This is used to determine if the button is currently being pressed to prevent multiple presses
    private bool isOverlapping = false;

    private float startTime;

    void OnEnable()
    {
        startTime = Time.time;
    }



    void Update()
    {
        // 2 second buffer to prevent immediate button press detection
        if (Time.time - startTime < 2f) return;

        if ( (rightHand == null && leftHand == null) || button == null || specificCollider == null) return;

        // Get the transform of the index fingertip from the skeleton
        var rightIndexTip = rightHand.Bones
            .FirstOrDefault(b => b.Id == OVRSkeleton.BoneId.Hand_IndexTip);
        var leftIndexTip = leftHand.Bones
            .FirstOrDefault(b => b.Id == OVRSkeleton.BoneId.Hand_IndexTip);

        if (rightIndexTip == null || leftIndexTip == null) return;  // If not found or skeleton not ready

        // Get the position of the index fingertip in world space
        Vector3 rightFingertipPos = rightIndexTip.Transform.position;
        Vector3 leftFingertipPos = leftIndexTip.Transform.position;

        // Check if the fingertip is overlapping with the button collider
        Collider[] hits = Physics.OverlapSphere(rightFingertipPos, fingertipCollisionRadius, buttonLayerMask);
        Collider[] leftHits = Physics.OverlapSphere(leftFingertipPos, fingertipCollisionRadius, buttonLayerMask);
        hits = hits.Concat(leftHits).ToArray();
        
        if (hits.Contains(specificCollider))
        {
            // If not already overlapping, fire the button onClick event
            if (!isOverlapping)
            {
                button.onClick?.Invoke();
                isOverlapping = true;
            }
        }
        else
        {
            // No overlap with the specific collider right now
            isOverlapping = false;
        }
    }

    // Used by keyboard keys to check if the button is being pressed
    public bool isOverlappingButton()
    {
        return isOverlapping;
    }

}
