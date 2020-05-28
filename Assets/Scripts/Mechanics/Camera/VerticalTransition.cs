using System.Collections;
using UnityEngine;

namespace Mechanics.Camera
{
    public enum Orientation{Horizontal, Vertical}
    public class VerticalTransition : MonoBehaviour
    {
        public Orientation orientation;
        private CameraFocusOnPlayer _mainCamera;
        private Transform _tr;
        private int _count = 0;

        private void Start()
        {
            _tr = GetComponent<Transform>();
            StartCoroutine(WaitForCamera());
        }
    
        private IEnumerator WaitForCamera()
        {
            GameObject camera = GameObject.Find("Main Camera(Clone)");
            while (camera == null)
            {
                yield return new WaitForSeconds(1);
                camera = GameObject.Find("Main Camera(Clone)");
            }

            _mainCamera = camera.GetComponent<CameraFocusOnPlayer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(_mainCamera!=null)_mainCamera.PlayerInTransition(_tr.position, orientation, other.gameObject);
        }
        
        /*private void OnTriggerStay2D(Collider2D other)
        {
            _count++;
            if(_count < 100) return;
            _count = 0;
            if(_mainCamera!=null)_mainCamera.PlayerInTransition(_tr.position, orientation, other.gameObject);
        }*/

        private void OnTriggerExit2D(Collider2D other)
        {
            if(_mainCamera!=null)_mainCamera.PlayerOutTransition(other.gameObject);
        }
    }
}
