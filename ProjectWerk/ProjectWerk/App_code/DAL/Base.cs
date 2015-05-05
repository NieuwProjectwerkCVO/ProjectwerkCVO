using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ProjectWerk.App_code.DAL
{
    public class Base
    {

        public string Feedback { get; set; }
        protected string ErrorMessage { get; set; }

        private static string connectionString = "Data Source=VINCENT-PC;Initial Catalog=Administratix_cursist;Persist Security Info=True;User ID=sa;Password=Popsy1234";
        //connectionstring voor server "Data Source=92.222.220.213,1501;Initial Catalog=Administratix_cursist;Persist Security Info=True;User ID=sa;Password=Boerderijm1n#s"
        protected SqlConnection connection;

        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public Base()
        {
            this.connection = new SqlConnection(ConnectionString);
        }

        public Base(string connectionString)
            : this()
        {
            ConnectionString = connectionString;
        }
    }
}