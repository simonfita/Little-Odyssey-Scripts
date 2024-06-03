using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Used only in MWC and JSON Exporter
namespace MerchantWorldCreator
{
    public struct Town
    { 
        public List<NPCData> NPCs;
    }

    public struct NPCData
    {
        public string name;
        public Shared.ERaces race;
        public Shared.ESchedules schedule;
        
    }


}
