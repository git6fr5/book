/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using LDtkTileData = World.LDtkTileData;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Tilemap))]
public class Page : MonoBehaviour {

    [System.Serializable]
    public struct PageObject {
        public GameObject gameObject;
        public Vector3 origin;

        public PageObject(GameObject gameObject, Vector3 origin) {
            this.gameObject = gameObject;
            this.origin = origin;

            this.gameObject.SetActive(false);
        }
    }

    /* --- Components --- */
    [SerializeField, HideInInspector] public Tilemap tilemap;
    [SerializeField] public Tilemap background;

    /* --- Properties --- */
    [SerializeField, ReadOnly] public int pageNumber;
    [SerializeField, ReadOnly] public List<PageObject> pageObjects = new List<PageObject>();
    [SerializeField, ReadOnly] public List<GameObject> currentObjects = new List<GameObject>();
    [SerializeField, ReadOnly] public bool isInitialized = false;
    [SerializeField, ReadOnly] public List<LDtkTileData> decorData = new List<LDtkTileData>();

    /* --- Unity --- */
    void Start() {
        if (!isInitialized) {
            Init();
        }
    }

    public void On(bool noDelay = true) {
        gameObject.SetActive(true);
        if (!isInitialized) {
            Init();
        }
        if (noDelay) {
            OnDelay();
        }
        else {
            foreach (Transform child in transform) {
                if (child.GetComponent<Coin>() != null || child.GetComponent<BreakableBlock>() != null) {
                    child.gameObject.SetActive(false);
                }
                if (child.GetComponent<Boulder>() != null) {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnDelay() {
        if (isInitialized && pageObjects != null) {
            CreateObjects();
        }
        foreach (Transform child in transform) {
            if (child.GetComponent<Coin>() != null || child.GetComponent<BreakableBlock>() != null) {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void Off() {
        if (isInitialized && pageObjects != null) {
            ClearObjects();
        }
        gameObject.SetActive(false);
    }

    /* --- Methods --- */
    public void Init() {
        tilemap = GetComponent<Tilemap>();

        CollectObjects();
        background.gameObject.SetActive(true);
        isInitialized = true;
    }

    void CollectObjects() {
        foreach (Transform child in transform) {
            bool exception = child.GetComponent<Coin>() != null || child.GetComponent<BreakableBlock>() != null || child.GetComponent<Tilemap>() != null;
            if (child.gameObject.activeSelf && !exception) {
                PageObject pageObject = new PageObject(child.gameObject, child.localPosition);
                pageObjects.Add(pageObject);
            }
        }
    }

    void CreateObjects() {
        for (int i = 0; i < pageObjects.Count; i++) {
            GameObject newObject = Instantiate(pageObjects[i].gameObject, pageObjects[i].origin, Quaternion.identity, transform);
            newObject.transform.localPosition = pageObjects[i].origin;
            newObject.SetActive(true);
            currentObjects.Add(newObject);
        }
    }

    public void ClearObjects() {
        if (currentObjects.Count <= 0) {
            print("No Objects");
            return;
        }
        for (int i = 0; i < currentObjects.Count; i++) {
            if (currentObjects[i] != null) {
                GameObject thisObject = currentObjects[i];
                thisObject.transform.SetParent(null);
                Destroy(thisObject);
            }
        }
        currentObjects = new List<GameObject>();
    }

    // Compare the depth of the meshes.
    public static int Compare(Page pageA, Page pageB) {
        return pageA.pageNumber.CompareTo(pageB.pageNumber);
    }

}
