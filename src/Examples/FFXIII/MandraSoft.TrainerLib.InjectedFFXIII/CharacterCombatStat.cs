using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.InjectedFFXIII
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct BattleChara
    {
        public int field_0;
        public fixed byte gap4[360];
        public int field_364;
        public fixed byte gap170[16];
        public int CharacterIndex;
        public fixed byte gap184[104];
        public int field_492;
        public fixed byte gap1F0[4];
        public int field_500;
        public int field_504;
        public int field_508;
        public int field_512;
        public int field_516;
        public int field_520;
        public int field_524;
        public int field_528;
        public int field_532;
        public int field_536;
        public int field_540;
        public int field_544;
        public fixed byte gap224[24];
        public int field_572;
        public fixed byte gap240[14];
        public char field_590;
        public fixed byte gap24F[5];
        public char field_596;
        public fixed byte gap255[11];
        public char field_608;
        public fixed byte gap261[2];
        public char field_611;
        public fixed byte gap264[44];
        public int Health;
        public fixed byte gap294[8];
        public int MaxHealth;
        public fixed byte gap2A0[36];
        public int BattleCharaParam;
        public fixed byte gap2C8[5992];
        public int field_6704;
        public fixed byte gap1A34[4];
        public int field_6712;
        public fixed byte gap1A3C[124];
        public int field_6840;
        public fixed byte gap1ABC[404];
        public int field_7248;
        public fixed byte gap1C54[12];
        public int field_7264;
        public fixed byte gap1C64[1172];
        public int field_8440;
    };

}
