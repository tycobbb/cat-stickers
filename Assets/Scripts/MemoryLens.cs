using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F = Fungus;
using UI = UnityEngine.UI;

public class MemoryLens: MonoBehaviour {
    // -- constants --
    private static Color kTransparent = new Color(1, 1, 1, 0);

    // -- props --
    private SpriteRenderer mMemory;
    private float mFadeDuration;
    private List<int> mStepsVisible;

    // -- commands --
    public void SetMemory(
        SpriteRenderer memory,
        // InvokeMethod can't use arrays
        int flash0 = 0,
        int flash1 = 0,
        int flash2 = 0,
        int flash3 = 0
    ) {
        // filter awful args
        List<int> stepsVisible = new List<int> { flash0, flash1, flash2, flash3 };
        stepsVisible.RemoveAll((step) => step == 0);

        // set memory and hide it
        mMemory = memory;
        mMemory.color = kTransparent;

        // animate it immediately if are no steps
        mStepsVisible = stepsVisible;
        if (mStepsVisible.Count == 0) {
            ShowMemory();
        }
    }

    public void SetStep(int step) {
        step += 1;

        // if we reached the last step, show the memory
        if (step == mStepsVisible[mStepsVisible.Count - 1]) {
            ShowMemory();
            return;
        }

        // otherwise, check if this is one of the visible steps and flash the
        // memory for a few frames
        var i = 0;
        foreach (int visible in mStepsVisible) {
            if (step == visible) {
                FlashMemory();
                break;
            }

            i++;
        }
    }

    public void SetFadeDuration(float duration) {
        mFadeDuration = duration;
    }

    // -- commands/helpers
    private void FlashMemory() {
        ShowMemory(() => {
            HideMemory();
        });
    }

    private void ShowMemory(Action callback = null) {
        AnimateMemory(Color.white, callback);
    }

    private void HideMemory(Action callback = null) {
        AnimateMemory(kTransparent, callback);
    }

    private void AnimateMemory(Color color, Action callback) {
        if (mMemory == null) {
            return;
        }

        F.SpriteFader.FadeSprite(mMemory, color, mFadeDuration, Vector2.zero, () => {
            if (callback != null) {
                callback();
            }
        });
    }
}
