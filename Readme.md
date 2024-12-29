
# Cyla - Cylindrical atmospheres rendering

This is a quick KSP plugin to add Cylindrical atmospheres to KSP vessels, for use with Niven rings or O'Neill cylinders.

![enter image description here](https://i.imgur.com/hq9F9iX.png)
![enter image description here](https://i.imgur.com/JQRay1I.png)
![enter image description here](https://i.imgur.com/vjlTNKB.png)
![enter image description here](https://i.imgur.com/A4mYvzG.png)
There will be a new part in Utility named "Cylindrical atmosphere renderer", add that to your craft to get started.

Note that I don't plan to develop this mode further, unless someone makes a robust O'Neill cylinder or Niven ring mod with functional gravity.
# Settings
Opening the part GUI brings up the following settings menu, you can change settings in editors or in flight.
![enter image description here](https://i.imgur.com/CLavprx.png)
## Dimensions
Refer to this diagram

![enter image description here](https://i.imgur.com/Ky4BINi.png)

## Lighting mode
There's a few lighting modes you can select from, we can understand the lighting modes by looking at this diagram of a cross-section

![enter image description here](https://i.imgur.com/wFH7jPW.png)

 - TransparentTop: Only the top is transparent, other sides will cast shadows, use for Niven rings. Sunsets are very difficult to see in this mode unless inner and outer radiuses are very close to each other and there's a decent height, otherwise, there will not be enough "grazing" distance for light to go through and scatter.
 - TransparentSideWalls: Only the sides are transparent, use preferably with an inner radius of zero, to emulate O'Neill cylinders lit from the sides.
 - TransparentFloor
 - Unlit: Light always reaches all parts of the atmo, no sunsets or color shifts with distance, use when you can't see the atmo due to geometry/self-shadowing.

## Atmosphere settings
The atmosphere settings include classic rayleigh and mie settings, note these are normalized relative to (outerRadius-innerRadius) so rescaling the dimensions mostly keeps the same look. With the scale height settings, make sure the atmosphere falls off sufficiently before reaching the inner radius or you end up with a hard cutoff.

# Installation
Go to [releases](https://github.com/LGhassen/Cyla/releases) and grab the latest .zip. Unzip it, merge the provided GameData folder with your game's GameData folder (typically **C:\Program Files\Steam\SteamApps\common\Kerbal Space Program\GameData**).

You should see the following folder structure:

```
Kerbal Space program
└──────GameData
		└──────Cyla
```

Make sure you downloaded the release linked above and not the code, if you see Cyla-Master you messed up and downloaded the code.

# Reporting issues

To report an issue add screenshots of the issue, reproduction steps and your KSP.log file, otherwise your report may not be taken into account.
