using UnityEngine;
using System;
using System.IO.Ports;


public class SerialRead
{
    private SerialPort serialPort;

    #region Port Logistics
    private string port;
    private short baudrate;
    private Parity parity;
    private byte dataBits;
    private StopBits stopBits;
    #endregion 

    public SerialRead (string port, short baudrate = 9600, Parity parity = Parity.None, byte dataBits = 8, StopBits stopBits = StopBits.One)
    {
        this.port = port;
        this.baudrate = baudrate;
        this.dataBits = dataBits;
        this.parity = parity;
        this.stopBits = stopBits;
    }

    // try to access our serial port
    public void Open()
    {
        try
        {
            // Debug log is not a functionality of a monobehaviour but of the unity software itself
            // as long as we're using a unity project, we can use debug.log!
            Debug.Log("Trying to open...");

            serialPort = new SerialPort(port, baudrate, parity, dataBits, stopBits);
            serialPort.DataReceived += SerialPortDataReceived;
            serialPort.Open();
        }
        catch (Exception err)
        {
            Debug.Log($"Failed. Serial port '{port}' cannot be initialized: {err.Message}");
        }
    }

    // read our data
    private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string data = serialPort.ReadLine();

        Debug.Log(data);
    }

    // close our serial port
    public void Close()
    {
        if (serialPort != null)
        {
            Debug.Log("Closing port...");

            serialPort.Close();
        }
        else
        {
            Debug.Log("No valid port has been identified. Cannot close null");
        }

    }
}
