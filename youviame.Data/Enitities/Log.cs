using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youviame.Data.Enitities
{
    public class Log : BaseEntity
    {
        public DateTime LogTime { get; set; }

        public string LogMessage { get; set; }
      
         public Log() {
            
        }
    }

}
