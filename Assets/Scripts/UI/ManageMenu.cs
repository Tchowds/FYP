using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// This script is attached to the manage pose/gesture menu to handle the numerous actions
// The main actions are delete, retrain and continue training poses/gestures
public class ManageMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text poseNameText;

    // This object handles the choosing of the pose/gesture to manage
    [SerializeField] private SelectionChooser selectionChooser;

    // The recorder is needed to set the pose/gesture name to be retrained or enhanced
    [SerializeField] private Recorder recorder;

    // Buttons for the delete, retrain and enhance actions
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button retrainButton;
    [SerializeField] private Button enhanceButton;

    // In the event of a deletion, the warning screen needs to be updated based on the pose/gesture name
    [SerializeField] private DeletionWarning deletionWarning;

    private string poseName;

    void OnEnable()
    {
        poseName = selectionChooser.getSelection();
        poseNameText.text = "Manage Pose/Gesture: " + poseName;

        // Adds listeners to each button to perform their respective actions when clicked
        if(deleteButton != null){
            deleteButton.onClick.AddListener(() =>{
                deletionWarning.setWarningText(poseName);
            });
        }
        if (retrainButton != null){
            retrainButton.onClick.AddListener(() => {
                recorder.SetNextToRecordName(poseName);
                recorder.setToRETRAIN();
            });            
        }
        if (enhanceButton != null){
            enhanceButton.onClick.AddListener(() => {
                recorder.SetNextToRecordName(poseName);
                recorder.setToENHANCE();
            });
        }
    }


}
