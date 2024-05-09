# Input Manager for Unity

## Introduction

The Input Manager package, created under the `com.Klazapp.Input` namespace, offers a robust and versatile system for handling inputs in Unity, integrating tightly with the Unity Input System and Enhanced Touch Support. This utility simplifies the process of detecting various input types such as mouse clicks, mouse positions, mouse scroll, and multi-touch gestures across different platforms.

## Features

- **Enhanced Touch Support:** Utilizes the Unity Input System's Enhanced Touch capabilities to provide detailed multi-touch handling and gestures like pinch.
- **Mouse Input Handling:** Retrieves the state of mouse buttons, mouse position, and scroll wheel movements.
- **Singleton Management:** Can be configured to behave as a singleton or a persistent singleton, ensuring a single, global point of access throughout the application.
- **Pinch Gesture Detection:** Measures pinch gestures with customizable thresholds for sensitive applications like map zooming or object scaling.

## Dependencies

- **Unity Input System:** This package requires the Unity Input System to handle complex input configurations and device compatibility.
- **Unity 2020.1 or Newer:** Required to take full advantage of the latest Input System enhancements and script capabilities.

## Compatibility

This package is designed to work with all Unity projects that utilize the Unity Input System, making it suitable for any rendering pipeline or platform.

| Compatibility | URP | BRP | HDRP |
|---------------|-----|-----|------|
| Compatible    | ✔️   | ✔️   | ✔️    |

## Installation

1. Download the Input Manager package from the [GitHub repository](https://github.com/klazapp/Unity-Input-Manager-Public.git) or via the Unity Package Manager.
2. Include the scripts in your Unity project, ideally within an Input or Systems directory.

## Usage

To utilize the Input Manager, simply access its static functions or properties through the `InputManager.Instance` when set as a singleton. Here's an example of detecting a pinch gesture:

```csharp
float pinchAmount = InputManager.GetTouchPinchAmount();
if (Math.Abs(pinchAmount) > 0)
{
    Debug.Log("Pinch detected with amount: " + pinchAmount);
}
```

## To-Do List (Future Features)

- [ ] Expand input handling to support more complex gestures and combinations of inputs.
- [ ] Integrate with virtual and augmented reality hardware for specialized input handling.
- [ ] Implement customizable input profiles that can be switched dynamically during runtime.

## License

This utility is released under the MIT License, allowing for free use, modification, and distribution within your projects.
