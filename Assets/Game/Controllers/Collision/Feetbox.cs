/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Feetbox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected BoxCollider2D box;

    /* --- Properties --- */
    // A container to store all objects currently in contact with.
    [SerializeField] private List<Collider2D> container = new List<Collider2D>();

    /* --- Calls--- */
    public bool empty => container.Count == 0;

    /* --- Unity --- */
    private void Start() {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Add(collider);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        Remove(collider);
    }

    /* --- Virtual Methods --- */
    // Initializes the script.
    private void Init() {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    /* --- Methods --- */
    private void Add(Collider2D collider) {
        if (!container.Contains(collider) && collider.tag == GameRules.GroundTag) {
            container.Add(collider);
        }
    }

    private void Remove(Collider2D collider) {
        if (container.Contains(collider)) {
            container.Remove(collider);
        }
    }

}
