/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    /* --- Components --- */
    public MeshSprite[] spritesheets;
    public int activeID;

    /* --- Parameters --- */

    /* --- Properties --- */
    [SerializeField, ReadOnly] int coins = 0;
    [SerializeField, ReadOnly] private KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] private KeyCode actionKey = KeyCode.J; // The key used to perform the action.

    /* --- Overridden Methods --- */
    protected override void Init() {
        base.Init();
        SetActiveBook(0);
    }

    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.
        
        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Get the jump.
        if (Input.GetKeyDown(jumpKey)) {
            jump = true;
        }
        if (Input.GetKey(jumpKey) && airborneFlag == Airborne.Rising) {
            weight *= floatiness;
        }

        // Get the action.
        if (Input.GetKeyDown(actionKey)) {
            action = true;
        }

    }

    /* --- Overridden Events --- */
    // Performs an action.
    protected override void Action() {
        base.Action(); // Runs the base action.
    }

    public void CollectCoin() {
        coins += 1;
    }

    public void SetActiveBook(int bookID) {
        spritesheets[activeID].gameObject.SetActive(false);
        spritesheets[bookID].gameObject.SetActive(true);
        activeID = bookID;
    }

}
