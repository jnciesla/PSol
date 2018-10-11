using System;
using Microsoft.Xna.Framework.Input;

namespace PSol.Client
{
    internal class KeyControl
    {
        private Keys current;
        private static bool previousMouse;
        private static DateTime previousClick;

        public bool KeyPress(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
            {
                current = key;
            }

            if (!Keyboard.GetState().IsKeyUp(key) || current != key) return false;
            current = Keys.None;
            return true;

        }

        public bool Click()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                previousMouse = true;
            if (Mouse.GetState().LeftButton != ButtonState.Released || !previousMouse) return false;
            previousMouse = false;
            previousClick = DateTime.Now;
            return true;
        }

        public bool DoubleClick()
        {
            var interval = (DateTime.Now - previousClick).TotalSeconds;
            return Click() && interval < 0.3F && interval > 0.1F;
        }

        public bool CheckAlt()
        {
            return Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt);
        }

        public bool CheckCtrl()
        {
            return Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl);
        }
        public bool CheckShift()
        {
            return Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);
        }
    }
}
