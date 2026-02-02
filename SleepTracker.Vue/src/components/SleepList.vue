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

function goToPreviousPage() {
  if (pageNumber.value <= 1) return;
  pageNumber.value--;
  loadSleeps();
}

function goToNextPage() {
  if (pageNumber.value >= totalPages.value) return;
  pageNumber.value++;
  loadSleeps();
}

function goToFirstPage() {
  if (pageNumber.value <= 1) return;
  pageNumber.value = 1;
  loadSleeps();
}

function goToLastPage() {
  if (pageNumber.value >= totalPages.value) return;
  pageNumber.value = totalPages.value;
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
            @change="filterStartDate = ($event.target as HTMLInputElement).value ? new Date(($event.target as HTMLInputElement).value) : null; onFilterChange()"
            />
            <label>Sleep Date To</label>
            <input
              type="date"
              :value="filterEndDate ? filterEndDate.toISOString().slice(0, 10) : ''"
              @change="filterEndDate = ($event.target as HTMLInputElement).value ? new Date(($event.target as HTMLInputElement).value) : null; onFilterChange()"
              />
              <button v-if="filterStartDate || filterEndDate" type="button" @click="clearFilters" class="btn btn-ghost btn-sm">Clear</button>
        </div>
      </div>
      <div class="header-actions">
        <button v-if="!timerRunning" type="button" @click="startSleepTimer" class="btn btn-primary">Start Sleep Timer</button>
        <template v-else>
          <span class="timer-text">{{ elapsedTime }}</span>
          <button type="button" @click="stopSleepTimer" class="btn btn-error">Stop</button>
        </template>
        <button type="button" @click="openCreateModal" class="btn btn-primary">Add Sleep Record</button>
      </div>
    </div>

    <div v-if="loading" class="loading-container">
      <p>Loading sleep records...</p>
    </div>

    <div v-if="error" class="error-container">
      <p>{{ error }}</p>
    </div>

    <div v-if="!loading && !error && sleeps.length > 0" class="table-container">
      <table class="table table-zebra">
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
              <button type="button" @click="openEditModal(sleep)" aria-label="Edit" class="btn btn-sm btn-ghost">Edit</button>
              <button type="button" @click="deleteSleepRecord(sleep)" aria-label="Delete" class="btn btn-sm btn-error">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="!loading && !error && sleeps.length > 0" class="pagination-info">
      <p>Showing {{ sleeps.length }} of {{ totalRecords }} records (Page {{ pageNumber }} of {{ totalPages }})</p>
      <div v-if="totalPages > 1" class="pagination-buttons">
        <button
          type="button"
          :disabled="pageNumber <= 1"
          @click="goToFirstPage"
        >
        First
        </button>
        <button
          type="button"
          :disabled="pageNumber <= 1"
          @click="goToPreviousPage"
        >
        Previous  
        </button>
        <button
          type="button"
          :disabled="pageNumber >= totalPages"
          @click="goToNextPage"
        >
        Next
        </button>
        <button
          type="button"
          :disabled="pageNumber >= totalPages"
          @click="goToLastPage"
        >
        Last
        </button>
      </div>
    </div>

    <div v-if="!loading && !error && sleeps.length === 0" class="empty-state">
      <p>No sleep records found.</p>
    </div>

    <div class="modal" :class="{ 'modal-open': showCreateModal }">
      <div class="modal-box">
        <h3 class="font-bold text-lg">Add Sleep Record</h3>
        <div class="form-control w-full gap-2 mt-4">
          <label for="create-start" class="label">
            <span class="label-text">Start</span>
          </label>
          <input
            id="create-start"
            v-model="createForm.start"
            type="datetime-local"
            class="input input-bordered w-full"
          />
          <label for="create-end" class="label">
            <span class="label-text">End</span>
          </label>
          <input
            id="create-end"
            v-model="createForm.end"
            type="datetime-local"
            class="input input-bordered w-full"
          />
        </div>
        <div class="modal-action">
          <button type="button" class="btn btn-ghost" @click="closeCreateModal">Cancel</button>
          <button type="button" class="btn btn-primary" @click="submitCreate">Save</button>
        </div>
      </div>
      <form method="dialog" class="modal-backdrop" @submit="closeCreateModal">
        <button type="submit">close</button>
      </form>
    </div>

    <div class="modal" :class="{ 'modal-open': showEditModal }">
      <div class="modal-box">
        <h3 class="font-bold text-lg">Edit Sleep Record</h3>
        <div class="form-control w-full gap-2 mt-4">
          <label for="edit-start" class="label">
            <span class="label-text">Start</span>
          </label>
          <input
            id="edit-start"
            v-model="editForm.start"
            type="datetime-local"
            class="input input-bordered w-full"
          />
          <label for="edit-end" class="label">
            <span class="label-text">End</span>
          </label>
          <input
            id="edit-end"
            v-model="editForm.end"
            type="datetime-local"
            class="input input-bordered w-full"
          />
        </div>
        <div class="modal-action">
          <button type="button" class="btn btn-ghost" @click="closeEditModal">Cancel</button>
          <button type="button" class="btn btn-primary" @click="submitEdit">Save</button>
        </div>
      </div>
      <form method="dialog" class="modal-backdrop" @submit="closeEditModal">
        <button type="submit">close</button>
      </form>
    </div>

  </div>
</template>

<style scoped>
.modal-form {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin: 1rem 0;
}

.modal-form label {
  font-weight: 500;
}

.pagination-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
  margin-top: 0.5rem;
}

.pagination-info {
  margin-top: 1rem;
  text-align: center;
  font-size: 0.875rem;
  color: #6b7280;
}

.empty-state {
  padding: 2rem;
  text-align: center;
  color: #6b7280;
}

.table-container {
  overflow-x: auto;
  margin-bottom: 1rem;
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