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

        public virtual string SysStartColumn { get; } = "SysStart";

        public TContext DbContext { get; private set; }
        public string UserName { get; private set; }

        private readonly DbContextService<TContext> _dbContextService;
        private readonly CountCache<TEntity> _countCache;
        private static readonly Dictionary<string, PropertyInfo> _propertyDict;
        private static readonly JsonSerializerOptions _jsonSerializerOptions
            = new()
            { PropertyNameCaseInsensitive = true };


        #endregion
        #region Constructor

        public CrudService(CrudServiceDependencies<TContext, TEntity> deps)
        {
            _dbContextService = deps.DbContextService;
            _countCache = deps.CountCache;

            DbContext = deps.DbContextService.GetDbContext(deps.Configuration);
            SetUserName(deps.AuthenticationStateProvider, deps.SecurityOptions);
        }

        #endregion
        #region Testing Support

        public void SetDbContext(DbContextType testDbContextType
            = DbContextType.SqlServerOpenTransaction, ITestOutputHelper output = null)
        {
            DbContext = _dbContextService.GetTestDbContext(testDbContextType, output);
        }


        #endregion
        #region Write Operations


        public void SetUserName(AuthenticationStateProvider authenticationStateProvider,
            SecurityOptions securityOptions)
        {
            Task.Run(async () =>
            {
                var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
                UserName = authState.User.Claims.FirstOrDefault(c => c.Type == securityOptions.IdpUserNameClaim)?.Value;
            });
        }


        public virtual async Task<TEntity> CreateAsync(TEntity input)
        {
            UpdateSysUser();
            BeforeCreate(input);

            if (input is IHasSysGuid iHasSysGuid && iHasSysGuid.SysGuid == default)
                iHasSysGuid.SysGuid = Guid.NewGuid();

            await DbContext.AddAsync(input);
            await DbContext.SaveChangesAsync();

            AfterCreate(input);

            return input;
        }


        public virtual async Task<TEntity> UpdateAsync(
            TEntity input, params object[] id)
        {
            var existing = await FindRequiredAsync(id);

            BeforeUpdate(existing);

            var entry = DbContext.Entry(existing);
            entry.CurrentValues.SetValues(input);
            entry.State = EntityState.Modified;

            UpdateSysUser();
            await DbContext.SaveChangesAsync();

            AfterUpdate(existing);

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
            //NOTE: a preliminary call to save changes is made PRIOR TO THE DELETE
            //in order to capture the SysUser associated with the delete
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


        /// <summary>
        /// Static constructor, which builds the PropertyInfo 
        /// dictionary statically
        /// </summary>
        static CrudService()
        {
            var properties = typeof(TEntity).GetProperties();
            _propertyDict = new Dictionary<string, PropertyInfo>();

            for (int i = 0; i < properties.Length; i++)
            {

                //PascalCase
                _propertyDict.Add(properties[i].Name, properties[i]);

                //camelCase
                _propertyDict.Add(properties[i].Name[..1].ToLower()
                    + properties[i].Name[1..], properties[i]);
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

        public async Task<TEntity> FindAsync(params object[] id)
            => await DbContext.FindAsync<TEntity>(id);


        public async Task<TEntity> FindRequiredAsync(params object[] id)
        {
            var existing = await DbContext.FindAsync<TEntity>(id)
                ?? throw new Exception($"{typeof(TEntity).Name} with key equal to {JsonSerializer.Serialize(id)} not found.");
            return existing;
        }

        #endregion
        #region DevExpress

        public virtual IQueryable<TEntity> GetQueryable()
        => GetQuery();

        #endregion
        #region Radzen

        /// <summary>
        /// Radzen, but use LoadData event (and set Count property)
        /// </summary>
        /// <param name="qryArgs">Serializable qryArgs parameters</param>
        /// <returns></returns>
        public virtual async Task<PageResult<TEntity>> GetPageAsync(Query qryArgs = null)
        {

            var (query, count) = BuildQuery(qryArgs);

            var data = await query.ToListAsync();
            return new PageResult<TEntity>
            {
                Data = data,
                CountAcrossPages = count
            };

        }


        /// <summary>
        /// Radzen, but use LoadData event (and set Count property)
        /// </summary>
        /// <param name="qryArgs">Serializable qryArgs parameters</param>
        /// <returns></returns>
        public virtual async Task<PageResult> GetPageSelectAsync(Query qryArgs = null)
        {

            var (query, count) = BuildQuery(qryArgs);

            if (qryArgs == null || qryArgs.Select == null)
            {
                var data = await query.ToDynamicListAsync();

                return new PageResult
                {
                    Data = data,
                    CountAcrossPages = count
                };
            }
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
        /// Returns an AsNoTracking IQueryable, but
        /// also applies AdjustQuery
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

        public IEnumerable<TEntity> GetModified(DateTime asOf)
        {
            var results = DbContext.Set<TEntity>()
                .Where(e => EF.Property<DateTime>(e, SysStartColumn) > asOf)
                .ToList();

            return results;
        }

        public DateTime GetMaxSysStart()
        {
            FormattedString sql = new($"SELECT MAX({SysStartColumn}) Value FROM {GetTableName()}");

            return DbContext.Database
                .SqlQuery<DateTime>(sql)
                .FirstOrDefault();
        }


        private static string _tableName;
        public string GetTableName()
        {
            if (_tableName == null)
            {
                var entityType = typeof(TEntity);
                var modelEntityType = DbContext.Model.FindEntityType(entityType);
                _tableName = modelEntityType.GetSchemaQualifiedTableName();
            }
            return _tableName;

        }


        private (IQueryable<TEntity> Query, int Count) BuildQuery(Query qryArgs = null)
        {

            var query = GetQuery();

            if (qryArgs != null && !string.IsNullOrWhiteSpace(qryArgs.Expand))
            {
                var includes = qryArgs.Expand.Split(',');
                foreach (var incl in includes)
                    query = query.Include(incl);
            }


            if (qryArgs != null && !string.IsNullOrEmpty(qryArgs.Filter))
            {
                if (qryArgs.FilterParameters != null)
                    query = query.Where(qryArgs.Filter, qryArgs.FilterParameters);
                else
                    query = query.Where(qryArgs.Filter);
            }

            var count = _countCache.GetCount(qryArgs, query, TimeSpan.FromSeconds(60));

            if (qryArgs != null && !string.IsNullOrEmpty(qryArgs.OrderBy))
            {
                query = query.OrderBy(qryArgs.OrderBy);
            }


            if (qryArgs != null && qryArgs.Skip != null && qryArgs.Skip > 0)
            {
                query = query.Skip(qryArgs.Skip.Value);
            }

            if (qryArgs != null && qryArgs.Top != null && qryArgs.Top > 0)
            {
                query = query.Take(qryArgs.Top.Value);
            }

            return (query, count);
        }

        #endregion

    }

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
