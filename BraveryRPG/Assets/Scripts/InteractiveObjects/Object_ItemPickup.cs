using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    // [SerializeField] private Vector2 dropForce = new Vector2(3, 10);
    [SerializeField] private Item_DataSO itemData;

    [Space]
    [SerializeField] private SpriteRenderer sr;
    // [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Collider2D col;

    private void OnValidate()
    {
        if (itemData == null) return;

        sr = GetComponent<SpriteRenderer>();
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        sr.sprite = itemData.itemIcon;
        gameObject.name = "Object_ItemPickup - " + itemData.itemName;
    }

    public void SetupItem(Item_DataSO itemData)
    {
        this.itemData = itemData;
        SetupVisuals();

        // float xDropForce = Random.Range(-dropForce.x, dropForce.x);
        // rb.linearVelocity = new Vector2(xDropForce, dropForce.y);
        // col.isTrigger = false;
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && col.isTrigger == false)
    //     {
    //         col.isTrigger = true;
    //         rb.constraints = RigidbodyConstraints2D.FreezeAll;
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory_Player inventory = collision.GetComponent<Inventory_Player>();

        if (inventory == null) return;

        Inventory_Item itemToAdd = new Inventory_Item(itemData);
        Inventory_Storage storage = inventory.storage;

        if (ItemType.Material == itemData.itemType)
        {
            storage.AddMaterialToStash(itemToAdd);
            Destroy(gameObject);
            return;
        }

        if (inventory.CanAddItem(itemToAdd))
        {
            inventory.AddItem(itemToAdd);
            Destroy(gameObject);
        }
    }
}
