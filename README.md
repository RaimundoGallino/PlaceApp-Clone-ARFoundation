# PlaceApp-Clone-ARFoundation

This application was inspired by the AR e-comerce app of IKEA. It was developed in unity v.2020.3.29f1 LTS using the AR Foundation framework. 

<div align="center" style="display:flex; flex-direction:row; justify-content:space-between;">
	<img src="https://i.imgur.com/eikVJnT.png" height="25%" width="25%">
	<img src="https://i.imgur.com/eikVJnT.png" height="25%" width="25%">
	<img src="https://i.imgur.com/eikVJnT.png" height="25%" width="25%">
</div>


# Architecture

<div align="center">
<img src="https://i.imgur.com/sgw9Yr1.png" >
</div>
<div align="center">
<img src="https://i.imgur.com/kmwxc5B.png" >
</div>

A simple but effective event based architecture was applied due to the simplicity of the application. However, making it easy to escalate in the future if needed. The application has three basic states. One for the main menu, other for the items menu (Selecting the item) and last the position of the object in the augmented reality. Other utilities are build and subscribe to this three main events. Such us the move, place rotate, SharedScreenShot and measurement tool. 

## Utilities

In this application the user is able to choose between real Scale 3D models and position it in the real world. When a item is selected, it can be moved, rotated, toggle its measures, positioned or deleted -This applies to already positioned models or new-.

## Flow

