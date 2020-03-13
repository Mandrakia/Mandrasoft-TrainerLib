using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Mandrasoft.TrainerLib.Wolcen
{
    static class Inventory
    {
        static public int Delay = 300;
        static public int MiniDelay = 300;
        static internal void MoveFromTradeToInv(IGameWriter writer, int tX, int tY, int iX, int iY, bool isFocused = false)
        {
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y,isFocused);
            Thread.Sleep(Delay);
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y,isFocused);
        }
        static internal void MoveFromTradeToTrade(IGameWriter writer, int tX, int tY, int iX, int iY)
        {
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
            Thread.Sleep(Delay);
            tradePosition = GetPositionTrade(iX, iY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
        }
        static internal void MoveFromInvToTrade(IGameWriter writer, int iX, int iY, int tX, int tY, bool isFocused = false)
        {
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y,isFocused);
            Thread.Sleep(MiniDelay);
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y,isFocused);
        }
        static internal void MoveStackElementFromInvToTrade(IGameWriter writer, int iX, int iY, int tX, int tY)
        {
            writer.PressKey(System.Windows.Forms.Keys.LShiftKey);
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y);
            writer.ReleaseKey(System.Windows.Forms.Keys.LShiftKey);
            Thread.Sleep(MiniDelay);
            var tradePosition = GetPositionTrade(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y);
        }
        static internal void MoveStackElementFromInvToInv(IGameWriter writer, int iX, int iY, int tX, int tY, bool isFocused  = false)
        {
            writer.PressKey(System.Windows.Forms.Keys.LShiftKey);
            Thread.Sleep(40);
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y,isFocused);
            Thread.Sleep(40 );
            writer.ReleaseKey(System.Windows.Forms.Keys.LShiftKey);
            Thread.Sleep(MiniDelay);
            var tradePosition = GetPositionInventory(tX, tY);
            writer.Click((int)tradePosition.X, (int)tradePosition.Y,isFocused);
        }
        static internal void RightClickOnInv(IGameWriter writer, int iX, int iY)
        {
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.RightClick((int)inventoryPosition.X, (int)inventoryPosition.Y);
        }
        static internal void ClickOnInv(IGameWriter writer, int iX, int iY)
        {
            var inventoryPosition = GetPositionInventory(iX, iY);
            writer.Click((int)inventoryPosition.X, (int)inventoryPosition.Y);
        }
        static internal void RightClickOnTrade(IGameWriter writer, int iX, int iY)
        {
            var inventoryPosition = GetPositionTrade(iX, iY);
            writer.RightClick((int)inventoryPosition.X, (int)inventoryPosition.Y);
        }
        static internal Point GetPositionInventory(int x, int y)
        {
            var resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            int iX = 0, iY = 0, deltaIX = 0, deltaIY = 0;
            if (resolution.Width == 1920)
            {
                iX = 1285;
                iY = 630;
                deltaIX = 65;
                deltaIY = deltaIX;
            }
            else if(resolution.Width == 3840)
            {
                iX = 2569;
                iY = 1260;
                deltaIX = 125;
                deltaIY = deltaIX;
            }


            return new Point(iX + x * deltaIX, iY + y * deltaIY);
        }
        static internal Point GetPositionTrade(int x, int y)
        {
            int sX = 94;
            int sY = 556;
            int deltaSX = 50;
            int deltaSY = deltaSX;
            var resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (resolution.Width == 3840)
            {
                sX = 187;
                sY = 1114;
                deltaSX = 101;
                deltaSY = deltaSX;
            }
            return new Point(sX + x * deltaSX, sY + y * deltaSY);
        }
    }
}
