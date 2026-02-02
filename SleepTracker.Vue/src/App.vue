<script setup lang="ts">
import { ref, onMounted } from 'vue'
import SleepList from './components/SleepList.vue'

const theme = ref('light')

function toggleTheme() {
  theme.value = theme.value === 'light' ? 'dark' : 'light'
  document.documentElement.setAttribute('data-theme', theme.value)
  localStorage.setItem('theme', theme.value)
}

onMounted(() => {
  const saved = localStorage.getItem('theme')
  if (saved === 'light' || saved === 'dark') {
    theme.value = saved
    document.documentElement.setAttribute('data-theme', saved)
  } else {
    document.documentElement.setAttribute('data-theme', 'light')
  }
})
</script>

<template>
  <div class="min-h-screen flex flex-col">
    <header class="navbar bg-base-200 px-6 py5 shadow-md border-b border-base-300">
      <div class="flex-1">
        <h1 class="text-3xl font-bold tracking-tight">Sleep Tracker</h1>
      </div>
      <div class="flex-none">
        <div class="flex items-center gap-2">
          <span class="text-base-content/70" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
          </span>
          <input
            type="checkbox"
            class="toggle toggle-primary"
            :checked="theme === 'dark'"
            :aria-label="theme === 'light' ? 'Switch to dark mode' : 'Switch to light mode'"
            @change="toggleTheme"
          />
          <span class="text-base-content/70" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
            </svg>
          </span>
        </div>
      </div>
    </header>
    <main class="flex-1">
      <SleepList />
    </main>
  </div>
</template>

<style scoped></style>
