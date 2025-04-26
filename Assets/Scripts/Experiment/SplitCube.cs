using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplitCube : MonoBehaviour
{
    // Private references that will be automatically set.
    private GameObject mainCube;
    private GameObject frontPlane;
    private List<GameObject> chunks;
    private bool hasSplit = false;

    public float velocityMagnitude = 1.0f;

    void Awake()
    {

        mainCube = transform.Find("MainCube").gameObject;
        frontPlane = transform.Find("FrontPlane").gameObject;

        if (mainCube != null)
        {
            // Gather all child GameObjects of mainCube.
            chunks = new List<GameObject>();
            foreach (Transform child in mainCube.transform)
            {
                chunks.Add(child.gameObject);
            }
        }
    }

    void Start()
    {
        // Ensure that all chunks are initially inactive.
        if (chunks != null)
        {
            foreach (GameObject chunk in chunks)
            {
                chunk.SetActive(false);
            }
        }


    }

    public void SplitAndDestroy()
    {
        StartCoroutine(WaitAndSplit(0f));
    }


    public void SplitAndDestroy(float time)
    {
        StartCoroutine(WaitAndSplit(time));
    }

    IEnumerator WaitAndSplit(float time)
    {
        yield return new WaitForSeconds(time);
        hasSplit = true;
        Split();
        Destroy(gameObject, 5f);
    }

    void Split()
    {

        // Activate all chunks.
        if (chunks != null)
        {
            foreach (GameObject chunk in chunks)
            {
                chunk.SetActive(true);
            }
        }

        // Disable the main cube's renderer so it becomes invisible.
        if (mainCube != null)
        {
            MeshRenderer mr = mainCube.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = false;
        }

        // Disable the frontPlane's renderer if it exists.
        if (frontPlane != null)
        {
            MeshRenderer planeRenderer = frontPlane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
                planeRenderer.enabled = false;
        }

        // Add Rigidbody components to each chunk so they fall under gravity.
        if (chunks != null)
        {
            foreach (GameObject chunk in chunks)
            {
                Rigidbody rb = chunk.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = chunk.AddComponent<Rigidbody>();

                rb.velocity = Random.insideUnitSphere * velocityMagnitude;
            }
        }
    }

    public bool HasSplit()
    {
        return hasSplit;
    }
}
