using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class IOSelector : MonoBehaviour {

    public enum LoadType {
        Save,
        Open
    }
    public LoadType loadType;

    /* --- Components --- */
    public Stream stream;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        switch (loadType) {
            case LoadType.Save:
                stream.OnSave.Invoke(stream.text);
                break;
            case LoadType.Open:
                stream.OnOpen.Invoke(stream.text);
                break;
            default:
                break;
        }
    }

    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }
}
