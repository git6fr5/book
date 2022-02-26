using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour {
    
    /* --- Components --- */
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D body;
    [HideInInspector] public CircleCollider2D box;

    /* --- Unity --- */
    void OnTriggerEnter2D(Collider2D collider) {
        ProcessCollision(collider);
    }

    /* --- Methods --- */
    public void Init(Vector2 velocity) {
        gameObject.SetActive(true);
        // Body
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.velocity = velocity;
        // Collider
        box = GetComponent<CircleCollider2D>();
        box.isTrigger = true;
        // Renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ProcessCollision(Collider2D collider) {
        if (collider.GetComponent<BreakableBlock>()) {
            collider.GetComponent<BreakableBlock>().Break();
            Destroy(gameObject);
        }
    }

}
