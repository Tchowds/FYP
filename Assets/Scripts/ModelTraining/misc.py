import numpy as np
import matplotlib.pyplot as plt
import json

# Miscellaneous functions for gesture data analysis


def find_gesture_distance(json_data, gesture_name):
    """
    Calculate the distance of a gesture by averaging the pose data and summing the distances between points.
    """
    data_arrays = np.array(json_data["gestures"][gesture_name]["poseGestureData"])
    data = np.mean(data_arrays, axis=0)
    points = data.reshape((-1, 3))
    print(points)
    distances = np.linalg.norm(np.diff(points, axis=0), axis=1)
    print(np.sum(distances))

def plot_gesture(json_data, gesture_names):
    """
    Plot the gesture data in 3D space for the given gesture names.
    """
    
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    
    for g_name in gesture_names:
        data = np.array(json_data["gestures"][g_name]["poseGestureData"][0])
        points = data.reshape((-1, 3))
        ax.plot(points[:, 0], points[:, 1], points[:, 2], marker='o', label=g_name)
    
    ax.set_title('3D Plot for Gestures')
    ax.set_xlim(-1, 1)
    ax.set_xticks(np.arange(-1, 1.1, 0.1))
    ax.set_ylim(-1, 1)
    ax.set_yticks(np.arange(-1, 1.1, 0.1))
    ax.set_zlim(-1, 1)
    ax.set_zticks(np.arange(-1, 1.1, 0.1))
    ax.set_ylabel('Y')
    ax.set_zlabel('Z')
    ax.legend()
    plt.show()


with open("PoseGestureData.json", "r") as f:
    json_data = json.load(f)

find_gesture_distance(json_data, "punch")

plot_gesture(json_data, ["right", "left", "up", "down"])