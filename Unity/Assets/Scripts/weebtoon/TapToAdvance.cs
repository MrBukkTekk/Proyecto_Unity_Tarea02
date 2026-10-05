using UnityEngine;
using UnityEngine.EventSystems;

// -----------------------------------------------------------------------
// Ponlo en el mismo GameObject que tu VIEWPORT del ScrollRect (el hijo que
// ya trae un Image + Mask por defecto cuando creas un "Scroll View" desde
// el menú UI de Unity). Ese Image ya tiene Raycast Target activado, que es
// justo lo que necesitamos para recibir el click.
//
// La ventaja de usar IPointerClickHandler en vez de Input.GetMouseButtonDown
// es que Unity YA distingue un tap de un drag: si el dedo/mouse se mueve más
// que el umbral de arrastre (EventSystem.pixelDragThreshold), esto no se
// dispara y el ScrollRect hace su scroll normal sin que lo interrumpamos.
// -----------------------------------------------------------------------
public class TapToAdvance : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private WebtoonController controller;

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.Advance();
    }
}
