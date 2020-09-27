using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F = Fungus;
using UI = UnityEngine.UI;

public class MemoryLens: MonoBehaviour {
    // -- props --
    private SpriteRenderer mMemory;
    private float mFadeDuration = 0.0f;
    private bool mIsAnimating = false;

    // -- commands --
    public void SetMemory(SpriteRenderer memory, float fadeDuration, bool shouldAnimateLens = true) {
        mMemory = memory;
        mFadeDuration = fadeDuration;
        SetPercentComplete(0.0f, shouldAnimateLens);
    }

    public void SetPercentComplete(float percent, bool shouldAnimateLens = true) {
        // stepwise fade the color block
        if (shouldAnimateLens) {
            SetLensAlpha(Mathf.Pow(percent, 2));
        }

        // hide the memory until complete
        if (percent != 1.0f) {
            HideMemory();
        } else {
            ShowMemory();
        }
    }

    // -- commands/helpers
    private void SetLensAlpha(float alpha) {
        Debug.Log("setting lens alpha");
        var canvas = GetComponent<CanvasGroup>();
        canvas.alpha = alpha;
    }

    private void HideMemory() {
        if (mMemory == null) {
            return;
        }

        mMemory.color = new Color(1, 1, 1, 0);
    }

    private void ShowMemory() {
        if (mMemory == null || mIsAnimating) {
            return;
        }

        mIsAnimating = true;
        F.SpriteFader.FadeSprite(mMemory, Color.white, mFadeDuration, Vector2.zero, () => {
            mIsAnimating = false;
            SetLensAlpha(0.0f);
        });
    }
}
