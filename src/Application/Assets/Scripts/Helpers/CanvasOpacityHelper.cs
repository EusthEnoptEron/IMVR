using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace IMVR.Helper
{
    /// <summary>
    /// Makes sure that mesh opacity correspondences to that of parenting canvas.
    /// </summary>
    public class CanvasOpacityHelper : MonoBehaviour
    {
        private CanvasGroup[] groups;
        private Dictionary<Material, float> materials = new Dictionary<Material, float>();
        private float m_opacity = 1;

        public bool useSharedMaterial = false;

        // Use this for initialization
        void Start()
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in renderers)
            {
                var newMaterials = useSharedMaterial ? renderer.sharedMaterials : renderer.materials;

                foreach (var material in newMaterials)
                {
                    if (!materials.ContainsKey(material))
                    {
                        materials.Add(material, material.color.a);
                    }
                }
            }

            if (materials.Count == 0) enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            groups = GetComponentsInParent<CanvasGroup>();
            float opacity = groups.Aggregate(1f, (factor, group) => group.alpha * factor);
            if (opacity != m_opacity)
            {
                m_opacity = opacity;

                foreach (var materialEntry in materials)
                {
                    var mat = materialEntry.Key;
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, m_opacity * materialEntry.Value);
                }
            }
        }
    }
}
