using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


// Matches data structure of the JSON file
public class PoseGestures
{
    public Dictionary<string, PoseGestureData> poses;
    public Dictionary<string, PoseGestureData> gestures;
}

public class PoseGestureData
{
    public int poseGestureIndex;
    public float[][] poseGestureData;
}




// Main Interface for pose and gesture data management
public static class DataInterface
{
    static string fileName;
    static string filePath;
    static PoseGestures poseGestures;

    static DataInterface()
    {
        fileName = "PoseGestureData.json";
        filePath = Application.persistentDataPath + "/" + fileName;

        // Load the pose and gesture data from the JSON file when the class is first accessed
        poseGestures = LoadPoseGestureData();
    }

    // Loads the pose and gesture data from the JSON file
    static PoseGestures LoadPoseGestureData()
    {
        if (!System.IO.File.Exists(filePath))
        {
            return new PoseGestures
            {
                poses = new Dictionary<string, PoseGestureData>(),
                gestures = new Dictionary<string, PoseGestureData>()
            };
        }

        string dataAsJson = System.IO.File.ReadAllText(filePath);
        try
        {
            return JsonConvert.DeserializeObject<PoseGestures>(dataAsJson);
        }
        catch
        {
            return new PoseGestures
            {
                poses = new Dictionary<string, PoseGestureData>(),
                gestures = new Dictionary<string, PoseGestureData>()
            };
        }
    }

    // Saves the pose and gesture data to the JSON file
    public static void savePoseGestureData()
    {
        string jsonData = JsonConvert.SerializeObject(poseGestures, Formatting.Indented);
        System.IO.File.WriteAllText(filePath, jsonData);
    }

    // Fully resets the pose and gesture data
    public static void reset()
    {
        poseGestures = new PoseGestures
        {
            poses = new Dictionary<string, PoseGestureData>(),
            gestures = new Dictionary<string, PoseGestureData>()
        };
        savePoseGestureData();
    }

    // Resets the pose or gesture data, depending on the method called
    public static void resetPoses()
    {
        poseGestures.poses = new Dictionary<string, PoseGestureData>();
        savePoseGestureData();
    }

    public static void resetGestures()
    {
        poseGestures.gestures = new Dictionary<string, PoseGestureData>();
        savePoseGestureData();
    }


    public static void addNewPoseData(string poseName, float[][] poseData)
    {
        // If pose already exists, enhance it instead of adding a new one
        if (poseGestures.poses.ContainsKey(poseName))
        {
            enhancePose(poseName, poseData);
            return;
        }

        // find the lowest index that hasn't been used yet
        int lowestIndex = 0;
        foreach (var poseElems in poseGestures.poses.Values)
        {
            if(poseElems.poseGestureIndex == lowestIndex) lowestIndex++;
        }
        PoseGestureData newPoseData = new PoseGestureData
        {
            poseGestureIndex = lowestIndex,
            poseGestureData = poseData
        };
        poseGestures.poses.Add(poseName, newPoseData);
        savePoseGestureData();
    }

    public static void addNewGestureData(string gestureName, float[][] gestureData)
    {
        // If gesture already exists, enhance it instead of adding a new one
        if (poseGestures.gestures.ContainsKey(gestureName))
        {
            enhanceGesture(gestureName, gestureData);
            return;
        }

        int lowestIndex = 0;
        foreach (var gestureElems in poseGestures.gestures.Values)
        {
            if (gestureElems.poseGestureIndex == lowestIndex) lowestIndex++;
        }
        PoseGestureData newGestureData = new PoseGestureData
        {
            poseGestureIndex = lowestIndex,
            poseGestureData = gestureData
        };
        poseGestures.gestures.Add(gestureName, newGestureData);
        savePoseGestureData();
    }

    // Replaces pose/gesture data with new data
    public static void retrainPose(string poseName, float[][] poseData)
    {
        poseGestures.poses[poseName].poseGestureData = poseData;
        savePoseGestureData();
    }

