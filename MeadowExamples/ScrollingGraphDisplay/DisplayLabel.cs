using Meadow.Foundation.Graphics;

namespace ScrollingGraphDisplay
{
    public class DisplayLabel
    {
        public enum Alignment { Center, Left, Right};

        public FontBase Font;
        public Meadow.Foundation.Color? Color;
        public string Text;
        public Alignment TextAlignment;
    }
}
