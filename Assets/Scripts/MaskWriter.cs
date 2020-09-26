using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UI = UnityEngine.UI;

class MaskWriter: Fungus.Writer {
    // -- constants --
    private static Regex kPattern = new Regex(@"\w", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // -- props --
    private bool mIsListening = true;
    private string mLastStartText = "";
    private StringBuilder mMaskedString = new StringBuilder(1024);

    // -- overrides --
    protected override void PartitionString(bool isWritingWholeWords, string input, int i) {
        base.PartitionString(isWritingWholeWords, input, i);

        // mask out the input string for when we're not listening
        var masked = kPattern.Replace(input, "•");
        mMaskedString.Length = 0;
        mMaskedString.Append(masked);
        mMaskedString.Length = leftString.Length;
    }

    protected override void ConcatenateString(string startText) {
        base.ConcatenateString(startText);

        // cache start text in case the state flips
        mLastStartText = startText;
    }

    protected override StringBuilder LeftString() {
        // return the masked string when not listening
        return mIsListening ? base.LeftString() : mMaskedString;
    }

    // -- commands --
    public void SetIsListening(bool isListening) {
        if (mIsListening == isListening) {
            return;
        }

        // update state
        mIsListening = isListening;

        // refresh text
        ConcatenateString(mLastStartText);
        textAdapter.Text = outputString.ToString();
    }
}
