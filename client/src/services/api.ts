import axios from 'axios';

const API_BASE_URL = (import.meta as any).env?.VITE_API_BASE_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authApi = {
  register: (data: { fullName: string; email: string; password: string }) =>
    api.post('/auth/register', data),
  login: (data: { email: string; password: string }) =>
    api.post('/auth/login', data),
  getCurrentUser: () => api.get('/auth/me'),
};

export const meetingsApi = {
  getAll: (page = 1, pageSize = 10) =>
    api.get(`/meetings?page=${page}&pageSize=${pageSize}`),
  getById: (id: string) => api.get(`/meetings/${id}`),
  create: (data: { title: string; description?: string; meetingDate?: string }) =>
    api.post('/meetings', data),
  delete: (id: string) => api.delete(`/meetings/${id}`),
  search: (term: string) => api.get(`/meetings/search?searchTerm=${term}`),
  upload: (formData: FormData) =>
    api.post('/meetings', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }),
};

export const aiApi = {
  processMeeting: (meetingId: string) =>
    api.post(`/ai/meetings/${meetingId}/process`),
  chat: (meetingId: string, message: string) =>
    api.post(`/ai/meetings/${meetingId}/chat`, { message }),
  getChatHistory: (meetingId: string) =>
    api.get(`/ai/meetings/${meetingId}/chat`),
};

export const dashboardApi = {
  getStats: () => api.get('/dashboard/overview'),
};

export const actionItemsApi = {
  getAll: (meetingId?: string) =>
    api.get(`/actionItems${meetingId ? `?meetingId=${meetingId}` : ''}`),
  update: (id: string, data: { status?: string; priority?: string }) =>
    api.put(`/actionItems/${id}`, data),
};

export default api;
