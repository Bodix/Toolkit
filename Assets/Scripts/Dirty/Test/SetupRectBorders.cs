using Evolutex.Evolunity.Extensions;
using UnityEngine;

namespace Dirty.Test
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SetupRectBorders : MonoBehaviour
    {
        [SerializeField]
        private float thickness = 1f;
        [Min(0.01f)]
        [Tooltip("The distance from point of view of perspective camera. " +
                 "There will no effect if the camera is orthographic")]
        [SerializeField]
        private float perspectiveZOffset = 1f;
        [SerializeField]
        private new Camera camera;

        private BoxCollider2D referenceCollider;
        private BoxCollider2D topBorder;
        private BoxCollider2D bottomBorder;
        private BoxCollider2D leftBorder;
        private BoxCollider2D rightBorder;

        private void Start()
        {
            if (!camera)
                camera = Camera.main;

            Setup(GetComponent<RectTransform>());
        }

        private void OnValidate()
        {
            referenceCollider = GetComponent<BoxCollider2D>();

            referenceCollider.enabled = false;
        }

        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.color = Color.green;
        //     if (topBorder)
        //         Gizmos.DrawSphere(topBorder.transform.position, thickness / 10f);
        //     if (bottomBorder)
        //         Gizmos.DrawSphere(bottomBorder.transform.position, thickness / 10f);
        //     if (leftBorder)
        //         Gizmos.DrawSphere(leftBorder.transform.position, thickness / 10f);
        //     if (rightBorder)
        //         Gizmos.DrawSphere(rightBorder.transform.position, thickness / 10f);
        // }

        private void Setup(RectTransform rectTransform = null)
        {
            Rect rect = rectTransform
                ? rectTransform.GetWorldRect()
                : camera.pixelRect;
            bool convertToWorldSpace = !rectTransform;

            Create();
            SetPositions(rect, convertToWorldSpace);
            SetSizes(rect, convertToWorldSpace);
        }

        private void Create()
        {
            topBorder = new GameObject("Top Border").AddComponent<BoxCollider2D>();
            bottomBorder = new GameObject("Bottom Border").AddComponent<BoxCollider2D>();
            leftBorder = new GameObject("Left Border").AddComponent<BoxCollider2D>();
            rightBorder = new GameObject("Right Border").AddComponent<BoxCollider2D>();

            topBorder.transform.SetParent(transform);
            bottomBorder.transform.SetParent(transform);
            leftBorder.transform.SetParent(transform);
            rightBorder.transform.SetParent(transform);

            topBorder.gameObject.layer = gameObject.layer;
            bottomBorder.gameObject.layer = gameObject.layer;
            leftBorder.gameObject.layer = gameObject.layer;
            rightBorder.gameObject.layer = gameObject.layer;
        }

        private void SetPositions(Rect screenRect, bool convertToWorldSpace)
        {
            Vector3 topBorderPosition = new Vector3(screenRect.x + screenRect.width / 2f, screenRect.yMax);
            Vector3 bottomBorderPosition = new Vector3(screenRect.x + screenRect.width / 2f, screenRect.y);
            Vector3 leftBorderPosition = new Vector3(screenRect.x, screenRect.y + screenRect.height / 2f);
            Vector3 rightBorderPosition = new Vector3(screenRect.xMax, screenRect.y + screenRect.height / 2f);

            if (convertToWorldSpace)
            {
                topBorderPosition = camera.ScreenToWorldPoint(!camera.orthographic
                    ? topBorderPosition.WithZ(perspectiveZOffset)
                    : topBorderPosition);
                bottomBorderPosition = camera.ScreenToWorldPoint(!camera.orthographic
                    ? bottomBorderPosition.WithZ(perspectiveZOffset)
                    : bottomBorderPosition);
                leftBorderPosition = camera.ScreenToWorldPoint(!camera.orthographic
                    ? leftBorderPosition.WithZ(perspectiveZOffset)
                    : leftBorderPosition);
                rightBorderPosition = camera.ScreenToWorldPoint(!camera.orthographic
                    ? rightBorderPosition.WithZ(perspectiveZOffset)
                    : rightBorderPosition);
            }

            topBorder.transform.position = topBorderPosition.AddY(thickness / 2f);
            bottomBorder.transform.position = bottomBorderPosition.AddY(-thickness / 2f);
            leftBorder.transform.position = leftBorderPosition.AddX(-thickness / 2f);
            rightBorder.transform.position = rightBorderPosition.AddX(thickness / 2f);
        }

        private void SetSizes(Rect screenRect, bool convertToWorldSpace)
        {
            float width = screenRect.width;
            float height = screenRect.height;

            if (convertToWorldSpace)
            {
                Vector3 rectMin = !camera.orthographic
                    ? screenRect.min.ToVector3().WithZ(perspectiveZOffset)
                    : screenRect.min.ToVector3();
                Vector3 rectMax = !camera.orthographic
                    ? screenRect.max.ToVector3().WithZ(perspectiveZOffset)
                    : screenRect.max.ToVector3();
            
                width = Vector2.Distance(
                    camera.ScreenToWorldPoint(rectMax.WithY(0)),
                    camera.ScreenToWorldPoint(rectMin));
                height = Vector2.Distance(
                    camera.ScreenToWorldPoint(rectMax.WithX(0)),
                    camera.ScreenToWorldPoint(rectMin));
            }

            topBorder.size = new Vector2(width, thickness);
            bottomBorder.size = new Vector2(width, thickness);
            leftBorder.size = new Vector2(thickness, height);
            rightBorder.size = new Vector2(thickness, height);
        }
    }
}