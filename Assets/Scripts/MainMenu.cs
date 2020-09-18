using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu: Fungus.MenuDialog {
    // -- commands --
    public override bool AddOption(string text, bool interactable, bool hideOption, Fungus.Block targetBlock) {
        // add the option
        var result = base.AddOption(text, interactable, hideOption, targetBlock);

        // show the appropriate dividers
        var i = 0;
        var dividers = gameObject.GetComponentsInChildren<Image>(true);

        foreach (var divider in dividers) {
            if (divider.name != "Divider") {
                continue;
            }

            // show any dividers between two active buttons (i.e. n-1 dividers) and
            // hide the rest
            var isVisible = i < DisplayedOptionsCount - 1;
            divider.gameObject.SetActive(isVisible);
            i++;
        }

        return result;
    }
}
