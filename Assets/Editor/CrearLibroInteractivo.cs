using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Events;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using System.IO;

public class CrearLibroInteractivo
{
    static Font fuente;

    [MenuItem("Tools/Crear Libro Interactivo")]
    static void Crear()
    {
        if (!EditorUtility.DisplayDialog("Crear Libro Interactivo",
            "Se crearán 3 escenas (Pagina1, Pagina2, Pagina3), " +
            "un sprite placeholder y se configurarán en Build Settings.\n\n¿Continuar?",
            "Crear", "Cancelar"))
            return;

        fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (fuente == null)
            fuente = Resources.GetBuiltinResource<Font>("Arial.ttf");

        EnsureFolder("Assets", "Sprites");
        EnsureFolder("Assets", "Scenes");

        CreatePlaceholderSprite();

        CreatePagina1();
        CreatePagina2();
        CreatePagina3();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Pagina1.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Pagina2.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Pagina3.unity", true),
        };

        EditorSceneManager.OpenScene("Assets/Scenes/Pagina1.unity");

        EditorUtility.DisplayDialog("¡Listo!",
            "Libro interactivo creado exitosamente.\n\n" +
            "• 3 escenas en Assets/Scenes/\n" +
            "• Sprite placeholder en Assets/Sprites/\n" +
            "• Escenas añadidas a Build Settings\n\n" +
            "PARA USAR TU PROPIA IMAGEN:\n" +
            "1. Coloca tu imagen PNG en Assets/Sprites/\n" +
            "2. Abre Pagina2\n" +
            "3. Selecciona 'SpriteArrastrable' en la jerarquía\n" +
            "4. Arrastra tu imagen al campo 'Sprite' del SpriteRenderer",
            "OK");
    }

    // ═══════════════════════════════════════════════════════
    //  PAGINA 1 — PORTADA
    // ═══════════════════════════════════════════════════════
    static void CreatePagina1()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        SetupCamera(new Color(0.08f, 0.08f, 0.28f), false);

        var canvas = CreateCanvas();
        CreateEventSystem();

        // Panel de fondo
        CreatePanel(canvas.transform, "PanelFondo", new Color(1f, 1f, 1f, 0.08f),
            Vector2.zero, new Vector2(750, 500));

        // Título
        var titulo = CreateText(canvas.transform, "Titulo", "Mi Libro Interactivo", 60,
            new Vector2(0, 100), new Vector2(700, 100), Color.white, TextAnchor.MiddleCenter);

        // Subtítulo
        CreateText(canvas.transform, "Subtitulo",
            "Actividad 2.3\nProgramación de Botones", 28,
            new Vector2(0, -10), new Vector2(700, 80),
            new Color(0.7f, 0.7f, 1f), TextAnchor.MiddleCenter);

        // Navigator
        var navObj = new GameObject("SceneNavigator");
        var nav = navObj.AddComponent<SceneNavigator>();

        // Botón Siguiente
        var btnSig = CreateButton(canvas.transform, "BtnSiguiente",
            "Siguiente  ▶", new Vector2(0, -200), new Vector2(220, 55));
        UnityEventTools.AddVoidPersistentListener(btnSig.onClick,
            new UnityAction(nav.NextPage));

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Pagina1.unity");
    }

    // ═══════════════════════════════════════════════════════
    //  PAGINA 2 — DRAG & DROP + CONTROL DE FUENTES
    // ═══════════════════════════════════════════════════════
    static void CreatePagina2()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var cam = SetupCamera(new Color(0.88f, 0.92f, 0.96f), true);
        cam.orthographicSize = 5;

        var canvas = CreateCanvas();
        CreateEventSystem();

        // ── Sprite arrastrable ──
        var spriteObj = new GameObject("SpriteArrastrable");
        var sr = spriteObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 1;
        spriteObj.AddComponent<BoxCollider2D>();
        spriteObj.AddComponent<DragAndDrop>();
        spriteObj.transform.position = new Vector3(-3.5f, 1.5f, 0f);

        // Configurar sailor.png como Sprite y cargarla
        string sailorPath = "Assets/Sprites/sailor.png";
        string placeholderPath = "Assets/Sprites/Placeholder.png";

        SetTextureAsSprite(sailorPath);
        SetTextureAsSprite(placeholderPath);

        Sprite spriteImg = AssetDatabase.LoadAssetAtPath<Sprite>(sailorPath);
        if (spriteImg == null)
            spriteImg = AssetDatabase.LoadAssetAtPath<Sprite>(placeholderPath);
        if (spriteImg != null)
            sr.sprite = spriteImg;

        // Etiqueta para el sprite
        CreateText(canvas.transform, "LblArrastrar",
            "◄ Arrastra la imagen\n     con el mouse", 18,
            new Vector2(-340, -100), new Vector2(280, 60),
            new Color(0.3f, 0.3f, 0.3f), TextAnchor.MiddleCenter);

        // ── Texto de ejemplo ──
        var textoEjemplo = CreateText(canvas.transform, "TextoEjemplo",
            "Texto de ejemplo\nUsa los botones para modificarme", 34,
            new Vector2(180, 140), new Vector2(480, 130),
            Color.black, TextAnchor.MiddleCenter);

        // ── FontController ──
        var fontObj = new GameObject("FontController");
        var fc = fontObj.AddComponent<FontController>();
        fc.targetText = textoEjemplo;

        // ── Botones de control de fuente ──
        float btnY = -160f;
        float spacing = 115f;
        float startX = -230f;

        // A+
        var btnAUp = CreateButton(canvas.transform, "BtnAumentar",
            "A+", new Vector2(startX, btnY), new Vector2(100, 48));
        UnityEventTools.AddVoidPersistentListener(btnAUp.onClick,
            new UnityAction(fc.IncreaseFontSize));

        // A-
        var btnADown = CreateButton(canvas.transform, "BtnDisminuir",
            "A-", new Vector2(startX + spacing, btnY), new Vector2(100, 48));
        UnityEventTools.AddVoidPersistentListener(btnADown.onClick,
            new UnityAction(fc.DecreaseFontSize));

        // Color
        var btnColor = CreateButton(canvas.transform, "BtnColor",
            "Color", new Vector2(startX + spacing * 2, btnY), new Vector2(100, 48));
        UnityEventTools.AddVoidPersistentListener(btnColor.onClick,
            new UnityAction(fc.CycleColor));

        // Negrita
        var btnBold = CreateButton(canvas.transform, "BtnNegrita",
            "B", new Vector2(startX + spacing * 3, btnY), new Vector2(100, 48));
        SetButtonTextBold(btnBold);
        UnityEventTools.AddVoidPersistentListener(btnBold.onClick,
            new UnityAction(fc.ToggleBold));

        // Cursiva
        var btnItalic = CreateButton(canvas.transform, "BtnCursiva",
            "I", new Vector2(startX + spacing * 4, btnY), new Vector2(100, 48));
        SetButtonTextItalic(btnItalic);
        UnityEventTools.AddVoidPersistentListener(btnItalic.onClick,
            new UnityAction(fc.ToggleItalic));

        // ── Navegación ──
        var navObj = new GameObject("SceneNavigator");
        var nav = navObj.AddComponent<SceneNavigator>();

        var btnPrev = CreateButton(canvas.transform, "BtnAnterior",
            "◀  Anterior", new Vector2(-350, -280), new Vector2(200, 50));
        UnityEventTools.AddVoidPersistentListener(btnPrev.onClick,
            new UnityAction(nav.PreviousPage));

        var btnNext = CreateButton(canvas.transform, "BtnSiguiente",
            "Siguiente  ▶", new Vector2(350, -280), new Vector2(200, 50));
        UnityEventTools.AddVoidPersistentListener(btnNext.onClick,
            new UnityAction(nav.NextPage));

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Pagina2.unity");
    }

    // ═══════════════════════════════════════════════════════
    //  PAGINA 3 — FINAL
    // ═══════════════════════════════════════════════════════
    static void CreatePagina3()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        SetupCamera(new Color(0.06f, 0.2f, 0.1f), false);

        var canvas = CreateCanvas();
        CreateEventSystem();

        // Panel
        CreatePanel(canvas.transform, "PanelFondo", new Color(1f, 1f, 1f, 0.08f),
            Vector2.zero, new Vector2(750, 450));

        // Título
        CreateText(canvas.transform, "TituloFinal",
            "¡Fin del Libro!", 58,
            new Vector2(0, 80), new Vector2(700, 90),
            Color.white, TextAnchor.MiddleCenter);

        // Resumen
        CreateText(canvas.transform, "TextoCierre",
            "Funciones exploradas:\n\n" +
            "•  Arrastrar sprites con el mouse\n" +
            "•  Controlar fuentes desde botones\n" +
            "•  Navegar entre páginas", 24,
            new Vector2(0, -40), new Vector2(600, 160),
            new Color(0.75f, 1f, 0.75f), TextAnchor.MiddleCenter);

        // Navigator
        var navObj = new GameObject("SceneNavigator");
        var nav = navObj.AddComponent<SceneNavigator>();

        // Botón Anterior
        var btnPrev = CreateButton(canvas.transform, "BtnAnterior",
            "◀  Anterior", new Vector2(-200, -220), new Vector2(200, 55));
        UnityEventTools.AddVoidPersistentListener(btnPrev.onClick,
            new UnityAction(nav.PreviousPage));

        // Botón Inicio
        var btnInicio = CreateButton(canvas.transform, "BtnInicio",
            "🏠  Inicio", new Vector2(200, -220), new Vector2(200, 55));
        UnityEventTools.AddStringPersistentListener(btnInicio.onClick,
            new UnityAction<string>(nav.GoToScene), "Pagina1");

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Pagina3.unity");
    }

    // ═══════════════════════════════════════════════════════
    //  HELPERS
    // ═══════════════════════════════════════════════════════

    static void CreatePlaceholderSprite()
    {
        string fullPath = Path.Combine(Application.dataPath, "Sprites", "Placeholder.png");
        if (File.Exists(fullPath)) return;

        int size = 256;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var gold = new Color(1f, 0.78f, 0.24f);
        var orange = new Color(0.95f, 0.45f, 0.15f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float cx = (x - size * 0.5f) / (size * 0.5f);
                float cy = (y - size * 0.5f) / (size * 0.5f);
                float dist = Mathf.Sqrt(cx * cx + cy * cy);

                if (dist < 0.82f)
                {
                    float t = dist / 0.82f;
                    Color c = Color.Lerp(gold, orange, t * t);
                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
                else if (dist < 0.88f)
                {
                    float edge = 1f - (dist - 0.82f) / 0.06f;
                    Color c = orange;
                    c.a = edge;
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        tex.Apply();

        File.WriteAllBytes(fullPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.Refresh();

        var importer = AssetImporter.GetAtPath("Assets/Sprites/Placeholder.png") as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.SaveAndReimport();
        }
    }

    static Camera SetupCamera(Color bgColor, bool orthographic)
    {
        var cam = Camera.main;
        if (cam == null) return null;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgColor;
        cam.orthographic = orthographic;
        return cam;
    }

    static Canvas CreateCanvas()
    {
        var obj = new GameObject("Canvas");
        var canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = obj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;

        obj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var obj = new GameObject("EventSystem");
        obj.AddComponent<EventSystem>();
        obj.AddComponent<InputSystemUIInputModule>();
    }

    static Image CreatePanel(Transform parent, string name, Color color,
        Vector2 pos, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var img = obj.AddComponent<Image>();
        img.color = color;

        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        return img;
    }

    static Text CreateText(Transform parent, string name, string content, int fontSize,
        Vector2 pos, Vector2 size, Color color, TextAnchor alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        var text = obj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.font = fuente;

        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        return text;
    }

    static Button CreateButton(Transform parent, string name,
        string label, Vector2 pos, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        var img = obj.AddComponent<Image>();
        img.color = new Color(0.92f, 0.92f, 0.92f);

        var btn = obj.AddComponent<Button>();
        var colores = btn.colors;
        colores.normalColor = new Color(0.92f, 0.92f, 0.92f);
        colores.highlightedColor = new Color(0.78f, 0.83f, 1f);
        colores.pressedColor = new Color(0.6f, 0.65f, 0.9f);
        colores.selectedColor = new Color(0.85f, 0.85f, 0.95f);
        btn.colors = colores;

        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        // Child text
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);

        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.fontSize = 22;
        text.color = new Color(0.15f, 0.15f, 0.15f);
        text.alignment = TextAnchor.MiddleCenter;
        text.font = fuente;

        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return btn;
    }

    static void SetButtonTextBold(Button btn)
    {
        var t = btn.GetComponentInChildren<Text>();
        if (t != null) t.fontStyle = FontStyle.Bold;
    }

    static void SetButtonTextItalic(Button btn)
    {
        var t = btn.GetComponentInChildren<Text>();
        if (t != null) t.fontStyle = FontStyle.Italic;
    }

    static void SetTextureAsSprite(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.SaveAndReimport();
        }
    }

    static void EnsureFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + folder))
            AssetDatabase.CreateFolder(parent, folder);
    }
}
