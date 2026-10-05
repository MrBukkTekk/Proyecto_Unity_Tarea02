using UnityEngine;

namespace SalaDeLaConsola
{
    // Enganchar este script al sprite de la consola (o cualquier objeto
    // clickeable de la habitacion en 2D). Necesita un Collider2D en el
    // mismo GameObject (por ejemplo BoxCollider2D) para que OnMouseDown funcione.
    public class ConsolaInteractable : MonoBehaviour
    {
        [Tooltip("El panel del Canvas con el acertijo. Se activa al hacer click en la consola.")]
        public GameObject panelConsola;

        private void OnMouseDown()
        {
            Debug.Log("click detectado en consola");
            if (panelConsola != null)
                panelConsola.SetActive(true);
        }

        // Conectar al boton de cerrar (la X del panel) si el jugador
        // puede salir del acertijo antes de resolverlo.
        public void CerrarPanel()
        {
            if (panelConsola != null)
                panelConsola.SetActive(false);
        }
    }
}
