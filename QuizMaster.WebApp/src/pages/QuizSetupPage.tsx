import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import type { AppDispatch, RootState } from '../store/store';
import { createQuiz, fetchCategories } from '../feature/quiz/quizSlice';
import Button from '../components/common/Button';
import Navbar from '../components/common/Navbar';
import { Settings, BookOpen, Zap, Hash } from 'lucide-react';
import toast from 'react-hot-toast';

const difficulties = ['easy', 'medium', 'hard'];
const questionCounts = [5, 10, 15, 20];

const QuizSetupPage = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const { categories, isLoading, currentQuiz, error } = useSelector(
    (state: RootState) => state.quiz
  );

  const [category, setCategory] = useState('');
  const [difficulty, setDifficulty] = useState('medium');
  const [count, setCount] = useState(10);

  useEffect(() => {
    dispatch(fetchCategories());
  }, [dispatch]);

  useEffect(() => {
    if (currentQuiz) navigate('/quiz/play');
  }, [currentQuiz, navigate]);

  useEffect(() => {
    if (error) toast.error(error);
  }, [error]);

  const handleStart = () => {
    if (!category) {
      toast.error('Please select a category');
      return;
    }
    dispatch(createQuiz({ category, difficulty, numberOfQuestions: count }));
  };

  const difficultyColors: Record<string, string> = {
    easy: 'border-green-500 bg-green-500/10 text-green-400',
    medium: 'border-yellow-500 bg-yellow-500/10 text-yellow-400',
    hard: 'border-red-500 bg-red-500/10 text-red-400',
  };

  return (
    <div className="min-h-screen">
      <Navbar />
      <div className="max-w-2xl mx-auto px-4 py-12">
        <div className="mb-8 text-center">
          <h1 className="text-3xl font-bold text-white mb-2">Setup Your Quiz</h1>
          <p className="text-gray-400">Customize your quiz experience</p>
        </div>

        <div className="glass rounded-2xl p-8 flex flex-col gap-8">
          {/* Category */}
          <div>
            <label className="flex items-center gap-2 text-sm font-medium text-gray-300 mb-3">
              <BookOpen className="w-4 h-4 text-violet-400" />
              Category
            </label>
            <select
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="w-full px-4 py-3 rounded-xl bg-white/5 border border-white/10 text-white focus:outline-none focus:border-violet-500 transition-all"
            >
              <option value="" className="bg-gray-900">Select a category</option>
              {categories.map((cat) => (
                <option key={cat} value={cat} className="bg-gray-900">
                  {cat}
                </option>
              ))}
            </select>
          </div>

          {/* Difficulty */}
          <div>
            <label className="flex items-center gap-2 text-sm font-medium text-gray-300 mb-3">
              <Zap className="w-4 h-4 text-violet-400" />
              Difficulty
            </label>
            <div className="grid grid-cols-3 gap-3">
              {difficulties.map((d) => (
                <button
                  key={d}
                  onClick={() => setDifficulty(d)}
                  className={`py-3 rounded-xl border-2 font-semibold capitalize text-sm transition-all ${
                    difficulty === d
                      ? difficultyColors[d]
                      : 'border-white/10 text-gray-400 hover:border-white/20'
                  }`}
                >
                  {d}
                </button>
              ))}
            </div>
          </div>

          {/* Question Count */}
          <div>
            <label className="flex items-center gap-2 text-sm font-medium text-gray-300 mb-3">
              <Hash className="w-4 h-4 text-violet-400" />
              Number of Questions
            </label>
            <div className="grid grid-cols-4 gap-3">
              {questionCounts.map((n) => (
                <button
                  key={n}
                  onClick={() => setCount(n)}
                  className={`py-3 rounded-xl border-2 font-semibold text-sm transition-all ${
                    count === n
                      ? 'border-violet-500 bg-violet-500/10 text-violet-400'
                      : 'border-white/10 text-gray-400 hover:border-white/20'
                  }`}
                >
                  {n}
                </button>
              ))}
            </div>
          </div>

          <Button
            fullWidth
            isLoading={isLoading}
            onClick={handleStart}
            className="mt-2"
          >
            <Settings className="w-4 h-4" />
            Start Quiz
          </Button>
        </div>
      </div>
    </div>
  );
};

export default QuizSetupPage;