using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI = UnityEngine.UI;
using DateTime = System.DateTime;

public class Calendar: MonoBehaviour {
    // -- constants --
    private static Color kOrange = new Color(0.95f, 0.64f, 0.24f, 1.0f);

    // -- fields --
    [Tooltip("The day text")]
    [SerializeField]
    protected UI.Text[] fDays;

    [Tooltip("The month text")]
    [SerializeField]
    protected UI.Text[] fMonths;

    // -- props --
    private DateTime mDate;
    private bool mIsBirthday;

    // -- commands --
    public void ResetDate() {
        mDate = new DateTime(2017, 8, 10);
        this.Render();
    }

    public void AdvanceDate() {
        mIsBirthday = false;
        var interval = Mathf.Max((NextBirthday() - mDate).Days - 10, 1);
        mDate = mDate.AddDays(Random.Range(1, interval));
        this.Render();
    }

    public void AdvanceToBirthday() {
        mIsBirthday = true;
        mDate = NextBirthday();
        this.Render();
    }

    public void SetTextColor() {
        var color = mIsBirthday ? kOrange : Color.black;

        foreach (var day in fDays) {
            day.color = color;
        }

        foreach (var month in fMonths) {
            month.color = color;
        }
    }

    // -- commands/helpers
    private void Render() {
        foreach (var day in fDays) {
            day.text = mDate.ToString("dd");
        }

        foreach (var month in fMonths) {
            month.text = mDate.ToString("m").Substring(0, 3);
        }
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
