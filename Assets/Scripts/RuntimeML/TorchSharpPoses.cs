// Uncomment if TorchSarp is imported

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// using TorchSharp;
// using static TorchSharp.torch;
// using static TorchSharp.torch.nn;
// using static TorchSharp.torch.nn.functional;

// // Pose model architecture using TorchSharp
// public class Net : Module<Tensor, Tensor>
// {
//     private Module<Tensor, Tensor> features;

//     public Net(string name, int numClasses, Device device = null) : base(name)
//     {
//         features = Sequential(
//             ("l1", Linear(22,32)),
//             ("r1", ReLU(inplace: true)),
//             ("l2", Linear(32,32)),
//             ("r2", ReLU(inplace: true)),
//             ("l3", Linear(32,numClasses))
//         );

//         RegisterComponents();

//         this.to(TorchSharp.DeviceType.CPU);
//     }

//     public override Tensor forward(Tensor input)
//     {
//         return features.forward(input);
//     }
// }

// // Wrapper for TorchSharp model training and prediction
// public static class TorchSharpPoses
// {
//     private static Net net;
//     public static int currentEpoch = 0;
    
//     // Train the model using TorchSharp
//     public static void train(float[][] inputsArr, float[] labelsArr)
//     {
//         try
//         {
//             // Calculates the number of classes based on the labels
//             int numClasses = 1;
//             for (int i = 0; i < labelsArr.Length; i++)
//             {
//                 if (labelsArr[i] > numClasses) numClasses = (int)labelsArr[i];
//             }
//             numClasses++;

//             net = new Net("net", numClasses);
//             var optimizer = torch.optim.SGD(net.parameters(), 0.01f);
//             var loss = torch.nn.CrossEntropyLoss();

//             // Convert the inputs and labels to TorchSharp tensors
//             var inputsMat = new float[inputsArr.Length, inputsArr[0].Length];
//             for (int i = 0; i < inputsArr.Length; i++)
//             {
//                 for (int j = 0; j < inputsArr[0].Length; j++)
//                 {
//                     inputsMat[i, j] = inputsArr[i][j];
//                 }
//             }

//             var inputs = torch.tensor(inputsMat, dtype: ScalarType.Float32);
//             var labels = torch.tensor(labelsArr).to_type(ScalarType.Int64);

//             // Training loop
//             for (int epoch = 0; epoch < 10; epoch++)
//             {
//                 currentEpoch = epoch;
//                 optimizer.zero_grad();
//                 var lossValue = loss.forward(net.forward(inputs), labels);
//                 lossValue.backward();
//                 optimizer.step();
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("error: " + ex);
//         }
//     }

//     // Inference method to predict the class of a given input
//     public static float[] predict(float[] inputsArr)
//     {
//         var inputs = torch.tensor(inputsArr);
//         var outputs = net.forward(inputs);
//         var probabilities = torch.nn.functional.softmax(outputs, 0);
//         return probabilities.data<float>().ToArray();
//     }

// }
