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
    }
}
