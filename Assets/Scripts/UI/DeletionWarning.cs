using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Script attached to the deletion warning menu to manage the warning text and delete button action
public class DeletionWarning : MonoBehaviour
{
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button deleteButton;
    [SerializeField] private bool isPoseSelection = true; // true for pose, false for gesture
    private string name;



    void Awake()
    {
        // Depending on the pose selection and mode, tie the button to delete the pose or gesture
        deleteButton.onClick.AddListener(() => {
            if (isPoseSelection) DataInterface.deletePose(name);
            else DataInterface.deleteGesture(name);
            Debug.Log("Deleted Pose/Gesture: " + name);
        });
    }

    // Used by other scripts to set the name of the pose/gesture to be deleted
    public void setWarningText(string name)
    {
        this.name = name;
        warningText.text = "WARNING: This deletion action is permanent and cannot be undone. \nThe following pose/gesture will be deleted:"+ name;
    }


}
