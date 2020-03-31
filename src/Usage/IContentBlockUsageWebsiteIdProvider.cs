namespace Perplex.ContentBlocks.Usage
{
    public interface IContentBlockUsageWebsiteIdProvider
    {
        int? GetWebsiteId(int pageId);
    }
}
