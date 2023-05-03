using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
using System.Text.Json;

namespace EDennis.BlazorUtils
{

    /// <summary>
    /// Used with GetWithDevExtreme QueryController endpoints.  
    /// DevExtreme packs query parameters into a special format, which
    /// requires work to parse and which results in a DataSourceLoader
    /// object.  This wrapper class allows the process to be divided
    /// into stages so that the resultant query can be modified
    /// (e.g., adding an Include directive) before being executed.
    /// </summary>
    public static class DataSourceLoadOptionsBuilder
    {

        private static readonly JsonSerializerOptions jsonSerializerOptions
            = new()
            { PropertyNameCaseInsensitive = true };


        /// <summary>
        /// Builds a DevExtreme DataSourceLoadOptions object
        /// from the provided parameters, which are formatted
        /// in the DevExpress proprietary format.
        /// </summary>
        /// <param name="select">like ["Field1","Field2"]</param>
        /// <param name="sort">like [{"selector": "Field1", "desc": false}] OR 
        /// [{"selector": "Field1", "desc": false},{"selector": "Field2", "asc": false}]</param>
        /// <param name="filter">like [["Field1","contains","en"]] OR 
        /// [["Field1","Bob"],["Field2","1/1/2000 02:03:04.005"]] OR
        /// ["Field1",">",0]
        /// </param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="totalSummary"></param>
        /// <param name="group"></param>
        /// <param name="groupSummary"></param>
        /// <param name="requireTotalCount"></param>
        /// <param name="isCountQuery">Whether is a count query</param>
        /// <param name="requireGroupCount"></param>
        /// <returns></returns>
        public static DataSourceLoadOptions Build(
            string select, string sort, string filter, int skip, int take,
            string totalSummary, string group, string groupSummary,
            bool requireTotalCount, bool isCountQuery, bool requireGroupCount)
        {

            var loadOptions = new DataSourceLoadOptions()
            {
                Skip = skip,
                Take = take,
                RequireTotalCount = requireTotalCount,
                RequireGroupCount = requireGroupCount,
                IsCountQuery = isCountQuery
            };

            try
            {
                loadOptions.Select = select == null ? null : JsonSerializer.Deserialize<string[]>(select);
            }
            catch
            {
                throw new ArgumentException($"Could not parse provided '{select}' argument into valid DevExtreme select expression");
            }

            try
            {
                loadOptions.Sort = sort == null ? null : JsonSerializer.Deserialize<SortingInfo[]>(sort, jsonSerializerOptions);
            }
            catch
            {
                throw new ArgumentException($"Could not parse provided '{sort}' argument into valid DevExtreme SortingInfo[] expression");
            }

            try
            {
                loadOptions.Filter = filter == null ? null : JsonSerializer.Deserialize<IList>(filter);
            }
            catch
            {
                throw new ArgumentException($"Could not parse provided '{filter}' argument into valid DevExtreme Filter expression");
            }

            try
            {
                loadOptions.TotalSummary = totalSummary == null ? null : JsonSerializer.Deserialize<SummaryInfo[]>(totalSummary, jsonSerializerOptions);
            }
            catch
            {
                throw new ArgumentException($"Could not parse provided '{totalSummary}' argument into valid DevExtreme SummaryInfo[] expression");
            }

            try
            {
                loadOptions.Group = group == null ? null : JsonSerializer.Deserialize<GroupingInfo[]>(group, jsonSerializerOptions);
            }
            catch
            {
                throw new ArgumentException($"Could not parse provided '{group}' argument into valid DevExtreme GroupingInfo[] expression");
            }

            try
            {
                loadOptions.GroupSummary = groupSummary == null ? null : JsonSerializer.Deserialize<SummaryInfo[]>(groupSummary, jsonSerializerOptions);
            }
            catch
            {
                throw new ArgumentException($"Could not parse provided '{groupSummary}' argument into valid DevExtreme SummaryInfo[] expression");
            }

            return loadOptions;
        }
    }

    /// <summary>
    /// Used to automatically bind the DevExtreme parameters to
    /// the query parameters specified in DataSourceLoadOptionsBuilder
    /// <see cref="DataSourceLoadOptionsBuilder"/>
    /// <see cref="DataSourceLoadOptionsBinder"/>
    /// </summary>
    [ModelBinder(BinderType = typeof(DataSourceLoadOptionsBinder))]
    public class DataSourceLoadOptions : DataSourceLoadOptionsBase
    {
        /// <summary>
        /// String representation of Linq Include expression
        /// </summary>
        public string Include { get; set; }
    }


    /// <summary>
    /// Used to automatically bind the DevExtreme parameters to
    /// the query parameters specified in DataSourceLoadOptionsBuilder
    /// <see cref="DataSourceLoadOptions"/>
    /// </summary>
    public class DataSourceLoadOptionsBinder : IModelBinder
    {

        /// <summary>
        /// Binds the DevExtreme parameters to the 
        /// DataSourceLoadOptionsBuilder parameters
        /// </summary>
        /// <param name="context">the ModelBindingContext</param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext context)
        {
            var loadOptions = new DataSourceLoadOptions();

            DataSourceLoadOptionsParser.Parse(loadOptions, key =>
                context.ValueProvider.GetValue(key).FirstOrDefault());

            context.Result = ModelBindingResult.Success(loadOptions);

            return Task.CompletedTask;
        }

    }




}
