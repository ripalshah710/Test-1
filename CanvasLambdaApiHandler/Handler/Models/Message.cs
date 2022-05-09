using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queue.Models
{
    public class Message
    {
        public string Topic { get; set; }
        public bool isTopicCreated { get; set; }
        public string Msg { get; set; }
        public string Aud { get; set; }
        public string CustomerID { get; set; }
    }
}