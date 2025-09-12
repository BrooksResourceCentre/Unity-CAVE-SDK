# Installation

## Prerequisites
This package supports the following Unity major versions, with the tested sub versions in brackets:
- Unity 6 (`6000.3.0a5`, `6000.0.56f1 LTS`, `6000.2.2f1`)
- Unity 2023 (Untested)
- Unity 2022 (`2022.3.66f1`, `2022.3.46f1`, `2022.3.62f1 LTS`)

It supports all major desktop platforms where the Unity Editor is available: Windows, macOS, and Linux.

## How to use
1. Download this repository, extract the contents, and within Unity's Package Manager add "Install Package from Disk"
2. Include the "CAVE_Core" prefab in your scene or create a variation for your specific number of walls desired.

For interaction thresholds and similar tweaking, see the `InputActionHandler.cs` component on the "CAVE_Core" prefab.
Camera projection is handled through the default Unity camera and supports both the Universal and High Definition Render Pipelines (UPR and HDRP), in addition to any other camera screen effects.
