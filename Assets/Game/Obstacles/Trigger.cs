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
    public Monster[] monsters;

    /* --- Unity --- */
    void Start() {
        Init();
    }
    
    void OnTriggerEnter2D(Collider2D collider) {
        monsters = (Monster[])GameObject.FindObjectsOfType(typeof(Monster));
        for (int i = 0; i < monsters.Length; i++) {
            monsters[i].isActive = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        for (int i = 0; i < monsters.Length; i++) {
            monsters[i].isActive = false;
        }
    }

    /* --- Methods --- */
    private void Init() {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;

    }

}
