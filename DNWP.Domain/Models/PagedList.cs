using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Domain.Models;

public class PagedList<T>
{
    public int CurrentPage { get; private set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    public List<T> Data { get; set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < TotalPages;

    public PagedList()
    {
    }

    //public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    //{
    //    TotalCount = count;
    //    PageSize = pageSize;
    //    CurrentPage = pageNumber;
    //    TotalPages = (int)Math.Ceiling((double)count / (double)pageSize);
    //    AddRange(items);
    //}

    //public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    //{
    //    return new PagedList<T>(count: await source.CountAsync(), items: await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(), pageNumber: pageNumber, pageSize: pageSize);
    //}

    //public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    //{
    //    int count = source.Count();
    //    IEnumerable<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    //    return new PagedList<T>(items, count, pageNumber, pageSize);
    //}

    //public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
    //{
    //    int count = source.Count();
    //    IQueryable<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    //    return new PagedList<T>(items, count, pageNumber, pageSize);
    //}
}
