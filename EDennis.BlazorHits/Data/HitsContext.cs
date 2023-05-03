using EDennis.BlazorUtils;
using Microsoft.EntityFrameworkCore;

namespace EDennis.BlazorHits
{
    public class DesignTimeDbContextFactory_HitsContext : DesignTimeDbContextFactory<HitsContext>
    {
    }

    public partial class HitsContext : DbContext
    {

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Song> Songs { get; set; }

        public HitsContext(DbContextOptions<HitsContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            if (Database.IsSqlServer())
            {
                builder.HasSequence<int>($"seq{nameof(Artist)}",
                opt =>
                {
                    opt.StartsAt(1);
                    opt.IncrementsBy(1);
                });

                builder.HasSequence<int>($"seq{nameof(Song)}",
                    opt =>
                    {
                        opt.StartsAt(1);
                        opt.IncrementsBy(1);
                    });

                builder.Entity<Artist>(e =>
                {
                    e.ToTable(nameof(Artist), tblBuilder =>
                    {
                        tblBuilder.IsTemporal(histBuilder =>
                        {
                            histBuilder.UseHistoryTable(nameof(Artist), "dbo_history");
                            histBuilder.HasPeriodStart("SysStart");
                            histBuilder.HasPeriodEnd("SysEnd");
                        });
                    });
                    e.HasKey(e => e.Id);
                    e.Property(p => p.Id)
                        .HasDefaultValueSql($@"(NEXT VALUE FOR [seq{nameof(Artist)}])");

                });

                builder.Entity<Song>(e =>
                {
                    e.ToTable(nameof(Song), tblBuilder =>
                    {
                        tblBuilder.IsTemporal(histBuilder =>
                        {
                            histBuilder.UseHistoryTable(nameof(Song), "dbo_history");
                            histBuilder.HasPeriodStart("SysStart");
                            histBuilder.HasPeriodEnd("SysEnd");
                        });
                    });
                    e.HasKey(e => e.Id);
                    e.Property(p => p.Id)
                        .HasDefaultValueSql($@"(NEXT VALUE FOR [seq{nameof(Song)}])");
                    e.HasOne(i => i.Artist)
                        .WithMany(i => i.Songs)
                        .HasForeignKey(i => i.ArtistId)
                        .HasPrincipalKey(i => i.Id)
                        .OnDelete(DeleteBehavior.ClientCascade);

                });
            } else
            {
                builder.Entity<Artist>(e =>
                {
                    e.ToTable(nameof(Artist));
                    e.HasKey(e => e.Id);
                    e.Property<DateTime>("SysStart")
                        .HasDefaultValue(DateTime.Now)
                        .ValueGeneratedOnAddOrUpdate();
                    e.Property<DateTime>("SysEnd")
                        .HasDefaultValue(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                });

                builder.Entity<Song>(e =>
                {
                    e.ToTable(nameof(Song));
                    e.HasKey(e => e.Id);
                    e.HasOne(i => i.Artist)
                        .WithMany(i => i.Songs)
                        .HasForeignKey(i => i.ArtistId)
                        .HasPrincipalKey(i => i.Id);
                    e.Property<DateTime>("SysStart")
                        .HasDefaultValue(DateTime.Now)
                        .ValueGeneratedOnAddOrUpdate();
                    e.Property<DateTime>("SysEnd")
                        .HasDefaultValue(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                });

            }

            builder.Entity<Artist>().HasData(TestRecords.GetArtists());
            builder.Entity<Song>().HasData(TestRecords.GetSongs());


        }


    }
}