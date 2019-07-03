----------------------------------
Simple Touch Controller (v1.0)
Author: Daniele Olivieri
Date: 08/03/2017
----------------------------------

--------------------------------------
HOW TO USE THE SIMPLE TOUCH CONTROLLER
--------------------------------------
Start with a new scene.
Create a new Canvas object.
Drag and drop two prefab "SimpleTouch" under it, or the "SimpleTouch Joystick" if you want to use the joysticks.
Move and place the prefabs like a standard UI element, renaming them "Left Joystick" and "Right Joystick".
Once you have place the touch controllers, delete the default camera and drag and drop the "Player" prefab.
Connect the left and right controller objects, created earlier, in the PlayerMoveController script, under the Player object.
Create a new Plane GameObject in the scene and reset all the axis to 0.

Hit Play and use the UI controller to move the player around.
You can always add the PlayerMoveController script to your existing character.
You can update the speed of the movement and the rotation of the player changing the value of the parameters listed in the script.

---------------
EXAMPLE 1 SCENE
---------------

Open the Example1.unity and press Play in the editor,
drag on the left side of screen (in the game view) to move,
do the same on the right side to look around.

The toggle at the top changes the aim behaviour:
- checked, the view will keep rotating in the direction of the drag,
- unchecked, the view will rotate proportionally at the lenght of the drag.


---------------
EXAMPLE 2 SCENE
---------------

This scene is the same as the first, but instead using the full screen as touch controller, we have the joystick images.
Left joystick to move, and the right to look around.
The same function for the Aim Rotation, checking the toggle at the top.