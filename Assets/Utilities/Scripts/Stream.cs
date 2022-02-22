/* --- Modules --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

/// <summary>
/// An interactable text box.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Stream : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class IOEvent : UnityEvent<string> { }
    public IOEvent OnSave;
    public IOEvent OnOpen;

    /* --- Components --- */
    public Ascii ascii; // The ascii textures that will be used.
    public SpriteRenderer characterRenderer; // The default character renderer object.
    public Material textMaterial; // The material to be used for the text.

    /* --- Variables --- */
    [SerializeField] public string text; // The text in this stream.
    [SerializeField] [ReadOnly] public bool isActive = false; // Controls whether this stream is being edited.
    [SerializeField] [ReadOnly] private SpriteRenderer[] characterRenderers; // Holds the currently rendered characters.
    [SerializeField] [Range(0.05f, 0.5f)] private float spacing = 0.4f; // The spacing between the characters.
    [SerializeField] [Range(1, 20)] private int maxCharacters = 5;
    [SerializeField] public bool acceptOnlyNumbers = false;

    /* --- Unity --- */
    void Start() {
        SetText(text);
        OnOpen.Invoke(text);
    }

    void OnMouseDown() {
        StartCoroutine(DelayedActivate());
        IEnumerator DelayedActivate() {
            yield return new WaitForSeconds(0f);
            isActive = true;
            yield return null;
        }
    }

    void Update() {

        // Get the input string if it's active.
        if (isActive) {
            GetInputText();
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
        }

        // The deactivation logic.
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) {
            isActive = false;
        }

    }

    /* --- Methods --- */
    void GetInputText() {
        foreach (char character in Input.inputString) {
            if (character == '\b' && text.Length != 0) {
                text = text.Substring(0, text.Length - 1);
                SetText(text);
            }
            else if (ascii.parser.ContainsKey(character) && text.Length < maxCharacters) {
                int temp;
                if (acceptOnlyNumbers && Int32.TryParse(character.ToString(), out temp)) {
                    text = text + character;
                }
                else if (!acceptOnlyNumbers) {
                    text = text + character;
                }
                SetText(text);
            }
        }
    }

    public void SetText(string text) {
        // Delete the previous text
        if (characterRenderers != null) {
            for (int i = 0; i < characterRenderers.Length; i++) {
                if (characterRenderers[i] != null) {
                    Destroy(characterRenderers[i].gameObject);
                }
            }
        }


        // Create the new characters
        characterRenderers = new SpriteRenderer[text.Length];
        for (int i = 0; i < text.Length; i++) {
            SpriteRenderer newCharacterRenderer = Instantiate(characterRenderer.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            newCharacterRenderer.transform.localPosition = characterRenderer.transform.localPosition + new Vector3(spacing * i, 0f, 0f);
            if (ascii.parser.ContainsKey(text[i])) {
                newCharacterRenderer.sprite = ascii.parser[text[i]];
            }
            else {
                newCharacterRenderer.sprite = ascii.parser[' '];
            }
            newCharacterRenderer.material = textMaterial;
            characterRenderers[i] = newCharacterRenderer;
        }
    }

}