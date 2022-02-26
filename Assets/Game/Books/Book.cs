using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

[RequireComponent(typeof(BoxCollider2D))]
public class Book : MonoBehaviour {

    /* --- Static Variables --- */
    public static int Width = 24;
    public static int Height = 16;
    public static float FlipInterval = 0.3f;

    /* --- Components --- */
    [HideInInspector] public BoxCollider2D box;
    [SerializeField] public Transform obstacleParentTransform;
    [SerializeField] public Counter counter;
    [SerializeField] public GameObject cover;

    /* --- Parameters --- */
    [SerializeField] public RuleTile tile;
    [SerializeField] public RuleTile background;
    [SerializeField] public Decoration decor;
    [SerializeField] public List<GameObject> decorations;

    [SerializeField] public int bookID;
    [SerializeField] public Vector2Int bookPosition;

    /* --- Switches --- */
    [SerializeField] public bool nextPage;
    [SerializeField] public bool prevPage;

    /* --- Properties --- */
    [SerializeField, ReadOnly] public List<Entity> obstacles;
    [SerializeField, ReadOnly] public Page[] pages;
    [SerializeField, ReadOnly] public int pageNumber;
    [SerializeField, ReadOnly] public int pagesCollected;
    [SerializeField, ReadOnly] public bool isActive;
    [SerializeField, ReadOnly] private bool isSelected;
    [SerializeField, ReadOnly] public bool isInitialized;
    [SerializeField, ReadOnly] public bool flippingPage;
    [SerializeField, ReadOnly] public Vector3 flipDirection;

    /* --- Unity --- */
    void Start() {
        Init();
    }

    void Update() {
        Flip();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        ProccessCollision(collider, true);
    }

    void OnTriggerExit2D(Collider2D collider) {
        ProccessCollision(collider, false);
    }

    void OnMouseEnter() {
        isSelected = true;
    }

    void OnMouseExit() {
        isSelected = false;
    }

    /* --- Methods --- */
    public void Init() {

        List<Page> L_Pages = new List<Page>();
        foreach (Transform child in transform) {
            if (child.GetComponent<Page>() != null) {
                L_Pages.Add(child.GetComponent<Page>());
            }
        }
        pages = L_Pages.ToArray();
        Array.Sort<Page>(pages, new Comparison<Page>((pageA, pageB) => Page.Compare(pageA, pageB)));

        if (pagesCollected == 0) {
            Deinit();
            return;
        }

        if (cover != null) {
            cover.SetActive(false);
        }

        for (int i = 0; i < pages.Length; i++) {
            pages[i].gameObject.SetActive(false);
        }

        pageNumber = 0;
        pages[0].On();
        World.LoadDecor(this, pages[pageNumber].decorData);

        box = GetComponent<BoxCollider2D>();
        box.enabled = true;
        box.size = new Vector2(Width, Height);
        box.offset = new Vector2(bookPosition.x * Width / 2f, bookPosition.y * Height / 2f);
        box.isTrigger = true;

        tag = GameRules.BookTag;
        isInitialized = true;
    }

    private void Deinit() {

        if (cover != null) {
            cover.SetActive(true);
        }
        for (int i = 0; i < pages.Length; i++) {
            pages[i].gameObject.SetActive(false);
        }

        box = GetComponent<BoxCollider2D>();
        box = GetComponent<BoxCollider2D>();
        box.enabled = true;
        box.size = new Vector2(Width, Height);
        box.offset = new Vector2(bookPosition.x * Width / 2f, bookPosition.y * Height / 2f);
        box.isTrigger = false;
        tag = GameRules.GroundTag;
    }

