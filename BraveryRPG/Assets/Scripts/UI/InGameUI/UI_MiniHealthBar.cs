using UnityEngine;
using UnityEngine.UI;

public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entityRef;
    public Slider slider { get; private set; }

    void Awake()
    {
        entityRef = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();

        if (slider == null)
        {
            Debug.LogWarning($"{gameObject.name} -> UI_MiniHealthBar.slider is null!");
        }

        // TODO: The health bar is scaling to match the parent scale,
        //  so for enemies, it will be 130% size. Can also explore
        //  using the [ExecuteInEditMode] decorator.
        // Transform parent = GetComponentInParent<Transform>();
        // transform.localScale = new Vector3(
        //     FixedScale,
        //     FixedScale,
        //     FixedScale);
        // transform.localScale = new Vector3(
        //     FixedScale / parent.transform.localScale.x,
        //     FixedScale / parent.transform.localScale.y,
        //     FixedScale / parent.transform.localScale.z);
    }

    void OnEnable()
    {
        // Subscribe to event.
        entityRef.OnFlipped += HandleFlip;
    }

    void OnDisable()
    {
        // Unsubscribe from event.
        entityRef.OnFlipped -= HandleFlip;
    }

    // Prevent Slider UI from flipping when the parent GameObject is flipped.
    // This is not performant to use in Update() lifecycle.
    private void HandleFlip() => transform.rotation = Quaternion.identity;
}
