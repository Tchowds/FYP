using UnityEngine;
using System.Collections;

// This script is responsible for spawning destructible cubes in the scene
public class CubeSpawner : MonoBehaviour
{
    public GameObject destructibleCubePrefab;


    void Start()
    {
        if (destructibleCubePrefab == null)
        {
            Debug.LogError("Destructible cube prefab not assigned in CubeSpawner.");
            return;
        }
    }

    // Method to spawn a cube with a given texture and material
    public GameObject spawnCube(Texture2D texture, Material material)
    {
        GameObject cube = Instantiate(destructibleCubePrefab, transform.position, transform.rotation);
        cube.transform.Find("MainCube").GetComponent<Renderer>().material = material;
        cube.transform.Find("FrontPlane").GetComponent<Renderer>().material = material;
        cube.transform.Find("FrontPlane").GetComponent<Renderer>().material.mainTexture = texture;
        return cube;
    }

}
