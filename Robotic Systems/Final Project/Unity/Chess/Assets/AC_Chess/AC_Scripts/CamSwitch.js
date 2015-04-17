
var camera1 : GameObject;
var camera2 : GameObject;
var camera3 : GameObject;
var camera4 : GameObject;
var camera5 : GameObject;
var camera6 : GameObject;


function OnGUI() {
	

  
    

    
    if (GUI.Button(Rect(20, 50, 100, 50), "Wood Lo")) {
        camera1.GetComponent.<Camera>().enabled = true;
        camera2.GetComponent.<Camera>().enabled = false;
        camera3.GetComponent.<Camera>().enabled = false;
        camera4.GetComponent.<Camera>().enabled = false;
        camera5.GetComponent.<Camera>().enabled = false;
        camera6.GetComponent.<Camera>().enabled = false;

    }
    if (GUI.Button(Rect(20, 100, 100, 50), "Wood Mid")) {
        camera1.GetComponent.<Camera>().enabled = false;
        camera2.GetComponent.<Camera>().enabled = true;
        camera3.GetComponent.<Camera>().enabled = false;
        camera4.GetComponent.<Camera>().enabled = false;
        camera5.GetComponent.<Camera>().enabled = false;
        camera6.GetComponent.<Camera>().enabled = false;
    }
    
    if (GUI.Button(Rect(20, 150, 100, 50), "Wood Hi")) {
        camera1.GetComponent.<Camera>().enabled = false;
        camera2.GetComponent.<Camera>().enabled = false;
        camera3.GetComponent.<Camera>().enabled = true;
        camera4.GetComponent.<Camera>().enabled = false;
        camera5.GetComponent.<Camera>().enabled = false;
        camera6.GetComponent.<Camera>().enabled = false;
    }

	if (GUI.Button(Rect(20, 200, 100, 50), "Marble Lo")) {
        camera1.GetComponent.<Camera>().enabled = false;
        camera2.GetComponent.<Camera>().enabled = false;
        camera3.GetComponent.<Camera>().enabled = false;
        camera4.GetComponent.<Camera>().enabled = true;
        camera5.GetComponent.<Camera>().enabled = false;
        camera6.GetComponent.<Camera>().enabled = false;

    }
    if (GUI.Button(Rect(20, 250, 100, 50), "Marble Mid")) {
        camera1.GetComponent.<Camera>().enabled = false;
        camera2.GetComponent.<Camera>().enabled = false;
        camera3.GetComponent.<Camera>().enabled = false;
        camera4.GetComponent.<Camera>().enabled = false;
        camera5.GetComponent.<Camera>().enabled = true;
        camera6.GetComponent.<Camera>().enabled = false;
    }
    
    if (GUI.Button(Rect(20, 300, 100, 50), "Marble Hi")) {
        camera1.GetComponent.<Camera>().enabled = false;
        camera2.GetComponent.<Camera>().enabled = false;
        camera3.GetComponent.<Camera>().enabled = false;
        camera4.GetComponent.<Camera>().enabled = false;
        camera5.GetComponent.<Camera>().enabled = false;
        camera6.GetComponent.<Camera>().enabled = true;
    }




}