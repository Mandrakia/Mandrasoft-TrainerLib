using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Mandrasoft.TrainerLib.Wolcen
{
    class Dupe2 : TogglePatch
    {
        public override string Title => "Auto Dupe";

        public override string Description => "Auto Dupe";

        CraftConfig Config { get; set; }

        Task Job { get; set; }
        CancellationTokenSource TokenSource { get; set; }

        private int Delay = 300;
        private int MiniDelay = 300;
        public override bool DisablePatch(IGameWriter writer)
        {
            TokenSource.Cancel();
            return true;
        }

        public override bool ApplyPatch(IGameWriter writer)
        {
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            Job = Task.Run(() => RunDupe(writer, token), token);
            return true;
        }
        void RunDupe(IGameWriter writer, CancellationToken token)
        {
            var Config = JsonConvert.DeserializeObject<CraftConfig>(File.ReadAllText("config.json"));
            Delay = Config.Delay;
            MiniDelay = Config.Delay;
            for (var y=0;y<6;y++)
            {
                //Move first stack

                for(var x = 0;x<10;x++)
                {
                    if (token.IsCancellationRequested) return;
                    MoveFromInvToTrade(writer, 0, y, x, y);
                    Thread.Sleep(Delay);                    
                    MoveStackElementFromInvToInv(writer, 1, y, 0, y);
                    Thread.Sleep(MiniDelay);
                    MoveFromTradeToInv(writer, x, y,0,y);
                    Thread.Sleep(Delay);
                }       
            }
        }
        void MoveFromTradeToInv(IGameWriter writer, int tX,int tY,int iX,int iY)
        {
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X,(int)tradePosition.Y);
            Thread.Sleep(MiniDelay);
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y);
        }
        void MoveFromTradeToTrade(IGameWriter writer, int tX, int tY, int iX, int iY)
        {
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
            Thread.Sleep(MiniDelay);
            tradePosition = GetPositionTrade(iX, iY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
        }
        void MoveFromInvToTrade(IGameWriter writer, int iX, int iY, int tX, int tY)
        {
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y);
            Thread.Sleep(MiniDelay);
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
        }
        void MoveStackElementFromInvToTrade(IGameWriter writer, int iX, int iY, int tX, int tY)
        {
            writer.PressKey(System.Windows.Forms.Keys.LShiftKey);
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y);
            writer.ReleaseKey(System.Windows.Forms.Keys.LShiftKey);
            Thread.Sleep(Delay);
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
        }
        void MoveStackElementFromInvToInv(IGameWriter writer, int iX, int iY, int tX, int tY)
        {
            writer.PressKey(System.Windows.Forms.Keys.LShiftKey);
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y);
            writer.ReleaseKey(System.Windows.Forms.Keys.LShiftKey);
            Thread.Sleep(MiniDelay);
            var tradePosition = GetPositionInventory(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
        }
        void RightClickOnInv(IGameWriter writer, int iX,int iY)
        {
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.RightClick((int)inventoryPosition.X, (int)inventoryPosition.Y);
        }
        void RightClickOnTrade(IGameWriter writer, int iX, int iY)
        {
            var inventoryPosition = GetPositionTrade(iX, iY);
            writer.RightClick((int)inventoryPosition.X, (int)inventoryPosition.Y);
        }
        Point GetPositionInventory(int x,int y)
        {
            int iX = 1285;
            int iY = 630;
            int deltaIX = 65;
            int deltaIY = deltaIX;


            return new Point(iX + x * deltaIX, iY + y * deltaIY);
        }
        Point GetPositionTrade(int x, int y)
        {
            int sX = 94;
            int sY = 556;
            int deltaSX = 50;
            int deltaSY = deltaSX;

            return new Point(sX + x * deltaSX, sY + y * deltaSY);
        }
        public override void Init(IGameWriter writer)
        {

        }
    }
}
