using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VRButtonCollisionController : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform leftHandTransform;

    private float fingertipCollisionRadius = 0.001f;
    [SerializeField] private LayerMask buttonLayerMask;
    [SerializeField] private Collider specificCollider;
    
    private bool isOverlapping = false;
    private float startTime;

    void OnEnable()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // Allow some time after enabling before checking for collisions
        if (Time.time - startTime < 2f)
            return;

        // Ensure at least one hand transform, the button, and the collider are assigned
        if ((rightHandTransform == null && leftHandTransform == null) || button == null || specificCollider == null)
            return;

        bool collisionFound = false;

        // Check collision for the right hand if assigned
        if (rightHandTransform != null)
        {
            Vector3 rightFingertipPos = rightHandTransform.position;
            Collider[] rightHits = Physics.OverlapSphere(rightFingertipPos, fingertipCollisionRadius, buttonLayerMask);
            if (rightHits.Contains(specificCollider))
            {
                collisionFound = true;
            }
        }

        // Check collision for the left hand if assigned
        if (leftHandTransform != null)
        {
            Vector3 leftFingertipPos = leftHandTransform.position;
            Collider[] leftHits = Physics.OverlapSphere(leftFingertipPos, fingertipCollisionRadius, buttonLayerMask);
            if (leftHits.Contains(specificCollider))
            {
                collisionFound = true;
            }
        }

        if (collisionFound)
        {
            if (!isOverlapping)
            {
                button.onClick?.Invoke();
                isOverlapping = true;
            }
        }
        else
        {
            isOverlapping = false;
        }
    }

    public bool IsOverlappingButton()
    {
        return isOverlapping;
    }
}
