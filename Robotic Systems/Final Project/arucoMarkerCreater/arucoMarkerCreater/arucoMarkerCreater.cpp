//Allows the user to create a set of markers
//Usage: arucoMarkerCreater.exe <start marker> <end marker> <name> <sizeInPixels>
//Ex: arucoMarkerCreater.exe 15 2000 marker 100

#include <opencv2/highgui/highgui.hpp>
#include <iostream>
#include <string>
#include "aruco.h"
#include "arucofidmarkers.h"
#include <direct.h>
using namespace cv;
using namespace std;
 
int main(int argc, char* argv[])
{
	int nStart = 0;
	int nEnd = 0;
	int nNumPixels = 0;
	std::string strFileName;

	if(argc < 5)
	{
		printf("Invalid number of arguments specified\n");
		system("PAUSE");
	}
	else
	{
		int nStart = atoi(argv[1]);
		int nEnd = atoi(argv[2]);

		std::string strPath = "Markers\\";
		std::string strFileName = argv[3];
		int nNumPixels = atoi(argv[4]);

		mkdir(strPath.c_str());
		
		char strNum[32];
		for(int i = nStart; i < nEnd; i++)
		{
			sprintf(strNum, "%d.jpg", i);
			std::string strFullPath = strPath + strFileName + strNum;

			Mat marker = aruco::FiducidalMarkers::createMarkerImage(i, nNumPixels);
			cv::imwrite(strFullPath, marker);
		}
	}

	return 0;
}

