using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace HorseShowAPI.Controllers
{
    public class IpadController : ApiController
    {
        #region Web Service Endpoint
        [HttpGet]
        [ActionName("GetHorseShowClasses")]
        public HttpResponseMessage GetHorseShowClasses(int flag, string ipadUDID, int showId)
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
                            var showclasses = (from sc in context.ShowClasses
                                               join s in context.Shows on sc.ShowId equals s.ShowId
                                               join sch in context.ShowClassJudges on sc.ShowClassId equals sch.ShowClassId
                                               join sj in context.ShowJudges on sch.ShowJudgesId equals sj.ShowJudgesId
                                               where s.IsActive == true && sj.IpadUdid == ipadUDID && sc.ShowId == showId

                                               select new
                                               {
                                                   s.ShowName,
                                                   s.StartDate,
                                                   s.EndDate,
                                                   sc.ShowClassId,
                                                   sc.ShowClassName,
                                                   s.ShowId,
                                                   sc.IsActive,
                                                   s.IsPointFive,
                                                   sc.EachoClassName
                                               }
                                         ).Distinct().AsEnumerable();
                            var temp = showclasses.Select(x => new
                            {
                                StartDate = x.StartDate.ToString(),
                                EndDate = x.EndDate.ToString(),
                                ShowName = x.ShowName,
                                ShowClassId = x.ShowClassId,
                                ShowClassName = x.ShowClassName,
                                ShowId = x.ShowId,
                                IsActive = x.IsActive,
                                IsPointFive = x.IsPointFive,
                                EachoClassName = x.EachoClassName.Split(':')[1].Trim(),
                                ClassOrder = getNumberFromString(x.ShowClassName)
                            });

                            temp = temp.OrderBy(a => a.ClassOrder);
                            temp = temp.OrderByDescending(a => a.IsActive);

                            string json = JsonConvert.SerializeObject(temp);
                            var resp = new HttpResponseMessage()
                            {
                                Content = new StringContent(json, System.Text.Encoding.UTF8)
                            };
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            return resp;
                        }
                        if (flag == 1)
                        {
                            var showclasses = (from sc in context.ShowClasses
                                               join s in context.Shows on sc.ShowId equals s.ShowId
                                               join sch in context.ShowClassJudges on sc.ShowClassId equals sch.ShowClassId
                                               join sj in context.ShowJudges on sch.ShowJudgesId equals sj.ShowJudgesId
                                               where sc.IsActive == true && sj.IpadUdid == ipadUDID && sc.ShowId == showId
                                               && sc.ShowId == showId
                                               select new
                                               {
                                                   s.ShowName,
                                                   s.StartDate,
                                                   s.EndDate,
                                                   sc.ShowClassId,
                                                   sc.ShowClassName,
                                                   s.ShowId,
                                                   s.IsPointFive,
                                                   sc.EachoClassName,
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
                                IsPointFive = x.IsPointFive,
                                EachoClassName = x.EachoClassName.Split(':')[1].Trim(),
                                ClassOrder = getNumberFromString(x.ShowClassName)
                            });

                            temp = temp.OrderBy(a => a.ClassOrder);

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
                LogClass.WriteLogsToFile(ex);
                throw;
            }
            return null;
        }

        [HttpGet]
        [ActionName("GetHorseList")]
        public HttpResponseMessage GetHorseList(int showclassid, int showid, string ipadUdid)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        ShowClass sclass = context.ShowClasses.Where(z => z.ShowClassId == showclassid).Single();
                        string sClassName = sclass.ShowClassName;

                        List<Catalog> lstCatalog = context.Catalogs.Where(a => a.ShowId == showid && a.ClassName == sClassName && a.IsActive == true && a.IsApprove == true && a.IsCatalogDelete != true)
                                                              .ToList();
                        foreach (Catalog item in lstCatalog)
                        {
                            //var catalog = context.Catalogs.Where(a => a.ShowId == showid && a.CatalogNumber == hcatalogNumber).FirstOrDefault();
                            if (!string.IsNullOrEmpty(item.RatedJudgesIDList))
                            {
                                string[] ratedJudgeList = item.RatedJudgesIDList.Split('|');
                                if (ratedJudgeList.Contains(ipadUdid))
                                {
                                    item.RatingDone = true;
                                }
                                else
                                {
                                    item.RatingDone = false;
                                }
                            }
                        }

                        var temp = lstCatalog.Select(x => new
                        {
                            HorseCatalog = x.CatalogNumber,
                            RatingDone = x.RatingDone,
                            ShowHorseActive = x.ShowHorseActive
                        }).OrderByDescending(z => z.HorseCatalog).Reverse();
                        temp = temp.OrderByDescending(a => a.ShowHorseActive);

                        string json = JsonConvert.SerializeObject(temp);
                        var resp = new HttpResponseMessage()
                        {
                            Content = new StringContent(json, System.Text.Encoding.UTF8)
                        };
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return resp;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                LogClass.WriteLogsToFile(ex);
                throw;
            }
            #region oldcode
            //try
            //{
            //    using (CurrentHorseShowEntities context = new CurrentHorseShowEntities())
            //    {
            //        var showclassHorses = (from sch in context.ShowClassHorses
            //                               where sch.ShowClassId == showclassid
            //                               join sh in context.ShowHorses
            //                               on
            //                               new { a = (int)(sch.ShowHorseId) } equals new { a = (int)(sh.ShowHorsesId) }
            //                               select new
            //                               {
            //                                   sh.HorseCatalog,
            //                                   sh.ShowHorsesId,
            //                                   sh.ShowId,
            //                                   sch.ShowClassId,
            //                                   sch.IsActive,
            //                                   sch.IsCompleted,
            //                                   sch.RatedJudgesIDList
            //                               }
            //                     ).AsEnumerable();
            //        var temp = showclassHorses.Select(x => new
            //        {
            //            HorseCatalog = x.HorseCatalog,
            //            ShowHorsesId = x.ShowHorsesId,
            //            ShowName = context.Shows.Single(y => y.ShowId == x.ShowId).ShowName,
            //            ShowStartDate = context.Shows.Single(y => y.ShowId == x.ShowId).StartDate,
            //            ShowClassId = x.ShowClassId,
            //            IsActive = x.IsActive,
            //            IsCompleted = x.IsCompleted,
            //            IsJudgeRated = IsJudgerated(ipadUdid, x.RatedJudgesIDList)
            //        }).OrderByDescending(z => z.HorseCatalog).Reverse();

            //        string json = JsonConvert.SerializeObject(temp);
            //        var resp = new HttpResponseMessage()
            //        {
            //            Content = new StringContent(json, System.Text.Encoding.UTF8)
            //        };
            //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //        return resp;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogClass.WriteLogsToFile(ex);
            //    throw;
            //}
            #endregion
        }
        [HttpGet]
        [ActionName("InsertRating")]
        public int InsertRating(string ratingJason)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        ratingJason = ratingJason.Replace('[', ' ').Replace(']', ' ').Trim();
                        RatingTempData ratingdata = new RatingTempData();
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        ratingdata = (RatingTempData)serializer.Deserialize(ratingJason, typeof(RatingTempData));
                        if (ratingdata != null)
                        {
                            Rating dataToInsert = GetRatingDataObject(ratingdata);

                            if (dataToInsert != null)
                            {
                                context.Ratings.Add(dataToInsert);
                                try
                                {
                                    int result = context.SaveChanges();
                                    int horseCatalog = Convert.ToInt32(dataToInsert.HorseCatalog);

                                    Catalog insertCatalog = context.Catalogs.Where(z => z.CatalogNumber == horseCatalog && z.ShowId == dataToInsert.ShowId).Single();

                                    string oldJudgeList = insertCatalog.RatedJudgesIDList;
                                    insertCatalog.RatedJudgesIDList = oldJudgeList + "|" + ratingdata.IpadUDID;

                                    context.SaveChanges();

                                    int? showClassID = dataToInsert.ShowClassId;

                                    int? showid = dataToInsert.ShowId;
                                    bool flag = CheckHorseRatingComplete(showClassID, horseCatalog, showid, dataToInsert.JudgeCode);
                                    if (flag)
                                    {
                                        Catalog horsecatalog = context.Catalogs.Where(z => z.CatalogNumber == horseCatalog && z.ShowId == dataToInsert.ShowId).Single();
                                        horsecatalog.IsCompleted = true;
                                        horsecatalog.RatingDone = true;


                                        //horsecatalog.RatedJudgesIDList=
                                        //ShowClassHors classHorses = context.ShowClassHorses.Where(z => z.ShowHorseId == dataToInsert.ShowHorsesId).Single();
                                        //classHorses.IsCompleted = true;
                                        //classHorses.IsActive = false;
                                        //classHorses.HorseScore = GetIndividualHorseScore(dataToInsert.HorseCatalog);

                                        //classHorses.RatedJudgesIDList = GetSerializeObject(classHorses.RatedJudgesIDList, dataToInsert.IpadUdid);
                                        // classHorses.RatedJudgesIDList =
                                        context.SaveChanges();
                                    }
                                    return 1;
                                }
                                catch
                                {
                                    return 0;
                                }
                            }
                            else
                                return 2;
                        }
                        else
                            return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogClass.WriteLogsToFile(ex);
                throw;
            }
        }

        [HttpGet]
        [ActionName("ClassCompleted")]
        public int ClassCompleted(int classCompleteFlag, int classid)
        {
            try
            {
                if (isValidIP())
                {
                    using (QrecApiEntities context = new QrecApiEntities())
                    {
                        ShowClass sclass = context.ShowClasses.Where(z => z.ShowClassId == classid).Single();
                        sclass.IsCompleted = true;
                        try
                        {
                            context.SaveChanges();
                            return 1;
                        }
                        catch
                        {
                            return 0;
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogClass.WriteLogsToFile(ex);
                throw;
            }
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
                        var query = (from a in context.ShowClassJudges
                                     where (a.ShowJudgesId != null && a.ShowJudgesId == showjudgesid) && (a.ShowId != null && a.ShowId == showId)
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
                LogClass.WriteLogsToFile(ex);
                throw;
            }
        }

        [HttpGet]
        [ActionName("SetConfig")]
        //public int SetConfig(string key,string value)
        //{
        //    try
        //    {
        //        using (CurrentHorseShowEntities context = new CurrentHorseShowEntities())
        //        {
        //            Config configObj=new Config();
        //            configObj.Key=key;
        //            configObj.Value=value;
        //            context.Configs.Add(configObj);
        //            return 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogClass.WriteLogsToFile(ex);
        //        throw;
        //    }
        //}        
        #endregion

        #region Helping Methods
        public bool IsJudgerated(string ipadID, string ratedJudgeList)
        {
            try
            {
                if (!string.IsNullOrEmpty(ratedJudgeList))
                {
                    var serializer = new JavaScriptSerializer();
                    List<string> judgeList = (List<string>)serializer.Deserialize(ratedJudgeList, typeof(List<string>));
                    int ShowJudgID = GetShowJudgeID(ipadID);
                    if (judgeList.Find(x => x.Equals(ShowJudgID.ToString())).Count() > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
                throw;
            }
        }

        public string GetSerializeObject(string ratedJudgeListObj, string IpadUdid)
        {
            try
            {
                using (QrecApiEntities context = new QrecApiEntities())
                {
                    int currentJudgeID = context.ShowJudges.Where(x => x.IpadUdid == IpadUdid).Single().ShowJudgesId;
                    // string ratedJudgeListObj = classHorses.RatedJudgesIDList;
                    List<string> judgeList = new List<string>();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    if (currentJudgeID > 0)
                    {
                        if (!string.IsNullOrEmpty(ratedJudgeListObj))
                        {
                            judgeList = (List<string>)serializer.Deserialize(ratedJudgeListObj, typeof(List<string>));
                        }
                        judgeList.Add(currentJudgeID.ToString());
                    }

                    string serializeObj = serializer.Serialize(judgeList);
                    return serializeObj;
                }
            }
            catch
            {
                throw;
            }
        }

        public decimal? GetIndividualHorseScore(string horseCatalog)
        {
            try
            {
                using (QrecApiEntities context = new QrecApiEntities())
                {
                    var horsescoreList = context.Ratings.Where(z => z.HorseCatalog == horseCatalog).Select(y => y.SumScore).ToList();
                    if (horsescoreList.Count() > 2)
                    {
                        horsescoreList.Remove(horsescoreList.Max());
                        horsescoreList.Remove(horsescoreList.Min());
                    }

                    int count = horsescoreList.Count();
                    decimal? result = 0;
                    foreach (var horsescore in horsescoreList)
                    {
                        result += horsescore;
                    }
                    result = result / count;
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        public Rating GetRatingDataObject(RatingTempData ratingdata)
        {
            try
            {
                using (QrecApiEntities context = new QrecApiEntities())
                {
                    //int ShowjudgesId=context.ShowJudges.Where(z=>z.IpadUdid==ratingdata.IpadUDID && z.ShowId==ratingdata.ShowId).Single().ShowJudgesId;
                    //int ShowClassJudgesId=context.ShowClassJudges.Where(y=>y.ShowJudgesId==ShowjudgesId && y.ShowId==ratingdata.ShowId && y.ShowClassId==ratingdata.ShowClassId).Single().ShowClassJudgesId;
                    //int ShowclassjudgesidRating = context.Ratings.Where(z => z.HorseCatalog ==ratingdata.HorseCatalog && z.ShowId == ratingdata.ShowId && z.ShowClassId == ratingdata.ShowClassId).Single().ShowClassJudgesId;
                    var CurrentJudgeratingList = context.Ratings.Where(x => x.IpadUdid == ratingdata.IpadUDID && x.ShowId == ratingdata.ShowId && x.HorseCatalog == ratingdata.HorseCatalog).ToList();
                    if (CurrentJudgeratingList.Count() <= 0)
                    {
                        Rating rating = new Rating();
                        rating.BandT = ratingdata.BandT;
                        rating.Date = DateTime.Now;
                        rating.HandN = ratingdata.HandN;
                        rating.HorseCatalog = ratingdata.HorseCatalog;
                        rating.IpadUdid = ratingdata.IpadUDID;
                        rating.Legs = ratingdata.Legs;
                        rating.Move = ratingdata.Move;
                        rating.Type = ratingdata.Type;
                        //rating.JudgeCode = ratingdata.JudgeCode;
                        //rating.JudgeName = ratingdata.JudgeName;
                        
                        int showclassID = context.ShowClasses.Where(x => x.IsActive == true && x.ShowId == ratingdata.ShowId).Single().ShowClassId;
                        rating.ShowClassId = showclassID;
                        var ratingdatatemp = ratingdata.IpadUDID;
                        int showjudgeid = context.ShowJudges.Where(x => x.IpadUdid == ratingdatatemp && x.ShowId == ratingdata.ShowId).Single().ShowJudgesId;
                        rating.ShowClassJudgesId = context.ShowClassJudges.Where(y => y.ShowClassId == showclassID && y.ShowJudgesId == showjudgeid && y.ShowId == ratingdata.ShowId).Single().ShowClassJudgesId;
                        //rating.ShowHorsesId = context.ShowHorses.Where(z => z.HorseCatalog == ratingdata.HorseCatalog).Single().ShowHorsesId;
                        //rating.ShowId = context.ShowHorses.Where(z => z.HorseCatalog == ratingdata.HorseCatalog && z.ShowId == ratingdata.ShowId).Single().ShowId;
                        rating.ShowId = ratingdata.ShowId;
                        //rating.SumScore = ratingdata.BandT + ratingdata.HandN + ratingdata.Legs + ratingdata.Move + ratingdata.Type;
                        return rating;
                    }
                    else
                        return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckHorseRatingComplete(int? showClassID, int? HorseCatalog, int? showid, string judgeCode)
        {
            try
            {
                using (QrecApiEntities context = new QrecApiEntities())
                {
                    string horsecatalog = Convert.ToString(HorseCatalog);

                    // bool flag = false;
                    var judgeIDList = context.ShowClassJudges.Where(x => x.ShowClassId == showClassID && x.ShowId == showid).Select(y => y.ShowClassJudgesId).ToList();
                    var ratingJudgeIdList = context.Ratings.Where(z => z.HorseCatalog == horsecatalog && z.ShowId == showid).Select(z => z.ShowClassJudgesId).Distinct().ToList();
                    
                    //  int count = 0;
                    if (judgeIDList.Count == ratingJudgeIdList.Count)
                        return true;

                  
                    else 
                        return false;
                    //foreach (var judgeid in judgeIDList)
                    //{
                    //    if (ratingJudgeIdList.Where(k => k.Value == judgeid).Count() > 0)
                    //    {
                    //        count++;
                    //    }
                    //}
                    //if (count == judgeIDList.Count())
                    //    flag = true;
                    //return flag;
                }
            }
            catch
            {
                throw;
            }
        }

        public int GetShowJudgeID(string IpadID)
        {
            try
            {
                using (QrecApiEntities context = new QrecApiEntities())
                {
                    int ShowJudgesId = context.ShowJudges.Where(z => z.IpadUdid == IpadID).Single().ShowJudgesId;
                    return ShowJudgesId;
                }
            }
            catch
            {
                throw;
            }
        }

        public int getNumberFromString(string number)
        {
            try
            {
                int result = 0;

                Regex classNumber = new Regex(@"[^\d]");
                result = Convert.ToInt32(classNumber.Replace(number, ""));

                return result;
            }
            catch 
            {
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
        #endregion

        #region Helping Classes
        public class LogClass
        {
            /// <summary>
            /// Convert exception object to exception message with its stack trace
            /// </summary>
            /// <param name="e">Exception object</param>
            /// <returns>Formated error message</returns>
            private static string GetExceptionMessage(Exception e)
            {
                StringBuilder message = new StringBuilder();
                bool flag = false;
                string strInnerExeption;
                while (e != null)
                {
                    if (flag)
                    {
                        strInnerExeption = "Inner Exception";
                    }
                    else
                    {
                        strInnerExeption = string.Empty;
                    }

                    // Get stack trace for the exception with source file information
                    var st = new System.Diagnostics.StackTrace(e, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();

                    string method = frame.GetMethod().ToString();
                    int line1 = frame.GetFileLineNumber();

                    message.Append(Environment.NewLine);
                    message.Append("----------" + strInnerExeption + " Exception----------");
                    message.Append(Environment.NewLine);
                    message.Append("\t\r\nType : " + e.GetType().FullName);
                    message.Append(Environment.NewLine);
                    message.Append("\t\r\nMessage  : " + e.Message);
                    message.Append(Environment.NewLine);
                    message.Append("\t\r\nSource : " + e.Source);
                    message.Append(Environment.NewLine);
                    message.Append("\t\r\nLine Number : " + line + " : " + method);
                    message.Append(Environment.NewLine);
                    message.Append("\t\r\nStack Trace : \n" + e.StackTrace);
                    message.Append(Environment.NewLine);
                    message.Append("\t\r\n----------" + strInnerExeption + " Exception----------");
                    message.Append(Environment.NewLine);
                    flag = true;
                    e = e.InnerException;
                }
                return message.ToString();
            }

            /// <summary>
            /// Log message to file. Note: Define log file location in ConfigList at key MessageLogFolderPath
            /// </summary>
            /// <param name="message">Log message</param>
            /// <param name="web">Web object of site which contains ConfigList list</param>
            public static void WriteLogsToFile(string message)
            {
                try
                {
                    string LogDIRPath = ConfigurationManager.AppSettings[Constants.LogDirPath];
                    string fileName = LogDIRPath + "\\" + Constants.LogFileName + DateTime.Today.ToString("yyyy_MM_dd") + ".txt";
                    bool isExists = System.IO.Directory.Exists(LogDIRPath);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(LogDIRPath);

                    if (!System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Create(fileName).Close();
                    }
                    using (System.IO.StreamWriter w = System.IO.File.AppendText(fileName))
                    {
                        w.WriteLine("\r\nLog Entry : {0}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        string msg = "\nMessage:" + message;
                        w.WriteLine(msg);
                        w.WriteLine("__________________________");
                        w.Flush();
                        w.Close();
                    }
                }
                catch (Exception)
                {
                }
            }

            /// <summary>
            /// Log message to file. Note: Define log file location in ConfigList at key MessageLogFolderPath
            /// </summary>
            /// <param name="message">Log message</param>
            /// <param name="web">Web object of site which contains ConfigList list</param>
            public static void WriteLogsToFile(Exception ex)
            {
                try
                {
                    string message = GetExceptionMessage(ex);
                    string LogDIRPath = ConfigurationManager.AppSettings[Constants.LogDirPath];
                    string fileName = LogDIRPath + "\\" + Constants.LogFileName + DateTime.Today.ToString("yyyy_MM_dd") + ".txt";
                    bool isExists = System.IO.Directory.Exists(LogDIRPath);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(LogDIRPath);

                    if (!System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Create(fileName).Close();
                    }
                    using (System.IO.StreamWriter w = System.IO.File.AppendText(fileName))
                    {
                        w.WriteLine("\r\nLog Entry : {0}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        string msg = "\nMessage:" + message;
                        w.WriteLine(msg);
                        w.WriteLine("__________________________");
                        w.Flush();
                        w.Close();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public class RatingTempData
        {
            public string IpadUDID { get; set; }
            public string HorseCatalog { get; set; }
            public decimal Type { get; set; }
            public decimal HandN { get; set; }
            public decimal BandT { get; set; }
            public decimal Legs { get; set; }
            public decimal Move { get; set; }
            public int ShowId { get; set; }
            public int ShowClassId { get; set; }
            public string JudgeCode { get; set; }
            public string JudgeName { get; set; }
        }

        public class ShowClasses
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string ShowName { get; set; }
            public int ShowClassId { get; set; }
            public string ShowClassName { get; set; }
            public int ShowId { get; set; }
        }

        public class Constants
        {
            //App Config key
            public const string LogDirPath = "LogDirPath";

            //General Constant
            public const string LogFileName = "QREC Error Log";
        }

        #endregion
    }
}
