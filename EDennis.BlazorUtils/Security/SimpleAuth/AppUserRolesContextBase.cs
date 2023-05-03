using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EDennis.BlazorUtils
{
    public abstract partial class AppUserRolesContextBase : DbContext
    {

        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        public virtual string AppUserTableName { get; } = "AppUser";
        public virtual string AppRoleTableName { get; } = "AppRole";
        public abstract IEnumerable<AppRole> RoleData { get; }
        public abstract IEnumerable<AppUser> UserData { get; }


        public AppUserRolesContextBase(DbContextOptions<AppUserRolesContextBase> options
            ) : base(options)
        {
        }

        protected AppUserRolesContextBase(DbContextOptions options)
        : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {

            OnModelBuilding(builder);

            if (Database.IsSqlServer())
            {

                builder.HasSequence<int>($"seq{AppUserTableName}",
                    opt =>
                    {
                        opt.StartsAt(1);
                        opt.IncrementsBy(1);
                    });

                builder.HasSequence<int>($"seq{AppRoleTableName}",
                    opt =>
                    {
                        opt.StartsAt(1);
                        opt.IncrementsBy(1);
                    });

                builder.Entity<AppUser>(e =>
                {
                    e.ToTable($"{AppUserTableName}", tblBuilder =>
                    {
                        tblBuilder.IsTemporal(histBuilder =>
                        {
                            histBuilder.UseHistoryTable($"{AppUserTableName}", "dbo_history");
                            histBuilder.HasPeriodStart("SysStart");
                            histBuilder.HasPeriodEnd("SysEnd");
                        });
                    });
                    e.HasKey(e => e.Id);
                    e.HasOne(i => i.AppRole)
                        .WithMany(i => i.AppUsers)
                        .HasForeignKey(i => i.RoleId)
                        .HasPrincipalKey(i => i.Id);
                    e.Property(p => p.Id)
                        .HasDefaultValueSql($@"(NEXT VALUE FOR [seq{AppUserTableName}])");


                });

                builder.Entity<AppRole>(e =>
                {
                    e.ToTable($"{AppRoleTableName}", tblBuilder =>
                    {
                        tblBuilder.IsTemporal(histBuilder =>
                        {
                            histBuilder.UseHistoryTable($"{AppRoleTableName}", "dbo_history");
                            histBuilder.HasPeriodStart("SysStart");
                            histBuilder.HasPeriodEnd("SysEnd");
                        });
                    });
                    e.HasKey(e => e.Id);
                    e.Property(p => p.Id)
                        .HasDefaultValueSql($@"(NEXT VALUE FOR [seq{AppRoleTableName}])");
                });

            }
            else
            {
                builder.Entity<AppUser>(e =>
                {
                    e.ToTable($"{AppUserTableName}");
                    e.HasKey(e => e.Id);
                    e.HasOne(i => i.AppRole)
                        .WithMany(i => i.AppUsers)
                        .HasForeignKey(i => i.RoleId)
                        .HasPrincipalKey(i => i.Id)
                        .OnDelete(DeleteBehavior.ClientSetNull);
                    e.Property<DateTime>("SysStart")
                        .HasDefaultValue(DateTime.Now)
                        .ValueGeneratedOnAddOrUpdate();
                    e.Property<DateTime>("SysEnd")
                        .HasDefaultValue(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                });

                builder.Entity<AppRole>(e =>
                {
                    e.ToTable($"{AppRoleTableName}");
                    e.HasKey(e => e.Id);
                    e.Property<DateTime>("SysStart")
                        .HasDefaultValue(DateTime.Now)
                        .ValueGeneratedOnAddOrUpdate();
                    e.Property<DateTime>("SysEnd")
                        .HasDefaultValue(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                });

            }


            if (RoleData.Any())
                builder.Entity<AppRole>().HasData(RoleData);

            if (UserData.Any())
                builder.Entity<AppUser>().HasData(UserData);


        }


    }
}