import { ref } from 'vue';

export type ToastType = 'success' | 'error';

interface Toast {
  id: number;
  message: string;
  type: ToastType;
}

const toasts = ref<Toast[]>([]);
let nextId = 0;
const DEFAULT_DURATION_MS = 4000;

export function useToast() {
  function show(message: string, type: ToastType = 'success') {
    const id = nextId++;
    toasts.value = [...toasts.value, { id, message, type }];
    setTimeout(() => {
      toasts.value = toasts.value.filter((t) => t.id !== id);
    }, DEFAULT_DURATION_MS);
  }

  function dismiss(id: number) {
    toasts.value = toasts.value.filter((t) => t.id !== id);
  }

  return { toasts, show, dismiss };
}