    public static void retrainGesture(string gestureName, float[][] gestureData)
    {
        poseGestures.gestures[gestureName].poseGestureData = gestureData;
        savePoseGestureData();
    }

    // Enhances existing pose/gesture data with new data
    public static void enhancePose(string poseName, float[][] poseData)
    {
        List<float[]> dataList = new List<float[]>(poseGestures.poses[poseName].poseGestureData);
        dataList.AddRange(poseData);
        poseGestures.poses[poseName].poseGestureData = dataList.ToArray();
        savePoseGestureData();
    }

    public static void enhanceGesture(string gestureName, float[][] gestureData)
    {
        List<float[]> dataList = new List<float[]>(poseGestures.gestures[gestureName].poseGestureData);
        dataList.AddRange(gestureData);
        poseGestures.gestures[gestureName].poseGestureData = dataList.ToArray();
        savePoseGestureData();
    }

    // Maps index to pose/gesture name
    public static string whichPose(int index)
    {
        foreach (var pair in poseGestures.poses)
        {
            if (pair.Value.poseGestureIndex == index)
                return pair.Key;
        }
        return "ERROR";
    }

    public static string whichGesture(int index)
    {
        foreach (var pair in poseGestures.gestures)
        {
            if (pair.Value.poseGestureIndex == index)
                return pair.Key;
        }
        return "ERROR";
    }

    // Returns all the currently stored poses and gestures
    public static List<string> getAllPoseNames()
    {
        return new List<string>(poseGestures.poses.Keys);
    }

    public static List<string> getAllGestureNames()
    {
        return new List<string>(poseGestures.gestures.Keys);
    }

    public static void deletePose(string poseName){
        if (poseGestures.poses.ContainsKey(poseName))
        {
            int removedIndex = poseGestures.poses[poseName].poseGestureIndex;

            // Find the pose with the largest index (including the one to remove)
            PoseGestureData poseWithMaxIndex = null;
            int maxIndex = -1;
            foreach (var pose in poseGestures.poses.Values)
            {
                if (pose.poseGestureIndex > maxIndex)
                {
                    maxIndex = pose.poseGestureIndex;
                    poseWithMaxIndex = pose;
                }
            }

            // If the removed pose is not the one with the highest index, reassign the index
            if (removedIndex != maxIndex)
            {
                poseWithMaxIndex.poseGestureIndex = removedIndex;
            }

            poseGestures.poses.Remove(poseName);
            savePoseGestureData();
        }
    }

    public static void deleteGesture(string gestureName)
    {
        if (poseGestures.gestures.ContainsKey(gestureName))
        {
            int removedIndex = poseGestures.gestures[gestureName].poseGestureIndex;

            // Find the gesture with the largest index (including the one to remove)
            PoseGestureData gestureWithMaxIndex = null;
            int maxIndex = -1;
            foreach (var gesture in poseGestures.gestures.Values)
            {
                if (gesture.poseGestureIndex > maxIndex)
                {
                    maxIndex = gesture.poseGestureIndex;
                    gestureWithMaxIndex = gesture;
                }
            }

            // If the removed gesture is not the one with the highest index, reassign the index
            if (removedIndex != maxIndex)
            {
                gestureWithMaxIndex.poseGestureIndex = removedIndex;
            }

            poseGestures.gestures.Remove(gestureName);
            savePoseGestureData();
        }
    }

    public static bool moreThanOnePose()
    {
        return poseGestures.poses.Count > 1;
    }
    public static bool moreThanOneGesture()
    {
        return poseGestures.gestures.Count > 1;
    }

    // Specific mapping function used exlusively for onboard training
    public static (float[][], float[]) convertToTrainingData()
    {
        List<float[]> trainingData = new List<float[]>();
        List<float> labels = new List<float>();

        foreach (var pose in poseGestures.poses.Values)
        {
            if (pose.poseGestureData != null)
            {
                foreach (var sample in pose.poseGestureData)
                {
                    trainingData.Add(sample);
                    labels.Add(pose.poseGestureIndex);
                }
            }
        }

        return (trainingData.ToArray(), labels.ToArray());
    }
}
