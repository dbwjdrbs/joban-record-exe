using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JobanRecordApp.Models.Members.Entity
{
    internal class Member
    {
        public long memberId { get; set; }
        public string name { get; set; }

        public Member(long memberId, string name)
        {
            this.memberId = memberId;
            this.name = name;
        }

        public class Authentication
        {
            public string accessToken { get; set; }

            public Authentication(string accessToken)
            {
                this.accessToken = accessToken;
            }
        }
    }
}
