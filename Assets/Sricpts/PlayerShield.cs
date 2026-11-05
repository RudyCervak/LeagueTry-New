using UnityEngine;

namespace LeagueTry
{
   
    public class PlayerShield : MonoBehaviour
    {
        public float protectionSeconds = 1.5f;

        // ES: Visible en el Inspector para comprobar el escudo durante Play.
        // EN: Visible in the Inspector to check the shield during Play.
        public bool hasShield;

        private float protectedUntil;

        public void CollectShield()
        {
            // ES: Guardamos un escudo; recoger otro no acumula cargas.
            // EN: Store one shield; collecting another does not stack charges.
            hasShield = true;
        }

        public bool TryBlockTackle()
        {
            // ES: Durante la protección breve ignoramos nuevos tackles.
            // EN: Ignore new tackles during the short protection period.
            if (Time.time < protectedUntil)
            {
                return true;
            }

            if (hasShield)
            {
                // ES: Consumimos el escudo y damos tiempo para separarse.
                // EN: Consume the shield and give time to move away.
                hasShield = false;
                protectedUntil = Time.time + protectionSeconds;
                return true;
            }

            return false;
        }

        public void ResetShield()
        {
            // ES: Cada intento comienza sin protección.
            // EN: Each attempt starts without protection.
            hasShield = false;
            protectedUntil = 0f;
        }
    }
}