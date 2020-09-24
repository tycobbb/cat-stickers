using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F = Fungus;
using UI = UnityEngine.UI;

public class MainMenu: F.MenuDialog {
    // -- constants --
    private float kDuration = 0.35f;
    private LeanTweenType kEasing = LeanTweenType.easeOutQuad;

    // -- props --
    private bool mFadeWhenDone = true;
    private bool mIsAwake = false;
    private float? mAnimationStart = null;

    // -- lifecycle --
    protected override void Awake() {
        base.Awake();
        mIsAwake = true;
    }

    protected virtual void Update() {
        if (IsAnimationComplete()) {
            StopAnimation();
        }
    }

    // -- overrides --
    public override void SetActive(bool isActive) {
        if (gameObject.activeSelf == isActive || IsAnimating()) {
            return;
        }

        // ignore SetActive(false) if preventing fade
        if (!isActive && !mFadeWhenDone) {
            return;
        }

        // short-circuit SetActive to instead set it alongside animation
        if (isActive) {
            StartAnimation(0.0f, 1.0f);
        } else {
            StartAnimation(1.0f, 0.0f);
        }
    }

    public override void SetButtonActive(UI.Button button, bool isActive) {
        // don't disable buttons when preventing menu fade to avoid UI flash
        if (!mFadeWhenDone && !isActive) {
            return;
        }

        base.SetButtonActive(button, isActive);
    }

    public override bool AddOption(string text, bool interactable, bool hideOption, Fungus.Block targetBlock) {
        var result = base.AddOption(text, interactable, hideOption, targetBlock);
        ShowDividers();
        return result;
    }

    public override void Clear() {
        // run the base implementation during setup
        if (!mIsAwake) {
            base.Clear();
            return;
        }

        // run the base implementation if no animation to bother with
        if (!mFadeWhenDone) {
            base.Clear();
            return;
        }

        // otherwise, short-circuit when animating out, since this sometimes gets called
        // before SetActive(false).
        if (gameObject.activeSelf && !IsAnimating()) {
            StartAnimation(1.0f, 0.0f);
            return;
        }

        base.Clear();
    }

    // -- commands --
    public void PreventFade() {
        mFadeWhenDone = false;
    }

    public void ResumeFade() {
        mFadeWhenDone = true;
    }

    private void ShowDividers() {
        var i = 0;
        var dividers = gameObject.GetComponentsInChildren<UI.Image>(true);

        // show any dividers between two active buttons (i.e. n-1 dividers) and
        // hide the rest
        foreach (var divider in dividers) {
            if (divider.name != "Divider") {
                continue;
            }

            var isVisible = i < DisplayedOptionsCount - 1;
            divider.gameObject.SetActive(isVisible);
            i++;
        }
    }

    // -- commands/animation
    private void StartAnimation(float from, float to) {
        if (to != 0.0f) {
            base.SetActive(true);
        }

        mAnimationStart = Time.time;
        var group = GetComponent<CanvasGroup>();
        group.alpha = from;
        LeanTween.alphaCanvas(group, new F.FloatData(to), new F.FloatData(kDuration)).setEase(kEasing);
    }

    private void StopAnimation() {
        mAnimationStart = null;

        var group = GetComponent<CanvasGroup>();
        if (group.alpha == 0.0f) {
            base.SetActive(false);
            base.Clear();
        }
    }

    // -- queries --
    private bool IsAnimating() {
        return mAnimationStart != null;
    }

    private bool IsAnimationComplete() {
        return IsAnimating() && Time.time - mAnimationStart >= kDuration;
    }
}
