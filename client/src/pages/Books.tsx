import { useState, useEffect } from 'react';
import { apiClient, type Book } from '../services/api';
import { LaunchPadLogo } from '../components/LaunchPadLogo';

export function Books() {
  const [books, setBooks] = useState<Book[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingBook, setEditingBook] = useState<Book | null>(null);
  const [formData, setFormData] = useState({
    title: '',
    author: '',
    isbn: '',
    year: new Date().getFullYear(),
    price: 0,
    stock: 0
  });
  const token = localStorage.getItem('token');
  const username = localStorage.getItem('username');

  useEffect(() => {
    if (!token) {
      window.location.href = '/';
      return;
    }

    loadBooks();
  }, [token]);

  const loadBooks = async () => {
    if (!token) return;

    try {
      setIsLoading(true);
      console.log('Fetching books from API...');
      const data = await apiClient.getBooks(token);
      console.log('Books fetched:', data);
      setBooks(data);
      setError('');
    } catch (err) {
      console.error('Error loading books:', err);
      setError(err instanceof Error ? err.message : 'Failed to load books');
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    window.location.href = '/';
  };

  const handleDelete = async (id: number) => {
    if (!token) return;
    if (!confirm('Are you sure you want to delete this book?')) return;

    try {
      await apiClient.deleteBook(id, token);
      await loadBooks(); // Reload list
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to delete book');
    }
  };

  const handleCreate = () => {
    setEditingBook(null);
    setFormData({
      title: '',
      author: '',
      isbn: '',
      year: new Date().getFullYear(),
      price: 0,
      stock: 0
    });
    setShowForm(true);
  };

  const handleEdit = (book: Book) => {
    setEditingBook(book);
    setFormData({
      title: book.title,
      author: book.author,
      isbn: book.isbn || '',
      year: book.year,
      price: book.price || 0,
      stock: book.stock || 0
    });
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token) return;

    try {
      if (editingBook) {
        console.log('Updating book:', editingBook.id, formData);
        await apiClient.updateBook(editingBook.id, formData, token);
        console.log('Book updated successfully');
      } else {
        console.log('Creating book:', formData);
        const newBook = await apiClient.createBook(formData, token);
        console.log('Book created successfully:', newBook);
      }
      setShowForm(false);
      console.log('Reloading books list...');
      await loadBooks();
      console.log('Books list reloaded');
    } catch (err) {
      console.error('Error saving book:', err);
      alert(err instanceof Error ? err.message : 'Failed to save book');
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingBook(null);
  };

  if (isLoading) {
    return <div style={{ padding: '20px', color: '#212529' }}>Loading books...</div>;
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f5f7fa' }}>
      <div style={{ 
        backgroundColor: '#ffffff',
        borderBottom: '2px solid #e1e8ed',
        padding: '20px 40px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.08)'
      }}>
        <div style={{ 
          maxWidth: '1200px',
          margin: '0 auto',
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'center'
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <LaunchPadLogo size={40} />
            <h1 style={{ 
              color: '#1a73e8', 
              margin: '0',
              fontSize: '28px',
              fontWeight: '600',
              letterSpacing: '-0.5px'
            }}>
              Books Library
            </h1>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
            <span style={{ 
              color: '#5f6368',
              fontSize: '15px',
              fontWeight: '500'
            }}>
              Welcome, <strong style={{ color: '#1a73e8' }}>{username}</strong>!
            </span>
            <button
              onClick={handleCreate}
              style={{
                padding: '10px 20px',
                backgroundColor: '#34a853',
                color: '#ffffff',
                border: 'none',
                borderRadius: '6px',
                cursor: 'pointer',
                fontSize: '14px',
                fontWeight: '500',
                transition: 'all 0.2s',
                boxShadow: '0 2px 4px rgba(52, 168, 83, 0.2)'
              }}
              onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#2d8e47'}
              onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#34a853'}
            >
              + Create Book
            </button>
            <button
              onClick={handleLogout}
              style={{
                padding: '10px 20px',
                backgroundColor: '#dc3545',
                color: '#ffffff',
                border: 'none',
                borderRadius: '6px',
                cursor: 'pointer',
                fontSize: '14px',
                fontWeight: '500',
                transition: 'all 0.2s',
                boxShadow: '0 2px 4px rgba(220, 53, 69, 0.2)'
              }}
              onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#c82333'}
              onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#dc3545'}
            >
              Logout
            </button>
          </div>
        </div>
      </div>
      
      <div style={{ padding: '30px 20px', maxWidth: '1200px', margin: '0 auto' }}>

      {error && (
        <div style={{ 
          color: 'red',
          padding: '10px',
          backgroundColor: '#ffe6e6',
          borderRadius: '4px',
          marginBottom: '20px'
        }}>
          {error}
        </div>
      )}

      {books.length === 0 ? (
        <div style={{ 
          textAlign: 'center', 
          padding: '60px 40px', 
          backgroundColor: '#ffffff',
          borderRadius: '12px',
          boxShadow: '0 2px 8px rgba(0,0,0,0.06)'
        }}>
          <p style={{ fontSize: '18px', color: '#5f6368', margin: '0' }}>No books found.</p>
        </div>
      ) : (
        <table style={{ 
          width: '100%', 
          borderCollapse: 'collapse',
          backgroundColor: '#ffffff',
          borderRadius: '12px',
          overflow: 'hidden',
          boxShadow: '0 2px 8px rgba(0,0,0,0.06)'
        }}>
          <thead>
            <tr style={{ backgroundColor: '#f8f9fa' }}>
              <th style={{ padding: '16px', textAlign: 'left', borderBottom: '2px solid #e1e8ed', color: '#5f6368', fontWeight: '600', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>ID</th>
              <th style={{ padding: '16px', textAlign: 'left', borderBottom: '2px solid #e1e8ed', color: '#5f6368', fontWeight: '600', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Title</th>
              <th style={{ padding: '16px', textAlign: 'left', borderBottom: '2px solid #e1e8ed', color: '#5f6368', fontWeight: '600', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Author</th>
              <th style={{ padding: '16px', textAlign: 'left', borderBottom: '2px solid #e1e8ed', color: '#5f6368', fontWeight: '600', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Year</th>
              <th style={{ padding: '16px', textAlign: 'left', borderBottom: '2px solid #e1e8ed', color: '#5f6368', fontWeight: '600', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {books.map((book) => (
              <tr key={book.id} style={{ borderBottom: '1px solid #f0f0f0', transition: 'background-color 0.2s' }}>
                <td style={{ padding: '16px', color: '#5f6368', fontSize: '14px', fontWeight: '500' }}>{book.id}</td>
                <td style={{ padding: '16px', color: '#202124', fontSize: '15px', fontWeight: '500' }}>{book.title}</td>
                <td style={{ padding: '16px', color: '#5f6368', fontSize: '14px' }}>{book.author}</td>
                <td style={{ padding: '16px', color: '#5f6368', fontSize: '14px' }}>{book.year}</td>
                <td style={{ padding: '16px' }}>
                  <div style={{ display: 'flex', gap: '8px' }}>
                    <button
                      onClick={() => handleEdit(book)}
                      style={{
                        padding: '6px 12px',
                        backgroundColor: '#1a73e8',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer',
                        fontSize: '14px',
                        fontWeight: '500'
                      }}
                      onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#1557b0'}
                      onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#1a73e8'}
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(book.id)}
                      style={{
                        padding: '6px 12px',
                        backgroundColor: '#dc3545',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer',
                        fontSize: '14px',
                        fontWeight: '500'
                      }}
                      onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#c82333'}
                      onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#dc3545'}
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
      </div>

      {/* Create/Edit Book Modal */}
      {showForm && (
        <div 
          onClick={handleCancel}
          style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'flex-start',
            paddingTop: '80px',
            zIndex: 1000,
            overflowY: 'auto'
          }}
        >
          <div 
            onClick={(e) => e.stopPropagation()}
            style={{
              backgroundColor: '#ffffff',
              borderRadius: '12px',
              padding: '30px',
              maxWidth: '500px',
              width: '90%',
              marginBottom: '40px',
              boxShadow: '0 8px 32px rgba(0,0,0,0.2)'
            }}
          >
            <h2 style={{ 
              color: '#202124', 
              marginTop: 0, 
              marginBottom: '20px',
              fontSize: '24px',
              fontWeight: '600'
            }}>
              {editingBook ? 'Edit Book' : 'Create New Book'}
            </h2>
            
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <div>
                <label style={{ 
                  display: 'block', 
                  marginBottom: '6px', 
                  color: '#5f6368',
                  fontSize: '14px',
                  fontWeight: '500'
                }}>
                  Title *
                </label>
                <input
                  type="text"
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                  required
                  style={{
                    width: '100%',
                    padding: '10px 12px',
                    border: '1px solid #dadce0',
                    borderRadius: '6px',
                    fontSize: '15px',
                    color: '#202124',
                    backgroundColor: '#ffffff',
                    boxSizing: 'border-box'
                  }}
                />
              </div>

              <div>
                <label style={{ 
                  display: 'block', 
                  marginBottom: '6px', 
                  color: '#5f6368',
                  fontSize: '14px',
                  fontWeight: '500'
                }}>
                  Author *
                </label>
                <input
                  type="text"
                  value={formData.author}
                  onChange={(e) => setFormData({ ...formData, author: e.target.value })}
                  required
                  style={{
                    width: '100%',
                    padding: '10px 12px',
                    border: '1px solid #dadce0',
                    borderRadius: '6px',
                    fontSize: '15px',
                    color: '#202124',
                    backgroundColor: '#ffffff',
                    boxSizing: 'border-box'
                  }}
                />
              </div>

              <div>
                <label style={{ 
                  display: 'block', 
                  marginBottom: '6px', 
                  color: '#5f6368',
                  fontSize: '14px',
                  fontWeight: '500'
                }}>
                  ISBN *
                </label>
                <input
                  type="text"
                  value={formData.isbn}
                  onChange={(e) => setFormData({ ...formData, isbn: e.target.value })}
                  required
                  style={{
                    width: '100%',
                    padding: '10px 12px',
                    border: '1px solid #dadce0',
                    borderRadius: '6px',
                    fontSize: '15px',
                    color: '#202124',
                    backgroundColor: '#ffffff',
                    boxSizing: 'border-box'
                  }}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
                <div>
                  <label style={{ 
                    display: 'block', 
                    marginBottom: '6px', 
                    color: '#5f6368',
                    fontSize: '14px',
                    fontWeight: '500'
                  }}>
                    Year *
                  </label>
                  <input
                    type="number"
                    value={formData.year}
                    onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
                    required
                    style={{
                      width: '100%',
                      padding: '10px 12px',
                      border: '1px solid #dadce0',
                      borderRadius: '6px',
                      fontSize: '15px',
                      color: '#202124',
                      backgroundColor: '#ffffff',
                      boxSizing: 'border-box'
                    }}
                  />
                </div>

                <div>
                  <label style={{ 
                    display: 'block', 
                    marginBottom: '6px', 
                    color: '#5f6368',
                    fontSize: '14px',
                    fontWeight: '500'
                  }}>
                    Price *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={formData.price}
                    onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) })}
                    required
                    style={{
                      width: '100%',
                      padding: '10px 12px',
                      border: '1px solid #dadce0',
                      borderRadius: '6px',
                      fontSize: '15px',
                      color: '#202124',
                      backgroundColor: '#ffffff',
                      boxSizing: 'border-box'
                    }}
                  />
                </div>
              </div>

              <div>
                <label style={{ 
                  display: 'block', 
                  marginBottom: '6px', 
                  color: '#5f6368',
                  fontSize: '14px',
                  fontWeight: '500'
                }}>
                  Stock *
                </label>
                <input
                  type="number"
                  value={formData.stock}
                  onChange={(e) => setFormData({ ...formData, stock: parseInt(e.target.value) })}
                  required
                  style={{
                    width: '100%',
                    padding: '10px 12px',
                    border: '1px solid #dadce0',
                    borderRadius: '6px',
                    fontSize: '15px',
                    color: '#202124',
                    backgroundColor: '#ffffff',
                    boxSizing: 'border-box'
                  }}
                />
              </div>

              <div style={{ 
                display: 'flex', 
                gap: '12px', 
                marginTop: '20px',
                justifyContent: 'flex-end'
              }}>
                <button
                  type="button"
                  onClick={handleCancel}
                  style={{
                    padding: '10px 24px',
                    backgroundColor: '#f1f3f4',
                    color: '#5f6368',
                    border: 'none',
                    borderRadius: '6px',
                    cursor: 'pointer',
                    fontSize: '14px',
                    fontWeight: '500'
                  }}
                  onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#e8eaed'}
                  onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#f1f3f4'}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  style={{
                    padding: '10px 24px',
                    backgroundColor: '#1a73e8',
                    color: '#ffffff',
                    border: 'none',
                    borderRadius: '6px',
                    cursor: 'pointer',
                    fontSize: '14px',
                    fontWeight: '500'
                  }}
                  onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#1557b0'}
                  onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#1a73e8'}
                >
                  {editingBook ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
