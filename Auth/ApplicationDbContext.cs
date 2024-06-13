using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RPFBE.Model.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Auth
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<AppliedJob> AppliedJobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSpecFile> SpecFiles { get; set; }  
        public DbSet<UserCV> UserCVs { get; set; }
        public DbSet<RequisitionProgress> RequisitionProgress { get; set; }
        public DbSet<JustificationFile> JustificationFiles { get; set; }
        public DbSet<PerformanceMonitoring> PerformanceMonitoring { get; set; }
        public DbSet<MonitoringSupportingDoc> MonitoringSupportingDoc { get; set; }
        public DbSet<MonitoringDoc> MonitoringDoc { get; set; }
        public DbSet<MonitoringDocView> MonitoringDocView { get; set; }
        public DbSet<ExitInterviewCard> ExitInterviewCard { get; set; }
        public DbSet<ExitInterviewForm> ExitInterviewForm { get; set; }
        public DbSet<EmployeeClearance> EmployeeClearance { get; set; }
        public DbSet<ProbationProgress> ProbationProgress { get; set; }
        public DbSet<EndofContractProgress> EndofContractProgress { get; set; }
        public DbSet<GrievanceList> GrievanceList { get; set; }
        public DbSet<DocumentSetting> DocumentSetting { get; set; }
        public DbSet<TrainingNeedList> TrainingNeedList { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MonitoringDoc>().HasMany(d => d.MonitoringDocView).WithOne(v => v.MonitoringDoc);

            //builder.Entity<ExitInterviewCard>().HasOne(d => d.ExitInterviewForm)
            //    .WithOne(v => v.ExitInterviewCard)
            //    .HasForeignKey<ExitInterviewForm>(v => v.ExitCardRef);
        }

    }
}
