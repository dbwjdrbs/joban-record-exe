using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JobanRecordApp.Models.Member.Entity
{
    internal class Member
    {
        public long memberId { get; set; }
        public string name { get; set; }
    }
}
