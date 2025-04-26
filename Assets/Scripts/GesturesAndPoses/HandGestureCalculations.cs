using System;
using UnityEngine;

// This class contains methods for transforming and normalizing hand gesture data
public static class HandGestureCalculations
{

    // Runs all the gesture transormations
    public static float[] runAllTransformations(Vector3[] buffer, Quaternion rotationAnchor, float boundingCubeSize)
    {
        Vector3[] translatedBuffer = translateNormalization(buffer);
        Vector3[] rotatedBuffer = rotateBuffer(translatedBuffer, rotationAnchor);
        float boundingWidth = findBoundingWidths(translatedBuffer);
        Vector3[] normalizedBuffer = normalizeBuffer(rotatedBuffer, boundingWidth, boundingCubeSize);
        float[] flattenedBuffer = flattenBuffer(normalizedBuffer);
        return flattenedBuffer;
    }

    // Position normalisation
    // This method translates the buffer to the origin (0,0,0) by subtracting the first point from all points in the buffer
    public static Vector3[] translateNormalization(Vector3[] buffer)
    {
        Vector3 origin = buffer[0];
        Vector3[] translatedBuffer = new Vector3[buffer.Length];

        for (int i = 0; i < buffer.Length; i++)
        {
            translatedBuffer[i] = buffer[i] - origin;
        }

        return translatedBuffer;
    }

    // Rotational normalisation
    // This method rotates the buffer points to align with the center eye anchor rotation
    public static Vector3[] rotateBuffer(Vector3[] buffer, Quaternion centerEyeAnchorRotation)
    {
        Vector3[] rotatedBuffer = new Vector3[buffer.Length];
        Quaternion inverseRotation = Quaternion.Inverse(centerEyeAnchorRotation);
        for (int i = 0; i < buffer.Length; i++)
        {
            rotatedBuffer[i] = inverseRotation * buffer[i];
        }
        return rotatedBuffer;
    }

    // Utility function to find the bounding widths of the space covered by the points in the buffer
    public static float findBoundingWidths(Vector3[] buffer)
    {
        Vector3 min = buffer[0];
        Vector3 max = buffer[0];

        for (int i = 0; i < buffer.Length; i++)
        {
            min = Vector3.Min(min, buffer[i]);
            max = Vector3.Max(max, buffer[i]);
        }

        float widthX = max.x - min.x;
        float widthY = max.y - min.y;
        float widthZ = max.z - min.z;

        return Mathf.Max(Mathf.Max(widthX, widthY), widthZ);
    }

    // This method normalizes the buffer points to fit within a bounding cube of a specified size
    // It scales the points based on the bounding width and the desired bounding cube size
    public static Vector3[] normalizeBuffer(Vector3[] buffer, float boundingWidth, float boundingCubeSize)
    {
        Vector3[] normalizedBuffer = new Vector3[buffer.Length];
        for (int i = 0; i < buffer.Length; i++)
        {
            normalizedBuffer[i] = (buffer[i] / boundingWidth) * boundingCubeSize;
        }
        return normalizedBuffer;
    }

    // This method flattens the 3D buffer points into a 1D array of floats for processing and storage
    public static float[] flattenBuffer(Vector3[] buffer)
    {
        float[] flattenedBuffer = new float[buffer.Length * 3];
        for (int i = 0; i < buffer.Length; i++)
        {
            flattenedBuffer[i * 3] = buffer[i].x;
            flattenedBuffer[i * 3 + 1] = buffer[i].y;
            flattenedBuffer[i * 3 + 2] = buffer[i].z;
        }
        return flattenedBuffer;
    }
}