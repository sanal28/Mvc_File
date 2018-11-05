using ETLCenter.CommonLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETLCenter.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [SessionAuthorize]
        public ActionResult AdminHome()
        {
            return View();
        }
        [SessionAuthorize]
        public ActionResult AddEmployee()
        {
            return View();
          
        }
        [HttpPost]
        [SessionAuthorize]
        public JsonResult EmployeeDetailsFirstLoad(int PageNo, int RowCount)
        {
            try
            {
                string employeeList = string.Empty;
                ETLEmployeeService.ETLCenterEmployeeService selectEmpdetails = new ETLEmployeeService.ETLCenterEmployeeService();
                selectEmpdetails.Url = Constants.EmployeeService;
                employeeList = selectEmpdetails.SelectEmployeeDetails(PageNo, RowCount);
                if (employeeList != string.Empty)
                    return Json(employeeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [SessionAuthorize]
        public JsonResult AddEmployee(string userName, string passWord, string emailId, bool checkBoxActive, int PageNo, int RowCount)
        {
            bool addEmployeeflag;
            int chkActive = checkBoxActive ? 1 : 0;

            try
            {
                ETLEmployeeService.ETLCenterEmployeeService addEmployeeService = new ETLEmployeeService.ETLCenterEmployeeService();
                addEmployeeService.Url = Constants.EmployeeService;
                addEmployeeflag = addEmployeeService.EmployeeCreation(0, userName, passWord, emailId, chkActive, 0, "",0, Convert.ToInt32(Session["EmployeeID"]),Convert.ToInt32(Session["EmployeeID"]), 1);
                return Json(new { employeeList = CommonFunctions.getEmployeeDetails(PageNo, RowCount), flag = addEmployeeflag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public JsonResult UpdateEmployeeDetails(string employeeId, string empolyeeName, bool employeeIsActive, int PageNo, int RowCount)
        {
            bool isEmployeeUpdate;

            try
            {
                ETLEmployeeService.ETLCenterEmployeeService updateUser = new ETLEmployeeService.ETLCenterEmployeeService();
                updateUser.Url = Constants.EmployeeService;
                isEmployeeUpdate = updateUser.EmployeeCreation(Convert.ToInt32(employeeId), empolyeeName, "", "", Convert.ToInt32(employeeIsActive), 0, "",0 ,Convert.ToInt32(Session["EmployeeID"]), Convert.ToInt32(Session["EmployeeID"]), 2);

                return Json(new { employeeList = CommonFunctions.getEmployeeDetails(PageNo, RowCount), flag = isEmployeeUpdate }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }


        [HttpPost]
        [SessionAuthorize]
        public JsonResult DeleteEmployee(string employeeId, int PageNo, int RowCount)
        {

            string isEmployeeDelete;
            int Returnvalue;
            try
            {
                ETLEmployeeService.ETLCenterEmployeeService updateUser = new ETLEmployeeService.ETLCenterEmployeeService();
                updateUser.Url = Constants.EmployeeService;
                isEmployeeDelete = updateUser.EmployeeDeletion(Convert.ToInt32(employeeId), "", "", "", 1, 0, "",0, Convert.ToInt32(Session["EmployeeID"]), Convert.ToInt32(Session["EmployeeID"]), 3);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(isEmployeeDelete);
                Returnvalue = Convert.ToInt32(dt.Rows[0]["Returnvalue"]);
                return Json(new { employeeList = CommonFunctions.getEmployeeDetails(PageNo, RowCount), flag = true, returnvalue = Returnvalue }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        public JsonResult CheckEmail(string emailId)
        {

            bool isEmailExists;

            try
            {

                ETLEmployeeService.ETLCenterEmployeeService checkEmail = new ETLEmployeeService.ETLCenterEmployeeService();
                checkEmail.Url = Constants.EmployeeService;
                isEmailExists = checkEmail.MailValidation(emailId);
                return Json(new { flag = isEmailExists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                Convert.ToString(ControllerContext.RouteData.Values["action"]),
                Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = -1 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                Dispose();
            }
        }

        [HttpPost]
        public JsonResult ChangeProfileImage()
        {
            bool isEmployeeUpdate;
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
                    path = ("../Uploads/") + Path.GetFileNameWithoutExtension(fileName) + "^_^_^" + random + Path.GetExtension(fileName);
                    filebase.SaveAs(Server.MapPath("../Uploads/") + path);
                }
                ETLEmployeeService.ETLCenterEmployeeService updateUser = new ETLEmployeeService.ETLCenterEmployeeService();
                updateUser.Url = Constants.EmployeeService;
                isEmployeeUpdate = updateUser.EmployeeCreation(Convert.ToInt32(Session["EmployeeID"]), "", "", "", 0, 0, path, 0,0, Convert.ToInt32(Session["EmployeeID"]), 4);
                return Json(new { flag = isEmployeeUpdate, imgpath = path }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = -1 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }

        }

        [HttpPost]
        public JsonResult DisplayProfileImage()
        {
            string employeeImage;
            try
            {
                ETLEmployeeService.ETLCenterEmployeeService empProfilePathService = new ETLEmployeeService.ETLCenterEmployeeService();
                empProfilePathService.Url = Constants.EmployeeService;
                string empProfilePic = empProfilePathService.SelectProfilePic(Convert.ToInt32(Session["EmployeeID"]));
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(empProfilePic);
                employeeImage = Convert.ToString(dt.Rows[0]["EmpProfileImagePath"]);
                return Json(new { image = employeeImage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                     Convert.ToString(ControllerContext.RouteData.Values["action"]),
                     Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { id = -1 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }


        }

        [SessionAuthorize]
        public ActionResult EmailAlert()
        {
            return View();
        }
       
        [HttpPost]
        [SessionAuthorize]
        public JsonResult AddEmailAlert(string PackaGroup, string EmployeeName, bool checkBoxActive, int PageNo, int RowCount)
        {
            bool addEmailAlert;
            int chkActive = checkBoxActive ? 1 : 0;

            try
            {
                ETLPackageCreation.ETLCenterPackageCreation getPackageDeatils = new ETLPackageCreation.ETLCenterPackageCreation();
                getPackageDeatils.Url = Constants.PackageService;
                addEmailAlert = getPackageDeatils.EmailAlertCreation(0, Convert.ToInt32(EmployeeName), Convert.ToInt32(PackaGroup), chkActive, 0, Convert.ToInt32(Session["EmployeeID"]), DateTime.Now, Convert.ToInt32(Session["EmployeeID"]), 1);
                return Json(new { emailAlertList = CommonFunctions.getEmailAlertDetails(PageNo, RowCount), flag = addEmailAlert }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult GetEmailId(int EmailId)
        {
            string employeeEmail;

            try
            {
                ETLPackageCreation.ETLCenterPackageCreation employeeemialIDService = new ETLPackageCreation.ETLCenterPackageCreation();
                employeeemialIDService.Url = Constants.PackageService;
                string JsonString = employeeemialIDService.EmployeeEmailId(EmailId);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonString);
                employeeEmail = Convert.ToString(dt.Rows[0]["EmpEmail"]);
                return Json(new { email = employeeEmail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { id = -1 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }

        }
        [HttpGet]
        public JsonResult GetGroupList()
        {
            ETLPackageCreation.ETLCenterPackageCreation packageGrouplistService = new ETLPackageCreation.ETLCenterPackageCreation();
            packageGrouplistService.Url = Constants.PackageService;
            try
            {
                string JsonString = packageGrouplistService.DropDownBind(1);
                if (JsonString != string.Empty)
                    return Json(JsonString, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;

            }
            finally
            {
                Dispose();

            }
            return Json(CommonLibrary.Constants.JsonError);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult EmailAlertFirstLoad(int PageNo, int RowCount)
        {
            try
            {
                string emailAlertList= string.Empty;
                ETLPackageCreation.ETLCenterPackageCreation selectEmailAlertDetails = new ETLPackageCreation.ETLCenterPackageCreation();
                selectEmailAlertDetails.Url = Constants.PackageService;
                 emailAlertList = selectEmailAlertDetails.SelectEmailAlertDetails(PageNo, RowCount);
                if (emailAlertList != string.Empty)
                    return Json(emailAlertList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteEmailAlert(string employeePackageId, int PageNo, int RowCount)
        {
            bool deletePackageflag;
            try
            {
                ETLPackageCreation.ETLCenterPackageCreation deleteEmailAlertService = new ETLPackageCreation.ETLCenterPackageCreation();
                deleteEmailAlertService.Url = Constants.PackageService;
                deletePackageflag = deleteEmailAlertService.EmailAlertCreation(Convert.ToInt32(employeePackageId), 1, 1, 1, 1, Convert.ToInt32(Session["EmployeeID"]), DateTime.Now, 1, 3);
                return Json(new { emailAlertList = CommonFunctions.getEmailAlertDetails(PageNo, RowCount), flag = deletePackageflag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        [HttpPost]
        [SessionAuthorize]
        public JsonResult UpdateEmailAlert(string EmpPackageId, string PackaGroup, string EmployeeName, bool checkBoxActive, int PageNo, int RowCount)
        {
            bool updateEmailAlert;
            int chkActive = checkBoxActive ? 1 : 0;

            try
            {
                ETLPackageCreation.ETLCenterPackageCreation updateEmailAlertService = new ETLPackageCreation.ETLCenterPackageCreation();
                updateEmailAlertService.Url = Constants.PackageService;
                updateEmailAlert = updateEmailAlertService.EmailAlertCreation(Convert.ToInt32(EmpPackageId), Convert.ToInt32(EmployeeName), Convert.ToInt32(PackaGroup), chkActive, 0, Convert.ToInt32(Session["EmployeeID"]), DateTime.Now, Convert.ToInt32(Session["EmployeeID"]), 2);
                return Json(new { emailAlertList = CommonFunctions.getEmailAlertDetails(PageNo, RowCount), flag = updateEmailAlert }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(new { flag = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult GetPackageGridData(int PageNo,int RowCount)
        {
            ETLPackageCreation.ETLCenterPackageCreation getPackageDeatils = new ETLPackageCreation.ETLCenterPackageCreation();
            getPackageDeatils.Url = Constants.PackageService;
            try
            {
                string jsonString = string.Empty;
                jsonString = getPackageDeatils.BindAdminDashBoardPackages(PageNo, RowCount); //change value of offset as per pagination case
                if (jsonString != string.Empty)
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }
        
             [HttpPost]
        [SessionAuthorize]
        public JsonResult GetUserGridData(int PageNo, int RowCount)
        {
            ETLPackageCreation.ETLCenterPackageCreation getPackageDeatils = new ETLPackageCreation.ETLCenterPackageCreation();
            getPackageDeatils.Url = Constants.PackageService;
            try
            {
                string jsonString = string.Empty;
                jsonString = getPackageDeatils.BindAdminDashBoardUserInformation(PageNo, RowCount); //change value of offset as per pagination case
                if (jsonString != string.Empty)
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult GetErrorrGridData(int PageNo, int RowCount)
        {
            ETLPackageCreation.ETLCenterPackageCreation getPackageDeatils = new ETLPackageCreation.ETLCenterPackageCreation();
            getPackageDeatils.Url = Constants.PackageService;
            try
            {
                string jsonString = string.Empty;
                jsonString = getPackageDeatils.BindAdminDashBoardErrorDetails(PageNo, RowCount); //change value of offset as per pagination case
                if (jsonString != string.Empty)
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult GetPackageStatisticGraph(int PageNo, int RowCount)
        {
            ETLPackageCreation.ETLCenterPackageCreation getPackageStatiticDeatils = new ETLPackageCreation.ETLCenterPackageCreation();
            getPackageStatiticDeatils.Url = Constants.PackageService;
            try
            {
                string jsonString = string.Empty;
                jsonString = getPackageStatiticDeatils.BindingPackagestatisticsGraph(PageNo, RowCount); //change value of offset as per pagination case
                if (jsonString != string.Empty)
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionAuthorize]
        public JsonResult GetEmployeePermissionList()
        {
            ETLEmployeeService.ETLCenterEmployeeService empPermission = new ETLEmployeeService.ETLCenterEmployeeService();
            empPermission.Url = Constants.EmployeeService;
            try
            {
                string jsonString = string.Empty;
                jsonString = empPermission.GetEmployeePermissionList(Convert.ToInt32(Session["EmployeeID"])); //change value of offset as per pagination case
                if (jsonString != string.Empty)
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonFunctions commonFun = new CommonFunctions();
                commonFun.ExceptionLog(ControllerContext.HttpContext, ex.Message, ex.TargetSite.Name,
                    Convert.ToString(ControllerContext.RouteData.Values["action"]),
                    Convert.ToString(ControllerContext.RouteData.Values["controller"]));
                commonFun = null;
                return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                Dispose();
            }
            return Json(CommonLibrary.Constants.JsonError, JsonRequestBehavior.AllowGet);
        }
        
    }
}
