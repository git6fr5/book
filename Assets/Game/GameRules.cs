using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameRules : MonoBehaviour {

    /* --- Static Tags --- */
    public static string PlayerTag = "Player";
    public static string GroundTag = "Ground";
    public static string BookTag = "Book";

    /* --- Static Variables --- */
    // Instance.
    public static GameRules Instance;
    // Player.
    public static Player MainPlayer;
    // Movement.
    public static float VelocityDamping = 0.95f;
    public static float MovementPrecision = 0.05f;
    public static float GravityScale = 1f;
    public static float Acceleration = 200f;
    public static float Floatiness = 0.4f;
    // Animation.
    public static int GridSize = 16;
    public static float FrameRate = 24f;
    public static float OutlineWidth = 1f / 16f;
    // Camera.
    public static UnityEngine.Camera MainCamera;
    public static Vector3 MousePosition => MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

    /* --- Parameters --- */
    public float timeScale = 1f;

    /* --- Properties --- */
    public Player mainPlayer;
    public float velocityDamping = 0.95f;
    public float gravityScale;
    public float frameRate;
    public int cameraX;
    public int cameraY;
    public bool isPaused;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {

        // Shake the camera
        if (shake) {
            shake = Shake();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (isPaused) {
                timeScale = 1f;
                isPaused = false;
            }
            else {
                isPaused = true;
                timeScale = 0f;
            }
        }

        Time.timeScale = timeScale;

    }

    /* --- Methods --- */
    private void Init() {
        // Set these static variables.
        // Singletons.
        MainCamera = Camera.main;
        MainPlayer = mainPlayer;

        // Movement
        VelocityDamping = velocityDamping;
        GravityScale = gravityScale;
        FrameRate = frameRate;

        // Instance
        Instance = this;
    }

    [Header("Shake")]
    [SerializeField, ReadOnly] public float shakeStrength = 1f;
    [SerializeField, ReadOnly] public float shakeDuration = 0.5f;
    [SerializeField, ReadOnly] float elapsedTime = 0f;
    [SerializeField, ReadOnly] public bool shake;
    public AnimationCurve curve;

    public bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position += (Vector3)Random.insideUnitCircle * shakeStrength;
        return true;
    }

    
    /* --- Events --- */
    public static void CameraShake(float strength, float duration) {
        if (strength == 0f) {
            return;
        }
        if (!Instance.shake) {
            Instance.shakeStrength = strength;
            Instance.shakeDuration = duration;
            Instance.shake = true;
        }
        else {
            Instance.shakeStrength = Mathf.Max(Instance.shakeStrength, strength);
            Instance.shakeDuration = Mathf.Max(Instance.shakeDuration, Instance.elapsedTime + duration);
        }
    }

    public static void ResetLevel() {

    }

    /* --- Debug --- */
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(cameraX, cameraY, 1f));

        Gizmos.DrawWireCube( new Vector3(Book.Width / 2f, Book.Height / 2f, 0f), new Vector3(Book.Width , Book.Height, 1f));
        Gizmos.DrawWireCube(new Vector3(Book.Width / 2f, -Book.Height / 2f, 0f), new Vector3(Book.Width, Book.Height, 1f));
        Gizmos.DrawWireCube(new Vector3(-Book.Width / 2f, Book.Height / 2f, 0f), new Vector3(Book.Width, Book.Height, 1f));
        Gizmos.DrawWireCube(new Vector3(-Book.Width / 2f, -Book.Height / 2f, 0f), new Vector3(Book.Width, Book.Height, 1f));



    }

}

