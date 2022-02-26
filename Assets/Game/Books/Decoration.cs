using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Decoration : MonoBehaviour {

    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    public void Init(Vector2Int vectorID) {

        int increment = sprites.Length / 4;
        int startIndex = vectorID.x * increment;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(startIndex, Mathf.Min(startIndex + increment, sprites.Length))];

        gameObject.SetActive(true);
    }

}
