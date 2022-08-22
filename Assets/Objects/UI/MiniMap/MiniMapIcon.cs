using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIcon : MonoBehaviour {

    public bool MimicRotation = true;
    public bool MimicScale = true;
    public Transform Subject;

    private void Update() {
        if(!Subject) { GameObject.Destroy(gameObject); return; }

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(Subject.position.x, Subject.position.z) * MiniMap.singleton.Container.rect.height / MiniMap.singleton.MapScale;
        if (MimicScale)
            rectTransform.sizeDelta = Vector2.one * Mathf.Max(Subject.lossyScale.x, 8) * MiniMap.singleton.Container.rect.height / MiniMap.singleton.MapScale;
        if (MimicRotation)
            rectTransform.localRotation = Quaternion.Euler(0,0, -Subject.rotation.eulerAngles.y - 90);
    }

}
