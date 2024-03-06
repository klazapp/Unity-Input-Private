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
        #endregion
        
        #region Public Access
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
