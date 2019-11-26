using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using System.Threading.Tasks;

namespace ScrollingTextDisplay
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private ST7789 display;
        private ISpiBus spiBus;
        private GraphicsLibrary graphicsLib;
        private IDigitalOutputPort _blueLED;
        private ScrollingTextDisplay _logger;

        public MeadowApp()
        {
            ConfigurePorts();
            CreateLogger();
            BlinkLED();
        }

        public void ConfigurePorts()
        {
            _blueLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);

            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);
            spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);
            display = new ST7789(device: Device, spiBus: spiBus,
               chipSelectPin: Device.Pins.D02,
               dcPin: Device.Pins.D01,
               resetPin: Device.Pins.D00,
               width: 135, height: 240);

            graphicsLib = new GraphicsLibrary(display);
            graphicsLib.CurrentFont = new Font8x8();

            graphicsLib.Clear();
            graphicsLib.CurrentRotation = GraphicsLibrary.Rotation._90Degrees;
            display.Clear(Meadow.Foundation.Color.Black, true);

                
        }

        public void CreateLogger()
        {
            if (graphicsLib.CurrentRotation == GraphicsLibrary.Rotation._90Degrees ||
                graphicsLib.CurrentRotation == GraphicsLibrary.Rotation._270Degrees)
            {
                _logger = new ScrollingTextDisplay(graphicsLib, (int)display.Width, (int)display.Height);
            }
            else
            {
                _logger = new ScrollingTextDisplay(graphicsLib, (int)display.Height, (int)display.Width);
            }
        }

        public void BlinkLED()
        {
            Task t = new Task(async () => {
                while (true)
                {
                    _blueLED.State = true;
                    _logger.WriteLine("Blue LED On");
                    await Task.Delay(250);
                    _blueLED.State = false;
                    _logger.WriteLine("Blue LED Off");
                    await Task.Delay(250);
                }
            });
            t.Start();
        }
    }
}