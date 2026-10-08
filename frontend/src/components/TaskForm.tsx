import React, { useState } from 'react';

interface TaskFormProps {
  onAddTask: (title: string, description: string) => Promise<void>;
  isLoading: boolean;
}

export const TaskForm: React.FC<TaskFormProps> = ({ onAddTask, isLoading }) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [validationError, setValidationError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    const trimmedTitle = title.trim();
    if (!trimmedTitle) {
      setValidationError('Task title is required.');
      return;
    }
    if (trimmedTitle.length > 100) {
      setValidationError('Title cannot exceed 100 characters.');
      return;
    }
    if (description.trim().length > 500) {
      setValidationError('Description cannot exceed 500 characters.');
      return;
    }

    try {
      await onAddTask(trimmedTitle, description.trim());
      setTitle('');
      setDescription('');
    } catch {
      // Error handled by parent component state
    }
  };

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      {validationError && <div className="form-error">{validationError}</div>}
      <div className="form-group">
        <input
          type="text"
          placeholder="Task title"
          value={title}
          maxLength={100}
          disabled={isLoading}
          onChange={(e) => setTitle(e.target.value)}
        />
      </div>
      <div className="form-group">
        <textarea
          placeholder="Description (optional)"
          value={description}
          maxLength={500}
          rows={3}
          disabled={isLoading}
          onChange={(e) => setDescription(e.target.value)}
        />
      </div>
      <button type="submit" disabled={isLoading || !title.trim()}>
        {isLoading ? 'Adding...' : 'Add Task'}
      </button>
    </form>
  );
};
