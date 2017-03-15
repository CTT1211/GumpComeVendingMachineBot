using LUISLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BoBot
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
             MakeRequest();
        }

        public static async void MakeRequest()
        {
            LUISResult luis = await LUISHelper.MakeRequest("状态");

            string meg = VendingLuisHelper.GetAnswer(luis.LUISIntent.intent, luis);
            return;
        }

    }
}