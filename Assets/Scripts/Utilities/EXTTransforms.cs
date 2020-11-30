using UnityEngine;

public static class Transforms {
    public static void DestroyChildren (this Transform t, bool destroyImmedietly = false) {
        foreach (Transform child in t) {
            if (destroyImmedietly) {
                MonoBehaviour.DestroyImmediate (child.gameObject);
            } else {
                MonoBehaviour.Destroy (child.gameObject);
            }
        }
    }
}