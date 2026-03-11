using System.IO.Ports;
using UnityEngine;

public class SerialTest : MonoBehaviour
{
    public string Port;
    public short Baudrate;

    private SerialRead sr;

    void Start()
    {
        sr = new SerialRead(Port, Baudrate);
        sr.Open();
    }
}
