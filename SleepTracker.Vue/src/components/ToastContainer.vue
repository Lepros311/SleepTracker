<script setup lang="ts">
import { useToast } from '../composables/useToast';

const { toasts, dismiss } = useToast();
</script>

<template>
  <div class="toast toast-end toast-top z-[9999] flex flex-col gap-2 p-4">
    <div
      v-for="t in toasts"
      :key="t.id"
      :class="[
        'alert shadow-lg',
        t.type === 'success' ? 'alert-success' : 'alert-error'
      ]"
      role="alert"
    >
      <span>{{ t.message }}</span>
      <button
        type="button"
        class="btn btn-ghost btn-xs"
        aria-label="Dismiss"
        @click="dismiss(t.id)"
      >
        ✕
      </button>
    </div>
  </div>
</template>