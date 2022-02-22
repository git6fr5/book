/* --- Libaries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Movement = Controller.Movement;
using Direction = Controller.Direction;
using Airborne = Controller.Airborne;
using ActionState = Controller.ActionState;

/// <summary>
/// Handles the collision framework and animation
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Mesh : MonoBehaviour {

    /* --- Data Structures --- */
    [System.Serializable]
    public struct AnimationData {
        public Sprite[] animation;
        public int startIndex;
        public int length;
        public float? interval;
        public float timeInterval;

        public AnimationData(Sprite[] animation, int startIndex, int length, float? interval = null) {
            this.animation = animation;
            this.startIndex = startIndex;
            this.length = length;
            this.interval = interval;
            this.timeInterval = 0f;
        }
    }

    [System.Serializable]
    public class EffectData {

        [HideInInspector] public Sprite[] animation;
        [SerializeField] public Particle particle;
        [SerializeField] public int frame;
        [HideInInspector] private bool clearedLastCheck;

        public bool Check(AnimationData animationData, int currFrame) {
            bool correctSettings = animation == animationData.animation && frame == currFrame;
            if (correctSettings) {
                if (clearedLastCheck) {
                    return false;
                }
                else {
                    clearedLastCheck = true;
                    return true;
                }
            }
            clearedLastCheck = false;
            return false;
        }

        public void Create() {
            if (particle == null) {
                // print("No particle effect for this action");
                return;
            }
            Particle newParticle = Instantiate(particle.gameObject, particle.transform.position, Quaternion.identity, null).GetComponent<Particle>();
            newParticle.gameObject.SetActive(true);
        }

    }

    /* --- Static Variables --- */
    public float postActionDuration = 8f / 24f;
    public static float PostJumpDuration = 4f / 24f;

    /* --- Dictionaries --- */
    public static Dictionary<Direction, Quaternion> DirectionQuaternions = new Dictionary<Direction, Quaternion>() {
        {Direction.Right, Quaternion.Euler(0, 0, 0) },
        {Direction.Left, Quaternion.Euler(0, 180, 0) }
    };

    /* --- Components --- */
    [HideInInspector] private Controller controller;
    [HideInInspector] private CapsuleCollider2D collisionBall;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [Space(2), Header("Collisions")]
    [SerializeField] public Hurtbox hurtbox; // Handles the damage collision checks.
    [SerializeField] public Feetbox feetbox; // Handles the ground collision checks.

    /* --- Parameters --- */
    [Space(2), Header("Animations")]
    [SerializeField] public Sprite[] idle = null;
    [SerializeField] public Sprite[] move = null;
    [SerializeField] public Sprite[] jumpRising = null;
    [SerializeField] public Sprite[] jumpFalling = null;
    [SerializeField] public Sprite[] postJump = null;
    [SerializeField] public Sprite[] preAction = null;
    [SerializeField] public Sprite[] action = null;
    [SerializeField] public Sprite[] postAction = null;
    [SerializeField] private float stretchiness = 0.1f;

    [Space(2), Header("Effects")]
    [SerializeField] private EffectData movementEffect;
    [SerializeField] private EffectData jumpEffect;

    /* --- Properties --- */
    [SerializeField] private AnimationData animationData; // Used to set what the current active animation is.
    [SerializeField, ReadOnly] private Vector2 stretch = Vector2.zero;
    [SerializeField, ReadOnly] private bool runningAction = false;
    [SerializeField, ReadOnly] private bool runningJump = false;
    [SerializeField, ReadOnly] private float postActionTimer = 0f;
    [SerializeField, ReadOnly] private float postJumpTimer = 0f;

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        int currFrame = Animate();
        Effect(currFrame);
        Flip();
        Stretch();
    }

    /* --- Methods --- */
    private void Init() {
        controller = transform.parent.GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collisionBall = GetComponent<CapsuleCollider2D>();
        animationData = new AnimationData(idle, 0, idle.Length);

        movementEffect.animation = move;
        jumpEffect.animation = jumpRising;

    }

    private int Animate() {
        GetAnimation();
        // Set the current frame.
        float frameRate = (animationData.interval != null) ? animationData.length / (float)animationData.interval : GameRules.FrameRate;
        int index = animationData.startIndex + ((int)Mathf.Floor(animationData.timeInterval * frameRate) % animationData.length);
        spriteRenderer.sprite = animationData.animation[index];
        return index;
    }

    private void Effect(int currFrame) {

        if (movementEffect.Check(animationData, currFrame)) {
            movementEffect.Create();
        }
        else if (jumpEffect.Check(animationData, currFrame)) {
            jumpEffect.Create();
        }

    }

    private void Flip() {
        transform.localRotation = DirectionQuaternions[controller.directionFlag];
    }

    private void Stretch() {
        stretch = new Vector2(1f, 1f);
        if (controller.airborneFlag != Airborne.Grounded) {
            float horizontalStretch = Mathf.Abs(controller.body.velocity.x) * stretchiness;
            float verticalStretch = Mathf.Abs(controller.body.velocity.y) * stretchiness;
            Vector2 deltaStretch = new Vector2((horizontalStretch - verticalStretch) / 2f, verticalStretch - horizontalStretch);
            stretch += deltaStretch;
        }
        spriteRenderer.material.SetVector("_Stretch", (Vector4)stretch);
    }

    /* --- Sub-Methods --- */
    private void GetAnimation() {
        Sprite[] prevAnimation = animationData.animation;
        animationData.timeInterval += Time.deltaTime;
        animationData.interval = null;

        if (controller.actionFlag != ActionState.None || runningAction) {
            AnimateAction();
        }
        else if (controller.airborneFlag != Airborne.Grounded || runningJump) {
            AnimateJump();
        }
        else if (controller.movementFlag != Movement.Idle) {
            AnimateMovement();
        }
        else {
            AnimateIdle();
        }

        if (animationData.animation != prevAnimation) {
            animationData.timeInterval = 0f;
        }

    }

    private void AnimateAction() {

        if (!runningAction) {
            postActionTimer = 0f;
            runningAction = true;
        }

        bool actionActive = controller.actionFlag != ActionState.None;
        if (!actionActive) {
            postActionTimer += Time.deltaTime;
        }

        // Run the pre-dash
        if (actionActive) {
            if (controller.actionFlag == ActionState.PreAction) {
                animationData.animation = preAction;
                animationData.startIndex = 0;
                animationData.length = preAction.Length;
            }
            else if (controller.actionFlag == ActionState.Action) {
                animationData.animation = action;
                animationData.startIndex = 0;
                animationData.length = action.Length;
            }
        }
        else if (!actionActive && postActionTimer < postActionDuration) {
            animationData.animation = postAction;
            animationData.startIndex = 0;
            animationData.length = postAction.Length;
            animationData.interval = postActionDuration; ;
        }

        if (!actionActive && postActionTimer > postActionDuration) {
            runningAction = false;
        }

    }

    protected virtual void AnimateJump() {

        if (!runningJump) {
            postJumpTimer = 0f;
            runningJump = true;
        }

        bool jumpActive = controller.airborneFlag != Airborne.Grounded;
        if (!jumpActive) {
            postJumpTimer += Time.deltaTime;
        }

        if (jumpActive) {
            if (controller.airborneFlag == Airborne.Rising || jumpFalling.Length == 0) {
                animationData.animation = jumpRising;
                animationData.startIndex = 0;
                animationData.length = jumpRising.Length;
            }
            else if (controller.airborneFlag == Airborne.Falling) {
                animationData.animation = jumpFalling;
                animationData.startIndex = 0;
                animationData.length = jumpFalling.Length;
            }
        }
        else if (!jumpActive && postJumpTimer < PostJumpDuration) {
            animationData.animation = postJump;
            animationData.startIndex = 0;
            animationData.length = postJump.Length;
            animationData.interval = PostJumpDuration; ;
        }

        if (!jumpActive && postJumpTimer > PostJumpDuration) {
            runningJump = false;
        }

    }

    protected virtual void AnimateMovement() {
        animationData.animation = move;
        animationData.startIndex = 0;
        animationData.length = move.Length;
    }

    protected virtual void AnimateIdle() {
        animationData.animation = idle;
        animationData.startIndex = 0;
        animationData.length = idle.Length;
    }


}
