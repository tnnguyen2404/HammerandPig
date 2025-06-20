using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPieces : MonoBehaviour
{
    void Start() {
        StartCoroutine(DestroyGameObjectAfterDelay(Random.Range(3, 5)));
    }

    IEnumerator DestroyGameObjectAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
