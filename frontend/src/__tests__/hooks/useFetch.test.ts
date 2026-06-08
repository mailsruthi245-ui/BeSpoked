import { renderHook, waitFor } from '@testing-library/react';
import { useFetch } from '../../hooks/useFetch';

test('starts in loading state', () => {
  const fetchFn = jest.fn().mockResolvedValue([]);
  const { result } = renderHook(() => useFetch(fetchFn));
  expect(result.current.loading).toBe(true);
});

test('returns data on success', async () => {
  const fetchFn = jest.fn().mockResolvedValue([{ id: 1 }]);
  const { result } = renderHook(() => useFetch(fetchFn));
  await waitFor(() => expect(result.current.loading).toBe(false));
  expect(result.current.data).toEqual([{ id: 1 }]);
  expect(result.current.error).toBe('');
});

test('sets error message on failure', async () => {
  const fetchFn = jest.fn().mockRejectedValue(new Error('Server error'));
  const { result } = renderHook(() => useFetch(fetchFn));
  await waitFor(() => expect(result.current.loading).toBe(false));
  expect(result.current.error).toBe('Server error');
  expect(result.current.data).toBeNull();
});
