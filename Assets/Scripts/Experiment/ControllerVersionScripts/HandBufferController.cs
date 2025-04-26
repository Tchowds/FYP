using System.Collections.Generic;
using UnityEngine;

public class HandBufferController : MonoBehaviour
{
    [SerializeField] private Transform handTransform; // The transform to sample for positions
    [SerializeField] private float bufferTimeLength = 3.0f;
    [SerializeField] private int framesPerSecond = 10;
    [SerializeField] private float deadzone = 0.1f;

    // We use a single buffer instead of a list of buffers
    private Queue<Vector3> buffer;
    private float lastUntrackedTime = 0.0f;
    private float lastUpdateTime = 0.0f;

    void Start()
    {
        buffer = new Queue<Vector3>();
    }

    void Update()
    {
        if (Time.time - lastUpdateTime >= (1.0f / framesPerSecond))
        {
            UpdateBuffer();
            lastUpdateTime = Time.time;
        }
    }

    // Instead of checking for tracking, we simply enqueue the handTransform's position.
    // If the handTransform is null, we enqueue a zero vector and update the last untracked time.
    private void UpdateBuffer()
    {
        if (handTransform != null)
        {
            buffer.Enqueue(handTransform.position);
        }
        else
        {
            buffer.Enqueue(Vector3.zero);
            lastUntrackedTime = Time.time;
        }

        // Maintain the buffer length according to bufferTimeLength and framesPerSecond
        while (buffer.Count > bufferTimeLength * framesPerSecond)
        {
            buffer.Dequeue();
        }
    }

    // Returns the buffer as an array, or null if there was a recent untracked period.
    public Vector3[] GetBuffer()
    {
        if (Time.time - lastUntrackedTime < bufferTimeLength)
            return null;
        else
            return buffer.ToArray();
    }

    // Returns the buffer as an array if the total trajectory length is above the deadzone.
    public Vector3[] GetBufferWithDeadzone()
    {
        if (Time.time - lastUntrackedTime < bufferTimeLength)
            return null;
        else
        {
            Vector3[] bufferArray = buffer.ToArray();
            float totalDistance = 0f;
            for (int i = 1; i < bufferArray.Length; i++)
            {
                totalDistance += Vector3.Distance(bufferArray[i - 1], bufferArray[i]);
            }
            return totalDistance >= deadzone ? bufferArray : null;
        }
    }

    // Helper method that determines if the total movement in the buffer is above the deadzone.
    public bool WithinDeadzone(Vector3[] bufferArray)
    {
        float totalDistance = 0f;
        for (int i = 1; i < bufferArray.Length; i++)
        {
            totalDistance += Vector3.Distance(bufferArray[i - 1], bufferArray[i]);
        }
        return totalDistance >= deadzone;
    }

    public float GetBufferTimeLength()
    {
        return bufferTimeLength;
    }

    // Interpolates the latest segment of the buffer corresponding to the specified time.
    public Vector3[] InterpolateBuffer(float time)
    {
        Vector3[] bufferArray = GetBufferWithDeadzone();
        if (bufferArray == null)
            return null;
        
        int totalFrames = bufferArray.Length;

        // Determine the number of frames corresponding to the desired time span
        int subFrames = Mathf.RoundToInt(time * framesPerSecond);
        subFrames = Mathf.Clamp(subFrames, 1, totalFrames);

        // Extract the last 'subFrames' frames from the full buffer
        Vector3[] subBuffer = new Vector3[subFrames];
        for (int i = 0; i < subFrames; i++)
        {
            subBuffer[i] = bufferArray[totalFrames - subFrames + i];
        }

        // Create an output array with the same number of elements as the full buffer.
        Vector3[] resultBuffer = new Vector3[totalFrames];

        // If there's only one frame, return it directly.
        if (totalFrames == 1)
        {
            resultBuffer[0] = subBuffer[0];
            return resultBuffer;
        }

        // Interpolate the subBuffer to match the full buffer's length.
        for (int i = 0; i < totalFrames; i++)
        {
            float t = (float)i / (totalFrames - 1); // Normalized [0, 1]
            float subIndex = t * (subFrames - 1);
            int index0 = Mathf.FloorToInt(subIndex);
            int index1 = Mathf.Min(index0 + 1, subFrames - 1);
            float lerpFactor = subIndex - index0;

            resultBuffer[i] = Vector3.Lerp(subBuffer[index0], subBuffer[index1], lerpFactor);
        }

        return WithinDeadzone(resultBuffer) ? resultBuffer : null;
    }

    // Generates an array of interpolated buffers based on the specified time increments.
    public Vector3[][] GetInterpolatedBuffers(float time, float minInterval, IntervalStrategy strategy)
    {
        if (time <= 0f)
            return null;
        
        List<Vector3[]> interpolatedBuffers = new List<Vector3[]>();
        
        if (strategy == IntervalStrategy.LARGEST)
        {
            interpolatedBuffers.Add(GetBufferWithDeadzone());
            for (float currentTime = bufferTimeLength - time; currentTime >= (time > minInterval ? time : minInterval); currentTime -= time)
            {
                Vector3[] interpolatedBuffer = InterpolateBuffer(currentTime);
                if (interpolatedBuffer != null)
                    interpolatedBuffers.Add(interpolatedBuffer);
                else
                    return null;
            }            
        }
        else
        {
            for (float currentTime = (time > minInterval ? time : minInterval); currentTime <= bufferTimeLength; currentTime += time)
            {
                Vector3[] interpolatedBuffer = InterpolateBuffer(currentTime);
                if (interpolatedBuffer != null)
                    interpolatedBuffers.Add(interpolatedBuffer);
                else
                    return null;
            }
            interpolatedBuffers.Add(GetBufferWithDeadzone());
        }

        return interpolatedBuffers.ToArray();
    }
}
