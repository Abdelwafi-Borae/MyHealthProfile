using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawtheiq.Application.Cores.General.Dtos
{
    public class MetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData;

        public PagedList(List<T> item, int count, int pagenumber, int pagesize)
        {
            MetaData = new MetaData
            {
                PageSize = pagesize,
                CurrentPage = pagenumber,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pagesize)
            };
            AddRange(item);
        }
        //ApplicationUser,GetUsersDto
        public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query, int pagenumber, int pagesize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToListAsync();
            var x= new PagedList<T>(items, count, pagenumber, pagesize);
            
            return x;
        }
    }
}
