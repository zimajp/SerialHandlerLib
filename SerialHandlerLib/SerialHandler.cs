using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace SerialHandlerLib
{
    public class SerialHandler: IDisposable
    {
        public delegate void SerialDataRecaivedEventhandler(string message);
        public delegate void SendLogEventHandler(string log);
        public event SerialDataRecaivedEventhandler OnDataReceived;
        public event SendLogEventHandler SendLog;

        private bool isReading = false;
        private string message;

        SerialPort serial;

        public void Init(string portName, int baudRate)
        {
            serial = new SerialPort(portName, baudRate);
            try
            {
                serial.Open();
                serial.ReadTimeout = 100;
                serial.WriteTimeout = 100;
                SendLog("Serialhandler was initialized.");
            }
            catch (Exception e)
            {
                serial = null;
                SendLog(e.Message);
            }
        }

        public void Dispose()
        {
            isReading = false;
            if (serial.IsOpen)
            {
                serial.Close();
            }
            if (serial != null)
            {
                serial.Dispose();
            }
            SendLog("Serialhandler was disposed.");
        }

        public void StartRead()
        {
            if (serial == null || !serial.IsOpen)
            {
                SendLog("serial port is not opened.");
                return;
            }

            isReading = true;
            try
            {
                Task.Run(() =>
                {
                    while (serial != null && serial.IsOpen && isReading)
                    {
                        message = serial.ReadLine();
                        OnDataReceived(message);
                    }
                    SendLog("Serialhandler ended to read messages from serial port.");
                });
                SendLog("Serialhandler started to read messages from serial port.");
            }
            catch(AggregateException ae)
            {
                foreach (var ie in ae.InnerExceptions)
                {
                    SendLog(ie.Message);
                }
            }
            catch(Exception e)
            {
                SendLog(e.Message);
            }
        }

        public void EndRead()
        {
            isReading = false;
        }

        public void Write(string message)
        {
            try
            {
                serial.Write(message);
                SendLog("Serialhandler wrote messsage to serial port.");
            }
            catch(Exception e)
            {
                SendLog(e.Message);
            }
        }
    }
}
