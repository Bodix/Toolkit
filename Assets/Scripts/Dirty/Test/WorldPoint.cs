using System;
using Evolutex.Evolunity.Extensions;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace AI
{
    // TODO: Handle behaviour in disabled state.

    [ExecuteAlways]
    public class WorldPoint : MonoBehaviour
    {
        public ObservableList<WorldPoint> ConnectedPoints;

        [SerializeField] private WorldPoint _connectionSlot;

        public bool IsDestroyed { get; private set; }

        private void Awake()
        {
            // For new points, the list is not instanced in OnEnable method yet.
            ConnectedPoints = ConnectedPoints ?? new ObservableList<WorldPoint>();

            // Update links after point duplication or restore links after undoing point deletion.
            foreach (WorldPoint point in ConnectedPoints)
                if (!point.ConnectedPoints.Contains(this))
                    point.ConnectPoint(this);
        }

        private void OnEnable()
        {
            ConnectedPoints.ItemAdded += OnPointAdded;
            ConnectedPoints.ItemRemoved += OnPointRemoved;
        }

        private void OnDisable()
        {
            ConnectedPoints.ItemAdded -= OnPointAdded;
            ConnectedPoints.ItemRemoved -= OnPointRemoved;
        }

        private void OnDestroy()
        {
            IsDestroyed = true;

            foreach (WorldPoint point in ConnectedPoints)
                point.DisconnectPoint(this);
        }

        private void OnValidate()
        {
            enabled = true;

            ConnectedPoints.RemoveDuplicates();

            if (_connectionSlot == this)
            {
                Debug.LogError("Unable to connect to itself");

                _connectionSlot = null;
            }

            if (ConnectedPoints.Contains(_connectionSlot))
            {
                Debug.LogError("This point is already connected");

                _connectionSlot = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.5f);

            foreach (WorldPoint point in ConnectedPoints)
                Gizmos.DrawLine(point.transform.position, transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        [Button("Connect From Slot")]
        private void ConnectFromSlot()
        {
            if (_connectionSlot)
            {
                ConnectPoint(_connectionSlot);

                _connectionSlot = null;
            }
            else
            {
                Debug.LogError("Assign a point in the connection slot");
            }
        }

        [Button("Connect New")]
        public WorldPoint ConnectNewPoint()
        {
            WorldPoint point = CreateNewPoint();
            ConnectPoint(point);

            return point;
        }

        [Button("Create New")]
        public void CreateNew()
        {
            CreateNewPoint();
        }

        public void ConnectPoint(WorldPoint point)
        {
            if (!ConnectedPoints.Contains(point))
                ConnectedPoints.Add(point);
            else throw new InvalidOperationException("Point already connected");
        }

        public bool DisconnectPoint(WorldPoint point)
        {
            return ConnectedPoints.Remove(point);
        }

        private void OnPointAdded(ObservableList<WorldPoint> sender, ListChangeEventArgs<WorldPoint> e)
        {
            // Prevent a null item when adding a first item or removing existing item through the Inspector.
            if (!e.Item)
            {
                ConnectedPoints.Remove(e.Item);

                return;
            }

            if (!e.Item.ConnectedPoints.Contains(this))
                e.Item.ConnectPoint(this);
        }

        private void OnPointRemoved(ObservableList<WorldPoint> sender, ListChangeEventArgs<WorldPoint> e)
        {
            if (!e.Item)
                return;

            if (!e.Item.IsDestroyed)
                e.Item.DisconnectPoint(this);
        }

        private WorldPoint CreateNewPoint()
        {
            WorldPoint point = new GameObject("Point").AddComponent<WorldPoint>();
            point.transform.SetParent(transform.parent);
            point.transform.position = transform.position + new Vector3().Randomize().WithY(0).normalized;

            Selection.activeGameObject = point.gameObject;

            return point;
        }
    }
}