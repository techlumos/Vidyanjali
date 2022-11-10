using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Vidyanjali.Areas.Admin.Models.Stakeholder;
using Vidyanjali.Areas.Admin.Models.Topic;
using Vidyanjali.Areas.EMS.Models;
using Vidyanjali.Areas.Admin.Models;

namespace Vidyanjali.Models
{
    public class CoreContext : DbContext
    {
        public DbSet<WebPage> WebPages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Testimonials> Testimonials { get; set; }
        public DbSet<Spotlight> Spotlights { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Gallery> Gallerys { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<Bugle> Bugles { get; set; }

        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            dbModelBuilder.Entity<WebPage>().HasOptional(parent => parent.ParentPage)
                .WithMany(child => child.ChildPages)
                .HasForeignKey(pk => pk.ParentId)
                .WillCascadeOnDelete(false);
        }
    }
}