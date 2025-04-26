using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This script is attached to an individual virtual keyboard key in the virtual keyboard.
public class KeyInput : MonoBehaviour
{

    [SerializeField] private VRButtonCollision button;
    [SerializeField] private KeyboardInputReceiver inputReceiver;
    // Character that the key represents
    [SerializeField] string character;
    [SerializeField] TMP_Text text;
    // Bools to determine the special function of the key
    [SerializeField] bool isCaps = false;
    [SerializeField] bool isBack = false;
    [SerializeField] bool isClear = false;


    // Adds a cooldown to the key press to prevent multiple inputs from a single press
    private float cooldown = 1.0f;
    private float lastPressTime = 0.0f;


    void Start()
    {
        lastPressTime = Time.time;   
    }

    void Update()
    {
        // Adjust key based on the caps state of the keyboard
        if (inputReceiver.getCaps()){
            character = character.ToUpper();
            text.text = character;
        }
        else{
            character = character.ToLower();
            text.text = character;
        }

        if (Time.time - lastPressTime < cooldown) return;

        // Incur key press action if cooldown has passed and button is pressed
        if(button.isOverlappingButton()){
            if(isBack) inputReceiver.OnBackspace();
            else if(isCaps) inputReceiver.onCaps();
            else if(isClear) inputReceiver.onClear();
            else inputReceiver.onKeyPressed(character);

            lastPressTime = Time.time;
        }
    }
}
