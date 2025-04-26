using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

// Matches the data structure of the rounnds JSON file
public class Level
{
    public double Speed { get; set; }
    public List<object[]> sequence { get; set; }
}

public class Levels
{
    public Level Easy { get; set; }
    public Level Medium { get; set; }
    public Level Hard { get; set; }
}


// Main class for the game, process responsible for spawning cubes, detecting gestures and poses, and managing the game state
public class GameProcess : MonoBehaviour
{
    // All the icons for the boxes corresponding to the gestures
    [Header("Gesture Texture Mappings")]
    [SerializeField] private Texture2D up;
    [SerializeField] private Texture2D down;
    [SerializeField] private Texture2D left;
    [SerializeField] private Texture2D right;
    [SerializeField] private Texture2D punch;
    [SerializeField] private Texture2D fist;
    [SerializeField] private Texture2D gun;
    [SerializeField] private Texture2D thumbs;

    [Header("Anchors")]
    [SerializeField] private Transform centreEyeAnchor;

    [Header("Config file")]
    [SerializeField] private TextAsset roundsFile;

    // Colours for the cubes
    [Header("Cube Materials")]
    [SerializeField] private Material rightCubeMaterial;
    [SerializeField] private Material leftCubeMaterial;

    [Header("Completion Text")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TMP_Text completionText;

    private CubeSpawner cubeSpawner;
    private GestureDetector gestureDetector;
    private PoseDetector poseDetector;

    // Holds all the sequence data for each game level
    private Levels levels;
    
    // Queues to define the sequences of cubes and the gestures to performed to destroy them
    private Dictionary<string, Texture2D> gestureTextures = new Dictionary<string, Texture2D>();
    private Queue<GameObject> rightCubes = new Queue<GameObject>();
    private Queue<string> rightGestures = new Queue<string>();
    private Queue<GameObject> leftCubes = new Queue<GameObject>();
    private Queue<string> leftGestures = new Queue<string>();

    // Score for counting blocks bro
    private int score = 0;

    // Number of blocks broken
    private int completed = 0;

    // Number of blocks that have been spawned
    private int totalBlocks = 1;
    private string currentLevel = "Easy";
    

    void Start()
    {
        // Set up the detectors and map all the gestures to their textures
        gestureDetector = GetComponent<GestureDetector>();
        poseDetector = GetComponent<PoseDetector>();
        gestureTextures.Add("up", up);
        gestureTextures.Add("down", down);
        gestureTextures.Add("left", left);
        gestureTextures.Add("right", right);
        gestureTextures.Add("punch", punch);
        gestureTextures.Add("fist", fist);
        gestureTextures.Add("gun", gun);
        gestureTextures.Add("thumbs", thumbs);

    }

    void Awake()
    {
        // set up the cube spawner and load the json data
        cubeSpawner = GetComponentInChildren<CubeSpawner>();
        string json = roundsFile.text;
        levels = JsonConvert.DeserializeObject<Levels>(json);
    }

    void OnEnable()
    {
        // Re position the game object to be in front of the user
        transform.position = centreEyeAnchor.position + centreEyeAnchor.forward * 7;
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        transform.rotation = Quaternion.LookRotation(transform.position - centreEyeAnchor.position);
    }

    public void StartGame()
    {
        if (currentLevel == "Easy") StartCoroutine(ProcessSequence(levels.Easy.sequence, Convert.ToSingle(levels.Easy.Speed), leftCubes, rightCubes, leftGestures, rightGestures, 0.25f, leftCubeMaterial, rightCubeMaterial));
        else if (currentLevel == "Medium") StartCoroutine(ProcessSequence(levels.Medium.sequence, Convert.ToSingle(levels.Medium.Speed), leftCubes, rightCubes, leftGestures, rightGestures, 0.25f, leftCubeMaterial, rightCubeMaterial));
        else if (currentLevel == "Hard") StartCoroutine(ProcessSequence(levels.Hard.sequence, Convert.ToSingle(levels.Hard.Speed), leftCubes, rightCubes, leftGestures, rightGestures, 0.25f, leftCubeMaterial, rightCubeMaterial));
    }

    void Update()
    {
        // If game is completed, reset the game state and show the completion panel
        if (completed >= totalBlocks)
        {
            completed = 0;
            rightCubes.Clear();
            leftCubes.Clear();
            rightGestures.Clear();
            leftGestures.Clear();
            completionPanel.SetActive(true);
            completionText.text = "You scored " + score + " points!";
            score = 0;
            gameObject.SetActive(false);
        }

        // Checks cubes to see if the user has performed the correct gesture or pose to destroy them
        checkCubes(rightCubes, rightGestures, gestureDetector.getRightGesture, gestureDetector.resetRightGesture, poseDetector.getRightPose, poseDetector.resetRightPose);
        checkCubes(leftCubes, leftGestures, gestureDetector.getLeftGesture, gestureDetector.resetLeftGesture, poseDetector.getLeftPose, poseDetector.resetLeftPose);

    }

    // Checks if the user has performed the correct gesture or pose to destroy the cube
    private void checkCubes(Queue<GameObject> cubes, Queue<string> gestures, Func<string> getGesture, Action resetGesture, Func<string> getPose, Action resetPose)
    {
        if (cubes.Count > 0)
        {
            GameObject cube = cubes.Peek();
            string gesture = gestures.Peek();

            // Handle previously broken cubes by removing them from the queue
            if (cube.GetComponent<SplitCube>().HasSplit())
            {
                cubes.Dequeue();
                gestures.Dequeue();
            }

            // If the gesture or cube has been matched, destroy the cube and update the score
            if (getGesture() == gesture)
            {
                resetGesture();
                cube.GetComponent<SplitCube>().SplitAndDestroy();
                cubes.Dequeue();
                gestures.Dequeue();
                score++;
            } else if (getPose() == gesture)
            {
                resetPose();
                cube.GetComponent<SplitCube>().SplitAndDestroy();
                cubes.Dequeue();
                gestures.Dequeue();
                score++;
            }
            else return;

            // On cube destruction, update the score and completed blocks
            completed++;
        }
    }

    // The main function that processes the sequence of cubes and their corresponding gestures
    private IEnumerator ProcessSequence(List<object[]> sequence, float speed, Queue<GameObject> leftCubes, Queue<GameObject> rightCubes, Queue<string> leftGestures, Queue<string> rightGestures, float posOffset, Material leftMaterial, Material rightMaterial)
    {
        totalBlocks = sequence.Count;
        float distance = Vector3.Distance(transform.position, centreEyeAnchor.position);
        // Calculate the time taken for a block to reach the user to determine when to split the cube
        float travelTime = distance / speed;

        // Loop through the sequence of cubes and spawn them with the corresponding gesture
        foreach (object[] command in sequence)
        {
            // Wait for the specified delay before spawning the next cube as defined in the JSON file
            float delay = Convert.ToSingle(command[1]);
            yield return new WaitForSeconds(delay);
            string gesture = command[0].ToString();
            string hand = command[2].ToString();

            // Spawn the cube with the corresponding gesture and hand
            bool isRight = hand == "right";

            Material material = isRight ? rightMaterial : leftMaterial;
            GameObject cube = cubeSpawner.spawnCube(gestureTextures[gesture], material);

            // Set up the cube's position and velocity
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = centreEyeAnchor.position - cube.transform.position;
                rb.velocity = direction.normalized * speed;

                Vector3 perpendicular = Vector3.Cross(direction.normalized, Vector3.up).normalized;
                cube.transform.position += perpendicular * (isRight ? posOffset : -posOffset);
            }
            
            // Set up the cube's lifetime and splitting behaviour
            cube.GetComponent<SplitCube>().SplitAndDestroy(travelTime - 0.2f);

            // Add the cube and gesture to the appropriate queue based on the hand
            if(isRight)
            {
                rightCubes.Enqueue(cube);
                rightGestures.Enqueue(gesture);
            }
            else
            {
                leftCubes.Enqueue(cube);
                leftGestures.Enqueue(gesture);
            }
        }
        
        // After all cubes have been spawned, wait for the travel time minus a small buffer before completing the game
        yield return new WaitForSeconds(travelTime - 0.2f);
        totalBlocks = 0;
    }

    public void setLevel(string level)
    {
        currentLevel = level;
    }
}
