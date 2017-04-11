using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCSql.Models
{
    public class dbconnection
    {
        public static string Configuration()
        {
            return @"data source = .\sqlexpress; integrated security = true; database = sqlinjection;";
        }
    }
}