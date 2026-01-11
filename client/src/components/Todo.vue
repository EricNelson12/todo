<script setup>
import { ref, computed, onMounted } from 'vue';
import { api } from '../api/client';

// --- State ---
const newTodo = ref('');
const hideCompleted = ref(false);
const todos = ref([]);
const loading = ref(false);
const error = ref('');

// --- API client ---
async function loadTodos() {
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

// --- UI actions ---
async function addTodo() {
  const text = newTodo.value.trim();
  if (!text) return;

  error.value = '';

  const optimistic = { id: `tmp-${Date.now()}`, text, done: false, _optimistic: true };
  todos.value.unshift(optimistic);
  newTodo.value = '';

  try {
    const { data } = await api.POST('/api/todos', { body: { text } });
    if (data) {
      const idx = todos.value.findIndex((t) => t.id === optimistic.id);
      if (idx !== -1) todos.value[idx] = data;
    } else {
      await loadTodos();
    }
  } catch (e) {
    todos.value = todos.value.filter((t) => t.id !== optimistic.id);
    error.value = e?.message ?? 'Failed to create todo';
  }
}

async function removeTodo(todo) {
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

async function toggleDone(todo) {
  const prevDone = todo.done;
  todo.done = !todo.done;

  try {
    await api.PUT(`/api/todos/${todo.id}`, { body: { done: todo.done } });
  } catch (e) {
    todo.done = prevDone; // Revert on failure
    error.value = e?.message ?? 'Failed to update todo';
  }
}

const visibleTodos = computed(() => {
  const sortedTodos = [...todos.value].sort((a, b) => {
    if (a.done !== b.done) {
      return a.done - b.done; // Incomplete first
    }
    return new Date(a.createdAt) - new Date(b.createdAt); // Sort by creation date
  });

  return hideCompleted.value ? sortedTodos.filter((t) => !t.done) : sortedTodos;
});

onMounted(loadTodos);
</script>

<template>
  <form @submit.prevent="addTodo">
    <input v-model="newTodo" required placeholder="new todo" :disabled="loading">
    <button :disabled="loading">Add Todo</button>
  </form>
  <p v-if="loading">Loading…</p>
  <p v-if="error" class="error">{{ error }}</p>
  <ul>
    <li v-for="todo in visibleTodos" :key="todo.id">
      <input type="checkbox" :checked="todo.done" @change="toggleDone(todo)" />
      <span :class="{ done: todo.done }">{{ todo.text }}</span>
      <button @click="removeTodo(todo)">X</button>
    </li>
  </ul>
  <button @click="hideCompleted = !hideCompleted">
    {{ hideCompleted ? 'Show all' : 'Hide completed' }}
  </button>
</template>

<style>
.done {
  text-decoration: line-through;
}
.error {
  color: #b00020;
}
</style>