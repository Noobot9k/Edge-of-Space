using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtention {
    public static Transform ClearAllChildren(this Transform transform) {
        foreach(Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }
}
