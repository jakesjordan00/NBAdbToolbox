using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBAdbToolbox
{
    class DbConfig
    {
        public string Server { get; set; }

        public string Alias { get; set; }
        public bool ?Create { get; set; }
        public bool? UseWindowsAuth { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
