using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Login dialog for GameJolt shown at startup on non-WebGL platforms.
/// Created entirely in code — no prefab required.
/// </summary>
public class GameJoltLoginUI : MonoBehaviour
{
    // ─── Entry point ──────────────────────────────────────────────────────────

    public static void Show(GameJoltManager manager, System.Action<bool> onComplete)
    {
        var go             = new GameObject("[GameJoltLogin]", typeof(RectTransform));
        DontDestroyOnLoad(go);
        var ui             = go.AddComponent<GameJoltLoginUI>();
        ui._manager        = manager;
        ui._onComplete     = onComplete;
        ui.Build();
    }

    // ─── State ────────────────────────────────────────────────────────────────

    private GameJoltManager     _manager;
    private System.Action<bool> _onComplete;
    private TMP_InputField      _usernameField;
    private TMP_InputField      _tokenField;
    private TextMeshProUGUI     _statusText;
    private Button              _loginButton;

    // ─── Build ────────────────────────────────────────────────────────────────

    private void Build()
    {
        // Canvas
        var canvas             = gameObject.AddComponent<Canvas>();
        canvas.renderMode      = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder    = 999;
        var scaler             = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode     = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight  = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();

        // Full-screen dark overlay
        var overlay = GO("Overlay", transform);
        overlay.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.87f);
        Stretch(RT(overlay));

        // Centered card (auto-height via ContentSizeFitter)
        var card    = GO("Card", RT(overlay));
        card.AddComponent<Image>().color = new Color(0.07f, 0.07f, 0.11f);
        var cardRT  = RT(card);
        cardRT.anchorMin = cardRT.anchorMax = cardRT.pivot = new Vector2(0.5f, 0.5f);
        cardRT.sizeDelta = new Vector2(440, 0); // width fixed; height auto

        var vl                    = card.AddComponent<VerticalLayoutGroup>();
        vl.padding                = new RectOffset(36, 36, 28, 28);
        vl.spacing                = 10;
        vl.childAlignment         = TextAnchor.UpperCenter;
        vl.childControlWidth      = true;
        vl.childControlHeight     = true;
        vl.childForceExpandWidth  = true;
        vl.childForceExpandHeight = false;

        var csf             = card.AddComponent<ContentSizeFitter>();
        csf.horizontalFit   = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit     = ContentSizeFitter.FitMode.PreferredSize;

