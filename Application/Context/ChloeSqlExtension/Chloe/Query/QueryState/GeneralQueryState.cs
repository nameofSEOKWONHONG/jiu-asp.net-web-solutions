
namespace Chloe.Query.QueryState
{
    class GeneralQueryState : QueryStateBase, IQueryState
    {
        public GeneralQueryState(QueryModel queryModel)
            : base(queryModel)
        {
        }

        public override QueryModel ToFromQueryModel()
        {
            QueryModel newQueryModel = new QueryModel(this.QueryModel.ScopeParameters, this.QueryModel.ScopeTables, this.QueryModel.IgnoreFilters);
            newQueryModel.FromTable = this.QueryModel.FromTable;
            newQueryModel.ResultModel = this.QueryModel.ResultModel;
            newQueryModel.Condition = this.QueryModel.Condition;
            if (!this.QueryModel.IgnoreFilters)
            {
                newQueryModel.GlobalFilters.AddRange(this.QueryModel.GlobalFilters);
                newQueryModel.ContextFilters.AddRange(this.QueryModel.ContextFilters);
            }

            return newQueryModel;
        }

    }
}
