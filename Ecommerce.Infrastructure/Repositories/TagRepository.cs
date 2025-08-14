using Azure;
using Ecommerce.Application.Common.Utils;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class TagRepository: ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tags> AddAsync(Tags tag)
        {
            if (await ExistsAsync(tag.TagName))
                throw new InvalidOperationException($"Tag '{tag.TagName}' already exists");

            tag.Slug = StringUtils.Slugify(tag.TagName);
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task AddTagsToProductAsync(int productId, IEnumerable<int> tagIds)
        {
            var existingTags = await _context.ProductTags
                .Where(pt => pt.ProductID == productId)
                .Select(pt => pt.TagID)
                .ToListAsync();

            var tagsToAdd = tagIds.Except(existingTags);

            foreach (var tagId in tagsToAdd)
            {
                await AddTagToProductAsync(productId, tagId);
            }
        }

        public async Task AddTagToProductAsync(int productId, int tagId)
        {
            if (await _context.ProductTags.AnyAsync(pt => pt.ProductID == productId && pt.TagID == tagId))
                return;

            var productTag = new ProductTags
            {
                ProductID = productId,
                TagID = tagId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductTags.Add(productTag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int tagId)
        {
            var existingTag = await _context.Tags.FindAsync(tagId);
            if (existingTag == null)
                throw new KeyNotFoundException("Tag not found");

            _context.Tags.Remove(existingTag);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string tagName)
        {
            return await _context.Tags
                .AnyAsync(t => t.TagName.ToLower() == tagName.ToLower());
        }

        public async Task<IReadOnlyList<Tags>> GetAllAsync()
        {
            return await _context.Tags
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tags> GetByIdAsync(int id)
        {
            return await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TagID == id);
        }

        public async Task<Tags> GetBySlugAsync(string slug)
        {
            return await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug.ToLowerInvariant());
        }

        public async Task<IReadOnlyList<Tags>> GetPopularTagsAsync(int count)
        {
            return await _context.ProductTags
                .GroupBy(pt => pt.TagID)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.First().Tags)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Products>> GetProductsByTagAsync(int tagId)
        {
            return await _context.ProductTags
                .Where(pt => pt.TagID == tagId)
                .Select(pt => pt.Products)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Tags>> GetTagsByProductAsync(int productId)
        {
            return await _context.ProductTags
                .Where(pt => pt.ProductID == productId)
                .Select(pt => new Tags
                {
                    TagID = pt.Tags.TagID,
                    TagName = pt.Tags.TagName,
                    Slug = pt.Tags.Slug
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsInUseAsync(int tagId)
        {
            return await _context.ProductTags
                .AnyAsync(pt => pt.TagID == tagId);
        }

        public async Task RemoveTagFromProductAsync(int productId, int tagId)
        {
            var productTag = await _context.ProductTags
                .FirstOrDefaultAsync(pt => pt.ProductID == productId && pt.TagID == tagId);

            if (productTag == null)
                return; 

            _context.ProductTags.Remove(productTag);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Tags>> SearchAsync(string keyword)
        {
            return await _context.Tags
                .Where(t => t.TagName.Contains(keyword) || t.Slug.Contains(keyword))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SyncProductTagsAsync(int productId, IEnumerable<int> tagIds)
        {
            var currentTags = await _context.ProductTags
                .Where(pt => pt.ProductID == productId)
                .ToListAsync();

            // Remove tags that are not in the new list
            var tagsToRemove = currentTags.Where(pt => !tagIds.Contains(pt.TagID));
            _context.ProductTags.RemoveRange(tagsToRemove);

            // Add new tags that aren't already associated
            var existingTagIds = currentTags.Select(pt => pt.TagID);
            var tagsToAdd = tagIds.Except(existingTagIds)
                .Select(tagId => new ProductTags
                {
                    ProductID = productId,
                    TagID = tagId,
                    CreatedAt = DateTime.UtcNow
                });

            _context.ProductTags.AddRange(tagsToAdd);
            await _context.SaveChangesAsync();
        }

        public async Task<Tags> UpdateAsync(Tags tag)
        {
            var existingTag = await _context.Tags.FindAsync(tag.TagID);
            if (existingTag == null)
                throw new KeyNotFoundException("Tag not found");

            existingTag.TagName = tag.TagName;
            existingTag.Slug = StringUtils.Slugify(tag.TagName);

            _context.Tags.Update(existingTag);
            await _context.SaveChangesAsync();
            return existingTag;
        }
    }
}
