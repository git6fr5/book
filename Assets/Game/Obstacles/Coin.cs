/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collect to get more pages
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Coin : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public CircleCollider2D box;

    /* --- Parameters --- */
    [SerializeField] public int bookID;

    /* --- Properties --- */
    public float floatDirection;
    public float floatSpeed;
    [HideInInspector] public Vector3? origin = null;

    /* --- Unity --- */
    void Start() {
        Init();
        origin = (Vector3?)transform.position;
    }

    void OnEnable() {
        if (origin != null) {
            transform.position = (Vector3)origin;
        }
        floatDirection = 1f;
        StartCoroutine(IEFloat());
    }

    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Float(deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collider) {
        ProcessCollision(collider);
    }

    /* --- Methods --- */
    public void Init() {
        box = GetComponent<CircleCollider2D>();
        box.isTrigger = true;
    }

    private void ProcessCollision(Collider2D collider) {
        Controller controller = collider.GetComponent<Hurtbox>()?.controller;
        if (controller == GameRules.MainPlayer) {
            Collect();
        }
    }

    private void Float(float deltaTime) {
        transform.position += Vector3.up * floatDirection * floatSpeed * deltaTime;
    }

    IEnumerator IEFloat() {
        while (true) {
            yield return new WaitForSeconds(1f);
            floatDirection *= -1f;
        }
    }

    private void Collect() {
        Book[] books = (Book[])GameObject.FindObjectsOfType(typeof(Book));
        for (int i = 0; i < books.Length; i++) {
            if (books[i].bookID == bookID) {
                books[i].pagesCollected += 1;
                WorldNoises.PlaySound(WorldNoises.Collect);

                if (!books[i].isInitialized) {
                    books[i].Init();
                }
            }
        }
        Destroy(gameObject);
    }

}
