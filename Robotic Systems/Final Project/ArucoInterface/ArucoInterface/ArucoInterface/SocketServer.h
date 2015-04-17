#include "stdio.h"
#include <winsock2.h>
#include <Windows.h>
#include <ws2tcpip.h>
#include <stdio.h>

class SocketServer
{
public:
	SocketServer(int port);
	bool SendData();
	void Send(int i_nDataSize, char* i_Data);

	~SocketServer();

	int m_nPort;
	SOCKET m_ListenSocket;
	SOCKET m_ServerSocket;

private:
};