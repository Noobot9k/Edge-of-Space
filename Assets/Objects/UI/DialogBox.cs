using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour {

    public TMP_Text DialogTextBox;
    public Button ConfirmButton;
    public Button CancelButton;
    public RectTransform Window;

    public static DialogBox singleton;

    private void Start() {
        singleton = this;
    }

    public IEnumerator RunDialogBox(string msg, bool canCancel = false) {
        print("running dialog box");
        DialogTextBox.text = msg;
        CancelButton.gameObject.SetActive(canCancel);
        Window.gameObject.SetActive(true);

        bool Terminate = false;
        bool Response = true;

        void confirm() {
            Response = true;
            Terminate = true;
            print("confirm");
        }
        void cancel() {
            Response = false;
            Terminate = true;
            print("cancel");
        }

        ConfirmButton.onClick.AddListener(confirm);
        CancelButton.onClick.AddListener(cancel);
        print("subscribed");

        while(!Terminate) { yield return new WaitForEndOfFrame(); }

        print("finished");
        ConfirmButton.onClick.RemoveListener(confirm);
        CancelButton.onClick.RemoveListener(cancel);

        Window.gameObject.SetActive(false);

        yield return Response;
    }
}

public class CoroutineWithData {
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target) {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run() {
        while(target.MoveNext()) {
            result = target.Current;
            yield return result;
        }
    }
}
