// ------------------------------------------------------------------------------
// OutlineObject.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using System;
using UnityEngine;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Attach to any GameObject to enable outline rendering.
    /// Supports MeshRenderer, SkinnedMeshRenderer, and SpriteRenderer.
    /// Automatically detects the appropriate outline mode based on the renderer type.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class OutlineObject : MonoBehaviour
    {
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField, Range(0f, 20f)] private float _outlineThickness = 3f;
        [SerializeField] private bool _outlineEnabled = true;
        [SerializeField] private bool _autoDetectMode = true;
        [SerializeField] private OutlineMode _outlineMode = OutlineMode.Outline3D;

        private Renderer _cachedRenderer;
        private bool _isSpriteRenderer;
        private bool _isMeshRenderer;

        /// <summary>
        /// The outline color.
        /// </summary>
        public Color OutlineColor
        {
            get => _outlineColor;
            set { _outlineColor = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// The outline thickness (0-20).
        /// </summary>
        public float OutlineThickness
        {
            get => _outlineThickness;
            set { _outlineThickness = Mathf.Clamp(value, 0f, 20f); OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether the outline is enabled.
        /// </summary>
        public bool OutlineEnabled
        {
            get => _outlineEnabled;
            set { _outlineEnabled = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether to automatically detect the outline mode based on renderer type.
        /// When enabled: SpriteRenderer → Outline2D, MeshRenderer/SkinnedMeshRenderer → Outline3D
        /// </summary>
        public bool AutoDetectMode
        {
            get => _autoDetectMode;
            set
            {
                _autoDetectMode = value;
                if (_autoDetectMode)
                {
                    DetectAndSetMode();
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The outline rendering mode. When AutoDetectMode is enabled, this is set automatically.
        /// </summary>
        public OutlineMode Mode
        {
            get => _outlineMode;
            set
            {
                if (_autoDetectMode && _outlineMode != value)
                {
                    Debug.LogWarning("[OutlineObject] AutoDetectMode is enabled. Manual mode changes will be overridden. " +
                        "Set AutoDetectMode to false to manually control the mode.");
                }
                _outlineMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The detected renderer component on this GameObject.
        /// </summary>
        public Renderer CachedRenderer => _cachedRenderer;

        /// <summary>
        /// Whether this is a 2D sprite object.
        /// </summary>
        public bool IsSpriteRenderer => _isSpriteRenderer;

        /// <summary>
        /// Whether this is a 3D mesh object.
        /// </summary>
        public bool IsMeshRenderer => _isMeshRenderer;

        /// <summary>
        /// Event invoked when any outline property changes.
        /// </summary>
        public event Action OnChanged;

        private void Awake()
        {
            CacheRenderer();
            if (_autoDetectMode)
            {
                DetectAndSetMode();
            }
        }

        private void OnEnable()
        {
            // Ensure manager exists before registering
            OutlineManager.GetOrCreateInstance();
            OutlineManager.Instance?.Register(this);
        }

        private void OnDisable()
        {
            OutlineManager.Instance?.Unregister(this);
        }

        private void OnValidate()
        {
            CacheRenderer();
            if (_autoDetectMode)
            {
                DetectAndSetMode();
            }
            OnPropertyChanged();
        }

        private void Reset()
        {
            CacheRenderer();
            if (_autoDetectMode)
            {
                DetectAndSetMode();
            }
        }

        /// <summary>
        /// Force refresh the outline mode detection.
        /// </summary>
        [ContextMenu("Refresh Mode Detection")]
        public void RefreshModeDetection()
        {
            CacheRenderer();
            DetectAndSetMode();
            OnPropertyChanged();
        }

        private void CacheRenderer()
        {
            _cachedRenderer = GetComponent<Renderer>();
            _isSpriteRenderer = _cachedRenderer is SpriteRenderer;
            _isMeshRenderer = (_cachedRenderer is MeshRenderer || _cachedRenderer is SkinnedMeshRenderer) && !_isSpriteRenderer;
        }

        private void DetectAndSetMode()
        {
            if (_isSpriteRenderer)
            {
                _outlineMode = OutlineMode.Outline2D;
            }
            else if (_isMeshRenderer)
            {
                _outlineMode = OutlineMode.Outline3D;
            }
        }

        private void OnPropertyChanged()
        {
            OnChanged?.Invoke();
        }

    }

    public enum OutlineMode
    {
        Outline3D,
        ScreenSpace,
        Outline2D
    }
}