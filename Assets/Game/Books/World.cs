/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class World : MonoBehaviour {

    /* --- Static Variables --- */
    // Layer Names
    public static string ControlLayer = "Control";
    public static string DecorLayer = "Decor";
    public static string ObstacleLayer = "Obstacles";
    public static string GroundLayer = "Ground";

    /* --- Data Structures --- */
    public class LDtkTileData {

        /* --- Properties --- */
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public int index;

        /* --- Constructor --- */
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
        }

    }

    /* --- Components --- */
    [SerializeField] public LDtkComponentProject lDtkData;
    [HideInInspector] public LdtkJson json;
    [SerializeField] public Book[] books;
    [SerializeField] public Page page;

    /* --- Unity --- */
    void Awake() {
        json = lDtkData.FromJson();
        OpenAllPages();
    }

    /* --- Methods --- */
    void OpenAllPages() {

        // Get the json file from the LDtk Data.
        for (int i = 0; i < json.Levels.Length; i++) {
            string[] splicedId = json.Levels[i].Identifier.Split('_');
            int bookID = int.Parse(splicedId[1]) - 1;
            Book book = GetBook(bookID);
            if (book != null) {
                int pageNumber = int.Parse(splicedId[3]);
                print(json.Levels[i].Identifier);
                LDtkLevel lDtkLevel = json.Levels[i];
                OpenPage(book, pageNumber, lDtkLevel);
            }
        }
    }

    public Book GetBook(int id) {
        for (int i = 0; i < books.Length; i++) {
            if (books[i].bookID == id) {
                return books[i];
            }
        }
        return null;
    }

    public void OpenPage(Book book) {
        LDtkLevel ldtkPage = GetPage(book.bookID, book.pageNumber);
        OpenPage(book, book.pageNumber, ldtkPage);
    }

    protected LDtkLevel GetPage(int bookID, int pageNumber) {
        // Get the json file from the LDtk Data.
        string identifier = "Book_" + bookID.ToString() + "Book_" + pageNumber.ToString();
        for (int i = 0; i < json.Levels.Length; i++) {
            // print(json.Levels[i].Identifier);
            if (json.Levels[i].Identifier == identifier) {
                return json.Levels[i];
            }
        }
        return null;
    }

    protected void OpenPage(Book book, int pageNumber, LDtkLevel ldtkLevel) {

        Page page = Instantiate(this.page, transform.position, Quaternion.identity, book.transform);
        page.tilemap = page.GetComponent<Tilemap>();
        page.pageNumber = pageNumber;

        if (ldtkLevel != null) {

            // Load the entity data.
            List<LDtkTileData> obstacleData = LoadLayer(ldtkLevel, ObstacleLayer, GameRules.GridSize);
            List<LDtkTileData> groundData = LoadLayer(ldtkLevel, GroundLayer, GameRules.GridSize);

            // Instatiantate and set up the entities using the data.
            book.FindEntities(book.obstacleParentTransform, ref book.obstacles);
            LoadEntities(book, page, obstacleData, book.obstacles);
            LoadTiles(book, page, groundData);
        }

    }

    private LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    // Returns the vector ID's of all the tiles in the layer.
    protected List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, List<LDtkTileData> layerData = null) {

        if (layerData == null) { layerData = new List<LDtkTileData>(); }

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            // Itterate through the tiles in the layer and get the directions at each position.
            for (int index = 0; index < layer.GridTiles.Length; index++) {

                // Get the tile at this index.
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / gridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / gridSize;

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(vectorID, gridPosition, index);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    private List<Entity> LoadEntities(Book book, Page page, List<LDtkTileData> entityData, List<Entity> entityList) {

        List<Entity> entities = new List<Entity>();

        for (int i = 0; i < entityData.Count; i++) {
            // Get the entity based on the environment.
            Entity entityBase = GetEntityByVectorID(entityData[i].vectorID, entityList);
            if (entityBase != null) {

                // Instantiate the entity
                Vector3 entityPosition = book.GridToWorldPosition(entityData[i].gridPosition);
                Entity newEntity = Instantiate(entityBase.gameObject, entityPosition, Quaternion.identity, page.transform).GetComponent<Entity>();
                newEntity.transform.localPosition = entityPosition;

                // Set up the entity.
                newEntity.gameObject.SetActive(true);
                newEntity.gridPosition = entityData[i].gridPosition;

                // Add the entity to the list
                entities.Add(newEntity);
            }
        }

        return entities;
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

    // Set all the tiles in a tilemap.
    private List<Vector3Int> LoadTiles(Book book, Page page, List<LDtkTileData> data) {
        List<Vector3Int> nonEmptyTiles = new List<Vector3Int>();
        for (int i = 0; i < data.Count; i++) {
            Vector3Int tilePosition = book.GridToTilePosition(data[i].gridPosition);
            page.tilemap.SetTile(tilePosition, book.tile);
            nonEmptyTiles.Add(tilePosition);
        }

        return nonEmptyTiles;
    }

    // 
    private Vector2Int? GetTileID(List<LDtkTileData> data, Vector2Int gridPosition) {
        for (int i = 0; i < data.Count; i++) {
            if (gridPosition == data[i].gridPosition) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }

}
