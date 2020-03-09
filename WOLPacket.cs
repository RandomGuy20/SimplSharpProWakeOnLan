using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharpPro;


namespace WakeOnLan
{
    public class WOLPacket
    {
        // Fields
        private SocketErrorCodes errorCode;
        private UDPServer wakeOnLan;
        private byte[] wolPacket = new byte[102];
        private int portNumber;
        private string ipAddress;
        private byte[] macAddress = new byte[5];

        
        // Constructor
        /// <summary>
        /// Format the Mac Address like this 00:11:22:33:44:55
        /// </summary>
        /// <param name="MacAddress"></param>
        /// <param name="IpAddress"></param>
        /// <param name="PortNumber"></param>
        public WOLPacket(string MacAddress, string IpAddress, int PortNumber)
        {
            try
            {
                wakeOnLan = new UDPServer();
                portNumber = PortNumber;
                ipAddress = IpAddress;

                //Convert String to byte
                macAddress = MacAddress.Split(':').Select(x => Convert.ToByte(x, 16)).ToArray(); 

                // Start Packet Create
                for (int i = 0; i < 6; i++)
                {
                    wolPacket[i] = 0xFF;
                }
                for (int i = 1; i <= 16; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        wolPacket[i * 6 + j] = macAddress[j];
                    }
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in UDp Constructor is: " + e);
            }
        }

        //Events and Delegates
        public delegate void UDPStatusEventHandler(SocketErrorCodes args);
        public event UDPStatusEventHandler UDPStatusChangeEvent;

        // Methods
        public void WakeOnLAn()
        {
            try
            {
     
                errorCode = wakeOnLan.EnableUDPServer(IPAddress.Any, 0, 9);

                if (errorCode != SocketErrorCodes.SOCKET_OK)
                    ErrorCodes(errorCode);             

                wakeOnLan.EthernetAdapterToBindTo = EthernetAdapterType.EthernetLANAdapter;
                
                errorCode = wakeOnLan.SendData(wolPacket, wolPacket.Length, ipAddress, portNumber, false);
                CrestronConsole.PrintLine("Ip Address is: {0} and Port is {1} and packet length is: {2}", ipAddress, portNumber, wolPacket.Length);
                if (errorCode != SocketErrorCodes.SOCKET_OK)
                    ErrorCodes(errorCode);
               

                string message = BitConverter.ToString(wolPacket);
                CrestronConsole.PrintLine(message);

                wakeOnLan.DisableUDPServer();
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("WakeOn LAn Send packet Error is: " + e);
            }


        }

        private void ErrorCodes(SocketErrorCodes err)
        {
            switch (err)
            {
                case SocketErrorCodes.SOCKET_ADDRESS_NOT_SPECIFIED:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_ADDRESS_NOT_SPECIFIED);
                    break;
                case SocketErrorCodes.SOCKET_BUFFER_NOT_ALLOCATED:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_BUFFER_NOT_ALLOCATED);
                    break;
                case SocketErrorCodes.SOCKET_CONNECTION_IN_PROGRESS:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_CONNECTION_IN_PROGRESS);
                    break;
                case SocketErrorCodes.SOCKET_INVALID_ADDRESS_ADAPTER_BINDING:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_INVALID_ADDRESS_ADAPTER_BINDING);
                    break;
                case SocketErrorCodes.SOCKET_INVALID_CLIENT_INDEX:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_INVALID_CLIENT_INDEX);
                    break;
                case SocketErrorCodes.SOCKET_INVALID_PORT_NUMBER:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_INVALID_PORT_NUMBER);
                    break;
                case SocketErrorCodes.SOCKET_INVALID_STATE:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_INVALID_STATE);
                    break;
                case SocketErrorCodes.SOCKET_MAX_CONNECTIONS_REACHED:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_MAX_CONNECTIONS_REACHED);
                    break;
                case SocketErrorCodes.SOCKET_NOT_ALLOWED_IN_SECURE_MODE:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_NOT_ALLOWED_IN_SECURE_MODE);
                    break;
                case SocketErrorCodes.SOCKET_NOT_CONNECTED:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_NOT_CONNECTED);
                    break;
                case SocketErrorCodes.SOCKET_OK:
                    UDPStatusChangeEvent(SocketErrorCodes.SOCKET_OK);
                    break;
            }
        }


    }
}