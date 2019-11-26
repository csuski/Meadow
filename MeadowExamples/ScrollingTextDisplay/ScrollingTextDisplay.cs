using Meadow.Foundation.Graphics;

namespace ScrollingTextDisplay
{
    public class ScrollingTextDisplay
    {
        private readonly GraphicsLibrary _graphicsLibrary;
        private readonly string[] _lineBuffer;
        private readonly int _lineHeight;
        private readonly int _lineLimit;
        private readonly int _characterLimit;
        private int _currentLine = 0;
        private Meadow.Foundation.Color _color = Meadow.Foundation.Color.White;

        public ScrollingTextDisplay(GraphicsLibrary graphicsLibrary, int height, int width)
        {
            _graphicsLibrary = graphicsLibrary;
            _lineHeight = _graphicsLibrary.CurrentFont.Height;
            _lineLimit = height / _lineHeight;
            _characterLimit = width / _graphicsLibrary.CurrentFont.Width;
            _lineBuffer = new string[_lineLimit];
        }

        public Meadow.Foundation.Color Color
        {
            get => _color; set
            {
                _color = value;
                DrawBuffer();
            }
        }

        public void WriteLine(string line)
        {
            AddLinesToBuffer(line);
            DrawBuffer();
        }

        private void AddLinesToBuffer(string line)
        {
            while (line.Length > _characterLimit)
            {
                var substr = line.Substring(0, _characterLimit);
                AddToBuffer(substr);
                line = line.Remove(0, _characterLimit);
            }
            AddToBuffer(line);
        }

        public void Clear()
        {
            ClearBuffer();
            DrawBuffer();
        }

        private void ClearBuffer()
        {
            for (int i = 0; i < _lineBuffer.Length; i++)
            {
                _lineBuffer[i] = string.Empty;
            }
            _currentLine = 0;
        }

        private void AddToBuffer(string newLine)
        {
            if (_currentLine < _lineLimit)
            {
                _lineBuffer[_currentLine++] = newLine;
            }
            else
            {
                for (int i = 0; i < _lineBuffer.Length - 1; i++)
                {
                    _lineBuffer[i] = _lineBuffer[i + 1];
                }
                _lineBuffer[_lineBuffer.Length - 1] = newLine;
            }
        }

        private void DrawBuffer()
        {
            _graphicsLibrary.Clear();
            for (int i = 0; i < _currentLine; i++)
            {
                _graphicsLibrary.DrawText(0, _lineHeight * i, _lineBuffer[i], _color);
            }
            _graphicsLibrary.Show();
        }
    }
}
