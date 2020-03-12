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
    class Dupe : TogglePatch
    {
        public override string Title => "Auto Dupe";

        public override string Description => "Auto Dupe";

        CraftConfig Config { get; set; }

        Task Job { get; set; }
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
            Job = Task.Run(() => RunDupe(writer, token), token);
            return true;
        }
        void RunDupe(IGameWriter writer, CancellationToken token)
        {
            var Config = JsonConvert.DeserializeObject<CraftConfig>(File.ReadAllText("config.json"));
            Inventory.Delay = Config.DupeDelay;
            Inventory.MiniDelay = Config.DupeMiniDelay;
            for (var y=0;y<Config.DupeLines;y++)
            {
                //Move first stack

                for(var x = 0;x<10;x++)
                {
                    if (token.IsCancellationRequested) return;
                    Inventory.MoveFromInvToTrade(writer, 0, y, x, y);
                    Thread.Sleep(Config.DupeDelay);                    
                    Inventory.MoveStackElementFromInvToInv(writer, 1, y, 0, y);
                    Thread.Sleep(Config.DupeMiniDelay);
                    Inventory.MoveFromTradeToInv(writer, x, y,0,y);
                    Thread.Sleep(Config.DupeDelay);
                }       
            }
        }

        public override void Init(IGameWriter writer)
        {

        }
    }
}
