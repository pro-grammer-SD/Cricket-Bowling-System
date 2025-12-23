using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CricketGameManager : MonoBehaviour
{
    public static CricketGameManager Instance { get; private set; }

    [Header("System")]
    public MeterSystem meter;
    [SerializeField] private GameObject ballPrefab;
    public Transform bounceMarker;
    public Transform spawnLeft, spawnRight;

    [Header("Visuals")]
    public Transform bowlerModel;

    [Header("Object Pooling")]
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> ballPool;

    [Header("UI Text")]
    public TextMeshProUGUI modeText;
    public TextMeshProUGUI dirText;

    [Header("Physics Settings")]
    [Range(10f, 25f)]
    public float targetBallSpeed = 25f;
    public float swingForce = 5f;
    public float spinAngle = 30f;

    [Header("Marker Bounds")]
    public float minX = -1.8f;
    public float maxX = 1.8f;
    public float minZ = 0f;
    public float maxZ = 14f;

    // State
    private bool isSwing = true;
    private bool isLeftOfWicket = true;
    private float selectedDirection = 0f;

    // Safety flag to prevent double bowling
    private bool canBowl = true;

    private WaitForSeconds resetDelay;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePool();
        resetDelay = new WaitForSeconds(2f);
    }

    void Start()
    {
        UpdateUI();
        UpdateBowlerPosition();
    }

    void Update()
    {
        HandleMarkerMovement();

        if (Input.GetKeyDown(KeyCode.Space) && canBowl)
        {
            Bowl();
        }
    }

    void UpdateBowlerPosition()
    {
        if (bowlerModel != null)
        {
            Transform targetSpawn = isLeftOfWicket ? spawnLeft : spawnRight;
            Vector3 newPos = new Vector3(targetSpawn.position.x, bowlerModel.position.y, targetSpawn.position.z);
            bowlerModel.position = newPos;
        }
    }

    void InitializePool()
    {
        ballPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.SetActive(false);
            ballPool.Enqueue(ball);
        }
    }

    GameObject GetBallFromPool(Vector3 position)
    {
        GameObject ball = ballPool.Dequeue();
        ball.transform.position = position;
        ballPool.Enqueue(ball);
        return ball;
    }

    public void Bowl()
    {
        // Lock the input so we can't bowl again immediately
        canBowl = false;

        // Stop the meter and get accuracy
        float accuracy = meter.GetAccuracy();

        Transform sp = isLeftOfWicket ? spawnLeft : spawnRight;

        // Get Ball from Pool
        GameObject ball = GetBallFromPool(sp.position);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        BallController bc = ball.GetComponent<BallController>();

        // Reset Physics BEFORE applying new velocity
        bc.ResetPhysics();

        // Math Calculations
        float distance = Vector3.Distance(sp.position, bounceMarker.position);
        float calculatedTime = distance / targetBallSpeed;

        float actualSwingForce = swingForce * accuracy;
        Vector3 targetPos = bounceMarker.position;

        if (isSwing && selectedDirection != 0)
        {
            float driftAmount = 0.5f * actualSwingForce * (calculatedTime * calculatedTime);
            Vector3 compensation = sp.right * selectedDirection * driftAmount;
            targetPos -= compensation;
        }

        Vector3 velocity = CalculateVelocity(sp.position, targetPos, calculatedTime);

        // Apply Velocity
        rb.linearVelocity = velocity;

        //Initialize Ball Data
        bc.Initialize(isSwing, accuracy, selectedDirection, swingForce, spinAngle);

        // Reset Game after delay
        StartCoroutine(ResetMeterRoutine());
    }

    IEnumerator ResetMeterRoutine()
    {
        yield return resetDelay;
        meter.SetActive(true); 
        canBowl = true;        
    }

    void HandleMarkerMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            bounceMarker.Translate(new Vector3(h, 0, v) * 5f * Time.deltaTime, Space.World);

            Vector3 currentPos = bounceMarker.position;
            currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
            currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ);
            currentPos.y = 0.12f;
            bounceMarker.position = currentPos;
        }
    }

    public void SetSwingMode() { isSwing = true; UpdateUI(); }
    public void SetSpinMode() { isSwing = false; UpdateUI(); }
    public void SetDirLeft() { selectedDirection = -1f; UpdateUI(); }
    public void SetDirStraight() { selectedDirection = 0f; UpdateUI(); }
    public void SetDirRight() { selectedDirection = 1f; UpdateUI(); }

    public void ToggleSide()
    {
        isLeftOfWicket = !isLeftOfWicket;
        UpdateBowlerPosition();
    }

    void UpdateUI()
    {
        if (modeText) modeText.text = isSwing ? "Mode: SWING" : "Mode: SPIN";
        if (dirText) dirText.text = selectedDirection == 0 ? "Dir: STRAIGHT" : (selectedDirection == 1 ? "Dir: RIGHT" : "Dir: LEFT");
    }

    Vector3 CalculateVelocity(Vector3 start, Vector3 end, float t)
    {
        Vector3 dist = end - start;
        float x = dist.x / t;
        float z = dist.z / t;
        float y = (dist.y + 0.5f * Mathf.Abs(Physics.gravity.y) * t * t) / t;
        return new Vector3(x, y, z);
    }
}