/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour {

    /* --- Components --- */
    [SerializeField] public Controller controller;
    [HideInInspector] public Collider2D box;

    /* --- Unity --- */
    void Start() {
        Init();
    }

    /* --- Methods --- */
    public void Init() {
        box = GetComponent<Collider2D>();
        box.isTrigger = true;
    }

}
