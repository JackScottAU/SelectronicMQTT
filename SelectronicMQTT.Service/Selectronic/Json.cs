using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectronicMQTT.Service.Selectronic
{
    public class SelectJsonResponse
    {
        public int item_count;

        public int now;

        public DeviceName? device;

        public Items? items;
    }

    public class DeviceName
    {
        public string? name;
    }

    public class Items
    {
        public double battery_in_wh_today { get; set; }

        public double battery_in_wh_total { get; set; }

        public double battery_out_wh_today { get; set; }
        
        public double battery_out_wh_total { get; set; }
        
        public double battery_soc { get; set; }

        public double battery_w { get; set; }

        public double fault_code { get; set; }

        public double fault_ts { get; set; }

        public double gen_status { get; set; }

        public double grid_in_wh_today { get; set; }

        public double grid_in_wh_total { get; set; }

        public double grid_out_wh_today { get; set; }

        public double grid_out_wh_total { get; set; }

        public double grid_w { get; set; }

        public double load_w { get; set; }

        public double load_wh_today { get; set; }

        public double load_wh_total { get; set; }

        public double shunt_w { get; set; }

        public double solar_wh_today { get; set; }

        public double solar_wh_total {get;set;} 

        public double solarinverter_w { get; set; }

        public double timestamp {get;set;}
    }
}
