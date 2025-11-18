/*
File Name:    FighterCameraController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterCameraController : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        [SerializeField]
        private Camera cam;

        private float defaultFOV;
        private Vector3 defaultLocalPos;

        private int zoomTimer;
        private int zoomMax;
        private bool zooming;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
            defaultFOV = cam.fieldOfView;
            defaultLocalPos = cam.transform.localPosition;
        }

        void Awake()
        {

        }

        public void StartUltimateZoom(int frames)
        {
            zooming = true;
            zoomTimer = frames;
            zoomMax = frames;
        }

        public void ResetUltimate()
        {
            zooming = false;
            cam.fieldOfView = defaultFOV;
            cam.transform.localPosition = defaultLocalPos;
        }

        public void SimulateCameraFrame()
        {
            if (!zooming) return;

            float t = 1f - (float)zoomTimer / zoomMax;

            cam.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV * 0.55f, t);

            cam.transform.localPosition =
                Vector3.Lerp(defaultLocalPos, defaultLocalPos + new Vector3(0, 0, -2f), t);

            zoomTimer--;
        }
    }

}