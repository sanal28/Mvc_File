using Newtonsoft.Json;
using NuvAsk.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NuvAsk.Controllers
{
    public class EmployeeHomeController : Controller
    {
        private string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "ABCXYZABCXYZ";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
            }

            return clearText;
        }

        // GET: EmployeeHome
        [SessionAuthorize]
        public JsonResult DeletePost(int postId)
        {
            try
            {
                using (SqlCommand deletePost = new SqlCommand("USPDeleteQuestionOrEmployee", DatabaseConnection.Connect()))
                {
                    deletePost.CommandType = CommandType.StoredProcedure;
                    deletePost.Parameters.Add("@PostIdOrUserId", SqlDbType.Int).Value = postId;
                    deletePost.Parameters.Add("@Operation", SqlDbType.Int).Value = 1;
                    deletePost.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    deletePost.ExecuteNonQuery();
                }
                return Json(JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return Json(JsonRequestBehavior.AllowGet);
            }
        }
        [SessionAuthorize]
        public ActionResult SearchByTagname()
        {
            try
            {

                return View();

            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }

        [SessionAuthorize]
        public ActionResult ResetPassword()
        {
            try
            {

                return View();

            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }

        [SessionAuthorize]
        public ActionResult SearchByPeople()
        {
            try
            {

                return View();

            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }
        [SessionAuthorize]
        public ActionResult MostVotedQuestions()
        {
            try
            {
                using (SqlCommand getMostVotedQuestions = new SqlCommand("USPTopQuestionsOrMostVotedQuestions", DatabaseConnection.Connect()))
                {
                    getMostVotedQuestions.CommandType = CommandType.StoredProcedure;
                    getMostVotedQuestions.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    getMostVotedQuestions.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = 2;
                    using (SqlDataReader MostVotedList = getMostVotedQuestions.ExecuteReader())
                    {
                        if (MostVotedList.HasRows)
                        {
                            DataTable MostVotedTable = new DataTable();
                            MostVotedTable.Load(MostVotedList);
                            return View(MostVotedTable);
                        }
                        else
                        {
                            ViewBag.SearchMostVotedCount = 0;
                            return View();
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }

        [SessionAuthorize]
        public ActionResult QuestionAsked()
        {
            try
            {
                using (SqlCommand getAskedQuestions = new SqlCommand("USPViewAskedQuestionsOrAnsweredQuestions", DatabaseConnection.Connect()))
                {
                    getAskedQuestions.CommandType = CommandType.StoredProcedure;
                    getAskedQuestions.Parameters.AddWithValue("@LoginId", SqlDbType.Int).Value = Convert.ToInt32(Session["userId"]);
                    getAskedQuestions.Parameters.AddWithValue("@Operation", SqlDbType.Int).Value = 1;
                    using (SqlDataReader askedQuestionsList = getAskedQuestions.ExecuteReader())
                    {
                        if (askedQuestionsList.HasRows)
                        {
                            DataTable askedQuestionTable = new DataTable();
                            askedQuestionTable.Load(askedQuestionsList);
                            return View(askedQuestionTable);
                        }
                        else
                        {
                            ViewBag.askedQuestionsCount = 0;
                            return View();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }

        [SessionAuthorize]
        public ActionResult QuestionAnswered()
        {
            try
            {
                using (SqlCommand getAnsweredQuestions = new SqlCommand("USPViewAskedQuestionsOrAnsweredQuestions", DatabaseConnection.Connect()))
                {
                    getAnsweredQuestions.CommandType = CommandType.StoredProcedure;
                    getAnsweredQuestions.Parameters.AddWithValue("@LoginId", SqlDbType.Int).Value = Convert.ToInt32(Session["userId"]);
                    getAnsweredQuestions.Parameters.AddWithValue("@Operation", SqlDbType.Int).Value = 2;
                    using (SqlDataReader answeredQuestionsList = getAnsweredQuestions.ExecuteReader())
                    {
                        if (answeredQuestionsList.HasRows)
                        {
                            DataTable answeredQuestionTable = new DataTable();
                            answeredQuestionTable.Load(answeredQuestionsList);
                            return View(answeredQuestionTable);
                        }
                        else
                        {
                            ViewBag.answeredQuestionsCount = 0;
                            return View();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }

        [SessionAuthorize]
        public ActionResult SearchResult(string searchKeyword)
        {
            try
            {
                using (SqlCommand getSearchResult = new SqlCommand("USPSearchTitleAndBodyOnPosts", DatabaseConnection.Connect()))
                {
                    getSearchResult.CommandType = CommandType.StoredProcedure;
                    getSearchResult.Parameters.AddWithValue("@SearchedKeyWord", SqlDbType.VarChar).Value = searchKeyword;
                    getSearchResult.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    using (SqlDataReader searchList = getSearchResult.ExecuteReader())
                    {
                        if (searchList.HasRows)
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(searchList);
                            return View(resultTable);
                        }
                        else
                        {
                            ViewBag.SearchResultCount = 0;
                            return View();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
        }
        [SessionAuthorize]
        public ActionResult EmployeeHome()
        {
            try
            {
                using (SqlCommand getRecentQuestions = new SqlCommand("USPTopQuestionsOrMostVotedQuestions", DatabaseConnection.Connect()))
                {
                    getRecentQuestions.CommandType = CommandType.StoredProcedure;
                    getRecentQuestions.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    getRecentQuestions.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = 1;
                    using (SqlDataReader recentQuestionsList = getRecentQuestions.ExecuteReader())
                    {
                        if (recentQuestionsList.HasRows)
                        {
                            DataTable recentQuestionsTable = new DataTable();
                            recentQuestionsTable.Load(recentQuestionsList);
                            return View(recentQuestionsTable);
                        }
                        else
                        {
                            ViewBag.RecentQuestionsCount = 0;
                            return View();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
            finally
            {

            }
        }
        [SessionAuthorize]
        public ActionResult AskQuestion()
        {
            try
            {
                return View();

            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
            finally
            {

            }
            
        }
        [SessionAuthorize]
        public ActionResult EditQuestion()
        {
            try
            {
                return View();

            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
            finally
            {

            }

        }
        [HttpPost]
        public JsonResult SearchPeople(string prefix)
        {
            try
            {
                using (SqlCommand searchPeople = new SqlCommand("USPSearchPeople", DatabaseConnection.Connect()))
                {
                    searchPeople.CommandType = CommandType.StoredProcedure;
                    searchPeople.Parameters.Add("@SearchPerson", SqlDbType.VarChar).Value = prefix;
                    searchPeople.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    using (SqlDataReader readPeople = searchPeople.ExecuteReader())
                    {
                        DataTable peopleTable = new DataTable();
                        peopleTable.Load(readPeople);

                        List<SearchPeopleModel> peopleList = new List<SearchPeopleModel>();
                        foreach (DataRow row in peopleTable.Rows)
                        {
                            peopleList.Add(new SearchPeopleModel
                            {
                                firstName = row["FirstName"].ToString(),
                                lastName = row["LastName"].ToString(),
                                searchedUserId = Convert.ToInt32(row["UserId"])
                            });
                        }

                        return Json(peopleList, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetQuestionsByPeople(int personId)
        {
            try
            {
                using (SqlCommand getQuestions = new SqlCommand("USPGetQuestionsPostedBySpecificUser", DatabaseConnection.Connect()))
                {
                    getQuestions.CommandType = CommandType.StoredProcedure;
                    getQuestions.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = personId;
                    using (SqlDataReader readQuestions = getQuestions.ExecuteReader())
                    {
                        DataTable questionTable = new DataTable();
                        questionTable.Load(readQuestions);
                        if (questionTable != null && questionTable.Rows.Count > 0)
                            return Json(JsonConvert.SerializeObject(questionTable), JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
                    }                    
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetQuestionsByTagname(string idList)
        {
            try
            {
                using (SqlCommand getQuestions = new SqlCommand("USPSearchByTagName", DatabaseConnection.Connect()))
                {
                    getQuestions.CommandType = CommandType.StoredProcedure;
                    getQuestions.Parameters.AddWithValue("@IdList", SqlDbType.Int).Value = idList;
                    getQuestions.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    using (SqlDataReader readQuestions = getQuestions.ExecuteReader())
                    {
                        DataTable questionTable = new DataTable();
                        questionTable.Load(readQuestions);
                        if (questionTable != null && questionTable.Rows.Count > 0)
                            return Json(JsonConvert.SerializeObject(questionTable), JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ResetUserPassword(string oldPassword, string newPassword)
        {
            try
            {
                using (SqlCommand changePassword = new SqlCommand("USPChangePassword", DatabaseConnection.Connect()))
                {
                    changePassword.CommandType = CommandType.StoredProcedure;
                    changePassword.Parameters.Add("@UserOldPassword", SqlDbType.VarChar).Value = Encrypt(oldPassword);
                    changePassword.Parameters.Add("@UserNewPassword", SqlDbType.VarChar).Value = Encrypt(newPassword);
                    changePassword.Parameters.Add("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["userId"]);
                    changePassword.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    SqlParameter flagResult = changePassword.Parameters.Add("@Flag", SqlDbType.Int);
                    flagResult.Direction = ParameterDirection.ReturnValue;
                    changePassword.ExecuteNonQuery();
                    if (Convert.ToInt32(flagResult.Value) == 1)
                        return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);

                    else
                        return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult NotifyWhenReferred(string notifyTo)
        {
            try
            {
                using (SqlCommand notifyPerson = new SqlCommand("USPNotificationWhenAPersonIsReferred", DatabaseConnection.Connect()))
                {
                    notifyPerson.CommandType = CommandType.StoredProcedure;
                    notifyPerson.Parameters.Add("@NotificationBy", SqlDbType.Int).Value = Convert.ToInt32(Session["userId"]);
                    notifyPerson.Parameters.Add("@NotificationTo", SqlDbType.Int).Value = Convert.ToInt32(notifyTo);
                    notifyPerson.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    notifyPerson.ExecuteNonQuery();
                }
                return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult PostQuestion()
        {
            string imagePaths = "";
            string fileName = "";
            var random = "";
            try
            {

                var title = System.Web.HttpContext.Current.Request.Form["title"];
                var tagIds = System.Web.HttpContext.Current.Request.Form["tagIds"];
                var contents = System.Web.HttpContext.Current.Request.Unvalidated.Form["contents"];

                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        random = DateTime.Now.ToString("ddMMyyhhmmss");
                        HttpPostedFileBase hpf = Request.Files[i];
                        fileName = Path.GetFileName(hpf.FileName);
                        hpf.SaveAs(Server.MapPath("../Images/") + random + fileName);
                        imagePaths = imagePaths + "../Images/" + random + fileName + ',';
                    }
                }
                using (SqlCommand cmd = new SqlCommand("USPInsertPost", DatabaseConnection.Connect()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PostTitle", SqlDbType.VarChar).Value = title;
                    cmd.Parameters.Add("@PostBody", SqlDbType.VarChar).Value = contents;
                    cmd.Parameters.Add("@PostCreatedBy", SqlDbType.VarChar).Value = Session["userId"];
                    cmd.Parameters.Add("@PostTags", SqlDbType.VarChar).Value = tagIds;
                    cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar).Value = Session["companyId"];
                    cmd.Parameters.Add("@Image", SqlDbType.VarChar).Value = imagePaths;
                    cmd.ExecuteNonQuery();
                }
                ViewBag.Message = "Registered";
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                ModelState.AddModelError("", "Registration Failed. Please try again");
                return Json(JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult PostEditedQuestion()
        {
            try
            {
                var title = System.Web.HttpContext.Current.Request.Form["title"];
                var tagIds = System.Web.HttpContext.Current.Request.Form["tagIds"];
                var contents = System.Web.HttpContext.Current.Request.Unvalidated.Form["contents"];
                using (SqlCommand cmd = new SqlCommand("USPUpdatePost", DatabaseConnection.Connect()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PostId", SqlDbType.Int).Value = Convert.ToInt32(Session["EditPostId"]);
                    cmd.Parameters.Add("@PostTitle", SqlDbType.VarChar).Value = title;
                    cmd.Parameters.Add("@PostBody", SqlDbType.VarChar).Value = contents;
                    cmd.Parameters.Add("@PostTags", SqlDbType.VarChar).Value = tagIds;
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    cmd.ExecuteNonQuery();
                }
                ViewBag.Message = "Updated";
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                ModelState.AddModelError("", "Updation Failed. Please try again");
                return Json(JsonRequestBehavior.AllowGet);
            }
        }


        [SessionAuthorize]
        public ActionResult EmployeeProfile(EmpolyeeRegistrationModel employeeView)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("USPProfileView", DatabaseConnection.Connect()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginId", Session["userId"]);
                    using (SqlDataReader employeeprofile = cmd.ExecuteReader())
                    {
                        while (employeeprofile.Read())
                        {
                            employeeView.fullName = employeeprofile["FullName"].ToString();
                            employeeView.email = employeeprofile["EmailId"].ToString();
                            DateTime dob = Convert.ToDateTime(employeeprofile["Dob"]);
                            employeeView.dateOfBirth = (dob.Date).ToShortDateString();

                            employeeView.employeeDesignation = employeeprofile["Designation"].ToString();
                            employeeView.companyName = employeeprofile["CompanyName"].ToString();
                            employeeView.employeePhoto = employeeprofile["Image"].ToString();
                            employeeView.phoneNumber = employeeprofile["Phone"].ToString();
                            employeeView.gender = employeeprofile["Gender"].ToString();
                            employeeView.address= employeeprofile["Address"].ToString();
                            ViewBag.Gender = employeeprofile["Gender"].ToString();
                            employeeView.userPoints =Convert.ToInt32(employeeprofile["Points"]);
                            employeeView.userBadge= employeeprofile["UserBadge"].ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
            finally
            {

            }
            return View(employeeView);
        }
        

        [SessionAuthorize]
        public JsonResult SearchWithName()
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlCommand cmd = new SqlCommand("USPSelectTags", DatabaseConnection.Connect()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                ModelState.AddModelError("", "Registration Failed. Please try again");
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            { }
        }
        [SessionAuthorize]
        public JsonResult GetQuestionDetailsForEdit()
        {
            try
            {
                using (SqlCommand getQuestionDetails = new SqlCommand("USPGetQuestionOrAnswerDetails", DatabaseConnection.Connect()))
                {
                    getQuestionDetails.CommandType = CommandType.StoredProcedure;
                    getQuestionDetails.Parameters.AddWithValue("@PostOrAnswerId", SqlDbType.Int).Value = Convert.ToInt32(Session["EditPostId"]);
                    getQuestionDetails.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    getQuestionDetails.Parameters.AddWithValue("@Operation", SqlDbType.Int).Value = 1;
                    using (SqlDataReader readQuestion = getQuestionDetails.ExecuteReader())
                    {
                        while (readQuestion.Read())
                        {
                            ViewBag.QuestionTitle = readQuestion["Title"].ToString();
                            ViewBag.QuestionBody = readQuestion["Body"].ToString();
                        }
                    }
                }
                using (SqlCommand getTagsOfQuestion = new SqlCommand("USPGetPostTagDetails", DatabaseConnection.Connect()))
                {
                    getTagsOfQuestion.CommandType = CommandType.StoredProcedure;
                    getTagsOfQuestion.Parameters.AddWithValue("@QuestionId", SqlDbType.Int).Value = Convert.ToInt32(Session["EditPostId"]);
                    using (SqlDataReader readTags = getTagsOfQuestion.ExecuteReader())
                    {
                        DataTable tagTable = new DataTable();
                        tagTable.Load(readTags);
                        return Json(new { QuestionTags = JsonConvert.SerializeObject(tagTable), QuestionTitle = ViewBag.QuestionTitle, QuestionBody = ViewBag.QuestionBody }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            { }
        }
        [SessionAuthorize]
        public JsonResult UpdateViewCount(int postId)
        {
            try
            {


                using (SqlCommand viewCount = new SqlCommand("USPUpdateViewCount", Models.DatabaseConnection.Connect()))
                {
                    viewCount.CommandType = CommandType.StoredProcedure;
                    viewCount.Parameters.AddWithValue("@ViewedPostId", SqlDbType.Int).Value = postId;
                    viewCount.Parameters.AddWithValue("@LoginUserId", SqlDbType.Int).Value = Convert.ToInt32(Session["UserId"]);
                    viewCount.ExecuteNonQuery();
                    Session["postId"] = postId;
                }
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                ModelState.AddModelError("", "Some Error Occurs");
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            { }
        }
        [SessionAuthorize]
        public JsonResult SetEditQuestionId(int postId)
        {
            try
            {
                Session["EditPostId"] = postId;
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            { }
        }

        [SessionAuthorize]
        public ActionResult ViewUnanswered()
        {
            try
            {
                using (SqlCommand getUnansweredQuestion = new SqlCommand("USPListUnansweredQuestions", DatabaseConnection.Connect()))
                {
                    getUnansweredQuestion.CommandType = CommandType.StoredProcedure;
                    getUnansweredQuestion.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    
                    using (SqlDataReader UnansweredList = getUnansweredQuestion.ExecuteReader())
                    {
                        if (UnansweredList.HasRows)
                        {
                            DataTable UnansweredTable = new DataTable();
                            UnansweredTable.Load(UnansweredList);
                            return View(UnansweredTable);
                        }
                        else
                        {
                            ViewBag.UnansweredCount = 0;
                            return View();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e);
                return RedirectToAction("UserLogin", "ErrorPage");
            }
            finally { }
        }
        [HttpPost]
        public JsonResult ChangeProfilePic()
        {
            try
            {
                var newImage = System.Web.HttpContext.Current.Request.Files["newImage"];
                var random = "";
                string path = null;
                if (newImage != null)
                {
                    random = DateTime.Now.ToString("ddMMyyhhmmss");
                    HttpPostedFile filebase = newImage;
                    var fileName = Path.GetFileName(filebase.FileName);
                    path = ("../Images/") + random + fileName;
                    filebase.SaveAs(Server.MapPath("../Images/") + random + fileName);
                }
                using (SqlCommand cmd = new SqlCommand("USPChangeUserProfilePicture", DatabaseConnection.Connect()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserImage", SqlDbType.VarChar).Value = path;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["userId"]);
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyId"]);
                    cmd.ExecuteNonQuery();
                    return Json(JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                ExceptionLog.Log(ex);
                return Json(new { id = ex }, JsonRequestBehavior.AllowGet);
            }
            finally { }
        }

    }
}
