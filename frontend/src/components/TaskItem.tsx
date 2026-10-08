import React, { useState } from 'react';
import { Task } from '../types/task';

interface TaskItemProps {
  task: Task;
  onToggle: (task: Task) => void;
  onDelete: (id: number) => void;
  onUpdate: (id: number, title: string, description: string) => Promise<void>;
}

export const TaskItem: React.FC<TaskItemProps> = ({ task, onToggle, onDelete, onUpdate }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState(task.title);
  const [editDesc, setEditDesc] = useState(task.description || '');
  const [isSaving, setIsSaving] = useState(false);

  const handleSave = async () => {
    if (!editTitle.trim()) return;
    setIsSaving(true);
    try {
      await onUpdate(task.id, editTitle.trim(), editDesc.trim());
      setIsEditing(false);
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className={`task-item ${task.isCompleted ? 'completed' : ''}`}>
      {isEditing ? (
        <div className="task-edit-block">
          <input
            type="text"
            value={editTitle}
            maxLength={100}
            onChange={(e) => setEditTitle(e.target.value)}
          />
          <textarea
            value={editDesc}
            maxLength={500}
            rows={2}
            onChange={(e) => setEditDesc(e.target.value)}
          />
          <div className="task-actions">
            <button onClick={handleSave} disabled={isSaving}>Save</button>
            <button onClick={() => setIsEditing(false)} disabled={isSaving}>Cancel</button>
          </div>
        </div>
      ) : (
        <div className="task-row">
          <label className="checkbox-wrapper">
            <input
              type="checkbox"
              checked={task.isCompleted}
              onChange={() => onToggle(task)}
            />
            <span className="task-title">{task.title}</span>
          </label>
          {task.description && <p className="task-desc">{task.description}</p>}
          <div className="task-actions">
            <button onClick={() => setIsEditing(true)}>Edit</button>
            <button className="btn-delete" onClick={() => onDelete(task.id)}>Delete</button>
          </div>
        </div>
      )}
    </div>
  );
};
