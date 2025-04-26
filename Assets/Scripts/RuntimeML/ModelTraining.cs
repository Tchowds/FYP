// Uncomment if TorchSharp is imported

// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using TMPro;

// public class ModelTraining : MonoBehaviour
// {
//     // Menu pages to disable and enable after training
//     [SerializeField] private GameObject toDisable;
//     [SerializeField] private GameObject toEnable;
//     // Text to say what epoch of training we are on
//     [SerializeField] private TMP_Text trainingText;

//     private bool trainingCompleted = false;

//     void OnEnable()
//     {
//         if(DataInterface.moreThanOnePose()){
//             var (trainingInputs, trainingOutputs) = DataInterface.convertToTrainingData();
//             StartCoroutine(TrainCoroutine(trainingInputs, trainingOutputs));
//         } else
//         {
//             toDisable.SetActive(false);
//             toEnable.SetActive(true);
//         }

//     }

//     private IEnumerator TrainCoroutine(float[][] inputsArr, float[] labelsArr)
//     {
//         // Run training in background
//         Task.Run(() =>
//         {
//             TorchSharpPoses.train(inputsArr, labelsArr);
//             trainingCompleted = true;
//         });

//         // Wait until the training is completed.
//         yield return new WaitUntil(() => trainingCompleted);

//         // Training is completed, update the UI
//         toDisable.SetActive(false);
//         toEnable.SetActive(true);
//     }

//     void update()
//     {
//         trainingText.text = "Training Models. Loading..." + TorchSharpPoses.currentEpoch.ToString();
//     }
// }
