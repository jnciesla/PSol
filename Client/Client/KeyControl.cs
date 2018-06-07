using System.Runtime.Serialization.Formatters;
using Microsoft.Xna.Framework.Input;

namespace Client
{
    class KeyControl
    {
        private Keys current;

        public bool KeyPress(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
            {
                current = key;
            }
            if (Keyboard.GetState().IsKeyUp(key) && current == key)
            {
                current = Keys.None;
                return true;
            }

            return false;
        }

        public bool CheckAlt()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt))
            {
                return true;
            }
            return false;
        }

        public bool CheckCtrl()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl))
            {
                return true;
            }
            return false;
        }
    }
}
