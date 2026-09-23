using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anypratice
{
    public class settings
    {
        public bool on = true;
        public bool baldurfix = true;
        public bool beamlock=true;
        public bool abyssremove=true;
        public bool orbindicator=true;
        public bool carereset=true;
        public bool legacycost=true;
        public bool skin=true;
        public bool indicator=true;
        public int cycle=0;
       //public bool superdash=true;

        // ---- 辐光招式控制 ----
        public bool crOn = true;
        public ChoiceMode crMode = ChoiceMode.Random;
        public string crA1 = "脸刺";
        public string crA2 = "脸刺";
        public string[] crSlots = new string[AttackSequence.SlotCount];
        public bool crLoop = true;
        public bool crTeleRepeat = false;
        public int crTelePos = 0;
    }
}
