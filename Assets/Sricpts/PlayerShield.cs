using UnityEngine;

namespace LeagueTry
{
   
    public class PlayerShield : MonoBehaviour
    {
        public float protectionSeconds = 1.5f;

        // Visible in the Inspector to check the shield during Play.// Visible en el Inspector para comprobar el escudo durante Play.
        
        public bool hasShield;

        private float protectedUntil;

        public void CollectShield()
        {
            // Store one shield; collecting another does not stack charges.// Guardamos un escudo; recoger otro no acumula cargas.
            
            hasShield = true;
        }

        public bool TryBlockTackle()
        {
            // Ignore new tackles during the short protection period.// Durante la protección breve ignoramos nuevos tackles.

            if (Time.time < protectedUntil)
            {
                return true;
            }

            if (hasShield)
            {
                // Consume the shield and give time to move away.// Consumimos el escudo y damos tiempo para separarse.
                
                hasShield = false;
                protectedUntil = Time.time + protectionSeconds;
                return true;
            }

            return false;
        }

        public void ResetShield()
        {
            // Each attempt starts without protection.// Cada intento comienza sin protección.
            
            hasShield = false;
            protectedUntil = 0f;
        }
    }
}