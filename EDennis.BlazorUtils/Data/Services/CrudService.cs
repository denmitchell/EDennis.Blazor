using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace EDennis.BlazorUtils
{
    public abstract class CrudService<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        #region Variables

        /// <summary>
        /// Allows specifying a different SysStart (PeriodStart) column for
        /// SQL Server temporal tables.
        /// </summary>
        public virtual string SysStartColumn { get; } = "SysStart";

        /// <summary>
        /// The Entity Framework DbContext class for communicating with the
        /// database.
        /// </summary>
        public TContext DbContext { get; private set; }

        /// <summary>
        /// The name of the authenticated Claims Principal, which is 
        /// used to update SysUser
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// A service for replacing the normal DbContext with a DbContext
        /// that is more suitable for testing (e.g., one with an open
        /// transaction).
        /// </summary>
        private readonly DbContextService<TContext> _dbContextService;

        /// <summary>
        /// A cache that is used to hold the count of records across pages
        /// for specific queries (by where/filter clause).  This is helpful
        /// to prevent recounting records when the user is merely paging
        /// across the same logical result set.
        /// </summary>
        private readonly CountCache<TEntity> _countCache;

        /// <summary>
        /// The table name associated with the entity.
        /// </summary>
        public static string TableName { get; private set; }

        /// <summary>
        /// Overrideable threshold for expiring the cached count of records.  This value
        /// need not be large because it is mainly supporting paging across records, which
        /// typically is done fairly quickly by users.
        /// </summary>
        public virtual double CountCacheExpirationInSeconds { get; private set; } = 60;

        /// <summary>
        /// Gets the table name associated with the entity
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            if (TableName == null)
            {
                var entityType = typeof(TEntity);
                var modelEntityType = DbContext.Model.FindEntityType(entityType);
                TableName = modelEntityType.GetSchemaQualifiedTableName();
            }
            return TableName;
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new instance of <see cref="CrudService{TContext, TEntity}"/> with
        /// various <see cref="CrudServiceDependencies{TContext, TEntity}"/> injected.
        /// The constructor sets up a reference to the DbContext, the DbContextService
        /// (for replacing the DbContext during testing), and the <see cref="CountCache{TEntity}"/>.
        /// The constructor also uses the <see cref="AuthenticationStateProvider"/> to
        /// set the UserName property from the Claims Principal name.
        /// </summary>
        /// <param name="deps">A bundled set of dependencies to inject</param>
        public CrudService(CrudServiceDependencies<TContext, TEntity> deps)
        {
            _dbContextService = deps.DbContextService;
            _countCache = deps.CountCache;

            DbContext = deps.DbContextService.GetDbContext(deps.Configuration);
            SetUserName(deps.AuthenticationStateProvider, deps.SecurityOptions);
        }

        #endregion
        #region Testing Support

        /// <summary>
        /// Uses the DbContextService to replace the existing DbContext with a 
        /// context that is more suitable for testing (e.g., one having an open
        /// transaction that automatically rolls back)
        /// </summary>
        /// <param name="testDbContextType">The nature of the new DbContext</param>
        /// <param name="output">An Xunit helper class for piping logs to the appropriate
        /// output stream during testing</param>
        public void SetDbContext(DbContextType testDbContextType
            = DbContextType.SqlServerOpenTransaction, ITestOutputHelper output = null)
        {
            DbContext = _dbContextService.GetTestDbContext(testDbContextType, output);
        }


        #endregion
        #region Write Operations


        /// <summary>
        /// Sets the UserName property of this service based upon the Claims Principal's 
        /// Name claim.  This class uses <see cref="MvcAuthenticationStateProvider"/>
        /// </summary>
        /// <param name="authenticationStateProvider"></param>
        /// <param name="securityOptions"></param>
        public void SetUserName(AuthenticationStateProvider authenticationStateProvider,
            SecurityOptions securityOptions)
        {
            Task.Run(async () =>
            {
                var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
                UserName = authState.User.Claims.FirstOrDefault(c => c.Type == securityOptions.IdpUserNameClaim)?.Value;
            });
        }


        /// <summary>
        /// Inserts a new record into the database, based upon data in
        /// the provided entity.
        /// </summary>
        /// <param name="input">The entity holding data to insert into the database</param>
        /// <returns></returns>
        public virtual async Task<TEntity> CreateAsync(TEntity input)
        {
            UpdateSysUser();
            BeforeCreate(input); //optional lifecycle method

            //update SysGuid when relevant
            if (input is IHasSysGuid iHasSysGuid && iHasSysGuid.SysGuid == default)
                iHasSysGuid.SysGuid = Guid.NewGuid();

            await DbContext.AddAsync(input);
            await DbContext.SaveChangesAsync();

            AfterCreate(input); //optional lifecycle method

            return input;
        }


        /// <summary>
        /// Updates a record in the database, based upon data
        /// contained in the provided entity.  Note that the 
        /// Id should match the primary key of the provided entity.
        /// </summary>
        /// <param name="input">The entity holding data to update</param>
        /// <param name="id">The primary key of the entity</param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(
            TEntity input, params object[] id)
        {
            var existing = await FindRequiredAsync(id);

            BeforeUpdate(existing); //optional lifecycle method

            //Entity Framework has some special methods for
            //updating entities.  This is the recommended pattern:
            var entry = DbContext.Entry(existing);
            entry.CurrentValues.SetValues(input);
            entry.State = EntityState.Modified;

            UpdateSysUser();
            await DbContext.SaveChangesAsync();

            AfterUpdate(existing); //optional lifecycle method

            return input;

        }



        /// <summary>
        /// Deletes an existing entity
        /// </summary>
        /// <param name="key">the primary key of the entity</param>
        /// <returns>OK or NoContent, if successful</returns>
        /// <seealso cref="Delete(string)"/>
        public async virtual Task DeleteAsync(params object[] id)
        {
            var existing = await FindRequiredAsync(id);

            BeforeDelete(existing);

            UpdateSysUser();
            await DbContext.SaveChangesAsync();
            DbContext.Remove(existing);
            await DbContext.SaveChangesAsync();

            AfterDelete(existing);
        }

        /// <summary>
        /// Updates SysUser in all changed entities that implement IHasSysUser
        /// </summary>
        public virtual void UpdateSysUser()
        {
            var entries = DbContext.ChangeTracker.Entries().ToList();
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Entity is IHasSysUser entity)
                    switch (entry.State)
                    {
                        case EntityState.Added:
                        case EntityState.Modified:
                        case EntityState.Deleted:
                            entity.SysUser = UserName;
                            break;
                        default:
                            break;
                    }
            }
        }

        #endregion
        #region Lifecycle Methods

        /// <summary>
        /// Overrideable method that will be executed before Create
        /// </summary>
        /// <param name="input">The entity to create</param>
        public virtual void BeforeCreate(TEntity input) { }

        /// <summary>
        /// Overrideable method that will be executed after Create
        /// </summary>
        /// <param name="input">The entity to create</param>
        public virtual void AfterCreate(TEntity input) { }

        /// <summary>
        /// Overrideable method that will be executed before Update
        /// </summary>
        /// <param name="existing">The entity to update</param>
        public virtual void BeforeUpdate(TEntity existing) { }

        /// <summary>
        /// Overrideable method that will be executed after Update
        /// </summary>
        /// <param name="existing">The entity to update</param>
        public virtual void AfterUpdate(TEntity existing) { }

        /// <summary>
        /// Overrideable method that will be executed before Delete
        /// </summary>
        /// <param name="existing">The entity to delete</param>
        public virtual void BeforeDelete(TEntity existing) { }

        /// <summary>
        /// Overrideable method that will be executed after Delete
        /// </summary>
        /// <param name="existing">The entity to delete</param>
        public virtual void AfterDelete(TEntity existing) { }

        #endregion
        #region Basic Read Operations

        /// <summary>
        /// Finds a record based upon the provided primary key.  Note that
        /// this method will return null if the record isn't found.
        /// See also <see cref="FindRequiredAsync(object[])"/>
        /// </summary>
        /// <param name="id">The primary key of the target record</param>
        /// <returns></returns>
        public async Task<TEntity> FindAsync(params object[] id)
            => await DbContext.FindAsync<TEntity>(id);


        /// <summary>
        /// Finds a record based upon the provided primary key.  Note that
        /// this method will throw an exception if the record isn't found.
        /// See also <see cref="FindAsync(object[])"/>
        /// </summary>
        /// <param name="id">The primary key of the target record</param>
        /// <returns></returns>
        public async Task<TEntity> FindRequiredAsync(params object[] id)
        {
            var existing = await DbContext.FindAsync<TEntity>(id)
                ?? throw new Exception($"{typeof(TEntity).Name} with key equal to {JsonSerializer.Serialize(id)} not found.");
            return existing;
        }

        #endregion
        #region DevExpress

        /// <summary>
        /// Merely returns a typed IQueryable.  DevExpress supports binding
        /// IQueryables to their data grid.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetQueryable()
        => GetQuery();

        #endregion
        #region Radzen

        /// <summary>
        /// Returns a page of data for the provided Radzen <see cref="Query"/> arguments. 
        /// This version of the method returns a typed page result.  It will ignore a
        /// provided Select argument.
        /// See also <see cref="GetPageSelectAsync(Query)"/>
        /// </summary>
        /// <param name="qryArgs">Serializable qryArgs parameters</param>
        /// <returns></returns>
        public virtual async Task<PageResult<TEntity>> GetPageAsync(Query qryArgs = null)
        {

            //build the query
            var (query, count) = BuildQuery(qryArgs);

            //return the results
            var data = await query.ToListAsync();
            return new PageResult<TEntity>
            {
                Data = data,
                CountAcrossPages = count
            };

        }


        /// <summary>
        /// Returns a page of data for the provided Radzen <see cref="Query"/> arguments. 
        /// This version of the method returns an untyped page result.  It will apply a
        /// provided Select argument, only retrieving specified columns.
        /// See also <see cref="GetPageAsync(Query)"/>
        /// </summary>
        /// <param name="qryArgs">Serializable qryArgs parameters</param>
        /// <returns></returns>
        public virtual async Task<PageResult> GetPageSelectAsync(Query qryArgs = null)
        {

            //build the query
            var (query, count) = BuildQuery(qryArgs);

            //return untyped result, even if no select is provided
            if (qryArgs == null || qryArgs.Select == null)
            {
                var data = await query.ToDynamicListAsync();

                return new PageResult
                {
                    Data = data,
                    CountAcrossPages = count
                };
            }
            //return untyped results that represent a project of the result set
            //(a subset of available columns)
            else
            {
                var projection =
                    await query.Select(qryArgs.Select).ToDynamicListAsync();

                return new PageResult
                {
                    Data = projection,
                    CountAcrossPages = count
                };
            }
        }


        #endregion
        #region Dynamic Linq

        /// <summary>
        /// Asynchronously gets a dynamic list result using a Dynamic Linq Expression
        /// https://github.com/StefH/System.Linq.Dynamic.Core
        /// https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions
        /// </summary>
        /// <param name="where">string Where expression</param>
        /// <param name="orderBy">string OrderBy expression (with support for descending)</param>
        /// <param name="select">string Select expression</param>
        /// <param name="include">string Include expression</param>
        /// <param name="skip">int number of records to skip</param>
        /// <param name="take">int number of records to return</param>
        /// <param name="totalRecords">total number of records across all pages (calculated by library)</param>
        /// <returns>dynamic-typed object</returns>
        public virtual async Task<DynamicLinqResult> GetDynamicLinqResultAsync(
                string select, string include = null,
                string where = null, string orderBy = null,
                int? skip = null, int? take = null, int? totalRecords = null
                )
        {

            var qryArgs = new Query
            {
                Filter = where,
                Select = select,
                Expand = include?.Replace(";", ","),
                OrderBy = orderBy,
                Skip = skip,
                Top = take
            };

            var skipValue = skip == null ? 0 : skip.Value;
            if (skipValue > totalRecords)
                skipValue = totalRecords.Value;

            var takeValue = take == null ? totalRecords.Value - skipValue : take.Value;


            var (query,count) = BuildQuery(qryArgs);
            var selectResult = query.Select(select);

            return new DynamicLinqResult
            {
                Data = await selectResult.ToDynamicListAsync(),
                PageCount = (int)Math.Ceiling(count / (double)take),
                CurrentPage = 1 + (int)Math.Ceiling(skipValue / (double)takeValue),
                RowCount = count
            };

        }


        /// <summary>
        /// Asynchronously gets a dynamic list result using a Dynamic Linq Expression
        /// https://github.com/StefH/System.Linq.Dynamic.Core
        /// https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions
        /// </summary>
        /// <param name="where">string Where expression</param>
        /// <param name="orderBy">string OrderBy expression (with support for descending)</param>
        /// <param name="select">string Select expression</param>
        /// <param name="include">string Include expression</param>
        /// <param name="skip">int number of records to skip</param>
        /// <param name="take">int number of records to return</param>
        /// <param name="totalRecords">total number of records across all pages (calculated by library)</param>
        /// <returns>dynamic-typed object</returns>
        public virtual async Task<DynamicLinqResult<TEntity>> GetDynamicLinqResultAsync(
                string include = null,
                string where = null, string orderBy = null,
                int? skip = null, int? take = null, int? totalRecords = null
                )
        {

            PageResult<TEntity> pageResult = await GetPageAsync(new Query
            {
                Filter = where,
                Expand = include?.Replace(";", ","),
                OrderBy = orderBy,
                Skip = skip,
                Top = take
            });

            var skipValue = skip == null ? 0 : skip.Value;
            if (skipValue > totalRecords)
                skipValue = totalRecords.Value;

            var takeValue = take == null ? totalRecords.Value - skipValue : take.Value;

            return new DynamicLinqResult<TEntity>
            {
                Data = pageResult.Data.ToList(),
                PageCount = (int)Math.Ceiling(pageResult.CountAcrossPages / (double)take),
                CurrentPage = 1 + (int)Math.Ceiling(skipValue / (double)takeValue),

                RowCount = pageResult.CountAcrossPages
            };

        }


        #endregion
        #region Helper Methods

        /// <summary>
        /// Returns an AsNoTracking IQueryable
        /// </summary>
        /// <returns></returns>
        private IQueryable<TEntity> GetQuery(bool asNoTracking = true)
        {
            var dbSet = DbContext
                .Set<TEntity>();

            IQueryable<TEntity> qry = dbSet.AsQueryable();

            if (asNoTracking)
                qry = qry
                .AsNoTracking();

            return qry;
        }

        /// <summary>
        /// Gets any modified records (helpful for testing purposes)
        /// </summary>
        /// <param name="asOf">Get all modifications after this datetime.
        /// NOTE: do not make this >=.  It will not work!</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetModified(DateTime asOf)
        {
            var results = DbContext.Set<TEntity>()
                .Where(e => EF.Property<DateTime>(e, SysStartColumn) > asOf)
                .ToList();

            return results;
        }

        /// <summary>
        /// Gets the most recent create/update datetime value for a given entity.
        /// When retrieved before an operation is performed, it can be used in 
        /// combination with <see cref="GetModified(DateTime)"/> to obtain all
        /// entities that were modified as a result of the operation (only when 
        /// isolated testing is performed).
        /// </summary>
        /// <returns></returns>
        public DateTime GetMaxSysStart()
        {
            FormattedString sql = new($"SELECT MAX({SysStartColumn}) Value FROM {GetTableName()}");

            return DbContext.Database
                .SqlQuery<DateTime>(sql)
                .FirstOrDefault();
        }



        /// <summary>
        /// Builds the Dynamic Linq IQueryable from Radzen <see cref="Query"/> arguments
        /// </summary>
        /// <param name="qryArgs">Radzen <see cref="Query"/> arguments</param>
        /// <returns></returns>
        private (IQueryable<TEntity> Query, int Count) BuildQuery(Query qryArgs = null)
        {

            var query = GetQuery();

            //handle Expand/Include -- including child records from navigation properties
            if (qryArgs != null && !string.IsNullOrWhiteSpace(qryArgs.Expand))
            {
                var includes = qryArgs.Expand.Split(',');
                foreach (var incl in includes)
                    query = query.Include(incl);
            }

            //handle Filter/Where -- limited the result set by a condition
            if (qryArgs != null && !string.IsNullOrEmpty(qryArgs.Filter))
            {
                if (qryArgs.FilterParameters != null)
                    query = query.Where(qryArgs.Filter, qryArgs.FilterParameters);
                else
                    query = query.Where(qryArgs.Filter);
            }

            //get the count of records.  Note that the count of records
            var count = _countCache.GetCount(qryArgs, query, TimeSpan.FromSeconds(CountCacheExpirationInSeconds));

            //handle OrderBy -- server-side sorting of records
            if (qryArgs != null && !string.IsNullOrEmpty(qryArgs.OrderBy))
            {
                query = query.OrderBy(qryArgs.OrderBy);
            }

            //handle skipping of a certain number of records -- part of paging
            if (qryArgs != null && qryArgs.Skip != null && qryArgs.Skip > 0)
            {
                query = query.Skip(qryArgs.Skip.Value);
            }

            //handling taking of a certain number of records -- part of paging
            if (qryArgs != null && qryArgs.Top != null && qryArgs.Top > 0)
            {
                query = query.Take(qryArgs.Top.Value);
            }

            //return the query and the count across records
            return (query, count);
        }

        #endregion

    }

    /// <summary>
    /// Workaround class to support dynamically building SQL string for 
    /// <see cref="CrudService{TContext, TEntity}.GetMaxSysStart"/>
    /// </summary>
    public class FormattedString : FormattableString
    {
        private readonly string _str;
        public FormattedString(FormattableString str)
        {
            _str = str.ToString();
        }

        public override int ArgumentCount => 0;

        public override string Format => _str;

        public override object GetArgument(int index)
        {
            return null;
        }

        public override object[] GetArguments()
        {
            return Array.Empty<object>();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return _str;
        }
    }
}
