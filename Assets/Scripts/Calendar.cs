using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI = UnityEngine.UI;
using DateTime = System.DateTime;

public class Calendar: MonoBehaviour {
    // -- fields --
    [Tooltip("The day text")]
    [SerializeField]
    protected UI.Text fDay;

    [Tooltip("The month text")]
    [SerializeField]
    protected UI.Text fMonth;

    // -- props --
    private DateTime mDate;

    // -- commands --
    public void ResetDate() {
        mDate = new DateTime(2017, 8, 10);
        this.Render();
    }

    public void AdvanceDate() {
        var interval = Mathf.Max((NextBirthday() - mDate).Days - 10, 1);
        mDate = mDate.AddDays(Random.Range(1, interval));
        this.Render();
    }

    public void AdvanceToBirthday() {
        mDate = NextBirthday();
        this.Render();
    }

    // -- commands/helpers
    private void Render() {
        fDay.text = mDate.ToString("dd");
        fMonth.text = mDate.ToString("m").Substring(0, 3);
    }

    // -- queries --
    private DateTime NextBirthday() {
        var year = mDate.Year;

        // next year if the birthday has already happened
        if (mDate.Month > 9 || (mDate.Month == 9 && mDate.Day > 18)) {
            year += 1;
        }

        return new DateTime(year, 9, 18);
    }
}
