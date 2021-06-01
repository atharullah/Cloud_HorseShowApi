using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace HorseShowAPI.Controllers
{
    public class ChampController : ApiController
    {
        [HttpGet]
        [ActionName("GetChampionship")]
        public HttpResponseMessage GetChampionship(int flag, string ipadUDID, int showId)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        var javascriptSerialise = new JavaScriptSerializer();
                        if (flag == 0)
                        {
                            var Championships = (from sc in context.ShowChampionships
                                                 join s in context.Shows on sc.ShowId equals s.ShowId
                                                 join cm in context.ChampionshipMethods on s.ChampionshipMethodId equals cm.ChampionshipMethodId
                                                 join c in context.Championships on sc.ChampionshipId equals c.ChampionshipId
                                                 join sch in context.ShowChampionshipJudges on sc.ShowChampionshipId equals sch.ShowChampionshipId
                                                 join sj in context.ShowJudges on sch.ShowJudgeId equals sj.ShowJudgesId
                                                 where s.IsActive == true && sc.ShowId == showId
                                                 && sj.IpadUdid == ipadUDID
                                                 select new
                                                 {
                                                     s.ShowName,
                                                     s.StartDate,
                                                     s.EndDate,
                                                     s.ChampionshipMethodId,
                                                     cm.ChampionshipMethodName,
                                                     c.ChampionshipId,
                                                     c.ChampionshipName,
                                                     s.ShowId,
                                                     sc.IsActive,
                                                 }
                                         ).Distinct().AsEnumerable();
                            var temp = Championships.Select(x => new
                            {
                                StartDate = x.StartDate.ToString(),
                                EndDate = x.EndDate.ToString(),
                                ShowName = x.ShowName,
                                ChampionshipMethodId = x.ChampionshipMethodId,
                                ChampionshipMethodName = x.ChampionshipMethodName,
                                ChampionshipId = x.ChampionshipId,
                                ChampionshipName = x.ChampionshipName,
                                ShowId = x.ShowId,
                                IsActive = x.IsActive,
                            });
                            string json = JsonConvert.SerializeObject(temp);
                            var resp = new HttpResponseMessage()
                            {
                                Content = new StringContent(json, System.Text.Encoding.UTF8)
                            };
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            return resp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HorseShowAPI.Controllers.IpadController.LogClass.WriteLogsToFile(ex);
                throw;
            }
            return null;
        }
        [HttpGet]
        [ActionName("GetWinnerType")]
        public HttpResponseMessage GetWinnerType(int flag, int championshipid, int methodid, int showId)
        {

            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        var javascriptSerialise = new JavaScriptSerializer();
                        if (flag == 1)
                        {
                            var Winnertypes = (from cw in context.ChampionshipWinnertypes
                                               //join s in context.Shows on sc.ShowId equals s.ShowId
                                               join w in context.WinnerTypes on cw.WinnertypeId equals w.WinnerTypeId
                                               join sc in context.ShowChampionships on cw.ChampionshipId equals sc.ChampionshipId
                                               //join sch in context.ShowChampionshipJudges on sc.ShowChampionshipId equals sch.ShowChampionshipId
                                               //join sj in context.ShowJudges on sch.ShowJudgeId equals sj.ShowJudgesId
                                               where cw.ChampionshipId == championshipid && sc.ShowId == showId
                                               //&& sj.IpadUdid == ipadUDID
                                               select new
                                               {
                                                   w.WinnerTypeId,
                                                   w.WinnerTypeName,
                                                   cw.IsActive,
                                               }
                                            ).Distinct().AsEnumerable();
                            var temp = Winnertypes.Select(x => new
                            {
                                WinnerTypeId = x.WinnerTypeId,
                                WinnerTypeName = x.WinnerTypeName,
                                IsActive = x.IsActive,

                            });

                            string json = JsonConvert.SerializeObject(temp);
                            var resp = new HttpResponseMessage()
                            {
                                Content = new StringContent(json, System.Text.Encoding.UTF8)
                            };
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            return resp;
                        }

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return null;
        }

        [HttpGet]
        [ActionName("GetChampHorseList")]
        public HttpResponseMessage GetHorseList(int championshipid, int showid, int winnertypeid, int methodid)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {

                        if (methodid == 1)//This is for scheme C
                        {
                            List<Catalog> schemeonehorses = context.Catalogs.Where(x => x.ChamionshipId == championshipid && x.ShowId == showid).ToList<Catalog>();
                            var temp = schemeonehorses.Select(x => new
                            {
                                HorseCatalog = x.CatalogNumber,
                                //RatingDone = x.RatingDone,
                                // ShowHorseActive = x.ShowHorseActive
                            }).OrderByDescending(z => z.HorseCatalog).Reverse();
                            //temp = temp.OrderByDescending(a => a.ShowHorseActive);
                            string json = JsonConvert.SerializeObject(temp);
                            var resp = new HttpResponseMessage()
                            {
                                Content = new StringContent(json, System.Text.Encoding.UTF8)
                            };
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            return resp;
                        }
                        else if (methodid == 2) // this is for scheme B
                        {
                            if (winnertypeid == 1)
                            {
                                List<Catalog> schemeonehorses = context.Catalogs.Where(x => x.ChamionshipId == championshipid && x.ShowId == showid && x.Rank == 1 && x.WinnerTypeId == null || x.WinnerTypeId == 0).ToList<Catalog>();
                                var temp = schemeonehorses.Select(x => new
                                {
                                    HorseCatalog = x.CatalogNumber,
                                    //RatingDone = x.RatingDone,
                                    // ShowHorseActive = x.ShowHorseActive
                                }).OrderByDescending(z => z.HorseCatalog).Reverse();


                                string json = JsonConvert.SerializeObject(temp);
                                var resp = new HttpResponseMessage()
                                {
                                    Content = new StringContent(json, System.Text.Encoding.UTF8)
                                };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                return resp;
                            }
                            if (winnertypeid == 2)
                            {
                                List<Catalog> schemeonehorses = context.Catalogs.Where(x => x.ChamionshipId == championshipid && x.ShowId == showid && x.Rank == 1 && x.WinnerTypeId == null || x.WinnerTypeId == 0
                                    || x.Rank == 2 && x.WinnerTypeId == null || x.WinnerTypeId == 0

                                    ).ToList<Catalog>();
                                var temp = schemeonehorses.Select(x => new
                                {
                                    HorseCatalog = x.CatalogNumber,
                                    //RatingDone = x.RatingDone,
                                    // ShowHorseActive = x.ShowHorseActive
                                }).OrderByDescending(z => z.HorseCatalog).Reverse();


                                string json = JsonConvert.SerializeObject(temp);
                                var resp = new HttpResponseMessage()
                                {
                                    Content = new StringContent(json, System.Text.Encoding.UTF8)
                                };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                return resp;
                            }
                            if (winnertypeid == 3)
                            {
                                List<Catalog> schemeonehorses = context.Catalogs.Where(x => x.ChamionshipId == championshipid && x.ShowId == showid && x.Rank == 1 && x.WinnerTypeId == null || x.WinnerTypeId == 0
                                    || x.Rank == 2 && x.WinnerTypeId == null || x.WinnerTypeId == 0
                                    || x.Rank == 3 && x.WinnerTypeId == null || x.WinnerTypeId == 0

                                    ).ToList<Catalog>();
                                var temp = schemeonehorses.Select(x => new
                                {
                                    HorseCatalog = x.CatalogNumber,
                                    //RatingDone = x.RatingDone,
                                    // ShowHorseActive = x.ShowHorseActive
                                }).OrderByDescending(z => z.HorseCatalog).Reverse();


                                string json = JsonConvert.SerializeObject(temp);
                                var resp = new HttpResponseMessage()
                                {
                                    Content = new StringContent(json, System.Text.Encoding.UTF8)
                                };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                return resp;
                            }
                            if (winnertypeid == 4)
                            {
                                List<Catalog> schemeonehorses = context.Catalogs.Where(x => x.ChamionshipId == championshipid && x.ShowId == showid && x.Rank == 1 && x.WinnerTypeId == null || x.WinnerTypeId == 0
                                    || x.Rank == 2 && x.WinnerTypeId == null || x.WinnerTypeId == 0
                                    || x.Rank == 3 && x.WinnerTypeId == null || x.WinnerTypeId == 0
                                    || x.Rank == 4 && x.WinnerTypeId == null || x.WinnerTypeId == 0

                                    ).ToList<Catalog>();
                                var temp = schemeonehorses.Select(x => new
                                {
                                    HorseCatalog = x.CatalogNumber,
                                    //RatingDone = x.RatingDone,
                                    // ShowHorseActive = x.ShowHorseActive
                                }).OrderByDescending(z => z.HorseCatalog).Reverse();


                                string json = JsonConvert.SerializeObject(temp);
                                var resp = new HttpResponseMessage()
                                {
                                    Content = new StringContent(json, System.Text.Encoding.UTF8)
                                };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                return resp;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                HorseShowAPI.Controllers.IpadController.LogClass.WriteLogsToFile(ex);
                throw;
            }

        }
        [HttpGet]
        [ActionName("ChampRating")]
        public int ChampRating(string champrating, int methodid)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        ChampionshipRating champRating = new ChampionshipRating();
                        if (methodid == 2)
                        {
                            champrating = champrating.Replace('[', ' ').Replace(']', ' ').Trim();
                            TempChampionshipRating ratingdata = new TempChampionshipRating();
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            ratingdata = (TempChampionshipRating)serializer.Deserialize(champrating, typeof(TempChampionshipRating));
                            champRating.ChampionshipId = ratingdata.ChampionshipId;
                            champRating.ChampionShipJudgeId = ratingdata.ChampionShipJudgeId;
                            champRating.HorseCatalog = ratingdata.HorseCatalog;
                            champRating.IpadUdid = ratingdata.IpadUdid;
                            champRating.Showid = ratingdata.Showid;
                            champRating.WinnerTypeId = ratingdata.WinnerTypeId;
                            context.ChampionshipRatings.Add(champRating);
                            int result = context.SaveChanges();
                            int horseCatalog = Convert.ToInt32(ratingdata.HorseCatalog);
                            Catalog insertCatalog = context.Catalogs.Where(z => z.CatalogNumber == horseCatalog && z.ShowId == ratingdata.Showid && z.ChamionshipId == ratingdata.ChampionshipId).Single();
                            insertCatalog.WinnerTypeId = ratingdata.WinnerTypeId;
                            context.SaveChanges();
                        }
                        else if (methodid == 1)
                        {
                            List<TempChampionshipRating> ratingdata = new List<TempChampionshipRating>();
                            ratingdata = JsonConvert.DeserializeObject<List<TempChampionshipRating>>(champrating);
                            TempChampionshipRating ratingtoInsert = new TempChampionshipRating();
                            foreach (var item in ratingdata)
                            {

                                champRating.ChampionshipId = item.ChampionshipId;
                                champRating.ChampionShipJudgeId = item.ChampionShipJudgeId;
                                champRating.HorseCatalog = item.HorseCatalog;
                                champRating.IpadUdid = item.IpadUdid;
                                champRating.Showid = item.Showid;
                                champRating.WinnerTypeId = item.WinnerTypeId;
                                context.ChampionshipRatings.Add(champRating);
                                int result = context.SaveChanges();
                                int horseCatalog = Convert.ToInt32(item.HorseCatalog);
                                Catalog insertCatalog = context.Catalogs.Where(z => z.CatalogNumber == horseCatalog && z.ShowId == item.Showid && z.ChamionshipId == item.ChampionshipId).Single();
                                insertCatalog.WinnerTypeId = item.WinnerTypeId;
                                context.SaveChanges();
                            }

                        }

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return 0;

        }

        [HttpGet]
        [ActionName("GetJudgeCode")]
        public HttpResponseMessage GetJudgeCode(string IpadUdid, int showId)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        var res = new HttpResponseMessage()
                        {
                            Content = new StringContent("Not Present", Encoding.UTF8)
                        };
                        res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        //int showid = context.Shows.Where(x => x.IsActive == true).Single().ShowId;
                        int showjudgesid = context.ShowJudges.Where(z => z.IpadUdid == IpadUdid && z.ShowId == showId).Single().ShowJudgesId;
                        // int i=context.ShowClassJudges.Where(q => q.ShowJudgesId == showjudgesid && q.ShowId == showid).Single().ShowClassJudgesId;
                        var query = (from a in context.ShowChampionshipJudges
                                     where (a.ShowJudgeId != null && a.ShowJudgeId == showjudgesid) && (a.ShowId != null && a.ShowId == showId)
                                     select a).FirstOrDefault();
                        if (query != null)
                        {

                            var JudgeCodeCollection = context.ShowJudges.Where(z => z.IpadUdid == IpadUdid && z.ShowId == showId).Select(a => new { JudgeCode = a.JudgeCode, JudgeName = a.JudgeName });

                            string JudgeCode = JsonConvert.SerializeObject(JudgeCodeCollection);

                            if (!string.IsNullOrEmpty(JudgeCode))
                            {
                                var resp = new HttpResponseMessage()
                                {
                                    Content = new StringContent(JudgeCode, System.Text.Encoding.UTF8)
                                };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                return resp;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        return res;
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                HorseShowAPI.Controllers.IpadController.LogClass.WriteLogsToFile(ex);
                throw;
            }
        }


        public bool isValidIP()
        {
            try
            {
                bool isApplyIPSecurity = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["IsApplyIPSecurity"]));
                bool result = false;

                if (isApplyIPSecurity)
                {
                    string[] IPList = ConfigurationManager.AppSettings["IPList"].Split(',');

                    string clientIPAddress = HttpContext.Current.Request.UserHostAddress;

                    if (IPList.Contains(clientIPAddress))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
                return result;
            }
            catch
            {
                throw;
            }
        }
        public class TempChampionshipRating
        {
            public int ChampionshipRatingId { get; set; }
            public Nullable<int> WinnerTypeId { get; set; }
            public Nullable<int> ChampionShipJudgeId { get; set; }
            public Nullable<int> HorseCatalog { get; set; }
            public string IpadUdid { get; set; }
            public Nullable<int> Showid { get; set; }
            public Nullable<int> ChampionshipId { get; set; }

        }
    }
}
    
