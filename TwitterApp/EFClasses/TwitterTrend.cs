using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.EFClasses
{
    public class TwitterTrend
    {
        public TwitterTrend()
        {

        }

        [Key]
        public string TrendName { get; set; }

    }
}