    private void Flip() {

        if (counter != null) {
            counter.transform.SetParent(null);
            counter.SetScore(pageNumber, pagesCollected, pages.Length);
        }

        if (pagesCollected == 0) {
            return;
        }

        if (flippingPage) {
            transform.position += flipDirection * 32f * Time.deltaTime / FlipInterval;
        }

        if (nextPage) {
            NextPage();
            nextPage = false;
        }

        if (prevPage) {
            PrevPage();
            prevPage = false;
        }

        if (!isActive && isSelected) {
            pages[pageNumber].tilemap.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else {
            pages[pageNumber].tilemap.color = new Color(1f, 1f, 1f);
        }

        nextPage = Input.GetMouseButtonDown(0) && !isActive && isSelected;
        prevPage = Input.GetMouseButtonDown(1) && !isActive && isSelected;
    }

    private void ProccessCollision(Collider2D collider, bool enable) {
        if (collider.GetComponent<Hurtbox>()?.controller == GameRules.MainPlayer) {
            // WorldNoises.PlaySound(WorldNoises.ChangeBooks);
            isActive = enable;
        }
        // For right now, it is just boulders and projectiles...
        else if (collider.GetComponent<Boulder>() || collider.GetComponent<Projectile>()) {
            if (enable) {
                collider.transform.parent = pages[pageNumber].transform;
                if (!pages[pageNumber].currentObjects.Contains(collider.gameObject)) {
                    pages[pageNumber].currentObjects.Add(collider.gameObject);
                }
            }
            else if (pages[pageNumber].currentObjects.Contains(collider.gameObject)) {
                pages[pageNumber].currentObjects.Remove(collider.gameObject);

            }
        }
    }

    private void NextPage() {
        if (flippingPage) {
            return;
        }
        if (pageNumber + 1 >= Mathf.Min(pages.Length, pagesCollected)) {
            return; // pageNumber = 0;
        }

        flippingPage = true;
        StartCoroutine(IEFlipPage(1));
        
    }

    private void PrevPage() {
        if (flippingPage) {
            return;
        }
        if (pageNumber - 1 < 0) {
            return; // pageNumber = Mathf.Min(pages.Length, pagesCollected) - 1;
        }

        flippingPage = true;
        StartCoroutine(IEFlipPage(-1));
        

    }

    private IEnumerator IEFlipPage(int index) {
        WorldNoises.PlaySound(WorldNoises.ChangePages);

        box.isTrigger = false;

        for (int i = 0; i < decorations.Count; i++) {
            decorations[i].SetActive(false);
        }

        if (index > 0) {
            flipDirection = new Vector3(bookPosition.x, 0f, 0f);
        }
        else {
            flipDirection = new Vector3(0f, bookPosition.y, 0f);
        }
        yield return new WaitForSeconds(FlipInterval);
        if (index > 0) {
            transform.position = new Vector3(0f, bookPosition.y * 32f, 0f);
            flipDirection = new Vector3(0f, -bookPosition.y, 0f);
        }
        else {
            transform.position = new Vector3(bookPosition.x * 32f, 0f, 0f);
            flipDirection = new Vector3(-bookPosition.x, 0f, 0f);
        }
        pages[pageNumber].Off();
        pageNumber += index;
        pages[pageNumber].On(false);
        yield return new WaitForSeconds(FlipInterval);
        pages[pageNumber].OnDelay();
        World.LoadDecor(this, pages[pageNumber].decorData);
        flippingPage = false;
        transform.position = Vector3.zero;

        box.isTrigger = true;

        yield return null;

    }

    /* --- External --- */
    public Vector3 GridToWorldPosition(Vector2Int gridPosition) {
        int x = bookPosition.x == -1 ? -1 : 0;
        int y = bookPosition.y == -1 ? 0 : 1;
        Vector3 offset = new Vector3(x * Width, y * Height - 1, 0f);
        return new Vector3(gridPosition.x + 0.5f, -gridPosition.y + 0.5f, 0f) + offset;
    }

    public Vector3Int GridToTilePosition(Vector2Int gridPosition) {
        int x = (int)(bookPosition.x / 2f - 0.5f);
        int y = (int)(bookPosition.y / 2f + 0.5f);
        Vector3Int offset = new Vector3Int(x * Width, y * Height - 1, 0);
        return new Vector3Int(gridPosition.x, -gridPosition.y, 0) + offset;
    }

    public void FindEntities(Transform parent, ref List<Entity> entityList) {
        entityList = new List<Entity>();
        foreach (Transform child in parent) {
            FindAllEntitiesInTransform(child, ref entityList);
        }
    }

    // Recursively searches through the transform for all entity components.
    void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {

        // If we've found an entity, don't go any deeper.
        if (parent.GetComponent<Entity>() != null) {
            entityList.Add(parent.GetComponent<Entity>());
        }
        else if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child, ref entityList);
            }
        }
    }

    // Returns the first found entity with a matching ID.
    public Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
        for (int i = 0; i < entityList.Count; i++) {
            if (entityList[i].vectorID == vectorID) {
                return entityList[i];
            }
        }
        return null;
    }

}
