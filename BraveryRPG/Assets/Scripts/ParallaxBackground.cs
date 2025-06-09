using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private ParallaxLayer[] backgroundLayers;

    private Camera mainCamera;
    private float lastCameraPositionX;
    private float cameraHalfWidth;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        InitializeLayers();
    }

    // Use FixedUpdate to prevent jittery pixel vibrations for static layers
    // with parallax multiplier 1.0.
    private void FixedUpdate()
    {
        float currentCameraPositionX = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPositionX - lastCameraPositionX;

        lastCameraPositionX = currentCameraPositionX;

        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

        foreach (var layer in backgroundLayers)
        {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    private void InitializeLayers()
    {
        foreach (var layer in backgroundLayers)
        {
            layer.CalculateImageWidth();
        }
    }
}
