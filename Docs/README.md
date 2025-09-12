# Unity CAVE SDK
## Overview
This [Unity](https://unity.com/) package aims to provide easy to use functionality for customisable configurations within [CAVE (Cave automatic virtual environment)](https://en.wikipedia.org/wiki/Cave_automatic_virtual_environment) spaces, with appropriate camera projection, touch interaction capabilities and extension features such as supporting the Microsoft Kinect.

This project is specifically configured for the [Manchester Metropolitan University](https://www.mmu.ac.uk/) Brooks Building CAVE (BR0.89). That CAVE is a 5m by 5m floorspace with ~2.8125m high walls, running on three walls and the floor, and supports multiple concurrent touch input.


<br/>
<p align="center">
  <img width="460" height="300" src="https://upload.wikimedia.org/wikipedia/commons/6/6d/CAVE_Crayoland.jpg"><br/>
  Image of a CAVE space. Credit to Wiki user Davepape.
</p>

## The CAVE Challenge
Camera projection within a CAVE space has unique challenges due to **image warping effects** where content is stretched at wall edges. This is due to how virtual cameras typically render 3D scenes in computer graphics. The image below is a [View Frustum](https://en.wikipedia.org/wiki/Viewing_frustum) and in simplified terms, everything behind the yellow 'Near' plane (from the cameras perspective) is what gets displayed to screen. 

<br/>
<p align="center">
  <img width="460" height="300" src="https://upload.wikimedia.org/wikipedia/commons/0/02/ViewFrustum.svg"><br/>
  Image of a View Frustrum. Credit to Wiki user MithrandirMage.
</p>
<br/>


For this context, the above yellow near plane can be understood as a wall within a CAVE facility where digital content will be projected onto. Given that, some considerations arise such as:

1. How will we project to multiple walls, simply more cameras?
2. Where do we position near planes and decide what will and wont be rendered?
3. Where should the camera point, or 'eye' be positioned


### Our Solution
Our solution is to use [off-axis projection in Unity](https://github.com/aptas/off-axis-projection-unity) as a base. With this technique, a camera's near plane and camera point of origin (or eye) can be unaligned. 

To ensure that only content outside the CAVE is rendered, each real surface (e.g. wall) has a camera with a corresponding near render plane positioned to replicate the real-world CAVE space.

To best ensure the images displayed on each wall do so as expected, the origin for each camera is set to the centre of the CAVE but raised to average head hight. In other words, viewing a 3D scene from the centre of the CAVE should give a user the most accurate perspective.
