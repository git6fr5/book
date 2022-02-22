/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Movement = Controller.Movement;
using Direction = Controller.Direction;
using Airborne = Controller.Airborne;
using ActionState = Controller.ActionState;

/// <summary>
///
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class NoiseMesh : MonoBehaviour {

    public Controller controller;
    public AudioSource audioSource;

    public AudioClip movementAudio;
    public AudioClip jumpAudio;
    public AudioClip landAudio;

    public Sound soundType;

    public float footstepInterval;
    public float footstepTicks;

    public bool waitForLand;

    public enum Sound {
        Jump,
        Move,
        Idle
    } 

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    private void Init() {
        controller = transform.parent.GetComponent<Controller>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        float deltaTime = Time.deltaTime;
        GetAudio(deltaTime);
    }

    /* --- Sub-Methods --- */
    private void GetAudio(float deltaTime) {

        bool playingJump = AudioJump(deltaTime);
        if (playingJump) {
            return;
        }

        if (controller.airborneFlag != Airborne.Grounded) {

        }
        else if (controller.movementFlag != Movement.Idle) {
            AudioMovement(deltaTime);
        }
        else {
            AudioIdle();
        }

    }

    private bool AudioJump(float deltaTime) {

        if (Input.GetKeyDown(KeyCode.Space)) {
            print("Jumping");
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
            audioSource.clip = jumpAudio;
            audioSource.Play();
            return true;
        }

        if (controller.airborneFlag == Airborne.Falling) {
            waitForLand = true;
        }

        if (controller.airborneFlag == Airborne.Grounded && waitForLand ) {
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
            audioSource.clip = landAudio;
            audioSource.Play();
            waitForLand = false;
            return true;
        }

        if ((audioSource.clip == jumpAudio || audioSource.clip == landAudio) && audioSource.isPlaying) {
            return true;
        }

        soundType = Sound.Jump;
        return false;

    }

    private void AudioMovement(float deltaTime) {

        footstepTicks -= deltaTime;
        if (footstepTicks < 0f) {
            audioSource.clip = movementAudio;
            if (!audioSource.isPlaying) {
                audioSource.Play();
                footstepTicks = footstepInterval;
            }
        }
        soundType = Sound.Move;
    }

    private void AudioIdle() {
        audioSource.clip = null;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        footstepTicks = 0f;
        soundType = Sound.Idle;
    }

}
