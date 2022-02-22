using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Experimental.Rendering.Universal;

public class Torch : MonoBehaviour {

    [SerializeField, ReadOnly] float glowDirection = 1f;
    public float glowRate;
    public Light2D torchLight;

    // Start is called before the first frame update
    void Start() {

        StartCoroutine(IEBob());

        IEnumerator IEBob() {
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            while (true) {
                glowDirection = 1f + glowRate;
                yield return new WaitForSeconds(1.5f);
                glowDirection = 1f - glowRate;
                yield return new WaitForSeconds(1.5f);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        torchLight.pointLightOuterRadius = 0.85f + Random.Range(0f, 0.15f);
        // torchLight.pointLightOuterRadius *= glowDirection;
        torchLight.transform.localPosition = new Vector3(Random.Range(0f, 0.05f), Random.Range(0f, 0.05f), 0f);
    }

}
