using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Portal Scroll", fileName = "Item Effect Data - Portal Scroll")]
public class ItemEffect_PortalScroll : ItemEffect_DataSO
{
    // TODO: The Inventory_Base.TryUseItem() function always "consumes" the item,
    // even if the effect cannot be execute.
    public override void ExecuteEffect()
    {
        if (SceneManager.GetActiveScene().name == "Level_0")
        {
            Debug.LogWarning($"{GetType().Name}.ExecuteEffect() -> Cannot open portal in town!");
            return;
        }

        Player player = Player.instance;
        // Spawn the portal near the player's current position.
        Vector3 portalPosition = player.transform.position + new Vector3(player.FacingDir * 1.5f, 0);

        Object_Portal.instance.ActivatePortal(portalPosition, player.FacingDir);
    }
}
