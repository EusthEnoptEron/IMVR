using UnityEngine;
using System.Collections;

namespace GalleryVR.Dbg
{
    [AddComponentMenu("Debug/Earth Placer")]
    public class EarthPlacer : MonoBehaviour
    {
        /// <summary>
        /// Degrees that determine the vertical position
        /// </summary>
        public float latitude = 46.95f;
        /// <summary>
        /// Degrees that determine the horizontal position
        /// </summary>
        public float longitude = 7.45f;

        public float radius = 7f;

        private GameObject point;
        // Use this for initialization
        void Start()
        {
            point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.parent = transform;
            point.transform.localScale *= 0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            float lat = latitude  * Mathf.PI / 180f;
            float lon = longitude * Mathf.PI / 180f;
 
            point.transform.localPosition = new Vector3(
                Mathf.Cos(lat) * Mathf.Sin(-lon),
                Mathf.Sin(lat),
                Mathf.Cos(lat) * Mathf.Cos(-lon)
            ) * radius;
        }
    }

}