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

export const loansAPI = {
  getAll: () => api.get('/loans'),
  getById: (id) => api.get(`/loans/${id}`),
  create: (data) => api.post('/loans', data),
  returnLoan: (id) => api.post(`/loans/${id}/return`),
  getUserLoans: (userId) => api.get(`/loans/user/${userId}`),
  getOverdue: () => api.get('/loans/overdue'),
};

export default api;
