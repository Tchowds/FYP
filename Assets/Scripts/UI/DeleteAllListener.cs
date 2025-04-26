using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// On click listener hook for the delete all button in pose and gesture management
public class DeleteAllListener : MonoBehaviour
{

    [SerializeField] private Button deleteAllButton;
    [SerializeField] private bool isPoseSelection = true; // true for pose, false for gesture

    void Awake()
    {
        // Depending on the selection, tie the button to delete all poses or gestures
        if (isPoseSelection) deleteAllButton.onClick.AddListener(() => DataInterface.resetPoses());
        else deleteAllButton.onClick.AddListener(() => DataInterface.resetGestures());
    }

    void OnDestroy() {
        if (isPoseSelection) deleteAllButton.onClick.RemoveListener(() => DataInterface.resetPoses());
        else deleteAllButton.onClick.RemoveListener(() => DataInterface.resetGestures());
    }
}
