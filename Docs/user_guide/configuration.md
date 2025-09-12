# Configuration
The CAVE dimensions this core is built for are three 1920x1080 displays for the walls (5760x1080 total) and one 1080x1080 display for the floor, if your CAVE setup is the same, dragging in the "CAVE_Core" prefab will work out of the box. Note: you will still need to ensure the project settings below are set correctly.

If you have a bespoke setup that does not match these dimensions, there may be some extra steps you need to take in altering the `ProjectionPlane.cs` and `ProjectionPlaneCamera.cs` variables.

## Multiple Displays
If you are using multiple displays (such as having 1 monitor for the walls and 1 for the floor), you will need to take some extra steps to everything working correctly.
1. Go to `Project Settings` → `Player` → `Resolution and Presentation`.
2. Set the Fullscreen Mode option to `Fullscreen Window`.
