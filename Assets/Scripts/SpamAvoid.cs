using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI = UnityEngine.UI;

public class SpamAvoid: MonoBehaviour {
    // -- constants --
    private const int kLastGeneration = 4;
    private const int kGenerationBase = 3;

    // -- fields --
    [Tooltip("The actual menu")]
    [SerializeField]
    protected GameObject fMenu;

    [Tooltip("The message on send on completion")]
    [SerializeField]
    protected string fOnComplete;

    // -- props --
    private GameObject mPrefab;
    private int mGeneration = 0;
    private bool mIsComplete = false;

    // -- lifecycle --
    protected void OnEnable() {
        var prefab = Object.Instantiate(fMenu, transform);
        prefab.SetActive(false);
        prefab.name = "AvoidButton";

        // remove extraneous components
        Destroy(prefab.GetComponent<MainMenu>());
        Destroy(prefab.GetComponent<Fungus.Character>());

        // destroy extraneous children
        var selectables = prefab.GetComponentsInChildren<UI.Selectable>();
        if (selectables == null) {
            return;
        }

        foreach (var selectable in selectables) {
            Destroy(selectable.gameObject);
        }

        var buttons = prefab.GetComponentsInChildren<UI.Button>(true);
        if (buttons == null) {
            return;
        }

        var i = 0;
        foreach (var b in buttons) {
            if (i++ != 0) {
                Destroy(b.gameObject);
            }
        }

        // show the menu at the right layer
        var canvas = prefab.GetComponent<Canvas>();
        canvas.sortingOrder = -2;

        // set the button text/style
        var button = buttons[0];
        button.gameObject.SetActive(true);
        button.GetComponentInChildren<UI.Text>().text = "Avoid";
        Destroy(button.GetComponentInChildren<UI.Image>().gameObject);

        // store the prefab
        mPrefab = prefab;

        // create the first generation
        StartCoroutine(SpawnInitialButton());
    }

    protected void OnDisable() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    // -- commands --
    private IEnumerator SpawnInitialButton() {
        yield return 0;
        StartCoroutine(SpawnButtons());
    }

    private IEnumerator SpawnButtons() {
        var generation = mGeneration++;
        var count = System.Math.Pow(kGenerationBase, generation);
        // var count = System.Math.Max(1, generation * kGenerationScale);

        for (var i = 0; i < count; i++) {
            var spawned = Object.Instantiate(mPrefab, transform);

            // assign it a random position based on generation
            var child = spawned.transform.Find("ButtonRow").GetComponent<RectTransform>();
            var point = Random.insideUnitCircle;
            child.anchoredPosition = new Vector2(point.x * 960.0f, point.y * 540.0f);

            // create buttons in the next generation on click
            var button = spawned.GetComponentInChildren<UI.Button>();
            button.onClick.AddListener(this.DidClickButton(spawned));

            spawned.SetActive(true);

            yield return 0;
        }
    }

    // -- events --
    private UnityAction DidClickButton(GameObject spawned) {
        return () => {
            Destroy(spawned);

            if (mGeneration <= kLastGeneration) {
                StartCoroutine(SpawnButtons());
            } else {
                DidClickTerminalButton();
            }
        };
    }

    private void DidClickTerminalButton() {
        if (mIsComplete) {
            return;
        }

        mIsComplete = true;
        Fungus.Flowchart.BroadcastFungusMessage(fOnComplete);
    }
}
