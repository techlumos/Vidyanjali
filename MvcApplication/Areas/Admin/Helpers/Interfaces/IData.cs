using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vidyanjali.Areas.Admin.Models.Stakeholder;
using Vidyanjali.Areas.Admin.Models.Topic;

namespace Vidyanjali.Areas.Admin.Helpers.Interfaces
{
    interface IWebPage : IDisposable
    {
        // Get All Webpages.
        ICollection<WebPage> GetWebPages();

        // Get Webpage By ID.
        WebPage GetWebPage(int id);

        // Add new Webpage.
        void AddWebPage(WebPage webPage);

        // Delete Webpage.
        void DeleteWebPage(int id);

        //Update Webpage.
        void UpdateWebPage(WebPage webPage);

        //Update Webpage.
        void Save();
    }


    ////Commented by sunil
    //interface IUserRepository : IDisposable
    //{
    //    // Get All Customers.
    //    ICollection<User> GetCustomers();

    //    // Get Customer By ID.
    //    Customer GetCustomer(int id);

    //    // Get Customer By OpenId.
    //    Customer GetCustomer(string openId);

    //    // Add new Customer.
    //    void AddCustomer(Customer customer);

    //    // Delete Customer.
    //    void DeleteCustomer(int id);

    //    // Update Customer.
    //    void UpdateCustomer(Customer customer);

    //    // Save changes.
    //    void Save();
    //}
}