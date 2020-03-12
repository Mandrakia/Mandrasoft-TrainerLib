using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mandrasoft.TrainerLib.Wolcen
{
    class CraftConfig
    {
        public int MinDamage { get; set; }
        public int MinDamageUbber { get; set; }
        public int MinResist { get; set; }
        public string AddressDamage { get; set; }
        public string AddressAffix { get; set; }
        public string AddressResist { get; set; }
        public int Delay { get; set; } = 350;
    }
    class AutoCraft : TogglePatch
    {
        public Stream VictorySound => Properties.Resources.finished_1;
        public Stream FailSound => Properties.Resources.fail_trombone_03;
        public override string Title => "Auto craft";

        public override string Description => "Auto craft";

        CraftConfig Config { get; set; }

        Task Job { get; set;}
        CancellationTokenSource TokenSource { get; set; }

        private int startX = 0;
        private int startY = 0;
        private int stackSize = 20;
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
        int previousDmg;
        void RunCraft(IGameWriter writer, CancellationToken token)
        {
            var Stats = JsonConvert.DeserializeObject<Dictionary<int, int>>(File.ReadAllText("stats.json"));
            Config = JsonConvert.DeserializeObject<CraftConfig>(File.ReadAllText("config.json"));
            int sX = 1285;
            int sY = 630;
            int stackSize = 20;
            int deltaX = 65;
            int deltaY = deltaX;

            for (var x = startX; x < 10; x++)
            {
                for (var y = startY; y < 6; y++)
                {
                    if (x == 9 && y == 4) break;
                    stackSize = this.stackSize;
                    while (stackSize > 0)
                    {
                        if (token.IsCancellationRequested)
                        {
                            this.startX = x;
                            this.startY = y;
                            this.stackSize = stackSize;
                            File.WriteAllText("stats.json", JsonConvert.SerializeObject(Stats));
                            SaveStats(Stats);
                            return;
                        }
                        writer.RightClick(sX + x * deltaX, sY + y * deltaY);
                        System.Threading.Thread.Sleep(100);
                        writer.Click(1850, 880);
                        stackSize--;
                        System.Threading.Thread.Sleep(120);

                        var bytes = writer.Read(new IntPtr(Convert.ToInt64(Config.AddressDamage, 16)), 12);
                        var txt = GetNullString(bytes);
                        var maxDmg = txt.Split('-')[1].Trim();
                        var dmg = int.Parse(maxDmg);

                        if (dmg != previousDmg)
                        {
                            if (Stats.ContainsKey(dmg)) Stats[dmg]++;
                            else Stats[dmg] = 1;
                        }
                        previousDmg = dmg;

                        if ( dmg >= Config.MinDamage)
                        {
                            this.startX = x;
                            this.startY = y;
                            this.stackSize = stackSize;
                            File.WriteAllText("stats.json", JsonConvert.SerializeObject(Stats));
                            SaveStats(Stats);

                            var playerv = new SoundPlayer(VictorySound);
                            playerv.Play();
                            return;
                        }
                    }
                    this.stackSize = 20;
                }
                this.startY = 0;
            }
            this.startX = 0;

            var player = new SoundPlayer(FailSound);
            player.Play();

            File.WriteAllText("stats.json", JsonConvert.SerializeObject(Stats));
            SaveStats(Stats);
        }

        private void SaveStats(Dictionary<int,int> stats)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Max damage;Frequency");
            foreach(var kv in stats)
            {
                sb.AppendLine(kv.Key.ToString() + ";" + kv.Value.ToString());
            }

            File.WriteAllText("stats.csv", sb.ToString());
        }
        private string GetNullString(byte[] buffer)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStringAnsi(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }
        public override void Init(IGameWriter writer)
        {

        }
    }
}
