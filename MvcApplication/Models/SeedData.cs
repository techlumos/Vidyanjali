using System;
using System.Collections.Generic;
using System.Data.Entity;
using Vidyanjali.Areas.Admin.Helpers;
using Vidyanjali.Areas.Admin.Models.Topic;

namespace Vidyanjali.Models
{
    public class SeedData : DropCreateDatabaseIfModelChanges<CoreContext>
         
    {
        protected override void Seed(CoreContext context)
        {
            try
            {
            }
            catch (Exception e)
            {

            }
        }
    }
}