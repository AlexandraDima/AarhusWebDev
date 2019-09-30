using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using AarhusWebDeveloperCorp.ViewModels;
using System.Net.Mail;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace AarhusWebDeveloperCorp.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        //This method can be accessed only when the user edits the form
        //handle form input data and send it as an email

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //Send email message
            //The method data receives the data as an object of type ContactForm
            MailMessage message = new MailMessage();
            message.To.Add("username@eaaa.dk");
            message.Subject = model.Subject;
            message.From = new MailAddress(model.Email, model.Name);
            message.Body = model.Message;

            //Send the message as an email
            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true; smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                //Removed Network credentials 
                smtp.Credentials = new System.Net.NetworkCredential("", "");
                // send mail 
                smtp.Send(message);
            }

            //Display a message to the user in case of success
            TempData["success"] = true;


            //Get the GuildUdi of the current page
            GuidUdi currentPageUdi = new GuidUdi(CurrentPage.ContentType.ItemType.ToString(), CurrentPage.Key);

            //Create the new content type with parameters: string name, Udi parentId , string contentTypeAlias
            IContent msg = Services.ContentService.CreateContent(model.Subject, currentPageUdi, "message");
            msg.SetValue("messageName", model.Name);
            msg.SetValue("email", model.Email);
            msg.SetValue("subject", model.Subject);
            msg.SetValue("messageContent", model.Message);
            msg.SetValue("umbracoNaviHide", true);

            //Save
            Services.ContentService.Save(msg);
            //declare a method that receives form input and send the page back to the client
            return RedirectToCurrentUmbracoPage();
        }
    }
}