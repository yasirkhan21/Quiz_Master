import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import type { AppDispatch, RootState } from '../store/store';
import { fetchDashboardStats } from '../feature/stats/statsSlice';
import { fetchQuizHistory } from '../feature/quiz/quizSlice';
import Navbar from '../components/common/Navbar';
import Button from '../components/common/Button';
import Loader from '../components/common/Loader';
import {
    Trophy,
    Target,
    BookOpen,
    TrendingUp,
    Plus,
    Clock,
} from 'lucide-react';
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer,
    Cell,
} from 'recharts';

const DashboardPage = () => {
    const dispatch = useDispatch<AppDispatch>();
    const navigate = useNavigate();
    const { dashboard, isLoading } = useSelector((state: RootState) => state.stats);
    const { quizHistory } = useSelector((state: RootState) => state.quiz);
    const { user } = useSelector((state: RootState) => state.auth);

    useEffect(() => {
        dispatch(fetchDashboardStats());
        dispatch(fetchQuizHistory());
    }, [dispatch]);

    const statCards = [
        {
            label: 'Quizzes Taken',
            value: dashboard?.totalQuizzesTaken ?? 0,
            icon: BookOpen,
            color: 'text-violet-400',
            bg: 'bg-violet-500/10',
        },
        {
            label: 'Avg Accuracy',
            value: `${Number(dashboard?.averageAccuracy ?? 0).toFixed(1)}%`,
            icon: Target,
            color: 'text-cyan-400',
            bg: 'bg-cyan-500/10',
        },
        {
            label: 'Total Score',
            value: dashboard?.totalScore ?? 0,
            icon: Trophy,
            color: 'text-yellow-400',
            bg: 'bg-yellow-500/10',
        },
        {
            label: 'Avg Time/Q',
            value: `${Number(dashboard?.averageTimePerQuestion ?? 0).toFixed(1)}s`,
            icon: Clock,
            color: 'text-green-400',
            bg: 'bg-green-500/10',
        },
    ];

    // Chart data from quiz history
    const chartData = quizHistory.slice(0, 7).reverse().map((q, i) => ({
        name: `Quiz ${i + 1}`,
        accuracy: Number(q.accuracy),
        score: q.totalScore,
    }));

    return (
        <div className="min-h-screen">
            <Navbar />

            <div className="max-w-6xl mx-auto px-4 py-8">
                {/* Welcome */}
                <div className="flex items-center justify-between mb-8">
                    <div>
                        <h1 className="text-2xl font-bold text-white">
                            Welcome back, <span className="gradient-text">{user?.username}</span>!
                        </h1>
                        <p className="text-gray-400 text-sm mt-1">
                            Ready to challenge your knowledge?
                        </p>
                    </div>
                    <Button onClick={() => navigate('/quiz/setup')}>
                        <Plus className="w-4 h-4" />
                        New Quiz
                    </Button>
                </div>

                {isLoading ? (
                    <Loader text="Loading your stats..." />
                ) : (
                    <>
                        {/* Stat Cards */}
                        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                            {statCards.map(({ label, value, icon: Icon, color, bg }) => (
                                <div key={label} className="glass rounded-2xl p-5">
                                    <div className={`inline-flex p-2 rounded-xl ${bg} mb-3`}>
                                        <Icon className={`w-5 h-5 ${color}`} />
                                    </div>
                                    <p className="text-2xl font-bold text-white">{value}</p>
                                    <p className="text-xs text-gray-500 mt-1">{label}</p>
                                </div>
                            ))}
                        </div>

                        {/* Chart + Insights Row */}
                        <div className="grid lg:grid-cols-3 gap-6 mb-8">
                            {/* Chart */}
                            <div className="glass rounded-2xl p-6 lg:col-span-2">
                                <h2 className="text-base font-semibold text-white mb-4 flex items-center gap-2">
                                    <TrendingUp className="w-4 h-4 text-violet-400" />
                                    Recent Performance
                                </h2>
                                {chartData.length > 0 ? (
                                    <ResponsiveContainer width="100%" height={200}>
                                        <BarChart data={chartData}>
                                            <XAxis
                                                dataKey="name"
                                                tick={{ fill: '#6b7280', fontSize: 12 }}
                                                axisLine={false}
                                                tickLine={false}
                                            />
                                            <YAxis
                                                tick={{ fill: '#6b7280', fontSize: 12 }}
                                                axisLine={false}
                                                tickLine={false}
                                                domain={[0, 100]}
                                            />
                                            <Tooltip
                                                contentStyle={{
                                                    backgroundColor: '#111827',
                                                    border: '1px solid rgba(255,255,255,0.1)',
                                                    borderRadius: '12px',
                                                    color: '#fff',
                                                }}
                                                formatter={(value: number) => [`${value}%`, 'Accuracy']} />
                                            <Bar dataKey="accuracy" radius={[6, 6, 0, 0]}>
                                                {chartData.map((_, index) => (
                                                    <Cell
                                                        key={index}
                                                        fill={`url(#gradient-${index})`}
                                                    />
                                                ))}
                                            </Bar>
                                            <defs>
                                                {chartData.map((_, index) => (
                                                    <linearGradient
                                                        key={index}
                                                        id={`gradient-${index}`}
                                                        x1="0"
                                                        y1="0"
                                                        x2="0"
                                                        y2="1"
                                                    >
                                                        <stop offset="0%" stopColor="#7c3aed" />
                                                        <stop offset="100%" stopColor="#0891b2" />
                                                    </linearGradient>
                                                ))}
                                            </defs>
                                        </BarChart>
                                    </ResponsiveContainer>
                                ) : (
                                    <div className="flex items-center justify-center h-48 text-gray-500">
                                        No quiz data yet. Take your first quiz!
                                    </div>
                                )}
                            </div>

                            {/* Insights */}
                            <div className="glass rounded-2xl p-6">
                                <h2 className="text-base font-semibold text-white mb-4">
                                    Insights
                                </h2>
                                <div className="flex flex-col gap-4">
                                    <div className="p-4 rounded-xl bg-green-500/10 border border-green-500/20">
                                        <p className="text-xs text-gray-400 mb-1">Best Category</p>
                                        <p className="text-sm font-semibold text-green-400">
                                            {dashboard?.bestCategory || 'N/A'}
                                        </p>
                                    </div>
                                    <div className="p-4 rounded-xl bg-red-500/10 border border-red-500/20">
                                        <p className="text-xs text-gray-400 mb-1">Needs Work</p>
                                        <p className="text-sm font-semibold text-red-400">
                                            {dashboard?.weakestCategory || 'N/A'}
                                        </p>
                                    </div>
                                    <div className="p-4 rounded-xl bg-violet-500/10 border border-violet-500/20">
                                        <p className="text-xs text-gray-400 mb-1">Questions Answered</p>
                                        <p className="text-sm font-semibold text-violet-400">
                                            {dashboard?.totalQuestionsAnswered ?? 0} total
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Recent Quizzes */}
                        <div className="glass rounded-2xl p-6">
                            <h2 className="text-base font-semibold text-white mb-4">
                                Recent Quizzes
                            </h2>
                            {quizHistory.length > 0 ? (
                                <div className="overflow-x-auto">
                                    <table className="w-full text-sm">
                                        <thead>
                                            <tr className="text-gray-500 border-b border-white/10">
                                                <th className="text-left pb-3 font-medium">Category</th>
                                                <th className="text-left pb-3 font-medium">Difficulty</th>
                                                <th className="text-left pb-3 font-medium">Score</th>
                                                <th className="text-left pb-3 font-medium">Accuracy</th>
                                                <th className="text-left pb-3 font-medium">Questions</th>
                                                <th className="text-left pb-3 font-medium">Date</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {quizHistory.map((quiz) => (
                                                <tr
                                                    key={quiz.quizId}
                                                    className="border-b border-white/5 hover:bg-white/5 transition-colors"
                                                >
                                                    <td className="py-3 text-white font-medium">
                                                        {quiz.category}
                                                    </td>
                                                    <td className="py-3">
                                                        <span className={`px-2 py-1 rounded-full text-xs font-semibold capitalize ${quiz.difficulty === 'easy'
                                                                ? 'bg-green-500/20 text-green-400'
                                                                : quiz.difficulty === 'medium'
                                                                    ? 'bg-yellow-500/20 text-yellow-400'
                                                                    : 'bg-red-500/20 text-red-400'
                                                            }`}>
                                                            {quiz.difficulty}
                                                        </span>
                                                    </td>
                                                    <td className="py-3 text-violet-400 font-bold">
                                                        {quiz.totalScore}
                                                    </td>
                                                    <td className="py-3 text-white">
                                                        {Number(quiz.accuracy).toFixed(1)}%
                                                    </td>
                                                    <td className="py-3 text-gray-400">
                                                        {quiz.correctAnswers}/{quiz.totalQuestions}
                                                    </td>
                                                    <td className="py-3 text-gray-500">
                                                        {new Date(quiz.timestamp).toLocaleDateString()}
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            ) : (
                                <div className="text-center py-12">
                                    <Trophy className="w-12 h-12 text-gray-700 mx-auto mb-3" />
                                    <p className="text-gray-500">No quizzes yet.</p>
                                    <Button
                                        variant="secondary"
                                        className="mt-4"
                                        onClick={() => navigate('/quiz/setup')}
                                    >
                                        Take your first quiz
                                    </Button>
                                </div>
                            )}
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};

export default DashboardPage;