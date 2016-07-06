using System;
using BlogData.Data;

namespace BlogData.DAL
{
    public class UnitOfWork : IDisposable
    {
        //private variables
        private readonly BlogContext _context = new BlogContext();
        private bool _disposed;

        //private generic repository
        private GenericRepository<Content> _contentRepository;
        private ContentHistoryDal _contentHistoryRepository;
        private GenericRepository<ContentState> _contentStateRepository;
        private ContentStateToRoleDal _contentStateToRoleRepository;
        private AuthorDal _authorRepository; 
       
        //initialize public repository
        public GenericRepository<Content> ContentRepository => _contentRepository ?? (_contentRepository = new GenericRepository<Content>(_context));
        public ContentHistoryDal ContentHistoryRepository => _contentHistoryRepository ?? (_contentHistoryRepository = new ContentHistoryDal(_context));
        public GenericRepository<ContentState> ContentStateRepository => _contentStateRepository ?? (_contentStateRepository = new GenericRepository<ContentState>(_context));
        public ContentStateToRoleDal ContentStateToRoleRepository => _contentStateToRoleRepository ?? (_contentStateToRoleRepository = new ContentStateToRoleDal(_context));

        public AuthorDal AuthorRepository => _authorRepository ?? (_authorRepository = new AuthorDal(_context)); 

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed && disposing)
                _context.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
