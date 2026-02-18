import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { statsApi } from '../../api/statsApi';
import type { DashboardStats } from '../../types';

interface StatsState {
  dashboard: DashboardStats | null;
  isLoading: boolean;
  error: string | null;
}

const initialState: StatsState = {
  dashboard: null,
  isLoading: false,
  error: null,
};

export const fetchDashboardStats = createAsyncThunk(
  'stats/fetchDashboard',
  async (_, { rejectWithValue }) => {
    try {
      const response = await statsApi.getDashboardStats();
      return response.data;
    } catch (error: any) {
      return rejectWithValue('Failed to fetch stats');
    }
  }
);

const statsSlice = createSlice({
  name: 'stats',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchDashboardStats.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(fetchDashboardStats.fulfilled, (state, action) => {
        state.isLoading = false;
        state.dashboard = action.payload;
      })
      .addCase(fetchDashboardStats.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      });
  },
});

export default statsSlice.reducer;