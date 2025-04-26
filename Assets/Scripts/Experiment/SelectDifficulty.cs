using UnityEngine;
using UnityEngine.UI;

// UI script to select the difficulty level of the game
public class SelectDifficulty : MonoBehaviour
{
    // Three buttons for selecting difficulty levels
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;

    // Game process and controller references to set the difficulty level
    [SerializeField] private GameProcess gameProcess;
    [SerializeField] private GameProcessController gameProcessController;

    void Start()
    {
        easyButton.onClick.AddListener(() => setDifficulty("Easy"));
        mediumButton.onClick.AddListener(() => setDifficulty("Medium"));
        hardButton.onClick.AddListener(() => setDifficulty("Hard"));
    }

    void setDifficulty(string difficulty)
    {
        // Depending on which modality mode is active, set the difficulty level in the game process or controller
        if(gameProcess){
           gameProcess.setLevel(difficulty);
           gameProcess.StartGame();
        } 
        if(gameProcessController){
            gameProcessController.setLevel(difficulty);
            gameProcessController.StartGame();
        }
    }



}