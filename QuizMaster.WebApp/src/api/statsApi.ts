import axiosInstance from './axiosInstance';
import type { ApiResponse, DashboardStats } from '../types';

export const statsApi = {
  getDashboardStats: async () => {
    const response = await axiosInstance.get<ApiResponse<DashboardStats>>(
      '/stats/dashboard'
    );
    return response.data;
  },
};