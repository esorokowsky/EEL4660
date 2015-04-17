#include "ArucoInterface.h"

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
	m_pSharedData->nWidth = 960;
	m_pSharedData->nHeight = 540;
	m_pSharedData->fMarkerSize = 0.055f;
	m_pSharedData->CamParams = camParams;

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

	//MDetector.setMinMaxSize(0.0005f);
	MDetector.setDesiredSpeed(0);

	VideoCapture capture(nDevice);
	if (capture.isOpened() == false)
	{
		MessageBox(NULL, "Failed to open capture device.", "Error", MB_OK);
		return;
	}

	capture.set(CV_CAP_PROP_FRAME_WIDTH, nWidth);
	capture.set(CV_CAP_PROP_FRAME_HEIGHT, nHeight);
	capture.set(CV_CAP_PROP_FPS, 5);

	camParams.resize(Size(nWidth, nHeight));
	
	capture.read(curFrame);
	while (curFrame.dims == 0)
	{
		Sleep(100);
		capture.read(curFrame);
	}

	int frameCount = 0;
	long Start = GetTickCount();
	while (io_SharedData->bShutdown == false)
	{
		capture.read(curFrame);

		Mat colorImage;
		curFrame.copyTo(colorImage);

		cv::cvtColor(curFrame, curFrame, cv::COLOR_BGR2GRAY);
		cv::inRange(curFrame, Scalar(175, 175, 175), Scalar(255, 255, 255), curFrame);
		
		Mat chessBoard;
		curFrame.copyTo(chessBoard);
		
		//Detect the first set of markers
		MDetector.detect(curFrame, arrMarkers);
		for (int i = 0; i < arrMarkers.size(); i++)
		{
			Marker tempMarker = arrMarkers[i];
			tempMarker.draw(colorImage, Scalar(0, 0, 255));
		}

		//Detect chess board location
		std::vector<cv::Point2f> corners;
		for (int i = 0; i < arrMarkers.size(); i++)
		{
			Marker curMarker = arrMarkers[i];
			if (curMarker.id == 1)
				corners.push_back(curMarker[1]);
			else if (curMarker.id == 2)
				corners.push_back(curMarker[0]);
			else if (curMarker.id == 3)
				corners.push_back(curMarker[3]);
			else if(curMarker.id == 4)
				corners.push_back(curMarker[2]);
		}

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
		}


		imshow("RawMarkers", colorImage);
		imshow("b&w", curFrame);
		waitKey(1);

		frameCount++;
		int difTime = GetTickCount() - Start;
		if (difTime >= 2000)
		{
			float fps = (((float)frameCount) / ((float)difTime)*1000.0f);
			printf("Fps: %f\n", fps);

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