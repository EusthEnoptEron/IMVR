using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace GalleryVR.Dbg
{
    [AddComponentMenu("Debug/Position Echoer")]
    public class PositionEchoer : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale *= 0.01f;

            //Debug.Log(eventData.pressPosition);
            var camera = eventData.pressEventCamera ?? Camera.main;

            // Calculate press position -- for this, we use the distance and add the near clip plane because that's where the distance originates from
            var pressPosition = new Vector3(eventData.position.x, eventData.position.y, 
                                            eventData.pointerPressRaycast.distance + camera.nearClipPlane);

            //Debug.Log(System.String.Format("{0} -> {1}", pressPosition, camera.ScreenToWorldPoint(pressPosition)));
            sphere.transform.position = camera.ScreenToWorldPoint(pressPosition);

            Destroy(sphere, 0.5f);
        }
    }
}
