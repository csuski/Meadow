using Meadow.Foundation.Graphics;
using System;
using System.Collections;
using System.Diagnostics;

namespace ScrollingGraphDisplay
{
    public class ScrollingGraphDisplay
    {
        private class GraphPane
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        private readonly GraphicsLibrary _graphicsLibrary;
        private readonly int _height;
        private readonly int _width;
        private GraphPane _graphPane;

        // Ideally we will get the height and width from the graphics library class and not need them as arguments
        public ScrollingGraphDisplay(GraphicsLibrary graphicsLibrary, int height, int width)
        {
            _graphicsLibrary = graphicsLibrary;
            _height = height;
            _width = width;
        }


        // Properties
        public DisplayLabel XAxis { get; set; }
        public DisplayLabel YAxis { get; set; }
        public DisplayLabel Title { get; set; }
        public Meadow.Foundation.Color LineColor { get; set; } = Meadow.Foundation.Color.Yellow;
        public Meadow.Foundation.Color BorderColor { get; set; } = Meadow.Foundation.Color.White;

        private Queue _data = new Queue();

        public void AddData(int val)
        {
            _data.Enqueue(val);
            if (_graphPane != null)
            {
                while (_data.Count > _graphPane.Width)
                {
                    _data.Dequeue();
                }
            }
        }

        public void Draw()
        {
            _graphicsLibrary.Clear();
            _graphPane = DrawLabels();
            // Clearing just the graph pane is way slower than clearing everything and redrawing
            /*            
            if (_graphPane == null)
            {
                // TODO: If any label stuff changes we need to set the graphpane to null
                _graphicsLibrary.Clear();
                _graphPane = DrawLabels();
            }
            else
            {
                ClearGraphPane();
            }*/
            DrawData();
            _graphicsLibrary.Show();
        }

        private void ClearGraphPane()
        {
            // TODO: can you change the background color?
            _graphicsLibrary.DrawRectangle(_graphPane.X, _graphPane.Y, _graphPane.Width, _graphPane.Height,
                Meadow.Foundation.Color.Black, true);
        }


        private void DrawData()
        {
            if (_data.Count < 2) return;
            while (_data.Count > _graphPane.Width)
            {
                // If this is the first draw, there may be more data than we can draw,
                // Dequeue until we match the width
                _data.Dequeue(); 
            }

            GetMinMax(_data, out int min, out int max);
            var range = max - min;
            
            var dataNorm = 1.0;
            if (range == 0)
            {
                // Center the line if there is no range
                max = max + _graphPane.Height / 2;
                min = min - _graphPane.Height / 2;
            }
            else
            {
                dataNorm = (double)range / (double)_graphPane.Height;
            }
            //Console.WriteLine($"Min = {min}, Max = {max}, Range = {range}, DataNorm = {dataNorm}");
            DrawDataLine(_data, dataNorm, min);
        }


