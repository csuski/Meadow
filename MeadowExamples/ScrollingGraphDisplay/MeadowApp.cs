using System;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace ScrollingGraphDisplay
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private St7789 _display;
        private ISpiBus _spiBus;
        private GraphicsLibrary _graphicsLib;
        private IDigitalOutputPort _blueLED;
        private ScrollingGraphDisplay _graph;

        public MeadowApp()
        {
            ConfigurePorts();
            CreateGraph();
            DrawRandomData();
        }

        public void ConfigurePorts()
        {
            _blueLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);

            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);
            _spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);
            _display = new St7789(device: Device, spiBus: _spiBus,
               chipSelectPin: Device.Pins.D02,
               dcPin: Device.Pins.D01,
               resetPin: Device.Pins.D00,
               width: 135, height: 240);

            _graphicsLib = new GraphicsLibrary(_display);
            _graphicsLib.CurrentFont = new Font8x8();
            _graphicsLib.Clear();
            _graphicsLib.Rotation = GraphicsLibrary.RotationType._90Degrees;
            _display.Clear(Meadow.Foundation.Color.Black, true);
        }

        public void CreateGraph()
        {
            Console.WriteLine("Create Graph");
            _graph = new ScrollingGraphDisplay(_graphicsLib, (int)_display.Width, (int)_display.Height);
            _graph.Title = new DisplayLabel
            {
                Color = Meadow.Foundation.Color.Red,
                Text = "TITLE",
                TextAlignment = DisplayLabel.Alignment.Center,
                Font = new Font12x16()
            };

            _graph.XAxis = new DisplayLabel
            {
                Color = Meadow.Foundation.Color.Blue,
                Text = "X Axis",
                TextAlignment = DisplayLabel.Alignment.Center,
                Font = new Font8x8()
            };

            _graph.YAxis = new DisplayLabel
            {
                Color = Meadow.Foundation.Color.Green,
                Text = "Y Axis",
                TextAlignment = DisplayLabel.Alignment.Center,
                Font = new Font8x8()
            };
        }

        public void DrawRandomData() { 
            Console.WriteLine("Drawing....");

            // Init with some 0s            
            for(int i =0; i< 20; i++)
                _graph.AddData(0);
            
            Random rand = new Random();
            var prevData = 0;
            while (true)
            {
                var data = rand.Next(prevData - 10, prevData + 10);
                _graph.AddData(data);
                _graph.Draw();
                prevData = data;
            }
        }
    }
}
