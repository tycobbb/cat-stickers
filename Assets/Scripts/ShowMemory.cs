using UnityEngine;
using F = Fungus;
using UI = UnityEngine.UI;

[F.CommandInfo("Custom", "ShowMemory", "Set alpha of the current memory.")]
[AddComponentMenu("")]
public class ShowMemory: F.Command {
    // -- fields --
    [Tooltip("The step variable")]
    [SerializeField]
    protected F.IntegerData fStep;

    [Tooltip("The last step variable")]
    [SerializeField]
    protected F.IntegerData fLastStep;

    [Tooltip("The memory container")]
    [SerializeField]
    protected MemoryLens fLens;

    // -- lifecycle --
    public override void OnEnter() {
        var percent = (float)(fStep.Value + 1) / fLastStep.Value;
        fLens.SetPercentComplete(percent);
        Continue();
    }
}
