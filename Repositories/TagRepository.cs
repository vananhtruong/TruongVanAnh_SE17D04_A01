using BusinessObjects.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly TagDAO _tagDAO;

        public TagRepository(TagDAO tagDAO)
        {
            _tagDAO = tagDAO;
        }

        public async Task<List<Tag>> GetAllTags()
        {
            return await _tagDAO.GetAllTags();
        }

        public async Task<Tag?> GetTagById(int tagId)
        {
            return await _tagDAO.GetTagById(tagId);
        }

        public async Task<Tag> CreateTag(Tag tag)
        {
            return await _tagDAO.CreateTag(tag);
        }

        public async Task<Tag?> UpdateTag(Tag tag)
        {
            return await _tagDAO.UpdateTag(tag);
        }

        public async Task<bool> DeleteTag(int tagId)
        {
            return await _tagDAO.DeleteTag(tagId);
        }
        public bool TagExists(int id)
        {
            return _tagDAO.TagExists(id);
        }
    }
}