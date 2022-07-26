#if UNITY_2020_1_OR_NEWER

using System;
using System.Collections.Generic;
using Evolutex.Evolunity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Evolutex.Evolunity.Components
{
    [RequireComponent(typeof(Canvas))]
    public class GraphicTouchHandler : MonoBehaviour
    {
        private Canvas _canvas;

        private readonly RaycastHit[] _hitsBuffer = new RaycastHit[256];

        /// <summary>
        /// The camera that will generate rays for this raycaster.
        /// </summary>
        /// <returns>
        /// - Null if Camera mode is ScreenSpaceOverlay or ScreenSpaceCamera and has no camera.
        /// - canvas.worldCanvas if not null
        /// - Camera.main.
        /// </returns>
        public Camera Camera
        {
            get
            {
                if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    || (_canvas.renderMode == RenderMode.ScreenSpaceCamera && _canvas.worldCamera == null))
                    return null;

                return _canvas.worldCamera ? _canvas.worldCamera : Camera.main;
            }
        }

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Update()
        {
            // if (Input.touchCount > 0)
            // {
            //     if (_canvas == null)
            //         return;
            //
            //     IList<Graphic> graphics = GraphicRegistry.GetRaycastableGraphicsForCanvas(_canvas);
            //     if (graphics == null || graphics.Count == 0)
            //         return;
            //
            //     foreach (Graphic graphic in graphics)
            //     {
            //         if (!graphic.raycastTarget || graphic.canvasRenderer.cull || graphic.depth == -1)
            //             continue;
            //         
            //         if (Camera != null && Camera.WorldToScreenPoint(graphic.rectTransform.position).z >
            //             Camera.farClipPlane)
            //             continue;
            //
            //         foreach (Touch touch in Input.touches)
            //         {
            //             if (!RectTransformUtility.RectangleContainsScreenPoint(
            //                     graphic.rectTransform, touch.position, Camera, graphic.raycastPadding))
            //                 continue;
            //             
            //             if (graphic.Raycast(touch.position, Camera))
            //                 Debug.Log(graphic.name);
            //         }
            //     }

            if (!Input.GetMouseButton(0))
                return;
            
            if (_canvas == null)
                return;

            IList<Graphic> graphics = GraphicRegistry.GetRaycastableGraphicsForCanvas(_canvas);
            if (graphics == null || graphics.Count == 0)
                return;

            List<Graphic> foundGraphics = new List<Graphic>();

            for (int i = 0; i < graphics.Count; ++i)
            {
                if (!graphics[i].raycastTarget || graphics[i].canvasRenderer.cull || graphics[i].depth == -1)
                    continue;

                if (Camera != null && Camera.WorldToScreenPoint(graphics[i].rectTransform.position).z >
                    Camera.farClipPlane)
                    continue;
                
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                        graphics[i].rectTransform, Input.mousePosition, Camera, graphics[i].raycastPadding))
                    continue;

                if (graphics[i].Raycast(Input.mousePosition, Camera))
                    foundGraphics.Add(graphics[i]);
            }

            Debug.Log(foundGraphics.AsString(x => x.name));


            // foreach (Touch touch in Input.touches)
            // {
            //     if (Physics.RaycastNonAlloc(camera.ScreenPointToRay(touch.position), _hitsBuffer) > 0)
            //     {
            //         foreach (RaycastHit hit in _hitsBuffer)
            //         {
            //             hit.transform.GetComponent<>()
            //         }
            //     }
            //     
            //     if (touch.phase == TouchPhase.Began)
            //     {
            //         
            //     }
            // }
        }
    }
}

#endif