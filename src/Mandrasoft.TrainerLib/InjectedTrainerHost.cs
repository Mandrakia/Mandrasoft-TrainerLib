using Mandrasoft.TrainerLib.UI;
using Mandrasoft.TrainerLib.UI.Models;
using System;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static Mandrasoft.TrainerLib.ImportsWin32;

namespace Mandrasoft.TrainerLib
{
    public unsafe sealed class InjectedTrainerHost
    {
        static private IntPtr stateAddr;
        static private TrainerStateStructure* _state;
        static private bool ModuleUnload = false;
        static private TrainerModel _Trainer;
        static private IntPtr _hookID;
        public static void Run<T>() where T : IInjectedTrainer
        {
            System.Windows.Application app = new System.Windows.Application();
            var window = new TrainerWindow();
            _Trainer = new TrainerModel((T)Activator.CreateInstance(typeof(T)));
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task t = Task.Run(() => ProcessChecker(token), token);
            window.DataContext = _Trainer;
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
            UnloadModule();
        }
        private static void ProcessChecker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var process = Process.GetProcessesByName(_Trainer.Trainer.ExecutableName);
                if (process.Count() == 1)
                {
                    if (_Trainer.GameFound == false)
                    {
                        _Trainer.GameFound = true;
                        _Trainer.Writer = new GameWriter(process.First());
                        LoadModule();
                    }
                    //Sync patches States.
                    else
                    {
                        if (stateAddr != IntPtr.Zero)
                        {
                            var b = _Trainer.Writer.Read(stateAddr, Marshal.SizeOf<TrainerStateStructure>());                            
                            for (var i = 0; i < _Trainer.Patches.Count; i++)
                            {
                                if (_Trainer.Patches[i].Patch is TogglePatch)
                                {
                                    if (b[i + 1] == 0x1)
                                        _Trainer.Patches[i].Enabled = true;
                                    else _Trainer.Patches[i].Enabled = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    _Trainer.GameFound = false;
                    _Trainer.Writer = null;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        private static void UnloadModule()
        {
            ModuleUnload = true;
            if (_Trainer.GameFound)
            {
                _Trainer.Writer.Write(stateAddr, new byte[] { 0x01 });
            }
        }
        private static void LoadModule()
        {
            var process = Process.GetProcessesByName(_Trainer.Trainer.ExecutableName)[0];
            var gameKernelModule = process.Modules.OfType<ProcessModule>().Where(x => x.ModuleName.ToLower().Contains("kernel32")).SingleOrDefault();
            //Get offset of LoadLibraryW function relative to Kernel32 base Adress.
            var addre = GetProcAddress(GetModuleHandle("kernel32"), "LoadLibraryW");
            var localKernelModule = Process.GetCurrentProcess().Modules.OfType<ProcessModule>().Where(x => x.ModuleName.ToLower().Contains("kernel32")).SingleOrDefault();
            int offset = addre.ToInt32() - localKernelModule.BaseAddress.ToInt32();
            //Allocate some memory in the game process and inject bootLoader
            var pHandle = OpenProcess(ProcessAccessFlags.All, false, process.Id);
            var f = new FileInfo("Mandrasoft.Bootloader.dll");
            var bytesLibPath = new byte[f.FullName.Length * 2 + 1];
            IntPtr lAddr = Marshal.StringToHGlobalUni(f.FullName);
            Marshal.Copy(lAddr, bytesLibPath, 0, bytesLibPath.Length);
            IntPtr baseAdress = VirtualAllocEx(pHandle, IntPtr.Zero, (IntPtr)bytesLibPath.Length, AllocationType.Commit | AllocationType.Reserve, AllocationProtect.PAGE_READWRITE);
            int written = 0;
            WriteProcessMemory(pHandle, baseAdress, bytesLibPath, bytesLibPath.Length, ref written);
            uint lThreadId;
            CreateRemoteThread(pHandle, IntPtr.Zero, 0, gameKernelModule.BaseAddress + offset, baseAdress, 0, out lThreadId);
            WaitForSingleObject((IntPtr)lThreadId, 0xFFFFFFFF);
            process.Refresh();
            //ReserveSpace for StateStructure  
            stateAddr = VirtualAllocEx(pHandle, IntPtr.Zero, (IntPtr)Marshal.SizeOf<TrainerStateStructure>(), AllocationType.Commit | AllocationType.Reserve, AllocationProtect.PAGE_EXECUTE_READWRITE);
        
            //Module loaded.
            //Now actually call the bootloader.
            string args = string.Empty;
            args +=_Trainer.Trainer.GetType().Assembly.Location + "|";
            args += _Trainer.Trainer.GetType().FullName + "|EntryPoint|" + stateAddr.ToInt32().ToString();
            var lArgsPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(args);
            var bytesStr = new byte[args.Length * 2 + 1];
            Marshal.Copy(lArgsPtr, bytesStr, 0, args.Length * 2 + 1);
            IntPtr strAddress = VirtualAllocEx(pHandle, IntPtr.Zero, (IntPtr)bytesStr.Length, AllocationType.Commit | AllocationType.Reserve, AllocationProtect.PAGE_EXECUTE_READWRITE);
            WriteProcessMemory(pHandle, strAddress, bytesStr, bytesStr.Length, ref written);
            while (!process.Modules.OfType<ProcessModule>().Where(x => x.FileName.ToLower().Contains("bootloader")).Any())
            {
                process.Refresh();
                System.Threading.Thread.Sleep(60);
            }
            IntPtr functionAddr = process.Modules.OfType<ProcessModule>().Where(x => x.FileName.ToLower().Contains("bootloader")).Single().BaseAddress;
            functionAddr += 0x19200;
            CreateRemoteThread(pHandle, IntPtr.Zero, 0, functionAddr, strAddress, 0, out lThreadId);
            WaitForSingleObject((IntPtr)lThreadId, 0xFFFFFFFF);
        }
        public static void SetHooks<T>(string stateAddre) where T : IInjectedTrainer
        {
            _state = (TrainerStateStructure*)int.Parse(stateAddre);
            Thread th = new Thread(InputHandler<T>);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        private static void InputHandler<T>() where T : IInjectedTrainer
        {
            var trainer = Activator.CreateInstance<T>();
            _Trainer = new TrainerModel(trainer);
            _Trainer.Writer = new InProcessGameWriter();
            while (!_state->ShouldStop)
            {
                foreach (var p in _Trainer.Patches)
                {
                    var st = Keyboard.GetKeyStates(KeyInterop.KeyFromVirtualKey((int)p.Key));
                    if (st == KeyStates.Down)
                    {
                        if (p.Toggleable && _state->PatchesState[_Trainer.Patches.IndexOf(p)])
                        {
                            if (((TogglePatch)p.Patch).DisablePatch(_Trainer.Writer)) _state->PatchesState[_Trainer.Patches.IndexOf(p)] = false;
                        }
                        else
                        {
                            p.Patch.ApplyPatch(_Trainer.Writer);
                            if(p.Toggleable)
                                _state->PatchesState[_Trainer.Patches.IndexOf(p)] = true;
                        }
                    }
                }
                Thread.Sleep(17);
            }
            //disable all patches
            for (var i = 0; i < _Trainer.Patches.Count; i++)
            {
                if (_Trainer.Patches[i].Toggleable && _state->PatchesState[i])
                {
                    ((TogglePatch)_Trainer.Patches[i].Patch).DisablePatch(_Trainer.Writer);
                }
            }
        }
    }
}
