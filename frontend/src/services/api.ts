import axios, { AxiosResponse } from 'axios';
import { Destination, Customer, Guide, Tour, Booking, LoginRequest, RegisterRequest, AuthResponse, UserInfo, RefreshTokenRequest } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:7001/api';
const AUTH_BASE_URL = process.env.REACT_APP_AUTH_URL || 'http://localhost:7002/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Token management
const TOKEN_KEY = 'auth_token';
const REFRESH_TOKEN_KEY = 'refresh_token';

// Add token to requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Handle token refresh on 401 responses
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
      if (refreshToken) {
        try {
          const response = await axios.post(`${API_BASE_URL}/auth/refresh`, {
            refreshToken
          });
          
          const { token, refreshToken: newRefreshToken } = response.data;
          localStorage.setItem(TOKEN_KEY, token);
          localStorage.setItem(REFRESH_TOKEN_KEY, newRefreshToken);
          
          // Retry the original request
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        } catch (refreshError) {
          // Refresh failed, redirect to login
          localStorage.removeItem(TOKEN_KEY);
          localStorage.removeItem(REFRESH_TOKEN_KEY);
          window.location.href = '/login';
        }
      } else {
        // No refresh token, redirect to login
        window.location.href = '/login';
      }
    }
    
    return Promise.reject(error);
  }
);

// Auth API - uses separate auth service
const authApi = axios.create({
  baseURL: AUTH_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authService = {
  login: (credentials: LoginRequest): Promise<AxiosResponse<AuthResponse>> =>
    authApi.post('/auth/login', credentials),
  
  register: (userData: RegisterRequest): Promise<AxiosResponse<AuthResponse>> =>
    authApi.post('/auth/register', userData),
  
  refreshToken: (refreshToken: string): Promise<AxiosResponse<AuthResponse>> =>
    authApi.post('/auth/refresh', { refreshToken }),
  
  getCurrentUser: (): Promise<AxiosResponse<UserInfo>> =>
    authApi.get('/auth/me'),
  
  logout: () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  },
  
  isAuthenticated: (): boolean => {
    return !!localStorage.getItem(TOKEN_KEY);
  },
  
  getToken: (): string | null => {
    return localStorage.getItem(TOKEN_KEY);
  }
};

// Destination API
export const destinationApi = {
  getAll: () => api.get<Destination[]>('/destinations'),
  getById: (id: number) => api.get<Destination>(`/destinations/${id}`),
  create: (destination: Omit<Destination, 'id' | 'createdAt' | 'updatedAt'>) => 
    api.post<Destination>('/destinations', destination),
  update: (id: number, destination: Partial<Destination>) => 
    api.put<Destination>(`/destinations/${id}`, destination),
  delete: (id: number) => api.delete(`/destinations/${id}`),
};

// Customer API
export const customerApi = {
  getAll: () => api.get<Customer[]>('/customers'),
  getById: (id: number) => api.get<Customer>(`/customers/${id}`),
  create: (customer: Omit<Customer, 'id' | 'createdAt' | 'updatedAt'>) => 
    api.post<Customer>('/customers', customer),
  update: (id: number, customer: Partial<Customer>) => 
    api.put<Customer>(`/customers/${id}`, customer),
  delete: (id: number) => api.delete(`/customers/${id}`),
};

// Guide API
export const guideApi = {
  getAll: () => api.get<Guide[]>('/guides'),
  getById: (id: number) => api.get<Guide>(`/guides/${id}`),
  create: (guide: Omit<Guide, 'id' | 'createdAt' | 'updatedAt'>) => 
    api.post<Guide>('/guides', guide),
  update: (id: number, guide: Partial<Guide>) => 
    api.put<Guide>(`/guides/${id}`, guide),
  delete: (id: number) => api.delete(`/guides/${id}`),
};

// Tour API
export const tourApi = {
  getAll: () => api.get<Tour[]>('/tours'),
  getById: (id: number) => api.get<Tour>(`/tours/${id}`),
  create: (tour: Omit<Tour, 'id' | 'createdAt' | 'updatedAt'>) => 
    api.post<Tour>('/tours', tour),
  update: (id: number, tour: Partial<Tour>) => 
    api.put<Tour>(`/tours/${id}`, tour),
  delete: (id: number) => api.delete(`/tours/${id}`),
};

// Booking API
export const bookingApi = {
  getAll: () => api.get<Booking[]>('/bookings'),
  getById: (id: number) => api.get<Booking>(`/bookings/${id}`),
  create: (booking: Omit<Booking, 'id' | 'createdAt' | 'updatedAt'>) => 
    api.post<Booking>('/bookings', booking),
  update: (id: number, booking: Partial<Booking>) => 
    api.put<Booking>(`/bookings/${id}`, booking),
  delete: (id: number) => api.delete(`/bookings/${id}`),
};

export default api; 