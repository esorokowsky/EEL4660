	function Update() {
		// Slowly rotate the object around its X axis at 1 degree/second.
		//transform.Rotate(Time.deltaTime, 0, 0);
		// ... at the same time as spinning it relative to the global 
		// Y axis at the same speed.
		transform.Rotate(0, 0.4, 0, Space.World);
	}