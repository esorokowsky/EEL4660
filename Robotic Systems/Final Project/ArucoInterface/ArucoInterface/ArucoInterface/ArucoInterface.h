#include <Windows.h>
#include <stdio.h>
#include <thread>
#include <aruco.h>
#include <opencv2/core/core.hpp>
#include "opencv2/opencv.hpp"
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/calib3d/calib3d.hpp>

using namespace std;
using namespace cv;
using namespace aruco;

class ArucoInterface
{
public:
	//Data structure to store marker information
	struct Marker
	{
		int id;				//The markers unique ID
		Point2f screenLoc;	//Location of the marker on the camera
		Point3f worldLoc;	//Location of the marker in 3D space
	};

	struct SharedData
	{
		int nDeviceId;	//The device ID of the camera
		int nWidth;		//Width of the camera image
		int nHeight;		//Height of the camera image

		CameraParameters CamParams;
		float fMarkerSize;

		HANDLE hMutex;
		volatile bool bShutdown;
		volatile bool bUpdate;

		//Each marker that was detected this frame
		std::vector<ArucoInterface::Marker> arrMarkers;

	};

	ArucoInterface(int i_nDeviceId);
	~ArucoInterface();

	std::vector<ArucoInterface::Marker> GetMarkers();
	void GetMarker(int i_nIndex, float &o_fScreenX, float &o_fScreenY, float &o_fWorldX, float &o_fWorldY, float &o_fWorldZ);
	bool NewData();

private:

	SharedData* m_pSharedData;
	HANDLE m_hThreadMutex;

	std::thread m_Thread;

};