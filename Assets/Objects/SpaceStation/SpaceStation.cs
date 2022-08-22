using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceStation : MonoBehaviour {

    public float InteractionCooldown = 10;

    float _lastInteractTick = 0;

    private void OnCollisionEnter(Collision collision) {
        PlayerHandler player = collision.gameObject.GetComponent<PlayerHandler>();
        if(!player) return;

        if(Time.time < _lastInteractTick + InteractionCooldown) return;
        _lastInteractTick = Time.time;


        // win
        if(StarmapHandler.singleton.CurrentStarSystem.IsVictoryStation) {
            IEnumerator win() {
                yield return DialogBox.singleton.RunDialogBox("You made it to safety. The people on this station are about to be taken hundreds of light-years away from danger and now you're with them. You win.");
                SceneManager.LoadScene(0);

            }
            StartCoroutine(win());
        } else {
            // save
            SaveManager.SavePlayerData(StarmapHandler.singleton.StarmapSeed, StarmapHandler.singleton.GetStarIndex(StarmapHandler.singleton.CurrentStarSystem), PlayerHandler.singleton);
            StartCoroutine(DialogBox.singleton.RunDialogBox("The game as been saved."));
        }
    }

}
