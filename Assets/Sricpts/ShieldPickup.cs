using UnityEngine;

namespace LeagueTry
{
    
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

            // Hide the object to prevent repeated collection.// Ocultamos el objeto para impedir recogerlo repetidamente.
            
            gameObject.SetActive(false);
        }
    }
}