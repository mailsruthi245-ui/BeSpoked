import { renderHook, act } from '@testing-library/react';
import { useEditState } from '../../hooks/useEditState';

interface Item { id: number; name: string; }

test('setEditing sets the editing value', () => {
  const { result } = renderHook(() => useEditState<Item>());
  act(() => result.current.setEditing({ id: 1, name: 'Bike' }));
  expect(result.current.editing).toEqual({ id: 1, name: 'Bike' });
});

test('set updates a single key on the editing object', () => {
  const { result } = renderHook(() => useEditState<Item>());
  act(() => result.current.setEditing({ id: 1, name: 'Bike' }));
  act(() => result.current.set('name', 'Road Bike'));
  expect(result.current.editing).toEqual({ id: 1, name: 'Road Bike' });
});

test('set does nothing when editing is null', () => {
  const { result } = renderHook(() => useEditState<Item>());
  act(() => result.current.set('name', 'Road Bike'));
  expect(result.current.editing).toBeNull();
});
