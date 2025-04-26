using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Main script used to manage the virtual keyboard input in the UI
public class KeyboardInputReceiver : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Recorder recorder;
    // Userd to track if the caps lock is on or off to change the appearance and functionality of the keys
    private bool isCaps = false;

    // Keys call this to add the character to the input field
    public void onKeyPressed(string character){
        inputField.text += character;
        inputField.caretPosition = inputField.text.Length;
    }

    // Special key functions for the keyboard
    // Backspace, Caps and Clear keys are handled here
    public void OnBackspace()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            inputField.caretPosition = inputField.text.Length;
        }
    }

    public void onCaps(){
        isCaps = !isCaps;
    }

    public bool getCaps(){
        return isCaps;
    }

    public void onClear(){
        inputField.text = "";
        inputField.caretPosition = inputField.text.Length;
    }

    // When disabled, this means the user is done with the input
    // The input field is cleared and the user input is sent to the pose/gesture recorder
    private void OnDisable()
    {
        if (recorder != null){
            recorder.SetNextToRecordName(inputField.text);
            recorder.setToNEW();
        }

        inputField.text = "";
        inputField.caretPosition = inputField.text.Length;
    }
    

}
