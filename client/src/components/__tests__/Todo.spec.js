import { describe, it, expect, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import Todo from '../Todo.vue'
import { api } from '../../api/client'

vi.mock('../../api/client.ts', () => ({
  api: {
    GET: vi.fn(),
    POST: vi.fn(),
    PUT: vi.fn(),
    DELETE: vi.fn(),
  },
}))

describe('Todo', () => {
  it('calls /api/todos on mount', async () => {
    api.GET.mockResolvedValue({ data: [] })

    mount(Todo)
    await flushPromises()

    expect(api.GET).toHaveBeenCalledWith('/api/todos')
  })
})
