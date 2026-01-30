<script setup lang="ts">
import {ref, onMounted, onUnmounted} from 'vue';
import {getSleeps, createSleep, updateSleep, deleteSleep} from '../services/sleep';
import type {SleepReadDto, SleepCreateDto, SleepUpdateDto} from '../services/sleep';

const sleeps = ref<SleepReadDto[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

const pageNumber = ref(1);
const pageSize = ref(10);
const totalRecords = ref(0);
const totalPages = ref(0);

const timerRunning = ref(false);
const timerStartTime = ref<Date |null>(null);
const elapsedTime = ref('00:00:00');
let timerInterval: any = null;

const filterStartDate = ref<Date | null>(null);
const filterEndDate = ref<Date | null>(null);

const showCreateModal = ref(false);
const createForm = ref<{start: string; end: string}>({start: '', end: ''});

const showEditModal = ref(false);
const editSleep = ref<SleepReadDto | null>(null);
const editForm = ref<{start: string; end: string}>({start: '', end: ''});

async function loadSleeps() {
  loading.value = true;
  error.value = null;

  try {
    const response = await getSleeps(
      pageNumber.value,
      pageSize.value,
      filterStartDate.value || undefined,
      filterEndDate.value || undefined
    );

    if (response.status === 0 && response.data) {
      sleeps.value = response.data;
      totalRecords.value = response.totalRecords;
      totalPages.value = response.totalPages;
    } else {
      error.value = response.message || 'Failed to load sleep records';
    }
  } catch (err) {
    error.value = 'Failed to load sleep records. Please try again.';
    console.error('Error loading sleeps: ', err);
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadSleeps();
});

onUnmounted(() => {
  if (timerInterval) {
    clearInterval(timerInterval);
  };
})

function onFilterChange() {
  pageNumber.value = 1;
  loadSleeps();
}

function clearFilters() {
  filterStartDate.value = null;
  filterEndDate.value = null;
  pageNumber.value = 1;
  loadSleeps();
}

function startSleepTimer() {
  const startTime = new Date();
  timerStartTime.value = startTime;
  timerRunning.value = true;
  elapsedTime.value = '00:00:00';

  timerInterval = setInterval(() => {
    if (timerStartTime.value) {
      const now = new Date();
      const elapsed = Math.floor((now.getTime() - timerStartTime.value.getTime()) / 1000);
      elapsedTime.value = formatElapsedTime(elapsed);
    }
  }, 1000);
}

function stopSleepTimer() {
  if (!timerStartTime.value) {
    return;
  }

  if (timerInterval) {
    clearInterval(timerInterval);
    timerInterval = null;
  }

  const startTime = timerStartTime.value;
  const endTime = new Date();

  const createDto: SleepCreateDto = {
    start: startTime.toISOString(),
    end: endTime.toISOString()
  };

  createSleep(createDto)
    .then(() => {
      timerRunning.value = false;
      timerStartTime.value = null;
      elapsedTime.value = '00:00:00';
      loadSleeps();
    })
    .catch((err) => {
      console.error('Error creating sleep: ', err);
    });
}

function formatElapsedTime(seconds: number): string {
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = seconds % 60;
  return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
}

function formatDateTime(dateTimeString: string): string {
  const date = new Date(dateTimeString);
  return date.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  });
}

function formatDuration(start: string, end: string): string {
  const startDate = new Date(start);
  const endDate = new Date(end);
  const diffMs = endDate.getTime() - startDate.getTime();
  const diffSeconds = Math.floor(diffMs / 1000);
  const diffMinutes = Math.floor(diffSeconds / 60);
  const hours = Math.floor(diffMinutes / 60);
  const minutes = diffMinutes % 60;
  const seconds = diffSeconds % 60;

  if (diffSeconds < 60) {
    return `${seconds} second${seconds !== 1 ? 's' : ''}`;
  } else if (hours === 0) {
    return `${minutes} minute${minutes !== 1 ? 's' : ''}`;
  } else if (minutes === 0) {
    return `${hours} hour${hours !== 1 ? 's' : ''}`;
  } else {
    return `${hours} hour${hours !== 1 ? 's' : ''} and ${minutes} minute${minutes !== 1 ? 's' : ''}`;
  }
}

