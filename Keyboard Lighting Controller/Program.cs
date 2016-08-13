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

            UnhookWindowsHookEx(_hookID);
            Console.WriteLine("unhooked");

        }

        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Console.WriteLine("Code="+ nCode);

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);

                //StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                //sw.Write((Keys)vkCode);
                //sw.Close();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
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
