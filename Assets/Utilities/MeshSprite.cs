/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class MeshSprite : MonoBehaviour {

    /* --- Components --- */
    public Mesh mesh;
    public Sprite[] sprites;

    /* --- Parameters --- */
    public int offset;
    public int idleLength;
    public int moveLength;
    public int jumpLength;
    public int fallLength;
    public int landLength;
    public int preActionLength;
    public int actionLength;
    public int postActionLength;

    /* --- Unity --- */
    private void Start() {
        Organize();
    }

    /* --- Methods --- */
    public void Organize() {
        print("Organize");

        int startIndex = SliceSprite(offset, idleLength, ref mesh.idle);
        startIndex = SliceSprite(startIndex, moveLength, ref mesh.move);

        startIndex = SliceSprite(startIndex, jumpLength, ref mesh.jumpRising);
        startIndex = SliceSprite(startIndex, fallLength, ref mesh.jumpFalling);
        startIndex = SliceSprite(startIndex, landLength, ref mesh.postJump);

        startIndex = SliceSprite(startIndex, preActionLength, ref mesh.preAction);
        startIndex = SliceSprite(startIndex, actionLength, ref mesh.action);
        startIndex = SliceSprite(startIndex, postActionLength, ref mesh.postAction);

    }

    private int SliceSprite(int startIndex, int length, ref Sprite[] array) {
        List<Sprite> splicedSprites = new List<Sprite>();
        for (int i = startIndex; i < startIndex + length; i++) {
            splicedSprites.Add(sprites[i]);
        }
        array = splicedSprites.ToArray();
        return startIndex + length;
    }

}
