# README

This repository contains all the code and assets for the Final Year Project for UCL MEng Computer Science Candidate KVWP1 for the academic year of 2024-2025.

## Installation

Follow these steps to set up the project:

1. **Install Unity Editor:**  
   Open Unity Hub and install the following version of the Unity Editor:

   `2022.3.17f1`

2. **Clone the Repository:**  
   In your workspace, run:

   `git clone hhttps://github.com/Tchowds/FYP.git`

3. **Add the Project:**  
   Open Unity Hub, click on `Add` → `Add Project From Disk`, and select the cloned repository.

4. **Configure for Meta Quest:**  
   Open the project in Unity Editor and set all the necessary settings for building on Meta Quest devices. Refer to the project documentation for detailed steps on configuring these settings. Ensure that experimental features and hand tracking are enabled on both the device and on the Unity Editor

## Main Components

The Unity scenes for the project can be found in the `Scenes` directory. The `prefabs` directory contains the main component prefabs as described in the report. The main code and assets for the project can be found in the `Assets/Scripts` folder. Here is the breakdown of each of the directories

`./Experiment/` - Contains all the scripts that were made for the user study and Experiment. In this parent directory, scripts that are used to make the experiment environment can be found. A sub-directory called `ControllerVersionScripts` contain scripts that duplicate functionality of other scripts found in the project but adapted for controller use instead of hand tracking. This also contains a subdirectory named `results_analysis` which contains python scripts that analysed the results from the experiment.

`./GesturesAndPoses/` - Contains all the scripts that handle pose and gesture operations which make up the bulk of the project. The parent directory contains scripts that are used for both recording and detecting poses/gestures as wel as data handling. The `Recording` subdirectory contains scripts to handle pose and gesture recording specifically. Similarly, the `Detection` subdirectory contains scripts to handle the pose and gesture detection.

`./RuntimeMl/` - Contains the currently unused scripts that were originally used to perform ML training onboard the headset, which was an approach dropped partway through the project. The code is still there but commented as the library used has been omitted from the repository. To find the library for this see the following post: [Ludic Worlds TorchSharp post](https://www.patreon.com/posts/torchsharp-for-106911262)

`./UI/` - Contains numerous scripts that were used to make up and help UI element control flow, including handling listeners for buttons and logic for menu control flow.

`./ModelTraining/` - Contains the Python scripts that used PyTorch to train and generate the pose and gesture detection models. Includes the JSon data file that was used to train the models used in the experiment.

### Scenes

`MenuTests` - The main scene where you can try and use the All-in-One Pose and Gesture Recogniser and Detector Component.

`Experiment` - The scene for the experiment environment using hand tracking

`ExperimentControllers` - The scene for the experiment environment using controllers

## Other Assets and Package

Refer to the report for Packages used throughout this project.
All experiment icons were found and used on an open-source site and the only other Package was `SkyboxSeries Freebie` to find the skybox for the experiment environment. The asset can be found at [this link](https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-series-free-103633)
