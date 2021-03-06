#include "ArucoInterface.h"

std::vector<ArucoInterface::Corner> m_arrCorners;

void MarkerDetectionThread(ArucoInterface::SharedData* io_SharedData);

ArucoInterface::ArucoInterface(int i_nDeviceId)
{
	CameraParameters camParams;
	camParams.readFromXMLFile("cameraCalibration.xml");
	m_hThreadMutex = CreateMutex(NULL, FALSE, NULL);

	m_pSharedData = new SharedData();
	m_pSharedData->bShutdown = false;
	m_pSharedData->hMutex = m_hThreadMutex;
	m_pSharedData->nDeviceId = i_nDeviceId;
	//m_pSharedData->nWidth = 960;
	//m_pSharedData->nHeight = 540;
	m_pSharedData->nWidth = 640;
	m_pSharedData->nHeight = 480;
	m_pSharedData->fMarkerSize = 0.055f;
	m_pSharedData->CamParams = camParams;

	m_arrCorners.push_back(Corner());
	m_arrCorners.push_back(Corner());
	m_arrCorners.push_back(Corner());
	m_arrCorners.push_back(Corner());

	m_Thread = std::thread(MarkerDetectionThread, m_pSharedData);
}

std::vector<ArucoInterface::Marker> ArucoInterface::GetMarkers()
{
	std::vector<ArucoInterface::Marker> retMarkers;
	DWORD waitResult = WaitForSingleObject(m_hThreadMutex, INFINITE);
	if (waitResult == WAIT_OBJECT_0)
	{
		retMarkers = m_pSharedData->arrMarkers;
		ReleaseMutex(m_hThreadMutex);
	}
	return retMarkers;
}

void sortCorners(std::vector<cv::Point2f>& corners, cv::Point2f center)
{
	std::vector<cv::Point2f> top, bot;

	for (int i = 0; i < corners.size(); i++)
	{
		if (corners[i].y < center.y)
			top.push_back(corners[i]);
		else
			bot.push_back(corners[i]);
	}

	cv::Point2f tl = top[0].x > top[1].x ? top[1] : top[0];
	cv::Point2f tr = top[0].x > top[1].x ? top[0] : top[1];
	cv::Point2f bl = bot[0].x > bot[1].x ? bot[1] : bot[0];
	cv::Point2f br = bot[0].x > bot[1].x ? bot[0] : bot[1];

	corners.clear();
	corners.push_back(tl);
	corners.push_back(tr);
	corners.push_back(br);
	corners.push_back(bl);
}

float Distance(Point2f p1, Point2f p2)
{
	return sqrt(pow(p2.x - p1.x, 2) + pow(p2.y - p1.y, 2));
}

