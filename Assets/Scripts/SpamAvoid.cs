﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using F = Fungus;
using UI = UnityEngine.UI;

public class SpamAvoid: MonoBehaviour {
    // -- constants --
    private static int[] kSpawnCountByGeneration = new int[] {
        1,
        4,
        16,
        40
    };

    // -- fields --
    [Tooltip("The actual menu")]
    [SerializeField]
    protected GameObject fMenu;

    [Tooltip("Sound effect to play on spam")]
    [SerializeField]
    protected AudioClip fSound;

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
        mGeneration = 0;
        StartCoroutine(SpawnButtons());
    }

    private IEnumerator SpawnButtons() {
        var count = kSpawnCountByGeneration[mGeneration];
        var generation = mGeneration++;

        for (var i = 0; i < count; i++) {
            var spawned = Object.Instantiate(mPrefab, transform);

            // assign it a random position based on generation
            var child = spawned.transform.Find("ButtonRow").GetComponent<RectTransform>();
            var point = Random.insideUnitCircle;
            child.anchoredPosition = new Vector2(point.x * 960.0f, point.y * 540.0f);

            // create buttons in the next generation on click
            var button = spawned.GetComponentInChildren<UI.Button>();
            button.onClick.AddListener(this.DidClickButton(spawned));

            // show button
            spawned.SetActive(true);

            // play sound
            var audio = F.FungusManager.Instance.MusicManager;
            audio.PlaySound(fSound, 1.0f);

            // wait a few frames between spawns
            var wait = generation < 2 ? 6 : 3;
            for (var j = 0; j < wait; j++) {
                yield return 0;
            }
        }
    }

    // -- events --
    private UnityAction DidClickButton(GameObject spawned) {
        return () => {
            Destroy(spawned);

            if (mGeneration > LastGeneration()) {
                DidClickTerminalButton();
            } else {
                StartCoroutine(SpawnButtons());
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

    // -- queries --
    private int LastGeneration() {
        return kSpawnCountByGeneration.Length - 1;
    }
}
