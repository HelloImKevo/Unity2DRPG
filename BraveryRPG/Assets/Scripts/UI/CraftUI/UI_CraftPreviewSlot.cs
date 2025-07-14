using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviewSlot : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialNameValue;

    public void SetupPreviewSlot(Item_DataSO itemData, int availableAmount, int requiredAmount)
    {
        materialIcon.sprite = itemData.itemIcon;

        // Animal Leather 4 / 5
        // Steel Ingot 2 / 3
        materialNameValue.text = itemData.itemName + " - " + availableAmount + "/" + requiredAmount;
    }
}
