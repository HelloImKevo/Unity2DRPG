using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;

    /// Summary:
    ///     1 = No layer movement
    ///     0.9 = Subtle scroll speed
    ///     0.7 = Medium scroll speed
    ///     0.5 = Fast scroll speed
    [Range(0, 1)]
    [Tooltip("1 = No layer movement\n0.9 = Subtle scroll speed\n0.7 = Medium scroll speed\n0.5 = Fast scroll speed")]
    [SerializeField] private float parallaxMultiplier;
    [Tooltip("Used for image forward-snapping for images with widths less than the camera width")]
    [SerializeField] private float imageWidthOffset = 10f;

    private float imageFullWidth;
    private float imageHalfWidth;

    public void CalculateImageWidth()
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        imageHalfWidth = imageFullWidth / 2;
    }

    public void Move(float distanceToMove)
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageRightEdge = (background.position.x + imageHalfWidth) - imageWidthOffset;
        float imageLeftEdge = (background.position.x - imageHalfWidth) + imageWidthOffset;

        if (imageRightEdge < cameraLeftEdge)
        {
            background.position += Vector3.right * imageFullWidth;
        }
        else if (imageLeftEdge > cameraRightEdge)
        {
            // Went too far to the right, we need to snap the image back.
            background.position += Vector3.right * -imageFullWidth;
        }
    }
}
