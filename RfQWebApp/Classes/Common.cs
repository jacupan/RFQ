using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.DirectoryServices;

namespace RfQWebApp.Classes
{
    public class Common
    {
        public enum WebUserInformation
        {
            DomainName = 0,
            Username = 1
        }

        public static string GetWebCurrentUser(WebUserInformation webUserInformation)
        {
            string result = "";
            try
            {
                //string[] userInformation = HttpContext.Current.User.Identity.Name.ToString().Split(@"\".ToCharArray());
                string[] userInformation = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.ToString().Split(@"\".ToCharArray());
                result = userInformation[(int)webUserInformation].ToString();
            }
            catch
            {
                result = "N/A";
            }
            //CommonFunctions.Iif(webUserInformation != WebUserInformation.DomainName, userInformation[0], userInformation[1]);

            // Return the result
            return result.ToUpper();
            //return userSTR;
        }
       
    }
  
}