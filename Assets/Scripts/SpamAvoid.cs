using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI = UnityEngine.UI;

public class SpamAvoid: MonoBehaviour {
    // -- constants --
    private const int kSpawnInterval = 1;
    private const int kSpawnGroup = 5 * kSpawnInterval;
    private const int kSpawnMax = 100 * kSpawnInterval;

    // -- fields --
    [Tooltip("The actual menu")]
    [SerializeField]
    protected GameObject fMenu;

    // -- props --
    private int mFrame = 0;
    private GameObject mPrototype;

    // -- lifecycle --
    protected void OnEnable() {
        var prototype = Object.Instantiate(fMenu, transform);
        prototype.name = "AvoidButton";

        // remove extraneous components
        Destroy(prototype.GetComponent<MainMenu>());
        Destroy(prototype.GetComponent<Fungus.Character>());

        // destroy extraneous children
        var selectables = prototype.GetComponentsInChildren<UI.Selectable>();
        if (selectables == null) {
            return;
        }

        foreach (var selectable in selectables) {
            Destroy(selectable.gameObject);
        }

        var buttons = prototype.GetComponentsInChildren<UI.Button>(true);
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
        var canvas = prototype.GetComponent<Canvas>();
        canvas.sortingOrder = -2;

        // set the button text/style
        var button = buttons[0];
        button.gameObject.SetActive(true);
        button.GetComponentInChildren<UI.Text>().text = "Avoid";
        Destroy(button.GetComponentInChildren<UI.Image>().gameObject);

        // show menu
        mPrototype = prototype;
    }

    protected void Update() {
        if (mFrame >= kSpawnMax) {
            return;
        }

        mFrame++;
        if (mFrame % kSpawnInterval != 0) {
            return;
        }

        // create a new menu
        var spawned = Object.Instantiate(mPrototype, transform);

        // assign it a random position based on frame interval
        var group = mFrame / kSpawnGroup + 1;
        var child = spawned.transform.Find("ButtonRow").GetComponent<RectTransform>();
        var point = Random.insideUnitCircle;
        child.anchoredPosition = point.normalized * group * 60.0f + point * group * 40.0f;
    }

    protected void OnDisable() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
}
