/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Trigger : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public BoxCollider2D box;
    [HideInInspector] public Monster monster;

    /* --- Unity --- */
    void Start() {
        Init();
    }
    
    void OnTriggerEnter2D(Collider2D collider) {
        monster.isActive = true;
    }

    void OnTriggerExit2D(Collider2D collider) {
        monster.isActive = false;
    }

    /* --- Methods --- */
    private void Init() {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;

        monster = transform.parent.GetComponent<Monster>();
    }

}
