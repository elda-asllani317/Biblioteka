import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests if available
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export const authAPI = {
  login: (data) => api.post('/auth/login', data),
  register: (data) => api.post('/auth/register', data),
};

export const booksAPI = {
  getAll: () => api.get('/books'),
  getById: (id) => api.get(`/books/${id}`),
  create: (data) => api.post('/books', data),
  update: (id, data) => api.put(`/books/${id}`, data),
  delete: (id) => api.delete(`/books/${id}`),
  search: (term) => api.get(`/books/search?term=${term}`),
};

export const authorsAPI = {
  getAll: () => api.get('/authors'),
  getById: (id) => api.get(`/authors/${id}`),
  create: (data) => api.post('/authors', data),
  update: (id, data) => api.put(`/authors/${id}`, data),
  delete: (id) => api.delete(`/authors/${id}`),
};

export const categoriesAPI = {
  getAll: () => api.get('/categories'),
  getById: (id) => api.get(`/categories/${id}`),
  create: (data) => api.post('/categories', data),
  update: (id, data) => api.put(`/categories/${id}`, data),
  delete: (id) => api.delete(`/categories/${id}`),
};

export const publishersAPI = {
  getAll: (params) => api.get('/publishers', { params }),
  getById: (id) => api.get(`/publishers/${id}`),
  create: (data) => api.post('/publishers', data),
  update: (id, data) => api.put(`/publishers/${id}`, data),
  delete: (id) => api.delete(`/publishers/${id}`),
};

export const loansAPI = {
  getAll: () => api.get('/loans'),
  getById: (id) => api.get(`/loans/${id}`),
  create: (data) => api.post('/loans', data),
   update: (id, data) => api.put(`/loans/${id}`, data),
   delete: (id) => api.delete(`/loans/${id}`),
  returnLoan: (id) => api.post(`/loans/${id}/return`),
  getUserLoans: (userId) => api.get(`/loans/user/${userId}`),
  getOverdue: () => api.get('/loans/overdue'),
};

export const usersAPI = {
  getAll: () => api.get('/users'),
  getById: (id) => api.get(`/users/${id}`),
  create: (data) => api.post('/users', data),
  update: (id, data) => api.put(`/users/${id}`, data),
  delete: (id) => api.delete(`/users/${id}`),
};

export const bookCopiesAPI = {
  getAll: () => api.get('/bookcopies'),
  getById: (id) => api.get(`/bookcopies/${id}`),
  getByBookId: (bookId) => api.get(`/bookcopies/book/${bookId}`),
  getAvailable: () => api.get('/bookcopies/available'),
  create: (data) => api.post('/bookcopies', data),
  update: (id, data) => api.put(`/bookcopies/${id}`, data),
  delete: (id) => api.delete(`/bookcopies/${id}`),
};

export const finesAPI = {
  getAll: () => api.get('/fines'),
  getById: (id) => api.get(`/fines/${id}`),
  getByUserId: (userId) => api.get(`/fines/user/${userId}`),
  getByStatus: (status) => api.get(`/fines/status/${status}`),
  create: (data) => api.post('/fines', data),
  update: (id, data) => api.put(`/fines/${id}`, data),
  delete: (id) => api.delete(`/fines/${id}`),
};

export default api;
