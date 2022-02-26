using System.Collections;
/* --- Libraries --- */
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
public class Counter : MonoBehaviour {

    /* --- Components --- */
    public int score; // The text in for this label.
    public int maxScore; // The text in for this label.
    public SpriteRenderer scoreRenderer; // The default character renderer object.
    public Sprite scoreSprite; // The default character renderer object.
    public Sprite emptySprite; // The default character renderer object.
    public Material textMaterial; // The material to be used for the text.

    /* --- Variables --- */
    [SerializeField] [ReadOnly] private SpriteRenderer[] scoreRenderers; // Holds the currently rendered characters.
    [SerializeField] [Range(0.05f, 4f)] private float spacing = 2f; // The spacing between the characters.

    /* --- Unity --- */
    void Start() {
        CreateScoreRenderers();
    }

    public void CreateScoreRenderers() {

        // Delete the previous text
        if (scoreRenderers != null) {
            for (int i = 0; i < scoreRenderers.Length; i++) {
                if (scoreRenderers[i] != null) {
                    Destroy(scoreRenderers[i].gameObject);
                }
            }
        }

        // Create the new characters
        scoreRenderers = new SpriteRenderer[maxScore];
        for (int i = 0; i < scoreRenderers.Length; i++) {
            SpriteRenderer newCharacterRenderer = Instantiate(scoreRenderer.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            newCharacterRenderer.transform.localPosition = new Vector3(2f * spacing * i, 0f, 0f);
            // newCharacterRenderer.material = textMaterial;
            scoreRenderers[i] = newCharacterRenderer;
        }

    }

    public void SetScore(int page, int score, int maxScore) {

        this.score = score;
        this.maxScore = maxScore;

        // Create the new characters
        for (int i = 0; i < scoreRenderers.Length; i++) {
            scoreRenderers[i].sprite = emptySprite;
        }

        // Create the new characters
        for (int i = 0; i < Mathf.Min(score, scoreRenderers.Length); i++) {
            scoreRenderers[i].sprite = scoreSprite;
            scoreRenderers[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }

        scoreRenderers[page].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

}
