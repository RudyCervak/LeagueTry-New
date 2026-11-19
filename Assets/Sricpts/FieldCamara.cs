using UnityEngine;

namespace LeagueTry
{
    
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class FieldCamara : MonoBehaviour
    {
        // This Camera size in the original 2D scene.// Tamaño de la cámara en la escena 2D original.
        
        public float camaraSize = 6.4f;

        // In the 3D scene, drag Player into Target.// En la escena 3D, arrastra Player a Target.
       
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
                // In the 3D, use perspective and the full viewport.// En 3D usamos perspectiva y toda la ventana.
             
                view.orthographic = false;
                view.rect = new Rect(0f, 0f, 1f, 1f);
                view.ResetAspect();

                // Seguimos a Player, no a Visual, para que la cámara permanezca estable cuando el personaje salta.//Follow Player, not Visual, so the camera stay steady when the character jumps.
               
                
                transform.position = target.position + offset;

                Vector3 lookPoint = target.position + new Vector3(0f, 1f, 5f);
                transform.LookAt(lookPoint);
                return;
            }

            // Without a target, keep the 16:9 framing from Project 1.// Sin Target, conservamos el encuadre 16:9 de Project 1.
            
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