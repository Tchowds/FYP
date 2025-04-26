using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Attached to the selection chooser in the pose/gesture management menu
// Allows users to choose a specific pose or gesture that is stored to manage
public class SelectionChooser : MonoBehaviour
{
    // Button objects to navigate the menu
    [SerializeField] private Button navigateLeft;
    [SerializeField] private Button navigateRight;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text selectionText;

    // Selection for pose or gesture behaviour
    [SerializeField] private bool isPoseSelection = true; // true for pose, false for gesture

    private string selection;
    
    // List of all available poses or gestures from the DataInterface
    private List<string> posesGestures;

    // Index used to loop through the poses/gestures
    private int currentPoseIndex;

    void OnEnable()
    {
        selection = "Selection: ";
        // Loads poses/gestures from the DataInterface
        if (isPoseSelection) posesGestures = DataInterface.getAllPoseNames();
        else posesGestures = DataInterface.getAllGestureNames();

        // Disable buttons if no poses are found
        if (posesGestures.Count == 0)
        {
            selectionText.text = "No Poses/Gestures Found: Next Button will not work";
            navigateLeft.interactable = false;
            navigateRight.interactable = false;
            nextButton.interactable = false;
            return;
        }

        currentPoseIndex = 0;

        selectionText.text = selection + posesGestures[currentPoseIndex];

        // Add button click listeners (added once in Start)
        navigateRight.onClick.AddListener(NavigateRight);
        navigateLeft.onClick.AddListener(NavigateLeft);
    }

    // Navigate to the next pose
    private void NavigateRight()
    {
        currentPoseIndex = (currentPoseIndex + 1) % posesGestures.Count;
        selectionText.text = selection + posesGestures[currentPoseIndex];
    }

    // Navigate to the previous pose
    private void NavigateLeft()
    {
        currentPoseIndex = (currentPoseIndex - 1 + posesGestures.Count) % posesGestures.Count;
        selectionText.text = selection + posesGestures[currentPoseIndex];
    }

    // Used by other scripts to get the current pose/gesture name
    public string getSelection()
    {
        return posesGestures[currentPoseIndex];
    }
}
