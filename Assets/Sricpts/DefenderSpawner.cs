using System.Collections.Generic;
using UnityEngine;

namespace LeagueTry
{
    [DisallowMultipleComponent]
    public class DefenderSpawner : MonoBehaviour
    {
        public RugbyDefender[] startingDefenders =
            new RugbyDefender[3];

        [Min(1)] public int defendersPerRound = 1;

        // Zero allows the number of defenders to keep increasing.// Cero permite que el número de defensores siga aumentando.

        [Min(0)] public int maximumDefenders = 0;

        public float minimumZ = 12f;
        public float maximumZ = 50f;

        private readonly List<RugbyDefender> defenders =
            new List<RugbyDefender>();

        private int startingCount;
        private int preparedRound = -1;

        public void PrepareRound(RoundManager rounds)
        {
            if (!enabled || rounds == null || rounds.isGameOver)
            {
                return;
            }

            // Prepare each round only once, even after losing a life.// Preparamos cada ronda una sola vez, incluso después de perder una vida.

            if (preparedRound == rounds.currentRound)
            {
                return;
            }

            // Register the defenders already placed in the scene.// Registramos los defensores que ya están colocados en la escena.

            if (defenders.Count == 0)
            {
                if (startingDefenders != null)
                {
                    foreach (RugbyDefender defender in startingDefenders)
                    {
                        if (defender != null &&
                            !defenders.Contains(defender))
                        {
                            defenders.Add(defender);
                        }
                    }
                }

                if (defenders.Count == 0)
                {
                    Debug.LogError(
                        "Assign the scene defenders in DefenderSpawner.",
                        this
                    );

                    enabled = false;
                    return;
                }

                startingCount = defenders.Count;
            }

            // Calculate how many defenders are needed for this round.// Calculamos cuántos defensores necesita esta ronda.

            int extraDefenders =
                Mathf.Max(0, rounds.currentRound - 1) *
                Mathf.Max(1, defendersPerRound);

            int targetCount = startingCount + extraDefenders;

            // Apply an optional limit without removing the starting defenders.// Aplicamos un límite opcional sin quitar los defensores iniciales.

            if (maximumDefenders > 0)
            {
                targetCount = Mathf.Min(
                    targetCount,
                    Mathf.Max(startingCount, maximumDefenders)
                );
            }

            // Use the first defender as the model for new opponents.// Usamos el primer defensor como modelo para los nuevos rivales.

            RugbyDefender template = defenders[0];

            while (defenders.Count < targetCount)
            {
                RugbyDefender newDefender = Instantiate(
                    template,
                    template.transform.position,
                    template.transform.rotation,
                    template.transform.parent
                );

                newDefender.name =
                    "Defender (" + defenders.Count + ")";

                newDefender.rounds = rounds;

                defenders.Add(newDefender);
            }

            // Spread defenders along the pitch and preserve their height.// Distribuimos los defensores a lo largo del campo y conservamos su altura.

            float firstZ = Mathf.Min(minimumZ, maximumZ);
            float lastZ = Mathf.Max(minimumZ, maximumZ);

            for (int i = 0; i < defenders.Count; i++)
            {
                RugbyDefender defender = defenders[i];

                defender.rounds = rounds;

                Rigidbody body = defender.GetComponent<Rigidbody>();

                float positionAlongPitch = defenders.Count > 1
                    ? (float)i / (defenders.Count - 1)
                    : 0.5f;

                Vector3 position = body.position;

                // Choose a horizontal position inside this defender's limits.// Elegimos una posición horizontal dentro de los límites de este defensor.

                position.x = Random.Range(
                    Mathf.Min(defender.minimumX, defender.maximumX),
                    Mathf.Max(defender.minimumX, defender.maximumX)
                );

                position.z = Mathf.Lerp(
                    firstZ,
                    lastZ,
                    positionAlongPitch
                );

                body.position = position;
            }

            preparedRound = rounds.currentRound;
        }
    }
}