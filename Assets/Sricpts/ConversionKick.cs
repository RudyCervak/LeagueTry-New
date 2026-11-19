using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LeagueTry
{
    [DisallowMultipleComponent]
    public class ConversionKick : MonoBehaviour
    {
        public GameObject conversionPanel;
        public Text instructionText;
        public Slider accuracySlider;
        public RectTransform successZone;

        public Camera gameCamera;
        public Camera conversionCamera;
        public Transform kickPoint;
        public Transform goalTarget;
        public Transform kickBall;

        public GameObject[] hideDuringKick;

        [Min(0.1f)] public float meterSpeed = 0.8f;
        [Range(0.02f, 0.4f)] public float successTolerance = 0.12f;
        [Min(0.1f)] public float flightDuration = 1.4f;
        [Min(0f)] public float arcHeight = 5f;
        [Min(0.1f)] public float missOffset = 8f;
        [Min(0f)] public float resultDuration = 1.5f;

        public bool IsRunning { get; private set; }

        private RoundManager rounds;

        private bool shotTaken;
        private bool waitingForRelease;

        private float meterPhase;
        private float previousTimeScale;

        private bool previousGameCameraState;
        private bool previousConversionCameraState;

        private readonly List<GameObject> hiddenObjects =
            new List<GameObject>();

        private void Awake()
        {
            // Keep conversion objects hidden during normal gameplay.// Mantenemos ocultos los objetos de conversión durante la partida normal.

            if (conversionPanel != null)
            {
                conversionPanel.SetActive(false);
            }

            if (conversionCamera != null &&
                conversionCamera != gameCamera)
            {
                conversionCamera.enabled = false;
            }

            if (kickBall != null)
            {
                kickBall.gameObject.SetActive(false);
            }
        }

        public bool BeginKick(RoundManager manager)
        {
            if (!isActiveAndEnabled ||
                IsRunning ||
                manager == null)
            {
                return false;
            }

            // Check references before pausing the game.// Comprobamos las referencias antes de pausar la partida.

            if (conversionPanel == null ||
                instructionText == null ||
                accuracySlider == null ||
                accuracySlider.handleRect == null ||
                successZone == null ||
                gameCamera == null ||
                conversionCamera == null ||
                conversionCamera == gameCamera ||
                !conversionCamera.gameObject.activeInHierarchy ||
                kickPoint == null ||
                goalTarget == null ||
                kickBall == null)
            {
                Debug.LogError(
                    "Assign the panel, text, slider, success zone, " +
                    "two different cameras, kick point, goal target " +
                    "and ball in ConversionKick.",
                    this
                );

                return false;
            }

            rounds = manager;

            shotTaken = false;
            waitingForRelease = true;
            meterPhase = Random.Range(0f, 2f);

            previousTimeScale = Time.timeScale;
            previousGameCameraState = gameCamera.enabled;
            previousConversionCameraState = conversionCamera.enabled;

            IsRunning = true;

            // Pause physics while the kick uses unscaled time.// Pausamos la física mientras la patada utiliza tiempo sin escalar.

            Time.timeScale = 0f;

            // Hide defenders and selected gameplay objects temporarily.// Ocultamos temporalmente los defensores y los objetos seleccionados de la partida.

            hiddenObjects.Clear();

            RugbyDefender[] defenders =
                FindObjectsByType<RugbyDefender>(
                    FindObjectsSortMode.None
                );

            foreach (RugbyDefender defender in defenders)
            {
                if (defender.rounds == manager)
                {
                    HideObject(defender.gameObject);
                }
            }

            if (hideDuringKick != null)
            {
                foreach (GameObject target in hideDuringKick)
                {
                    HideObject(target);
                }
            }

            HideObject(manager.shieldPickup);
            HideObject(manager.speedPickup);

            // Match the green zone to the slider handle's movement area.// Ajustamos la zona verde al área de movimiento del indicador.

            accuracySlider.minValue = 0f;
            accuracySlider.maxValue = 1f;
            accuracySlider.wholeNumbers = false;

            accuracySlider.direction =
                Slider.Direction.LeftToRight;

            accuracySlider.interactable = false;

            accuracySlider.transition =
                Selectable.Transition.None;

            accuracySlider.value =
                Mathf.PingPong(meterPhase, 1f);

            successZone.SetParent(
                accuracySlider.handleRect.parent,
                false
            );

            successZone.SetAsFirstSibling();

            successZone.anchorMin = new Vector2(
                0.5f - successTolerance,
                0f
            );

            successZone.anchorMax = new Vector2(
                0.5f + successTolerance,
                1f
            );

            successZone.offsetMin = Vector2.zero;
            successZone.offsetMax = Vector2.zero;
            successZone.localScale = Vector3.one;

            kickBall.position = kickPoint.position;
            kickBall.rotation = kickPoint.rotation;
            kickBall.gameObject.SetActive(true);

            gameCamera.enabled = false;
            conversionCamera.enabled = true;

            instructionText.text =
                "CONVERSION KICK\n" +
                "Press SPACE in the green zone to recover one life.";

            conversionPanel.SetActive(true);

            return true;
        }

        private void Update()
        {
            if (!IsRunning || shotTaken)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;

            // Release Space first so the last jump does not trigger a kick.// Soltamos Espacio primero para que el último salto no active una patada.

            if (waitingForRelease)
            {
                if (keyboard == null ||
                    !keyboard.spaceKey.isPressed)
                {
                    waitingForRelease = false;
                }
            }
            else if (keyboard != null &&
                     keyboard.spaceKey.wasPressedThisFrame)
            {
                // Evaluate the marker position that was visible before this press.// Evaluamos la posición del indicador visible antes de esta pulsación.

                float error = accuracySlider.value - 0.5f;

                bool success =
                    Mathf.Abs(error) <= successTolerance;

                shotTaken = true;

                StartCoroutine(ShowKick(success, error));

                return;
            }

            meterPhase += Time.unscaledDeltaTime * meterSpeed;

            accuracySlider.value =
                Mathf.PingPong(meterPhase, 1f);
        }

        private IEnumerator ShowKick(bool success, float error)
        {
            // Animate the ball; the timing bar determines the result.// Animamos el balón; la barra de precisión determina el resultado.

            instructionText.text = "KICK!";

            Vector3 start = kickPoint.position;
            Vector3 target = goalTarget.position;

            Vector3 forward = target - start;
            forward.y = 0f;

            forward = forward.sqrMagnitude > 0.001f
                ? forward.normalized
                : Vector3.forward;

            Vector3 right =
                Vector3.Cross(Vector3.up, forward);

            if (!success)
            {
                target += right *
                    (error < 0f ? -missOffset : missOffset);
            }

            // Continue beyond the goal line to show the ball passing through.// Continuamos más allá de la línea para mostrar el paso del balón.

            Vector3 end = target + forward * 4f;

            float elapsed = 0f;
            float duration = Mathf.Max(0.1f, flightDuration);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress =
                    Mathf.Clamp01(elapsed / duration);

                kickBall.position =
                    Vector3.Lerp(start, end, progress) +
                    Vector3.up *
                    Mathf.Sin(progress * Mathf.PI) *
                    arcHeight;

                kickBall.Rotate(
                    Vector3.right,
                    540f * Time.unscaledDeltaTime,
                    Space.Self
                );

                yield return null;
            }

            kickBall.position = end;

            if (success)
            {
                instructionText.text =
                    rounds.livesRemaining < rounds.startingLives
                    ? "CONVERTED!\n+1 LIFE"
                    : "CONVERTED!\nLIVES ALREADY FULL";
            }
            else
            {
                instructionText.text =
                    "MISSED!\nNo life lost.";
            }

            yield return new WaitForSecondsRealtime(
                resultDuration
            );

            // Queue the result before restoring the normal view.// Registramos el resultado antes de restaurar la vista normal.

            rounds.RequestConversionResult(success);

            RestoreView();
        }

        private void HideObject(GameObject target)
        {
            if (target == null ||
                !target.activeInHierarchy ||
                hiddenObjects.Contains(target))
            {
                return;
            }

            // Keep the controller and its parent objects active.// Mantenemos activos el controlador y sus objetos padres.

            if (transform.IsChildOf(target.transform))
            {
                return;
            }

            hiddenObjects.Add(target);

            target.SetActive(false);
        }

        private void RestoreView()
        {
            IsRunning = false;

            if (conversionPanel != null)
            {
                conversionPanel.SetActive(false);
            }

            if (kickBall != null)
            {
                kickBall.gameObject.SetActive(false);
            }

            if (gameCamera != null)
            {
                gameCamera.enabled =
                    previousGameCameraState;
            }

            if (conversionCamera != null)
            {
                conversionCamera.enabled =
                    previousConversionCameraState;
            }

            foreach (GameObject target in hiddenObjects)
            {
                if (target != null)
                {
                    target.SetActive(true);
                }
            }

            hiddenObjects.Clear();

            Time.timeScale = previousTimeScale;
        }

        public void CancelKick()
        {
            if (!IsRunning)
            {
                return;
            }

            StopAllCoroutines();

            RestoreView();
        }

        private void OnDisable()
        {
            // Restore the view if the component is disabled during a kick.// Restauramos la vista si el componente se desactiva durante una patada.

            CancelKick();
        }
    }
}