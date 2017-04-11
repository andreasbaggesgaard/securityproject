using MVCSql.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCSql.Controllers
{
    public class SQLController : Controller
    {
        // GET: SQL
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginPerson()
        {
            return View();
        }

        public ActionResult LogoutPerson()
        {
            Session["user"] = null;

            return View("LoginPerson");
        }

        [HttpPost]
        public ActionResult LoginPerson(PersonModel model)
        {
            SqlConnection conn = new SqlConnection(dbconnection.Configuration());
            SqlCommand cmd = null;
            SqlDataReader rdr = null;
            string dbSaltedPassword = "";
            var sqlselect = "SELECT * FROM [user] WHERE username = @username";
            //var sqlselect = "SELECT * FROM [user] WHERE username = @username AND password = @password";
            //var sqlselect2 = "SELECT * FROM [user] WHERE username = '" + model.username + "' AND password = '" + model.password + "'";

            try
            {
                conn.Open();

                cmd = new SqlCommand(sqlselect, conn);
                SqlParameter p1 = new SqlParameter("username", model.username);
                cmd.Parameters.Add(p1);      

                rdr = cmd.ExecuteReader();

                rdr.Read();
                
                dbSaltedPassword = (string)rdr["password"];

                conn.Close();

                if (PersonModel.ValidatePassword(model.password, dbSaltedPassword))
                {
                    Session["user"] = model.username;

                    return RedirectToAction("PersonPage", "SQL", Session["user"]);
                }
                else
                {
                    ViewBag.Message = "Invalid username or password - Try again.";
                }
                                
            }         
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            finally
            {
                conn.Close();
            }
         
            return View();
        }

        //[Authorize]
        public ActionResult PersonPage()
        {
            return View();
        }

        public ActionResult CreatePerson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePerson(PersonModel model)
        {
            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(dbconnection.Configuration());
                SqlCommand cmd = null;
                string sqlins = "insert into [user] values(@username, @password)";

                try
                {
                    string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$";
                    Regex re = new Regex(passwordRegex);
                    if (!re.IsMatch(model.password))
                    {
                        ModelState.AddModelError("Password", "Password must contain at least 1 capital letter, 1 lowercase letter, 1 alphabet, 1 numeric, 1 special character and 8 characters");
                    }
                    else
                    {
                        conn.Open();

                        cmd = new SqlCommand(sqlins, conn);
                        cmd.Parameters.Add("@username", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar);

                        string saltedPassword = PersonModel.HashPassword(model.password);

                        cmd.Parameters["@username"].Value = model.username;
                        cmd.Parameters["@password"].Value = saltedPassword;

                        cmd.ExecuteNonQuery();
                        ViewBag.message = MvcHtmlString.Create("User created with username:" + " <b>" + model.username + "</b> " + "and can now <a href='LoginPerson'>login</a>");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.message = ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            return View();
        }



    }
}