        public void DrawDataLine(IEnumerable data, double normalize, int min)
        {
            int x = 0, startY = 0, endY;
            foreach(int d in data)
            {
                if(x == 0)
                {
                    startY = (int)(d / normalize);
                    x++;
                    continue;
                }
                endY = (int)(d / normalize);
                DrawLine(x-1, startY, x, endY, (int)(min / normalize));
                x++;
                startY = endY;
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int minVal)
        {
            //Console.WriteLine($"Draw Line {_graphPane.X + x1}, {_graphPane.Y + _graphPane.Height + (y1 - minVal)} " +
            //    $"to {_graphPane.X + x2}, {_graphPane.Y + _graphPane.Height + (y2 - minVal)} with min value = {minVal}");
            // draw the line in relation to the graph pane
            _graphicsLibrary.DrawLine(
                _graphPane.X + x1,
                _graphPane.Y + _graphPane.Height + minVal - y1,
                _graphPane.X + x2,
                _graphPane.Y + _graphPane.Height + minVal - y2, LineColor);
        }


        public void GetMinMax(IEnumerable data, out int min, out int max)
        {
            min = int.MaxValue;
            max = int.MinValue;

            foreach(int d in data)
            {
                min = Math.Min(min, d);
                max = Math.Max(max, d);
            }
        }

        private GraphPane DrawLabels()
        {
            var graphX = 0;
            var graphY = 0;
            var graphWidth = _width;
            var graphHeight = _height;


            if(Title != null && !string.IsNullOrWhiteSpace(Title.Text))
            {
                int x = GetStartX(Title, _width);
                int y = 0;

                graphY = Title.Font == null ? _graphicsLibrary.CurrentFont.Height : Title.Font.Height;
                graphHeight -= graphY;

                DrawLabel(x, y, Title.Text, Title.Font, Title.Color);
            }
            if (XAxis != null && !string.IsNullOrWhiteSpace(XAxis.Text))
            {
                int x = GetStartX(XAxis, _width);
                var fontHeight = XAxis.Font == null ? _graphicsLibrary.CurrentFont.Height : XAxis.Font.Height;
                int y = _height - fontHeight;
                graphHeight -= fontHeight;
                DrawLabel(x, y, XAxis.Text, XAxis.Font, XAxis.Color);
            }
            if (YAxis != null && !string.IsNullOrWhiteSpace(YAxis.Text))
            {
                int x = GetStartX(YAxis, _height);
                int y = 0;

                graphX = YAxis.Font == null ? _graphicsLibrary.CurrentFont.Height : YAxis.Font.Height;
                graphWidth -= graphY;
                _graphicsLibrary.CurrentRotation = PreviousRotation(_graphicsLibrary.CurrentRotation);
                DrawLabel(x, y, YAxis.Text, YAxis.Font, YAxis.Color);
                _graphicsLibrary.CurrentRotation = NextRotation(_graphicsLibrary.CurrentRotation);
            }
            DrawBorder(graphX + 1, graphY + 1, graphX + graphWidth - 1, graphY + graphHeight - 1);
            return new GraphPane { X = graphX + 2, Y = graphY + 2, Width = graphWidth - 4, Height = graphHeight - 4 };
        }

        private void DrawBorder(int x1, int y1, int x2, int y2)
        {
            _graphicsLibrary.DrawLine(x1, y1, x1, y2, BorderColor);
            _graphicsLibrary.DrawLine(x1, y2, x2, y2, BorderColor);
            _graphicsLibrary.DrawLine(x2, y2, x2, y1, BorderColor);
            _graphicsLibrary.DrawLine(x2, y1, x1, y1, BorderColor);
        }


        private static GraphicsLibrary.Rotation NextRotation(GraphicsLibrary.Rotation rotation)
        {
            if ((int)rotation == 3) return 0;
            return rotation + 1;
        }

        private static GraphicsLibrary.Rotation PreviousRotation(GraphicsLibrary.Rotation rotation)
        {
            if ((int)rotation == 0) return (GraphicsLibrary.Rotation)3;
            return rotation - 1;
        }

        private void DrawLabel(int x, int y, string text, FontBase font, Meadow.Foundation.Color? color)
        {
            FontBase oldFont = null;
            if (font != null)
            {
                oldFont = _graphicsLibrary.CurrentFont;
                _graphicsLibrary.CurrentFont = font;
            }
            if (color.HasValue)
            {
                _graphicsLibrary.DrawText(x, y, text, color.Value);
            }
            else
            {
                _graphicsLibrary.DrawText(x, y, text);
            }
            if (oldFont != null)
            {
                _graphicsLibrary.CurrentFont = oldFont;
            }
        }

        private int GetStartX(DisplayLabel label, int width)
        {
            var fontWidth = label.Font != null ? label.Font.Width : _graphicsLibrary.CurrentFont.Width;
            var textLength = label.Text.Length * fontWidth;

            var startX = 0;

            // If it is greater than or equal to the available space, 
            // just write as much as we can starting from the beginning - igore alignment
            if (textLength < width && label.TextAlignment != DisplayLabel.Alignment.Left)
            {
                startX = width - textLength; // right alignment
                if (label.TextAlignment == DisplayLabel.Alignment.Center)
                {
                    startX /= 2;
                }
            }
            return startX;
        }






    }
}
