using UnityEngine;

namespace LeagueTry
{
    
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class FieldCamara : MonoBehaviour
    {
        // ES: Tamaño de la cámara en la escena 2D original.
        // EN: Camera size in the original 2D scene.
        public float camaraSize = 6.4f;

        // ES: En la escena 3D, arrastra Player a Target.
        // EN: In the 3D scene, drag Player into Target.
        public Transform target;
        public Vector3 offset = new Vector3(0f, 6f, -9f);

        private Camera view;

        private void OnEnable()
        {
            view = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (view == null) return;

            if (target != null)
            {
                // ES: En 3D usamos perspectiva y toda la ventana.
                // EN: In 3D, use perspective and the full viewport.
                view.orthographic = false;
                view.rect = new Rect(0f, 0f, 1f, 1f);
                view.ResetAspect();

                // ES: Seguimos a Player, no a Visual, para que la cámara
                // permanezca estable cuando el personaje salta.
                // EN: Follow Player, not Visual, so the camera stays
                // steady when the character jumps.
                transform.position = target.position + offset;

                Vector3 lookPoint = target.position + new Vector3(0f, 1f, 5f);
                transform.LookAt(lookPoint);
                return;
            }

            // ES: Sin Target, conservamos el encuadre 16:9 de Project 1.
            // EN: Without a target, keep the 16:9 framing from Project 1.
            if (Screen.width == 0 || Screen.height == 0) return;

            float targetAspect = 16f / 9f;
            float windowAspect = (float)Screen.width / Screen.height;

            view.orthographic = true;
            view.orthographicSize = Mathf.Max(1f, camaraSize);
            view.aspect = targetAspect;

            if (windowAspect > targetAspect)
            {
                float width = targetAspect / windowAspect;
                view.rect = new Rect((1f - width) / 2f, 0f, width, 1f);
            }
            else
            {
                float height = windowAspect / targetAspect;
                view.rect = new Rect(0f, (1f - height) / 2f, 1f, height);
            }
        }

        private void OnDisable()
        {
            if (view == null) return;

            view.rect = new Rect(0f, 0f, 1f, 1f);
            view.ResetAspect();
        }
    }
}