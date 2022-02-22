using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Particle : MonoBehaviour {

    /* --- Data Structures --- */
    [System.Serializable]
    public struct ParticleData {
        [SerializeField, ReadOnly] public Sprite[] animation;
        [SerializeField, ReadOnly] public int length;
        [SerializeField, ReadOnly] public float timeInterval;

        public ParticleData(Sprite[] animation, int length) {
            this.animation = animation;
            this.length = animation.Length;
            this.timeInterval = 0f;
        }
    }

    /* --- Components --- */
    private SpriteRenderer spriteRenderer;

    /* --- Parameters --- */
    [SerializeField] private Sprite[] particle;
    [SerializeField] private int frameRate = -1;
    [SerializeField] public bool loop;
    [SerializeField] public bool pause;
    [SerializeField] public bool pauseOnEnd;

    /* --- Properties --- */
    [SerializeField] private ParticleData particleData;

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        float deltaTime = Time.deltaTime;
        if (!pause) {
            Animate(deltaTime);
            bool isEnd = spriteRenderer.sprite == particleData.animation[particleData.animation.Length - 1];
            if (pauseOnEnd && isEnd) {
                pause = true;
            }
        }
    }

    /* --- Methods --- */
    private void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleData = new ParticleData(particle, particle.Length);
    }

    private void Animate(float deltaTime) {
        // Set the current frame.
        particleData.timeInterval += deltaTime;
        float frameRate = this.frameRate > 0 ? (float)this.frameRate : GameRules.FrameRate;
        int index = (int)Mathf.Floor(particleData.timeInterval * frameRate);
        if (!loop && index >= particleData.length) {
            Destroy(gameObject);
        }
        index = index % particleData.length;
        spriteRenderer.sprite = particleData.animation[index];
    }

}
