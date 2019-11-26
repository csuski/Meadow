using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace ScrollingTextDisplay
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private ST7789 display;
        private ISpiBus spiBus;
        private GraphicsLibrary graphicsLib;

        public MeadowApp()
        {
            ConfigurePorts();
            DrawText();
        }

        public void ConfigurePorts()
        {
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

        public void DrawText()
        {
            graphicsLib.DrawText(0, 0, "test text", Meadow.Foundation.Color.White);
            graphicsLib.Show();
        }
    }
}