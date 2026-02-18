import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { quizApi } from '../../api/quizApi';
import type { Quiz, QuizSummary, AnswerResult, CreateQuizPayload, SubmitAnswerPayload } from '../../types';

interface QuizState {
  currentQuiz: Quiz | null;
  currentQuestionIndex: number;
  quizHistory: QuizSummary[];
  answerResult: AnswerResult | null;
  isLoading: boolean;
  isSubmitting: boolean;
  error: string | null;
  categories: string[];
  quizCompleted: boolean;
  finalSummary: QuizSummary | null;
}

const initialState: QuizState = {
  currentQuiz: null,
  currentQuestionIndex: 0,
  quizHistory: [],
  answerResult: null,
  isLoading: false,
  isSubmitting: false,
  error: null,
  categories: [],
  quizCompleted: false,
  finalSummary: null,
};

export const fetchCategories = createAsyncThunk(
  'quiz/fetchCategories',
  async (_, { rejectWithValue }) => {
    try {
      const response = await quizApi.getCategories();
      return response.data;
    } catch (error: any) {
      return rejectWithValue('Failed to fetch categories');
    }
  }
);

export const createQuiz = createAsyncThunk(
  'quiz/create',
  async (payload: CreateQuizPayload, { rejectWithValue }) => {
    try {
      const response = await quizApi.createQuiz(payload);
      if (!response.success) return rejectWithValue(response.message);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create quiz');
    }
  }
);

export const submitAnswer = createAsyncThunk(
  'quiz/submitAnswer',
  async (payload: SubmitAnswerPayload, { rejectWithValue }) => {
    try {
      const response = await quizApi.submitAnswer(payload);
      if (!response.success) return rejectWithValue(response.message);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to submit answer');
    }
  }
);

export const completeQuiz = createAsyncThunk(
  'quiz/complete',
  async (quizId: string, { rejectWithValue }) => {
    try {
      const response = await quizApi.completeQuiz(quizId);
      if (!response.success) return rejectWithValue(response.message);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to complete quiz');
    }
  }
);

export const fetchQuizHistory = createAsyncThunk(
  'quiz/fetchHistory',
  async (_, { rejectWithValue }) => {
    try {
      const response = await quizApi.getMyQuizzes();
      return response.data;
    } catch (error: any) {
      return rejectWithValue('Failed to fetch quiz history');
    }
  }
);

const quizSlice = createSlice({
  name: 'quiz',
  initialState,
  reducers: {
    nextQuestion: (state) => {
      state.currentQuestionIndex += 1;
      state.answerResult = null;
    },
    resetQuiz: (state) => {
      state.currentQuiz = null;
      state.currentQuestionIndex = 0;
      state.answerResult = null;
      state.quizCompleted = false;
      state.finalSummary = null;
      state.error = null;
    },
    clearAnswerResult: (state) => {
      state.answerResult = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch categories
    builder
      .addCase(fetchCategories.fulfilled, (state, action) => {
        state.categories = action.payload;
      });

    // Create quiz
    builder
      .addCase(createQuiz.pending, (state) => {
        state.isLoading = true;
        state.error = null;
        state.quizCompleted = false;
        state.finalSummary = null;
      })
      .addCase(createQuiz.fulfilled, (state, action) => {
        state.isLoading = false;
        state.currentQuiz = action.payload;
        state.currentQuestionIndex = 0;
        state.answerResult = null;
      })
      .addCase(createQuiz.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      });

    // Submit answer
    builder
      .addCase(submitAnswer.pending, (state) => {
        state.isSubmitting = true;
      })
      .addCase(submitAnswer.fulfilled, (state, action) => {
        state.isSubmitting = false;
        state.answerResult = action.payload;
      })
      .addCase(submitAnswer.rejected, (state, action) => {
        state.isSubmitting = false;
        state.error = action.payload as string;
      });

    // Complete quiz
    builder
      .addCase(completeQuiz.fulfilled, (state, action) => {
        state.quizCompleted = true;
        state.finalSummary = action.payload;
      });

    // Fetch history
    builder
      .addCase(fetchQuizHistory.fulfilled, (state, action) => {
        state.quizHistory = action.payload;
      });
  },
});

export const { nextQuestion, resetQuiz, clearAnswerResult } = quizSlice.actions;
export default quizSlice.reducer;