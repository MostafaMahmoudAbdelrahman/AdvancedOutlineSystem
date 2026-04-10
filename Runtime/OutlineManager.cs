// ------------------------------------------------------------------------------
// OutlineManager.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Singleton manager that tracks all active OutlineObjects and
    /// provides batched data to the render feature.
    /// Can be created automatically when an OutlineObject is enabled.
    /// </summary>
    [DisallowMultipleComponent]
    public class OutlineManager : MonoBehaviour
    {
        private static OutlineManager _instance;
        public static OutlineManager Instance => _instance;

        private readonly List<OutlineObject> _objects3D     = new List<OutlineObject>();
        private readonly List<OutlineObject> _objectsScreen = new List<OutlineObject>();
        private readonly List<OutlineObject> _objects2D     = new List<OutlineObject>();

        public IReadOnlyList<OutlineObject> Objects3D     => _objects3D;
        public IReadOnlyList<OutlineObject> ObjectsScreen => _objectsScreen;
        public IReadOnlyList<OutlineObject> Objects2D     => _objects2D;

        /// <summary>
        /// Gets or creates an OutlineManager instance.
        /// Use this to ensure the manager exists before registering objects.
        /// </summary>
        public static OutlineManager GetOrCreateInstance()
        {
            if (_instance != null) return _instance;

            // Try to find existing manager in scene
            var managerObject = FindObjectOfType<OutlineManager>();
            if (managerObject != null)
            {
                return managerObject;
            }

            // Create new manager
            var go = new GameObject("OutlineManager");
            _instance = go.AddComponent<OutlineManager>();
            DontDestroyOnLoad(go);
            Debug.Log("[OutlineSystem] OutlineManager created automatically.");
            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public void Register(OutlineObject obj)
        {
            if (obj == null) return;
            GetList(obj.Mode).Add(obj);
        }

        public void Unregister(OutlineObject obj)
        {
            if (obj == null) return;
            _objects3D.Remove(obj);
            _objectsScreen.Remove(obj);
            _objects2D.Remove(obj);
        }

        /// <summary>Re-buckets an object after its Mode property changes.</summary>
        public void RefreshObject(OutlineObject obj)
        {
            Unregister(obj);
            if (obj.OutlineEnabled)
                Register(obj);
        }

        private List<OutlineObject> GetList(OutlineMode mode)
        {
            switch (mode)
            {
                case OutlineMode.ScreenSpace: return _objectsScreen;
                case OutlineMode.Outline2D:   return _objects2D;
                default:                      return _objects3D;
            }
        }
    }
}
