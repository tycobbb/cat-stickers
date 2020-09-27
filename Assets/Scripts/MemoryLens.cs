using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryLens: MonoBehaviour {
    // -- fields --
    [Tooltip("The actual menu")]
    [SerializeField]
    protected SpriteRenderer fMemory;

    // -- commands --
    public void SetMemory(SpriteRenderer memory) {
        fMemory = memory;
        SetPercentComplete(0.0f);
    }

    public void SetPercentComplete(float percent) {
        fMemory.color = new Color(1, 1, 1, percent);
    }
}
