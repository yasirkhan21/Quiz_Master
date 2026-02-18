import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../feature/auth/authSlice';
import quizReducer from '../feature/quiz/quizSlice';
import statsReducer from '../feature/stats/statsSlice';
export const store = configureStore({
  reducer: {
    auth: authReducer,
    quiz: quizReducer,
    stats: statsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;