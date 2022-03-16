# PlaceApp-Clone-ARFoundation

This application was inspired by the AR E-commerce app of IKEA. It was developed in unity v.2020.3.29f1 LTS using the AR Foundation framework. The examples below are running on Android but iOS builds where tested and work as well.


# Architecture


<div align="center">
<img src="https://i.imgur.com/kmwxc5B.png" >
</div>

A simple but effective event based architecture was applied due to the simplicity of the application. However, making it easy to escalate in the future if needed. The application has three basic states. One for the main menu, other for the items menu (Selecting the item) and last the position of the object in the augmented reality. Other utilities are build and subscribe to this three main events. Such us the move, place rotate, SharedScreenShot and measurement tool. 

## Utilities

In this application the user is able to choose between real Scale 3D models and position it in the real world. When a item is selected, it can be moved, rotated, toggle its measures, positioned or deleted -This applies to already positioned models or new-.

-  **Item generator tool** : generates an scriptable object based on the item characteristics and saves it for the app to use it. Find it at Tools>Item Generator
<div align="center">
<img src="https://i.imgur.com/wfaVBS5.png" height="40%" width="40%">
</div>

- **Measurement Tool**: Tool that measures de dimensions of the object 


- **Draw bounds component**: Tool that draws bounds and center of the selected object
<div align="center">
<img src="https://i.imgur.com/AYSvnss.png" height="40%" width="40%">
</div>
<div align="center">
<img src="https://i.imgur.com/7KivqgD.png" height="40%" width="40%">
</div>

## Flow

<div align="center">
<img src="https://i.imgur.com/ghlxJPn.png" >
</div>

<br/>

<div align="center" style="display:flex; flex-direction:row; justify-content:space-between; width:100%;">
	<img src="Assets/Readme/MainMenuGif.gif" height="25%" width="25%">
	<img src="https://im5.ezgif.com/tmp/ezgif-5-51281d89f3.gif" height="25%" width="25%">
	<img src="https://im5.ezgif.com/tmp/ezgif-5-09fedab8e2.gif" height="25%" width="25%">
</div>

## Setting up the project enviroment / adding dependencies

The app was build using this specific versions on the dependencies. Different versions may vary its results.

- AR Foundation v4.1.7
- ARCore XR Plugin v4.1.7
- ARKit XR Plugin v4.1.9
- TextMeshPro 3.0.9



