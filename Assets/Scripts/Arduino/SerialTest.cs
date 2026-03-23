using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class SerialTest : MonoBehaviour

    // we're running this script in editor and play mode, so we are going to add a couple functionalities to it
{
    #region Serial Port Setup
    [Tooltip("The port to check")]
    public string Port;
    [Tooltip("The associated baudrate with the port")]
    public short Baudrate;
    #endregion

    #region Misc
    [Tooltip("The speed at which data sent to the serial port (if any) should be read")]
    public short ReadSpeed;
    #endregion

    private SerialRead sr;

    void Start()
    {
        // serial read only exists when the application is playing it seems
        sr = new SerialRead(Port, Baudrate);
        sr.Open();
        sr.StartMonitoring(ReadSpeed);

        Application.quitting += (() =>
        {
            sr.Close();
        });
    }
}
