/* --- Modules --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Associates a set of textures with the corresponding inputs.
/// </summary>
public class Ascii : MonoBehaviour {
    
    /* --- Components --- */
    public Sprite[] sprites;

    /* --- Variables --- */
    [HideInInspector] public Dictionary<char, Sprite> parser;
    [SerializeField] [ReadOnly] private string input = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";


    /* --- Unity --- */
    protected virtual void Awake() {
        // Set the dictionary
        parser = new Dictionary<char, Sprite>();
        int length = (int)Mathf.Min(input.Length, sprites.Length);
        for (int i = 0; i < length; i++) {
            parser.Add(input[i], sprites[i]);
        }
    }

}
