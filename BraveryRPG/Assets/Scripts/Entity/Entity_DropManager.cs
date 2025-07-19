using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemList_DataSO dropData;

    [Header("Drop Restrctions")]
    [SerializeField] private int maxRarityAmount = 1200;
    [SerializeField] private int maxItemsToDrop = 3;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            DropItems();
        }
    }

    public virtual void DropItems()
    {
        if (dropData == null)
        {
            Debug.Log("You need to assign drop data on entity: " + gameObject.name);
            return;
        }

        List<Item_DataSO> itemsToDrop = RollDrops();
        int amountToDrop = Mathf.Min(itemsToDrop.Count, maxItemsToDrop);

        for (int i = 0; i < amountToDrop; i++)
        {
            CreateItemDrop(itemsToDrop[i]);
        }
    }

    protected void CreateItemDrop(Item_DataSO itemToDrop)
    {
        GameObject newItem = Instantiate(
            itemDropPrefab,
            transform.position,
            Quaternion.identity
        );
        // Configure the item collider so it is not "A Trigger",
        // so that it will respect physics interactions with the ground.
        newItem.GetComponent<Object_ItemPickup>().SetupItem(itemToDrop);
    }

    public List<Item_DataSO> RollDrops()
    {
        List<Item_DataSO> possibleDrops = new();

        // Step 1: Roll each item based on rarity and max drop chance
        foreach (var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();

            if (Random.Range(0, 100) <= dropChance)
            {
                possibleDrops.Add(item);
            }
        }

        // Step 2: Sort by rarity (highest to lowest)
        // If you rolled a Legendary drop, you should get that (favored
        // over an inferior-rarity drop, like an Uncommon or Epic)
        possibleDrops = possibleDrops.OrderByDescending(
            item => item.itemRarity
        ).ToList();

        List<Item_DataSO> finalDrops = new();
        // For testing purposes; do not reduce the global maxRarityAmount;
        // this will allow for items to be dropped repeatedly.
        float maxRarityAmount = this.maxRarityAmount;

        // Step 3: Add items to final drop list until rarity limit on entity is reached
        foreach (var item in possibleDrops)
        {
            if (maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);
                // Reduce rarity, so we aren't dropping several high-rarity items.
                maxRarityAmount -= item.itemRarity;
            }
        }

        return finalDrops;
    }
}
