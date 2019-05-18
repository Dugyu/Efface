A Shared Landscape Responsive to Existence


1. Download an install the Tobii Eye Tracker driver (Tobii Eye Tracking Core Software v2.16.4

) at https://gaming.tobii.com/getstarted/

after installation finished, calibrate the eye tracker to your eyes.


2. Test out. 

There are two ways to test out the project:

a. Using Build Standalone Excutable File (Windows Only):
download v1.0-alpha release at https://github.com/Dugyu/Efface/releases/tag/v1.0-alpha

a.1 Using eye gaze and head pose as input
unzip Efface_Win_Eye 
Run Efface.exe.
move head as if you are diving into the scene

a.2 Alternatively, you can use mouse as input
unzip Efface_Win_Mouse
Run Efface.exe.
WASDRF for forward/left/back/right/up/down




b. Using Unity Editor (Windows Only).
Download Efface master folder.

b.1 Go to Unity download site, choose the *exact* version 2018.3.8f1. 
https://unity3d.com/get-unity/download/archive
In the selection tab at the bottom of the page, select 2018.x, then scroll down, select 2018.3.8, choose "Downloads(Win)".

b.2 Install the downloaded file. 

b.3 Open the EffaceTest.unity file in Efface\Assets\Scenes.

b.4 Click the "Scripting" object in the hierarchy, enable "Mouse Ray" and "Presence of Me" scripts in the inspector to interact using mouse input, or disable them, enable "Eye Ray" and "Presence of Me Eye"  scripts in the inspector to interact using eye gaze input.

b.5 Click run button in the editor.