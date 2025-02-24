using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class TagDAO
    {
        private readonly FunewsManagementContext _context;

        public TagDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả thẻ
        public async Task<List<Tag>> GetAllTags()
        {
            return await _context.Tags.ToListAsync();
        }

        // Lấy thẻ theo ID
        public async Task<Tag?> GetTagById(int tagId)
        {
            return await _context.Tags.FindAsync(tagId);
        }

        // Tạo mới thẻ
        public async Task<Tag> CreateTag(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        // Cập nhật thẻ
        public async Task<Tag?> UpdateTag(Tag tag)
        {
            var existingTag = await _context.Tags.FindAsync(tag.TagId);
            if (existingTag != null)
            {
                _context.Entry(existingTag).CurrentValues.SetValues(tag);
                await _context.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }

        // Xóa thẻ
        public async Task<bool> DeleteTag(int tagId)
        {
            var tag = await _context.Tags.FindAsync(tagId);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}