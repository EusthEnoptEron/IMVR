using UnityEngine;
using UnityEngine.EventSystems;

namespace Gestures
{

    public class CanvasDragEventData : PointerEventData
    {
        public CanvasDragEventData(EventSystem system)
            : base(system)
        {
        }

        public GenericHand Hand;
        public Vector3 worldDelta;
        public Quaternion deltaRotation;
        public GameObject canvas;
    }

    public interface ICanvasDragHandler : IEventSystemHandler
    {
        void OnDragStart(CanvasDragEventData e);
        void OnDragEnd(CanvasDragEventData e);
        void OnDragMove(CanvasDragEventData e);
    }

    public interface ISelectableCanvas : IEventSystemHandler
    {
        void OnSelect(BaseEventData e);
        void OnUnselect(BaseEventData e);
    }

    public interface IMovableCanvas : IEventSystemHandler
    {
        void OnMove(Vector3 position, Vector2 size, Quaternion rotation);
    }
}