function toDateTimeLocal(isoString: string): string {
  const d = new Date(isoString);
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function fromDateTimeLocal(dateTimeLocal: string): string {
  return new Date(dateTimeLocal).toISOString();
}

function openCreateModal() {
  const now = new Date();
  createForm.value = {
    start: toDateTimeLocal(now.toISOString()),
    end: toDateTimeLocal(now.toISOString())
  };
  showCreateModal.value = true;
}

function closeCreateModal() {
  showCreateModal.value = false;
}

async function submitCreate() {
  try {
    await createSleep({
      start: fromDateTimeLocal(createForm.value.start),
      end: fromDateTimeLocal(createForm.value.end)
    });
    closeCreateModal();
    loadSleeps();
  } catch (err) {
    console.error('Error creating sleep: ', err);
  }
}

function openEditModal(sleep: SleepReadDto) {
  editSleep.value = sleep;
  editForm.value = {
    start: toDateTimeLocal(sleep.start),
    end: toDateTimeLocal(sleep.end)
  };
  showEditModal.value = true;
}

function closeEditModal() {
  showEditModal.value = false;
  editSleep.value = null;
}

async function submitEdit() {
  if (!editSleep.value) return;
  try {
    await updateSleep(editSleep.value.id, {
      start: fromDateTimeLocal(editForm.value.start),
      end: fromDateTimeLocal(editForm.value.end)
    });
    closeEditModal();
    loadSleeps();
  } catch (err) {
    console.error('Error updating sleep: ', err);
  }
}

async function deleteSleepRecord(sleep: SleepReadDto) {
  const confirmed = confirm(
    `Are you sure you want to delete this sleep record?\n\n` +
    `Start: ${formatDateTime(sleep.start)}\n` +
    `End: ${formatDateTime(sleep.end)}\n` +
    `Duration: ${formatDuration(sleep.start, sleep.end)}`
  );

  if (!confirmed) {
    return;
  }

  try {
    await deleteSleep(sleep.id);
    loadSleeps();
  } catch (err) {
    console.error('Error deleting sleep: ', err);
    alert('Failed to delete sleep record. Please try again.');
  }
}
</script>

<template>
  <div class="sleep-list-container">
    <div class="header-section">
      <div class="filter-section">
        <div class="filter-fields">
          <label>Sleep Date From</label>
          <input
            type="date"
            :value="filterStartDate ? filterStartDate.toISOString().slice(0, 10) : ''"
            @input="filterStartDate = ($event.target as HTMLInputElement).value ? new Date(($event.target as HTMLInputElement).value) : null; onFilterChange()"
            />
            <label>Sleep Date To</label>
            <input
              type="date"
              :value="filterEndDate ? filterEndDate.toISOString().slice(0, 10) : ''"
              @input="filterEndDate = ($event.target as HTMLInputElement).value ? new Date(($event.target as HTMLInputElement).value) : null; onFilterChange()"
              />
              <button v-if="filterStartDate || filterEndDate" type="button" @click="clearFilters">Clear</button>
        </div>
      </div>
      <div class="header-actions">
        <button v-if="!timerRunning" type="button" @click="startSleepTimer">Start Sleep Timer</button>
        <template v-else>
          <span class="timer-text">{{ elapsedTime }}</span>
          <button type="button" @click="stopSleepTimer">Stop</button>
        </template>
        <button type="button" @click="openCreateModal">Add Sleep Record</button>
      </div>
    </div>

    <div v-if="loading" class="loading-container">
      <p>Loading sleep records...</p>
    </div>

    <div v-if="error" class="error-container">
      <p>{{ error }}</p>
    </div>

    <div v-if="!loading && !error && sleeps.length > 0" class="table-container">
      <table class="sleep-table">
        <thead>
          <tr>
            <th>Sleep Date/Time</th>
            <th>Wake Date/Time</th>
            <th>Duration</th>
            <th class="actions-column">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sleep in sleeps" :key="sleep.id">
            <td>{{ formatDateTime(sleep.start) }}</td>
            <td>{{ formatDateTime(sleep.end) }}</td>
            <td>{{ formatDuration(sleep.start, sleep.end) }}</td>
            <td class="actions-column">
              <button type="button" @click="openEditModal(sleep)" aria-label="Edit">Edit</button>
              <button type="button" @click="deleteSleepRecord(sleep)" aria-label="Delete">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.table-container {
  overflow-x: auto;
  margin-bottom: 1rem;
}

.sleep-table {
  width: 100%;
  border-collapse: collapse;
}

.sleep-table th,
.sleep-table td {
  padding: 0.5rem;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.sleep-table thead {
  background-color: #f3f4f6;
}

.actions-column {
  white-space: nowrap;
}

.header-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1rem;
}

.filter-fields {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  align-items: center;
}

.header-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  align-items: center;
}

.timer-text {
  font-family: monospace;
  font-weight: 500;
}

.sleep-list-container {
  padding: 1rem;
}

.loading-container,
.error-container {
  padding: 2rem;
  text-align: center;
}

.error-container {
  color: #b91c1c;
}
</style>