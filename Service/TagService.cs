using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<Tag>> GetAllTags()
        {
            return await _tagRepository.GetAllTags();
        }

        public async Task<Tag?> GetTagById(int tagId)
        {
            return await _tagRepository.GetTagById(tagId);
        }

        public async Task<Tag> CreateTag(Tag tag)
        {
            if (string.IsNullOrEmpty(tag.TagName))
            {
                throw new ArgumentException("Tag name is required.");
            }
            return await _tagRepository.CreateTag(tag);
        }

        public async Task<Tag?> UpdateTag(Tag tag)
        {
            var existingTag = await _tagRepository.GetTagById(tag.TagId);
            if (existingTag == null)
            {
                return null;
            }
            return await _tagRepository.UpdateTag(tag);
        }

        public async Task<bool> DeleteTag(int tagId)
        {
            var tag = await _tagRepository.GetTagById(tagId);
            if (tag == null)
            {
                return false;
            }
            return await _tagRepository.DeleteTag(tagId);
        }
        public bool TagExists(int id)
        {
            return _tagRepository.TagExists(id);
        }
    }
}