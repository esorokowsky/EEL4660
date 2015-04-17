#include "SocketServer.h"

#pragma comment(lib, "Ws2_32.lib")


SocketServer::SocketServer(int port)
{
	m_nPort = port;
	char strPort[32];
	sprintf(strPort, "%d", m_nPort);

	struct addrinfo *result = NULL, *ptr = NULL, hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;

	WSADATA wsaData;

	if (WSAStartup(0x202, &wsaData) != 0)
	{
		printf("ERROR: Initialization failure.\n");
		return;
	}

	// Resolve the local address and port to be used by the server
	int iResult = getaddrinfo(NULL, strPort, &hints, &result);
	if (iResult != 0) 
	{
		printf("getaddrinfo failed: %d\n", iResult);
		WSACleanup();
		return;
	}

	// Create a SOCKET for the server to listen for client connections
	m_ListenSocket = INVALID_SOCKET;
	m_ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);

	if (m_ListenSocket == INVALID_SOCKET) 
	{
		printf("Error at socket(): %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		WSACleanup();
		return;
	}

	// Setup the TCP listening socket
	iResult = bind(m_ListenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) {
		printf("bind failed with error: %d\n", WSAGetLastError());
		freeaddrinfo(result);
		closesocket(m_ListenSocket);
		WSACleanup();
		return;
	}

	freeaddrinfo(result);

	//Listen for a connection
	if (listen(m_ListenSocket, SOMAXCONN) == SOCKET_ERROR)
	{
		printf("Listen failed with error: %ld\n", WSAGetLastError());
		closesocket(m_ListenSocket);
		WSACleanup();
		return;
	}

	//Accept a client socket
	m_ServerSocket = INVALID_SOCKET;
	m_ServerSocket = accept(m_ListenSocket, NULL, NULL);
	if (m_ServerSocket == INVALID_SOCKET) 
	{
		printf("accept failed: %d\n", WSAGetLastError());
		closesocket(m_ListenSocket);
		WSACleanup();
		return;
	}

	int timeout = 30;
	//setsockopt(m_ServerSocket, SOL_SOCKET, SO_RCVTIMEO, (char *)&timeout, sizeof(int));
}

bool SocketServer::SendData()
{
	if (m_ServerSocket == INVALID_SOCKET)
	{
		printf("Not connected to server\n");
		return false;
	}
	
	bool bSend = false;
	int result = recv(m_ServerSocket, (char*)&bSend, sizeof(bSend), 0);
	if (result > 0)
	{
		return bSend;
	}
	else if (result == 0)
	{
		printf("Connection closing...\n");
	}
	else
	{
		printf("recv failed: %d\n", WSAGetLastError());
		closesocket(m_ServerSocket);
		WSACleanup();
		return false;
	}
}

void SocketServer::Send(int i_nDataSize, char* i_Data)
{
	if (m_ServerSocket == INVALID_SOCKET)
	{
		printf("Not connected to server\n");
		return;
	}

	//Send the data
	int result = send(m_ServerSocket, i_Data, i_nDataSize, 0);
	if (result == SOCKET_ERROR) 
	{
		printf("send failed: %d\n", WSAGetLastError());
		closesocket(m_ServerSocket);
		WSACleanup();
		return;
	}
}

SocketServer::~SocketServer()
{

}