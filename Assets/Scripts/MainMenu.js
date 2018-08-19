#pragma strict
import UnityEngine.UI;

var CameraObject : Animator;
var hoverSound : GameObject;
var backSound : GameObject;
var selectSound : GameObject;
var windowSound: GameObject;
var areYouSure : GameObject;

// Animation
function Position2(){
	CameraObject.SetFloat("Animate",1);
}

function Position1(){
	CameraObject.SetFloat("Animate",0);
}

// Sound
function PlayHover(){
	hoverSound.GetComponent.<AudioSource>().Play();
}

function PlaySelect(){
	selectSound.GetComponent.<AudioSource>().Play();
}

function PlayWindow(){
	windowSound.GetComponent.<AudioSource>().Play();
}

function PlayBack(){
	backSound.GetComponent.<AudioSource>().Play();
}

// Quit
function AreYouSure(){
	areYouSure.gameObject.active = true;
}

function No(){
	areYouSure.gameObject.active = false;
}

function Yes(){
	Application.Quit();
}
