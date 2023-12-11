using Library.DTO;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Library.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        readonly LibraryDbContext _context;
        public LibraryRepository(LibraryDbContext context) 
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Books>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<IEnumerable<Libraries>> GetAllLibraries()
        {
            return await _context.Libraries.ToListAsync();
        }

        public async Task<BookResponse> GetBookByGuid(Guid guid)
        {
            var book = await _context.Books.FirstOrDefaultAsync(e => e.Book_uid.Equals(guid));
            if (book == null)
            {
                return null;
            }
            return new BookResponse
            {
                Author = book.Author,
                Book_uid = book.Book_uid,
                Genre = book.Genre,
                Name = book.Name
            };
        }

        public async Task<Books> GetBookById(int Id)
        {
            return await _context.Books.FirstOrDefaultAsync(e => e.Id.Equals(Id));
        }

        public async Task<PaginationResponse<LibraryResponse>> GetCityLibraries(int? page, int? size, string city)
        {
            var query = _context.Libraries.Where(l => l.City == city);
            var total = await query.CountAsync();

            if (page.HasValue && size.HasValue)
            {
                query = query.OrderBy(l => l.Id).Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            var libraries = await query.ToListAsync();

            var libs = new List<LibraryResponse>();
            foreach (var library in libraries)
            {
                libs.Add(new LibraryResponse { Address = library.Address,
                                               City = library.City,
                                               Name = library.Name,
                                               Library_uid = library.Library_uid});
            }

            var response = new PaginationResponse<LibraryResponse>() 
            {
                Page = page,
                PageSize = size,
                TotalElements = total,
                Items = libs
            };

            return response;
        }

        public async Task<PaginationResponse<LibraryBookResponse>> GetLibraryBooks(int? page, int? size, Guid libraryGuid, bool? allShow = false)
        {
            var queryLibrary = await _context.Libraries.FirstOrDefaultAsync(e => e.Library_uid.Equals(libraryGuid));

            if (queryLibrary == null)
            {
                return null;
            }
            //Получение книг, привязанных к библиотеке
            var lbs = _context.LibraryBooks.Where(lb => lb.Library_id.Equals(queryLibrary.Id)).ToList();
            var books = from b in _context.Books.AsEnumerable() 
                        join lb in lbs on 
                        b.Id equals lb.Book_id 
                        select new LibraryBookResponse 
                        { 
                            Book_uid = b.Book_uid,
                            Author = b.Author,
                            Name = b.Name,
                            Condition = b.Condition,
                            Genre = b.Genre,
                            AvailableCount = lb.Available_count
                        };

            if (page.HasValue && size.HasValue)
            {
                books = books.OrderBy(l => l.Book_uid).Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            if (allShow == false) 
            {
                books = books.Where(b => b.AvailableCount > 0);
            }

            var total = books.Count();

            return new PaginationResponse<LibraryBookResponse>
            {
                Page = page,
                PageSize = size,
                TotalElements = total,
                Items = books
            };
        }

        public async Task<LibraryResponse> GetLibraryByGuid(Guid guid)
        {
            var library = await _context.Libraries.FirstOrDefaultAsync(e => e.Library_uid.Equals(guid));
            if (library == null) 
            {
                return null;
            }
            return new LibraryResponse
            {
                Address = library.Address,
                City = library.City,
                Name = library.Name,
                Library_uid = library.Library_uid
            };
        }

        public async Task<Libraries> GetLibraryById(int Id)
        {
            return await _context.Libraries.FirstOrDefaultAsync(e => e.Id.Equals(Id)).ConfigureAwait(false);
        }

        public void RentBook(LibraryBooks libraryBooks)
        {
            _context.LibraryBooks.Update(libraryBooks);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<LibraryBooks> GetLibraryBookAsync(Guid bookGuid, Guid libraryGuid)
        {
            var lb =  await _context.LibraryBooks.Where(lb => lb.Library.Library_uid == libraryGuid && lb.Book.Book_uid == bookGuid).FirstOrDefaultAsync();
            return lb != null ? lb : null;
        }

        //TODO
        public async Task<bool> CheckLibraryBookCount(Guid bookGuid, Guid libraryGuid)
        {
            var libraryBook = await _context.LibraryBooks.Where(lb => lb.Library.Library_uid == libraryGuid && lb.Book.Book_uid == bookGuid).FirstOrDefaultAsync();

            return libraryBook?.Available_count > 0;
        }
    }
}
