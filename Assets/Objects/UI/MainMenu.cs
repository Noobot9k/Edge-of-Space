using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void Continue() {
        SceneManager.LoadScene(1);
    }
    IEnumerator newGame() {
        CoroutineWithData coroutineData = new CoroutineWithData(this, DialogBox.singleton.RunDialogBox("This will delete any existing save data. Are you sure this is what you want?", true));
        yield return coroutineData.coroutine;
        if(!(bool)coroutineData.result) yield break;

        //DialogBox.singleton.RunDialogBox("This will delete any existing save data. Are you sure this is what you want?");
        SaveManager.DeletePlayerData();
        SceneManager.LoadScene(1);
    }
    public void NewGame() {
        StartCoroutine(newGame());
    }

}
