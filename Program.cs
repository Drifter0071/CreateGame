using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;

namespace CreateGame;

public class Program
{
        private static IWindow? window;
        private static GL? Gl;

        public static void Main()
        {
                var options = WindowOptions.Default;
                options.Size = new Vector2D<int>(1280, 720);
                options.Title = "The Create Game";

                window = Window.Create(options);

                // window events
                window.Load += OnLoad;
                window.Render += OnRender;
                window.Closing += OnClose;

                // start game loop
                window.Run();
        }

        private static void OnLoad()
        {
                Gl = GL.GetApi(window);
                IInputContext input = window!.CreateInput();
                for (int i = 0; i < input.Keyboards.Count; i++)
                {
                        input.Keyboards[i].KeyDown += KeyDown;
                }
        }

        private static void OnRender(double deltaTime)
        {
                float backgroundHue = 245f / 255f;

                Gl!.ClearColor(backgroundHue, backgroundHue, backgroundHue, 1.0f);

                // clear the screen
                Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static void OnClose()
        {
                Gl!.Dispose();
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
                if (key == Key.Escape)
                {
                        window!.Close();
                }
        }
}