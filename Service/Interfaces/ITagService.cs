using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllTags();
        Task<Tag?> GetTagById(int tagId);
        Task<Tag> CreateTag(Tag tag);
        Task<Tag?> UpdateTag(Tag tag);
        Task<bool> DeleteTag(int tagId);
    }
}