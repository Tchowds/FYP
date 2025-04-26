using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// Constantly tracks the hand trajectory providing the trajectory buffer to the gesture recorder and detector
public class HandBuffer : MonoBehaviour
{
    [SerializeField] private OVRSkeleton skeleton;
    [SerializeField] private OVRHand hand;
    [SerializeField] private float bufferTimeLength = 3.0f;
    [SerializeField] private int framesPerSecond = 10;
    
    // Full tracking of all bones or just the hand start bone
    // Used only for recording and not for gesture detection
    [SerializeField] private bool fullTracking = true;

    // Deadzone distance for trajectory
    [SerializeField] private float deadzone = 0.1f;


    // Keeps track of all the buffers
    private List<Queue<Vector3>> bufferList;

    // Timing value to determine when the hand was last untracked to determine buffer validity
    private float lastUntrackedTime = 0.0f;
    //Timing value to determine when next to add to the buffer
    private float lastUpdateTime = 0.0f;


    void Start()
    {
        // Initiates and populates the buffer
        bufferList = new List<Queue<Vector3>>();
        int count = fullTracking ? 19 : 1;
        for (int i = 0; i < count; i++) bufferList.Add(new Queue<Vector3>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdateTime >= (1.0f / framesPerSecond))
        {
            if (fullTracking) updateAllBuffers();
            else updateBuffer(0);
            lastUpdateTime = Time.time;
        }
    }

