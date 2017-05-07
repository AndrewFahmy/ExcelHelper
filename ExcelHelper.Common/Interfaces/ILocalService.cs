namespace ExcelHelper.Common.Interfaces
{
    public interface ILocalService
    {
        IConnectionsService ConnectionsService { get; }
        IOptionsService OptionsService { get; }

        ISnippetService SnippetsService { get; }
    }
}