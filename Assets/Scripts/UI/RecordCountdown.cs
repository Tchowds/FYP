using UnityEngine;
using TMPro;
using System.Collections;

// Script to manage the countdown before starting the recording
public class RecordCountdown : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TMP_Text countdownText;

    // When the countdown is complete, the current object will be disabled and the next object will be enabled
    [Header("GameObjects to Toggle")]
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;

    // After the countdown, the recorder will be started
    [Header("Recording Script to enable")]
    [SerializeField] private Recorder recorder;

    [Header("Countdown Settings")]
    [SerializeField] private int startValue = 5;



    private void OnEnable()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        int current = startValue;

        // Update the countdown counter text every second
        while (current >= 0)
        {
            countdownText.text = current.ToString();
            yield return new WaitForSeconds(1f);
            current--;
        }


        recorder.StartRecording();
        countdownText.text = "Recording...";
        countdownText.fontSize = 48;
        StartCoroutine(RecordingRoutine());
    }

    // When the recording is finished, move onto the next screen
    private IEnumerator RecordingRoutine()
    {

        while (recorder.enabled)
        {
            yield return null;
        }

        Debug.Log("[RecordCountdown] Recording stopped...");
        if (objectToHide != null) objectToHide.SetActive(false);
        if (objectToShow != null) objectToShow.SetActive(true);
    }
}
