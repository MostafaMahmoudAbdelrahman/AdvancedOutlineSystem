// ------------------------------------------------------------------------------
// DemoSceneSetup.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using AdvancedOutlineSystem;

namespace AdvancedOutlineSystem.Samples
{
    /// <summary>
    /// Procedurally creates demo objects at runtime.
    /// No hard scene dependencies — attach to any empty GameObject.
    /// </summary>
    public class DemoSceneSetup : MonoBehaviour
    {
        [Header("3D Outline")]
        public Color color3D     = Color.cyan;
        public float thickness3D = 4f;

        [Header("Screen-Space Outline")]
        public Color colorSS     = Color.yellow;
        public float thicknessSS = 1.5f;

        [Header("2D Outline")]
        public Color color2D     = Color.green;
        public float thickness2D = 3f;

        private void Start()
        {
            EnsureManager();
            Create3DDemo();
            Create2DDemo();
            CreateScreenSpaceDemo();
        }

        private void EnsureManager()
        {
            if (OutlineManager.Instance != null) return;
            new GameObject("OutlineManager").AddComponent<OutlineManager>();
        }

        private void Create3DDemo()
        {
            SpawnPrimitive(PrimitiveType.Cube,    new Vector3(-3, 0, 0), color3D,    thickness3D, OutlineMode.Outline3D);
            SpawnPrimitive(PrimitiveType.Sphere,  new Vector3( 0, 0, 0), Color.red,  thickness3D, OutlineMode.Outline3D);
            SpawnPrimitive(PrimitiveType.Capsule, new Vector3( 3, 0, 0), Color.magenta, thickness3D, OutlineMode.Outline3D);
        }

        private void Create2DDemo()
        {
            var go = new GameObject("OutlinedSprite");
            go.transform.position = new Vector3(0, -3, 0);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            AddOutline(go, color2D, thickness2D, OutlineMode.Outline2D);
        }

        private void CreateScreenSpaceDemo()
        {
            var go = new GameObject("ScreenSpaceController");
            go.transform.position = new Vector3(6, 0, 0);
            go.AddComponent<MeshFilter>().sharedMesh =
                Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            go.AddComponent<MeshRenderer>();
            AddOutline(go, colorSS, thicknessSS, OutlineMode.ScreenSpace);
        }

        private void SpawnPrimitive(PrimitiveType type, Vector3 pos,
            Color color, float thickness, OutlineMode mode)
        {
            var go = GameObject.CreatePrimitive(type);
            go.transform.position = pos;
            AddOutline(go, color, thickness, mode);
        }

        private static void AddOutline(GameObject go, Color color,
            float thickness, OutlineMode mode)
        {
            var o = go.AddComponent<OutlineObject>();
            o.OutlineColor     = color;
            o.OutlineThickness = thickness;
            o.Mode             = mode;
            o.OutlineEnabled   = true;
        }
    }
}
