using System.Linq;
using System.Runtime.CompilerServices;
using com.Klazapp.Utility;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace com.Klazapp.Input
{
    [ScriptHeader("Manages touch support and provides methods for enabling and disabling touch input.")]
    public class InputManager : MonoSingletonGlobal<InputManager>
    {
        #region Variables
        //Touch Inputs
        public static bool isActiveTouch1Down;
        public static float2 activeTouch1Position;
        public static float2 activeTouch2Position;
        
        //Mouse Inputs
        private static Vector2Control mousePosition;
        private static DeltaControl mouseScrollAxis;
        public static bool mouseLeftButtonPressed;
            
        //Pinch Distance
        private static float initPinchDistance;
        private const float PINCH_THRESHOLD_AMOUNT = 0.5f;
        #endregion

        #region Lifecycle Flow
        public void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        public void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }
        
        public static void Update()
        {
#if UNITY_EDITOR
            mousePosition = Mouse.current.position;
            mouseScrollAxis = Mouse.current.scroll;
            mouseLeftButtonPressed = Mouse.current.leftButton.isPressed;
#else
            if (Touch.activeTouches.Count == 0) 
                return;

            isActiveTouch1Down = Touch.activeTouches[0].inProgress;
            activeTouch1Position = Touch.activeTouches[0].screenPosition;
            
            if (Touch.activeTouches.Count == 2)
            {
                activeTouch2Position = Touch.activeTouches[1].screenPosition;
            }
#endif
        }
        #endregion
        
        #region Public Access
        public static float2 GetMousePosition()
        {
            if (mousePosition != null)
            {
                var mouseXY = new float2(mousePosition.x.value, mousePosition.y.value);
                return mouseXY;
            }
            else
            {
                return 0;
            }
        }
        
        public static float GetPinchAmount(float pinchThreshold = PINCH_THRESHOLD_AMOUNT)
        {
            var activeTouches = GetActiveTouches();

            if (activeTouches.Count < 2)
                return 0f;

            var touch1 = activeTouches[0];
            var touch2 = activeTouches[1];

            var pinchDistance = math.distance(touch1.screenPosition , touch2.screenPosition);

            if (touch2.phase == TouchPhase.Began)
            {
                initPinchDistance = pinchDistance;
            }

            var pinchInputAmount = pinchDistance - initPinchDistance;

            //Check if the pinch distance change exceeds the threshold
            if (!(math.abs(pinchInputAmount) > pinchThreshold))
                return 0f; 
            
            initPinchDistance = pinchDistance;
            
            return pinchInputAmount;
        }


        public static float GetMouseScrollAmount()
        {
#if UNITY_EDITOR
            var scrollAmount = mouseScrollAxis?.y?.value;

            if (!scrollAmount.HasValue) return 0f;

            if (mathExtension.approximately(scrollAmount.Value, 0f)) 
                return 0f;

            var scrollInputAmount = scrollAmount.Value;
            return scrollInputAmount;
#endif
            return 0;
        }
        #endregion
        
        #region Modules
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlyArray<Touch> GetActiveTouches()
        {
            return Touch.activeTouches;
        }
        #endregion
    }
}
