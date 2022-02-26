/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can be pushed by the player.
/// Breaks breakable blocks.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Boulder : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D body;
    [HideInInspector] public CircleCollider2D box;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    // Runs once per frame.
    void Update() {
        Damping();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        ProcessCollision(collision);
    }

    /* --- Methods --- */
    public void Init() {
        // Body
        body = GetComponent<Rigidbody2D>();
        body.mass = 1f; // 1e-6f;
        body.gravityScale = 15f;
        // Collision
        box = GetComponent<CircleCollider2D>();
        // Renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Damping() {
        body.velocity *= 0.95f; // GameRules.VelocityDamping;
    }

    private void ProcessCollision(Collision2D collision) {
        print(collision.gameObject.name);
        if (collision.gameObject.GetComponent<BreakableBlock>()) {
            collision.gameObject.GetComponent<BreakableBlock>().Break();
            // Destroy(gameObject);
        }
    }

}
