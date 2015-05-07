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

        public float rotationSpeed = 10;

        public GameObject markerPrefab;

        private GameObject point;

        private float actualRadius
        {
            get
            {
                if (longitude == 0 && latitude == 0)
                    return 0;
                else
                    return radius;
            }
        }
        // Use this for initialization
        void Start()
        {
            //point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point = GameObject.Instantiate<GameObject>(markerPrefab);
            point.transform.parent = transform;
            point.transform.localScale = Vector3.one;
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
            ) * actualRadius;

            transform.localRotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

}