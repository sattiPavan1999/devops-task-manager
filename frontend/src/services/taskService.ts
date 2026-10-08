import { Task, CreateTaskPayload, UpdateTaskPayload } from '../types/task';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';
const TASKS_ENDPOINT = API_BASE_URL ? `${API_BASE_URL.replace(/\/$/, '')}/api/tasks` : '/api/tasks';

export const taskService = {
  async getAll(): Promise<Task[]> {
    const response = await fetch(TASKS_ENDPOINT);
    if (!response.ok) {
      throw new Error(`Failed to load tasks (${response.status})`);
    }
    return response.json();
  },

  async getById(id: number): Promise<Task> {
    const response = await fetch(`${TASKS_ENDPOINT}/${id}`);
    if (!response.ok) {
      throw new Error(`Task ${id} not found (${response.status})`);
    }
    return response.json();
  },

  async create(payload: CreateTaskPayload): Promise<Task> {
    const response = await fetch(TASKS_ENDPOINT, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      const err = await response.json().catch(() => ({}));
      throw new Error(err.message || `Failed to create task (${response.status})`);
    }
    return response.json();
  },

  async update(id: number, payload: UpdateTaskPayload): Promise<Task> {
    const response = await fetch(`${TASKS_ENDPOINT}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      const err = await response.json().catch(() => ({}));
      throw new Error(err.message || `Failed to update task (${response.status})`);
    }
    return response.json();
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${TASKS_ENDPOINT}/${id}`, {
      method: 'DELETE'
    });
    if (!response.ok) {
      throw new Error(`Failed to delete task (${response.status})`);
    }
  }
};
