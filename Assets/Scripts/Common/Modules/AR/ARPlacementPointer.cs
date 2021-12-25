// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using UnityEngine;

namespace AR
{
    // TODO: Make the class more versatile and reusable.
    
    public class ARPlacementPointer : MonoBehaviour
    {
        public Color HitColor = Color.green;
        public Color MissColor = Color.red;

        [SerializeField]
        private ARRaycaster raycaster = null;
        [SerializeField]
        private SpriteRenderer outerSpriteRenderer = null;
        [SerializeField]
        private SpriteRenderer innerSpriteRenderer = null;

        private void Awake()
        {
            raycaster.RaycastHit += hits =>
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;

                ToggleSprites(true);
                outerSpriteRenderer.color = HitColor;
                innerSpriteRenderer.color = HitColor;
            };
            raycaster.RaycastMiss += () =>
            {
                outerSpriteRenderer.color = MissColor;
                innerSpriteRenderer.color = MissColor;
            };

            gameObject.SetActive(false);
            ToggleSprites(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ToggleSprites(bool isOn)
        {
            outerSpriteRenderer.enabled = isOn;
            innerSpriteRenderer.enabled = isOn;
        }
    }
}