/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Monster : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [SerializeField] public Projectile projectile;

    /* --- Parameters --- */
    [SerializeField] private float fireInterval;
    [SerializeField] public Vector2 direction;
    [SerializeField] private float speed;

    /* --- Properties --- */
    [SerializeField, ReadOnly] public bool isActive;
    [SerializeField, ReadOnly] private bool isFiring;

    /* --- Unity --- */
    void Start() {
        Init();
    }

    void Update() {
        if (isActive && !isFiring) {
            StartCoroutine(IEFire());
            isFiring = true;
        }
    }

    /* --- Methods --- */
    private void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FireProjectile() {
        Projectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity, null);
        newProjectile.Init(direction.normalized * speed);
    }

    /* --- Coroutines --- */
    private IEnumerator IEFire() {
        while (isActive) {
            FireProjectile();
            yield return new WaitForSeconds(fireInterval);
        }
        isFiring = false;
    }

}