        // Widgets
        Lbl(card, "CONNEXION GAMEJOLT", 19, FontStyles.Bold, new Color(0.92f, 0.88f, 1f), 36);
        Lbl(card, "Profil GameJolt  ›  Settings  ›  Game Token",
            10, FontStyles.Italic, new Color(0.50f, 0.50f, 0.62f), 20);
        Spacer(card, 4);
        _usernameField = Field(card, "Nom d'utilisateur…", 44);
        _tokenField    = Field(card, "Game Token…",        44);
        _statusText    = Lbl(card, "", 11, FontStyles.Italic, new Color(1f, 0.35f, 0.35f), 20);
        Spacer(card, 2);
        _loginButton   = Btn(card, "SE CONNECTER",      new Color(0.13f, 0.50f, 0.13f), OnLoginClick, 45);
                         Btn(card, "JOUER SANS COMPTE", new Color(0.20f, 0.20f, 0.28f), OnGuestClick, 36);
    }

    // ─── Interaction ──────────────────────────────────────────────────────────

    private void OnLoginClick()
    {
        string u = _usernameField.text.Trim();
        string t = _tokenField.text.Trim();

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(t))
        {
            Status("Remplis les deux champs.", new Color(1f, 0.40f, 0.40f));
            return;
        }

        _loginButton.interactable = false;
        Status("Connexion en cours…", new Color(1f, 0.82f, 0.20f));

        _manager.Authenticate(u, t, ok =>
        {
            if (ok)
            {
                Status("Connecté ! Bon jeu \U0001f3ae", new Color(0.20f, 1f, 0.35f));
                StartCoroutine(CloseAfter(0.9f, true));
            }
            else
            {
                _loginButton.interactable = true;
                Status("Échec — vérifie ton username et ton Game Token.", new Color(1f, 0.35f, 0.35f));
            }
        });
    }

    private void OnGuestClick()
    {
        _onComplete?.Invoke(false);
        Destroy(gameObject);
    }

    private void Status(string msg, Color col) { _statusText.text = msg; _statusText.color = col; }

    private IEnumerator CloseAfter(float delay, bool success)
    {
        yield return new WaitForSeconds(delay);
        _onComplete?.Invoke(success);
        Destroy(gameObject);
    }

    // ─── Widget factories ─────────────────────────────────────────────────────

    private static TextMeshProUGUI Lbl(GameObject parent, string text, float size,
        FontStyles style, Color color, int height)
    {
        var go = GO("Lbl", RT(parent));
        PH(go, height);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = size; t.fontStyle = style;
        t.color = color; t.alignment = TextAlignmentOptions.Center;
        return t;
    }

    private static TMP_InputField Field(GameObject parent, string placeholder, int height)
    {
        var c = GO("Input", RT(parent));
        PH(c, height);
        c.AddComponent<Image>().color = new Color(0.13f, 0.13f, 0.18f);

        // Viewport (clips overflowing text)
        var vp   = GO("TextArea", RT(c));
        vp.AddComponent<RectMask2D>();
        var vpRT = RT(vp);
        vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = new Vector2(10, 1); vpRT.offsetMax = new Vector2(-10, -1);

        // Placeholder
        var ph = GO("Placeholder", vpRT);
        Stretch(RT(ph));
        var phT = ph.AddComponent<TextMeshProUGUI>();
        phT.text = placeholder; phT.fontSize = 14;
        phT.color = new Color(0.42f, 0.42f, 0.50f); phT.fontStyle = FontStyles.Italic;
        phT.alignment = TextAlignmentOptions.MidlineLeft;

        // Editable text
        var txt   = GO("Text", vpRT);
        Stretch(RT(txt));
        var mainT = txt.AddComponent<TextMeshProUGUI>();
        mainT.fontSize = 14; mainT.color = Color.white;
        mainT.alignment = TextAlignmentOptions.MidlineLeft;

        var field           = c.AddComponent<TMP_InputField>();
        field.textViewport  = vpRT;
        field.textComponent = mainT;
        field.placeholder   = phT;
        return field;
    }

    private static Button Btn(GameObject parent, string label, Color bg,
        UnityEngine.Events.UnityAction onClick, int height)
    {
        var go  = GO("Btn", RT(parent));
        PH(go, height);
        var img = go.AddComponent<Image>();
        img.color = bg;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        var lbl = GO("Lbl", RT(go));
        Stretch(RT(lbl));
        var t = lbl.AddComponent<TextMeshProUGUI>();
        t.text = label; t.fontSize = 15; t.fontStyle = FontStyles.Bold;
        t.color = Color.white; t.alignment = TextAlignmentOptions.Center;
        return btn;
    }

    private static void Spacer(GameObject parent, int height)
    {
        PH(GO("Spacer", RT(parent)), height);
    }

    // ─── Micro-helpers ────────────────────────────────────────────────────────

    // Create a GO with RectTransform, parented under a Transform
    private static GameObject GO(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    // Overload: parent is already a RectTransform (which IS a Transform)
    private static GameObject GO(string name, RectTransform parent) => GO(name, (Transform)parent);

    // Get the RectTransform of a GameObject (regular static method — NOT an extension)
    private static RectTransform RT(GameObject go) => go.GetComponent<RectTransform>();

    // Set preferred + min height via LayoutElement
    private static void PH(GameObject go, int height)
    {
        var le             = go.AddComponent<LayoutElement>();
        le.preferredHeight = le.minHeight = height;
    }

    // Stretch a RectTransform to fill its parent
    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
