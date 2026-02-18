import axiosInstance from './axiosInstance';
import type { ApiResponse, AuthResponse } from '../types';

export const authApi = {
  register: async (data: {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
  }) => {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/register',
      data
    );
    return response.data;
  },

  login: async (data: { email: string; password: string }) => {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/login',
      data
    );
    return response.data;
  },

  getMe: async () => {
    const response = await axiosInstance.get<ApiResponse<AuthResponse>>(
      '/auth/me'
    );
    return response.data;
  },
};