using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
//using Vidyanjali.Areas.Admin.Models.Catalog;
using Vidyanjali.Models;

namespace Vidyanjali.Comman
{
    public static class Extension
    {
        public static void ShowMessage(this Controller controller, MessageType messageType, string message, bool showAfterRedirect = false)
        {
            var messageTypeKey = messageType.ToString();
            if (showAfterRedirect)
            {
                controller.TempData[messageTypeKey] = message;
            }
            else
            {
                controller.ViewData[messageTypeKey] = message;
            }
        }

        public enum MessageType
        {
            Alert,
            Error,
            Success,
            Notification
        }

        /// <summary>
        /// Render all messages that have been set during execution of the controller action.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString RenderMessages(this HtmlHelper htmlHelper)
        {
            var messages = String.Empty;
            foreach (var messageType in Enum.GetNames(typeof(MessageType)))
            {
                var message = htmlHelper.ViewContext.ViewData.ContainsKey(messageType)
                                ? htmlHelper.ViewContext.ViewData[messageType]
                                : htmlHelper.ViewContext.TempData.ContainsKey(messageType)
                                    ? htmlHelper.ViewContext.TempData[messageType]
                                    : null;
                if (message != null)
                {
                    var messageBoxBuilder = new TagBuilder("div");
                    messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", messageType.ToLowerInvariant()));
                    messageBoxBuilder.SetInnerText(message.ToString());
                    messages += messageBoxBuilder.ToString();
                }
            }
            return MvcHtmlString.Create(messages);
        }

        public static string ToInrCurrency(this decimal amount, bool usePercentage = false)
        {
            return !usePercentage ? "&#8377;" + amount.ToString("#") : amount.ToString("#") + "%";
        }

        //Previously commneted content - Sunil
        //public static KeyValuePair<bool, decimal> CheckAndApplyDiscount(this Product product)
        //{
        //    TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime indianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianZone);

        //    var discountCanApplicable = false;
        //    decimal discountAmount = 0;
        //    decimal specialPrice = product.Price;

        //     Applicable
        //    if ((product.Discount != null))
        //    {
        //         Today is between applicable dates
        //        if (product.Discount.IsActive && (indianDateTime >= product.Discount.StartDate && indianDateTime <= product.Discount.EndDate))
        //        {
        //            if (product.Discount.UsePercentage)
        //                discountAmount = product.Price - ((product.Price / 100) * product.Discount.DiscountAmountOrPercentage);
        //            else
        //                discountAmount = product.Price - product.Discount.DiscountAmountOrPercentage;
        //            discountCanApplicable = true;
        //            specialPrice = discountAmount;
        //            using (var db = new CoreContext())
        //            {
        //                product.SpecialPrice = specialPrice;
        //                db.Entry(product).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //    }

        //    return new KeyValuePair<bool, decimal>(discountCanApplicable, discountAmount);
        //}

        //Commneted By Sunil - 13Mar17
        //public static bool CheckAndApplyDiscount(this Product product)
        //{
        //    TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime indianDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianZone);
        //    var isapplicable = product.Discount != null &&
        //                       (product.Discount.IsActive &&
        //                        (indianDateTime >= product.Discount.StartDate &&
        //                         indianDateTime <= product.Discount.EndDate));
        //    return isapplicable;
        //}
    }

    /// <summary>
    /// If we're dealing with ajax requests, any message that is in the view data goes to
    /// the http header.
    /// </summary>
    public class AjaxMessagesFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var viewData = filterContext.Controller.ViewData;
                var response = filterContext.HttpContext.Response;

                foreach (var messageType in Enum.GetNames(typeof(Extension.MessageType)))
                {
                    var message = viewData.ContainsKey(messageType)
                                    ? viewData[messageType]
                                    : null;
                    if (message != null) // We store only one message in the http header. First message that comes wins.
                    {
                        response.AddHeader("X-Message-Type", messageType);
                        response.AddHeader("X-Message", message.ToString());
                        return;
                    }
                }
            }
        }
    }
}