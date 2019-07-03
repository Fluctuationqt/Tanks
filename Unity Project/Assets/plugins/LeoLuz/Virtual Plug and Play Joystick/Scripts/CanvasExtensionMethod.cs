using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LeoLuz.PlugAndPlayJoystick {
//    public class ButtonFixedUpdateRaycastHandler : MonoBehaviour {
//        Canvas canvas;
//        RectTransform CanvasRect;
//        // Use this for initialization
//        void Start() {
//            canvas = GetComponent<Canvas>();
//            CanvasRect = canvas.GetComponent<RectTransform>();
//        }

//        // Update is called once per frame
//        void Update() {
//            if (TouchAbstraction.TouchCont > 0)
//            {
//                for (int i = 0; i < TouchAbstraction.TouchCont; i++)
//                {
//                    //var touch = TouchAbstraction.GetTouch(i);
//                    //Physics2D.Raycast()
//                    //switch (touch.phase)
//                    //{
//                    //    case TouchPhase.Began:
//                    //        break;
//                    //    case TouchPhase.Moved:
//                    //        break;
//                    //    case TouchPhase.Stationary:
//                    //        break;
//                    //    case TouchPhase.Ended:
//                    //        break;
//                    //    default:
//                    //        break;
//                    //}

//                }
//            }
//        }
//    }

public static class CanvasEtensionMethod
{
    public static Vector3 ScreenToCanvasWorldSpacePosition(this Canvas canvas, Vector2 screenPosition)
    {
        var CanvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        var CanvasWorldSize = CanvasSize * canvas.transform.lossyScale.x;
        var PixelPositionZero = canvas.transform.position - ((Vector3)CanvasWorldSize * 0.5f);
        var coord = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
        return PixelPositionZero + (new Vector3(coord.x * CanvasWorldSize.x, coord.y * CanvasWorldSize.y));


    }
}
}

