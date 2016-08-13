using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using CUE.NET;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Exceptions;
using CUE.NET.Devices.Generic.Enums;
using System.Drawing;
using CUE.NET.Devices.Keyboard.Keys;
using CUE.NET.Devices.Keyboard.Enums;
using System.Threading.Tasks;
using System.Threading;

namespace Keyboard_Lighting_Controller
{
    class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static IntPtr _hookID = IntPtr.Zero;

        //These Dll's will handle the hooks. Yaaar mateys!

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // The two dll imports below will handle the window hiding.

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("line0");
            //var handle = GetConsoleWindow();

            // Hide
            //ShowWindow(handle, SW_HIDE);

            _hookID = SetHook(HookCallback);

            Console.WriteLine("Startup");
            //Application.Run();

            TestKeyboardLights();

            //UnhookWindowsHookEx(_hookID);
            Console.WriteLine("unhooked");

        }

        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Console.WriteLine("Code=" + nCode);

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                CorsairKeyboardKeyId kID = ConvertKeyToCorsairKeyID((Keys)vkCode);

                Console.WriteLine((Keys)vkCode + " - "+kID);

                //StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                //sw.Write((Keys)vkCode);
                //sw.Close();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static CorsairKeyboardKeyId ConvertKeyToCorsairKeyID(Keys k)
        {
            switch (k)
            {
                case Keys.Oemtilde: return CorsairKeyboardKeyId.GraveAccentAndTilde;
                case Keys.D1: return CorsairKeyboardKeyId.D1;
                case Keys.D2: return CorsairKeyboardKeyId.D2;
                case Keys.D3: return CorsairKeyboardKeyId.D3;
                case Keys.D4: return CorsairKeyboardKeyId.D4;
                case Keys.D5: return CorsairKeyboardKeyId.D5;
                case Keys.D6: return CorsairKeyboardKeyId.D6;
                case Keys.D7: return CorsairKeyboardKeyId.D7;
                case Keys.D8: return CorsairKeyboardKeyId.D8;
                case Keys.D9: return CorsairKeyboardKeyId.D9;
                case Keys.D0: return CorsairKeyboardKeyId.D0;
                case Keys.OemMinus: return CorsairKeyboardKeyId.MinusAndUnderscore;
                case Keys.Oemplus: return CorsairKeyboardKeyId.EqualsAndPlus;
                case Keys.Back: return CorsairKeyboardKeyId.Backspace;
                case Keys.A: return CorsairKeyboardKeyId.A;
                case Keys.B: return CorsairKeyboardKeyId.B;
                case Keys.C: return CorsairKeyboardKeyId.C;
                case Keys.D: return CorsairKeyboardKeyId.D;
                case Keys.E: return CorsairKeyboardKeyId.E;
                case Keys.F: return CorsairKeyboardKeyId.F;
                case Keys.G: return CorsairKeyboardKeyId.G;
                case Keys.H: return CorsairKeyboardKeyId.H;
                case Keys.I: return CorsairKeyboardKeyId.I;
                case Keys.J: return CorsairKeyboardKeyId.J;
                case Keys.K: return CorsairKeyboardKeyId.K;
                case Keys.L: return CorsairKeyboardKeyId.L;
                case Keys.M: return CorsairKeyboardKeyId.M;
                case Keys.N: return CorsairKeyboardKeyId.N;
                case Keys.O: return CorsairKeyboardKeyId.O;
                case Keys.P: return CorsairKeyboardKeyId.P;
                case Keys.Q: return CorsairKeyboardKeyId.Q;
                case Keys.R: return CorsairKeyboardKeyId.R;
                case Keys.S: return CorsairKeyboardKeyId.S;
                case Keys.T: return CorsairKeyboardKeyId.T;
                case Keys.U: return CorsairKeyboardKeyId.U;
                case Keys.V: return CorsairKeyboardKeyId.V;
                case Keys.W: return CorsairKeyboardKeyId.W;
                case Keys.X: return CorsairKeyboardKeyId.X;
                case Keys.Y: return CorsairKeyboardKeyId.Y;
                case Keys.Z: return CorsairKeyboardKeyId.Z;
                case Keys.OemOpenBrackets: return CorsairKeyboardKeyId.BracketLeft;
                case Keys.OemCloseBrackets: return CorsairKeyboardKeyId.BracketRight;
                case Keys.OemBackslash: return CorsairKeyboardKeyId.Backslash;
                case Keys.OemSemicolon: return CorsairKeyboardKeyId.SemicolonAndColon;
                case Keys.OemQuotes: return CorsairKeyboardKeyId.ApostropheAndDoubleQuote;
                case Keys.Oemcomma: return CorsairKeyboardKeyId.CommaAndLessThan;
                case Keys.OemPeriod: return CorsairKeyboardKeyId.PeriodAndBiggerThan;
                case Keys.OemQuestion: return CorsairKeyboardKeyId.SlashAndQuestionMark;
                case Keys.Return: return CorsairKeyboardKeyId.Enter;
                case Keys.Tab: return CorsairKeyboardKeyId.Tab;
                case Keys.CapsLock: return CorsairKeyboardKeyId.CapsLock;
                case Keys.LControlKey: return CorsairKeyboardKeyId.LeftCtrl;
                case Keys.LShiftKey: return CorsairKeyboardKeyId.LeftShift;
                case Keys.LWin: return CorsairKeyboardKeyId.LeftGui;
                case Keys.LMenu: return CorsairKeyboardKeyId.LeftAlt;
                case Keys.RMenu: return CorsairKeyboardKeyId.RightAlt;
                case Keys.RControlKey: return CorsairKeyboardKeyId.RightCtrl;
                case Keys.RShiftKey: return CorsairKeyboardKeyId.RightShift;
                case Keys.RWin: return CorsairKeyboardKeyId.RightGui;
                case Keys.Apps: return CorsairKeyboardKeyId.Application;
                case Keys.Space: return CorsairKeyboardKeyId.Space;
                case Keys.Escape: return CorsairKeyboardKeyId.Escape;
                case Keys.F1: return CorsairKeyboardKeyId.F1;
                case Keys.F2: return CorsairKeyboardKeyId.F2;
                case Keys.F3: return CorsairKeyboardKeyId.F3;
                case Keys.F4: return CorsairKeyboardKeyId.F4;
                case Keys.F5: return CorsairKeyboardKeyId.F5;
                case Keys.F6: return CorsairKeyboardKeyId.F6;
                case Keys.F7: return CorsairKeyboardKeyId.F7;
                case Keys.F8: return CorsairKeyboardKeyId.F8;
                case Keys.F9: return CorsairKeyboardKeyId.F9;
                case Keys.F10: return CorsairKeyboardKeyId.F10;
                case Keys.F11: return CorsairKeyboardKeyId.F11;
                case Keys.F12: return CorsairKeyboardKeyId.F12;
                case Keys.PrintScreen: return CorsairKeyboardKeyId.PrintScreen;
                case Keys.Scroll: return CorsairKeyboardKeyId.ScrollLock;
                case Keys.Pause: return CorsairKeyboardKeyId.PauseBreak;
                case Keys.Insert: return CorsairKeyboardKeyId.Insert;
                case Keys.Home: return CorsairKeyboardKeyId.Home;
                case Keys.PageUp: return CorsairKeyboardKeyId.PageUp;
                case Keys.Delete: return CorsairKeyboardKeyId.Delete;
                case Keys.End: return CorsairKeyboardKeyId.End;
                case Keys.PageDown: return CorsairKeyboardKeyId.PageDown;
                case Keys.Up: return CorsairKeyboardKeyId.UpArrow;
                case Keys.Down: return CorsairKeyboardKeyId.DownArrow;
                case Keys.Left: return CorsairKeyboardKeyId.LeftArrow;
                case Keys.Right: return CorsairKeyboardKeyId.RightArrow;
                case Keys.NumLock: return CorsairKeyboardKeyId.NumLock;
                case Keys.Divide: return CorsairKeyboardKeyId.KeypadSlash;
                case Keys.Multiply: return CorsairKeyboardKeyId.KeypadAsterisk;
                case Keys.Subtract: return CorsairKeyboardKeyId.KeypadMinus;
                case Keys.Add: return CorsairKeyboardKeyId.KeypadPlus;
                //case Keys.Enter: return CorsairKeyboardKeyId.KeypadEnter;
                case Keys.Decimal: return CorsairKeyboardKeyId.KeypadPeriodAndDelete;
                case Keys.NumPad0: return CorsairKeyboardKeyId.Keypad0;
                case Keys.NumPad1: return CorsairKeyboardKeyId.Keypad1;
                case Keys.NumPad2: return CorsairKeyboardKeyId.Keypad2;
                case Keys.NumPad3: return CorsairKeyboardKeyId.Keypad3;
                case Keys.NumPad4: return CorsairKeyboardKeyId.Keypad4;
                case Keys.NumPad5: return CorsairKeyboardKeyId.Keypad5;
                case Keys.NumPad6: return CorsairKeyboardKeyId.Keypad6;
                case Keys.NumPad7: return CorsairKeyboardKeyId.Keypad7;
                case Keys.NumPad8: return CorsairKeyboardKeyId.Keypad8;
                case Keys.NumPad9: return CorsairKeyboardKeyId.Keypad9;
                case Keys.MediaPlayPause: return CorsairKeyboardKeyId.PlayPause;
                case Keys.MediaPreviousTrack: return CorsairKeyboardKeyId.ScanPreviousTrack;
                case Keys.MediaNextTrack: return CorsairKeyboardKeyId.ScanNextTrack;
                case Keys.MediaStop: return CorsairKeyboardKeyId.Stop;
                case Keys.VolumeMute: return CorsairKeyboardKeyId.Mute;
                default: return CorsairKeyboardKeyId.Invalid;
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static void TestKeyboardLights()
        {
            try
            {
                CueSDK.Initialize();
                Console.WriteLine("Initialized with " + CueSDK.LoadedArchitecture + "-SDK");

                // Get connected keyboard or throw exception if there is no light controllable keyboard connected
                CorsairKeyboard keyboard = CueSDK.KeyboardSDK;
                if (keyboard == null)
                    throw new WrapperException("No keyboard found");
                
                keyboard.UpdateMode = UpdateMode.Continuous;
                //keyboard.Brush = new SolidColorBrush(Color.Blue);

                RectangleF spot = new RectangleF(keyboard.KeyboardRectangle.Width / 2f, keyboard.KeyboardRectangle.Y / 2f, 160, 80);
                PointF target = new PointF(spot.X, spot.Y);
                //RectangleKeyGroup spotGroup = new RectangleKeyGroup(keyboard, spot) { Brush = new LinearGradientBrush(new RainbowGradient()) };


                keyboard.Update();

                int keyNo = 0;
                const int framesPerKey = 4;

                const float glowTime = 2;


                PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);

                float brushModeTimer = 0;
                keyboard.Updating += (sender, eventArgs) =>
                {
                    brushModeTimer += eventArgs.DeltaTime;

                    foreach (CorsairKey k in keyboard.Keys)
                    {
                        k.Led.Color = Color.WhiteSmoke;
                    }

                    if (keyboard['Q'].Led.Color == Color.Red)
                        keyboard['Q'].Led.Color = Color.Black;
                    else
                        keyboard['Q'].Led.Color = Color.Red;

                    if (keyboard['E'].Led.Color == Color.Red)
                        keyboard['E'].Led.Color = Color.Black;
                    else if (keyboard['E'].Led.Color == Color.Black)
                        keyboard['E'].Led.Color = Color.Blue;
                    else
                        keyboard['E'].Led.Color = Color.Red;

                    keyboard['G'].Led.Color = Color.Green;

                    CorsairKeyboardKeyId glowKey = CorsairKeyboardKeyId.KeypadMinus;
                    int r = (int)(512 * (brushModeTimer / glowTime)) % 512;
                    if (r > 255) r = (512 - r);
                    keyboard[glowKey].Led.Color = Color.FromArgb(r, 0, 0);

                    CorsairKey k2 = null;
                    while (k2 == null)
                    {
                        if (keyNo / framesPerKey > 154) keyNo = 0;
                        int k2Val = ++keyNo / framesPerKey;
                        k2 = keyboard[(CorsairKeyboardKeyId)k2Val];
                    }
                    k2.Led.Color = Color.Blue;

                    if(keyNo%40==0)
                    {
                        var TotalPhysicalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
                        var TotalVirtualMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalVirtualMemory;
                        var AvailablePhysicalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
                        var AvailableVirtualMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().AvailableVirtualMemory;

                        Console.WriteLine("CPU% = " + cpuCounter.NextValue() + "%");
                        Console.WriteLine("RAM Available = " + ramCounter.NextValue() + "MB");
                        Console.WriteLine("TotalPhysicalMemory = " + TotalPhysicalMemory);
                        Console.WriteLine("TotalVirtualMemory = " + TotalVirtualMemory);
                        Console.WriteLine("AvailablePhysicalMemory = " + AvailablePhysicalMemory);
                        Console.WriteLine("AvailableVirtualMemory = " + AvailableVirtualMemory);
                    }
                };
            }
            catch (CUEException ex)
            {
                Console.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), ex.Error));
            }
            catch (WrapperException ex)
            {
                Console.WriteLine("Wrapper Exception! Message:" + ex.Message);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("Null Exception! Message:" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception! Message:" + ex.Message);
            }

            while (true)
                Thread.Sleep(1000); // Don't exit after exception
        }
    }
}