void MarkerDetectionThread(ArucoInterface::SharedData* io_SharedData)
{
	HANDLE hMutex = io_SharedData->hMutex;
	CameraParameters camParams = io_SharedData->CamParams;
	int nDevice = io_SharedData->nDeviceId;
	int nWidth = io_SharedData->nWidth;
	int nHeight = io_SharedData->nHeight;
	float fMarkerSize = io_SharedData->fMarkerSize;

	Mat curFrame;
	MarkerDetector MDetector;
	std::vector<Marker> arrMarkers;

	MDetector.setMinMaxSize(0.001f);
	MDetector.setDesiredSpeed(0);

	VideoCapture capture(nDevice);
	if (capture.isOpened() == false)
	{
		MessageBox(NULL, "Failed to open capture device.", "Error", MB_OK);
		return;
	}

	camParams.resize(Size(nWidth, nHeight));
	
	capture.read(curFrame);
	while (curFrame.dims == 0)
	{
		Sleep(100);
		capture.read(curFrame);
	}

	bool w = capture.set(CV_CAP_PROP_FRAME_WIDTH, nWidth);
	bool h = capture.set(CV_CAP_PROP_FRAME_HEIGHT, nHeight);
	capture.set(CV_CAP_PROP_FPS, 15);

	int frameCount = 0;
	long Start = GetTickCount();
	while (io_SharedData->bShutdown == false)
	{
		capture.read(curFrame);

		Mat colorImage;
		curFrame.copyTo(colorImage);

		cv::cvtColor(curFrame, curFrame, cv::COLOR_BGR2GRAY);
		cv::inRange(curFrame, Scalar(125, 125, 125), Scalar(255, 255, 255), curFrame);
		
		Mat chessBoard;
		curFrame.copyTo(chessBoard);
		
		//Detect the first set of markers
		MDetector.detect(curFrame, arrMarkers);
		for (int i = 0; i < arrMarkers.size(); i++)
		{
			Marker tempMarker = arrMarkers[i];
			tempMarker.draw(colorImage, Scalar(0, 0, 255));
		}

		for (int i = 0; i < m_arrCorners.size(); i++)
		{
			m_arrCorners[i].nFrameCount++;
		}

		//Detect chess board location
		std::vector<cv::Point2f> corners;
		for (int i = 0; i < arrMarkers.size(); i++)
		{
			Marker curMarker = arrMarkers[i];
			if (curMarker.id == 1)
			{
				m_arrCorners[0].nFrameCount = 0;
				m_arrCorners[0].ScreenLoc = curMarker[1];
			}
			else if (curMarker.id == 2)
			{
				m_arrCorners[1].nFrameCount = 0;
				m_arrCorners[1].ScreenLoc = curMarker[0];
			}
			else if (curMarker.id == 3)
			{
				m_arrCorners[2].nFrameCount = 0;
				m_arrCorners[2].ScreenLoc = curMarker[3];
			}
			else if (curMarker.id == 4)
			{
				m_arrCorners[3].nFrameCount = 0;
				m_arrCorners[3].ScreenLoc = curMarker[2];
			}
		}

		for (int i = 0; i < m_arrCorners.size(); i++)
		{
			if (m_arrCorners[i].nFrameCount < 3)
				corners.push_back(m_arrCorners[i].ScreenLoc);
		}


		//If we only found 3 corners lets try to make the other one
		//experimental only
		/*if (corners.size() == 3)
		{
			if (b1 && b2 && b3)
			{
				Point2f newP;
				Point2f avg = arrMarkers[0][1] + arrMarkers[2][3]; //marker 1 and 3
				Point2f vec;

				avg.x = avg.x / 2.0f;
				avg.y = avg.y / 2.0f;

				circle(colorImage, avg, 15, Scalar(0, 0, 255));
				imshow("RawMarkers", colorImage);
				waitKey(1);


				vec = avg - arrMarkers[1][0]; //marker 2
				float dist = sqrt(pow(vec.x,2) + pow(vec.y,2));

				vec.x = vec.x / dist;
				vec.y = vec.y / dist;

				newP.x = vec.x * dist * 2.0f;
				newP.y = vec.y * dist * 2.0f;

				newP = newP + arrMarkers[1][0];

				circle(colorImage, newP, 15, Scalar(0, 255, 0));
				imshow("RawMarkers", colorImage);
				waitKey(1);
				bool hit = true;
			}
			else if (b2 && b3 && b4)
			{

			}
			else if (b3 && b4 && b1)
			{

			}
			else if (b4 && b1 && b2)
			{

			}
		}*/

		if (corners.size() == 4)
		{
			std::vector<cv::Point2f> approx;
			cv::approxPolyDP(cv::Mat(corners), approx,
				cv::arcLength(cv::Mat(corners), true) * 0.02, true);

			if (approx.size() == 4)
			{
				// Get mass center
				cv::Point2f center(0, 0);
				for (int i = 0; i < corners.size(); i++)
					center += corners[i];

				center *= (1. / corners.size());
				sortCorners(corners, center);

				// Define the destination image
				cv::Mat quad = cv::Mat::zeros(400, 400, CV_8UC3);

				// Corners of the destination image
				std::vector<cv::Point2f> quad_pts;
				quad_pts.push_back(cv::Point2f(0, 0));
				quad_pts.push_back(cv::Point2f(quad.cols, 0));
				quad_pts.push_back(cv::Point2f(quad.cols, quad.rows));
				quad_pts.push_back(cv::Point2f(0, quad.rows));

				// Get transformation matrix
				cv::Mat transmtx = cv::getPerspectiveTransform(corners, quad_pts);

				// Apply perspective transformation
				cv::warpPerspective(chessBoard, chessBoard, transmtx, quad.size());
			}

			std::vector<Marker> arrMarkers2;
			MDetector.detect(chessBoard, arrMarkers2);

			DWORD waitResult = WaitForSingleObject(hMutex, INFINITE);
			if (waitResult == WAIT_OBJECT_0)
			{
				io_SharedData->arrMarkers.clear();

				for (int i = 0; i < arrMarkers2.size(); i++)
				{
					Marker tempMarker = arrMarkers2[i];
					ArucoInterface::Marker curMarker;
					curMarker.id = tempMarker.id;
					curMarker.screenLoc = tempMarker.getCenter();

					tempMarker.draw(chessBoard, Scalar(0, 0, 255));

					io_SharedData->arrMarkers.push_back(curMarker);
					io_SharedData->bUpdate = true;

				}

				ReleaseMutex(hMutex);
			}
			imshow("ChessBoard", chessBoard);
			moveWindow("ChessBoard", 1260 + nWidth, nHeight);
		}


		imshow("RawMarkers", colorImage);
		moveWindow("RawMarkers", 1260, 0);

		imshow("b&w", curFrame);
		moveWindow("b&w", 1260 + nWidth, 0);
		waitKey(1);

		frameCount++;
		int difTime = GetTickCount() - Start;
		if (difTime >= 2000)
		{
			float fps = (((float)frameCount) / ((float)difTime)*1000.0f);
			printf("Fps: %f\n", fps);

			//capture.set(CV_CAP_PROP_FPS, (int)fps);

			Start = GetTickCount();
			frameCount = 0;
		}
	}
}

bool ArucoInterface::NewData()
{
	bool update = false;
	DWORD waitResult = WaitForSingleObject(m_hThreadMutex, 20);
	if (waitResult == WAIT_OBJECT_0)
	{
		update = m_pSharedData->bUpdate;
		m_pSharedData->bUpdate = false;
		ReleaseMutex(m_hThreadMutex);
	}

	return update;
}

ArucoInterface::~ArucoInterface()
{
	m_pSharedData->bShutdown = true;
	m_Thread.join();

	if (m_pSharedData)
		delete m_pSharedData;
}