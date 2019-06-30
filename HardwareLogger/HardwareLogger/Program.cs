using System;
using System.Threading;
using OpenHardwareMonitor.Hardware;

namespace HardwareLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            RuntimeConfig.RequireAdministrator();

            while (true)
            {
                GetSystemInfo();
                Thread.Sleep(1000);
            }
        }

        static void GetSystemInfo()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true
            };
            computer.Open();
            computer.Accept(updateVisitor);

            Timer timer = new Timer(Callback, null, 0, 1000);

            void Callback(object state)
            {
                Console.Clear();
                Console.WriteLine("{0}\n", DateTime.Now);

                foreach (IHardware hardware in computer.Hardware)
                {
                    hardware.Update();

                    Console.WriteLine("{0}: {1}", hardware.HardwareType, hardware.Name);
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        // Celsius is default unit
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            Console.WriteLine("{0}: {1}°C", sensor.Name, sensor.Value);
                            // Console.WriteLine("{0}: {1}°F", sensor.Name, sensor.Value*1.8+32);
                        }

                    }

                    Console.WriteLine();
                }
            }

            computer.Close();
        }
    }
}
