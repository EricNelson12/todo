import { ref } from 'vue';
import { api } from '../api/client';

export function TodoHelper() {
  const todos = ref([]);
  const loading = ref(false);
  const error = ref('');

  async function load() {
    loading.value = true;
    error.value = '';
    try {
      const { data } = await api.GET('/api/todos');
      todos.value = data ?? [];
    } catch (e) {
      error.value = e?.message ?? 'Failed to load todos';
      todos.value = [];
    } finally {
      loading.value = false;
    }
  }

  // Optimistic add: immediately show the new todo, then replace with server response
  async function add(text) {
    if (!text) return;

    error.value = '';

    const optimistic = { id: `tmp-${Date.now()}`, text, done: false };
    todos.value.unshift(optimistic);

    try {
      const { data } = await api.POST('/api/todos', { body: { text } });
      if (data) {
        const idx = todos.value.findIndex((t) => t.id === optimistic.id);
        if (idx !== -1) todos.value[idx] = data;
      } else {
        await load();
      }
    } catch (e) {
      todos.value = todos.value.filter((t) => t.id !== optimistic.id);
      error.value = e?.message ?? 'Failed to create todo';
    }
  }

  // Optimistic remove: hide immediately, restore on failure
  async function remove(todo) {
    error.value = '';

    const prev = todos.value;
    todos.value = todos.value.filter((t) => t.id !== todo.id);

    try {
      await api.DELETE(`/api/todos/${todo.id}`);
    } catch (e) {
      todos.value = prev;
      error.value = e?.message ?? 'Failed to delete todo';
    }
  }

  // Optimistic toggle: flip state immediately, revert on failure
  async function toggleDone(todo) {
    const prevDone = todo.done;
    todo.done = !todo.done;

    try {
      await api.PUT(`/api/todos/${todo.id}`, { body: { done: todo.done } });
    } catch (e) {
      todo.done = prevDone;
      error.value = e?.message ?? 'Failed to update todo';
    }
  }

  return {
    todos,
    loading,
    error,
    load,
    add,
    remove,
    toggleDone,
  };
}
