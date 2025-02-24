using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTags();
        Task<Tag?> GetTagById(int tagId);
        Task<Tag> CreateTag(Tag tag);
        Task<Tag?> UpdateTag(Tag tag);
        Task<bool> DeleteTag(int tagId);
    }
}