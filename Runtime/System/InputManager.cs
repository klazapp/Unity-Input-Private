using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace com.Klazapp.Input
{
    public class InputManager : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        [Tooltip("Toggle this to set script's singleton status. Status will be set on script's OnAwake function")]
        private ScriptBehavior scriptBehavior = ScriptBehavior.None;
        public static InputManager Instance { get; private set; }

        //Pinch Distance
        private static float initPinchDistance;
        private const float PINCH_THRESHOLD_AMOUNT = 0.5F;
        
        //Touch Drag
     	private static float2 initialTouchPosition;
        private static bool isTouchDrag;
        private const float touchDragThreshold = 30f; // Pixels, adjust this value based on your needs
        private static float2 lastTouchPressedPosition; // Store the last tap position
        private static bool touchPressedDetectedWithoutDrag = false;
        
        //Mouse Drag
        private static bool isMouseDrag = false;
        private float2 initialMousePosition;
        private const float mouseDragThreshold = 5.0f; // Threshold to consider the movement a drag

        private static bool touchPressedDown = false;
        private static bool touchReleased = false;
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

        private void Awake()
        {
            SetScriptBehaviour(scriptBehavior);
        }
        
        private void Update()
        {
            touchPressedDown = false;
            touchReleased = false;

            touchPressedDetectedWithoutDrag = false; // Ensure it's reset each frame

            foreach (var touch in Touch.activeTouches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchPressedDown = true;
                        initialTouchPosition = touch.screenPosition;
                        isTouchDrag = false;
                        break;
                    case TouchPhase.Moved:
                        if (math.distance(initialTouchPosition, touch.screenPosition) > touchDragThreshold)
                        {
                            isTouchDrag = true;
                        }
                        break;
                    case TouchPhase.Ended:
                        if (!isTouchDrag)
                        {
                            touchPressedDetectedWithoutDrag = true;
                            lastTouchPressedPosition = touch.screenPosition; // Set tap position
                        }
                        touchReleased = true;
			isTouchDrag = false;
                        break;
                }
            }
            
            
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    // Record the starting position of the mouse when the left button is first pressed
                    initialMousePosition = Mouse.current.position.ReadValue();
                    isMouseDrag = false;
                }
                else if (Mouse.current.leftButton.isPressed)
                {
                    // Check the distance the mouse has moved since the button was pressed
                    Vector2 currentMousePosition = Mouse.current.position.ReadValue();
                    if (!isMouseDrag && Vector2.Distance(initialMousePosition, currentMousePosition) > mouseDragThreshold)
                    {
                        isMouseDrag = true; // Start dragging
                    }
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    if (isMouseDrag)
                    {
                        // Handle end of drag here if needed
                    }
                    isMouseDrag = false;
                }
            }
        }
		#endregion

		#region Public Access
 	public static bool IsMouseButtonReleased()
    	{
        	return Mouse.current?.leftButton.wasReleasedThisFrame ?? false;
    	}

        public static bool IsMouseDrag()
        {
            return isMouseDrag;
        }
        
        public static bool IsTouchPressedDown()
        {
            return touchPressedDown;
        }

        public static bool IsTouchPressedUp()
        {
            return touchReleased;
        }
        
        public static float2 GetTouchPositionIfNoDrag()
        {
            return lastTouchPressedPosition;
        }

        public static bool IsTouchDrag()
        {
            return isTouchDrag;
        }
        
        public static float2 GetTouchPosition()
        {
            return lastTouchPressedPosition;
        }

	public static float2 GetInitialTouchPosition()
        {
            return initialTouchPosition;
        }

        // Method to get the tap detection state
        public static bool IsTouchDetected()
        {
            var temp = touchPressedDetectedWithoutDrag;
            touchPressedDetectedWithoutDrag = false; // Reset after checking to handle each tap as a new event
            return temp;
        }
        
        public static (bool leftMouseButton, bool rightMouseButton) GetMouseButtonPressed()
        {
            var mouse = Mouse.current;
            return (mouse.leftButton.isPressed, mouse.rightButton.isPressed); 
        }
        
        public static float2 GetMousePosition()
        {
            var mouse = Mouse.current;
            return mouse != null ? new float2(mouse.position.x.ReadValue(), mouse.position.y.ReadValue()) : float2.zero;
        }

        public static float GetMouseScrollAmount()
        {
            var mouse = Mouse.current;
            return mouse != null ? mouse.scroll.ReadValue().y : 0f;
        }

        public static float GetTouchPinchAmount(float pinchThreshold = PINCH_THRESHOLD_AMOUNT)
        {
            var activeTouches = Touch.activeTouches;

            if (activeTouches.Count < 2)
                return 0f;

            var touch1 = activeTouches[0];
            var touch2 = activeTouches[1];

            var pinchDistance = math.distance(touch1.screenPosition, touch2.screenPosition);

            if (touch2.phase == TouchPhase.Began)
            {
                initPinchDistance = pinchDistance;
            }

            var pinchInputAmount = pinchDistance - initPinchDistance;

            //Check if the pinch distance change exceeds the threshold
            if (math.abs(pinchInputAmount) <= pinchThreshold)
                return 0f; 
            
            initPinchDistance = pinchDistance;
            
            return pinchInputAmount;
        }
        
        public static bool GetTouchFirstDown()
        {
            if (!EnhancedTouchSupport.enabled)
                return false;
           
            return Touch.activeTouches.Count != 0 && Touch.activeTouches[0].inProgress;
        }
        #endregion
        
        #region Modules
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetScriptBehaviour(ScriptBehavior behavior)
        {
            if (behavior is not (ScriptBehavior.Singleton or ScriptBehavior.PersistentSingleton)) 
                return;
            
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
                
            Instance = this;

            if (behavior == ScriptBehavior.PersistentSingleton)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        #endregion
    }
}
