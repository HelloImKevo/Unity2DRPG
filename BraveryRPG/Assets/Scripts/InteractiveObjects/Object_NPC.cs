using UnityEngine;

public class Object_NPC : MonoBehaviour, IInteractable
{
    protected Transform player;
    protected UI ui;
    protected Player_QuestManager questManager;

    [Header("Quest Info")]
    [SerializeField] private string npcTargetQuestId;
    [SerializeField] private RewardType rewardNpc;

    [Space]
    [SerializeField] private Transform npc;
    [Tooltip("Floating button hint that becomes visible when the player is within range.")]
    [SerializeField] private GameObject interactTooltip;
    private bool facingRight = true;

    [Header("Floaty Tooltip")]
    [SerializeField] private float floatSpeed = 8f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = interactTooltip.transform.position;
        // Hide the "Interact with this NPC" tooltip by default.
        interactTooltip.SetActive(false);
    }

    protected virtual void Start()
    {
        questManager = Player.GetInstance().questManager;
    }

    protected virtual void Update()
    {
        HandleNpcFlip();
        HandleTooltipFloat();
    }

    /// <summary>
    /// Updates the floating animation by smoothly moving the hint up and down over time.
    /// </summary>
    private void HandleTooltipFloat()
    {
        if (interactTooltip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactTooltip.transform.position = startPosition + new Vector3(0, yOffset);
        }
    }

    private void HandleNpcFlip()
    {
        if (player == null || npc == null) return;

        if (npc.position.x > player.position.x && facingRight)
        {
            // Flip NPC to face left towards the player.
            npc.transform.Rotate(0, 180f, 0);
            facingRight = false;
        }
        else if (npc.position.x < player.position.x && !facingRight)
        {
            // Flip NPC to face right towards the player.
            npc.transform.Rotate(0, 180f, 0);
            facingRight = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;
        interactTooltip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        interactTooltip.SetActive(false);
    }

    public virtual void Interact()
    {
        questManager.TryAddProgress(npcTargetQuestId);
        questManager.TryGiveRewardFrom(rewardNpc);
    }
}
