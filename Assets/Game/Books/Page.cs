/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    /* --- Properties --- */
    [SerializeField, ReadOnly] public int pageNumber;
    [SerializeField, ReadOnly] public List<PageObject> pageObjects = new List<PageObject>();
    [SerializeField, ReadOnly] public List<GameObject> currentObjects = new List<GameObject>();
    [SerializeField, ReadOnly] public bool isInitialized = false;

    /* --- Unity --- */
    void Start() {
        if (!isInitialized) {
            Init();
        }
    }

    public void On() {
        gameObject.SetActive(true);
        if (!isInitialized) {
            Init();
        }
        if (isInitialized && pageObjects != null) {
            CreateObjects();
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
        isInitialized = true;
    }

    void CollectObjects() {
        foreach (Transform child in transform) {
            if (child.gameObject.activeSelf) {
                PageObject pageObject = new PageObject(child.gameObject, child.position);
                pageObjects.Add(pageObject);
            }
        }
    }

    void CreateObjects() {
        for (int i = 0; i < pageObjects.Count; i++) {
            GameObject newObject = Instantiate(pageObjects[i].gameObject, pageObjects[i].origin, Quaternion.identity, transform);
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
