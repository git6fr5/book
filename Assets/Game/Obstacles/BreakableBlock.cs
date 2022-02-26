/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can be broken b
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class BreakableBlock : MonoBehaviour {

    /* --- Components --- */
    public bool _break;
    public bool isBroken;

    /* --- Properties --- */
    public List<BreakableBlock> group = new List<BreakableBlock>();

    void Update() {
        Group();

        if (_break) {
            Break();
        }
    }

    private void Group() {
        group = new List<BreakableBlock>();

        Check(Vector3.right);
        Check(Vector3.left);
        Check(Vector3.up);
        Check(Vector3.down);

    }

    private void Check(Vector3 direction) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction.normalized, 1f);
        for (int i = 0; i < hits.Length; i++) {
            BreakableBlock block = hits[i].collider.GetComponent<BreakableBlock>();
            if (block != null && block != this) {
                group.Add(block);
            }
        }
    }

    public void Break() {
        isBroken = true;
        for (int i = 0; i < group.Count; i++) {
            if (group[i] != null && !group[i].isBroken) {
                group[i].Break();
            }
        }
        Destroy(gameObject);
    }

}
