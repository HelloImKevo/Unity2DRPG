using UnityEngine;

public enum UISlotType
{
    [Tooltip("Default enum value.")]
    None,

    [Tooltip("UI slot representing Merchant inventory. Shows a 'Buy Price'")]
    MerchantSlot,

    [Tooltip("UI slot representing Player inventory. Shows a 'Sell Price'")]
    PlayerSlot
}