using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace DystirWeb.Models
{
    public partial class DystirDBContext : DbContext
    {

        public DystirDBContext(DbContextOptions<DystirDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<EventsOfMatches> EventsOfMatches { get; set; }
        public virtual DbSet<MatchTypes> MatchTypes { get; set; }
        public virtual DbSet<Matches> Matches { get; set; }
        public virtual DbSet<HandballMatches> HandballMatches { get; set; }
        public virtual DbSet<Players> Players { get; set; }
        public virtual DbSet<PlayersOfMatches> PlayersOfMatches { get; set; }
        public virtual DbSet<Round> Round { get; set; }
        public virtual DbSet<Sponsors> Sponsors { get; set; }
        public virtual DbSet<Squad> Squad { get; set; }
        public virtual DbSet<Statuses> Statuses { get; set; }
        public virtual DbSet<Teams> Teams { get; set; }
        public virtual DbSet<HandballTeams> HandballTeams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DystirDatabase");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:DefaultSchema", "dystir");

            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.ToTable("Administrators", "dbo");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    ;

                entity.Property(e => e.AdministratorId).HasColumnName("AdministratorID");

                entity.Property(e => e.AdministratorTeamId).HasColumnName("AdministratorTeamID");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.ToTable("Categories", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CategorieId).HasColumnName("CategorieID");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.ToTable("Events", "dbo");

                entity.Property(e => e.EventId)
                    .HasColumnName("EventID")
                    ;

                entity.Property(e => e.EventName).IsUnicode(false);

                entity.Property(e => e.EventTextFour).IsUnicode(false);

                entity.Property(e => e.EventTextOne).IsUnicode(false);

                entity.Property(e => e.EventTextTwo).IsUnicode(false);
            });

            modelBuilder.Entity<EventsOfMatches>(entity =>
            {
                entity.HasKey(e => e.EventOfMatchId);

                entity.ToTable("EventsOfMatches", "dbo");

                entity.Property(e => e.EventOfMatchId).HasColumnName("EventOfMatchID");

                entity.Property(e => e.EventMinute).IsUnicode(false);

                entity.Property(e => e.EventName).IsUnicode(false);

                entity.Property(e => e.EventPeriodId).HasColumnName("EventPeriodID");

                entity.Property(e => e.EventTeam).IsUnicode(false);

                entity.Property(e => e.EventText).IsUnicode(false);

                entity.Property(e => e.EventTime).HasColumnType("datetime");

                entity.Property(e => e.EventTotalTime).IsUnicode(false);

                entity.Property(e => e.MainPlayerOfMatchId).HasColumnName("MainPlayerOfMatchID");

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.SecondPlayerOfMatchId).HasColumnName("SecondPlayerOfMatchID");

                entity.Property(e => e.MainPlayerOfMatchNumber).IsUnicode(false);

                entity.Property(e => e.SecondPlayerOfMatchNumber).IsUnicode(false);
            });

            modelBuilder.Entity<MatchTypes>(entity =>
            {
                entity.ToTable("MatchTypes", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MatchTypeId).HasColumnName("MatchTypeID");
            });

            modelBuilder.Entity<Matches>(entity =>
            {
                entity.HasKey(e => e.MatchID);

                entity.ToTable("Matches", "dbo");

                entity.Property(e => e.MatchID).HasColumnName("MatchID");

                entity.Property(e => e.AwayTeam).IsUnicode(false);

                entity.Property(e => e.AwayTeamId).HasColumnName("AwayTeamID");

                entity.Property(e => e.HomeTeam).IsUnicode(false);

                entity.Property(e => e.HomeTeamId).HasColumnName("HomeTeamID");

                entity.Property(e => e.Location).IsUnicode(false);

                entity.Property(e => e.MatchTypeID).HasColumnName("MatchTypeID");

                entity.Property(e => e.RoundID).HasColumnName("RoundID");

                entity.Property(e => e.RoundName).IsUnicode(false);

                entity.Property(e => e.StatusID).HasColumnName("StatusID");

                entity.Property(e => e.StatusTime).HasColumnType("datetime");

                entity.Property(e => e.TeamAdminId).HasColumnName("TeamAdminID");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.Property(e => e.HomeTeamScore).HasColumnName("HomeTeamScore");

                entity.Property(e => e.AwayTeamScore).HasColumnName("AwayTeamScore");

                entity.Property(e => e.HomeTeamOnTarget).HasColumnName("HomeTeamOnTarget");

                entity.Property(e => e.AwayTeamOnTarget).HasColumnName("AwayTeamOnTarget");

                entity.Property(e => e.HomeTeamCorner).HasColumnName("HomeTeamCorner");

                entity.Property(e => e.AwayTeamCorner).HasColumnName("AwayTeamCorner");

                entity.Ignore("ExtraMinutes");

                entity.Ignore("ExtraSeconds");
            });

            modelBuilder.Entity<HandballMatches>(entity =>
            {
                entity.HasKey(e => e.HandballMatchId);

                entity.ToTable("HandballMatches", "dbo");

                entity.Property(e => e.HandballMatchId).HasColumnName("HandballMatchID");

                entity.Property(e => e.AwayTeam).IsUnicode(false);

                entity.Property(e => e.AwayTeamId).HasColumnName("AwayTeamID");

                entity.Property(e => e.HomeTeam).IsUnicode(false);

                entity.Property(e => e.HomeTeamId).HasColumnName("HomeTeamID");

                entity.Property(e => e.Location).IsUnicode(false);

                entity.Property(e => e.MatchTypeId).HasColumnName("MatchTypeID");

                entity.Property(e => e.RoundId).HasColumnName("RoundID");

                entity.Property(e => e.RoundName).IsUnicode(false);

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.StatusTime).HasColumnType("datetime");

                entity.Property(e => e.TeamAdminId).HasColumnName("TeamAdminID");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.Property(e => e.HomeTeamScore).HasColumnName("HomeTeamScore");

                entity.Property(e => e.AwayTeamScore).HasColumnName("AwayTeamScore");

                entity.Property(e => e.HomeTeamOnTarget).HasColumnName("HomeTeamOnTarget");

                entity.Property(e => e.AwayTeamOnTarget).HasColumnName("AwayTeamOnTarget");

                entity.Property(e => e.HomeTeamCorner).HasColumnName("HomeTeamCorner");

                entity.Property(e => e.AwayTeamCorner).HasColumnName("AwayTeamCorner");

                entity.Ignore("ExtraMinutes");

                entity.Ignore("ExtraSeconds");
            });

            modelBuilder.Entity<Players>(entity =>
            {
                entity.HasKey(e => e.PlayerId);

                entity.ToTable("Players", "dbo");

                entity.Property(e => e.PlayerId).HasColumnName("PlayerID");

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Nationality).IsUnicode(false);

                entity.Property(e => e.Team).IsUnicode(false);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");
            });

            modelBuilder.Entity<PlayersOfMatches>(entity =>
            {
                entity.HasKey(e => e.PlayerOfMatchId);

                entity.ToTable("PlayersOfMatches", "dbo");

                entity.Property(e => e.PlayerOfMatchId).HasColumnName("PlayerOfMatchID");

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.Lastname).IsUnicode(false);

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.MatchTypeId).HasColumnName("MatchTypeID");

                entity.Property(e => e.MatchTypeName).IsUnicode(false);

                entity.Property(e => e.PlayerId).HasColumnName("PlayerID");

                entity.Property(e => e.Position).IsUnicode(false);

                entity.Property(e => e.SubIn).HasColumnName("SubIN");

                entity.Property(e => e.SubOut).HasColumnName("SubOUT");

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.Goal).HasColumnName("Goal");

                entity.Property(e => e.OwnGoal).HasColumnName("OwnGoal");

                entity.Property(e => e.Assist).HasColumnName("Assist");

                entity.Property(e => e.YellowCard).HasColumnName("YellowCard");

                entity.Property(e => e.RedCard).HasColumnName("RedCard");

                entity.Property(e => e.TeamName).IsUnicode(false);
            });

            modelBuilder.Entity<Round>(entity =>
            {
                entity.ToTable("Round", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RoundId).HasColumnName("RoundID");

                entity.Property(e => e.RoundName).IsUnicode(false);
            });

            modelBuilder.Entity<Sponsors>(entity =>
            {
                entity.ToTable("Sponsors", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SponsorId).HasColumnName("SponsorID");
            });

            modelBuilder.Entity<Squad>(entity =>
            {
                entity.ToTable("Squad", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SquadId).HasColumnName("SquadID");

                entity.Property(e => e.SquadShortName).HasMaxLength(50);
            });

            modelBuilder.Entity<Statuses>(entity =>
            {
                entity.ToTable("Statuses", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");
            });

            modelBuilder.Entity<Teams>(entity =>
            {
                entity.ToTable("Teams", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TeamLocation).IsUnicode(false);

                entity.Property(e => e.TeamLogo).IsUnicode(false);

                entity.Property(e => e.TeamName).IsUnicode(false);
            });

            modelBuilder.Entity<HandballTeams>(entity =>
            {
                entity.ToTable("HandballTeams", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TeamLocation).IsUnicode(false);

                entity.Property(e => e.TeamLogo).IsUnicode(false);

                entity.Property(e => e.TeamName).IsUnicode(false);
            });
        }
    }
}
