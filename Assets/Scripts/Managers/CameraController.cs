using System;
using UnityEngine;

namespace Managers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]private Camera cam;
        private void Start()
        {
            EventManager.GetInstance.onGameStart += AdjustCamera;
        }

        private void AdjustCamera(int w,int h,int c)
        {
            transform.position = new Vector3(w/2f-0.5f, h/2f-0.5f,-10f);
            cam.orthographicSize = Mathf.Max(w, h) * 1.1f;
        }
    }
}