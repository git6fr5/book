using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Label : MonoBehaviour {

    /* --- Components --- */
    public string text; // The text in for this label.
    public Ascii ascii; // The ascii textures that will be used.
    public SpriteRenderer characterRenderer; // The default character renderer object.
    public Material textMaterial; // The material to be used for the text.

    /* --- Variables --- */
    [SerializeField] [ReadOnly] private SpriteRenderer[] characterRenderers; // Holds the currently rendered characters.
    [SerializeField] [Range(0.05f, 0.5f)] private float spacing = 0.4f; // The spacing between the characters.

    /* --- Unity --- */
    void Start() {
        SetText(text);
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
            newCharacterRenderer.transform.localPosition = new Vector3(spacing * i, 0f, 0f) ;
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
