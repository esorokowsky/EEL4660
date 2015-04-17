using UnityEngine;
using System.Collections;

public class Utilities 
{

	public class Marker
	{
		public int ID;
        public int FrameCount;
		public Vector2 ScreenLoc;
		public Vector3 WorldLoc;
	}

	//Assuming 0,0 is bottom left
	public static Vector3 IndexToWorld(int x, int y)
	{
		Vector3 retLoc = Vector3.zero;
		
		//Calculate the x
		retLoc.x = 9.6f*(x*0.125f - 0.5f + 0.0625f);
		
		//Calculate the y
		retLoc.z = 9.6f*(y*0.125f - 0.5f + 0.0625f);
		
		return retLoc;
	}

	
	public static Vector3 IndexToWorld(Vector2 loc)
	{
		return IndexToWorld((int)(loc.x+0.5), (int)(loc.y+0.5));
	}
	
	//Assuming 0,0 is bottom left
	public static Vector2 WorldToIndex(Vector3 world)
	{
		Vector2 retVector = Vector2.zero;
		
		//Calculate the x
		retVector.x = ((world.x/9.6f)+0.5f-0.0625f)/0.125f;
		
		//Calculate the y
		retVector.y = ((world.z/9.6f)+0.5f-0.0625f)/0.125f;
		
		return retVector;
	}
}
