using Mandrasoft.TrainerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandraSoft.TrainerLib.FFXIII
{
    class NoCostCrystallium : TogglePatch
    {
        public override string Description => "Upgrading will not cost CP anymore";

        public override string Title => "Infinite CP";

        private byte[] oldBytes;
        public override bool ApplyPatch(IGameWriter writer)
        {
            var res = writer.SearchMask(new MaskItem[] { 0x8b, 0x84, 0x24, 0x28, 0x01, 0x00, 0x00, 0x8b, 0x88, 0xc0, 0x44, 0x00, 0x00, "?", "?", "?", "?" });
            if (res.Success)
            {
                oldBytes = res.Matches.Single().Captures.ToArray();
                writer.ApplyPatch(new MaskItem[] { 0x8b, 0x84, 0x24, 0x28, 0x01, 0x00, 0x00, 0x8b, 0x88, 0xc0, 0x44, 0x00, 0x00, "?", "?", "?", "?" }, new byte[] { 0x90, 0x90, 0x90, 0x90 },res.Matches.Single());
                return true;
            }
            return false;
        }

        public override bool DisablePatch(IGameWriter writer)
        {
            var res = writer.SearchMask(new MaskItem[] { 0x8b, 0x84, 0x24, 0x28, 0x01, 0x00, 0x00, 0x8b, 0x88, 0xc0, 0x44, 0x00, 0x00, "?", "?", "?", "?" });
            if (res.Success)
            {
                writer.ApplyPatch(new MaskItem[] { 0x8b, 0x84, 0x24, 0x28, 0x01, 0x00, 0x00, 0x8b, 0x88, 0xc0, 0x44, 0x00, 0x00, "?", "?", "?", "?" }, oldBytes,res.Matches.Single());
                return true;
            }
            return false;
        }
    }
}
