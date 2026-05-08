using UnityEngine;

/// <summary>
/// Place ce script sur le GameObject "Ground" (SpriteRenderer Draw Mode = Tiled).
///
/// - Suit la caméra → couvre toujours tout l'écran (monde infini)
/// - Scrolle les UV de la texture en fonction de la position monde
///   → le joueur voit le sol défiler sous lui (sensation de déplacement)
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class GroundScroller : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Coverage</size></b></color>")]
    [Tooltip("Multiplicateur de couverture écran. 1.5 = sécurisé contre les bords.")]
    [SerializeField] private float _coverageMultiplier = 1.5f;

    [Header("<color=yellow><b><size=15>Scroll</size></b></color>")]
    [Tooltip("Taille d'une tuile en unités monde. Ajuste selon la taille de ta texture.")]
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

        // Crée une instance du material pour ne pas modifier le shared material
        _instanceMat = _spriteRenderer.material;
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

        // SpriteRenderer Tiled : Size pour tiler proprement, pas localScale
        _spriteRenderer.size = new Vector2(width, height);
    }

    private void ScrollTexture()
    {
        if (_tileSize <= 0f) return;

        Vector3 pos = _cam.position;

        // Décale les UV en fonction de la position monde
        // → la texture défile sous le joueur comme si le sol était ancré dans le monde
        _instanceMat.mainTextureOffset = new Vector2(
            pos.x / _tileSize,
            pos.y / _tileSize
        );
    }
    #endregion
}
