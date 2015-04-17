using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class ClientSocket
{
    private Socket _ClientSocket; 
    private byte[] _RecieveBuffer = new byte[8142];

    private List<Utilities.Marker> _Markers;

	public int Port = 5003;

	// Use this for initialization
	public void Init (int i_nPort = -1) 
	{
		if (i_nPort != -1)
			Port = i_nPort;

		try
		{
			_ClientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            _ClientSocket.Connect(new IPEndPoint(IPAddress.Loopback, Port));
            _ClientSocket.BeginReceive(_RecieveBuffer, 0, _RecieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
 
		}
		catch(SocketException ex)
		{
			Debug.Log(ex.Message);
		}
	}
    
    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are recieved and call EndRecieve to finalize handshake
        int recieved = _ClientSocket.EndReceive(AR);

        if (recieved <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_RecieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData
        _Markers = DecodeMarkers(recieved, _RecieveBuffer);

        //Start receiving again
        _ClientSocket.BeginReceive(_RecieveBuffer, 0, _RecieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    public List<Utilities.Marker> GetMarkers()
	{
        List<Utilities.Marker> tempBuffer = new List<Utilities.Marker>();

        if (_Markers != null)
        {
            foreach(Utilities.Marker curMarker in _Markers)
            {
                tempBuffer.Add(curMarker);
            }
            _Markers.Clear();
        }

        return tempBuffer;
	}

	private List<Utilities.Marker> DecodeMarkers(int i_nSize, Byte[] i_Data)
	{
		List<Utilities.Marker> retList = new List<Utilities.Marker>();

        if(i_nSize > 0)
        {
            int numMarkers = BitConverter.ToInt32(i_Data, 0);
            int dataOffset = sizeof(int);

            for(int i = 0; i < numMarkers; i++)
            {
                Utilities.Marker tempMarker = new Utilities.Marker();

                tempMarker.ID = BitConverter.ToInt32(i_Data, dataOffset);
                dataOffset += sizeof(int);

                tempMarker.ScreenLoc.x = BitConverter.ToSingle(i_Data, dataOffset);
                dataOffset += sizeof(float);
                tempMarker.ScreenLoc.y = BitConverter.ToSingle(i_Data, dataOffset);
                dataOffset += sizeof(float);

                //tempMarker.WorldLoc.x = BitConverter.ToSingle(i_Data, dataOffset);
                //dataOffset += sizeof(float);
                //tempMarker.WorldLoc.y = BitConverter.ToSingle(i_Data, dataOffset);
                //dataOffset += sizeof(float);
                //tempMarker.WorldLoc.z = BitConverter.ToSingle(i_Data, dataOffset);
                //dataOffset += sizeof(float);

                retList.Add(tempMarker);
            }
        }

        return retList;
	}
}
