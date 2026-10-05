using System.Collections.Generic;
using UnityEngine;

// -----------------------------------------------------------------------
// Datos de una burbuja/globo de texto (es una IMAGEN, no texto real).
// -----------------------------------------------------------------------
[System.Serializable]
public class BubbleData
{
    [Tooltip("Sprite del globo de texto (placeholder por ahora)")]
    public Sprite bubbleSprite;

    [Tooltip("Posición del globo dentro del panel (anchoredPosition, 0,0 = centro del panel)")]
    public Vector2 anchoredPosition;

    [Tooltip("Tamaño del globo en pixeles")]
    public Vector2 size = new Vector2(400f, 200f);

    [Tooltip("Si está en true, este globo se revela solo (sin necesidad de tap) tras el delay")]
    public bool autoReveal = false;

    [Tooltip("Delay en segundos antes de auto-revelarse (solo si autoReveal = true)")]
    public float autoRevealDelay = 0.5f;

    [Header("Shake")]
    [Tooltip("Si está en true, el globo tiembla una vez revelado")]
    public bool shakeEnabled = false;

    [Tooltip("Eje en el que tiembla el globo")]
    public ShakeAxis shakeAxis = ShakeAxis.Vertical;

    [Tooltip("Qué tan fuerte es el temblor, en pixeles")]
    [Range(0f, 50f)]
    public float shakeIntensity = 6f;

    [Tooltip("Qué tan rápido tiembla (más alto = más nervioso)")]
    [Range(0.1f, 30f)]
    public float shakeSpeed = 8f;

    [Tooltip("Si está en true, tiembla para siempre mientras esté visible. Si está en false, tiembla solo por 'shakeDuration' segundos y luego se queda quieto")]
    public bool shakeForever = true;

    [Tooltip("Duración del temblor en segundos (solo se usa si shakeForever = false)")]
    public float shakeDuration = 1f;
}

// Eje en el que puede temblar un globo de texto.
public enum ShakeAxis
{
    Horizontal,
    Vertical,
    Both
}

// -----------------------------------------------------------------------
// Datos de un panel del webtoon: el dibujo + sus globos.
// -----------------------------------------------------------------------
[System.Serializable]
public class PanelData
{
    [Tooltip("Imagen/dibujo del panel (storyboard placeholder)")]
    public Sprite artwork;

    [Tooltip("Alto en pixeles con el que se mostrará el panel en el scroll. Ajusta según el aspect ratio real de tu arte.")]
    public float displayHeight = 1200f;

    [Tooltip("Globos de texto que aparecen SOBRE este panel, en orden de aparición")]
    public List<BubbleData> bubbles = new List<BubbleData>();
}

// -----------------------------------------------------------------------
// Asset que representa una historia corta completa (un "nivel").
// Click derecho en el Project -> Create -> Webtoon -> Story Data
// -----------------------------------------------------------------------
[CreateAssetMenu(fileName = "NewStory", menuName = "Webtoon/Story Data")]
public class StoryData : ScriptableObject
{
    public string storyTitle;

    [Tooltip("Paneles en orden de lectura (de arriba hacia abajo)")]
    public List<PanelData> panels = new List<PanelData>();
}