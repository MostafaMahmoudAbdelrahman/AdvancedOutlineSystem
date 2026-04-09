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
    /// </summary>
    [DisallowMultipleComponent]
    public class OutlineObject : MonoBehaviour
    {
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField, Range(0f, 20f)] private float _outlineThickness = 3f;
        [SerializeField] private bool _outlineEnabled = true;
        [SerializeField] private OutlineMode _outlineMode = OutlineMode.Outline3D;

        public Color OutlineColor
        {
            get => _outlineColor;
            set { _outlineColor = value; OnPropertyChanged(); }
        }

        public float OutlineThickness
        {
            get => _outlineThickness;
            set { _outlineThickness = Mathf.Clamp(value, 0f, 20f); OnPropertyChanged(); }
        }

        public bool OutlineEnabled
        {
            get => _outlineEnabled;
            set { _outlineEnabled = value; OnPropertyChanged(); }
        }

        public OutlineMode Mode
        {
            get => _outlineMode;
            set { _outlineMode = value; OnPropertyChanged(); }
        }

        public event Action OnChanged;

        private void OnEnable()
        {
            OutlineManager.Instance?.Register(this);
        }

        private void OnDisable()
        {
            OutlineManager.Instance?.Unregister(this);
        }

        private void OnValidate()
        {
            OnPropertyChanged();
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
