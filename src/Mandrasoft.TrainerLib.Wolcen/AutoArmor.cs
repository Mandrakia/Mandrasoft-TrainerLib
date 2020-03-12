using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Mandrasoft.TrainerLib.Wolcen
{
    class AutoArmorCraft : TogglePatch
    {
        public override string Title => "Auto armor";

        public override string Description => "Auto armor";

        private int startX = 0;
        private int startY = 0;
        private int stackSize = 20;

        CraftConfig Config { get; set; }

        Task Job { get; set;}
        CancellationTokenSource TokenSource { get; set; }

        public override bool DisablePatch(IGameWriter writer)
        {
            TokenSource.Cancel();
            return true;
        }

        public override bool ApplyPatch(IGameWriter writer)
        {
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            Job =  Task.Run(() => RunCraft(writer,token),token);
           return true;
        }
        
        void RunCraft(IGameWriter writer,CancellationToken token)
        {
            Config = JsonConvert.DeserializeObject<CraftConfig>(File.ReadAllText("config.json"));
            int sX = 1285;
            int sY = 630;
            int stackSize = 20;
            int deltaX = 65;
            int deltaY = deltaX;

            for(var x = startX; x < 10;x++)
            {
                for(var y = startY; y < 4;y++)
                {
                    stackSize = this.stackSize;
                    while(stackSize > 0)
                    {
                        if (token.IsCancellationRequested)
                        {
                            this.startX = x;
                            this.startY = y;
                            this.stackSize = stackSize;
                            return;
                        }
                        RightClick(sX + x * deltaX, sY + y * deltaY);
                        System.Threading.Thread.Sleep(60);
                        Click(1850, 880);
                        stackSize--;
                        System.Threading.Thread.Sleep(60);

                        var bytes = writer.Read(new IntPtr(Convert.ToInt64(Config.AddressResist,16)),4);
                        var resist = int.Parse(ASCIIEncoding.ASCII.GetString(bytes));

                        if (resist > Config.MinResist)
                        {
                            this.startX = x;
                            this.startY = y;
                            this.stackSize = stackSize;
                            return;
                        }
                    }
                    this.stackSize = 20;
                }
                this.startY = 0;
            }
            this.startX = 0;
        }
        public override void Init(IGameWriter writer)
        {

        }

        void Click(int x,int y)
        {
            SetCursorPosition(x, y);
            System.Threading.Thread.Sleep(120);
            MouseEvent(MouseEventFlags.LeftDown);
            System.Threading.Thread.Sleep(60);
            MouseEvent(MouseEventFlags.LeftUp);
        }
        void RightClick(int x, int y)
        {
            SetCursorPosition(x, y);
            System.Threading.Thread.Sleep(120);
            MouseEvent(MouseEventFlags.RightDown);
            System.Threading.Thread.Sleep(60);
            MouseEvent(MouseEventFlags.RightUp);
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }
        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
