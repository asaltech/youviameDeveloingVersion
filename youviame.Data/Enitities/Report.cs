using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youviame.Data.Enitities
{
    public class Report : BaseEntity
    {
        public Guid ReporterId { get; set; }
        public Guid ReportedUser { get; set; }
        public string ReportMessage { get; set; }
        public ReportReason ReportReason { get; set; }
         public Report() {
            
        }
    }


    public enum ReportReason
    {
        InappropriateMessages ,
        InappropiatePictures ,
        InappropriateBehaviour ,
        Other 

    }
}
