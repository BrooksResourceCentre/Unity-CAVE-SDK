# Touch Input
The Unity CAVE SDK implements the Unity [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/index.html) for its UI input events and uses the [Enhanced Input Support](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.html) for any world-space objects or general CAVE interactions.
As a result, the CAVE walls are designed to function as a touchscreen with the SDK system out of the box for both UI and any objects you design to be interactable.

In the future a library of prefabs will be available, providing an easy drag-and-drop solution for many common interations you may want in your experience, such as buttons and toggles.

# Implementing your own interactable objects
To make an object in your scene respond to touch input you must have a script component attached that inherits from 'InteractionObject'.
To do this, add ```using MMUCAVE``` to the top of your class file, replace ```MonoBehaviour``` with ```InteractionObject``` after your class name and override each of the ```OnTap(), OnHold(), OnSwipe()``` methods inside the class.

For example:
``` C#
      using MMUCAVE;
      using UnityEngine;
      public class ExampleClass : InteractionObject
      {
          public override void OnTouch()
          {
            //My touch code here
          }
          public override void OnHold()
          {
            //My hold code here
          }
          public override void OnSwipe()
          {
            //My swipe code here
          }
```

If there are any of these inputs that you don't want to make use of on your object then simply ```return;```
