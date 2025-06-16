using UnityEngine;

public interface ICounterable
{
    public bool CanBeCountered { get; }

    // Summary:
    //     Called when the [ICounterable] enemy receives a counterattack
    //     within the counter window. The enemy should react with an effect
    //     such as becoming briefly stunned.
    public void OnReceiveCounterattack();
}
