using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Barracuda;

// Wrapper class for the ML model to run inference on input data
public class MLClassifier : MonoBehaviour
{
    [SerializeField] private NNModel modelAsset;

    // The input dimension of the model for checking the input size
    // 22 for pose, 90 for gesture
    [SerializeField] private int inputDimension;

    // Barracuda worker to run the model
    private IWorker worker;
    private Tensor inputTensor;

    // Load the model and create a worker when the script is enabled
    void Start()
    {
        Model model = ModelLoader.Load(modelAsset);
        worker = model.CreateWorker();
        inputTensor = new Tensor(1, inputDimension);
    }

    // Method to run inference with a float array input
    public float[] RunInference(float[] inputArray)
    {
        // Ensure the input array size matches the expected input size
        if (inputArray.Length != inputDimension)
        {
            Debug.LogError("Input array size does not match the expected, actual size: " + inputArray.Length);
            return null;
        }

        inputTensor = new Tensor(1, inputArray.Length, inputArray);

        // Run inference with the input tensor
        worker.Execute(inputTensor);

        // Get the output tensor
        Tensor outputTensor = worker.PeekOutput();

        float[] outputArray = new float[outputTensor.length];
        for (int i = 0; i < outputTensor.length; i++)
        {
            outputArray[i] = outputTensor[0, i];
        }

        // Dispose of the tensors after use
        outputTensor.Dispose();
        inputTensor.Dispose();

        return outputArray;
    }

    // Clean up
    private void OnDestroy()
    {
        if (worker != null)
        {
            worker.Dispose();  // Dispose the worker when no longer needed
        }
        if (inputTensor != null)
        {
            inputTensor.Dispose();  // Dispose the input tensor when no longer needed
        }
    }
}
