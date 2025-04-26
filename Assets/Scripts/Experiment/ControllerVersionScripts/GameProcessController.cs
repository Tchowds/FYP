using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;


// A completely symmetrical version of GameProcess but using controller versions of the detectors
// Refer to GameProcess for comments on the code
public class GameProcessController : MonoBehaviour
{
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

    [Header("Cube Materials")]
    [SerializeField] private Material rightCubeMaterial;
    [SerializeField] private Material leftCubeMaterial;

    [Header("Completion Text")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TMP_Text completionText;



    private CubeSpawner cubeSpawner;
    private GestureDetectorController gestureDetector;
    private PoseDetectorSubController poseDetector;

    private Levels levels;
    private Dictionary<string, Texture2D> gestureTextures = new Dictionary<string, Texture2D>();
    private Queue<GameObject> rightCubes = new Queue<GameObject>();
    private Queue<string> rightGestures = new Queue<string>();
    private Queue<GameObject> leftCubes = new Queue<GameObject>();
    private Queue<string> leftGestures = new Queue<string>();

    private int score = 0;
    private int completed = 0;

    private int totalBlocks = 1;
    private string currentLevel = "Easy";
    

    void Start()
    {
        gestureDetector = GetComponent<GestureDetectorController>();
        poseDetector = GetComponent<PoseDetectorSubController>();
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
        cubeSpawner = GetComponentInChildren<CubeSpawner>();
        string json = roundsFile.text;
        levels = JsonConvert.DeserializeObject<Levels>(json);
    }

    void OnEnable()
    {
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
        if (completed >= totalBlocks)
        {
            completionPanel.SetActive(true);
            completionText.text = "You scored " + score + " points!";
            score = 0;
            completed = 0;
            rightCubes.Clear();
            leftCubes.Clear();
            rightGestures.Clear();
            leftGestures.Clear();
            gameObject.SetActive(false);
        }
        checkCubes(rightCubes, rightGestures, gestureDetector.getRightGesture, gestureDetector.resetRightGesture, poseDetector.getRightPose, poseDetector.resetRightPose);
        checkCubes(leftCubes, leftGestures, gestureDetector.getLeftGesture, gestureDetector.resetLeftGesture, poseDetector.getLeftPose, poseDetector.resetLeftPose);

    }

    private void checkCubes(Queue<GameObject> cubes, Queue<string> gestures, Func<string> getGesture, Action resetGesture, Func<string> getPose, Action resetPose)
    {
        if (cubes.Count > 0)
        {
            GameObject cube = cubes.Peek();
            string gesture = gestures.Peek();

            if (cube.GetComponent<SplitCube>().HasSplit())
            {
                cubes.Dequeue();
                gestures.Dequeue();
            }
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
            completed++;
            Debug.Log("Completed: " + completed);
        }
    }

    private IEnumerator ProcessSequence(List<object[]> sequence, float speed, Queue<GameObject> leftCubes, Queue<GameObject> rightCubes, Queue<string> leftGestures, Queue<string> rightGestures, float posOffset, Material leftMaterial, Material rightMaterial)
    {
        totalBlocks = sequence.Count;
        float distance = Vector3.Distance(transform.position, centreEyeAnchor.position);
        float travelTime = distance / speed;
        foreach (object[] command in sequence)
        {
            float delay = Convert.ToSingle(command[1]);
            yield return new WaitForSeconds(delay);
            string gesture = command[0].ToString();
            string hand = command[2].ToString();

            bool isRight = hand == "right";

            Material material = isRight ? rightMaterial : leftMaterial;
            GameObject cube = cubeSpawner.spawnCube(gestureTextures[gesture], material);

            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = centreEyeAnchor.position - cube.transform.position;
                rb.velocity = direction.normalized * speed;

                Vector3 perpendicular = Vector3.Cross(direction.normalized, Vector3.up).normalized;
                cube.transform.position += perpendicular * (isRight ? posOffset : -posOffset);
            }
            cube.GetComponent<SplitCube>().SplitAndDestroy(travelTime - 0.2f);
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
        yield return new WaitForSeconds(travelTime - 0.2f);
        totalBlocks = 0;
    }

    public void setLevel(string level)
    {
        currentLevel = level;
    }
}