    // Gets all the positions of the bones in the hand and adds them to the buffer
    private void updateAllBuffers()
    {
        if (hand.IsTracked)
        {
            bufferList[0].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform.position);
            bufferList[1].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Start].Transform.position);
            bufferList[2].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb0].Transform.position);
            bufferList[3].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb1].Transform.position);
            bufferList[4].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb2].Transform.position);
            bufferList[5].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb3].Transform.position);
            bufferList[6].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position);
            bufferList[7].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index2].Transform.position);
            bufferList[8].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index3].Transform.position);
            bufferList[9].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.position);
            bufferList[10].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle2].Transform.position);
            bufferList[11].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle3].Transform.position);
            bufferList[12].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring1].Transform.position);
            bufferList[13].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring2].Transform.position);
            bufferList[14].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring3].Transform.position);
            bufferList[15].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky0].Transform.position);
            bufferList[16].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky1].Transform.position);
            bufferList[17].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky2].Transform.position);
            bufferList[18].Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky3].Transform.position);
        }
        // If the hand is not tracked, add a zero vector to the buffer to maintain the buffer length
        else
        {
            foreach (Queue<Vector3> boneBuffer in bufferList)
            {
                boneBuffer.Enqueue(new Vector3(0, 0, 0));
            }
            // Since the hand was not tracked, the buffer is no longer valid for a set time
            lastUntrackedTime = Time.time;
        }

        // If buffer is overfull, remove the oldest element
        while (bufferList[0].Count > bufferTimeLength * framesPerSecond)
        {
            foreach (Queue<Vector3> boneBuffer in bufferList)
            {
                boneBuffer.Dequeue();
            }
        }
    }

    // Update the singular buffer for the hand start bone
    private void updateBuffer(int index)
    {
        Queue<Vector3> currentBuffer = bufferList[index];
        if (hand.IsTracked) currentBuffer.Enqueue(skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Start].Transform.position);
        // If the hand is not tracked, add a zero vector to the buffer to maintain the buffer length
        else
        {
            currentBuffer.Enqueue(new Vector3(0, 0, 0));
            // Since the hand was not tracked, the buffer is no longer valid for a set time
            lastUntrackedTime = Time.time;
        }

        // If buffer is overfull, remove the oldest element
        while (currentBuffer.Count > bufferTimeLength * framesPerSecond)
        {
            currentBuffer.Dequeue();
        }
    }
    
    // Returns all the buffers in the buffer list
    public Vector3[][] getAllBuffers()
    {
        if (Time.time - lastUntrackedTime < bufferTimeLength) return null;
        else
        {
            Vector3[][] buffers = new Vector3[bufferList.Count][];
            for (int i = 0; i < bufferList.Count; i++)
            {
                buffers[i] = bufferList[i].ToArray();
            }
            return buffers;
        }
    }

    // Returns the singular buffer for the hand start bone
    public Vector3[] getBuffer()
    {
        if (Time.time - lastUntrackedTime < bufferTimeLength) return null;
        else return bufferList[1].ToArray();
    }

    // Passes buffer through a deadzone check to determine if the trajectory is valid
    public Vector3[] getBufferWithDeadzone()
    {
        if (Time.time - lastUntrackedTime < bufferTimeLength) return null;

        Vector3[] buffer = bufferList[1].ToArray();
        return withinDeadzone(buffer) ? buffer : null;
    }

    // Deadzone distance check
    public bool withinDeadzone(Vector3[] buffer)
    {
        float totalDistance = 0f;
        for (int i = 1; i < buffer.Length; i++)
        {
            totalDistance += Vector3.Distance(buffer[i - 1], buffer[i]);
        }
        return totalDistance >= deadzone;
    }

    public float getBufferTimeLength()
    {
        return bufferTimeLength;
    }

    // The following functions implement the dynamic time warping algorithm to interpolate the trajectory buffer

    // Given a subset length of a buffer, takes an appropriately sized subset from the end of the buffer
    // and interpolates it to the full buffer length
    public Vector3[] interpolateBuffer(float time)
    {
        // Get the full trajectory buffer.
        Vector3[] buffer = getBufferWithDeadzone();
        if (buffer == null) return null;
        
        int totalFrames = buffer.Length;

        // Determine the number of frames corresponding to 'time' seconds.
        int subFrames = Mathf.RoundToInt(time * framesPerSecond);
        subFrames = Mathf.Clamp(subFrames, 1, totalFrames);

        // Extract the last 'subFrames' frames from the full buffer.
        Vector3[] subBuffer = new Vector3[subFrames];
        for (int i = 0; i < subFrames; i++)
        {
            subBuffer[i] = buffer[totalFrames - subFrames + i];
        }

        // Create an output array with the same number of elements as the full buffer.
        Vector3[] resultBuffer = new Vector3[totalFrames];

        // Edge case: If there's only one frame, there's nothing to interpolate.
        if (totalFrames == 1)
        {
            resultBuffer[0] = subBuffer[0];
            return resultBuffer;
        }

        // Interpolate the subBuffer to 'totalFrames' elements.
        // We map an index i in the output (normalized to 0...1) to a position in subBuffer.
        for (int i = 0; i < totalFrames; i++)
        {
            float t = (float)i / (totalFrames - 1);
            float subIndex = t * (subFrames - 1);
            int index0 = Mathf.FloorToInt(subIndex);
            int index1 = Mathf.Min(index0 + 1, subFrames - 1);
            float lerpFactor = subIndex - index0;

            resultBuffer[i] = Vector3.Lerp(subBuffer[index0], subBuffer[index1], lerpFactor);
        }

        // Check if the interpolated buffer is within the deadzone.
        return withinDeadzone(resultBuffer) ? resultBuffer : null;
    }

    // Returns all the subsets of the buffer interpolated to the full buffer length
    // The time parameter is the interval between each subset
    public Vector3[][] getInterpolatedBuffers(float time, float minInterval, IntervalStrategy strategy)
    {
        if (time <= 0f) return null;
        
        List<Vector3[]> bufferList = new List<Vector3[]>();
        
        // Loop from time to bufferTimeLength in increments of time


        // The strategy determines the order of the buffer list (LARGEST or SMALLEST)
        if (strategy == IntervalStrategy.LARGEST)
        {
            bufferList.Add(getBufferWithDeadzone());
            for (float currentTime = bufferTimeLength - time; currentTime >= (time > minInterval ? time : minInterval) ; currentTime -= time)
            {
                Vector3[] interpolatedBuffer = interpolateBuffer(currentTime);
                if (interpolatedBuffer != null) bufferList.Add(interpolatedBuffer);
                else return null;
            }            
        }
        else
        {
            for (float currentTime = (time > minInterval ? time : minInterval); currentTime <= bufferTimeLength; currentTime += time)
            {
                Vector3[] interpolatedBuffer = interpolateBuffer(currentTime);
                if (interpolatedBuffer != null) bufferList.Add(interpolatedBuffer);
                else return null;
            }
            bufferList.Add(getBufferWithDeadzone());
        }

        
        // Convert list to array and return
        return bufferList.ToArray();
    }

}
