<script setup>
import { ref, computed, onMounted } from 'vue';
import { TodoHelper } from '../helpers/todoHelper';

const { todos, loading, error, load, add, remove, toggleDone } = TodoHelper();

const newTodo = ref('');
const hideCompleted = ref(false);

async function handleSubmit() {
  const text = newTodo.value.trim();
  if (!text) return;
  newTodo.value = '';
  await add(text);
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

onMounted(load);
</script>

<template>
  <form @submit.prevent="handleSubmit" class="d-flex gap-2 mb-3">
    <input v-model="newTodo" required maxlength="500" placeholder="New todo" :disabled="loading" class="form-control">
    <button :disabled="loading" class="btn btn-primary">Add</button>
  </form>
  <div v-if="loading" class="text-muted mb-2">Loading…</div>
  <div v-if="error" class="alert alert-danger py-2">{{ error }}</div>
  <ul class="list-group mb-3">
    <li class="list-group-item d-flex align-items-center gap-2" v-for="todo in visibleTodos" :key="todo.id">
      <input class="form-check-input m-0" type="checkbox" :id="`todo-${todo.id}`" :checked="todo.done" @change="toggleDone(todo)" />
      <label class="form-check-label flex-grow-1" :for="`todo-${todo.id}`" :class="{ done: todo.done }">
        {{ todo.text }}
      </label>
      <button type="button" class="btn btn-sm btn-outline-danger" @click="remove(todo)">✕</button>
    </li>
  </ul>
  <button type="button" class="btn btn-secondary" @click="hideCompleted = !hideCompleted">
    {{ hideCompleted ? 'Show all' : 'Hide completed' }}
  </button>
</template>

<style>
.done {
  text-decoration: line-through;
}

.form-check-label {
  white-space: normal;
  word-break: break-word;
  overflow-wrap: anywhere;
}
</style>