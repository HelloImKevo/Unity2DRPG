using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counterattack Details")]
    [Tooltip("Brief punishment window for the player missing the counterattack parry window")]
    [SerializeField] private float counterattackRecovery = 0.25f;

    public bool CounterattackPerformed()
    {
        bool hasPerformedCounterattack = false;

        foreach (var target in GetDetectedColliders())
        {
            // Optimization - Resolves Unity lint warning:
            // GetComponent allocates even if no component is found.
            // Previous logic:
            // ICounterable counterable = target.GetComponent<ICounterable>();
            if (target.TryGetComponent<ICounterable>(out var counterable))
            {
                if (counterable.CanBeCountered)
                {
                    counterable.OnReceiveCounterattack();
                    hasPerformedCounterattack = true;
                }
            }
        }

        return hasPerformedCounterattack;
    }

    public float GetCounterattackRecoveryDuration() => counterattackRecovery;
}
