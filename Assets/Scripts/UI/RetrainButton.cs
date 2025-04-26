using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Attached to the retrain button in the pose/gesture manage menu
// Affords functionality to set the recorder to retrain, enhance or create a new pose/gesture
// Should particularly be used in retrain mode
public class RetrainButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RecordingMode recordingMode;
    [SerializeField] private Recorder recorder;
    [SerializeField] private Button button;
    void Start()
    {
        button.onClick.AddListener(() => {
            if (recordingMode == RecordingMode.RETRAIN) recorder.setToRETRAIN();
            else if (recordingMode == RecordingMode.ENHANCE) recorder.setToENHANCE();
            else recorder.setToNEW();
        });
    }
}
