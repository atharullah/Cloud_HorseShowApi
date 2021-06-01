using HorseShowAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace HorseShowAPI.Controllers
{
    public class LoginController : ApiController
    {
        Login[] logins = new Login[] 
        { 
            new Login { UserName = "TomatoSoup", Password = "Groceries" }, 
            new Login { UserName = "Yo-yo", Password = "Toys" }, 
            new Login { UserName = "Hammer", Password = "Hardware"} 
        };



        public IEnumerable<Login> GetAllLogins()
        {
            return logins;
        }

        public IHttpActionResult GetLogin(string  username, string password)
        {
            var login = logins.FirstOrDefault((p) => p.UserName == username && p.Password == password);
            if (login == null)
            {
                return NotFound();
            }
            //return Json<Login>(login);
            return Ok(login);
        }

        public string GetHorseShowClasses(int flag)
        {
            try
            {
                using (QrecApiEntities context = new QrecApiEntities())
                {
                    var javascriptSerialise = new JavaScriptSerializer();
                    if (flag == 0)
                    {
                        var showclasses = (from sc in context.ShowClasses
                                           join s in context.Shows on sc.ShowId equals s.ShowId
                                           select new
                                           {
                                               s.ShowName,
                                               s.StartDate,
                                               s.EndDate,
                                               sc.ShowClassId,
                                               sc.ShowClassName,
                                               s.ShowId,
                                               sc.IsActive
                                           }
                                     ).AsEnumerable();
                        var temp = showclasses.Select(x => new
                        {
                            StartDate = x.StartDate.ToString(),
                            EndDate = x.EndDate.ToString(),
                            ShowName = x.ShowName,
                            ShowClassId = x.ShowClassId,
                            ShowClassName = x.ShowClassName,
                            ShowId = x.ShowId,
                            IsActive = x.IsActive
                        });
                        return javascriptSerialise.Serialize(temp);
                    }
                    if (flag == 1)
                    {
                        var showclasses = (from sc in context.ShowClasses
                                           join s in context.Shows on sc.ShowId equals s.ShowId
                                           where sc.IsActive == true
                                           select new
                                           {
                                               s.ShowName,
                                               s.StartDate,
                                               s.EndDate,
                                               sc.ShowClassId,
                                               sc.ShowClassName,
                                               s.ShowId
                                           }
                                     ).AsEnumerable();
                        var temp = showclasses.Select(x => new
                        {
                            StartDate = x.StartDate.ToString(),
                            EndDate = x.EndDate.ToString(),
                            ShowName = x.ShowName,
                            ShowClassId = x.ShowClassId,
                            ShowClassName = x.ShowClassName,
                            ShowId = x.ShowId
                        });
                        return javascriptSerialise.Serialize(temp);
                    }
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
