using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VNStep
{
    [Tooltip("Si dejas esto vacío, se mantendrá la imagen del paso anterior.")]
    public Sprite artwork;

    [Tooltip("Opcional: Quién está hablando")]
    public string speakerName;

    [TextArea(3, 5)]
    public string dialogueText;

    [Tooltip("Escribe algo aquí (ej. 'iniciar_point_click') para avisarle a tus otros scripts que hagan algo especial.")]
    public string triggerEvent;
}

[CreateAssetMenu(fileName = "NewVNScene", menuName = "Visual Novel/Scene Data")]
public class VNSceneData : ScriptableObject
{
    public List<VNStep> steps = new List<VNStep>();
}