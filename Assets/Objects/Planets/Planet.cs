using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public int Form = -1;

    public List<GameObject> visualForms = new List<GameObject>();

    public void Start() {
        if(Form == -1) Debug.LogWarning("planet was not given a form."); //SetVisualForm(Random.Range(1, visualForms.Count));
    }

    public void SetVisualForm(int index) {
        Form = index;
        foreach(GameObject form in visualForms) {
            form.SetActive(false);
        }
        visualForms[index].SetActive(true);
    }

}
