using UnityEngine;
using UnityEngine.UI;

public class WebtoonTrigger : MonoBehaviour
{
    public RectTransform triggerInvisible;
    public RectTransform centroPantalla;
    public Image imagenFondo;

    private bool yaSeActivo = false;

    void Update()
    {
        if (triggerInvisible.position.y >= centroPantalla.position.y && !yaSeActivo)
        {
            imagenFondo.color = Color.black;
            yaSeActivo = true;
            Debug.Log("¡Trigger cruzado! Fondo cambiado a negro.");
        }
        else if (triggerInvisible.position.y < centroPantalla.position.y && yaSeActivo)
        {
            imagenFondo.color = Color.white;
            yaSeActivo = false;
        }
    }
}