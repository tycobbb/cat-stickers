using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F = Fungus;
using UI = UnityEngine.UI;

public class MainMenu: F.MenuDialog {
    // -- constants --
    private const LeanTweenType kEasing = LeanTweenType.easeOutQuad;

    // -- field --
    [Tooltip("The fade duration")]
    [SerializeField]
    private float fFadeDuration = 0.35f;

    [Tooltip("The click sound")]
    [SerializeField]
    private AudioClip fSound;

    // -- props --
    private bool mIsAwake = false;
    private bool mIsVisibilityLocked = false;
    private bool mIsBecomingActive = false;
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
        // ignore redudant calls to SetActive
        if (IsAnimatingToActive(isActive)) {
            return;
        }

        // force the buttons to clear if something is interrupting the hide
        // animation (e.g. a new menu in debug mode)
        if (IsAnimatingToActive(false) && isActive) {
            base.Clear();
        }

        // if visibility is locked, ignore SetActive(false)
        if (mIsVisibilityLocked && !isActive) {
            return;
        }

        // if visibility is locked, ignore redudant calls to SetActive
        if (mIsVisibilityLocked && gameObject.active == isActive) {
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
        // if visibility is locked, don't disable buttons to prevent flicker
        if (mIsVisibilityLocked && !isActive) {
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

        // if visibility is locked there are no animations, so run the base implementation
        if (mIsVisibilityLocked) {
            base.Clear();
            return;
        }

        // otherwise, if active short-circuit to animate out, since this sometimes gets called
        // before SetActive(false).
        if (gameObject.activeSelf && !IsAnimating()) {
            StartAnimation(1.0f, 0.0f);
            return;
        }

        base.Clear();
    }

    // -- overrides/helpers
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

    // -- commands --
    public void SetFadeDuration(float duration) {
        fFadeDuration = duration;
    }

    public void LockVisibility() {
        mIsVisibilityLocked = true;
    }

    public void UnlockVisibility() {
        mIsVisibilityLocked = false;
    }

    public void PlaySound() {
        var audio = F.FungusManager.Instance.MusicManager;
        audio.PlaySound(fSound, 1.0f);
    }

    // -- commands/animation
    private void StartAnimation(float from, float to) {
        mIsBecomingActive = to != 0.0f;

        if (mIsBecomingActive) {
            base.SetActive(true);
        }

        mAnimationStart = Time.time;
        var group = GetComponent<CanvasGroup>();
        group.alpha = from;
        LeanTween.alphaCanvas(group, new F.FloatData(to), new F.FloatData(fFadeDuration)).setEase(kEasing);
    }

    private void StopAnimation() {
        mAnimationStart = null;

        if (!mIsBecomingActive) {
            base.SetActive(false);
            base.Clear();
        }
    }

    // -- queries --
    private bool IsAnimating() {
        return mAnimationStart != null;
    }

    private bool IsAnimatingToActive(bool isActive) {
        return IsAnimating() && mIsBecomingActive == isActive;
    }

    private bool IsAnimationComplete() {
        return IsAnimating() && Time.time - mAnimationStart >= fFadeDuration;
    }
}
