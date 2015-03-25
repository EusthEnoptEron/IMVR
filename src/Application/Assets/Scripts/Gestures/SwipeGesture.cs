using UnityEngine;
using System.Collections;
using Gestures;
using Leap;

public class SwipeGesture : MonoBehaviour {

    private Leap.Controller controller;
    private float waitTime = 0;
    private float targetAlpha = 1;

    public HandProvider handController;
    // Use this for initialization
    protected void Start()
    {
        // Make controller
        controller = new Leap.Controller();
        controller.EnableGesture(Leap.Gesture.GestureType.TYPE_SWIPE);
    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<CanvasGroup>().alpha = Mathf.Lerp(GetComponent<CanvasGroup>().alpha, targetAlpha, Time.deltaTime);

        waitTime = Mathf.Max(0, waitTime - Time.deltaTime);

        if (waitTime <= 0)
        {
            var frame = controller.Frame();

            if (frame.Hands.Count > 0) targetAlpha = 1;
            else targetAlpha = 0.3f;
            foreach (var gesture in frame.Gestures())
            {
                if (gesture.Type == Leap.Gesture.GestureType.TYPE_SWIPE)
                {

                    GetComponent<Rigidbody>().AddRelativeTorque(
                         handController.transform.TransformDirection(gesture.Hands[0].PalmVelocity.ToUnity() * Time.deltaTime * 10)
                    );


                    return;

                    var hand = gesture.Hands[0].IsLeft ? handController.LeftHand : handController.RightHand;
                    if (hand != null)
                    {

                        bool abort = false;
                        foreach (var finger in hand.Fingers)
                        {
                            if (finger.Type == FingerType.Index || finger.Type == FingerType.Middle)
                            {
                                if (!finger.Extended) { abort = true; break; }
                            }

                            if (finger.Type == FingerType.Ring || finger.Type == FingerType.Pinky)
                            {
                                if (finger.Extended) { abort = true; break; }
                            }
                        }
                        if (abort) continue;

                        var palmNormal = hand.PalmNormal;

                        if (Vector3.Dot(palmNormal, Camera.main.transform.right) < -0.5)
                        {
                            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 100, 0));

                            waitTime = 1;
                            return;
                        }
                        //var palmDirection = hand.PalmDirection;
                        //var palmNormal = hand.PalmNormal;

                        //if (Vector3.Dot(palmNormal, Camera.main.transform.up) > 0)
                        //{
                        //    var dotProduct = Vector3.Dot(palmDirection, Camera.main.transform.forward);

                        //    if (dotProduct > 0.7)
                        //    {
                        //        // 
                        //        SetMenuActive(true);
                        //        waitTime = 1;
                        //        return;
                        //    }
                        //    else if (Mathf.Abs(dotProduct) < 0.2)
                        //    {
                        //        SetMenuActive(false);
                        //        waitTime = 1;
                        //        return;
                        //    }
                        //}
                    }

                }
            }
        }
    }
}
