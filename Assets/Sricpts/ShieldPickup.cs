using UnityEngine;

namespace LeagueTry
{
    // ES: Creado con ayuda de ChatGPT para recoger el escudo.
    // EN: Created with ChatGPT assistance to collect the shield.
    public class ShieldPickup : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            PlayerShield shield =
                other.GetComponentInParent<PlayerShield>();

            if (shield == null)
            {
                return;
            }

            shield.CollectShield();

            // ES: Ocultamos el objeto para impedir recogerlo repetidamente.
            // EN: Hide the object to prevent repeated collection.
            gameObject.SetActive(false);
        }
    }
}