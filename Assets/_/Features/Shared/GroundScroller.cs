using UnityEngine;

// Attach to the "Ground" GameObject that already has a SpriteRenderer (Draw Mode = Tiled).
// Follows the camera to maintain infinite coverage and scrolls the texture UV
// based on world position so the ground appears to move under the player.
[RequireComponent(typeof(SpriteRenderer))]
public class GroundScroller : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Coverage</size></b></color>")]
    [Tooltip("Screen coverage multiplier. 1 = exact screen size, 1.5 = safe margin.")]
    [SerializeField] private float _coverageMultiplier = 1.5f;

    [Header("<color=yellow><b><size=15>Scroll</size></b></color>")]
    [Tooltip("World units per texture tile. Adjust to match your texture size.")]
    [SerializeField] private float _tileSize = 3f;
    #endregion

    #region State
    private SpriteRenderer _spriteRenderer;
    private Material       _instanceMat;
    private Transform      _cam;
    private Camera         _mainCamera;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mainCamera     = Camera.main;
        _cam            = _mainCamera?.transform;
        _instanceMat    = _spriteRenderer.material; // creates a material instance
    }

    private void LateUpdate()
    {
        if (_cam == null) return;

        FollowCamera();
        ResizeToCoverScreen();
        ScrollTexture();
    }

    private void OnDestroy()
    {
        if (_instanceMat != null)
            Destroy(_instanceMat);
    }
    #endregion

    #region Ground Logic
    private void FollowCamera()
    {
        Vector3 camPos = _cam.position;
        transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
    }

    private void ResizeToCoverScreen()
    {
        if (_mainCamera.orthographicSize <= 0f) return;

        float height = _mainCamera.orthographicSize * 2f * _coverageMultiplier;
        float width  = height * _mainCamera.aspect;

        // Use Size (not localScale) to tile correctly with Draw Mode = Tiled
        _spriteRenderer.size = new Vector2(width, height);
    }

    private void ScrollTexture()
    {
        if (_tileSize <= 0f) return;

        Vector3 pos = _cam.position;

        // Offset UVs based on world position so the texture scrolls with the player
        _instanceMat.mainTextureOffset = new Vector2(
            pos.x / _tileSize,
            pos.y / _tileSize
        );
    }
    #endregion
}
