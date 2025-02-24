﻿using Project5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using MimeKit;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Project5.Controllers
{
    public class HomeController : Controller
    {
        private ElectionEntities1 db = new ElectionEntities1();
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the national ID and email match
                var existingUser = db.Users.FirstOrDefault(u => u.national_id == user.national_id && u.email == user.email);

                if (existingUser != null)
                {
                    // Generate a reset token
                    string resetToken = GenerateResetToken();

                    // Send the reset token to the user's email
                    SendResetTokenEmail(existingUser.email, resetToken);

                    // Store the token and email temporarily (e.g., in TempData) and redirect to the confirm reset token view
                    TempData["UserEmail"] = existingUser.email;
                    TempData["ResetToken"] = resetToken;

                    return RedirectToAction("ConfirmResetToken");
                }
                else
                {
                    ModelState.AddModelError("", "الرقم الوطني والبريد الإلكتروني لا يتطابقان.");
                }
            }

            return View(user);
        }

        // GET: ConfirmResetToken
        [HttpGet]
        public ActionResult ConfirmResetToken()
        {
            return View();
        }

        // POST: ConfirmResetToken
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmResetToken(string email, string resetToken)
        {
            // Check if the token matches
            if (resetToken == TempData["ResetToken"]?.ToString() && email == TempData["UserEmail"]?.ToString())
            {
                return RedirectToAction("SetNewPassword");
            }
            else
            {
                ModelState.AddModelError("", "رمز إعادة تعيين كلمة المرور أو البريد الإلكتروني غير صحيح.");
            }

            return View();
        }

        // GET: SetNewPassword
        [HttpGet]
        public ActionResult SetNewPassword()
        {
            return View();
        }

        // POST: SetNewPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword == confirmPassword)
            {
                // Find the user by email
                var user = db.Users.FirstOrDefault(u => u.email == email);
                if (user != null)
                {
                    // Update the user's password
                    user.password = newPassword;
                    db.SaveChanges();

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "المستخدم غير موجود.");
                }
            }
            else
            {
                ModelState.AddModelError("", "كلمة المرور الجديدة وتأكيد كلمة المرور لا تتطابقان.");
            }

            return View();
        }

        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString().Substring(0, 8); // Example: generates a random 8-character string
        }

        private void SendResetTokenEmail(string toEmail, string resetToken)
        {

            var fromEmail = "techlearnhub.contact@gmail.com";
            var SmtpPassword = "lyrlogeztsxclank";
            string subjectText = "Your Confirmation Code";
            string messageText = $"Your confirmation code is {resetToken}";

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourAppName", fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Your Password Reset Token";
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subjectText;
                mailMessage.Body = messageText;
                mailMessage.IsBodyHtml = false;

                using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(fromEmail, SmtpPassword);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                }
            }



        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                // Authenticate the user
                var existingUser = db.Users.FirstOrDefault(u => u.email == user.email && u.password == user.password);
                if (existingUser != null)
                {
                    Session["UserEmail"] = existingUser.email;

                    // Redirect to a secure area or home page
                    return RedirectToAction("DistrictArea", "home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View(user);
        }



        /// ////////////////////////////////////////////////////////////////////////



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DistrictArea()
        {
            var userEmail = Session["UserEmail"] as string;
            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login if the session has expired or user is not logged in
                return RedirectToAction("Login", "home");
            }

            var user = db.Users.FirstOrDefault(u => u.email == userEmail);
            if (user != null)
            {
                var electionarea = user.electionarea;
                ViewBag.electionarea = electionarea;
                return View();
            }
            else
            {
                // Handle the case where the user is not found
                return RedirectToAction("ListType", "home");
            }

            //var user_id = Session["email"];
            //var user_id = 1;
            //var user = db.Users.Find(user_id);
            //var electionarea = user.electionarea;
            //ViewBag.electionarea = electionarea;
            //return View();
        }


        public ActionResult ListType()
        {
            //var em = Session["email"];
            //var user = db.Users.Find(em);

            var userEmail = Session["UserEmail"] as string;
            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login if the session has expired or user is not logged in
                return RedirectToAction("Login", "Home");
            }

            var user = db.Users.FirstOrDefault(u => u.email == userEmail);
            if (user != null)
            {
                var electionarea = user.electionarea;
                ViewBag.electionarea = electionarea;
                Session["VoteLocal"] = user.voteLocal;
                Session["VoteParty"] = user.voteParty;
                return View();
            }
            else
            {
                // Handle the case where the user is not found
                return RedirectToAction("Party", "Home");
            }
        }
    
        public ActionResult firstIrbidLocal()
        {
            return View();
        }
        public ActionResult secondIrbidLocal()
        {
            return View();
        }
        public ActionResult Party()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult Party(string party)
        {
            if (!string.IsNullOrEmpty(party))
            {
                var record = db.PartyLists.SingleOrDefault(x => x.listname == party.Trim());
                if (record != null)
                {
                    // Increment the value of the specified column
                    record.votes_counter += 1;
                    db.SaveChanges();
                    Session["VoteParty"] = 1;
                }

            }
          var loginedUserEmail = Session["UserEmail"].ToString();
            var data = db.Users.Where(y => y.email == loginedUserEmail).FirstOrDefault();
            data.voteParty += 1;
            db.SaveChanges();

            return RedirectToAction("ListType", "Home");
        }


        public ActionResult Election(string name)
        {
            var listName = name;
            Session["listName"] = listName; 
            var lists = db.LocalCandidates.Where(x => x.listname == listName).ToList();
            ViewBag.CandidateNames = lists;
            return View(lists);

        }

        [HttpPost]
        public ActionResult Election(string[] selectedCandidates)
        {
            if (selectedCandidates != null && selectedCandidates.Length > 0)
            {
                var listName = Session["listName"] as string;

                if (!string.IsNullOrEmpty(listName))
                {
                    var record = db.LocalCandidates.FirstOrDefault(x => x.listname == listName.Trim());
                    if (record != null)
                    {
                        record.votes_counter += 1;
                        db.SaveChanges();
                        Session["VoteLocal"] = 1;
                    }

                    var loginedUserEmail = Session["UserEmail"].ToString();
                    var user = db.Users.FirstOrDefault(y => y.email == loginedUserEmail);
                    if (user != null)
                    {
                        user.voteLocal = 1;
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("ListType", "Home");
        }
        public ActionResult MafraqLocal()
        {
            return View();
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}