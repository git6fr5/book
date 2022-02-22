/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collect to get more pages
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Coin : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public CircleCollider2D box;

    /* --- Parameters --- */
    [SerializeField] public int bookID;

    /* --- Unity --- */
    void Start() {
        Init();
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

    private void Collect() {
        Book[] books = (Book[])GameObject.FindObjectsOfType(typeof(Book));
        for (int i = 0; i < books.Length; i++) {
            if (books[i].bookID == bookID) {
                books[i].pagesCollected += 1;
                if (!books[i].isInitialized) {
                    books[i].Init();
                }
            }
        }
        Destroy(gameObject);
    }

}
