using UnityEngine;

namespace LeagueTry
{
    public class PickupRandomizer : MonoBehaviour
    {
        public float minimumX = -8f;
        public float maximumX = 8f;
        public float minimumZ = 6f;
        public float maximumZ = 52f;

        [Min(0f)] public float distanceFromSpawn = 5f;
        [Min(0f)] public float distanceBetweenPickups = 3f;
        [Min(0.1f)] public float clearanceRadius = 1.25f;

        [Min(1)] public int placementAttempts = 100;

        public void RespawnPickups(
            GameObject shieldPickup,
            GameObject speedPickup,
            Vector3 spawnPosition)
        {
            // Hide both pickups before changing their positions.// Ocultamos ambos powerups antes de cambiar sus posiciones.

            if (shieldPickup != null)
            {
                shieldPickup.SetActive(false);
            }

            if (speedPickup != null)
            {
                speedPickup.SetActive(false);
            }

            // Update physics positions before checking for nearby players.// Actualizamos las posiciones físicas antes de buscar jugadores cercanos.

            Physics.SyncTransforms();

            // Place the shield first and reserve its position.// Colocamos primero el escudo y reservamos su posición.

            GameObject placedShield = null;

            if (TryPlacePickup(shieldPickup, spawnPosition, null))
            {
                placedShield = shieldPickup;
            }

            // Place the speed boost away from the shield.// Colocamos el boost de velocidad separado del escudo.

            TryPlacePickup(speedPickup, spawnPosition, placedShield);
        }

        private bool TryPlacePickup(
            GameObject pickup,
            Vector3 spawnPosition,
            GameObject otherPickup)
        {
            if (pickup == null)
            {
                return false;
            }

            // Keep the current height so the pickup stays above the pitch.// Conservamos la altura actual para mantener el powerup sobre el campo.

            float height = pickup.transform.position.y;

            for (int attempt = 0;
                 attempt < Mathf.Max(1, placementAttempts);
                 attempt++)
            {
                // Randomise X and Z inside the allowed area.// Elegimos X y Z aleatorios dentro del área permitida.

                Vector3 position = new Vector3(
                    Random.Range(minimumX, maximumX),
                    height,
                    Random.Range(minimumZ, maximumZ)
                );

                // Keep pickups away from the player's starting point.// Mantenemos los powerups alejados del punto inicial del jugador.

                if (DistanceOnPitch(position, spawnPosition) <
                    distanceFromSpawn)
                {
                    continue;
                }

                // Prevent both pickups from appearing too close together.// Evitamos que ambos powerups aparezcan demasiado juntos.

                if (otherPickup != null &&
                    DistanceOnPitch(
                        position,
                        otherPickup.transform.position
                    ) < distanceBetweenPickups)
                {
                    continue;
                }

                // Reject positions occupied by the player or defenders.// Rechazamos posiciones ocupadas por el jugador o los defensores.

                if (HasNearbyPlayer(position))
                {
                    continue;
                }

                pickup.transform.position = position;
                pickup.SetActive(true);

                return true;
            }

            // Keep the pickup hidden if no valid position is found.// Mantenemos el powerup oculto si no encontramos una posición válida.

            Debug.LogWarning(
                "No free position found for " + pickup.name +
                ". Check PickupRandomizer limits and distances.",
                this
            );

            return false;
        }

        private bool HasNearbyPlayer(Vector3 position)
        {
            // Include trigger colliders when checking the area.// Incluimos los colliders de tipo trigger al comprobar el área.

            Collider[] nearbyColliders = Physics.OverlapSphere(
                position,
                clearanceRadius,
                Physics.AllLayers,
                QueryTriggerInteraction.Collide
            );

            foreach (Collider nearby in nearbyColliders)
            {
                PitchMovement player =
                    nearby.GetComponentInParent<PitchMovement>();

                RugbyDefender defender =
                    nearby.GetComponentInParent<RugbyDefender>();

                if (player != null || defender != null)
                {
                    return true;
                }
            }

            return false;
        }

        private float DistanceOnPitch(Vector3 first, Vector3 second)
        {
            // Measure distance along the pitch without using height.// Medimos la distancia sobre el campo sin utilizar la altura.

            Vector2 firstPosition = new Vector2(first.x, first.z);
            Vector2 secondPosition = new Vector2(second.x, second.z);

            return Vector2.Distance(firstPosition, secondPosition);
        }
    }
}