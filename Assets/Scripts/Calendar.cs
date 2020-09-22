using DateTime = System.DateTime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI = UnityEngine.UI;

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
    private int interval;

    // -- commands --
    protected void ResetDate() {
        mDate = new DateTime(2017, 8, 10);
        this.Render();
    }

    protected void AdvanceDate() {
        var days = (NextBirthday() - mDate).Days;
        var interval = Mathf.Max(days - 10, 1);
        Debug.Log("days: " + days + " interval: " + interval);
        mDate = mDate.AddDays(Random.Range(1, interval));
        this.Render();
    }

    protected void AdvanceToBirthday() {
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
