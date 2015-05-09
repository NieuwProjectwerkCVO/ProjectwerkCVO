using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CVOService
{
    public partial class Prototype : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            begin_time.Text
                = DateTime.Now.Hour.ToString() + ":" +
                  DateTime.Now.Minute.ToString();
            end_time.Text
                = DateTime.Now.Hour.ToString() + ":" +
                  DateTime.Now.Minute.ToString();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Regex.Match(begin_time.Text, @"[0-9]{2}:[0-9]{2}");
            Regex.Match(begin_date.Text, @"");
            Regex.Match(end_time.Text, @"[0-9]{2}:[0-9]{2}");
            Regex.Match(end_date.Text, @"");
        }


    }
}