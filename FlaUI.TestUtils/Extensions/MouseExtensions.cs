using System.Drawing;
using FlaUI.Core.Input;

namespace UI.TestUtils.Extensions
{
    public static class MouseEx
    {
        public static void Click(MouseButton button, Point point, bool moveMouse)
        {
            if (moveMouse)
                Mouse.MoveTo(point);

            Mouse.Click(button, point);
        }
    }
}
