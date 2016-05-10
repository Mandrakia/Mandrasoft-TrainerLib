using Mandrasoft.TrainerLib.UI;
using Mandrasoft.TrainerLib.UI.Models;
using System;

using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using static Mandrasoft.TrainerLib.ImportsWin32;

namespace Mandrasoft.TrainerLib
{
    public sealed class TrainerHost
    {
        static private TrainerModel _Trainer;
        static private IntPtr _hookID;
        public static void Run<T>() where T : ITrainer
        {
            System.Windows.Application app = new System.Windows.Application();
            var window = new TrainerWindow();
            _Trainer = new TrainerModel((T)Activator.CreateInstance(typeof(T)));
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task t = Task.Run(()=>ProcessChecker(token),token);
            window.DataContext = _Trainer;
            _hookID = SetHook(KeyboardHookCallback);
            app.Run(window);
            tokenSource.Cancel();
            try
            {
                t.Wait();
            }
            catch (AggregateException e)
            {             
            }
            finally
            {
                tokenSource.Dispose();
            }
            UnhookWindowsHookEx(_hookID);
        }
        private static void ProcessChecker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var process = Process.GetProcessesByName(_Trainer.Trainer.ExecutableName);
                if (process.Count() == 1)
                {
                    _Trainer.GameFound = true;
                    _Trainer.Writer = new GameWriter(process.First());
                }
                else
                {
                    _Trainer.GameFound = false;
                    _Trainer.Writer = null;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
           
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && _Trainer.GameFound)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                //find patch
                var patch = _Trainer.Patches.SingleOrDefault(x => x.Key == (Keys)vkCode);
                if (patch != null)
                {
                    if (patch.Toggleable && patch.Enabled)
                    {
                        var res = ((TogglePatch)patch.Patch).DisablePatch(_Trainer.Writer);
                        if (res)
                        {
                            Trace.WriteLine("Disabled");
                            patch.Enabled = false;
                        }
                    }
                    else
                    {
                        var res = patch.Patch.ApplyPatch(_Trainer.Writer);
                        if (res)
                        {
                            Trace.WriteLine("Enabled");
                            patch.Enabled = true;
                        }
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
