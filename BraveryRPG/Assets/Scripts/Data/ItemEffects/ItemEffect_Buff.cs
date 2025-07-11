using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Buff Effect", fileName = "Item Effect Data - Buff")]
public class ItemEffect_Buff : ItemEffect_DataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float duration;
    /// <summary>
    /// An arbitrary unique string generated when this ScriptableObject is created
    /// in the Unity Editor, to identify this buff provided by a consumable item,
    /// so only one instance of this item buff can be applied to the player.
    /// </summary>
    [SerializeField] private string source = Guid.NewGuid().ToString();

    public override bool CanBeUsed(Player player)
    {
        if (player.Stats.CanApplyBuffOf(source))
        {
            this.player = player;
            return true;
        }
        else
        {
            Debug.Log("Same buff effect cannot be applied twice!");
            return false;
        }
    }

    public override void ExecuteEffect()
    {
        player.Stats.ApplyBuff(buffsToApply, duration, source);
        player = null;
    }
}
