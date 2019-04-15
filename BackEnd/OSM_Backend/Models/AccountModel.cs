using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OSM_Backend.Models
{
    public class AccountModel
    {
        public static DataTable GetAll()
        {
            DataTable vR;
            String SQL = "SELECT * FROM accounts ORDER BY username";
            SqlCommand cmd = new SqlCommand(SQL);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
    }
}