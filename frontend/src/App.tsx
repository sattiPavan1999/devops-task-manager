import { useEffect, useState, useCallback } from 'react';
import { Task } from './types/task';
import { taskService } from './services/taskService';
import { TaskForm } from './components/TaskForm';
import { TaskList } from './components/TaskList';
import './App.css';

export const App = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [formSubmitting, setFormSubmitting] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const fetchTasks = useCallback(async () => {
    setLoading(true);
    setErrorMessage(null);
    try {
      const data = await taskService.getAll();
      setTasks(data);
    } catch {
      setErrorMessage('Unable to connect to the backend API. Ensure the service is running.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  const handleAddTask = async (title: string, description: string) => {
    setFormSubmitting(true);
    setErrorMessage(null);
    try {
      const newTask = await taskService.create({ title, description });
      setTasks((prev) => [newTask, ...prev]);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error adding task';
      setErrorMessage(msg);
      throw err;
    } finally {
      setFormSubmitting(false);
    }
  };

  const handleToggleTask = async (task: Task) => {
    setErrorMessage(null);
    try {
      const updated = await taskService.update(task.id, {
        title: task.title,
        description: task.description || undefined,
        isCompleted: !task.isCompleted
      });
      setTasks((prev) => prev.map((t) => (t.id === task.id ? updated : t)));
    } catch {
      setErrorMessage('Failed to update task status.');
    }
  };

  const handleUpdateTask = async (id: number, title: string, description: string) => {
    setErrorMessage(null);
    const existing = tasks.find((t) => t.id === id);
    if (!existing) return;

    try {
      const updated = await taskService.update(id, {
        title,
        description,
        isCompleted: existing.isCompleted
      });
      setTasks((prev) => prev.map((t) => (t.id === id ? updated : t)));
    } catch {
      setErrorMessage('Failed to update task.');
    }
  };

  const handleDeleteTask = async (id: number) => {
    setErrorMessage(null);
    try {
      await taskService.delete(id);
      setTasks((prev) => prev.filter((t) => t.id !== id));
    } catch {
      setErrorMessage('Failed to delete task.');
    }
  };

  return (
    <div className="app-container">
      <header className="app-header">
        <h1>Task Manager</h1>
      </header>

      {errorMessage && (
        <div className="alert-banner">
          {errorMessage}
          <button onClick={fetchTasks}>Retry</button>
        </div>
      )}

      <main>
        <TaskForm onAddTask={handleAddTask} isLoading={formSubmitting} />

        <hr className="divider" />

        <h2>Tasks</h2>
        {loading ? (
          <div className="loading-state">Loading tasks...</div>
        ) : (
          <TaskList
            tasks={tasks}
            onToggle={handleToggleTask}
            onDelete={handleDeleteTask}
            onUpdate={handleUpdateTask}
          />
        )}
      </main>
    </div>
  );
};

export default App;
