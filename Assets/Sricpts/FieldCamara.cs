using UnityEngine;
namespace LeagueTry
{
    [ExecuteAlways]
public class FieldCamara : MonoBehaviour
   {
     
        public float camaraSize = 6.4f;
        private Camera view;

        private void OnEnable()
        {
            view = GetComponent<Camera>();

        }
        private void Update()
        {
            if(view == null || Screen.height == 0)
            {
                return;
            }
            //with this function I keep 16:9 so the pitch is not distored or cropped. // Conservamos 16:9 para no deformar ni recortar el campo.
            float targetAspect = 16f / 9f;
            float windowAspect = (float)Screen.width / Screen.height;
            view.orthographicSize = Mathf.Max(1f, camaraSize);
            view.aspect = targetAspect;

            if(windowAspect > targetAspect)
            {
                //this is the window center camara with margins on the sides // Ventana ancha centramos la camara dejando margenes laterales.
                float width = targetAspect / windowAspect;
                view.rect = new Rect((1f - width) / 2f, 0f, width,1f);


            }
            else
            {
                //this is the tall window leave space adove and below.// Ventana alta dejamos espacio por arriba y por abajo. 
                float height = windowAspect / targetAspect;
                view.rect = new Rect(0f, (1f - height) / 2f, 1f, height);
            }




        }
        private void OnDisable()
        {
            // when removing the script, return framing control to Unity. // Al quitar el script, devolvemos el control del encuadre a Unity.
            if (view != null)
            {
                view.rect = new Rect(0f, 0f, 1f, 1f);
                view.ResetAspect();
            }

        }
    




    }
}
