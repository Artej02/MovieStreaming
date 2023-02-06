using Kendo.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UBOResearchTool.Custom.Attributes;

namespace UBOResearchTool.Custom.Helpers
{
    public class KendoGridHelper
    {

        public static string ApplyFilter<T>(IFilterDescriptor filter)
        {
            string parameters = String.Empty;
            string filterMessage = String.Empty;
            List<string> convertValue = new List<string>();
            string filerOperators = String.Empty;
            if (filter is CompositeFilterDescriptor)
            {
                foreach (IFilterDescriptor childFilter in ((CompositeFilterDescriptor)filter).FilterDescriptors)
                {
                    parameters += " " + ApplyFilter<T>(childFilter) + " " + ((CompositeFilterDescriptor)filter).LogicalOperator;
                }

                char[] logical = ((CompositeFilterDescriptor)filter).LogicalOperator.ToString().ToCharArray();
                parameters = parameters.Trim(logical);
            }
            else
            {
                FilterDescriptor filterDescriptor = (FilterDescriptor)filter;
                filterMessage = filterDescriptor.Member;
                try
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(filterDescriptor.Member);
                    object[] attribute = propertyInfo.GetCustomAttributes(typeof(JoinAlias), true);
                    if (attribute.Length > 0)
                    {
                        JoinAlias myAttribute = (JoinAlias)attribute[0];
                        string joinAlias = myAttribute.Value;
                        if (!string.IsNullOrWhiteSpace(joinAlias))
                            filterMessage = String.Join('.', joinAlias, filterMessage);
                    }
                }
                catch (Exception) { }

                dynamic value;
                //var type = filterDescriptor.Value.GetType();
                if (filterDescriptor.Value is DateTime)
                {
                    filterMessage = "CONVERT(DATE, " + filterMessage + ")";
                    value = "'" + $"{filterDescriptor.Value:MM/dd/yyyy}" + "'";
                }
                else if (filterDescriptor.Value is double || filterDescriptor.Value is int)
                {
                    value = Convert.ToInt32(filterDescriptor.Value);
                }
                else
                {
                    value = "'" + $"{filterDescriptor.Value}" + "'";
                }
                switch (filterDescriptor.Operator)
                {
                    case FilterOperator.IsEqualTo:
                        parameters += filterMessage + "=" + value;
                        break;
                    case FilterOperator.IsNotEqualTo:
                        parameters += filterMessage + "<>" + value;
                        break;
                    case FilterOperator.StartsWith:
                        parameters += filterMessage + " like " + value + "+'%'";
                        break;
                    case FilterOperator.Contains:
                        parameters += filterMessage + " like '%'+" + value + "+'%'";
                        break;
                    case FilterOperator.DoesNotContain:
                        parameters += filterMessage + " Not like '%'+" + value + "+'%'";
                        break;
                    case FilterOperator.EndsWith:
                        parameters += filterMessage + " like '%'+" + value;
                        break;
                    case FilterOperator.IsLessThanOrEqualTo:
                        parameters += filterMessage + "<=" + value;
                        break;
                    case FilterOperator.IsLessThan:
                        parameters += filterMessage + "<" + value;
                        break;
                    case FilterOperator.IsGreaterThanOrEqualTo:
                        parameters += filterMessage + ">=" + value;
                        break;
                    case FilterOperator.IsGreaterThan:
                        parameters += filterMessage + ">" + value;
                        break;
                    case FilterOperator.IsNull:
                        parameters += filterMessage + " IS NULL";
                        break;
                    case FilterOperator.IsNotNull:
                        parameters += filterMessage + " IS NOT NULL";
                        break;
                }
            }

            return parameters;
        }

        public static string ApplySort<T>(IList<SortDescriptor> model, string sort = "Id asc")
        {

            if (model.Count != 0)
            {
                sort = "";
                foreach (var sortDescriptor in model)
                {
                    string sortColumn = sortDescriptor.Serialize();
                    try
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(sortDescriptor.Member);
                        object[] attribute = propertyInfo.GetCustomAttributes(typeof(JoinAlias), true);
                        if (attribute.Length > 0)
                        {
                            JoinAlias myAttribute = (JoinAlias)attribute[0];
                            string joinAlias = myAttribute.Value;
                            if (!string.IsNullOrWhiteSpace(joinAlias))
                                sortColumn = String.Join('.', joinAlias, sortColumn);
                        }
                    }
                    catch (Exception) { }

                    sort += String.Concat(sortColumn, ",");
                }

                sort = sort.Replace("-", " ");
                sort = sort.Trim(new[] { ',' });
            }
            return sort;
        }
    }
}
