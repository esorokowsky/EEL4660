#include "SocketServer.h"
#include "ArucoInterface.h"

#define FLOAT_SIZE sizeof(float)

int main(int argc, char* argv[])
{
	ArucoInterface* arInterface = new ArucoInterface(0);
	SocketServer* comSocket = new SocketServer(5003);

	bool bClose = false;
	while (bClose == false)
	{
		if (arInterface->NewData())
		{
			std::vector<ArucoInterface::Marker> markers = arInterface->GetMarkers();

			int nNumMarkers = markers.size();
			int nDataSize = sizeof(int) + (FLOAT_SIZE * 3)*markers.size();

			char* dataBuffer = (char*)malloc(nDataSize);
			int offset = 0;

			memcpy(dataBuffer, (char*)&nNumMarkers, sizeof(int));
			offset += sizeof(int);

			for (int i = 0; i < nNumMarkers; i++)
			{
				ArucoInterface::Marker curMarker = markers[i];

				memcpy(dataBuffer+offset, (char*)&(curMarker.id), sizeof(int));
				offset += sizeof(int);

				memcpy(dataBuffer + offset, (char*)&(curMarker.screenLoc.x), sizeof(float));
				offset += sizeof(float);
				memcpy(dataBuffer + offset, (char*)&(curMarker.screenLoc.y), sizeof(float));
				offset += sizeof(float);

				printf("Marker: %d %03.2f, %03.2f\n", curMarker.id, curMarker.screenLoc.x, curMarker.screenLoc.y);
			}

			comSocket->Send(nDataSize, dataBuffer);
		}
		else
		{
			Sleep(200);
		}


		char key = waitKey(1);
		bClose = (key == 'ESC') || (key == 'q') || (key == 'Q');
	}
}