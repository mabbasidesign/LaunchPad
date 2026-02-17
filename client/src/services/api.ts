const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  message: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface Book {
  id: number;
  title: string;
  author: string;
  isbn?: string;
  price?: number;
  stock?: number;
  year: number;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  isbn: string;
  price: number;
  stock: number;
  year: number;
}

class ApiClient {
  private baseUrl: string;

  constructor() {
    this.baseUrl = API_URL;
  }

  async login(username: string, password: string): Promise<LoginResponse> {
    const response = await fetch(`${this.baseUrl}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });

    if (!response.ok) {
      throw new Error('Login failed');
    }

    return response.json();
  }

  async register(username: string, password: string): Promise<{ message: string }> {
    const response = await fetch(`${this.baseUrl}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });

    if (!response.ok) {
      throw new Error('Registration failed');
    }

    return response.json();
  }

  async getBooks(token: string): Promise<Book[]> {
    const response = await fetch(`${this.baseUrl}/v1.0/books`, {
      headers: { 
        'Authorization': `Bearer ${token}` 
      }
    });

    if (!response.ok) {
      throw new Error('Failed to fetch books');
    }

    return response.json();
  }

  async getBookById(id: number, token: string): Promise<Book> {
    const response = await fetch(`${this.baseUrl}/v1.0/books/${id}`, {
      headers: { 
        'Authorization': `Bearer ${token}` 
      }
    });

    if (!response.ok) {
      throw new Error('Failed to fetch book');
    }

    return response.json();
  }

  async createBook(book: CreateBookRequest, token: string): Promise<Book> {
    const response = await fetch(`${this.baseUrl}/v1.0/books`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(book)
    });

    if (!response.ok) {
      throw new Error('Failed to create book');
    }

    return response.json();
  }

  async updateBook(id: number, book: CreateBookRequest, token: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/v1.0/books/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(book)
    });

    if (!response.ok) {
      throw new Error('Failed to update book');
    }
  }

  async deleteBook(id: number, token: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/v1.0/books/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (!response.ok) {
      throw new Error('Failed to delete book');
    }
  }
}

export const apiClient = new ApiClient();
